namespace System.Web.Mvc.Test {
    using System;
    using System.Web.Mvc;
    using System.Web.TestUtil;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

    [TestClass]
    public class ModelBinderDictionaryTest {

        [TestMethod]
        public void DefaultBinderIsInstanceOfDefaultModelBinder() {
            // Arrange
            ModelBinderDictionary binders = new ModelBinderDictionary();

            // Act
            IModelBinder defaultBinder = binders.DefaultBinder;

            // Assert
            Assert.IsInstanceOfType(defaultBinder, typeof(DefaultModelBinder));
        }

        [TestMethod]
        public void DefaultBinderProperty() {
            // Arrange
            ModelBinderDictionary binders = new ModelBinderDictionary();
            IModelBinder binder = new Mock<IModelBinder>().Object;

            // Act & assert
            MemberHelper.TestPropertyWithDefaultInstance(binders, "DefaultBinder", binder);
        }

        [TestMethod]
        public void DictionaryInterface() {
            // Arrange
            DictionaryHelper<Type, IModelBinder> helper = new DictionaryHelper<Type, IModelBinder>() {
                Creator = () => new ModelBinderDictionary(),
                SampleKeys = new Type[] { typeof(object), typeof(string), typeof(int), typeof(long), typeof(long) },
                SampleValues = new IModelBinder[] { new DefaultModelBinder(), new DefaultModelBinder(), new DefaultModelBinder(), new DefaultModelBinder(), new DefaultModelBinder() },
                ThrowOnKeyNotFound = false
            };

            // Act & assert
            helper.Execute();
        }

        [TestMethod]
        public void GetBinderDoesNotReturnDefaultBinderIfAskedNotTo() {
            // Proper order of precedence:
            // 1. Binder registered in the global table
            // 2. Binder attribute defined on the type
            // 3. <null>

            // Arrange
            IModelBinder registeredFirstBinder = new Mock<IModelBinder>().Object;
            ModelBinderDictionary binders = new ModelBinderDictionary() {
                { typeof(MyFirstConvertibleType), registeredFirstBinder }
            };

            // Act
            IModelBinder binder1 = binders.GetBinder(typeof(MyFirstConvertibleType), false /* fallbackToDefault */);
            IModelBinder binder2 = binders.GetBinder(typeof(MySecondConvertibleType), false /* fallbackToDefault */);
            IModelBinder binder3 = binders.GetBinder(typeof(object), false /* fallbackToDefault */);

            // Assert
            Assert.AreSame(registeredFirstBinder, binder1, "First binder should have been matched in the registered binders table.");
            Assert.IsInstanceOfType(binder2, typeof(MySecondBinder), "Second binder should have been matched on the type.");
            Assert.IsNull(binder3, "Third binder should have returned null since asked not to use default.");
        }

        [TestMethod]
        public void GetBinderResolvesBindersWithCorrectPrecedence() {
            // Proper order of precedence:
            // 1. Binder registered in the global table
            // 2. Binder attribute defined on the type
            // 3. Default binder

            // Arrange
            IModelBinder registeredFirstBinder = new Mock<IModelBinder>().Object;
            ModelBinderDictionary binders = new ModelBinderDictionary() {
                { typeof(MyFirstConvertibleType), registeredFirstBinder }
            };

            IModelBinder defaultBinder = new Mock<IModelBinder>().Object;
            binders.DefaultBinder = defaultBinder;

            // Act
            IModelBinder binder1 = binders.GetBinder(typeof(MyFirstConvertibleType));
            IModelBinder binder2 = binders.GetBinder(typeof(MySecondConvertibleType));
            IModelBinder binder3 = binders.GetBinder(typeof(object));

            // Assert
            Assert.AreSame(registeredFirstBinder, binder1, "First binder should have been matched in the registered binders table.");
            Assert.IsInstanceOfType(binder2, typeof(MySecondBinder), "Second binder should have been matched on the type.");
            Assert.AreSame(defaultBinder, binder3, "Third binder should have been the fallback.");
        }

        [TestMethod]
        public void GetBinderThrowsIfModelTypeContainsMultipleAttributes() {
            // Arrange
            ModelBinderDictionary binders = new ModelBinderDictionary();

            // Act & Assert
            ExceptionHelper.ExpectInvalidOperationException(
                delegate {
                    binders.GetBinder(typeof(ConvertibleTypeWithSeveralBinders), true /* fallbackToDefault */);
                },
                "The type 'System.Web.Mvc.Test.ModelBinderDictionaryTest+ConvertibleTypeWithSeveralBinders'"
                + " contains multiple attributes inheriting from CustomModelBinderAttribute.");
        }

        [TestMethod]
        public void GetBinderThrowsIfModelTypeIsNull() {
            // Arrange
            ModelBinderDictionary binders = new ModelBinderDictionary();

            // Act & Assert
            ExceptionHelper.ExpectArgumentNullException(
                delegate {
                    binders.GetBinder(null);
                }, "modelType");
        }

        [ModelBinder(typeof(MyFirstBinder))]
        private class MyFirstConvertibleType {
        }

        private class MyFirstBinder : IModelBinder {
            public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext) {
                throw new NotImplementedException();
            }
        }

        [ModelBinder(typeof(MySecondBinder))]
        private class MySecondConvertibleType {
        }

        private class MySecondBinder : IModelBinder {
            public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext) {
                throw new NotImplementedException();
            }
        }

        [ModelBinder(typeof(MySecondBinder))]
        [MySubclassedBinder]
        private class ConvertibleTypeWithSeveralBinders {
        }

        private class MySubclassedBinderAttribute : CustomModelBinderAttribute {
            public override IModelBinder GetBinder() {
                throw new NotImplementedException();
            }
        }

    }
}
