namespace Microsoft.Web.Mvc.Test {
    using System;
    using System.Reflection;
    using System.Web.Mvc;
    using System.Web.TestUtil;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.Web.Mvc;
    using Moq;

    [TestClass]
    public class ReflectedAsyncControllerDescriptorTest {

        [TestMethod]
        public void ConstructorSetsControllerTypeProperty() {
            // Arrange
            Type controllerType = typeof(string);

            // Act
            ReflectedAsyncControllerDescriptor cd = new ReflectedAsyncControllerDescriptor(controllerType);

            // Assert
            Assert.AreSame(controllerType, cd.ControllerType);
        }

        [TestMethod]
        public void ConstructorThrowsIfControllerTypeIsNull() {
            // Act & assert
            ExceptionHelper.ExpectArgumentNullException(
                delegate {
                    new ReflectedAsyncControllerDescriptor(null);
                }, "controllerType");
        }

        [TestMethod]
        public void FindActionReturnsActionDescriptorIfFound() {
            // Arrange
            Type controllerType = typeof(MyController);
            MethodInfo setupMethod = controllerType.GetMethod("Foo");
            MethodInfo completionMethod = controllerType.GetMethod("FooCompleted");
            ReflectedAsyncControllerDescriptor cd = new ReflectedAsyncControllerDescriptor(controllerType);

            // Act
            ActionDescriptor ad = cd.FindAction(new Mock<ControllerContext>().Object, "NewName");

            // Assert
            Assert.AreEqual("NewName", ad.ActionName);
            Assert.IsInstanceOfType(ad, typeof(ReflectedEventPatternActionDescriptor));
            ReflectedEventPatternActionDescriptor castAd = (ReflectedEventPatternActionDescriptor)ad;

            Assert.AreSame(setupMethod, castAd.SetupMethod, "SetupMethod pointed to wrong method.");
            Assert.AreSame(completionMethod, castAd.CompletionMethod, "CompletionMethod pointed to wrong method.");
            Assert.AreSame(cd, ad.ControllerDescriptor, "Controller did not point back to correct descriptor.");
        }

        [TestMethod]
        public void FindActionReturnsNullIfNoActionFound() {
            // Arrange
            Type controllerType = typeof(MyController);
            ReflectedAsyncControllerDescriptor cd = new ReflectedAsyncControllerDescriptor(controllerType);

            // Act
            ActionDescriptor ad = cd.FindAction(new Mock<ControllerContext>().Object, "NonExistent");

            // Assert
            Assert.IsNull(ad);
        }

        [TestMethod]
        public void FindActionThrowsIfActionNameIsEmpty() {
            // Arrange
            Type controllerType = typeof(MyController);
            ReflectedAsyncControllerDescriptor cd = new ReflectedAsyncControllerDescriptor(controllerType);

            // Act & assert
            ExceptionHelper.ExpectArgumentExceptionNullOrEmpty(
                delegate {
                    cd.FindAction(new Mock<ControllerContext>().Object, "");
                }, "actionName");
        }

        [TestMethod]
        public void FindActionThrowsIfActionNameIsNull() {
            // Arrange
            Type controllerType = typeof(MyController);
            ReflectedAsyncControllerDescriptor cd = new ReflectedAsyncControllerDescriptor(controllerType);

            // Act & assert
            ExceptionHelper.ExpectArgumentExceptionNullOrEmpty(
                delegate {
                    cd.FindAction(new Mock<ControllerContext>().Object, null);
                }, "actionName");
        }

        [TestMethod]
        public void FindActionThrowsIfControllerContextIsNull() {
            // Arrange
            Type controllerType = typeof(MyController);
            ReflectedAsyncControllerDescriptor cd = new ReflectedAsyncControllerDescriptor(controllerType);

            // Act & assert
            ExceptionHelper.ExpectArgumentNullException(
                delegate {
                    cd.FindAction(null, "someName");
                }, "controllerContext");
        }

        [TestMethod]
        public void GetCanonicalActionsReturnsEmptyArray() {
            // this method does nothing by default

            // Arrange
            Type controllerType = typeof(MyController);
            ReflectedAsyncControllerDescriptor cd = new ReflectedAsyncControllerDescriptor(controllerType);

            // Act
            ActionDescriptor[] canonicalActions = cd.GetCanonicalActions();

            // Assert
            Assert.AreEqual(0, canonicalActions.Length);
        }

        [TestMethod]
        public void GetCustomAttributesCallsTypeGetCustomAttributes() {
            // Arrange
            object[] expected = new object[0];
            Mock<Type> mockType = new Mock<Type>();
            mockType.Expect(t => t.GetCustomAttributes(true)).Returns(expected);
            ReflectedAsyncControllerDescriptor cd = new ReflectedAsyncControllerDescriptor(mockType.Object);

            // Act
            object[] returned = cd.GetCustomAttributes(true);

            // Assert
            Assert.AreSame(expected, returned);
        }

        [TestMethod]
        public void GetCustomAttributesWithAttributeTypeCallsTypeGetCustomAttributes() {
            // Arrange
            object[] expected = new object[0];
            Mock<Type> mockType = new Mock<Type>();
            mockType.Expect(t => t.GetCustomAttributes(typeof(ObsoleteAttribute), true)).Returns(expected);
            ReflectedAsyncControllerDescriptor cd = new ReflectedAsyncControllerDescriptor(mockType.Object);

            // Act
            object[] returned = cd.GetCustomAttributes(typeof(ObsoleteAttribute), true);

            // Assert
            Assert.AreSame(expected, returned);
        }

        [TestMethod]
        public void IsDefinedCallsTypeIsDefined() {
            // Arrange
            Mock<Type> mockType = new Mock<Type>();
            mockType.Expect(t => t.IsDefined(typeof(ObsoleteAttribute), true)).Returns(true);
            ReflectedAsyncControllerDescriptor cd = new ReflectedAsyncControllerDescriptor(mockType.Object);

            // Act
            bool isDefined = cd.IsDefined(typeof(ObsoleteAttribute), true);

            // Assert
            Assert.IsTrue(isDefined);
        }

        private class MyController : Controller {

            [ActionName("NewName")]
            public void Foo() {
            }

            public void FooCompleted() {
            }

        }

    }
}
