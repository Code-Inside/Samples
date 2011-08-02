namespace Microsoft.Web.Mvc.Test {
    using System;
    using System.Web.Mvc;

    [KeyedActionFilter(Key = "BaseClass", Order = 0)]
    [KeyedAuthorizationFilter(Key = "BaseClass", Order = 0)]
    [KeyedExceptionFilter(Key = "BaseClass", Order = 0)]
    internal class GetMemberChainController : Controller {

        [KeyedActionFilter(Key = "BaseMethod", Order = 0)]
        [KeyedAuthorizationFilter(Key = "BaseMethod", Order = 0)]
        public virtual void SomeVirtual() {
        }

    }

    [KeyedActionFilter(Key = "DerivedClass", Order = 1)]
    internal class GetMemberChainDerivedController : GetMemberChainController {

    }

    [KeyedActionFilter(Key = "SubderivedClass", Order = 2)]
    internal class GetMemberChainSubderivedController : GetMemberChainDerivedController {

        [KeyedActionFilter(Key = "SubderivedMethod", Order = 2)]
        public override void SomeVirtual() {
        }

    }

    internal abstract class KeyedFilterAttribute : FilterAttribute {
        public string Key {
            get;
            set;
        }
    }

    internal class KeyedAuthorizationFilterAttribute : KeyedFilterAttribute, IAuthorizationFilter {
        public void OnAuthorization(AuthorizationContext filterContext) {
            throw new NotImplementedException();
        }
    }

    internal class KeyedExceptionFilterAttribute : KeyedFilterAttribute, IExceptionFilter {
        public void OnException(ExceptionContext filterContext) {
            throw new NotImplementedException();
        }
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
    internal class KeyedActionFilterAttribute : KeyedFilterAttribute, IActionFilter, IResultFilter {
        public void OnActionExecuting(ActionExecutingContext filterContext) {
            throw new NotImplementedException();
        }
        public void OnActionExecuted(ActionExecutedContext filterContext) {
            throw new NotImplementedException();
        }
        public void OnResultExecuting(ResultExecutingContext filterContext) {
            throw new NotImplementedException();
        }

        public void OnResultExecuted(ResultExecutedContext filterContext) {
            throw new NotImplementedException();
        }
    }

}
