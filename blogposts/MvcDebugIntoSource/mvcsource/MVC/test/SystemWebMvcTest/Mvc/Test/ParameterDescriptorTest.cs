namespace System.Web.Mvc.Test {
    using System;
    using System.Reflection;
    using System.Web.Mvc;
    using System.Web.TestUtil;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

    [TestClass]
    public class ParameterDescriptorTest {

        [TestMethod]
        public void BindingInfoProperty() {
            // Arrange
            Type emptyBindingInfoType = typeof(ParameterDescriptor).GetNestedType("EmptyParameterBindingInfo", BindingFlags.NonPublic);
            ParameterDescriptor pd = GetParameterDescriptor(typeof(object), "someName");

            // Act
            ParameterBindingInfo bindingInfo = pd.BindingInfo;

            // Assert
            Assert.IsInstanceOfType(bindingInfo, emptyBindingInfoType);
        }

        [TestMethod]
        public void GetCustomAttributesReturnsEmptyArrayOfAttributeType() {
            // Arrange
            ParameterDescriptor pd = GetParameterDescriptor();

            // Act
            ObsoleteAttribute[] attrs = (ObsoleteAttribute[])pd.GetCustomAttributes(typeof(ObsoleteAttribute), true);

            // Assert
            Assert.AreEqual(0, attrs.Length);
        }

        [TestMethod]
        public void GetCustomAttributesThrowsIfAttributeTypeIsNull() {
            // Arrange
            ParameterDescriptor pd = GetParameterDescriptor();

            // Act & assert
            ExceptionHelper.ExpectArgumentNullException(
                delegate {
                    pd.GetCustomAttributes(null /* attributeType */, true);
                }, "attributeType");
        }

        [TestMethod]
        public void GetCustomAttributesWithoutAttributeTypeCallsGetCustomAttributesWithAttributeType() {
            // Arrange
            object[] expected = new object[0];
            Mock<ParameterDescriptor> mockDescriptor = new Mock<ParameterDescriptor>() { CallBase = true };
            mockDescriptor.Expect(d => d.GetCustomAttributes(typeof(object), true)).Returns(expected);
            ParameterDescriptor pd = mockDescriptor.Object;

            // Act
            object[] returned = pd.GetCustomAttributes(true /* inherit */);

            // Assert
            Assert.AreSame(expected, returned);
        }

        [TestMethod]
        public void IsDefinedReturnsFalse() {
            // Arrange
            ParameterDescriptor pd = GetParameterDescriptor();

            // Act
            bool isDefined = pd.IsDefined(typeof(object), true);

            // Assert
            Assert.IsFalse(isDefined);
        }

        [TestMethod]
        public void IsDefinedThrowsIfAttributeTypeIsNull() {
            // Arrange
            ParameterDescriptor pd = GetParameterDescriptor();

            // Act & assert
            ExceptionHelper.ExpectArgumentNullException(
                delegate {
                    pd.IsDefined(null /* attributeType */, true);
                }, "attributeType");
        }

        private static ParameterDescriptor GetParameterDescriptor() {
            return GetParameterDescriptor(typeof(object), "someName");
        }

        private static ParameterDescriptor GetParameterDescriptor(Type type, string name) {
            Mock<ParameterDescriptor> mockDescriptor = new Mock<ParameterDescriptor>() { CallBase = true };
            mockDescriptor.Expect(d => d.ParameterType).Returns(type);
            mockDescriptor.Expect(d => d.ParameterName).Returns(name);
            return mockDescriptor.Object;
        }

    }
}
