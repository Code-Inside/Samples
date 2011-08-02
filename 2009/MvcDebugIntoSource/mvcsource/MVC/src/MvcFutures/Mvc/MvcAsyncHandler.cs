namespace Microsoft.Web.Mvc {
    using System;
    using System.Globalization;
    using System.Threading;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Routing;
    using Microsoft.Web.Resources;

    public class MvcAsyncHandler : MvcHandler, IHttpAsyncHandler {

        private delegate void ExecuteDelegate(RequestContext requestContext);
        private delegate IAsyncResult BeginExecuteDelegate(RequestContext requestContext, AsyncCallback callback, object state);
        private delegate void EndExecuteDelegate(IAsyncResult asyncResult);

        private ControllerBuilder _controllerBuilder;

        private static readonly object _processRequestTag = new object();

        public MvcAsyncHandler(RequestContext requestContext)
            : this(requestContext, null /* syncContext */) {
        }

        public MvcAsyncHandler(RequestContext requestContext, SynchronizationContext syncContext)
            : base(requestContext) {
            SynchronizationContext = syncContext ?? SynchronizationContextHelper.GetSynchronizationContext();
        }

        internal ControllerBuilder ControllerBuilder {
            get {
                if (_controllerBuilder == null) {
                    _controllerBuilder = ControllerBuilder.Current;
                }
                return _controllerBuilder;
            }
            set {
                _controllerBuilder = value;
            }
        }

        public SynchronizationContext SynchronizationContext {
            get;
            private set;
        }

        protected virtual IAsyncResult BeginProcessRequest(HttpContext context, AsyncCallback cb, object extraData) {
            HttpContextBase abstractContext = new HttpContextWrapper(context);
            return BeginProcessRequest(abstractContext, cb, extraData);
        }

        protected internal virtual IAsyncResult BeginProcessRequest(HttpContextBase httpContext, AsyncCallback callback, object state) {
            AddVersionHeader(httpContext);

            // Get the controller type
            string controllerName = RequestContext.RouteData.GetRequiredString("controller");

            // Instantiate the controller and call Execute
            IControllerFactory factory = ControllerBuilder.GetControllerFactory();
            IController controller = factory.CreateController(RequestContext, controllerName);
            if (controller == null) {
                throw new InvalidOperationException(
                    String.Format(
                        CultureInfo.CurrentUICulture,
                        MvcResources.ControllerBuilder_FactoryReturnedNull,
                        factory.GetType(),
                        controllerName));
            }

            BeginExecuteDelegate beginExecute;
            EndExecuteDelegate endExecute;

            IAsyncController asyncController = controller as IAsyncController;
            if (asyncController != null) {
                beginExecute = asyncController.BeginExecute;
                endExecute = asyncController.EndExecute;
            }
            else {
                // execute synchronous controller asynchronously
                ExecuteDelegate executeDelegate = rc => SynchronizationContext.Sync(() => controller.Execute(rc));
                beginExecute = executeDelegate.BeginInvoke;
                endExecute = executeDelegate.EndInvoke;
            }

            BeginInvokeCallback beginCallback = (innerCallback, innerState) => {
                try {
                    return beginExecute(RequestContext, innerCallback, innerState);
                }
                catch {
                    factory.ReleaseController(controller);
                    throw;
                }
            };

            AsyncCallback endCallback = ar => {
                try {
                    endExecute(ar);
                }
                finally {
                    factory.ReleaseController(controller);
                }
            };

            return AsyncResultWrapper.Wrap(callback, state, beginCallback, endCallback, _processRequestTag);
        }

        protected internal virtual void EndProcessRequest(IAsyncResult result) {
            AsyncResultWrapper.UnwrapAndContinue(result, _processRequestTag);
        }

        #region IHttpAsyncHandler Members
        IAsyncResult IHttpAsyncHandler.BeginProcessRequest(HttpContext context, AsyncCallback cb, object extraData) {
            return BeginProcessRequest(context, cb, extraData);
        }

        void IHttpAsyncHandler.EndProcessRequest(IAsyncResult result) {
            EndProcessRequest(result);
        }
        #endregion
    }
}
