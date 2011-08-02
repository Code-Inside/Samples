namespace Microsoft.Web.Mvc {
    using System;
    using System.Reflection;
    using System.Web.Mvc;

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public sealed class AcceptAjaxAttribute : ActionMethodSelectorAttribute {

        public override bool IsValidForRequest(ControllerContext controllerContext, MethodInfo methodInfo) {
            if (controllerContext == null) {
                throw new ArgumentNullException("controllerContext");
            }

            return controllerContext.HttpContext.Request.IsAjaxRequest();
        }

    }
}
