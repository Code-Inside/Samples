namespace Microsoft.Web.Mvc.Test {
    using System;
    using System.Threading;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Routing;
    using System.Web.TestUtil;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.Web.Mvc;
    using Moq;

    [TestClass]
    public class MvcAsyncHandlerTest {

        [TestMethod]
        public void ProcessRequestThrowsIfControllerNotFound() {
            // Arrange
            Mock<HttpContextBase> mockHttpContext = new Mock<HttpContextBase>();
            mockHttpContext.Expect(c => c.Response.AppendHeader(MvcHandler.MvcVersionHeaderName, "1.0")).Verifiable();

            RequestContext requestContext = new RequestContext(mockHttpContext.Object, GetRouteData());
            MvcAsyncHandler handler = new MvcAsyncHandler(requestContext);

            ControllerBuilder builder = new ControllerBuilder();
            builder.SetControllerFactory(new SimpleControllerFactory(null));
            handler.ControllerBuilder = builder;

            // Act & assert
            ExceptionHelper.ExpectInvalidOperationException(
                delegate {
                    handler.BeginProcessRequest(mockHttpContext.Object, null, null);
                },
                @"The IControllerFactory 'Microsoft.Web.Mvc.Test.MvcAsyncHandlerTest+SimpleControllerFactory' did not return a controller for a controller named 'Foo'.");

            mockHttpContext.Verify();
        }

        [TestMethod]
        public void ProcessRequestWhereControllerBeginExecuteThrows() {
            // Arrange
            Mock<HttpContextBase> mockHttpContext = new Mock<HttpContextBase>();
            mockHttpContext.Expect(c => c.Response.AppendHeader(MvcHandler.MvcVersionHeaderName, "1.0")).Verifiable();

            RequestContext requestContext = new RequestContext(mockHttpContext.Object, GetRouteData());
            MvcAsyncHandler handler = new MvcAsyncHandler(requestContext);

            Mock<IAsyncController> mockController = new Mock<IAsyncController>();
            mockController.Expect(c => c.BeginExecute(requestContext, It.IsAny<AsyncCallback>(), It.IsAny<object>())).Throws(new InvalidOperationException("Some exception text.")).Verifiable();
            mockController.As<IDisposable>().Expect(c => c.Dispose()).AtMostOnce().Verifiable();

            ControllerBuilder builder = new ControllerBuilder();
            builder.SetControllerFactory(new SimpleControllerFactory(mockController.Object));
            handler.ControllerBuilder = builder;

            // Act
            ExceptionHelper.ExpectInvalidOperationException(
                delegate {
                    handler.BeginProcessRequest(mockHttpContext.Object, null, null);
                },
                @"Some exception text.");

            mockHttpContext.Verify();
            mockController.Verify();
        }

        [TestMethod]
        public void ProcessRequestWithNormalControlFlowForAsynchronousController() {
            // Arrange
            Mock<HttpContextBase> mockHttpContext = new Mock<HttpContextBase>();
            mockHttpContext.Expect(c => c.Response.AppendHeader(MvcHandler.MvcVersionHeaderName, "1.0")).Verifiable();

            RequestContext requestContext = new RequestContext(mockHttpContext.Object, GetRouteData());
            MvcAsyncHandler handler = new MvcAsyncHandler(requestContext);

            MockAsyncResult asyncResult = new MockAsyncResult();
            Mock<IAsyncController> mockController = new Mock<IAsyncController>();
            mockController.Expect(c => c.BeginExecute(requestContext, It.IsAny<AsyncCallback>(), It.IsAny<object>())).Returns(asyncResult).Verifiable();
            mockController.Expect(c => c.EndExecute(asyncResult)).Verifiable();
            mockController.As<IDisposable>().Expect(c => c.Dispose()).AtMostOnce().Verifiable();

            ControllerBuilder builder = new ControllerBuilder();
            builder.SetControllerFactory(new SimpleControllerFactory(mockController.Object));
            handler.ControllerBuilder = builder;

            // Act
            IAsyncResult returnedAsyncResult = handler.BeginProcessRequest(mockHttpContext.Object, null, null);
            handler.EndProcessRequest(returnedAsyncResult);

            mockHttpContext.Verify();
            mockController.Verify();
        }

        [TestMethod]
        public void ProcessRequestWithNormalControlFlowForSynchronousController() {
            // Arrange
            Mock<HttpContextBase> mockHttpContext = new Mock<HttpContextBase>();
            mockHttpContext.Expect(c => c.Response.AppendHeader(MvcHandler.MvcVersionHeaderName, "1.0")).Verifiable();

            RequestContext requestContext = new RequestContext(mockHttpContext.Object, GetRouteData());
            MvcAsyncHandler handler = new MvcAsyncHandler(requestContext);

            Mock<IController> mockController = new Mock<IController>();
            mockController.Expect(c => c.Execute(requestContext)).Verifiable();
            mockController.As<IDisposable>().Expect(c => c.Dispose()).AtMostOnce().Verifiable();

            ControllerBuilder builder = new ControllerBuilder();
            builder.SetControllerFactory(new SimpleControllerFactory(mockController.Object));
            handler.ControllerBuilder = builder;

            // Act
            IAsyncResult asyncResult = handler.BeginProcessRequest(mockHttpContext.Object, null, null);
            handler.EndProcessRequest(asyncResult);

            mockHttpContext.Verify();
            mockController.Verify();
        }

        [TestMethod]
        public void ProcessRequestWithRouteWithoutControllerThrows() {
            // Arrange
            Mock<HttpContextBase> mockHttpContext = new Mock<HttpContextBase>();
            mockHttpContext.Expect(c => c.Response.AppendHeader(MvcHandler.MvcVersionHeaderName, "1.0")).Verifiable();

            RequestContext requestContext = new RequestContext(mockHttpContext.Object, new RouteData());
            MvcAsyncHandler handler = new MvcAsyncHandler(requestContext);

            // Act & assert
            ExceptionHelper.ExpectInvalidOperationException(
                delegate {
                    handler.BeginProcessRequest(mockHttpContext.Object, null, null);
                },
                @"The RouteData must contain an item named 'controller' with a non-empty string value.");

            mockHttpContext.Verify();
        }

        [TestMethod]
        public void SynchronizationContextPropertyExplicitlySetByConstructor() {
            // Arrange
            SynchronizationContext syncContext = new SynchronizationContext();
            RequestContext requestContext = new RequestContext(new Mock<HttpContextBase>().Object, GetRouteData());

            // Act
            MvcAsyncHandler invoker = new MvcAsyncHandler(requestContext, syncContext);

            // Assert
            Assert.AreEqual(syncContext, invoker.SynchronizationContext);
        }

        [TestMethod]
        public void SynchronizationContextPropertyHasDefaultValue() {
            // Arrange
            RequestContext requestContext = new RequestContext(new Mock<HttpContextBase>().Object, GetRouteData());

            // Act
            MvcAsyncHandler invoker = new MvcAsyncHandler(requestContext);

            // Assert
            Assert.IsNotNull(invoker.SynchronizationContext);
        }

        private static RouteData GetRouteData() {
            RouteData rd = new RouteData();
            rd.Values["controller"] = "Foo";
            return rd;
        }

        private class SimpleControllerFactory : IControllerFactory {

            private IController _instance;

            public SimpleControllerFactory(IController instance) {
                _instance = instance;
            }

            public IController CreateController(RequestContext context, string controllerName) {
                return _instance;
            }

            public void ReleaseController(IController controller) {
                IDisposable disposable = controller as IDisposable;
                if (disposable != null) {
                    disposable.Dispose();
                }
            }
        }

    }
}
