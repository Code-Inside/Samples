using System.Web.Mvc;
using Raven.Client;

namespace EmbeddedRavenDB.Infrastructure.Raven
{
    /// <summary>
    /// This filter will manage the session for all of the controllers that needs a Raven Document Session.
    /// It does so by automatically injecting a session to the first public property of type IDocumentSession available
    /// on the controller.
    /// </summary>
    public class RavenActionFilterAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext.IsChildAction)
            {
                DocumentStoreHolder.TrySetSession(filterContext.Controller, (IDocumentSession)filterContext.HttpContext.Items[this]);
                return;
            }
            filterContext.HttpContext.Items[this] = DocumentStoreHolder.TryAddSession(filterContext.Controller);
        }

        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            if (filterContext.IsChildAction)
                return;
            DocumentStoreHolder.TryComplete(filterContext.Controller, filterContext.Exception == null);
        }
    }
}