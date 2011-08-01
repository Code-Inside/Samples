namespace System.Web.Mvc.Test {
    using System;
    using System.Web.Mvc;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class BindAttributeTest {
        [TestMethod]
        public void PrefixProperty() {
            // Arrange
            BindAttribute attr = new BindAttribute { Prefix = "somePrefix" };

            // Act & assert
            Assert.AreEqual("somePrefix", attr.Prefix);
        }

        [TestMethod]
        public void PrefixPropertyDefaultsToNull() {
            // Arrange
            BindAttribute attr = new BindAttribute();

            // Act & assert
            Assert.IsNull(attr.Prefix);
        }

        [TestMethod]
        public void IncludePropertyDefaultsToEmptyString() {
            // Arrange
            BindAttribute attr = new BindAttribute { Include = null };

            // Act & assert
            Assert.AreEqual(String.Empty, attr.Include);
        }

        [TestMethod]
        public void ExcludePropertyDefaultsToEmptyString() {
            // Arrange
            BindAttribute attr = new BindAttribute { Exclude = null };

            // Act & assert
            Assert.AreEqual(String.Empty, attr.Exclude);
        }

        [TestMethod]
        public void IsPropertyAllowedReturnsFalseForBlacklistedPropertiesIfBindPropertiesIsExclude() {
            // Setup
            BindAttribute attr = new BindAttribute { Exclude = "FOO,BAZ" };

            // Act & assert
            Assert.IsFalse(attr.IsPropertyAllowed("foo"));
            Assert.IsTrue(attr.IsPropertyAllowed("bar"));
            Assert.IsFalse(attr.IsPropertyAllowed("baz"));
        }

        [TestMethod]
        public void IsPropertyAllowedReturnsTrueAlwaysIfBindPropertiesIsAll() {
            // Setup
            BindAttribute attr = new BindAttribute();

            // Act & assert
            Assert.IsTrue(attr.IsPropertyAllowed("foo"));
            Assert.IsTrue(attr.IsPropertyAllowed("bar"));
            Assert.IsTrue(attr.IsPropertyAllowed("baz"));
        }

        [TestMethod]
        public void IsPropertyAllowedReturnsTrueForWhitelistedPropertiesIfBindPropertiesIsInclude() {
            // Setup
            BindAttribute attr = new BindAttribute { Include = "FOO,BAR" };

            // Act & assert
            Assert.IsTrue(attr.IsPropertyAllowed("foo"));
            Assert.IsTrue(attr.IsPropertyAllowed("bar"));
            Assert.IsFalse(attr.IsPropertyAllowed("baz"));
        }

        [TestMethod]
        public void IsPropertyAllowedReturnsFalseForBlacklistOverridingWhitelistedProperties() {
            // Setup
            BindAttribute attr = new BindAttribute { Include = "FOO,BAR", Exclude = "bar,QUx" };

            // Act & assert
            Assert.IsTrue(attr.IsPropertyAllowed("foo"));
            Assert.IsFalse(attr.IsPropertyAllowed("bar"));
            Assert.IsFalse(attr.IsPropertyAllowed("baz"));
            Assert.IsFalse(attr.IsPropertyAllowed("qux"));
        }
    }
}
