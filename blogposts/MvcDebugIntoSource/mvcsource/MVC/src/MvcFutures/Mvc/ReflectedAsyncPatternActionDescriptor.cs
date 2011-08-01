namespace Microsoft.Web.Mvc {
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;
    using System.Threading;
    using System.Web.Mvc;
    using Microsoft.Web.Resources;

    public class ReflectedAsyncPatternActionDescriptor : AsyncActionDescriptor {

        private readonly string _actionName;
        private readonly ControllerDescriptor _controllerDescriptor;
        private ParameterDescriptor[] _parametersCache;

        private static readonly object _executeTag = new object();

        public ReflectedAsyncPatternActionDescriptor(MethodInfo beginMethod, MethodInfo endMethod, string actionName, ControllerDescriptor controllerDescriptor)
            : this(beginMethod, endMethod, actionName, controllerDescriptor, true /* validateMethods */) {
        }

        internal ReflectedAsyncPatternActionDescriptor(MethodInfo beginMethod, MethodInfo endMethod, string actionName, ControllerDescriptor controllerDescriptor, bool validateMethods) {
            if (validateMethods) {
                ValidateBeginMethod(beginMethod);
                ValidateEndMethod(endMethod);
            }
            if (String.IsNullOrEmpty(actionName)) {
                throw new ArgumentException(MvcResources.Common_NullOrEmpty, "actionName");
            }
            if (controllerDescriptor == null) {
                throw new ArgumentNullException("controllerDescriptor");
            }

            BeginMethod = beginMethod;
            EndMethod = endMethod;
            _actionName = actionName;
            _controllerDescriptor = controllerDescriptor;
        }

        public override string ActionName {
            get {
                return _actionName;
            }
        }

        public MethodInfo BeginMethod {
            get;
            private set;
        }

        public override ControllerDescriptor ControllerDescriptor {
            get {
                return _controllerDescriptor;
            }
        }

        public MethodInfo EndMethod {
            get;
            private set;
        }

        public override IAsyncResult BeginExecute(ControllerContext controllerContext, IDictionary<string, object> parameters, AsyncCallback callback, object state) {
            if (controllerContext == null) {
                throw new ArgumentNullException("controllerContext");
            }
            if (parameters == null) {
                throw new ArgumentNullException("parameters");
            }

            ParameterInfo[] parameterInfos = BeginMethod.GetParameters();
            IEnumerable<object> rawParameterValues = from parameterInfo in parameterInfos
                                                     select ExtractParameterFromDictionary(parameterInfo, parameters, BeginMethod);
            List<object> parametersList = rawParameterValues.Take(parameterInfos.Length - 2).ToList();

            MethodDispatcher beginDispatcher = DispatcherCache.GetDispatcher(BeginMethod);
            MethodDispatcher endDispatcher = DispatcherCache.GetDispatcher(EndMethod);

            // need to add callback + state object to list
            BeginInvokeCallback beginCallback = (innerCallback, innerState) => {
                parametersList.Add(innerCallback);
                parametersList.Add(innerState);
                object[] parametersArray = parametersList.ToArray();

                IAsyncResult innerAsyncResult = (IAsyncResult)beginDispatcher.Execute(controllerContext.Controller, parametersArray);
                return innerAsyncResult;
            };

            AsyncCallback<object> endCallback = ar => {
                object actionReturnValue = endDispatcher.Execute(controllerContext.Controller, new object[] { ar });
                return actionReturnValue;
            };

            // Set the timeout and go
            IAsyncManagerContainer helperContainer = controllerContext.Controller as IAsyncManagerContainer;
            int timeout = (helperContainer != null) ? helperContainer.AsyncManager.Timeout : Timeout.Infinite;
            return AsyncResultWrapper.WrapWithTimeout(callback, state, beginCallback, endCallback, timeout, _executeTag);
        }

        public override object EndExecute(IAsyncResult asyncResult) {
            return AsyncResultWrapper.UnwrapAndContinue<object>(asyncResult, _executeTag);
        }

        public override object[] GetCustomAttributes(bool inherit) {
            return BeginMethod.GetCustomAttributes(inherit);
        }

        public override object[] GetCustomAttributes(Type attributeType, bool inherit) {
            return BeginMethod.GetCustomAttributes(attributeType, inherit);
        }

        public override FilterInfo GetFilters() {
            // For simplicity, we only consider filters on BeginFoo() since it is the entry point to the action

            // Enumerable.OrderBy() is a stable sort, so this method preserves scope ordering.
            FilterAttribute[] typeFilters = (FilterAttribute[])BeginMethod.ReflectedType.GetCustomAttributes(typeof(FilterAttribute), true /* inherit */);
            FilterAttribute[] methodFilters = (FilterAttribute[])BeginMethod.GetCustomAttributes(typeof(FilterAttribute), true /* inherit */);
            List<FilterAttribute> orderedFilters = typeFilters.Concat(methodFilters).OrderBy(attr => attr.Order).ToList();

            FilterInfo filterInfo = new FilterInfo();
            MergeFiltersIntoList(orderedFilters, filterInfo.ActionFilters);
            MergeFiltersIntoList(orderedFilters, filterInfo.AuthorizationFilters);
            MergeFiltersIntoList(orderedFilters, filterInfo.ExceptionFilters);
            MergeFiltersIntoList(orderedFilters, filterInfo.ResultFilters);
            return filterInfo;
        }

        public override ParameterDescriptor[] GetParameters() {
            ParameterDescriptor[] parameters = LazilyFetchParametersCollection();

            // need to clone array so that user modifications aren't accidentally stored
            return (ParameterDescriptor[])parameters.Clone();
        }

        public override ICollection<ActionSelector> GetSelectors() {
            ActionMethodSelectorAttribute[] attrs = (ActionMethodSelectorAttribute[])BeginMethod.GetCustomAttributes(typeof(ActionMethodSelectorAttribute), true /* inherit */);
            ActionSelector[] selectors = Array.ConvertAll(attrs, attr => (ActionSelector)(controllerContext => attr.IsValidForRequest(controllerContext, BeginMethod)));
            return selectors;
        }

        public override bool IsDefined(Type attributeType, bool inherit) {
            return BeginMethod.IsDefined(attributeType, inherit);
        }

        private ParameterDescriptor[] LazilyFetchParametersCollection() {
            return DescriptorUtil.LazilyFetchOrCreate<ParameterInfo, ParameterDescriptor>(
                ref _parametersCache /* cacheLocation */,
                () => {
                    ParameterInfo[] parameters = BeginMethod.GetParameters();
                    return parameters.Take(parameters.Length - 2); // leave off the AsyncCallback + state parameters
                } /* intializer */,
                parameterInfo => new ReflectedParameterDescriptor(parameterInfo, this) /* converter */);
        }

        private static void ValidateBeginMethod(MethodInfo beginMethod) {
            if (beginMethod == null) {
                throw new ArgumentNullException("beginMethod");
            }

            string errorMessage = GetActionMethodErrorMessage(beginMethod);
            if (errorMessage == null) {
                // Method must have signature (..., AsyncCallback, object) -> IAsyncResult
                bool signatureValid = false;

                Type returnType = beginMethod.ReturnType;
                if (returnType == typeof(IAsyncResult)) {
                    ParameterInfo[] parameters = beginMethod.GetParameters();
                    if (parameters.Length >= 2) {
                        ParameterInfo asyncCallbackParameter = parameters[parameters.Length - 2];
                        ParameterInfo stateParameter = parameters[parameters.Length - 1];
                        if (asyncCallbackParameter.ParameterType == typeof(AsyncCallback) && stateParameter.ParameterType == typeof(object)) {
                            signatureValid = true;
                        }
                    }
                }

                if (!signatureValid) {
                    errorMessage = String.Format(CultureInfo.CurrentUICulture, MvcResources.ReflectedAsyncPatternActionDescriptor_BeginMethodHasWrongSignature, beginMethod);
                }
            }

            if (errorMessage != null) {
                throw new ArgumentException(errorMessage, "beginMethod");
            }
        }

        private static void ValidateEndMethod(MethodInfo endMethod) {
            if (endMethod == null) {
                throw new ArgumentNullException("endMethod");
            }

            string errorMessage = GetActionMethodErrorMessage(endMethod);
            if (errorMessage == null) {
                // Method must have signature IAsyncResult -> {anything}
                bool signatureValid = false;

                ParameterInfo[] parameters = endMethod.GetParameters();
                if (parameters.Length == 1) {
                    if (parameters[0].ParameterType == typeof(IAsyncResult)) {
                        signatureValid = true;
                    }
                }

                if (!signatureValid) {
                    errorMessage = String.Format(CultureInfo.CurrentUICulture, MvcResources.ReflectedAsyncPatternActionDescriptor_EndMethodHasWrongSignature, endMethod);
                }
            }

            if (errorMessage != null) {
                throw new ArgumentException(errorMessage, "endMethod");
            }
        }

    }
}
