namespace Microsoft.Web.Mvc.Test {
    using System;
    using System.Web.Mvc;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.Web.Mvc;

    [TestClass]
    public class ByteArrayModelBinderTest {
        internal const string Base64TestString = "Fys1";
        internal static readonly byte[] Base64TestBytes = new byte[] { 23, 43, 53 };

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

            ByteArrayModelBinder binder = new ByteArrayModelBinder();

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

            ByteArrayModelBinder binder = new ByteArrayModelBinder();

            // Act
            object binderResult = binder.BindModel(null, bindingContext);

            // Assert
            Assert.IsNull(binderResult);
        }

        [TestMethod]
        public void BindModelWithBase64QuotedValueReturnsByteArray() {
            // Arrange
            string base64Value = Base64TestString;
            ValueProviderDictionary valueProvider = new ValueProviderDictionary(null) {
                { "foo", new ValueProviderResult("\"" + base64Value + "\"", "\"" + base64Value + "\"", null) }
            };

            ModelBindingContext bindingContext = new ModelBindingContext() {
                ModelName = "foo",
                ValueProvider = valueProvider
            };

            ByteArrayModelBinder binder = new ByteArrayModelBinder();

            // Act
            byte[] boundValue = binder.BindModel(null, bindingContext) as byte[];

            // Assert
            CollectionAssert.AreEqual(Base64TestBytes, boundValue);
        }

        [TestMethod]
        public void BindModelWithBase64UnquotedValueReturnsByteArray() {
            // Arrange
            string base64Value = Base64TestString;
            ValueProviderDictionary valueProvider = new ValueProviderDictionary(null) {
                { "foo", new ValueProviderResult(base64Value, base64Value, null) }
            };

            ModelBindingContext bindingContext = new ModelBindingContext() {
                ModelName = "foo",
                ValueProvider = valueProvider
            };

            ByteArrayModelBinder binder = new ByteArrayModelBinder();

            // Act
            byte[] boundValue = binder.BindModel(null, bindingContext) as byte[];

            // Assert
            CollectionAssert.AreEqual(Base64TestBytes, boundValue);
        }
    }
}
