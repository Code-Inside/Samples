namespace Microsoft.Web.Mvc {
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Threading;
    using System.Web;
    using System.Web.Mvc;
    using Microsoft.Web.Resources;

    public class AsyncControllerActionInvoker : ControllerActionInvoker, IAsyncActionInvoker {

        private delegate object ExecuteDelegate(ControllerContext controllerContext, IDictionary<string, object> parameters);
        private delegate IAsyncResult BeginExecuteDelegate(ControllerContext controllerContext, IDictionary<string, object> parameters, AsyncCallback callback, object state);
        private delegate object EndExecuteDelegate(IAsyncResult asyncResult);

        private readonly static AsyncControllerDescriptorCache _staticDescriptorCache = new AsyncControllerDescriptorCache();
        private AsyncControllerDescriptorCache _instanceDescriptorCache;

        private static readonly object _invokeActionTag = new object();
        private static readonly object _invokeActionMethodTag = new object();
        private static readonly object _invokeActionMethodWithFiltersTag = new object();

        public AsyncControllerActionInvoker()
            : this(null /* syncContext */) {
        }

        public AsyncControllerActionInvoker(SynchronizationContext syncContext) {
            SynchronizationContext = syncContext ?? SynchronizationContextHelper.GetSynchronizationContext();
        }

        internal AsyncControllerDescriptorCache DescriptorCache {
            get {
                if (_instanceDescriptorCache == null) {
                    _instanceDescriptorCache = _staticDescriptorCache;
                }
                return _instanceDescriptorCache;
            }
            set {
                _instanceDescriptorCache = value;
            }
        }

        public SynchronizationContext SynchronizationContext {
            get;
            private set;
        }

        public virtual IAsyncResult BeginInvokeAction(ControllerContext controllerContext, string actionName, AsyncCallback callback, object state) {
            if (controllerContext == null) {
                throw new ArgumentNullException("controllerContext");
            }
            if (String.IsNullOrEmpty(actionName)) {
                throw new ArgumentException(MvcResources.Common_NullOrEmpty, "actionName");
            }

            ControllerDescriptor controllerDescriptor = GetControllerDescriptor(controllerContext);
            ActionDescriptor actionDescriptor = FindAction(controllerContext, controllerDescriptor, actionName);
            if (actionDescriptor == null) {
                // notify controller that no method matched
                return CreateInvokeActionNotFoundAsyncResult(callback, state);
            }

            Action continuation;
            FilterInfo filterInfo = GetFilters(controllerContext, actionDescriptor);
            try {
                AuthorizationContext authContext = InvokeAuthorizationFilters(controllerContext, filterInfo.AuthorizationFilters, actionDescriptor);
                if (authContext.Result != null) {
                    // the auth filter signaled that we should let it short-circuit the request
                    continuation = () => {
                        BeginInvokeActionEndContinuation(controllerContext, filterInfo.ExceptionFilters, () => {
                            InvokeActionResult(controllerContext, authContext.Result);
                        });
                    };
                }
                else {
                    if (controllerContext.Controller.ValidateRequest) {
                        ValidateRequest(controllerContext.HttpContext.Request);
                    }

                    IDictionary<string, object> parameters = GetParameterValues(controllerContext, actionDescriptor);

                    return AsyncResultWrapper.Wrap(callback, state,
                        (innerCallback, innerState) => BeginInvokeActionMethodWithFilters(controllerContext, filterInfo.ActionFilters, actionDescriptor, parameters, innerCallback, innerState),
                        ar => {
                            BeginInvokeActionEndContinuation(controllerContext, filterInfo.ExceptionFilters, () => {
                                ActionExecutedContext postActionContext = EndInvokeActionMethodWithFilters(ar);
                                InvokeActionResultWithFilters(controllerContext, filterInfo.ResultFilters, postActionContext.Result);
                            });
                            return true;
                        },
                        _invokeActionTag);
                }
            }
            catch (ThreadAbortException) {
                // This type of exception occurs as a result of Response.Redirect(), but we special-case so that
                // the filters don't see this as an error.
                throw;
            }
            catch (Exception ex) {
                // something blew up, so execute the exception filters
                ExceptionContext exceptionContext = InvokeExceptionFilters(controllerContext, filterInfo.ExceptionFilters, ex);
                if (!exceptionContext.ExceptionHandled) {
                    throw;
                }

                continuation = () => InvokeActionResult(controllerContext, exceptionContext.Result);
            }

            return CreateInvokeActionContinuationAsyncResult(callback, state, continuation);
        }

        internal void BeginInvokeActionEndContinuation(ControllerContext controllerContext, IList<IExceptionFilter> exceptionFilters, Action continuation) {
            try {
                continuation();
            }
            catch (ThreadAbortException) {
                // This type of exception occurs as a result of Response.Redirect(), but we special-case so that
                // the filters don't see this as an error.
                throw;
            }
            catch (Exception ex) {
                // something blew up, so execute the exception filters
                ExceptionContext exceptionContext = InvokeExceptionFilters(controllerContext, exceptionFilters, ex);
                if (!exceptionContext.ExceptionHandled) {
                    throw;
                }

                InvokeActionResult(controllerContext, exceptionContext.Result);
            }
        }

        protected internal virtual IAsyncResult BeginInvokeActionMethod(ControllerContext controllerContext, ActionDescriptor actionDescriptor, IDictionary<string, object> parameters, AsyncCallback callback, object state) {
            BeginExecuteDelegate beginExecute;
            EndExecuteDelegate endExecute;

            IAsyncActionDescriptor asyncDescriptor = actionDescriptor as IAsyncActionDescriptor;
            if (asyncDescriptor != null) {
                beginExecute = asyncDescriptor.BeginExecute;
                endExecute = asyncDescriptor.EndExecute;
            }
            else {
                // execute synchronous descriptor asynchronously
                ExecuteDelegate execute = (cc, p) => SynchronizationContext.Sync(() => actionDescriptor.Execute(cc, p));
                beginExecute = execute.BeginInvoke;
                endExecute = execute.EndInvoke;
            }

            return AsyncResultWrapper.Wrap(callback, state,
                (innerCb, innerState) => beginExecute(controllerContext, parameters, innerCb, innerState),
                ar => {
                    object returnValue = endExecute(ar);
                    ActionResult result = CreateActionResult(controllerContext, actionDescriptor, returnValue);
                    return result;
                }, _invokeActionMethodTag);
        }

        public virtual bool EndInvokeAction(IAsyncResult asyncResult) {
            return AsyncResultWrapper.UnwrapAndContinue<bool>(asyncResult, _invokeActionTag);
        }

        protected internal virtual ActionResult EndInvokeActionMethod(IAsyncResult asyncResult) {
            return AsyncResultWrapper.UnwrapAndContinue<ActionResult>(asyncResult, _invokeActionMethodTag);
        }

        internal static IAsyncResult BeginInvokeActionMethodFilter(IActionFilter filter, ActionExecutingContext preContext, BeginInvokeCallback beginContinuation, AsyncCallback<ActionExecutedContext> endContinuation, AsyncCallback callback, object state) {
            filter.OnActionExecuting(preContext);
            if (preContext.Result != null) {
                ActionExecutedContext shortCircuitContext = new ActionExecutedContext(preContext, preContext.ActionDescriptor, true /* canceled */, null /* exception */) {
                    Result = preContext.Result
                };
                return new ObjectAsyncResult<ActionExecutedContext>(shortCircuitContext).ToAsyncResultWrapper(callback, state);
            }

            try {
                return AsyncResultWrapper.Wrap(callback, state, beginContinuation,
                    ar => BeginInvokeActionMethodFilterEndContinuation(filter, preContext, () => endContinuation(ar)));
            }
            catch (ThreadAbortException) {
                // This type of exception occurs as a result of Response.Redirect(), but we special-case so that
                // the filters don't see this as an error.
                ActionExecutedContext postContext = new ActionExecutedContext(preContext, preContext.ActionDescriptor, false /* canceled */, null /* exception */);
                filter.OnActionExecuted(postContext);
                throw;
            }
            catch (Exception ex) {
                ActionExecutedContext postContext = new ActionExecutedContext(preContext, preContext.ActionDescriptor, false /* canceled */, ex);
                filter.OnActionExecuted(postContext);
                if (!postContext.ExceptionHandled) {
                    throw;
                }

                return new ObjectAsyncResult<ActionExecutedContext>(postContext).ToAsyncResultWrapper(callback, state);
            }
        }

        internal static ActionExecutedContext BeginInvokeActionMethodFilterEndContinuation(IActionFilter filter, ActionExecutingContext preContext, Func<ActionExecutedContext> endContinuation) {
            bool wasError = false;
            ActionExecutedContext postContext = null;

            try {
                postContext = endContinuation();
            }
            catch (ThreadAbortException) {
                // This type of exception occurs as a result of Response.Redirect(), but we special-case so that
                // the filters don't see this as an error.
                postContext = new ActionExecutedContext(preContext, preContext.ActionDescriptor, false /* canceled */, null /* exception */);
                filter.OnActionExecuted(postContext);
                throw;
            }
            catch (Exception ex) {
                wasError = true;
                postContext = new ActionExecutedContext(preContext, preContext.ActionDescriptor, false /* canceled */, ex);
                filter.OnActionExecuted(postContext);
                if (!postContext.ExceptionHandled) {
                    throw;
                }
            }
            if (!wasError) {
                filter.OnActionExecuted(postContext);
            }
            return postContext;
        }

        private static IAsyncResult CreateInvokeActionContinuationAsyncResult(AsyncCallback callback, object state, Action continuation) {
            BeginInvokeCallback beginCallback = (innerCallback, innerState) => {
                ManualAsyncResult asyncResult = new ManualAsyncResult() { AsyncState = innerState };
                asyncResult.MarkCompleted(true /* completedSynchronously */, innerCallback);
                return asyncResult;
            };
            AsyncCallback<bool> endCallback = ar => {
                continuation();
                return true;
            };

            return AsyncResultWrapper.Wrap(callback, state, beginCallback, endCallback, _invokeActionTag);
        }

        private static IAsyncResult CreateInvokeActionNotFoundAsyncResult(AsyncCallback callback, object state) {
            BeginInvokeCallback beginCallback = (innerCallback, innerState) => {
                ManualAsyncResult asyncResult = new ManualAsyncResult() { AsyncState = innerState };
                asyncResult.MarkCompleted(true /* completedSynchronously */, innerCallback);
                return asyncResult;
            };
            AsyncCallback<bool> endCallback = ar => {
                return false;
            };

            return AsyncResultWrapper.Wrap(callback, state, beginCallback, endCallback, _invokeActionTag);
        }

        internal static ActionExecutedContext EndInvokeActionMethodFilter(IAsyncResult asyncResult) {
            return AsyncResultWrapper.UnwrapAndContinue<ActionExecutedContext>(asyncResult);
        }

        protected internal virtual IAsyncResult BeginInvokeActionMethodWithFilters(ControllerContext controllerContext, IList<IActionFilter> filters, ActionDescriptor actionDescriptor, IDictionary<string, object> parameters, AsyncCallback callback, object state) {
            ActionExecutingContext preContext = new ActionExecutingContext(controllerContext, actionDescriptor, parameters);

            // what makes this different from the synchronous version of this method is that we have to
            // aggregate both the begin + end delegates together. overall, though, it's the same logic.
            var continuation = new {
                Begin = (BeginInvokeCallback)((innerCallback, innerState) => BeginInvokeActionMethod(controllerContext, actionDescriptor, parameters, innerCallback, innerState)),
                End = (AsyncCallback<ActionExecutedContext>)(ar => new ActionExecutedContext(controllerContext, actionDescriptor, false /* canceled */, null /* exception */) {
                    Result = EndInvokeActionMethod(ar)
                })
            };

            // need to reverse the filter list because the continuations are built up backward
            var invocation = filters.Reverse().Aggregate(continuation,
                (next, filter) => new {
                    Begin = (BeginInvokeCallback)((innerCallback, innerState) => BeginInvokeActionMethodFilter(filter, preContext, next.Begin, next.End, innerCallback, innerState)),
                    End = (AsyncCallback<ActionExecutedContext>)EndInvokeActionMethodFilter
                });

            return AsyncResultWrapper.Wrap(callback, state, invocation.Begin, invocation.End, _invokeActionMethodWithFiltersTag);
        }

        protected internal virtual ActionExecutedContext EndInvokeActionMethodWithFilters(IAsyncResult asyncResult) {
            return AsyncResultWrapper.UnwrapAndContinue<ActionExecutedContext>(asyncResult, _invokeActionMethodWithFiltersTag);
        }

        protected override ControllerDescriptor GetControllerDescriptor(ControllerContext controllerContext) {
            Type controllerType = controllerContext.Controller.GetType();
            ControllerDescriptor controllerDescriptor = DescriptorCache.GetDescriptor(controllerType);
            return controllerDescriptor;
        }

        [SuppressMessage("Microsoft.Performance", "CA1804:RemoveUnusedLocals", MessageId = "rawUrl",
            Justification = "We only care about the property getter's side effects, not the returned value.")]
        private static void ValidateRequest(HttpRequestBase request) {
            // DevDiv 214040: Enable Request Validation by default for all controller requests
            // 
            // Note that we grab the Request's RawUrl to force it to be validated. Calling ValidateInput()
            // doesn't actually validate anything. It just sets flags indicating that on the next usage of
            // certain inputs that they should be validated. We special case RawUrl because the URL has already
            // been consumed by routing and thus might contain dangerous data. By forcing the RawUrl to be
            // re-read we're making sure that it gets validated by ASP.NET.

            request.ValidateInput();
            string rawUrl = request.RawUrl;
        }

    }
}
