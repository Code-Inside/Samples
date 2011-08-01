namespace Microsoft.Web.Mvc {
    using System;
    using System.Globalization;
    using System.Web.Mvc;
    using Microsoft.Web.Resources;

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public class AsyncTimeoutAttribute : ActionFilterAttribute {

        // duration is specified in seconds
        public AsyncTimeoutAttribute(int duration) {
            if (duration < -1) {
                throw new ArgumentOutOfRangeException("duration", MvcResources.AsyncTimeoutAttribute_DurationMustBeNonNegative);
            }

            Duration = duration;
        }

        public int Duration {
            get;
            private set;
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext) {
            if (filterContext == null) {
                throw new ArgumentNullException("filterContext");
            }

            IAsyncManagerContainer helperContainer = filterContext.Controller as IAsyncManagerContainer;
            if (helperContainer == null) {
                string message = String.Format(CultureInfo.CurrentUICulture,
                    MvcResources.AsyncCommon_ControllerMustImplementIAsyncManagerContainer, filterContext.Controller.GetType());
                throw new InvalidOperationException(message);
            }

            helperContainer.AsyncManager.Timeout = Duration;

            base.OnActionExecuting(filterContext);
        }

    }
}
