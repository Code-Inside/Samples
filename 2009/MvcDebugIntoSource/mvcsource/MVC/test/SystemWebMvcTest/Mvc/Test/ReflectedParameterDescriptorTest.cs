namespace System.Web.Mvc.Test {
    using System;
    using System.Reflection;
    using System.Web.Mvc;
    using System.Web.TestUtil;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

    [TestClass]
    public class ReflectedParameterDescriptorTest {

        [TestMethod]
        public void ConstructorSetsActionDescriptorProperty() {
            // Arrange
            ParameterInfo pInfo = typeof(MyController).GetMethod("Foo").GetParameters()[0];
            ActionDescriptor ad = new Mock<ActionDescriptor>().Object;

            // Act
            ReflectedParameterDescriptor pd = new ReflectedParameterDescriptor(pInfo, ad);

            // Assert
            Assert.AreSame(ad, pd.ActionDescriptor);
        }

        [TestMethod]
        public void ConstructorSetsParameterInfo() {
            // Arrange
            ParameterInfo pInfo = typeof(MyController).GetMethod("Foo").GetParameters()[0];

            // Act
            ReflectedParameterDescriptor pd = new ReflectedParameterDescriptor(pInfo, new Mock<ActionDescriptor>().Object);

            // Assert
            Assert.AreSame(pInfo, pd.ParameterInfo);
        }

        [TestMethod]
        public void ConstructorThrowsIfActionDescriptorIsNull() {
            // Act & assert
            ExceptionHelper.ExpectArgumentNullException(
                delegate {
                    new ReflectedParameterDescriptor(new Mock<ParameterInfo>().Object, null);
                }, "actionDescriptor");
        }

        [TestMethod]
        public void ConstructorThrowsIfParameterInfoIsNull() {
            // Act & assert
            ExceptionHelper.ExpectArgumentNullException(
                delegate {
                    new ReflectedParameterDescriptor(null, new Mock<ActionDescriptor>().Object);
                }, "parameterInfo");
        }

        [TestMethod]
        public void GetCustomAttributesCallsParameterInfoGetCustomAttributes() {
            // Arrange
            object[] expected = new object[0];
            Mock<ParameterInfo> mockParameter = new Mock<ParameterInfo>();
            mockParameter.Expect(pi => pi.Member).Returns(new Mock<MemberInfo>().Object);
            mockParameter.Expect(pi => pi.GetCustomAttributes(true)).Returns(expected);
            ReflectedParameterDescriptor pd = GetParameterDescriptor(mockParameter.Object);

            // Act
            object[] returned = pd.GetCustomAttributes(true);

            // Assert
            Assert.AreSame(expected, returned);
        }

        [TestMethod]
        public void GetCustomAttributesWithAttributeTypeCallsParameterInfoGetCustomAttributes() {
            // Arrange
            object[] expected = new object[0];
            Mock<ParameterInfo> mockParameter = new Mock<ParameterInfo>();
            mockParameter.Expect(pi => pi.Member).Returns(new Mock<MemberInfo>().Object);
            mockParameter.Expect(pi => pi.GetCustomAttributes(typeof(ObsoleteAttribute), true)).Returns(expected);
            ReflectedParameterDescriptor pd = GetParameterDescriptor(mockParameter.Object);

            // Act
            object[] returned = pd.GetCustomAttributes(typeof(ObsoleteAttribute), true);

            // Assert
            Assert.AreSame(expected, returned);
        }

        [TestMethod]
        public void IsDefinedCallsParameterInfoIsDefined() {
            // Arrange
            Mock<ParameterInfo> mockParameter = new Mock<ParameterInfo>();
            mockParameter.Expect(pi => pi.Member).Returns(new Mock<MemberInfo>().Object);
            mockParameter.Expect(pi => pi.IsDefined(typeof(ObsoleteAttribute), true)).Returns(true);
            ReflectedParameterDescriptor pd = GetParameterDescriptor(mockParameter.Object);

            // Act
            bool isDefined = pd.IsDefined(typeof(ObsoleteAttribute), true);

            // Assert
            Assert.IsTrue(isDefined);
        }

        [TestMethod]
        public void ParameterNameProperty() {
            // Arrange
            ParameterInfo pInfo = typeof(MyController).GetMethod("Foo").GetParameters()[0];

            // Act
            ReflectedParameterDescriptor pd = GetParameterDescriptor(pInfo);

            // Assert
            Assert.AreEqual("s1", pd.ParameterName);
        }

        [TestMethod]
        public void ParameterTypeProperty() {
            // Arrange
            ParameterInfo pInfo = typeof(MyController).GetMethod("Foo").GetParameters()[0];

            // Act
            ReflectedParameterDescriptor pd = GetParameterDescriptor(pInfo);

            // Assert
            Assert.AreEqual(typeof(string), pd.ParameterType);
        }

        private static ReflectedParameterDescriptor GetParameterDescriptor(ParameterInfo parameterInfo) {
            return new ReflectedParameterDescriptor(parameterInfo, new Mock<ActionDescriptor>().Object);
        }

        private class MyController : Controller {
            public void Foo(string s1) {
            }
        }

    }
}
