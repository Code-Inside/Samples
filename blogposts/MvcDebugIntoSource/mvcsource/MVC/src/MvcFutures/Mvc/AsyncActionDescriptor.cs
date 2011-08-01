namespace Microsoft.Web.Mvc {
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Reflection;
    using System.Web.Mvc;
    using Microsoft.Web.Resources;

    public abstract class AsyncActionDescriptor : ActionDescriptor, IAsyncActionDescriptor {

        private readonly static MethodDispatcherCache _staticDispatcherCache = new MethodDispatcherCache();
        private MethodDispatcherCache _instanceDispatcherCache;

        internal MethodDispatcherCache DispatcherCache {
            get {
                if (_instanceDispatcherCache == null) {
                    _instanceDispatcherCache = _staticDispatcherCache;
                }
                return _instanceDispatcherCache;
            }
            set {
                _instanceDispatcherCache = value;
            }
        }

        public abstract IAsyncResult BeginExecute(ControllerContext controllerContext, IDictionary<string, object> parameters, AsyncCallback callback, object state);

        public abstract object EndExecute(IAsyncResult asyncResult);

        public override object Execute(ControllerContext controllerContext, IDictionary<string, object> parameters) {
            // vanilla wrapper that can be called from synchronous code - the EndExecute() call blocks
            IAsyncResult asyncResult = BeginExecute(controllerContext, parameters, null /* callback */, null /* state */);
            return EndExecute(asyncResult);
        }

        internal static object ExtractParameterFromDictionary(ParameterInfo parameterInfo, IDictionary<string, object> parameters, MethodInfo methodInfo) {
            object value;

            if (!parameters.TryGetValue(parameterInfo.Name, out value)) {
                // the key should always be present, even if the parameter value is null
                string message = String.Format(CultureInfo.CurrentUICulture, MvcResources.AsyncActionDescriptor_ParameterNotInDictionary,
                    parameterInfo.Name, parameterInfo.ParameterType, methodInfo, methodInfo.DeclaringType);
                throw new ArgumentException(message, "parameters");
            }

            if (value == null && !TypeHelpers.TypeAllowsNullValue(parameterInfo.ParameterType)) {
                // tried to pass a null value for a non-nullable parameter type
                string message = String.Format(CultureInfo.CurrentUICulture, MvcResources.AsyncActionDescriptor_ParameterCannotBeNull,
                    parameterInfo.Name, parameterInfo.ParameterType, methodInfo, methodInfo.DeclaringType);
                throw new ArgumentException(message, "parameters");
            }

            if (value != null && !parameterInfo.ParameterType.IsInstanceOfType(value)) {
                // value was supplied but is not of the proper type
                string message = String.Format(CultureInfo.CurrentUICulture, MvcResources.AsyncActionDescriptor_ParameterValueHasWrongType,
                    parameterInfo.Name, methodInfo, methodInfo.DeclaringType, value.GetType(), parameterInfo.ParameterType);
                throw new ArgumentException(message, "parameters");
            }

            return value;
        }

        internal static string GetActionMethodErrorMessage(MethodInfo methodInfo) {
            // we can't call instance methods where the 'this' parameter is a type other than ControllerBase
            if (!methodInfo.IsStatic && !typeof(ControllerBase).IsAssignableFrom(methodInfo.ReflectedType)) {
                return String.Format(CultureInfo.CurrentUICulture, MvcResources.AsyncActionDescriptor_CannotCallInstanceMethodOnNonControllerType,
                    methodInfo, methodInfo.ReflectedType.FullName);
            }

            // we can't call methods with open generic type parameters
            if (methodInfo.ContainsGenericParameters) {
                return String.Format(CultureInfo.CurrentUICulture, MvcResources.AsyncActionDescriptor_CannotCallOpenGenericMethods,
                    methodInfo, methodInfo.ReflectedType.FullName);
            }

            // we can't call methods with ref/out parameters
            ParameterInfo[] parameterInfos = methodInfo.GetParameters();
            foreach (ParameterInfo parameterInfo in parameterInfos) {
                if (parameterInfo.IsOut || parameterInfo.ParameterType.IsByRef) {
                    return String.Format(CultureInfo.CurrentUICulture, MvcResources.AsyncActionDescriptor_CannotCallMethodsWithOutOrRefParameters,
                        methodInfo, methodInfo.ReflectedType.FullName, parameterInfo);
                }
            }

            // we can call this method
            return null;
        }

        internal static AsyncManager GetAsyncManager(ControllerBase controller) {
            IAsyncManagerContainer helperContainer = controller as IAsyncManagerContainer;
            if (helperContainer == null) {
                throw new InvalidOperationException(String.Format(
                    CultureInfo.CurrentUICulture, MvcResources.AsyncCommon_ControllerMustImplementIAsyncManagerContainer, controller.GetType()));
            }

            return helperContainer.AsyncManager;
        }

        internal static void MergeFiltersIntoList<TFilter>(IList<FilterAttribute> allFilters, IList<TFilter> destFilters) where TFilter : class {
            foreach (FilterAttribute filter in allFilters) {
                TFilter castFilter = filter as TFilter;
                if (castFilter != null) {
                    destFilters.Add(castFilter);
                }
            }
        }

    }
}
