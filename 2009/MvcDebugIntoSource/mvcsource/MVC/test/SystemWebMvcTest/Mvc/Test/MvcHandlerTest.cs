namespace System.Web.Mvc.Test {
    using System;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Routing;
    using System.Web.TestUtil;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;
    using Moq.Protected;

    [TestClass]
    public class MvcHandlerTest {
        [TestMethod]
        public void ConstructorWithNullRequestContextThrows() {
            ExceptionHelper.ExpectArgumentNullException(
                delegate {
                    new MvcHandler(null);
                },
                "requestContext");
        }

        [TestMethod]
        public void ProcessRequestWithRouteWithoutControllerThrows() {
            // Arrange
            Mock<HttpContextBase> contextMock = new Mock<HttpContextBase>();
            Mock<HttpResponseBase> responseMock = new Mock<HttpResponseBase>();
            responseMock.Expect(r => r.AppendHeader(MvcHandler.MvcVersionHeaderName, "1.0")).Verifiable();
            contextMock.Expect(c => c.Response).Returns(responseMock.Object);
            RouteData rd = new RouteData();
            MvcHandler mvcHandler = new MvcHandler(new RequestContext(contextMock.Object, rd));

            // Act
            ExceptionHelper.ExpectException<InvalidOperationException>(
                delegate {
                    mvcHandler.ProcessRequest(contextMock.Object);
                },
                "The RouteData must contain an item named 'controller' with a non-empty string value.");

            // Assert
            responseMock.Verify();
        }

        [TestMethod]
        public void ProcessRequestAddsServerHeaderCallsExecute() {
            // Arrange
            Mock<HttpContextBase> contextMock = new Mock<HttpContextBase>();
            Mock<HttpResponseBase> responseMock = new Mock<HttpResponseBase>();
            responseMock.Expect(r => r.AppendHeader(MvcHandler.MvcVersionHeaderName, "1.0")).Verifiable();
            contextMock.Expect(c => c.Response).Returns(responseMock.Object);

            RouteData rd = new RouteData();
            rd.Values.Add("controller", "foo");
            RequestContext requestContext = new RequestContext(contextMock.Object, rd);
            MvcHandler mvcHandler = new MvcHandler(requestContext);

            Mock<ControllerBase> controllerMock = new Mock<ControllerBase>();
            controllerMock.Protected().Expect("Execute", requestContext).Verifiable();

            ControllerBuilder cb = new ControllerBuilder();
            Mock<IControllerFactory> controllerFactoryMock = new Mock<IControllerFactory>();
            controllerFactoryMock.Expect(o => o.CreateController(requestContext, "foo")).Returns(controllerMock.Object);
            controllerFactoryMock.Expect(o => o.ReleaseController(controllerMock.Object));
            cb.SetControllerFactory(controllerFactoryMock.Object);
            mvcHandler.ControllerBuilder = cb;

            // Act
            mvcHandler.ProcessRequest(contextMock.Object);

            // Assert
            responseMock.Verify();
            controllerMock.Verify();
        }

        [TestMethod]
        public void ProcessRequestWithDisabledServerHeaderOnlyCallsExecute() {
            bool oldResponseHeaderValue = MvcHandler.DisableMvcResponseHeader;
            try {
                // Arrange
                MvcHandler.DisableMvcResponseHeader = true;
                Mock<HttpContextBase> contextMock = new Mock<HttpContextBase>();

                RouteData rd = new RouteData();
                rd.Values.Add("controller", "foo");
                RequestContext requestContext = new RequestContext(contextMock.Object, rd);
                MvcHandler mvcHandler = new MvcHandler(requestContext);

                Mock<ControllerBase> controllerMock = new Mock<ControllerBase>();
                controllerMock.Protected().Expect("Execute", requestContext).Verifiable();

                ControllerBuilder cb = new ControllerBuilder();
                Mock<IControllerFactory> controllerFactoryMock = new Mock<IControllerFactory>();
                controllerFactoryMock.Expect(o => o.CreateController(requestContext, "foo")).Returns(controllerMock.Object);
                controllerFactoryMock.Expect(o => o.ReleaseController(controllerMock.Object));
                cb.SetControllerFactory(controllerFactoryMock.Object);
                mvcHandler.ControllerBuilder = cb;

                // Act
                mvcHandler.ProcessRequest(contextMock.Object);

                // Assert
                controllerMock.Verify();
            }
            finally {
                MvcHandler.DisableMvcResponseHeader = oldResponseHeaderValue;
            }
        }

        [TestMethod]
        public void ProcessRequestDisposesControllerIfExecuteDoesNotThrowException() {
            // Arrange
            Mock<ControllerBase> mockController = new Mock<ControllerBase>();
            mockController.Protected().Expect("Execute", ItExpr.IsAny<RequestContext>()).Verifiable();
            mockController.As<IDisposable>().Expect(d => d.Dispose()).AtMostOnce().Verifiable();

            ControllerBuilder builder = new ControllerBuilder();
            builder.SetControllerFactory(new SimpleControllerFactory(mockController.Object));

            Mock<HttpContextBase> contextMock = new Mock<HttpContextBase>();
            Mock<HttpResponseBase> responseMock = new Mock<HttpResponseBase>();
            responseMock.Expect(r => r.AppendHeader(MvcHandler.MvcVersionHeaderName, "1.0")).Verifiable();
            contextMock.Expect(c => c.Response).Returns(responseMock.Object);
            RequestContext requestContext = new RequestContext(contextMock.Object, new RouteData());
            requestContext.RouteData.Values["controller"] = "fooController";
            MvcHandler handler = new MvcHandler(requestContext) {
                ControllerBuilder = builder
            };

            // Act
            handler.ProcessRequest(requestContext.HttpContext);

            // Assert
            mockController.Verify();
            responseMock.Verify();
        }

        [TestMethod]
        public void ProcessRequestDisposesControllerIfExecuteThrowsException() {
            // Arrange
            Mock<ControllerBase> mockController = new Mock<ControllerBase>(MockBehavior.Strict);
            mockController.Protected().Expect("Execute", ItExpr.IsAny<RequestContext>()).Throws(new Exception("some exception"));
            mockController.As<IDisposable>().Expect(d => d.Dispose()).AtMostOnce().Verifiable();

            ControllerBuilder builder = new ControllerBuilder();
            builder.SetControllerFactory(new SimpleControllerFactory(mockController.Object));

            Mock<HttpContextBase> contextMock = new Mock<HttpContextBase>();
            Mock<HttpResponseBase> responseMock = new Mock<HttpResponseBase>();
            responseMock.Expect(r => r.AppendHeader(MvcHandler.MvcVersionHeaderName, "1.0")).Verifiable();
            contextMock.Expect(c => c.Response).Returns(responseMock.Object);
            RequestContext requestContext = new RequestContext(contextMock.Object, new RouteData());
            requestContext.RouteData.Values["controller"] = "fooController";
            MvcHandler handler = new MvcHandler(requestContext) {
                ControllerBuilder = builder
            };

            // Act
            ExceptionHelper.ExpectException<Exception>(
                delegate {
                    handler.ProcessRequest(requestContext.HttpContext);
                },
                "some exception");

            // Assert
            mockController.Verify();
            responseMock.Verify();
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
