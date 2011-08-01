namespace System.Web.Mvc.Test {
    using System;
    using System.Web.Mvc;
    using System.Web.TestUtil;
    using System.Web.UI;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class OutputCacheAttributeTest {

        [TestMethod]
        public void CacheProfileProperty() {
            // Arrange
            OutputCacheAttribute attr = new OutputCacheAttribute();

            // Act & assert
            MemberHelper.TestStringProperty(attr, "CacheProfile", String.Empty, false /* testDefaultValue */, true /* allowNullAndEmpty */);
        }

        [TestMethod]
        public void CacheSettingsProperty() {
            // Arrange
            OutputCacheAttribute attr = new OutputCacheAttribute() {
                CacheProfile = "SomeProfile",
                Duration = 50,
                Location = OutputCacheLocation.Downstream,
                NoStore = true,
                SqlDependency = "SomeSqlDependency",
                VaryByContentEncoding = "SomeContentEncoding",
                VaryByCustom = "SomeCustom",
                VaryByHeader = "SomeHeader",
                VaryByParam = "SomeParam",
            };

            // Act
            OutputCacheParameters cacheSettings = attr.CacheSettings;

            // Assert
            Assert.AreEqual("SomeProfile", cacheSettings.CacheProfile);
            Assert.AreEqual(50, cacheSettings.Duration);
            Assert.AreEqual(OutputCacheLocation.Downstream, cacheSettings.Location);
            Assert.AreEqual(true, cacheSettings.NoStore);
            Assert.AreEqual("SomeSqlDependency", cacheSettings.SqlDependency);
            Assert.AreEqual("SomeContentEncoding", cacheSettings.VaryByContentEncoding);
            Assert.AreEqual("SomeCustom", cacheSettings.VaryByCustom);
            Assert.AreEqual("SomeHeader", cacheSettings.VaryByHeader);
            Assert.AreEqual("SomeParam", cacheSettings.VaryByParam);
        }

        [TestMethod]
        public void DurationProperty() {
            // Arrange
            OutputCacheAttribute attr = new OutputCacheAttribute();

            // Act & assert
            MemberHelper.TestInt32Property(attr, "Duration", 10, 20);
        }

        [TestMethod]
        public void LocationProperty() {
            // Arrange
            OutputCacheAttribute attr = new OutputCacheAttribute();

            // Act & assert
            MemberHelper.TestPropertyValue(attr, "Location", OutputCacheLocation.ServerAndClient);
        }

        [TestMethod]
        public void NoStoreProperty() {
            // Arrange
            OutputCacheAttribute attr = new OutputCacheAttribute();

            // Act & assert
            MemberHelper.TestBooleanProperty(attr, "NoStore", false /* initialValue */, false /* testDefaultValue */);
        }

        [TestMethod]
        public void OnResultExecutingThrowsIfFilterContextIsNull() {
            // Arrange
            OutputCacheAttribute attr = new OutputCacheAttribute();

            // Act & assert
            ExceptionHelper.ExpectArgumentNullException(
                delegate {
                    attr.OnResultExecuting(null);
                }, "filterContext");
        }

        [TestMethod]
        public void SqlDependencyProperty() {
            // Arrange
            OutputCacheAttribute attr = new OutputCacheAttribute();

            // Act & assert
            MemberHelper.TestStringProperty(attr, "SqlDependency", String.Empty, false /* testDefaultValue */, true /* allowNullAndEmpty */);
        }

        [TestMethod]
        public void VaryByContentEncodingProperty() {
            // Arrange
            OutputCacheAttribute attr = new OutputCacheAttribute();

            // Act & assert
            MemberHelper.TestStringProperty(attr, "VaryByContentEncoding", String.Empty, false /* testDefaultValue */, true /* allowNullAndEmpty */);
        }

        [TestMethod]
        public void VaryByCustomProperty() {
            // Arrange
            OutputCacheAttribute attr = new OutputCacheAttribute();

            // Act & assert
            MemberHelper.TestStringProperty(attr, "VaryByCustom", String.Empty, false /* testDefaultValue */, true /* allowNullAndEmpty */);
        }

        [TestMethod]
        public void VaryByHeaderProperty() {
            // Arrange
            OutputCacheAttribute attr = new OutputCacheAttribute();

            // Act & assert
            MemberHelper.TestStringProperty(attr, "VaryByHeader", String.Empty, false /* testDefaultValue */, true /* allowNullAndEmpty */);
        }

        [TestMethod]
        public void VaryByParamProperty() {
            // Arrange
            OutputCacheAttribute attr = new OutputCacheAttribute();

            // Act & assert
            MemberHelper.TestStringProperty(attr, "VaryByParam", String.Empty, false /* testDefaultValue */, true /* allowNullAndEmpty */);
        }

    }
}
