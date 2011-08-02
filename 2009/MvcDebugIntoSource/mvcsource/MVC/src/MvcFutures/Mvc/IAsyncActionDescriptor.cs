namespace Microsoft.Web.Mvc {
    using System;
    using System.Collections.Generic;
    using System.Web.Mvc;

    public interface IAsyncActionDescriptor {

        IAsyncResult BeginExecute(ControllerContext controllerContext, IDictionary<string, object> parameters, AsyncCallback callback, object state);
        object EndExecute(IAsyncResult asyncResult);

    }
}
