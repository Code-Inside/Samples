namespace Microsoft.Web.Mvc {
    using System;
    using System.Web.Mvc;
    using System.Web.Routing;

    public interface IAsyncController : IController {

        IAsyncResult BeginExecute(RequestContext requestContext, AsyncCallback callback, object state);
        void EndExecute(IAsyncResult asyncResult);

    }
}
