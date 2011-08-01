namespace Microsoft.Web.Mvc.Test {
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.TestUtil;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.Web.Mvc;
    using Moq;

    [TestClass]
    public class AsyncControllerActionInvokerTest {

        [TestMethod]
        public void BeginInvokeActionEndContinuationWithHandledException() {
            // Arrange
            ActionResult result = new ViewResult();
            InvalidOperationException ex = new InvalidOperationException("Some exception text.");
            ControllerContext controllerContext = new ControllerContext();
            List<IExceptionFilter> exFilters = new List<IExceptionFilter>();

            Mock<AsyncControllerActionInvokerHelper> mockHelper = new Mock<AsyncControllerActionInvokerHelper>() { CallBase = true };
            mockHelper.Expect(h => h.PublicInvokeExceptionFilters(controllerContext, exFilters, ex)).Returns(new ExceptionContext() { ExceptionHandled = true, Result = result }).Verifiable();
            mockHelper.Expect(h => h.PublicInvokeActionResult(controllerContext, result)).Verifiable();
            AsyncControllerActionInvokerHelper helper = mockHelper.Object;

            // Act
            helper.BeginInvokeActionEndContinuation(controllerContext, exFilters, () => { throw ex; });

            // Assert
            mockHelper.Verify();
        }

        [TestMethod]
        public void BeginInvokeActionEndContinuationWithNormalControlFlow() {
            // Arrange
            bool wasCalled = false;
            AsyncControllerActionInvoker invoker = new AsyncControllerActionInvoker();

            // Act
            invoker.BeginInvokeActionEndContinuation(null, null, () => { wasCalled = true; });

            // Assert
            Assert.IsTrue(wasCalled);
        }

        [TestMethod]
        public void BeginInvokeActionEndContinuationWithThreadAbortException() {
            // Arrange
            ThreadAbortException ex = (ThreadAbortException)Activator.CreateInstance(typeof(ThreadAbortException), true /* nonPublic */);
            AsyncControllerActionInvoker invoker = new AsyncControllerActionInvoker();

            // Act & assert
            ExceptionHelper.ExpectException<ThreadAbortException>(
                delegate {
                    invoker.BeginInvokeActionEndContinuation(null, null, () => { throw ex; });
                });
        }

        [TestMethod]
        public void BeginInvokeActionEndContinuationWithUnhandledException() {
            // Arrange
            InvalidOperationException ex = new InvalidOperationException("Some exception text.");
            ControllerContext controllerContext = new ControllerContext();
            List<IExceptionFilter> exFilters = new List<IExceptionFilter>();

            Mock<AsyncControllerActionInvokerHelper> mockHelper = new Mock<AsyncControllerActionInvokerHelper>() { CallBase = true };
            mockHelper.Expect(h => h.PublicInvokeExceptionFilters(controllerContext, exFilters, ex)).Returns(new ExceptionContext() { ExceptionHandled = false }).Verifiable();
            mockHelper.Expect(h => h.PublicInvokeActionResult(It.IsAny<ControllerContext>(), It.IsAny<ActionResult>())).Never();
            AsyncControllerActionInvokerHelper helper = mockHelper.Object;

            // Act & assert
            ExceptionHelper.ExpectException<InvalidOperationException>(
                delegate {
                    helper.BeginInvokeActionEndContinuation(controllerContext, exFilters, () => { throw ex; });
                },
                @"Some exception text.");

            mockHelper.Verify();
        }

        [TestMethod]
        public void GetControllerDescriptor() {
            // Arrange
            ControllerContext controllerContext = new ControllerContext() {
                Controller = new EmptyController()
            };

            AsyncControllerActionInvokerHelper helper = new AsyncControllerActionInvokerHelper();

            // Act
            ControllerDescriptor cd = helper.PublicGetControllerDescriptor(controllerContext);

            // Assert
            Assert.IsInstanceOfType(cd, typeof(ReflectedAsyncControllerDescriptor));
            Assert.AreEqual(typeof(EmptyController), cd.ControllerType);
        }

        [TestMethod]
        public void InvokeActionMethodFilterWhereBeginContinuationThrowsExceptionAndIsHandled() {
            // Arrange
            List<string> actions = new List<string>();
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            Exception exception = new Exception();
            ActionExecutingContext preContext = GetEmptyActionExecutingContext();

            ActionFilterImpl filter = new ActionFilterImpl() {
                OnActionExecutingImpl = delegate(ActionExecutingContext filterContext) {
                    actions.Add("OnActionExecuting");
                },
                OnActionExecutedImpl = delegate(ActionExecutedContext filterContext) {
                    actions.Add("OnActionExecuted");
                    Assert.AreEqual(exception, filterContext.Exception, "Exception did not match.");
                    Assert.AreEqual(preContext.ActionDescriptor, filterContext.ActionDescriptor, "Descriptor did not match.");
                    Assert.IsFalse(filterContext.ExceptionHandled);
                    filterContext.ExceptionHandled = true;
                }
            };

            BeginInvokeCallback beginContinuation = (innerCallback, innerState) => {
                actions.Add("BeginContinuation");
                throw exception;
            };

            AsyncCallback<ActionExecutedContext> endContinuation = ar => {
                Assert.Fail("Continuation should not be called.");
                return null;
            };

            // Act
            IAsyncResult asyncResult = AsyncControllerActionInvoker.BeginInvokeActionMethodFilter(filter, preContext, beginContinuation, endContinuation, null, null);
            ActionExecutedContext postContext = AsyncControllerActionInvoker.EndInvokeActionMethodFilter(asyncResult);

            // Assert
            Assert.AreEqual(3, actions.Count);
            Assert.AreEqual("OnActionExecuting", actions[0]);
            Assert.AreEqual("BeginContinuation", actions[1]);
            Assert.AreEqual("OnActionExecuted", actions[2]);
            Assert.AreEqual(exception, postContext.Exception, "Exception did not match.");
            Assert.AreEqual(preContext.ActionDescriptor, postContext.ActionDescriptor, "Descriptor did not match.");
            Assert.IsTrue(postContext.ExceptionHandled);
        }

        [TestMethod]
        public void InvokeActionMethodFilterWhereBeginContinuationThrowsExceptionAndIsNotHandled() {
            // Arrange
            List<string> actions = new List<string>();
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            Exception exception = new Exception();
            ActionExecutingContext preContext = GetEmptyActionExecutingContext();

            ActionFilterImpl filter = new ActionFilterImpl() {
                OnActionExecutingImpl = delegate(ActionExecutingContext filterContext) {
                    actions.Add("OnActionExecuting");
                },
                OnActionExecutedImpl = delegate(ActionExecutedContext filterContext) {
                    actions.Add("OnActionExecuted");
                    Assert.AreEqual(exception, filterContext.Exception, "Exception did not match.");
                    Assert.AreEqual(preContext.ActionDescriptor, filterContext.ActionDescriptor, "Descriptor did not match.");
                    Assert.IsFalse(filterContext.ExceptionHandled);
                }
            };

            BeginInvokeCallback beginContinuation = (innerCallback, innerState) => {
                actions.Add("BeginContinuation");
                throw exception;
            };

            AsyncCallback<ActionExecutedContext> endContinuation = ar => {
                Assert.Fail("Continuation should not be called.");
                return null;
            };

            // Act
            Exception thrownException = ExceptionHelper.ExpectException<Exception>(
                delegate {
                    AsyncControllerActionInvoker.BeginInvokeActionMethodFilter(filter, preContext, beginContinuation, endContinuation, null, null);
                });

            // Assert
            Assert.AreEqual(3, actions.Count);
            Assert.AreEqual("OnActionExecuting", actions[0]);
            Assert.AreEqual("BeginContinuation", actions[1]);
            Assert.AreEqual("OnActionExecuted", actions[2]);

            Assert.AreEqual(exception, thrownException);
        }

        [TestMethod]
        public void InvokeActionMethodFilterWhereBeginContinuationThrowsThreadAbortException() {
            // Arrange
            List<string> actions = new List<string>();
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            ActionExecutingContext preContext = GetEmptyActionExecutingContext();

            ActionFilterImpl filter = new ActionFilterImpl() {
                OnActionExecutingImpl = delegate(ActionExecutingContext filterContext) {
                    actions.Add("OnActionExecuting");
                },
                OnActionExecutedImpl = delegate(ActionExecutedContext filterContext) {
                    actions.Add("OnActionExecuted");
                    Thread.ResetAbort();
                    Assert.IsNull(filterContext.Exception, "Exception should not have shown up.");
                    Assert.AreEqual(preContext.ActionDescriptor, filterContext.ActionDescriptor, "Descriptor did not match.");
                    filterContext.ExceptionHandled = true;
                }
            };

            BeginInvokeCallback beginContinuation = (innerCallback, innerState) => {
                actions.Add("BeginContinuation");
                Thread.CurrentThread.Abort();
                return null;
            };

            AsyncCallback<ActionExecutedContext> endContinuation = ar => {
                Assert.Fail("Continuation should not be called.");
                return null;
            };

            // Act
            ExceptionHelper.ExpectException<ThreadAbortException>(
                delegate {
                    AsyncControllerActionInvoker.BeginInvokeActionMethodFilter(filter, preContext, beginContinuation, endContinuation, null, null);
                },
                "Thread was being aborted.");

            // Assert
            Assert.AreEqual(3, actions.Count);
            Assert.AreEqual("OnActionExecuting", actions[0]);
            Assert.AreEqual("BeginContinuation", actions[1]);
            Assert.AreEqual("OnActionExecuted", actions[2]);
        }

        [TestMethod]
        public void InvokeActionMethodFilterWhereEndContinuationThrowsExceptionAndIsHandled() {
            // Arrange
            List<string> actions = new List<string>();
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            Exception exception = new Exception();
            ActionExecutingContext preContext = GetEmptyActionExecutingContext();
            MockAsyncResult asyncResult = new MockAsyncResult();

            ActionFilterImpl filter = new ActionFilterImpl() {
                OnActionExecutingImpl = delegate(ActionExecutingContext filterContext) {
                    actions.Add("OnActionExecuting");
                },
                OnActionExecutedImpl = delegate(ActionExecutedContext filterContext) {
                    actions.Add("OnActionExecuted");
                    Assert.AreEqual(exception, filterContext.Exception, "Exception did not match.");
                    Assert.AreEqual(preContext.ActionDescriptor, filterContext.ActionDescriptor, "Descriptor did not match.");
                    Assert.IsFalse(filterContext.ExceptionHandled);
                    filterContext.ExceptionHandled = true;
                }
            };

            BeginInvokeCallback beginContinuation = (innerCallback, innerState) => {
                actions.Add("BeginContinuation");
                return asyncResult;
            };

            AsyncCallback<ActionExecutedContext> endContinuation = ar => {
                Assert.AreEqual(asyncResult, ar);
                actions.Add("EndContinuation");
                throw exception;
            };

            // Act
            IAsyncResult returnedResult = AsyncControllerActionInvoker.BeginInvokeActionMethodFilter(filter, preContext, beginContinuation, endContinuation, null, null);
            ActionExecutedContext postContext = AsyncControllerActionInvoker.EndInvokeActionMethodFilter(returnedResult);

            // Assert
            Assert.AreEqual(4, actions.Count);
            Assert.AreEqual("OnActionExecuting", actions[0]);
            Assert.AreEqual("BeginContinuation", actions[1]);
            Assert.AreEqual("EndContinuation", actions[2]);
            Assert.AreEqual("OnActionExecuted", actions[3]);
            Assert.AreEqual(exception, postContext.Exception, "Exception did not match.");
            Assert.AreEqual(preContext.ActionDescriptor, postContext.ActionDescriptor, "Descriptor did not match.");
            Assert.IsTrue(postContext.ExceptionHandled);
        }

        [TestMethod]
        public void InvokeActionMethodFilterWhereEndContinuationThrowsExceptionAndIsNotHandled() {
            // Arrange
            List<string> actions = new List<string>();
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            Exception exception = new Exception();
            ActionExecutingContext preContext = GetEmptyActionExecutingContext();
            MockAsyncResult asyncResult = new MockAsyncResult();

            ActionFilterImpl filter = new ActionFilterImpl() {
                OnActionExecutingImpl = delegate(ActionExecutingContext filterContext) {
                    actions.Add("OnActionExecuting");
                },
                OnActionExecutedImpl = delegate(ActionExecutedContext filterContext) {
                    actions.Add("OnActionExecuted");
                    Assert.AreEqual(exception, filterContext.Exception, "Exception did not match.");
                    Assert.AreEqual(preContext.ActionDescriptor, filterContext.ActionDescriptor, "Descriptor did not match.");
                    Assert.IsFalse(filterContext.ExceptionHandled);
                }
            };

            BeginInvokeCallback beginContinuation = (innerCallback, innerState) => {
                actions.Add("BeginContinuation");
                return asyncResult;
            };

            AsyncCallback<ActionExecutedContext> endContinuation = ar => {
                Assert.AreEqual(asyncResult, ar);
                actions.Add("EndContinuation");
                throw exception;
            };

            // Act
            IAsyncResult returnedResult = AsyncControllerActionInvoker.BeginInvokeActionMethodFilter(filter, preContext, beginContinuation, endContinuation, null, null);
            Exception thrownException = ExceptionHelper.ExpectException<Exception>(
                delegate {
                    AsyncControllerActionInvoker.EndInvokeActionMethodFilter(returnedResult);
                });

            // Assert
            Assert.AreEqual(4, actions.Count);
            Assert.AreEqual("OnActionExecuting", actions[0]);
            Assert.AreEqual("BeginContinuation", actions[1]);
            Assert.AreEqual("EndContinuation", actions[2]);
            Assert.AreEqual("OnActionExecuted", actions[3]);

            Assert.AreEqual(exception, thrownException);
        }

        [TestMethod]
        public void InvokeActionMethodFilterWhereEndContinuationThrowsThreadAbortException() {
            // Arrange
            List<string> actions = new List<string>();
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            ActionExecutingContext preContext = GetEmptyActionExecutingContext();
            MockAsyncResult asyncResult = new MockAsyncResult();

            ActionFilterImpl filter = new ActionFilterImpl() {
                OnActionExecutingImpl = delegate(ActionExecutingContext filterContext) {
                    actions.Add("OnActionExecuting");
                },
                OnActionExecutedImpl = delegate(ActionExecutedContext filterContext) {
                    actions.Add("OnActionExecuted");
                    Thread.ResetAbort();
                    Assert.IsNull(filterContext.Exception, "Exception should not have shown up.");
                    Assert.AreEqual(preContext.ActionDescriptor, filterContext.ActionDescriptor, "Descriptor did not match.");
                    filterContext.ExceptionHandled = true;
                }
            };

            BeginInvokeCallback beginContinuation = (innerCallback, innerState) => {
                actions.Add("BeginContinuation");
                return asyncResult;
            };

            AsyncCallback<ActionExecutedContext> endContinuation = ar => {
                Assert.AreEqual(asyncResult, ar);
                actions.Add("EndContinuation");
                Thread.CurrentThread.Abort();
                return null;
            };

            // Act
            // Act
            IAsyncResult returnedResult = AsyncControllerActionInvoker.BeginInvokeActionMethodFilter(filter, preContext, beginContinuation, endContinuation, null, null);
            ExceptionHelper.ExpectException<ThreadAbortException>(
                delegate {
                    AsyncControllerActionInvoker.EndInvokeActionMethodFilter(returnedResult);
                },
                "Thread was being aborted.");

            // Assert
            Assert.AreEqual(4, actions.Count);
            Assert.AreEqual("OnActionExecuting", actions[0]);
            Assert.AreEqual("BeginContinuation", actions[1]);
            Assert.AreEqual("EndContinuation", actions[2]);
            Assert.AreEqual("OnActionExecuted", actions[3]);
        }

        [TestMethod]
        public void InvokeActionMethodFilterWhereOnActionExecutingCancels() {
            // Arrange
            bool wasCalled = false;
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            ActionResult actionResult = new EmptyResult();

            ActionExecutingContext preContext = GetEmptyActionExecutingContext();

            ActionFilterImpl filter = new ActionFilterImpl() {
                OnActionExecutingImpl = delegate(ActionExecutingContext filterContext) {
                    Assert.IsFalse(wasCalled);
                    wasCalled = true;
                    filterContext.Result = actionResult;
                },
            };

            BeginInvokeCallback beginContinuation = (innerCallback, innerState) => {
                Assert.Fail("The continuation should not be called.");
                return null;
            };

            AsyncCallback<ActionExecutedContext> endContinuation = ar => {
                Assert.Fail("The continuation should not be called.");
                return null;
            };

            // Act
            IAsyncResult asyncResult = AsyncControllerActionInvoker.BeginInvokeActionMethodFilter(filter, preContext, beginContinuation, endContinuation, null, null);
            ActionExecutedContext postContext = AsyncControllerActionInvoker.EndInvokeActionMethodFilter(asyncResult);

            // Assert
            Assert.IsTrue(wasCalled);
            Assert.IsNull(postContext.Exception);
            Assert.IsTrue(postContext.Canceled);
            Assert.AreEqual(actionResult, postContext.Result, "Result was incorrect.");
            Assert.AreEqual(preContext.ActionDescriptor, postContext.ActionDescriptor, "Descriptor was incorrect.");
        }

        [TestMethod]
        public void InvokeActionMethodFilterWithNormalControlFlow() {
            // Arrange
            List<string> actions = new List<string>();
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            ActionDescriptor action = new Mock<ActionDescriptor>().Object;
            MockAsyncResult asyncResult = new MockAsyncResult();
            object state = new object();

            ActionExecutingContext preContext = new Mock<ActionExecutingContext>().Object;
            ActionExecutedContext postContext = new Mock<ActionExecutedContext>().Object;

            ActionFilterImpl filter = new ActionFilterImpl() {
                OnActionExecutingImpl = delegate(ActionExecutingContext filterContext) {
                    Assert.AreEqual(preContext, filterContext);
                    Assert.IsNull(filterContext.Result);
                    actions.Add("OnActionExecuting");
                },
                OnActionExecutedImpl = delegate(ActionExecutedContext filterContext) {
                    Assert.AreEqual(postContext, filterContext);
                    actions.Add("OnActionExecuted");
                }
            };

            BeginInvokeCallback beginContinuation = (innerCallback, innerState) => {
                actions.Add("BeginContinuation");
                return asyncResult;
            };

            AsyncCallback<ActionExecutedContext> endContinuation = ar => {
                Assert.AreEqual(asyncResult, ar);
                actions.Add("EndContinuation");
                return postContext;
            };

            // Act
            IAsyncResult returnedAsyncResult = AsyncControllerActionInvoker.BeginInvokeActionMethodFilter(filter, preContext, beginContinuation, endContinuation, null, state);
            ActionExecutedContext returnedPostContext = AsyncControllerActionInvoker.EndInvokeActionMethodFilter(returnedAsyncResult);

            // Assert
            Assert.AreEqual(4, actions.Count);
            Assert.AreEqual("OnActionExecuting", actions[0]);
            Assert.AreEqual("BeginContinuation", actions[1]);
            Assert.AreEqual("EndContinuation", actions[2]);
            Assert.AreEqual("OnActionExecuted", actions[3]);

            Assert.AreEqual(state, returnedAsyncResult.AsyncState);
            Assert.AreEqual(postContext, returnedPostContext);
        }

        [TestMethod]
        public void InvokeActionMethodWithAsynchronousDescriptor() {
            // Arrange
            ControllerContext controllerContext = new Mock<ControllerContext>().Object;
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            ActionResult expectedResult = new ViewResult();

            IAsyncResult innerResult = new MockAsyncResult();

            Mock<ActionDescriptor> mockDescriptor = new Mock<ActionDescriptor>();
            IMock<IAsyncActionDescriptor> mockAsyncDescriptor = mockDescriptor.As<IAsyncActionDescriptor>();
            mockAsyncDescriptor.Expect(d => d.BeginExecute(controllerContext, parameters, It.IsAny<AsyncCallback>(), It.IsAny<object>())).Returns(innerResult);
            mockAsyncDescriptor.Expect(d => d.EndExecute(innerResult)).Returns(expectedResult);
            ActionDescriptor descriptor = mockDescriptor.Object;

            AsyncControllerActionInvoker invoker = new AsyncControllerActionInvoker();

            // Act
            IAsyncResult asyncResult = invoker.BeginInvokeActionMethod(controllerContext, descriptor, parameters, null, null);
            ActionResult returnedResult = invoker.EndInvokeActionMethod(asyncResult);

            // Assert
            Assert.AreEqual(expectedResult, returnedResult);
        }

        [TestMethod]
        public void InvokeActionMethodWithSynchronousDescriptor() {
            // Arrange
            ControllerContext controllerContext = new Mock<ControllerContext>().Object;
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            ActionResult expectedResult = new ViewResult();

            Mock<ActionDescriptor> mockDescriptor = new Mock<ActionDescriptor>();
            mockDescriptor.Expect(d => d.Execute(controllerContext, parameters)).Returns(expectedResult);
            ActionDescriptor descriptor = mockDescriptor.Object;

            SignalContainer<IAsyncResult> callbackContainer = new SignalContainer<IAsyncResult>();
            AsyncCallback callback = ar => {
                callbackContainer.Signal(ar);
            };

            object state = new object();

            AsyncControllerActionInvoker invoker = new AsyncControllerActionInvoker();

            // Act
            IAsyncResult asyncResult = invoker.BeginInvokeActionMethod(controllerContext, descriptor, parameters, callback, state);
            IAsyncResult passedToCallback = callbackContainer.Wait();
            ActionResult returnedResult = invoker.EndInvokeActionMethod(asyncResult);

            // Assert
            Assert.AreEqual(asyncResult, passedToCallback, "Returned IAsyncResult and IAsyncResult provided to callback were different.");
            Assert.AreEqual(state, asyncResult.AsyncState);
            Assert.AreEqual(expectedResult, returnedResult);
        }

        [TestMethod]
        public void InvokeActionMethodWithFiltersOrdersFiltersCorrectly() {
            // Arrange
            List<string> actions = new List<string>();
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            MockAsyncResult asyncResult = new MockAsyncResult();
            ActionResult actionResult = new ViewResult();

            ActionFilterImpl filter1 = new ActionFilterImpl() {
                OnActionExecutingImpl = delegate(ActionExecutingContext filterContext) {
                    actions.Add("OnActionExecuting1");
                },
                OnActionExecutedImpl = delegate(ActionExecutedContext filterContext) {
                    actions.Add("OnActionExecuted1");
                }
            };
            ActionFilterImpl filter2 = new ActionFilterImpl() {
                OnActionExecutingImpl = delegate(ActionExecutingContext filterContext) {
                    actions.Add("OnActionExecuting2");
                },
                OnActionExecutedImpl = delegate(ActionExecutedContext filterContext) {
                    actions.Add("OnActionExecuted2");
                }
            };

            ControllerContext controllerContext = new ControllerContext() { Controller = new Mock<Controller>().Object };
            ActionDescriptor ad = new Mock<ActionDescriptor>().Object;

            Mock<AsyncControllerActionInvoker> mockInvoker = new Mock<AsyncControllerActionInvoker>() { CallBase = true };
            mockInvoker.Expect(i => i.BeginInvokeActionMethod(controllerContext, ad, parameters, It.IsAny<AsyncCallback>(), It.IsAny<object>())).Returns(asyncResult);
            mockInvoker.Expect(i => i.EndInvokeActionMethod(asyncResult)).Returns(actionResult);

            AsyncControllerActionInvoker invoker = mockInvoker.Object;
            List<IActionFilter> filters = new List<IActionFilter>() { filter1, filter2 };

            // Act
            IAsyncResult returnedAsyncResult = invoker.BeginInvokeActionMethodWithFilters(controllerContext, filters, ad, parameters, null, null);
            ActionExecutedContext returnedPostContext = invoker.EndInvokeActionMethodWithFilters(returnedAsyncResult);

            // Assert
            Assert.AreEqual(4, actions.Count);
            Assert.AreEqual("OnActionExecuting1", actions[0]);
            Assert.AreEqual("OnActionExecuting2", actions[1]);
            Assert.AreEqual("OnActionExecuted2", actions[2]);
            Assert.AreEqual("OnActionExecuted1", actions[3]);

            Assert.AreEqual(actionResult, returnedPostContext.Result);
            Assert.IsNull(returnedPostContext.Exception);
        }

        [TestMethod]
        public void InvokeActionReturnsFalseIfActionNotFound() {
            // Arrange
            AsyncControllerActionInvokerHelper helper = new AsyncControllerActionInvokerHelper();
            ControllerContext controllerContext = new ControllerContext() {
                Controller = new EmptyController()
            };

            // Act
            IAsyncResult asyncResult = helper.BeginInvokeAction(controllerContext, "ActionNotFound", null, null);
            bool retVal = helper.EndInvokeAction(asyncResult);

            // Assert
            Assert.IsFalse(retVal);
        }

        [TestMethod]
        public void InvokeActionThrowsIfActionNameIsEmpty() {
            // Arrange
            AsyncControllerActionInvoker invoker = new AsyncControllerActionInvoker();
            ControllerContext controllerContext = new ControllerContext();

            // Act & assert
            ExceptionHelper.ExpectArgumentExceptionNullOrEmpty(
                delegate {
                    invoker.BeginInvokeAction(controllerContext, String.Empty, null, null);
                }, "actionName");
        }

        [TestMethod]
        public void InvokeActionThrowsIfActionNameIsNull() {
            // Arrange
            AsyncControllerActionInvoker invoker = new AsyncControllerActionInvoker();
            ControllerContext controllerContext = new ControllerContext();

            // Act & assert
            ExceptionHelper.ExpectArgumentExceptionNullOrEmpty(
                delegate {
                    invoker.BeginInvokeAction(controllerContext, null, null, null);
                }, "actionName");
        }

        [TestMethod]
        public void InvokeActionThrowsIfBeginInvokeActionMethodThrowsThreadAbortException() {
            // Arrange
            ActionResult actionResult = new ViewResult();
            FilterInfo filters = new FilterInfo();
            ControllerDescriptor cd = new Mock<ControllerDescriptor>().Object;
            ActionDescriptor ad = new Mock<ActionDescriptor>().Object;
            Dictionary<string, object> parameterValues = new Dictionary<string, object>();
            ThreadAbortException ex = (ThreadAbortException)Activator.CreateInstance(typeof(ThreadAbortException), true /* nonPublic */);
            ControllerContext controllerContext = new ControllerContext() {
                Controller = new EmptyController() { ValidateRequest = false }
            };

            Mock<AsyncControllerActionInvokerHelper> mockHelper = new Mock<AsyncControllerActionInvokerHelper>() { CallBase = true };
            mockHelper.Expect(h => h.PublicGetControllerDescriptor(controllerContext)).Returns(cd).Verifiable();
            mockHelper.Expect(h => h.PublicFindAction(controllerContext, cd, "SomeAction")).Returns(ad).Verifiable();
            mockHelper.Expect(h => h.PublicGetFilters(controllerContext, ad)).Returns(filters).Verifiable();
            mockHelper.Expect(h => h.PublicInvokeAuthorizationFilters(controllerContext, filters.AuthorizationFilters, ad)).Returns(new AuthorizationContext()).Verifiable();
            mockHelper.Expect(h => h.PublicGetParameterValues(controllerContext, ad)).Returns(parameterValues).Verifiable();
            mockHelper.Expect(h => h.BeginInvokeActionMethodWithFilters(controllerContext, filters.ActionFilters, ad, parameterValues, It.IsAny<AsyncCallback>(), It.IsAny<object>())).Throws(ex).Verifiable();
            AsyncControllerActionInvokerHelper helper = mockHelper.Object;

            // Act & assert
            ExceptionHelper.ExpectException<ThreadAbortException>(
                delegate {
                    helper.BeginInvokeAction(controllerContext, "SomeAction", null, null);
                });

            mockHelper.Verify();
        }

        [TestMethod]
        public void InvokeActionThrowsIfBeginInvokeActionMethodThrowsUnhandledException() {
            // Arrange
            ActionResult actionResult = new ViewResult();
            FilterInfo filters = new FilterInfo();
            ControllerDescriptor cd = new Mock<ControllerDescriptor>().Object;
            ActionDescriptor ad = new Mock<ActionDescriptor>().Object;
            Dictionary<string,object> parameterValues = new Dictionary<string,object>();
            InvalidOperationException ex = new InvalidOperationException("Some exception text.");
            ControllerContext controllerContext = new ControllerContext() {
                Controller = new EmptyController() { ValidateRequest = false }
            };

            Mock<AsyncControllerActionInvokerHelper> mockHelper = new Mock<AsyncControllerActionInvokerHelper>() { CallBase = true };
            mockHelper.Expect(h => h.PublicGetControllerDescriptor(controllerContext)).Returns(cd).Verifiable();
            mockHelper.Expect(h => h.PublicFindAction(controllerContext, cd, "SomeAction")).Returns(ad).Verifiable();
            mockHelper.Expect(h => h.PublicGetFilters(controllerContext, ad)).Returns(filters).Verifiable();
            mockHelper.Expect(h => h.PublicInvokeAuthorizationFilters(controllerContext, filters.AuthorizationFilters, ad)).Returns(new AuthorizationContext()).Verifiable();
            mockHelper.Expect(h => h.PublicGetParameterValues(controllerContext, ad)).Returns(parameterValues).Verifiable();
            mockHelper.Expect(h => h.BeginInvokeActionMethodWithFilters(controllerContext, filters.ActionFilters, ad, parameterValues, It.IsAny<AsyncCallback>(), It.IsAny<object>())).Throws(ex).Verifiable();
            mockHelper.Expect(h => h.PublicInvokeExceptionFilters(controllerContext, filters.ExceptionFilters, ex)).Returns(new ExceptionContext() { ExceptionHandled = false }).Verifiable();
            AsyncControllerActionInvokerHelper helper = mockHelper.Object;

            // Act & assert
            ExceptionHelper.ExpectException<InvalidOperationException>(
                delegate {
                    helper.BeginInvokeAction(controllerContext, "SomeAction", null, null);
                },
                @"Some exception text.");

            mockHelper.Verify();
        }

        [TestMethod]
        public void InvokeActionThrowsIfControllerContextIsNull() {
            // Arrange
            AsyncControllerActionInvoker invoker = new AsyncControllerActionInvoker();

            // Act & assert
            ExceptionHelper.ExpectArgumentNullException(
                delegate {
                    invoker.BeginInvokeAction(null, null, null, null);
                }, "controllerContext");
        }

        [TestMethod]
        public void InvokeActionThrowsIfRequestIsInvalid() {
            // Arrange
            ActionResult actionResult = new ViewResult();
            FilterInfo filters = new FilterInfo();
            ControllerDescriptor cd = new Mock<ControllerDescriptor>().Object;
            ActionDescriptor ad = new Mock<ActionDescriptor>().Object;
            HttpRequestValidationException ex = new HttpRequestValidationException();

            Mock<ControllerContext> mockControllerContext = new Mock<ControllerContext>();
            mockControllerContext.Expect(c => c.HttpContext.Request.RawUrl).Throws(ex);
            mockControllerContext.Expect(c => c.Controller).Returns(new EmptyController() { ValidateRequest = true });
            ControllerContext controllerContext = mockControllerContext.Object;

            Mock<AsyncControllerActionInvokerHelper> mockHelper = new Mock<AsyncControllerActionInvokerHelper>() { CallBase = true };
            mockHelper.Expect(h => h.PublicGetControllerDescriptor(controllerContext)).Returns(cd).Verifiable();
            mockHelper.Expect(h => h.PublicFindAction(controllerContext, cd, "SomeAction")).Returns(ad).Verifiable();
            mockHelper.Expect(h => h.PublicGetFilters(controllerContext, ad)).Returns(filters).Verifiable();
            mockHelper.Expect(h => h.PublicInvokeAuthorizationFilters(controllerContext, filters.AuthorizationFilters, ad)).Returns(new AuthorizationContext()).Verifiable();
            mockHelper.Expect(h => h.PublicInvokeExceptionFilters(controllerContext, filters.ExceptionFilters, ex)).Returns(new ExceptionContext() { ExceptionHandled = false }).Verifiable();
            AsyncControllerActionInvokerHelper helper = mockHelper.Object;

            // Act & assert
            ExceptionHelper.ExpectException<HttpRequestValidationException>(
                delegate {
                    helper.BeginInvokeAction(controllerContext, "SomeAction", null, null);
                });

            mockHelper.Verify();
        }

        [TestMethod]
        public void InvokeActionWithAuthorizationFilterWhichShortCircuitsResult() {
            // Arrange
            ActionResult actionResult = new ViewResult();
            FilterInfo filters = new FilterInfo();
            ControllerDescriptor cd = new Mock<ControllerDescriptor>().Object;
            ActionDescriptor ad = new Mock<ActionDescriptor>().Object;
            ControllerContext controllerContext = new ControllerContext();

            bool invokeActionResultWasCalled = false;

            Mock<AsyncControllerActionInvokerHelper> mockHelper = new Mock<AsyncControllerActionInvokerHelper>() { CallBase = true };
            mockHelper.Expect(h => h.PublicGetControllerDescriptor(controllerContext)).Returns(cd).Verifiable();
            mockHelper.Expect(h => h.PublicFindAction(controllerContext, cd, "SomeAction")).Returns(ad).Verifiable();
            mockHelper.Expect(h => h.PublicGetFilters(controllerContext, ad)).Returns(filters).Verifiable();
            mockHelper.Expect(h => h.PublicInvokeAuthorizationFilters(controllerContext, filters.AuthorizationFilters, ad)).Returns(new AuthorizationContext() { Result = actionResult }).Verifiable();
            mockHelper
                .Expect(h => h.PublicInvokeActionResult(controllerContext, actionResult))
                .Callback(() => { invokeActionResultWasCalled = true; })
                .Verifiable();

            AsyncControllerActionInvokerHelper helper = mockHelper.Object;

            // Act & assert
            IAsyncResult asyncResult = helper.BeginInvokeAction(controllerContext, "SomeAction", null, null);
            Assert.IsFalse(invokeActionResultWasCalled, "InvokeActionResult() should not have been called yet.");
            bool retVal = helper.EndInvokeAction(asyncResult);

            Assert.IsTrue(retVal);
            mockHelper.Verify();
        }

        [TestMethod]
        public void InvokeActionWithNormalControlFlow() {
            // Arrange
            ActionResult actionResult = new ViewResult();
            FilterInfo filters = new FilterInfo();
            ControllerDescriptor cd = new Mock<ControllerDescriptor>().Object;
            ActionDescriptor ad = new Mock<ActionDescriptor>().Object;
            Dictionary<string, object> parameterValues = new Dictionary<string, object>();
            ControllerContext controllerContext = new ControllerContext() {
                Controller = new EmptyController() { ValidateRequest = false }
            };

            MockAsyncResult asyncResult = new MockAsyncResult();

            bool invokeActionResultWasCalled = false;

            Mock<AsyncControllerActionInvokerHelper> mockHelper = new Mock<AsyncControllerActionInvokerHelper>() { CallBase = true };
            mockHelper.Expect(h => h.PublicGetControllerDescriptor(controllerContext)).Returns(cd).Verifiable();
            mockHelper.Expect(h => h.PublicFindAction(controllerContext, cd, "SomeAction")).Returns(ad).Verifiable();
            mockHelper.Expect(h => h.PublicGetFilters(controllerContext, ad)).Returns(filters).Verifiable();
            mockHelper.Expect(h => h.PublicInvokeAuthorizationFilters(controllerContext, filters.AuthorizationFilters, ad)).Returns(new AuthorizationContext()).Verifiable();
            mockHelper.Expect(h => h.PublicGetParameterValues(controllerContext, ad)).Returns(parameterValues).Verifiable();
            mockHelper.Expect(h => h.BeginInvokeActionMethodWithFilters(controllerContext, filters.ActionFilters, ad, parameterValues, It.IsAny<AsyncCallback>(), It.IsAny<object>())).Returns(asyncResult).Verifiable();
            mockHelper.Expect(h => h.EndInvokeActionMethodWithFilters(asyncResult)).Returns(new ActionExecutedContext() { Result = actionResult }).Verifiable();
            mockHelper
                .Expect(h => h.PublicInvokeActionResult(controllerContext, actionResult))
                .Callback(() => { invokeActionResultWasCalled = true; })
                .Verifiable();

            AsyncControllerActionInvokerHelper helper = mockHelper.Object;

            // Act & assert
            IAsyncResult returnedAsyncResult = helper.BeginInvokeAction(controllerContext, "SomeAction", null, null);
            Assert.IsFalse(invokeActionResultWasCalled, "InvokeActionResult() should not yet have been called.");
            bool retVal = helper.EndInvokeAction(returnedAsyncResult);

            Assert.IsTrue(retVal);
            mockHelper.Verify();
        }

        [TestMethod]
        public void InvokeActionWhereBeginInvokeActionMethodThrowsExceptionAndIsHandled() {
            // Arrange
            ActionResult actionResult = new ViewResult();
            FilterInfo filters = new FilterInfo();
            ControllerDescriptor cd = new Mock<ControllerDescriptor>().Object;
            ActionDescriptor ad = new Mock<ActionDescriptor>().Object;
            Dictionary<string, object> parameterValues = new Dictionary<string, object>();
            InvalidOperationException ex = new InvalidOperationException("Some exception text.");
            ControllerContext controllerContext = new ControllerContext() {
                Controller = new EmptyController() { ValidateRequest = false }
            };

            bool invokeActionResultWasCalled = false;

            Mock<AsyncControllerActionInvokerHelper> mockHelper = new Mock<AsyncControllerActionInvokerHelper>() { CallBase = true };
            mockHelper.Expect(h => h.PublicGetControllerDescriptor(controllerContext)).Returns(cd).Verifiable();
            mockHelper.Expect(h => h.PublicFindAction(controllerContext, cd, "SomeAction")).Returns(ad).Verifiable();
            mockHelper.Expect(h => h.PublicGetFilters(controllerContext, ad)).Returns(filters).Verifiable();
            mockHelper.Expect(h => h.PublicInvokeAuthorizationFilters(controllerContext, filters.AuthorizationFilters, ad)).Returns(new AuthorizationContext()).Verifiable();
            mockHelper.Expect(h => h.PublicGetParameterValues(controllerContext, ad)).Returns(parameterValues).Verifiable();
            mockHelper.Expect(h => h.BeginInvokeActionMethodWithFilters(controllerContext, filters.ActionFilters, ad, parameterValues, It.IsAny<AsyncCallback>(), It.IsAny<object>())).Throws(ex).Verifiable();
            mockHelper.Expect(h => h.PublicInvokeExceptionFilters(controllerContext, filters.ExceptionFilters, ex)).Returns(new ExceptionContext() { ExceptionHandled = true, Result = actionResult }).Verifiable();
            mockHelper
                .Expect(h => h.PublicInvokeActionResult(controllerContext, actionResult))
                .Callback(() => { invokeActionResultWasCalled = true; })
                .Verifiable();

            AsyncControllerActionInvokerHelper helper = mockHelper.Object;

            // Act & assert
            IAsyncResult asyncResult = helper.BeginInvokeAction(controllerContext, "SomeAction", null, null);
            Assert.IsFalse(invokeActionResultWasCalled, "InvokeActionResult() should not yet have been called.");
            bool retVal = helper.EndInvokeAction(asyncResult);

            Assert.IsTrue(retVal);
            mockHelper.Verify();
        }

        [TestMethod]
        public void SynchronizationContextPropertyExplicitlySetByConstructor() {
            // Arrange
            SynchronizationContext syncContext = new SynchronizationContext();

            // Act
            AsyncControllerActionInvoker invoker = new AsyncControllerActionInvoker(syncContext);

            // Assert
            Assert.AreEqual(syncContext, invoker.SynchronizationContext);
        }

        [TestMethod]
        public void SynchronizationContextPropertyHasDefaultValue() {
            // Act
            AsyncControllerActionInvoker invoker = new AsyncControllerActionInvoker();

            // Assert
            Assert.IsNotNull(invoker.SynchronizationContext);
        }

        private static ActionExecutingContext GetEmptyActionExecutingContext() {
            ControllerContext controllerContext = new ControllerContext() {
                Controller = new Mock<Controller>().Object
            };
            return new ActionExecutingContext(controllerContext, new Mock<ActionDescriptor>().Object, new Dictionary<string, object>());
        }

        private class ActionFilterImpl : IActionFilter, IResultFilter {

            public Action<ActionExecutingContext> OnActionExecutingImpl {
                get;
                set;
            }

            public void OnActionExecuting(ActionExecutingContext filterContext) {
                OnActionExecutingImpl(filterContext);
            }

            public Action<ActionExecutedContext> OnActionExecutedImpl {
                get;
                set;
            }

            public void OnActionExecuted(ActionExecutedContext filterContext) {
                OnActionExecutedImpl(filterContext);
            }

            public Action<ResultExecutingContext> OnResultExecutingImpl {
                get;
                set;
            }

            public void OnResultExecuting(ResultExecutingContext filterContext) {
                OnResultExecutingImpl(filterContext);
            }

            public Action<ResultExecutedContext> OnResultExecutedImpl {
                get;
                set;
            }

            public void OnResultExecuted(ResultExecutedContext filterContext) {
                OnResultExecutedImpl(filterContext);
            }

        }

        public class AsyncControllerActionInvokerHelper : AsyncControllerActionInvoker {

            public AsyncControllerActionInvokerHelper() {
                DescriptorCache = new AsyncControllerDescriptorCache();
            }

            protected override ControllerDescriptor GetControllerDescriptor(ControllerContext controllerContext) {
                return PublicGetControllerDescriptor(controllerContext);
            }
            public virtual ControllerDescriptor PublicGetControllerDescriptor(ControllerContext controllerContext) {
                return base.GetControllerDescriptor(controllerContext);
            }
            protected override ExceptionContext InvokeExceptionFilters(ControllerContext controllerContext, IList<IExceptionFilter> filters, Exception exception) {
                return PublicInvokeExceptionFilters(controllerContext, filters, exception);
            }
            public virtual ExceptionContext PublicInvokeExceptionFilters(ControllerContext controllerContext, IList<IExceptionFilter> filters, Exception exception) {
                return base.InvokeExceptionFilters(controllerContext, filters, exception);
            }
            protected override void InvokeActionResult(ControllerContext controllerContext, ActionResult actionResult) {
                PublicInvokeActionResult(controllerContext, actionResult);
            }
            public virtual void PublicInvokeActionResult(ControllerContext controllerContext, ActionResult actionResult) {
                base.InvokeActionResult(controllerContext, actionResult);
            }
            protected override FilterInfo GetFilters(ControllerContext controllerContext, ActionDescriptor actionDescriptor) {
                return PublicGetFilters(controllerContext, actionDescriptor);
            }
            public virtual FilterInfo PublicGetFilters(ControllerContext controllerContext, ActionDescriptor actionDescriptor) {
                return base.GetFilters(controllerContext, actionDescriptor);
            }
            protected override AuthorizationContext InvokeAuthorizationFilters(ControllerContext controllerContext, IList<IAuthorizationFilter> filters, ActionDescriptor actionDescriptor) {
                return PublicInvokeAuthorizationFilters(controllerContext, filters, actionDescriptor);
            }
            public virtual AuthorizationContext PublicInvokeAuthorizationFilters(ControllerContext controllerContext, IList<IAuthorizationFilter> filters, ActionDescriptor actionDescriptor) {
                return base.InvokeAuthorizationFilters(controllerContext, filters, actionDescriptor);
            }
            protected override ActionDescriptor FindAction(ControllerContext controllerContext, ControllerDescriptor controllerDescriptor, string actionName) {
                return PublicFindAction(controllerContext, controllerDescriptor, actionName);
            }
            public virtual ActionDescriptor PublicFindAction(ControllerContext controllerContext, ControllerDescriptor controllerDescriptor, string actionName) {
                return base.FindAction(controllerContext, controllerDescriptor, actionName);
            }
            protected override IDictionary<string, object> GetParameterValues(ControllerContext controllerContext, ActionDescriptor actionDescriptor) {
                return PublicGetParameterValues(controllerContext, actionDescriptor);
            }
            public virtual IDictionary<string, object> PublicGetParameterValues(ControllerContext controllerContext, ActionDescriptor actionDescriptor) {
                return base.GetParameterValues(controllerContext, actionDescriptor);
            }
        }

        private class EmptyController : AsyncController {
        }

    }
}
