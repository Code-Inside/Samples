namespace System.Web.Mvc.Test {
    using System.Text;
    using System.Web.TestUtil;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

    [TestClass]
    public class JsonResultTest {
        private static readonly object _jsonData = new object[] { 1, 2, "three", "four" };
        private static readonly string _jsonSerializedData = "[1,2,\"three\",\"four\"]";

        [TestMethod]
        public void AllPropertiesDefaultToNull() {
            // Act
            JsonResult result = new JsonResult();

            // Assert
            Assert.IsNull(result.Data);
            Assert.IsNull(result.ContentEncoding);
            Assert.IsNull(result.ContentType);
        }

        [TestMethod]
        public void EmptyContentTypeRendersDefault() {
            // Arrange
            object data = _jsonData;
            Encoding contentEncoding = Encoding.UTF8;

            // Arrange expectations
            Mock<ControllerContext> mockControllerContext = new Mock<ControllerContext>(MockBehavior.Strict);
            mockControllerContext.ExpectSet(c => c.HttpContext.Response.ContentType, "application/json").Verifiable();
            mockControllerContext.ExpectSet(c => c.HttpContext.Response.ContentEncoding, contentEncoding).Verifiable();
            mockControllerContext.Expect(c => c.HttpContext.Response.Write(_jsonSerializedData)).Verifiable();

            JsonResult result = new JsonResult {
                Data = data,
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
            object data = _jsonData;
            string contentType = "Some content type.";
            Encoding contentEncoding = Encoding.UTF8;

            // Arrange expectations
            Mock<ControllerContext> mockControllerContext = new Mock<ControllerContext>(MockBehavior.Strict);
            mockControllerContext.ExpectSet(c => c.HttpContext.Response.ContentType, contentType).Verifiable();
            mockControllerContext.ExpectSet(c => c.HttpContext.Response.ContentEncoding, contentEncoding).Verifiable();
            mockControllerContext.Expect(c => c.HttpContext.Response.Write(_jsonSerializedData)).Verifiable();

            JsonResult result = new JsonResult {
                Data = data,
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
                    new JsonResult().ExecuteResult(null /* context */);
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

            JsonResult result = new JsonResult {
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
            object data = _jsonData;
            string contentType = "Some content type.";

            // Arrange expectations
            Mock<ControllerContext> mockControllerContext = new Mock<ControllerContext>(MockBehavior.Strict);
            mockControllerContext.ExpectSet(c => c.HttpContext.Response.ContentType, contentType).Verifiable();
            mockControllerContext.Expect(c => c.HttpContext.Response.Write(_jsonSerializedData)).Verifiable();

            JsonResult result = new JsonResult {
                Data = data,
                ContentType = contentType,
            };

            // Act
            result.ExecuteResult(mockControllerContext.Object);

            // Assert
            mockControllerContext.Verify();
        }

        [TestMethod]
        public void NullContentTypeRendersDefault() {
            // Arrange
            object data = _jsonData;
            Encoding contentEncoding = Encoding.UTF8;

            // Arrange expectations
            Mock<ControllerContext> mockControllerContext = new Mock<ControllerContext>(MockBehavior.Strict);
            mockControllerContext.ExpectSet(c => c.HttpContext.Response.ContentType, "application/json").Verifiable();
            mockControllerContext.ExpectSet(c => c.HttpContext.Response.ContentEncoding, contentEncoding).Verifiable();
            mockControllerContext.Expect(c => c.HttpContext.Response.Write(_jsonSerializedData)).Verifiable();

            JsonResult result = new JsonResult {
                Data = data,
                ContentEncoding = contentEncoding
            };

            // Act
            result.ExecuteResult(mockControllerContext.Object);

            // Assert
            mockControllerContext.Verify();
        }

    }
}
