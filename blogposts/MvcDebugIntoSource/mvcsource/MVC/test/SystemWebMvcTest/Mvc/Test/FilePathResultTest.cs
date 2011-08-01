namespace System.Web.Mvc.Test {
    using System;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.TestUtil;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

    [TestClass]
    public class FilePathResultTest {

        [TestMethod]
        public void ConstructorSetsFileNameProperty() {
            // Act
            FilePathResult result = new FilePathResult("someFile", "contentType");

            // Assert
            Assert.AreEqual("someFile", result.FileName);
        }

        [TestMethod]
        public void ConstructorThrowsIfFileNameIsEmpty() {
            // Act & assert
            ExceptionHelper.ExpectArgumentExceptionNullOrEmpty(
                delegate {
                    new FilePathResult(String.Empty, "contentType");
                }, "fileName");
        }

        [TestMethod]
        public void ConstructorThrowsIfFileNameIsNull() {
            // Act & assert
            ExceptionHelper.ExpectArgumentExceptionNullOrEmpty(
                delegate {
                    new FilePathResult(null, "contentType");
                }, "fileName");
        }

        [TestMethod]
        public void WriteFileTransmitsFileToOutputStream() {
            // Arrange
            Mock<HttpResponseBase> mockResponse = new Mock<HttpResponseBase>();
            mockResponse.Expect(r => r.TransmitFile("someFile")).Verifiable();

            FilePathResultHelper helper = new FilePathResultHelper("someFile", "application/octet-stream");

            // Act
            helper.PublicWriteFile(mockResponse.Object);

            // Assert
            mockResponse.Verify();
        }

        private class FilePathResultHelper : FilePathResult {
            public FilePathResultHelper(string fileName, string contentType)
                : base(fileName, contentType) {
            }
            public void PublicWriteFile(HttpResponseBase response) {
                WriteFile(response);
            }
        }

    }
}
