namespace Microsoft.Web.Mvc {
    using System;
    using System.Web.Mvc;

    public interface IAsyncActionInvoker : IActionInvoker {

        IAsyncResult BeginInvokeAction(ControllerContext controllerContext, string actionName, AsyncCallback callback, object state);
        bool EndInvokeAction(IAsyncResult asyncResult);

    }
}
