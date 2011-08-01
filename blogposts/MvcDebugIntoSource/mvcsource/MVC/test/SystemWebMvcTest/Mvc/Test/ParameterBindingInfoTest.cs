namespace System.Web.Mvc.Test {
    using System;
    using System.Collections.Generic;
    using System.Web.Mvc;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class ParameterBindingInfoTest {

        [TestMethod]
        public void BinderProperty() {
            // Arrange
            ParameterBindingInfo bindingInfo = new ParameterBindingInfoHelper();

            // Act & assert
            Assert.IsNull(bindingInfo.Binder, "Binder should default to null.");
        }

        [TestMethod]
        public void ExcludeProperty() {
            // Arrange
            ParameterBindingInfo bindingInfo = new ParameterBindingInfoHelper();

            // Act
            ICollection<string> exclude = bindingInfo.Exclude;

            // Assert
            Assert.IsNotNull(exclude, "Exclude should have returned an empty collection.");
            Assert.AreEqual(0, exclude.Count, "Exclude should have returned an empty collection.");
        }

        [TestMethod]
        public void IncludeProperty() {
            // Arrange
            ParameterBindingInfo bindingInfo = new ParameterBindingInfoHelper();

            // Act
            ICollection<string> include = bindingInfo.Include;

            // Assert
            Assert.IsNotNull(include, "Include should have returned an empty collection.");
            Assert.AreEqual(0, include.Count, "Include should have returned an empty collection.");
        }

        [TestMethod]
        public void PrefixProperty() {
            // Arrange
            ParameterBindingInfo bindingInfo = new ParameterBindingInfoHelper();

            // Act & assert
            Assert.IsNull(bindingInfo.Prefix, "Prefix should default to null.");
        }

        private class ParameterBindingInfoHelper : ParameterBindingInfo {
        }

    }
}
