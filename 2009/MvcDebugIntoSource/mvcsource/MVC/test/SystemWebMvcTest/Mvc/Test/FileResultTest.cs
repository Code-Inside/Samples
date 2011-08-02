namespace System.Web.Mvc.Test {
    using System;
    using System.Net.Mime;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.TestUtil;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

    [TestClass]
    public class FileResultTest {

        [TestMethod]
        public void ConstructorSetsContentTypeProperty() {
            // Act
            FileResult result = new EmptyFileResult("someContentType");

            // Assert
            Assert.AreEqual("someContentType", result.ContentType);
        }

        [TestMethod]
        public void ConstructorThrowsIfContentTypeIsEmpty() {
            // Act & assert
            ExceptionHelper.ExpectArgumentExceptionNullOrEmpty(
                delegate {
                    new EmptyFileResult(String.Empty);
                }, "contentType");
        }

        [TestMethod]
        public void ConstructorThrowsIfContentTypeIsNull() {
            // Act & assert
            ExceptionHelper.ExpectArgumentExceptionNullOrEmpty(
                delegate {
                    new EmptyFileResult(null);
                }, "contentType");
        }

        [TestMethod]
        public void ContentDispositionHeaderIsEncodedCorrectly() {
            // See comment in FileResult.cs detailing how the FileDownloadName should be encoded.

            // Arrange
            Mock<ControllerContext> mockControllerContext = new Mock<ControllerContext>(MockBehavior.Strict);
            mockControllerContext.ExpectSet(c => c.HttpContext.Response.ContentType, "application/my-type").Verifiable();
            mockControllerContext.Expect(c => c.HttpContext.Response.AddHeader("Content-Disposition", @"attachment; filename=""some\\file""")).Verifiable();

            EmptyFileResult result = new EmptyFileResult("application/my-type") {
                FileDownloadName = @"some\file"
            };

            // Act
            result.ExecuteResult(mockControllerContext.Object);

            // Assert
            Assert.IsTrue(result.WasWriteFileCalled);
            mockControllerContext.Verify();
        }

        [TestMethod]
        public void ExecuteResultDoesNotSetContentDispositionIfNotSpecified() {
            // Arrange
            Mock<ControllerContext> mockControllerContext = new Mock<ControllerContext>(MockBehavior.Strict);
            mockControllerContext.ExpectSet(c => c.HttpContext.Response.ContentType, "application/my-type").Verifiable();

            EmptyFileResult result = new EmptyFileResult("application/my-type");

            // Act
            result.ExecuteResult(mockControllerContext.Object);

            // Assert
            Assert.IsTrue(result.WasWriteFileCalled);
            mockControllerContext.Verify();
        }

        [TestMethod]
        public void ExecuteResultSetsContentDispositionIfSpecified() {
            // Arrange
            Mock<ControllerContext> mockControllerContext = new Mock<ControllerContext>(MockBehavior.Strict);
            mockControllerContext.ExpectSet(c => c.HttpContext.Response.ContentType, "application/my-type").Verifiable();
            mockControllerContext.Expect(c => c.HttpContext.Response.AddHeader("Content-Disposition", "attachment; filename=filename.ext")).Verifiable();

            EmptyFileResult result = new EmptyFileResult("application/my-type") {
                FileDownloadName = "filename.ext"
            };

            // Act
            result.ExecuteResult(mockControllerContext.Object);

            // Assert
            Assert.IsTrue(result.WasWriteFileCalled);
            mockControllerContext.Verify();
        }

        [TestMethod]
        public void ExecuteResultThrowsIfContextIsNull() {
            // Arrange
            FileResult result = new EmptyFileResult();

            // Act & assert
            ExceptionHelper.ExpectArgumentNullException(
                delegate {
                    result.ExecuteResult(null);
                }, "context");
        }

        [TestMethod]
        public void FileDownloadNameProperty() {
            // Arrange
            FileResult result = new EmptyFileResult();

            // Act & assert
            MemberHelper.TestStringProperty(result, "FileDownloadName", String.Empty, false /* testDefaultValue */, true /* allowNullAndEmpty */);
        }

        private class EmptyFileResult : FileResult {
            public bool WasWriteFileCalled;
            public EmptyFileResult()
                : this(MediaTypeNames.Application.Octet) {
            }
            public EmptyFileResult(string contentType)
                : base(contentType) {
            }
            protected override void WriteFile(HttpResponseBase response) {
                WasWriteFileCalled = true;
            }
        }

    }
}
