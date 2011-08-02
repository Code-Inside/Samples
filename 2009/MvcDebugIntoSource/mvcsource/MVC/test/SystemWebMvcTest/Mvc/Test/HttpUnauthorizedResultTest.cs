namespace System.Web.Mvc.Test {
    using System.Web;
    using System.Web.Routing;
    using System.Web.TestUtil;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;
    using Moq.Properties;

    [TestClass]
    public class HttpUnauthorizedResultTest {

        [TestMethod]
        public void ExecuteResult() {
            // Arrange
            Mock<ControllerContext> mockControllerContext = new Mock<ControllerContext>(MockBehavior.Strict);
            mockControllerContext.ExpectSet(c => c.HttpContext.Response.StatusCode, 401).Verifiable();

            HttpUnauthorizedResult result = new HttpUnauthorizedResult();

            // Act
            result.ExecuteResult(mockControllerContext.Object);

            // Assert
            mockControllerContext.Verify();
        }

        [TestMethod]
        public void ExecuteResultWithNullContextThrows() {
            ExceptionHelper.ExpectArgumentNullException(
                delegate {
                    new HttpUnauthorizedResult().ExecuteResult(null /* context */);
                }, "context");
        }

    }
}
