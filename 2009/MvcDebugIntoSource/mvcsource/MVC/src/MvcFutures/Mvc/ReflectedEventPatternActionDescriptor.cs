namespace Microsoft.Web.Mvc {
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;
    using System.Threading;
    using System.Web.Mvc;
    using Microsoft.Web.Resources;

    public class ReflectedEventPatternActionDescriptor : AsyncActionDescriptor {

        private readonly string _actionName;
        private readonly ControllerDescriptor _controllerDescriptor;
        private ParameterDescriptor[] _parametersCache;

        private static readonly object _executeTag = new object();

        public ReflectedEventPatternActionDescriptor(MethodInfo setupMethod, MethodInfo completionMethod, string actionName, ControllerDescriptor controllerDescriptor)
            : this(setupMethod, completionMethod, actionName, controllerDescriptor, true /* validateMethods */) {
        }

        internal ReflectedEventPatternActionDescriptor(MethodInfo setupMethod, MethodInfo completionMethod, string actionName, ControllerDescriptor controllerDescriptor, bool validateMethods) {
            if (validateMethods) {
                ValidateMethods(setupMethod, completionMethod);
            }
            if (String.IsNullOrEmpty(actionName)) {
                throw new ArgumentException(MvcResources.Common_NullOrEmpty, "actionName");
            }
            if (controllerDescriptor == null) {
                throw new ArgumentNullException("controllerDescriptor");
            }

            SetupMethod = setupMethod;
            CompletionMethod = completionMethod;
            _actionName = actionName;
            _controllerDescriptor = controllerDescriptor;
        }

        public override string ActionName {
            get {
                return _actionName;
            }
        }

        public MethodInfo CompletionMethod {
            get;
            private set;
        }

        public override ControllerDescriptor ControllerDescriptor {
            get {
                return _controllerDescriptor;
            }
        }

        public MethodInfo SetupMethod {
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

            AsyncManager asyncHelper = GetAsyncManager(controllerContext.Controller);
            SingleFireEvent setupCompletedEvent = new SingleFireEvent();

            BeginInvokeCallback beginCallback = (innerCallback, innerState) => {
                ManualAsyncResult asyncResult = new ManualAsyncResult() {
                    AsyncState = innerState
                };

                // Get parameters for async setup method, then execute
                ParameterInfo[] setupParametersInfos = SetupMethod.GetParameters();
                var rawSetupParameterValues = from parameterInfo in setupParametersInfos
                                              select ExtractParameterFromDictionary(parameterInfo, parameters, SetupMethod);
                object[] setupParametersArray = rawSetupParameterValues.ToArray();

                // to simplify the logic, force an asynchronous callback
                asyncHelper.OutstandingOperations.Completed += delegate {
                    if (setupCompletedEvent.Signal()) {
                        ThreadPool.QueueUserWorkItem(o => {
                            asyncResult.MarkCompleted(false /* completedSynchronously */, innerCallback);
                        });
                    }
                };

                MethodDispatcher setupDispatcher = DispatcherCache.GetDispatcher(SetupMethod);
                asyncHelper.OutstandingOperations.Increment();
                setupDispatcher.Execute(controllerContext.Controller, setupParametersArray);
                asyncHelper.OutstandingOperations.Decrement();
                return asyncResult;
            };

            AsyncCallback<object> endCallback = ar => {
                if (setupCompletedEvent.Signal()) {
                    // the setup method did not complete before this callback executed
                    throw new InvalidOperationException(MvcResources.AsyncActionDescriptor_EndExecuteCalledPrematurely);
                }

                // Get parameters for action method, then execute
                ParameterInfo[] completionParametersInfos = CompletionMethod.GetParameters();
                var rawCompletionParameterValues = from parameterInfo in completionParametersInfos
                                                   select ExtractParameterOrDefaultFromDictionary(parameterInfo, asyncHelper.Parameters);
                object[] completionParametersArray = rawCompletionParameterValues.ToArray();

                MethodDispatcher completionDispatcher = DispatcherCache.GetDispatcher(CompletionMethod);
                object actionReturnValue = completionDispatcher.Execute(controllerContext.Controller, completionParametersArray);
                return actionReturnValue;
            };

            // Set the timeout and go
            int timeout = asyncHelper.Timeout;
            return AsyncResultWrapper.WrapWithTimeout(callback, state, beginCallback, endCallback, timeout, _executeTag);
        }

        public override object EndExecute(IAsyncResult asyncResult) {
            return AsyncResultWrapper.UnwrapAndContinue<object>(asyncResult, _executeTag);
        }

        public override object Execute(ControllerContext controllerContext, IDictionary<string, object> parameters) {
            string message = String.Format(CultureInfo.CurrentUICulture,
                MvcResources.AsyncActionDescriptor_CannotBeCalledSynchronously, ActionName);
            throw new InvalidOperationException(message);
        }

        private static object ExtractParameterOrDefaultFromDictionary(ParameterInfo parameterInfo, IDictionary<string, object> parameters) {
            Type parameterType = parameterInfo.ParameterType;

            object value;
            parameters.TryGetValue(parameterInfo.Name, out value);

            // if wrong type, replace with default instance
            if (parameterType.IsInstanceOfType(value)) {
                return value;
            }
            else {
                return (TypeHelpers.TypeAllowsNullValue(parameterType)) ? null : Activator.CreateInstance(parameterType);
            }
        }

        public override object[] GetCustomAttributes(bool inherit) {
            return SetupMethod.GetCustomAttributes(inherit);
        }

        public override object[] GetCustomAttributes(Type attributeType, bool inherit) {
            return SetupMethod.GetCustomAttributes(attributeType, inherit);
        }

        public override FilterInfo GetFilters() {
            // For simplicity, we only consider filters on the setup method since it is the entry point to the action

            // Enumerable.OrderBy() is a stable sort, so this method preserves scope ordering.
            FilterAttribute[] typeFilters = (FilterAttribute[])SetupMethod.ReflectedType.GetCustomAttributes(typeof(FilterAttribute), true /* inherit */);
            FilterAttribute[] methodFilters = (FilterAttribute[])SetupMethod.GetCustomAttributes(typeof(FilterAttribute), true /* inherit */);
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
            ActionMethodSelectorAttribute[] attrs = (ActionMethodSelectorAttribute[])SetupMethod.GetCustomAttributes(typeof(ActionMethodSelectorAttribute), true /* inherit */);
            ActionSelector[] selectors = Array.ConvertAll(attrs, attr => (ActionSelector)(controllerContext => attr.IsValidForRequest(controllerContext, SetupMethod)));
            return selectors;
        }

        public override bool IsDefined(Type attributeType, bool inherit) {
            return SetupMethod.IsDefined(attributeType, inherit);
        }

        private ParameterDescriptor[] LazilyFetchParametersCollection() {
            return DescriptorUtil.LazilyFetchOrCreate<ParameterInfo, ParameterDescriptor>(
                ref _parametersCache /* cacheLocation */,
                SetupMethod.GetParameters /* initializer */,
                parameterInfo => new ReflectedParameterDescriptor(parameterInfo, this) /* converter */);
        }

        private static void ValidateMethods(MethodInfo setupMethod, MethodInfo completionMethod) {
            if (setupMethod == null) {
                throw new ArgumentNullException("setupMethod");
            }
            else {
                string errorMessage = GetActionMethodErrorMessage(setupMethod);
                if (errorMessage != null) {
                    throw new ArgumentException(errorMessage, "setupMethod");
                }
            }
            if (completionMethod == null) {
                throw new ArgumentNullException("completionMethod");
            }
            else {
                string errorMessage = GetActionMethodErrorMessage(completionMethod);
                if (errorMessage != null) {
                    throw new ArgumentException(errorMessage, "completionMethod");
                }
            }
        }

    }
}
