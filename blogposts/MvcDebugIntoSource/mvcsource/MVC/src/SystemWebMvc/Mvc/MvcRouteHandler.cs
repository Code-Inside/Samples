namespace System.Web.Mvc {
    using System.Web.Routing;

    public class MvcRouteHandler : IRouteHandler {
        protected virtual IHttpHandler GetHttpHandler(RequestContext requestContext) {
            return new MvcHandler(requestContext);
        }

        #region IRouteHandler Members
        IHttpHandler IRouteHandler.GetHttpHandler(RequestContext requestContext) {
            return GetHttpHandler(requestContext);
        }
        #endregion
    }
}
