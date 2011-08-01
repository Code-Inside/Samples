namespace Microsoft.Web.Mvc.Test {
    using System;
    using System.Collections.Generic;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.Web.Mvc;

    [TestClass]
    public class TypeHelpersTest {

        [TestMethod]
        public void TypeAllowsNullValueReturnsFalseForNonNullableGenericValueType() {
            Assert.IsFalse(TypeHelpers.TypeAllowsNullValue(typeof(KeyValuePair<int, string>)));
        }

        [TestMethod]
        public void TypeAllowsNullValueReturnsFalseForNonNullableGenericValueTypeDefinition() {
            Assert.IsFalse(TypeHelpers.TypeAllowsNullValue(typeof(KeyValuePair<,>)));
        }

        [TestMethod]
        public void TypeAllowsNullValueReturnsFalseForNonNullableValueType() {
            Assert.IsFalse(TypeHelpers.TypeAllowsNullValue(typeof(int)));
        }

        [TestMethod]
        public void TypeAllowsNullValueReturnsTrueForInterfaceType() {
            Assert.IsTrue(TypeHelpers.TypeAllowsNullValue(typeof(IDisposable)));
        }

        [TestMethod]
        public void TypeAllowsNullValueReturnsTrueForNullableType() {
            Assert.IsTrue(TypeHelpers.TypeAllowsNullValue(typeof(int?)));
        }

        [TestMethod]
        public void TypeAllowsNullValueReturnsTrueForReferenceType() {
            Assert.IsTrue(TypeHelpers.TypeAllowsNullValue(typeof(object)));
        }

        [TestMethod]
        public void TypeIsParameterlessDelegateReturnsFalseForNonDelegateType() {
            // Arrange
            Type type = typeof(string);

            // Act
            bool returned = TypeHelpers.TypeIsParameterlessDelegate(type);

            // Assert
            Assert.IsFalse(returned);
        }

        [TestMethod]
        public void TypeIsParameterlessDelegateReturnsFalseForDelegateTypeWithoutParameters() {
            // Arrange
            Type type = typeof(Action);

            // Act
            bool returned = TypeHelpers.TypeIsParameterlessDelegate(type);

            // Assert
            Assert.IsTrue(returned);
        }

        [TestMethod]
        public void TypeIsParameterlessDelegateReturnsFalseForDelegateTypeWithParameters() {
            // Arrange
            Type type = typeof(Action<string>);

            // Act
            bool returned = TypeHelpers.TypeIsParameterlessDelegate(type);

            // Assert
            Assert.IsFalse(returned);
        }

    }
}
