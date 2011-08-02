namespace System.Web.Mvc.Test {
    using System.Web.TestUtil;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

    [TestClass]
    public class JavaScriptResultTest {

        [TestMethod]
        public void AllPropertiesDefaultToNull() {
            // Act
            JavaScriptResult result = new JavaScriptResult();

            // Assert
            Assert.IsNull(result.Script);
        }

        [TestMethod]
        public void ExecuteResult() {
            // Arrange
            string script = "alert('foo');";
            string contentType = "application/x-javascript";

            // Arrange expectations
            Mock<ControllerContext> mockControllerContext = new Mock<ControllerContext>(MockBehavior.Strict);
            mockControllerContext.ExpectSet(c => c.HttpContext.Response.ContentType, contentType).Verifiable();
            mockControllerContext.Expect(c => c.HttpContext.Response.Write(script)).Verifiable();

            JavaScriptResult result = new JavaScriptResult {
                Script = script
            };

            // Act
            result.ExecuteResult(mockControllerContext.Object);

            // Assert
            mockControllerContext.Verify();
        }

        [TestMethod]
        public void ExecuteResultWithNullContextThrows() {
            ExceptionHelper.ExpectArgumentNullException(
                delegate {
                    new JavaScriptResult().ExecuteResult(null /* context */);
                }, "context");
        }

        [TestMethod]
        public void NullScriptIsNotOutput() {
            // Arrange
            string contentType = "application/x-javascript";

            // Arrange expectations
            Mock<ControllerContext> mockControllerContext = new Mock<ControllerContext>(MockBehavior.Strict);
            mockControllerContext.ExpectSet(c => c.HttpContext.Response.ContentType, contentType).Verifiable();

            JavaScriptResult result = new JavaScriptResult();

            // Act
            result.ExecuteResult(mockControllerContext.Object);

            // Assert
            mockControllerContext.Verify();
        }

    }
}
