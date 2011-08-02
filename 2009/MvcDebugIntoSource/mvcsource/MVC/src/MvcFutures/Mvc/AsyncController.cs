namespace Microsoft.Web.Mvc {
    using System;
    using System.Web.Mvc;
    using System.Web.Routing;

    public abstract class AsyncController : Controller, IAsyncController, IAsyncManagerContainer {

        private delegate bool InvokeActionDelegate(ControllerContext controllerContext, string actionName);
        private delegate IAsyncResult BeginInvokeActionDelegate(ControllerContext controllerContext, string actionName, AsyncCallback callback, object state);
        private delegate bool EndInvokeActionDelegate(IAsyncResult asyncResult);

        private readonly AsyncManager _asyncManager = new AsyncManager();

        private static readonly object _executeTag = new object();
        private static readonly object _executeCoreTag = new object();

        protected AsyncController() {
            ActionInvoker = new AsyncControllerActionInvoker();
        }

        public AsyncManager AsyncManager {
            get {
                return _asyncManager;
            }
        }

        protected virtual IAsyncResult BeginExecute(RequestContext requestContext, AsyncCallback callback, object state) {
            if (requestContext == null) {
                throw new ArgumentNullException("requestContext");
            }

            Initialize(requestContext);
            return AsyncResultWrapper.Wrap(callback, state, BeginExecuteCore, EndExecuteCore, _executeTag);
        }

        protected internal virtual IAsyncResult BeginExecuteCore(AsyncCallback callback, object state) {
            TempData.Load(ControllerContext, TempDataProvider);

            string actionName = RouteData.GetRequiredString("action");
            IActionInvoker invoker = ActionInvoker;
            IAsyncActionInvoker asyncInvoker = invoker as IAsyncActionInvoker;

            BeginInvokeActionDelegate beginDelegate;
            EndInvokeActionDelegate endDelegate;

            if (asyncInvoker != null) {
                beginDelegate = asyncInvoker.BeginInvokeAction;
                endDelegate = asyncInvoker.EndInvokeAction;
            }
            else {
                // execute synchronous method asynchronously
                InvokeActionDelegate invokeDelegate = (cc, an) => AsyncManager.SynchronizationContext.Sync(() => invoker.InvokeAction(cc, an));
                beginDelegate = invokeDelegate.BeginInvoke;
                endDelegate = invokeDelegate.EndInvoke;
            }

            return AsyncResultWrapper.Wrap(callback, state,
                (innerCallback, innerState) => {
                    try {
                        return beginDelegate(ControllerContext, actionName, innerCallback, innerState);
                    }
                    catch {
                        TempData.Save(ControllerContext, TempDataProvider);
                        throw;
                    }
                },
                ar => {
                    try {
                        bool wasActionExecuted = endDelegate(ar);
                        if (!wasActionExecuted) {
                            HandleUnknownAction(actionName);
                        }
                    }
                    finally {
                        TempData.Save(ControllerContext, TempDataProvider);
                    }
                },
                _executeCoreTag);
        }

        protected virtual void EndExecute(IAsyncResult asyncResult) {
            AsyncResultWrapper.UnwrapAndContinue(asyncResult, _executeTag);
        }

        protected internal virtual void EndExecuteCore(IAsyncResult asyncResult) {
            AsyncResultWrapper.UnwrapAndContinue(asyncResult, _executeCoreTag);
        }

        #region IAsyncController Members
        IAsyncResult IAsyncController.BeginExecute(RequestContext requestContext, AsyncCallback callback, object state) {
            return BeginExecute(requestContext, callback, state);
        }

        void IAsyncController.EndExecute(IAsyncResult asyncResult) {
            EndExecute(asyncResult);
        }
        #endregion

    }
}
