namespace System.Web.Mvc.Test {
    using System;
    using System.Globalization;
    using System.Web.Mvc;
    using System.Web.TestUtil;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class ValueProviderResultTest {

        [TestMethod]
        public void ConstructorSetsProperties() {
            // Arrange
            object rawValue = new object();
            string attemptedValue = "some string";
            CultureInfo culture = CultureInfo.GetCultureInfo("fr-FR");

            // Act
            ValueProviderResult result = new ValueProviderResult(rawValue, attemptedValue, culture);

            // Assert
            Assert.AreSame(rawValue, result.RawValue);
            Assert.AreSame(attemptedValue, result.AttemptedValue);
            Assert.AreSame(culture, result.Culture);
        }

        [TestMethod]
        public void ConvertToCanConvertArraysToArrays() {
            // Arrange
            ValueProviderResult vpr = new ValueProviderResult(new int[] { 1, 20, 42 }, "", CultureInfo.InvariantCulture);

            // Act
            string[] converted = (string[])vpr.ConvertTo(typeof(string[]));

            // Assert
            Assert.IsNotNull(converted);
            Assert.AreEqual(3, converted.Length);
            Assert.AreEqual("1", converted[0]);
            Assert.AreEqual("20", converted[1]);
            Assert.AreEqual("42", converted[2]);
        }

        [TestMethod]
        public void ConvertToCanConvertArraysToSingleElements() {
            // Arrange
            ValueProviderResult vpr = new ValueProviderResult(new int[] { 1, 20, 42 }, "", CultureInfo.InvariantCulture);

            // Act
            string converted = (string)vpr.ConvertTo(typeof(string));

            // Assert
            Assert.AreEqual("1", converted);
        }

        [TestMethod]
        public void ConvertToCanConvertSingleElementsToArrays() {
            // Arrange
            ValueProviderResult vpr = new ValueProviderResult(42, "", CultureInfo.InvariantCulture);

            // Act
            string[] converted = (string[])vpr.ConvertTo(typeof(string[]));

            // Assert
            Assert.IsNotNull(converted);
            Assert.AreEqual(1, converted.Length);
            Assert.AreEqual("42", converted[0]);
        }

        [TestMethod]
        public void ConvertToCanConvertSingleElementsToSingleElements() {
            // Arrange
            ValueProviderResult vpr = new ValueProviderResult(42, "", CultureInfo.InvariantCulture);

            // Act
            string converted = (string)vpr.ConvertTo(typeof(string));

            // Assert
            Assert.IsNotNull(converted);
            Assert.AreEqual("42", converted);
        }

        [TestMethod]
        public void ConvertToChecksTypeConverterCanConvertFrom() {
            // Arrange
            object original = "someValue";
            ValueProviderResult vpr = new ValueProviderResult(original, null, CultureInfo.GetCultureInfo("fr-FR"));

            // Act
            DefaultModelBinderTest.StringContainer returned = (DefaultModelBinderTest.StringContainer)vpr.ConvertTo(typeof(DefaultModelBinderTest.StringContainer));

            // Assert
            Assert.AreEqual(returned.Value, "someValue (fr-FR)");
        }

        [TestMethod]
        public void ConvertToChecksTypeConverterCanConvertTo() {
            // Arrange
            object original = new DefaultModelBinderTest.StringContainer("someValue");
            ValueProviderResult vpr = new ValueProviderResult(original, "", CultureInfo.GetCultureInfo("en-US"));

            // Act
            string returned = (string)vpr.ConvertTo(typeof(string));

            // Assert
            Assert.AreEqual(returned, "someValue (en-US)");
        }

        [TestMethod]
        public void ConvertToReturnsNullIfArrayElementValueIsNull() {
            // Arrange
            ValueProviderResult vpr = new ValueProviderResult(new string[] { null }, null, CultureInfo.InvariantCulture);

            // Act
            object outValue = vpr.ConvertTo(typeof(int));

            // Assert
            Assert.IsNull(outValue);
        }

        [TestMethod]
        public void ConvertToReturnsNullIfTryingToConvertEmptyArrayToSingleElement() {
            // Arrange
            ValueProviderResult vpr = new ValueProviderResult(new int[0], "", CultureInfo.InvariantCulture);

            // Act
            object outValue = vpr.ConvertTo(typeof(int));

            // Assert
            Assert.IsNull(outValue);
        }

        [TestMethod]
        public void ConvertToReturnsNullIfValueIsEmptyString() {
            // Arrange
            ValueProviderResult vpr = new ValueProviderResult("", null, CultureInfo.InvariantCulture);

            // Act
            object outValue = vpr.ConvertTo(typeof(int));

            // Assert
            Assert.IsNull(outValue);
        }

        [TestMethod]
        public void ConvertToReturnsNullIfValueIsNull() {
            // Arrange
            ValueProviderResult vpr = new ValueProviderResult(null /* rawValue */, null /* attemptedValue */, CultureInfo.InvariantCulture);

            // Act
            object outValue = vpr.ConvertTo(typeof(int[]));

            // Assert
            Assert.IsNull(outValue);
        }


        [TestMethod]
        public void ConvertToReturnsValueIfArrayElementInstanceOfDestinationType() {
            // Arrange
            ValueProviderResult vpr = new ValueProviderResult(new object[] { "some string" }, null, CultureInfo.InvariantCulture);

            // Act
            object outValue = vpr.ConvertTo(typeof(string));

            // Assert
            Assert.AreEqual("some string", outValue);
        }

        [TestMethod]
        public void ConvertToReturnsValueIfInstanceOfDestinationType() {
            // Arrange
            string[] original = new string[] { "some string" };
            ValueProviderResult vpr = new ValueProviderResult(original, null, CultureInfo.InvariantCulture);            

            // Act
            object outValue = vpr.ConvertTo(typeof(string[]));

            // Assert
            Assert.AreSame(original, outValue);
        }

        [TestMethod]
        public void ConvertToThrowsIfConverterThrows() {
            // Arrange
            ValueProviderResult vpr = new ValueProviderResult("x", null, CultureInfo.InvariantCulture);
            Type destinationType = typeof(DefaultModelBinderTest.StringContainer);

            // Act & Assert
            // Will throw since the custom converter assumes the first 5 characters to be digits
            InvalidOperationException exception = ExceptionHelper.ExpectInvalidOperationException(
                delegate {
                    vpr.ConvertTo(destinationType);
                },
                "The parameter conversion from type 'System.String' to type 'System.Web.Mvc.Test.DefaultModelBinderTest+StringContainer' failed. See the inner exception for more information.");

            Exception innerException = exception.InnerException;
            Assert.AreEqual("Value must have at least 3 characters.", innerException.Message);
        }

        [TestMethod]
        public void ConvertToThrowsIfNoConverterExists() {
            // Arrange
            ValueProviderResult vpr = new ValueProviderResult("x", null, CultureInfo.InvariantCulture);
            Type destinationType = typeof(MyClassWithoutConverter);

            // Act & Assert
            ExceptionHelper.ExpectInvalidOperationException(
                delegate {
                    vpr.ConvertTo(destinationType);
                },
                "The parameter conversion from type 'System.String' to type 'System.Web.Mvc.Test.ValueProviderResultTest+MyClassWithoutConverter' failed because no TypeConverter can convert between these types.");
        }

        [TestMethod]
        public void ConvertToThrowsIfTypeIsNull() {
            // Arrange
            ValueProviderResult vpr = new ValueProviderResult("x", null, CultureInfo.InvariantCulture);

            // Act & Assert
            ExceptionHelper.ExpectArgumentNullException(
                delegate {
                    vpr.ConvertTo(null);
                }, "type");
        }

        [TestMethod]
        public void ConvertToUsesProvidedCulture() {
            // Arrange
            object original = "someValue";
            CultureInfo gbCulture = CultureInfo.GetCultureInfo("en-GB");
            ValueProviderResult vpr = new ValueProviderResult(original, null, CultureInfo.GetCultureInfo("fr-FR"));

            // Act
            DefaultModelBinderTest.StringContainer returned = (DefaultModelBinderTest.StringContainer)vpr.ConvertTo(typeof(DefaultModelBinderTest.StringContainer), gbCulture);

            // Assert
            Assert.AreEqual(returned.Value, "someValue (en-GB)");
        }

        [TestMethod]
        public void CulturePropertyDefaultsToInvariantCulture() {
            // Arrange
            ValueProviderResult result = new ValueProviderResult(null, null, null);

            // Act & assert
            Assert.AreSame(CultureInfo.InvariantCulture, result.Culture);
        }

        private class MyClassWithoutConverter {
        }
    }
}
