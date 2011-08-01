namespace System.Web.Mvc.Test {
    using System;
    using System.Linq;
    using System.Reflection;
    using System.Web.Mvc;
    using System.Web.TestUtil;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

    [TestClass]
    public class ReflectedControllerDescriptorTest {

        [TestMethod]
        public void ConstructorSetsControllerTypeProperty() {
            // Arrange
            Type controllerType = typeof(string);

            // Act
            ReflectedControllerDescriptor cd = new ReflectedControllerDescriptor(controllerType);

            // Assert
            Assert.AreSame(controllerType, cd.ControllerType);
        }

        [TestMethod]
        public void ConstructorThrowsIfControllerTypeIsNull() {
            // Act & assert
            ExceptionHelper.ExpectArgumentNullException(
                delegate {
                    new ReflectedControllerDescriptor(null);
                }, "controllerType");
        }

        [TestMethod]
        public void FindActionReturnsActionDescriptorIfFound() {
            // Arrange
            Type controllerType = typeof(MyController);
            MethodInfo targetMethod = controllerType.GetMethod("AliasedMethod");
            ReflectedControllerDescriptor cd = new ReflectedControllerDescriptor(controllerType);

            // Act
            ActionDescriptor ad = cd.FindAction(new Mock<ControllerContext>().Object, "NewName");

            // Assert
            Assert.AreEqual("NewName", ad.ActionName);
            Assert.IsInstanceOfType(ad, typeof(ReflectedActionDescriptor));
            Assert.AreSame(targetMethod, ((ReflectedActionDescriptor)ad).MethodInfo, "MethodInfo pointed to wrong method.");
            Assert.AreSame(cd, ad.ControllerDescriptor, "Controller did not point back to correct descriptor.");
        }

        [TestMethod]
        public void FindActionReturnsNullIfNoActionFound() {
            // Arrange
            Type controllerType = typeof(MyController);
            ReflectedControllerDescriptor cd = new ReflectedControllerDescriptor(controllerType);

            // Act
            ActionDescriptor ad = cd.FindAction(new Mock<ControllerContext>().Object, "NonExistent");

            // Assert
            Assert.IsNull(ad);
        }

        [TestMethod]
        public void FindActionThrowsIfActionNameIsEmpty() {
            // Arrange
            Type controllerType = typeof(MyController);
            ReflectedControllerDescriptor cd = new ReflectedControllerDescriptor(controllerType);

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
            ReflectedControllerDescriptor cd = new ReflectedControllerDescriptor(controllerType);

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
            ReflectedControllerDescriptor cd = new ReflectedControllerDescriptor(controllerType);

            // Act & assert
            ExceptionHelper.ExpectArgumentNullException(
                delegate {
                    cd.FindAction(null, "someName");
                }, "controllerContext");
        }

        [TestMethod]
        public void GetCanonicalActionsWrapsMethodInfos() {
            // Arrange
            Type controllerType = typeof(MyController);
            MethodInfo mInfo0 = controllerType.GetMethod("AliasedMethod");
            MethodInfo mInfo1 = controllerType.GetMethod("NonAliasedMethod");
            ReflectedControllerDescriptor cd = new ReflectedControllerDescriptor(controllerType);

            // Act
            ActionDescriptor[] aDescsFirstCall = cd.GetCanonicalActions();
            ActionDescriptor[] aDescsSecondCall = cd.GetCanonicalActions();

            // Assert
            Assert.AreNotSame(aDescsFirstCall, aDescsSecondCall, "GetCanonicalActions() should return a new array on each invocation.");
            Assert.IsTrue(aDescsFirstCall.SequenceEqual(aDescsSecondCall), "Array elements were not equal.");
            Assert.AreEqual(2, aDescsFirstCall.Length);

            ReflectedActionDescriptor aDesc0 = aDescsFirstCall[0] as ReflectedActionDescriptor;
            ReflectedActionDescriptor aDesc1 = aDescsFirstCall[1] as ReflectedActionDescriptor;

            Assert.IsNotNull(aDesc0, "Action 0 should have been of type ReflectedActionDescriptor.");
            Assert.AreEqual("AliasedMethod", aDesc0.ActionName, "Action 0 Name did not match.");
            Assert.AreSame(mInfo0, aDesc0.MethodInfo, "Action 0 MethodInfo did not match.");
            Assert.AreSame(cd, aDesc0.ControllerDescriptor, "Action 0 Controller did not point back to controller descriptor.");
            Assert.IsNotNull(aDesc1, "Action 1 should have been of type ReflectedActionDescriptor.");
            Assert.AreEqual("NonAliasedMethod", aDesc1.ActionName, "Action 1 Name did not match.");
            Assert.AreSame(mInfo1, aDesc1.MethodInfo, "Action 1 MethodInfo did not match.");
            Assert.AreSame(cd, aDesc1.ControllerDescriptor, "Action 1 Controller did not point back to controller descriptor.");
        }

        [TestMethod]
        public void GetCustomAttributesCallsTypeGetCustomAttributes() {
            // Arrange
            object[] expected = new object[0];
            Mock<Type> mockType = new Mock<Type>();
            mockType.Expect(t => t.GetCustomAttributes(true)).Returns(expected);
            ReflectedControllerDescriptor cd = new ReflectedControllerDescriptor(mockType.Object);

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
            ReflectedControllerDescriptor cd = new ReflectedControllerDescriptor(mockType.Object);

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
            ReflectedControllerDescriptor cd = new ReflectedControllerDescriptor(mockType.Object);

            // Act
            bool isDefined = cd.IsDefined(typeof(ObsoleteAttribute), true);

            // Assert
            Assert.IsTrue(isDefined);
        }

        private class MyController : Controller {

            [ActionName("NewName")]
            public void AliasedMethod() {
            }

            public void NonAliasedMethod() {
            }

            public void GenericMethod<T>() {
            }

            private void PrivateMethod() {
            }

        }

    }
}
