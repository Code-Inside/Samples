namespace Microsoft.Web.Mvc {
    using System;
    using System.Threading;
    using System.Web;
    using System.Web.Mvc;

    public class MvcHttpAsyncHandler : MvcHttpHandler, IHttpAsyncHandler {

        private delegate void ProcessRequestDelegate(HttpContext context);
        private delegate IAsyncResult BeginProcessRequestDelegate(HttpContext context, AsyncCallback cb, object extraData);

        private static readonly object _processRequestTag = new object();

        public MvcHttpAsyncHandler()
            : this(null /* syncContext */) {
        }

        public MvcHttpAsyncHandler(SynchronizationContext syncContext) {
            SynchronizationContext = syncContext ?? SynchronizationContextHelper.GetSynchronizationContext();
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
            IHttpHandler requestHandler = GetHttpHandler(httpContext);

            BeginProcessRequestDelegate beginCallback;
            AsyncCallback endCallback;

            IHttpAsyncHandler asyncHandler = requestHandler as IHttpAsyncHandler;
            if (asyncHandler != null) {
                beginCallback = asyncHandler.BeginProcessRequest;
                endCallback = asyncHandler.EndProcessRequest;
            }
            else {
                // execute synchronous IHttpHandler asynchronously
                ProcessRequestDelegate processRequestDelegate = hc => SynchronizationContext.Sync(() => requestHandler.ProcessRequest(hc));
                beginCallback = processRequestDelegate.BeginInvoke;
                endCallback = processRequestDelegate.EndInvoke;
            }

            return AsyncResultWrapper.Wrap(callback, state,
                (innerCallback, innerState) => beginCallback(HttpContext.Current, innerCallback, innerState),
                endCallback, _processRequestTag);
        }

        protected virtual void EndProcessRequest(IAsyncResult result) {
            AsyncResultWrapper.UnwrapAndContinue(result, _processRequestTag);
        }

        private static IHttpHandler GetHttpHandler(HttpContextBase httpContext) {
            MvcVerificationHttpHandler verificationHandler = new MvcVerificationHttpHandler();
            verificationHandler.PublicProcessRequest(httpContext);
            return verificationHandler.HttpHandler;
        }

        #region IHttpAsyncHandler Members
        IAsyncResult IHttpAsyncHandler.BeginProcessRequest(HttpContext context, AsyncCallback cb, object extraData) {
            return BeginProcessRequest(context, cb, extraData);
        }

        void IHttpAsyncHandler.EndProcessRequest(IAsyncResult result) {
            EndProcessRequest(result);
        }
        #endregion

        private sealed class MvcVerificationHttpHandler : MvcHttpHandler {
            public IHttpHandler HttpHandler;

            public void PublicProcessRequest(HttpContextBase httpContext) {
                ProcessRequest(httpContext);
            }

            protected override void VerifyAndProcessRequest(IHttpHandler httpHandler, HttpContextBase httpContext) {
                HttpHandler = httpHandler;
            }
        }

    }
}
