namespace System.Web.Mvc.Test {
    using System;
    using System.Web.Mvc;
    using System.Web.TestUtil;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

    [TestClass]
    public class ControllerDescriptorTest {

        [TestMethod]
        public void ControllerNamePropertyReturnsControllerTypeName() {
            // Arrange
            ControllerDescriptor cd = GetControllerDescriptor(typeof(object));

            // Act
            string name = cd.ControllerName;

            // Assert
            Assert.AreEqual("Object", name);
        }

        [TestMethod]
        public void ControllerNamePropertyReturnsControllerTypeNameWithoutControllerSuffix() {
            // Arrange
            Mock<Type> mockType = new Mock<Type>();
            mockType.Expect(t => t.Name).Returns("somecontroller");
            ControllerDescriptor cd = GetControllerDescriptor(mockType.Object);

            // Act
            string name = cd.ControllerName;

            // Assert
            Assert.AreEqual("some", name);
        }

        [TestMethod]
        public void GetCustomAttributesReturnsEmptyArrayOfAttributeType() {
            // Arrange
            ControllerDescriptor cd = GetControllerDescriptor();

            // Act
            ObsoleteAttribute[] attrs = (ObsoleteAttribute[])cd.GetCustomAttributes(typeof(ObsoleteAttribute), true);

            // Assert
            Assert.AreEqual(0, attrs.Length);
        }

        [TestMethod]
        public void GetCustomAttributesThrowsIfAttributeTypeIsNull() {
            // Arrange
            ControllerDescriptor cd = GetControllerDescriptor();

            // Act & assert
            ExceptionHelper.ExpectArgumentNullException(
                delegate {
                    cd.GetCustomAttributes(null /* attributeType */, true);
                }, "attributeType");
        }

        [TestMethod]
        public void GetCustomAttributesWithoutAttributeTypeCallsGetCustomAttributesWithAttributeType() {
            // Arrange
            object[] expected = new object[0];
            Mock<ControllerDescriptor> mockDescriptor = new Mock<ControllerDescriptor>() { CallBase = true };
            mockDescriptor.Expect(d => d.GetCustomAttributes(typeof(object), true)).Returns(expected);
            ControllerDescriptor cd = mockDescriptor.Object;

            // Act
            object[] returned = cd.GetCustomAttributes(true /* inherit */);

            // Assert
            Assert.AreSame(expected, returned);
        }

        [TestMethod]
        public void IsDefinedReturnsFalse() {
            // Arrange
            ControllerDescriptor cd = GetControllerDescriptor();

            // Act
            bool isDefined = cd.IsDefined(typeof(object), true);

            // Assert
            Assert.IsFalse(isDefined);
        }

        [TestMethod]
        public void IsDefinedThrowsIfAttributeTypeIsNull() {
            // Arrange
            ControllerDescriptor cd = GetControllerDescriptor();

            // Act & assert
            ExceptionHelper.ExpectArgumentNullException(
                delegate {
                    cd.IsDefined(null /* attributeType */, true);
                }, "attributeType");
        }

        private static ControllerDescriptor GetControllerDescriptor() {
            return GetControllerDescriptor(null);
        }

        private static ControllerDescriptor GetControllerDescriptor(Type controllerType) {
            Mock<ControllerDescriptor> mockDescriptor = new Mock<ControllerDescriptor>() { CallBase = true };
            mockDescriptor.Expect(d => d.ControllerType).Returns(controllerType);
            return mockDescriptor.Object;
        }

    }
}
