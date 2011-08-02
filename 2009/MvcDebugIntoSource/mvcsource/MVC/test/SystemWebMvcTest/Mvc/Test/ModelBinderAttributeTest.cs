namespace System.Web.Mvc.Test {
    using System;
    using System.Web.Mvc;
    using System.Web.TestUtil;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class ModelBinderAttributeTest {

        [TestMethod]
        public void ConstructorWithInvalidBinderTypeThrows() {
            // Arrange
            Type badType = typeof(string);

            // Act & Assert
            ExceptionHelper.ExpectArgumentException(
                delegate {
                    new ModelBinderAttribute(badType);
                },
                "The type 'System.String' does not implement the IModelBinder interface.\r\nParameter name: binderType");
        }

        [TestMethod]
        public void ConstructorWithNullBinderTypeThrows() {
            // Act & Assert
            ExceptionHelper.ExpectArgumentNullException(
                delegate {
                    new ModelBinderAttribute(null);
                }, "binderType");
        }

        [TestMethod]
        public void BinderTypeProperty() {
            // Arrange
            Type binderType = typeof(GoodConverter);
            ModelBinderAttribute attr = new ModelBinderAttribute(binderType);

            // Act & Assert
            Assert.AreSame(binderType, attr.BinderType);
        }

        [TestMethod]
        public void GetBinder() {
            // Arrange
            Type binderType = typeof(GoodConverter);
            ModelBinderAttribute attr = new ModelBinderAttribute(binderType);

            // Act
            IModelBinder binder = attr.GetBinder();

            // Assert
            Assert.IsInstanceOfType(binder, binderType);
        }

        [TestMethod]
        public void GetBinderWithBadConstructorThrows() {
            // Arrange
            Type binderType = typeof(BadConverter);
            ModelBinderAttribute attr = new ModelBinderAttribute(binderType);

            // Act & Assert
            ExceptionHelper.ExpectInvalidOperationException(
                delegate {
                    attr.GetBinder();
                },
                "There was an error creating the IModelBinder 'System.Web.Mvc.Test.ModelBinderAttributeTest+BadConverter'."
                + " Check that it has a public parameterless constructor.");
        }

        private class GoodConverter : IModelBinder {
            public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext) {
                throw new NotImplementedException();
            }
        }

        private class BadConverter : IModelBinder {
            // no public parameterless constructor
            public BadConverter(string s) {
            }
            public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext) {
                throw new NotImplementedException();
            }
        }

    }
}