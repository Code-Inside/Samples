namespace System.Web.Mvc {
    using System.Web.Routing;
    using System.Web.SessionState;

    public class MvcHttpHandler : UrlRoutingHandler, IRequiresSessionState {

        protected override void VerifyAndProcessRequest(IHttpHandler httpHandler, HttpContextBase httpContext) {
            if (httpHandler == null) {
                throw new ArgumentNullException("httpHandler");
            }

            httpHandler.ProcessRequest(HttpContext.Current);
        }
    }
}
