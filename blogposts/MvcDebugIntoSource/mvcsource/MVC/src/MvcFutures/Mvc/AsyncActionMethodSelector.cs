namespace Microsoft.Web.Mvc {
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using System.Web.Mvc;
    using Microsoft.Web.Resources;

    internal sealed class AsyncActionMethodSelector {

        public AsyncActionMethodSelector(Type controllerType) {
            ControllerType = controllerType;
            PopulateLookupTables();
        }

        public Type ControllerType {
            get;
            private set;
        }

        public MethodInfo[] AliasedMethods {
            get;
            private set;
        }

        public ILookup<string, MethodInfo> NonAliasedMethods {
            get;
            private set;
        }

        private AmbiguousMatchException CreateAmbiguousActionMatchException(IEnumerable<MethodInfo> ambiguousMethods, string actionName) {
            string ambiguityList = CreateAmbiguousMatchList(ambiguousMethods);
            string message = String.Format(CultureInfo.CurrentUICulture, MvcResources.ActionMethodSelector_AmbiguousActionMatch,
                actionName, ControllerType.Name, ambiguityList);
            return new AmbiguousMatchException(message);
        }

        private AmbiguousMatchException CreateAmbiguousMethodMatchException(IEnumerable<MethodInfo> ambiguousMethods, string methodName) {
            string ambiguityList = CreateAmbiguousMatchList(ambiguousMethods);
            string message = String.Format(CultureInfo.CurrentUICulture, MvcResources.ActionMethodSelector_AmbiguousMethodMatch,
                methodName, ControllerType.Name, ambiguityList);
            return new AmbiguousMatchException(message);
        }

        private static string CreateAmbiguousMatchList(IEnumerable<MethodInfo> ambiguousMethods) {
            StringBuilder exceptionMessageBuilder = new StringBuilder();
            foreach (MethodInfo methodInfo in ambiguousMethods) {
                exceptionMessageBuilder.AppendLine();
                exceptionMessageBuilder.AppendFormat(CultureInfo.CurrentUICulture, MvcResources.ActionMethodSelector_AmbiguousMatchType, methodInfo, methodInfo.DeclaringType.FullName);
            }

            return exceptionMessageBuilder.ToString();
        }

        public ActionDescriptorCreator FindActionMethod(ControllerContext controllerContext, string actionName) {
            List<MethodInfo> methodsMatchingName = GetMatchingAliasedMethods(controllerContext, actionName);
            methodsMatchingName.AddRange(NonAliasedMethods[actionName]);
            List<MethodInfo> finalMethods = RunSelectionFilters(controllerContext, methodsMatchingName);

            switch (finalMethods.Count) {
                case 0:
                    return null;

                case 1:
                    MethodInfo entryMethod = finalMethods[0];
                    return GetActionDescriptorDelegate(entryMethod);

                default:
                    throw CreateAmbiguousActionMatchException(finalMethods, actionName);
            }
        }

        private ActionDescriptorCreator GetActionDescriptorDelegate(MethodInfo entryMethod) {
            // Is this the BeginFoo() / EndFoo() pattern?
            if (entryMethod.Name.StartsWith("Begin", StringComparison.OrdinalIgnoreCase)) {
                string endMethodName = "End" + entryMethod.Name.Substring("Begin".Length);
                MethodInfo endMethod = GetMethodByName(endMethodName);
                if (endMethod == null) {
                    string errorMessage = String.Format(CultureInfo.CurrentUICulture, MvcResources.ActionMethodSelector_CouldNotFindMethod, endMethodName, ControllerType.FullName);
                    throw new InvalidOperationException(errorMessage);
                }
                return (actionName, controllerDescriptor) => new ReflectedAsyncPatternActionDescriptor(entryMethod, endMethod, actionName, controllerDescriptor);
            }

            // Is this the Foo() / FooCompleted() pattern?
            {
                string completionMethodName = entryMethod.Name + "Completed";
                MethodInfo completionMethod = GetMethodByName(completionMethodName);
                if (completionMethod != null) {
                    return (actionName, controllerDescriptor) => new ReflectedEventPatternActionDescriptor(entryMethod, completionMethod, actionName, controllerDescriptor);
                }
            }

            // Does Foo() return a delegate that represents the continuation?
            if (TypeHelpers.TypeIsParameterlessDelegate(entryMethod.ReturnType)) {
                return (actionName, controllerDescriptor) => new ReflectedDelegatePatternActionDescriptor(entryMethod, actionName, controllerDescriptor);
            }

            // Fallback to synchronous method
            return (actionName, controllerDescriptor) => new ReflectedActionDescriptor(entryMethod, actionName, controllerDescriptor);
        }

        private static string GetCanonicalMethodName(MethodInfo methodInfo) {
            string methodName = methodInfo.Name;
            return (methodName.StartsWith("Begin", StringComparison.OrdinalIgnoreCase))
                ? methodName.Substring("Begin".Length)
                : methodName;
        }

        internal List<MethodInfo> GetMatchingAliasedMethods(ControllerContext controllerContext, string actionName) {
            // find all aliased methods which are opting in to this request
            // to opt in, all attributes defined on the method must return true

            var methods = from methodInfo in AliasedMethods
                          let attrs = (ActionNameSelectorAttribute[])methodInfo.GetCustomAttributes(typeof(ActionNameSelectorAttribute), true /* inherit */)
                          where attrs.All(attr => attr.IsValidName(controllerContext, actionName, methodInfo))
                          select methodInfo;
            return methods.ToList();
        }

        private static bool IsMethodDecoratedWithAliasingAttribute(MethodInfo methodInfo) {
            return methodInfo.IsDefined(typeof(ActionNameSelectorAttribute), true /* inherit */);
        }

        private MethodInfo GetMethodByName(string methodName) {
            List<MethodInfo> methods = (from MethodInfo methodInfo in ControllerType.GetMember(methodName, MemberTypes.Method, BindingFlags.Instance | BindingFlags.Public | BindingFlags.InvokeMethod | BindingFlags.IgnoreCase)
                                        where IsValidActionMethod(methodInfo, false /* stripInfrastructureMethods */)
                                        select methodInfo).ToList();

            switch (methods.Count) {
                case 0:
                    return null;

                case 1:
                    return methods[0];

                default:
                    throw CreateAmbiguousMethodMatchException(methods, methodName);
            }
        }

        private static bool IsValidActionMethod(MethodInfo methodInfo) {
            return IsValidActionMethod(methodInfo, true /* stripInfrastructureMethods */);
        }

        private static bool IsValidActionMethod(MethodInfo methodInfo, bool stripInfrastructureMethods) {
            if (methodInfo.IsSpecialName) {
                // not a normal method, e.g. a constructor or an event
                return false;
            }

            if (methodInfo.GetBaseDefinition().DeclaringType.IsAssignableFrom(typeof(AsyncController))) {
                // is a method on Object, ControllerBase, Controller, or AsyncController
                return false;
            };

            if (stripInfrastructureMethods) {
                string methodName = methodInfo.Name;
                if (methodName.StartsWith("End", StringComparison.OrdinalIgnoreCase) || methodName.EndsWith("Completed", StringComparison.OrdinalIgnoreCase)) {
                    // do not match EndFoo() or FooCompleted() methods, as these are infrastructure methods
                    return false;
                }
            }

            return true;
        }

        private void PopulateLookupTables() {
            MethodInfo[] allMethods = ControllerType.GetMethods(BindingFlags.InvokeMethod | BindingFlags.Instance | BindingFlags.Public);
            MethodInfo[] actionMethods = Array.FindAll(allMethods, IsValidActionMethod);

            AliasedMethods = Array.FindAll(actionMethods, IsMethodDecoratedWithAliasingAttribute);
            NonAliasedMethods = actionMethods.Except(AliasedMethods).ToLookup(GetCanonicalMethodName, StringComparer.OrdinalIgnoreCase);
        }

        private static List<MethodInfo> RunSelectionFilters(ControllerContext controllerContext, List<MethodInfo> methodInfos) {
            // remove all methods which are opting out of this request
            // to opt out, at least one attribute defined on the method must return false

            List<MethodInfo> matchesWithSelectionAttributes = new List<MethodInfo>();
            List<MethodInfo> matchesWithoutSelectionAttributes = new List<MethodInfo>();

            foreach (MethodInfo methodInfo in methodInfos) {
                ActionMethodSelectorAttribute[] attrs = (ActionMethodSelectorAttribute[])methodInfo.GetCustomAttributes(typeof(ActionMethodSelectorAttribute), true /* inherit */);
                if (attrs.Length == 0) {
                    matchesWithoutSelectionAttributes.Add(methodInfo);
                }
                else if (attrs.All(attr => attr.IsValidForRequest(controllerContext, methodInfo))) {
                    matchesWithSelectionAttributes.Add(methodInfo);
                }
            }

            // if a matching action method had a selection attribute, consider it more specific than a matching action method
            // without a selection attribute
            return (matchesWithSelectionAttributes.Count > 0) ? matchesWithSelectionAttributes : matchesWithoutSelectionAttributes;
        }

    }
}
