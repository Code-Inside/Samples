namespace System.Web.Mvc.Test {
    using System;
    using System.Web.Mvc;
    using System.Web.TestUtil;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class ModelBindingContextTest {

        [TestMethod]
        public void ModelNameProperty() {
            // Arrange
            ModelBindingContext bindingContext = new ModelBindingContext();

            // Act & assert
            MemberHelper.TestStringProperty(bindingContext, "ModelName", String.Empty, false /* testDefaultValue */, true /* allowNullAndEmpty */);
        }

        [TestMethod]
        public void ModelStateProperty() {
            // Arrange
            ModelBindingContext bindingContext = new ModelBindingContext();
            ModelStateDictionary modelState = new ModelStateDictionary();

            // Act & assert
            MemberHelper.TestPropertyWithDefaultInstance(bindingContext, "ModelState", modelState);
        }

        [TestMethod]
        public void PropertyFilterPropertyDefaultInstanceReturnsTrueForAnyInput() {
            // Arrange
            ModelBindingContext bindingContext = new ModelBindingContext();

            // Act
            Predicate<string> propertyFilter = bindingContext.PropertyFilter;

            // Assert
            // We can't test all inputs, but at least this gives us high confidence that we ignore the parameter by default
            Assert.IsTrue(propertyFilter(null));
            Assert.IsTrue(propertyFilter(String.Empty));
            Assert.IsTrue(propertyFilter("Foo"));
        }

        [TestMethod]
        public void PropertyFilterPropertyReturnsDefaultInstance() {
            // Arrange
            ModelBindingContext bindingContext = new ModelBindingContext();
            Predicate<string> propertyFilter = _ => true;

            // Act & assert
            MemberHelper.TestPropertyWithDefaultInstance(bindingContext, "PropertyFilter", propertyFilter);
        }

    }
}
