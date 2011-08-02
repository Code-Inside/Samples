namespace System.Web.Mvc.Test {
    using System.Text;
    using System.Web.TestUtil;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

    [TestClass]
    public class ContentResultTest {

        [TestMethod]
        public void AllPropertiesDefaultToNull() {
            // Act
            ContentResult result = new ContentResult();

            // Assert
            Assert.IsNull(result.Content);
            Assert.IsNull(result.ContentEncoding);
            Assert.IsNull(result.ContentType);
        }

        [TestMethod]
        public void EmptyContentTypeIsNotOutput() {
            // Arrange
            string content = "Some content.";
            Encoding contentEncoding = Encoding.UTF8;

            // Arrange expectations
            Mock<ControllerContext> mockControllerContext = new Mock<ControllerContext>(MockBehavior.Strict);
            mockControllerContext.ExpectSet(c => c.HttpContext.Response.ContentEncoding, contentEncoding).Verifiable();
            mockControllerContext.Expect(c => c.HttpContext.Response.Write(content)).Verifiable();

            ContentResult result = new ContentResult {
                Content = content,
                ContentType = String.Empty,
                ContentEncoding = contentEncoding
            };

            // Act
            result.ExecuteResult(mockControllerContext.Object);

            // Assert
            mockControllerContext.Verify();
        }

        [TestMethod]
        public void ExecuteResult() {
            // Arrange
            string content = "Some content.";
            string contentType = "Some content type.";
            Encoding contentEncoding = Encoding.UTF8;
            
            // Arrange expectations
            Mock<ControllerContext> mockControllerContext = new Mock<ControllerContext>(MockBehavior.Strict);
            mockControllerContext.ExpectSet(c => c.HttpContext.Response.ContentType, contentType).Verifiable();
            mockControllerContext.ExpectSet(c => c.HttpContext.Response.ContentEncoding, contentEncoding).Verifiable();
            mockControllerContext.Expect(c => c.HttpContext.Response.Write(content)).Verifiable();

            ContentResult result = new ContentResult {
                Content = content,
                ContentType = contentType,
                ContentEncoding = contentEncoding
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
                    new ContentResult().ExecuteResult(null /* context */);
                }, "context");
        }

        [TestMethod]
        public void NullContentIsNotOutput() {
            // Arrange
            string contentType = "Some content type.";
            Encoding contentEncoding = Encoding.UTF8;

            // Arrange expectations
            Mock<ControllerContext> mockControllerContext = new Mock<ControllerContext>(MockBehavior.Strict);
            mockControllerContext.ExpectSet(c => c.HttpContext.Response.ContentType, contentType).Verifiable();
            mockControllerContext.ExpectSet(c => c.HttpContext.Response.ContentEncoding, contentEncoding).Verifiable();

            ContentResult result = new ContentResult {
                ContentType = contentType,
                ContentEncoding = contentEncoding
            };

            // Act
            result.ExecuteResult(mockControllerContext.Object);

            // Assert
            mockControllerContext.Verify();
        }

        [TestMethod]
        public void NullContentEncodingIsNotOutput() {
            // Arrange
            string content = "Some content.";
            string contentType = "Some content type.";

            // Arrange expectations
            Mock<ControllerContext> mockControllerContext = new Mock<ControllerContext>(MockBehavior.Strict);
            mockControllerContext.ExpectSet(c => c.HttpContext.Response.ContentType, contentType).Verifiable();
            mockControllerContext.Expect(c => c.HttpContext.Response.Write(content)).Verifiable();

            ContentResult result = new ContentResult {
                Content = content,
                ContentType = contentType,
            };

            // Act
            result.ExecuteResult(mockControllerContext.Object);

            // Assert
            mockControllerContext.Verify();
        }

        [TestMethod]
        public void NullContentTypeIsNotOutput() {
            // Arrange
            string content = "Some content.";
            Encoding contentEncoding = Encoding.UTF8;

            // Arrange expectations
            Mock<ControllerContext> mockControllerContext = new Mock<ControllerContext>(MockBehavior.Strict);
            mockControllerContext.ExpectSet(c => c.HttpContext.Response.ContentEncoding, contentEncoding).Verifiable();
            mockControllerContext.Expect(c => c.HttpContext.Response.Write(content)).Verifiable();

            ContentResult result = new ContentResult {
                Content = content,
                ContentEncoding = contentEncoding
            };

            // Act
            result.ExecuteResult(mockControllerContext.Object);

            // Assert
            mockControllerContext.Verify();
        }

    }
}
