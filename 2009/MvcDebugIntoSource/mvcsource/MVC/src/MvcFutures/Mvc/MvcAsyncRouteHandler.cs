namespace Microsoft.Web.Mvc {
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Routing;

    public class MvcAsyncRouteHandler : MvcRouteHandler {

        protected override IHttpHandler GetHttpHandler(RequestContext requestContext) {
            return new MvcAsyncHandler(requestContext);
        }

    }
}
