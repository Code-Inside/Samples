namespace Microsoft.Web.Mvc.Test {
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Routing;
    using System.Web.TestUtil;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.Web.Mvc;
    using Moq;

    [TestClass]
    public class FileCollectionModelBinderTest {

        [TestMethod]
        public void BindModelCanHandleMultipleFilesWithIndexedKey() {
            // Arrange
            HttpPostedFileBase foo_0 = GetFile("foo_0", 100);
            HttpPostedFileBase foo_1 = GetFile("", 0); // this file shouldn't be part of the returned list since it was empty
            HttpPostedFileBase foo_2 = GetFile("foo_2", 200);

            Mock<HttpContextBase> mockHttpContext = new Mock<HttpContextBase>();
            mockHttpContext.Expect(c => c.Request.Files["foo[0]"]).Returns(foo_0);
            mockHttpContext.Expect(c => c.Request.Files["foo[1]"]).Returns(foo_1);
            mockHttpContext.Expect(c => c.Request.Files["foo[2]"]).Returns(foo_2);
            ControllerContext controllerContext = GetControllerContext(mockHttpContext.Object);

            FileCollectionModelBinder binder = new FileCollectionModelBinder();
            ModelBindingContext bindingContext = new ModelBindingContext() { ModelName = "foo", ModelType = typeof(IList<HttpPostedFileBase>) };

            // Act
            object result = binder.BindModel(controllerContext, bindingContext);

            // Assert
            Assert.IsInstanceOfType(result, typeof(IList<HttpPostedFileBase>));
            IList<HttpPostedFileBase> castResult = (IList<HttpPostedFileBase>)result;
            Assert.IsTrue(castResult.SequenceEqual(new HttpPostedFileBase[] { foo_0, null, foo_2 }));
        }

        [TestMethod]
        public void BindModelCanHandleMultipleFilesWithSameKey() {
            // Arrange
            HttpPostedFileBase foo_0 = GetFile("foo_0", 100);
            HttpPostedFileBase foo_1 = GetFile("", 0); // this file shouldn't be part of the returned list since it was empty
            HttpPostedFileBase foo_2 = GetFile("foo_2", 200);

            Mock<HttpContextBase> mockHttpContext = new Mock<HttpContextBase>();
            mockHttpContext.Expect(c => c.Request.Files.AllKeys).Returns(new string[] { "foo", "FOO", "foo[0]", "Foo" });
            mockHttpContext.Expect(c => c.Request.Files[0]).Returns(foo_0);
            mockHttpContext.Expect(c => c.Request.Files[1]).Returns(foo_1);
            mockHttpContext.Expect(c => c.Request.Files[3]).Returns(foo_2);
            ControllerContext controllerContext = GetControllerContext(mockHttpContext.Object);

            FileCollectionModelBinder binder = new FileCollectionModelBinder();
            ModelBindingContext bindingContext = new ModelBindingContext() { ModelName = "foo", ModelType = typeof(IList<HttpPostedFileBase>) };

            // Act
            object result = binder.BindModel(controllerContext, bindingContext);

            // Assert
            Assert.IsInstanceOfType(result, typeof(IList<HttpPostedFileBase>));
            IList<HttpPostedFileBase> castResult = (IList<HttpPostedFileBase>)result;
            Assert.IsTrue(castResult.SequenceEqual(new HttpPostedFileBase[] { foo_0, null, foo_2 }));
        }

        [TestMethod]
        public void BindModelReturnsNullIfNoFileInputElementsMatchingKeyInRequest() {
            // Arrange
            Mock<HttpContextBase> mockHttpContext = new Mock<HttpContextBase>();
            mockHttpContext.Expect(c => c.Request.Files.AllKeys).Returns(new string[] { "bar", "baz", "quux" });
            ControllerContext controllerContext = GetControllerContext(mockHttpContext.Object);

            FileCollectionModelBinder binder = new FileCollectionModelBinder();
            ModelBindingContext bindingContext = new ModelBindingContext() { ModelName = "foo" };

            // Act
            object result = binder.BindModel(controllerContext, bindingContext);

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public void BindModelSupportsCollectionTypes() {
            TestBindModelSupportForCollectionType<HttpPostedFileBase[]>();
            TestBindModelSupportForCollectionType<IEnumerable<HttpPostedFileBase>>();
            TestBindModelSupportForCollectionType<ICollection<HttpPostedFileBase>>();
            TestBindModelSupportForCollectionType<IList<HttpPostedFileBase>>();
            TestBindModelSupportForCollectionType<Collection<HttpPostedFileBase>>();
            TestBindModelSupportForCollectionType<List<HttpPostedFileBase>>();
        }

        [TestMethod]
        public void BindModelThrowsIfBindingContextIsNull() {
            // Arrange
            FileCollectionModelBinder binder = new FileCollectionModelBinder();
            ControllerContext controllerContext = GetControllerContext(new Mock<HttpContextBase>().Object);

            // Act & assert
            ExceptionHelper.ExpectArgumentNullException(
                delegate {
                    binder.BindModel(controllerContext, null);
                }, "bindingContext");
        }

        [TestMethod]
        public void BindModelThrowsIfControllerContextIsNull() {
            // Arrange
            FileCollectionModelBinder binder = new FileCollectionModelBinder();

            // Act & assert
            ExceptionHelper.ExpectArgumentNullException(
                delegate {
                    binder.BindModel(null, null);
                }, "controllerContext");
        }

        [TestMethod]
        public void BindModelThrowsIfModelTypeIsNotSupportedCollectionType() {
            // Arrange
            HttpPostedFileBase foo_0 = GetFile("foo_0", 100);
            HttpPostedFileBase foo_1 = GetFile("", 0); // this file shouldn't be part of the returned collection since it was empty
            HttpPostedFileBase foo_2 = GetFile("foo_2", 200);

            Mock<HttpContextBase> mockHttpContext = new Mock<HttpContextBase>();
            mockHttpContext.Expect(c => c.Request.Files["foo[0]"]).Returns(foo_0);
            mockHttpContext.Expect(c => c.Request.Files["foo[1]"]).Returns(foo_1);
            mockHttpContext.Expect(c => c.Request.Files["foo[2]"]).Returns(foo_2);
            ControllerContext controllerContext = GetControllerContext(mockHttpContext.Object);

            FileCollectionModelBinder binder = new FileCollectionModelBinder();
            ModelBindingContext bindingContext = new ModelBindingContext() { ModelName = "foo", ModelType = typeof(string) };

            // Act & assert
            ExceptionHelper.ExpectInvalidOperationException(
                delegate {
                    binder.BindModel(controllerContext, bindingContext);
                },
                "This model binder does not support the model type 'System.String'.");
        }

        [TestMethod]
        public void RegisterBinderPopulatesModelBinderDictionary() {
            // Arrange
            ModelBinderDictionary binderDict = new ModelBinderDictionary();

            // Act
            FileCollectionModelBinder.RegisterBinder(binderDict);

            // Assert
            Assert.AreEqual(6, binderDict.Count);
            Assert.IsInstanceOfType(binderDict.GetBinder(typeof(HttpPostedFileBase[])), typeof(FileCollectionModelBinder));
            Assert.IsInstanceOfType(binderDict.GetBinder(typeof(IEnumerable<HttpPostedFileBase>)), typeof(FileCollectionModelBinder));
            Assert.IsInstanceOfType(binderDict.GetBinder(typeof(ICollection<HttpPostedFileBase>)), typeof(FileCollectionModelBinder));
            Assert.IsInstanceOfType(binderDict.GetBinder(typeof(IList<HttpPostedFileBase>)), typeof(FileCollectionModelBinder));
            Assert.IsInstanceOfType(binderDict.GetBinder(typeof(Collection<HttpPostedFileBase>)), typeof(FileCollectionModelBinder));
            Assert.IsInstanceOfType(binderDict.GetBinder(typeof(List<HttpPostedFileBase>)), typeof(FileCollectionModelBinder));
        }

        private static void TestBindModelSupportForCollectionType<T>() where T : IEnumerable<HttpPostedFileBase> {
            // Arrange
            HttpPostedFileBase foo_0 = GetFile("foo_0", 100);
            HttpPostedFileBase foo_1 = GetFile("", 0); // this file shouldn't be part of the returned collection since it was empty
            HttpPostedFileBase foo_2 = GetFile("foo_2", 200);

            Mock<HttpContextBase> mockHttpContext = new Mock<HttpContextBase>();
            mockHttpContext.Expect(c => c.Request.Files["foo[0]"]).Returns(foo_0);
            mockHttpContext.Expect(c => c.Request.Files["foo[1]"]).Returns(foo_1);
            mockHttpContext.Expect(c => c.Request.Files["foo[2]"]).Returns(foo_2);
            ControllerContext controllerContext = GetControllerContext(mockHttpContext.Object);

            FileCollectionModelBinder binder = new FileCollectionModelBinder();
            ModelBindingContext bindingContext = new ModelBindingContext() { ModelName = "foo", ModelType = typeof(T) };

            // Act
            object result = binder.BindModel(controllerContext, bindingContext);

            // Assert
            Assert.IsInstanceOfType(result, typeof(T));
            T castResult = (T)result;
            Assert.IsTrue(castResult.SequenceEqual(new HttpPostedFileBase[] { foo_0, null, foo_2 }));
        }

        private static ControllerContext GetControllerContext(HttpContextBase httpContext) {
            ControllerContext controllerContext = new ControllerContext(httpContext, new RouteData(), new Mock<ControllerBase>().Object);
            return controllerContext;
        }

        private static HttpPostedFileBase GetFile(string filename, int contentLength) {
            Mock<HttpPostedFileBase> mockFile = new Mock<HttpPostedFileBase>();
            mockFile.Expect(f => f.ContentLength).Returns(contentLength);
            mockFile.Expect(f => f.FileName).Returns(filename);
            return mockFile.Object;
        }

    }
}
