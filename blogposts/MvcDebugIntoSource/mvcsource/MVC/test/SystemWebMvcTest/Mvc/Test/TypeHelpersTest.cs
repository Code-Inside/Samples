namespace System.Web.Mvc.Test {
    using System;
    using System.Collections.Generic;
    using System.Web.Mvc;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

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

    }
}
