namespace System.Web.Mvc.Test {
    using System;
    using System.Web.Routing;
    using System.Web.TestUtil;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

    [TestClass]
    public class MvcHttpHandlerTest {
        [TestMethod]
        public void ConstructorDoesNothing() {
            new MvcHttpHandler();
        }

        [TestMethod]
        public void VerifyAndProcessRequestWithNullHandlerThrows() {
            // Arrange
            PublicMvcHttpHandler handler = new PublicMvcHttpHandler();

            // Act
            ExceptionHelper.ExpectArgumentNullException(
                delegate {
                    handler.PublicVerifyAndProcessRequest(null, null);
                },
                "httpHandler");
        }

        [TestMethod]
        public void ProcessRequestCallsExecute() {
            // Arrange
            PublicMvcHttpHandler handler = new PublicMvcHttpHandler();
            Mock<IHttpHandler> mockTargetHandler = new Mock<IHttpHandler>();
            mockTargetHandler.Expect(h => h.ProcessRequest(It.IsAny<HttpContext>())).Verifiable();

            // Act
            handler.PublicVerifyAndProcessRequest(mockTargetHandler.Object, null);

            // Assert
            mockTargetHandler.Verify();
        }

        private sealed class DummyHttpHandler : IHttpHandler {
            bool IHttpHandler.IsReusable {
                get {
                    throw new NotImplementedException();
                }
            }

            void IHttpHandler.ProcessRequest(HttpContext context) {
                throw new NotImplementedException();
            }
        }

        private sealed class PublicMvcHttpHandler : MvcHttpHandler {
            public void PublicVerifyAndProcessRequest(IHttpHandler httpHandler, HttpContextBase httpContext) {
                base.VerifyAndProcessRequest(httpHandler, httpContext);
            }
        }
    }
}
