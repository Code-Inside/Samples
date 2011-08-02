namespace Microsoft.Web.Mvc.Test {
    using System;
    using System.Data.Linq;
    using System.Web.Mvc;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.Web.Mvc;

    [TestClass]
    public class LinqBinaryModelBinderTest {
        [TestMethod]
        public void BindModelWithNonExistentValueReturnsNull() {
            // Arrange
            ValueProviderDictionary valueProvider = new ValueProviderDictionary(null) {
                { "foo", new ValueProviderResult(null, null, null) }
            };

            ModelBindingContext bindingContext = new ModelBindingContext() {
                ModelName = "foo",
                ValueProvider = valueProvider
            };

            LinqBinaryModelBinder binder = new LinqBinaryModelBinder();

            // Act
            object binderResult = binder.BindModel(null, bindingContext);

            // Assert
            Assert.IsNull(binderResult);
        }

        [TestMethod]
        public void BinderWithEmptyStringValueReturnsNull() {
            // Arrange
            ValueProviderDictionary valueProvider = new ValueProviderDictionary(null) {
                { "foo", new ValueProviderResult(String.Empty, null, null) }
            };

            ModelBindingContext bindingContext = new ModelBindingContext() {
                ModelName = "foo",
                ValueProvider = valueProvider
            };

            LinqBinaryModelBinder binder = new LinqBinaryModelBinder();

            // Act
            object binderResult = binder.BindModel(null, bindingContext);

            // Assert
            Assert.IsNull(binderResult);
        }

        [TestMethod]
        public void BindModelWithBase64QuotedValueReturnsBinary() {
            // Arrange
            string base64Value = ByteArrayModelBinderTest.Base64TestString;
            ValueProviderDictionary valueProvider = new ValueProviderDictionary(null) {
                { "foo", new ValueProviderResult("\"" + base64Value + "\"", "\"" + base64Value + "\"", null) }
            };

            ModelBindingContext bindingContext = new ModelBindingContext() {
                ModelName = "foo",
                ValueProvider = valueProvider
            };

            LinqBinaryModelBinder binder = new LinqBinaryModelBinder();

            // Act
            Binary boundValue = binder.BindModel(null, bindingContext) as Binary;

            // Assert
            Assert.AreEqual(ByteArrayModelBinderTest.Base64TestBytes, boundValue);
        }

        [TestMethod]
        public void BindModelWithBase64UnquotedValueReturnsBinary() {
            // Arrange
            string base64Value = ByteArrayModelBinderTest.Base64TestString;
            ValueProviderDictionary valueProvider = new ValueProviderDictionary(null) {
                { "foo", new ValueProviderResult(base64Value, base64Value, null) }
            };

            ModelBindingContext bindingContext = new ModelBindingContext() {
                ModelName = "foo",
                ValueProvider = valueProvider
            };

            LinqBinaryModelBinder binder = new LinqBinaryModelBinder();

            // Act
            Binary boundValue = binder.BindModel(null, bindingContext) as Binary;

            // Assert
            Assert.AreEqual(ByteArrayModelBinderTest.Base64TestBytes, boundValue);
        }
    }
}
