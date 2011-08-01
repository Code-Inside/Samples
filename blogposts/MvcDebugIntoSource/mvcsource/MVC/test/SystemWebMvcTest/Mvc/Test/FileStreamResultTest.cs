namespace System.Web.Mvc.Test {
    using System;
    using System.IO;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.TestUtil;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

    [TestClass]
    public class FileStreamResultTest {

        private static readonly Random _random = new Random();

        [TestMethod]
        public void ConstructorSetsFileStreamProperty() {
            // Arrange
            Stream stream = new MemoryStream();

            // Act
            FileStreamResult result = new FileStreamResult(stream, "contentType");

            // Assert
            Assert.AreSame(stream, result.FileStream);
        }

        [TestMethod]
        public void ConstructorThrowsIfFileStreamIsNull() {
            // Act & assert
            ExceptionHelper.ExpectArgumentNullException(
                delegate {
                    new FileStreamResult(null, "contentType");
                }, "fileStream");
        }

        [TestMethod]
        public void WriteFileCopiesProvidedStreamToOutputStream() {
            // Arrange
            int byteLen = 0x1234;
            byte[] originalBytes = GetRandomByteArray(byteLen);
            MemoryStream originalStream = new MemoryStream(originalBytes);
            MemoryStream outStream = new MemoryStream();

            Mock<HttpResponseBase> mockResponse = new Mock<HttpResponseBase>();
            mockResponse.Expect(r => r.OutputStream).Returns(outStream);

            FileStreamResultHelper helper = new FileStreamResultHelper(originalStream, "application/octet-stream");

            // Act
            helper.PublicWriteFile(mockResponse.Object);

            // Assert
            byte[] outBytes = outStream.ToArray();
            Assert.IsTrue(originalBytes.SequenceEqual(outBytes), "Output stream does not match input stream.");
            mockResponse.Verify();
        }

        private static byte[] GetRandomByteArray(int length) {
            byte[] bytes = new byte[length];
            _random.NextBytes(bytes);
            return bytes;
        }

        private class FileStreamResultHelper : FileStreamResult {
            public FileStreamResultHelper(Stream fileStream, string contentType)
                : base(fileStream, contentType) {
            }
            public void PublicWriteFile(HttpResponseBase response) {
                WriteFile(response);
            }
        }

    }
}
