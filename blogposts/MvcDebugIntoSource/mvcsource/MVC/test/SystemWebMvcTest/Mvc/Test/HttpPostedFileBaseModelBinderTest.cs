namespace System.Web.Mvc.Test {
    using System.Web.Mvc;
    using System.Web.TestUtil;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;
    using Moq.Properties;

    [TestClass]
    public class HttpPostedFileBaseModelBinderTest {

        [TestMethod]
        public void BindModelReturnsEmptyResultIfEmptyFileInputElementInPost() {
            // Arrange
            Mock<HttpPostedFileBase> mockFile = new Mock<HttpPostedFileBase>();
            mockFile.Expect(f => f.ContentLength).Returns(0);
            mockFile.Expect(f => f.FileName).Returns(String.Empty);
            Mock<ControllerContext> mockControllerContext = new Mock<ControllerContext>();
            mockControllerContext.ExpectGet(c => c.HttpContext.Request.Files["fileName"]).Returns(mockFile.Object);

            HttpPostedFileBaseModelBinder binder = new HttpPostedFileBaseModelBinder();
            ModelBindingContext bindingContext = new ModelBindingContext() { ModelName = "fileName" };

            // Act
            object result = binder.BindModel(mockControllerContext.Object, bindingContext);

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public void BindModelReturnsNullIfNoFileInputElementInPost() {
            // Arrange
            Mock<ControllerContext> mockControllerContext = new Mock<ControllerContext>();
            mockControllerContext.Expect(c => c.HttpContext.Request.Files["fileName"]).Returns((HttpPostedFileBase)null);

            HttpPostedFileBaseModelBinder binder = new HttpPostedFileBaseModelBinder();
            ModelBindingContext bindingContext = new ModelBindingContext() { ModelName = "fileName" };

            // Act
            object result = binder.BindModel(mockControllerContext.Object, bindingContext);

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public void BindModelReturnsResultIfFileFound() {
            // Arrange
            Mock<HttpPostedFileBase> mockFile = new Mock<HttpPostedFileBase>();
            mockFile.Expect(f => f.ContentLength).Returns(1234);
            mockFile.Expect(f => f.FileName).Returns("somefile");
            Mock<ControllerContext> mockControllerContext = new Mock<ControllerContext>();
            mockControllerContext.ExpectGet(c => c.HttpContext.Request.Files["fileName"]).Returns(mockFile.Object);

            HttpPostedFileBaseModelBinder binder = new HttpPostedFileBaseModelBinder();
            ModelBindingContext bindingContext = new ModelBindingContext() { ModelName = "fileName" };

            // Act
            object result = binder.BindModel(mockControllerContext.Object, bindingContext);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreSame(mockFile.Object, result, "Returned file did not match mock file.");
        }

        [TestMethod]
        public void BindModelThrowsIfBindingContextIsNull() {
            // Arrange
            ControllerContext controllerContext = new Mock<ControllerContext>().Object;
            HttpPostedFileBaseModelBinder binder = new HttpPostedFileBaseModelBinder();

            // Act & assert
            ExceptionHelper.ExpectArgumentNullException(
                delegate {
                    binder.BindModel(controllerContext, null);
                }, "bindingContext");
        }

        [TestMethod]
        public void BindModelThrowsIfControllerContextIsNull() {
            // Arrange
            HttpPostedFileBaseModelBinder binder = new HttpPostedFileBaseModelBinder();

            // Act & assert
            ExceptionHelper.ExpectArgumentNullException(
                delegate {
                    binder.BindModel(null, null);
                }, "controllerContext");
        }

    }
}
