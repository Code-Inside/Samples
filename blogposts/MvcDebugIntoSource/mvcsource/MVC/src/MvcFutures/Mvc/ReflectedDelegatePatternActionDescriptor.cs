namespace Microsoft.Web.Mvc {
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;
    using System.Threading;
    using System.Web.Mvc;
    using Microsoft.Web.Resources;

    public class ReflectedDelegatePatternActionDescriptor : AsyncActionDescriptor {

        private readonly string _actionName;
        private readonly ControllerDescriptor _controllerDescriptor;
        private ParameterDescriptor[] _parametersCache;

        private static readonly object[] _emptyParameters = new object[0];
        private static readonly object _executeTag = new object();

        public ReflectedDelegatePatternActionDescriptor(MethodInfo actionMethod, string actionName, ControllerDescriptor controllerDescriptor)
            : this(actionMethod, actionName, controllerDescriptor, true /* validateMethod */) {
        }

        internal ReflectedDelegatePatternActionDescriptor(MethodInfo actionMethod, string actionName, ControllerDescriptor controllerDescriptor, bool validateMethod) {
            if (validateMethod) {
                ValidateActionMethod(actionMethod);
            }
            if (String.IsNullOrEmpty(actionName)) {
                throw new ArgumentException(MvcResources.Common_NullOrEmpty, "actionName");
            }
            if (controllerDescriptor == null) {
                throw new ArgumentNullException("controllerDescriptor");
            }

            ActionMethod = actionMethod;
            _actionName = actionName;
            _controllerDescriptor = controllerDescriptor;
        }

        public MethodInfo ActionMethod {
            get;
            private set;
        }

        public override string ActionName {
            get {
                return _actionName;
            }
        }

        public override ControllerDescriptor ControllerDescriptor {
            get {
                return _controllerDescriptor;
            }
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
            ContinuationListener listener = new ContinuationListener();
            object theDelegate = null;

            BeginInvokeCallback beginCallback = (innerCallback, innerState) => {
                ManualAsyncResult asyncResult = new ManualAsyncResult() {
                    AsyncState = innerState
                };

                // Get parameters for async setup method, then execute
                ParameterInfo[] setupParametersInfos = ActionMethod.GetParameters();
                var rawSetupParameterValues = from parameterInfo in setupParametersInfos
                                              select ExtractParameterFromDictionary(parameterInfo, parameters, ActionMethod);
                object[] setupParametersArray = rawSetupParameterValues.ToArray();

                // to simplify the logic, force an asynchronous callback
                asyncHelper.OutstandingOperations.Completed += delegate {
                    if (setupCompletedEvent.Signal()) {
                        listener.SetContinuation(() => {
                            ThreadPool.QueueUserWorkItem(o => {
                                asyncResult.MarkCompleted(false /* completedSynchronously */, innerCallback);
                            });
                        });
                    }
                };

                MethodDispatcher setupDispatcher = DispatcherCache.GetDispatcher(ActionMethod);
                asyncHelper.OutstandingOperations.Increment();
                object returnedDelegate = setupDispatcher.Execute(controllerContext.Controller, setupParametersArray);
                ValidateDelegateNotNull(returnedDelegate, ActionMethod);
                asyncHelper.OutstandingOperations.Decrement();

                Thread.VolatileWrite(ref theDelegate, returnedDelegate);
                listener.Signal();
                return asyncResult;
            };

            AsyncCallback<object> endCallback = ar => {
                if (setupCompletedEvent.Signal()) {
                    // the setup method did not complete before this callback executed
                    throw new InvalidOperationException(MvcResources.AsyncActionDescriptor_EndExecuteCalledPrematurely);
                }

                object returnedDelegate = Thread.VolatileRead(ref theDelegate);
                MethodInfo invokeMethod = returnedDelegate.GetType().GetMethod("Invoke", Type.EmptyTypes);

                MethodDispatcher invokeDispatcher = DispatcherCache.GetDispatcher(invokeMethod);
                object actionReturnValue = invokeDispatcher.Execute(returnedDelegate, _emptyParameters);
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

        public override object[] GetCustomAttributes(bool inherit) {
            return ActionMethod.GetCustomAttributes(inherit);
        }

        public override object[] GetCustomAttributes(Type attributeType, bool inherit) {
            return ActionMethod.GetCustomAttributes(attributeType, inherit);
        }

        public override FilterInfo GetFilters() {
            // For simplicity, we only consider filters on XxxAsync() since it is the entry point to the action

            // Enumerable.OrderBy() is a stable sort, so this method preserves scope ordering.
            FilterAttribute[] typeFilters = (FilterAttribute[])ActionMethod.ReflectedType.GetCustomAttributes(typeof(FilterAttribute), true /* inherit */);
            FilterAttribute[] methodFilters = (FilterAttribute[])ActionMethod.GetCustomAttributes(typeof(FilterAttribute), true /* inherit */);
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
            ActionMethodSelectorAttribute[] attrs = (ActionMethodSelectorAttribute[])ActionMethod.GetCustomAttributes(typeof(ActionMethodSelectorAttribute), true /* inherit */);
            ActionSelector[] selectors = Array.ConvertAll(attrs, attr => (ActionSelector)(controllerContext => attr.IsValidForRequest(controllerContext, ActionMethod)));
            return selectors;
        }

        public override bool IsDefined(Type attributeType, bool inherit) {
            return ActionMethod.IsDefined(attributeType, inherit);
        }

        private ParameterDescriptor[] LazilyFetchParametersCollection() {
            return DescriptorUtil.LazilyFetchOrCreate<ParameterInfo, ParameterDescriptor>(
                ref _parametersCache /* cacheLocation */,
                ActionMethod.GetParameters /* initializer */,
                parameterInfo => new ReflectedParameterDescriptor(parameterInfo, this) /* converter */);
        }

        private static void ValidateActionMethod(MethodInfo actionMethod) {
            if (actionMethod == null) {
                throw new ArgumentNullException("actionMethod");
            }
            else {
                if (!TypeHelpers.TypeIsParameterlessDelegate(actionMethod.ReturnType)) {
                    string message = String.Format(CultureInfo.CurrentUICulture,
                        MvcResources.ReflectedDelegatePatternActionDescriptor_MethodHasWrongReturnType, actionMethod);
                    throw new ArgumentException(message, "actionMethod");
                }
            }
        }

        private static void ValidateDelegateNotNull(object returnedDelegate, MethodInfo actionMethod) {
            if (returnedDelegate == null) {
                string message = String.Format(CultureInfo.CurrentUICulture,
                    MvcResources.ReflectedDelegatePatternActionDescriptor_MethodReturnedNull, actionMethod);
                throw new InvalidOperationException(message);
            }
        }

    }
}
