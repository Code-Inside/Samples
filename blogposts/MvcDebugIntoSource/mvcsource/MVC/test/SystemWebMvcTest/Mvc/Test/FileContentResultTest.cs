namespace System.Web.Mvc.Test {
    using System.IO;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.TestUtil;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

    [TestClass]
    public class FileContentResultTest {

        [TestMethod]
        public void ConstructorSetsFileContentsProperty() {
            // Arrange
            byte[] fileContents = new byte[0];

            // Act
            FileContentResult result = new FileContentResult(fileContents, "contentType");

            // Assert
            Assert.AreSame(fileContents, result.FileContents);
        }

        [TestMethod]
        public void ConstructorThrowsIfFileContentsIsNull() {
            // Act & assert
            ExceptionHelper.ExpectArgumentNullException(
                delegate {
                    new FileContentResult(null, "contentType");
                }, "fileContents");
        }

        [TestMethod]
        public void WriteFileCopiesBufferToOutputStream() {
            // Arrange
            byte[] buffer = new byte[] { 1, 2, 3, 4, 5 };

            Mock<Stream> mockOutputStream = new Mock<Stream>();
            mockOutputStream.Expect(s => s.Write(buffer, 0, buffer.Length)).Verifiable();
            Mock<HttpResponseBase> mockResponse = new Mock<HttpResponseBase>();
            mockResponse.Expect(r => r.OutputStream).Returns(mockOutputStream.Object);

            FileContentResultHelper helper = new FileContentResultHelper(buffer, "application/octet-stream");

            // Act
            helper.PublicWriteFile(mockResponse.Object);

            // Assert
            mockOutputStream.Verify();
            mockResponse.Verify();
        }

        private class FileContentResultHelper : FileContentResult {
            public FileContentResultHelper(byte[] fileContents, string contentType)
                : base(fileContents, contentType) {
            }
            public void PublicWriteFile(HttpResponseBase response) {
                WriteFile(response);
            }
        }

    }
}
