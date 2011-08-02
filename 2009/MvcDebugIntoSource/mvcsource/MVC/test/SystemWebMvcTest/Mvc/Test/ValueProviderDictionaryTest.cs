namespace System.Web.Mvc.Test {
    using System;
    using System.Collections.Specialized;
    using System.Globalization;
    using System.Threading;
    using System.Web.Mvc;
    using System.Web.Routing;
    using System.Web.TestUtil;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

    [TestClass]
    public class ValueProviderDictionaryTest {

        [TestMethod]
        public void ConstructorCreatesEmptyDictionaryIfControllerContextIsNull() {
            // Act
            ValueProviderDictionary dict = new ValueProviderDictionary(null);

            // Assert
            Assert.AreEqual(0, dict.Count);
        }

        [TestMethod]
        public void ControllerContextProperty() {
            // Arrange
            ControllerContext expected = GetControllerContext();
            ValueProviderDictionary dict = new ValueProviderDictionary(expected);

            // Act
            ControllerContext returned = dict.ControllerContext;

            // Assert
            Assert.AreEqual(expected, returned);
        }

        [TestMethod]
        public void DictionaryInterface() {
            // Arrange
            DictionaryHelper<string, ValueProviderResult> helper = new DictionaryHelper<string, ValueProviderResult>() {
                Creator = () => new ValueProviderDictionary(null),
                Comparer = StringComparer.OrdinalIgnoreCase,
                SampleKeys = new string[] { "foo", "bar", "baz", "quux", "QUUX" },
                SampleValues = new ValueProviderResult[] {
                    new ValueProviderResult(null, null, null),
                    new ValueProviderResult(null, null, null),
                    new ValueProviderResult(null, null, null),
                    new ValueProviderResult(null, null, null),
                    new ValueProviderResult(null, null, null)
                },
                ThrowOnKeyNotFound = false
            };

            // Act & assert
            helper.Execute();
        }

        [TestMethod]
        public void NullAndEmptyKeysAreIgnored() {
            // DevDiv Bugs #216667: Exception thrown when querystring contains name without value

            // Arrange
            ValueProviderDictionary dict = GetAndPopulateDictionary();

            // Act
            bool emptyKeyFound = dict.ContainsKey(String.Empty);

            // Assert
            Assert.IsFalse(emptyKeyFound);
        }

        [TestMethod]
        public void ValueFromForm() {
            // Arrange
            ValueProviderDictionary dict;

            // Act
            using (ReplaceCurrentCulture("fr-FR")) {
                dict = GetAndPopulateDictionary();
            }
            ValueProviderResult result = dict["foo"];

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("fooFromForm", result.AttemptedValue);
            Assert.IsInstanceOfType(result.RawValue, typeof(string[]));
            Assert.AreEqual(1, ((string[])result.RawValue).Length);
            Assert.AreEqual("fooFromForm", ((string[])result.RawValue)[0]);
            Assert.AreEqual(CultureInfo.GetCultureInfo("fr-FR"), result.Culture);
        }

        [TestMethod]
        public void ValueFromQueryString() {
            // Arrange
            ValueProviderDictionary dict;

            // Act
            using (ReplaceCurrentCulture("fr-FR")) {
                dict = GetAndPopulateDictionary();
            }
            ValueProviderResult result = dict["baz"];

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("bazFromQueryString", result.AttemptedValue);
            Assert.IsInstanceOfType(result.RawValue, typeof(string[]));
            Assert.AreEqual(1, ((string[])result.RawValue).Length);
            Assert.AreEqual("bazFromQueryString", ((string[])result.RawValue)[0]);
            Assert.AreEqual(CultureInfo.InvariantCulture, result.Culture);
        }

        public void ValueFromRoute() {
            // Arrange
            ValueProviderDictionary dict;

            // Act
            using (ReplaceCurrentCulture("fr-FR")) {
                dict = GetAndPopulateDictionary();
            }
            ValueProviderResult result = dict["bar"];

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("barFromRoute", result.AttemptedValue);
            Assert.AreEqual("barFromRoute", result.RawValue);
            Assert.AreEqual(CultureInfo.InvariantCulture, result.Culture);
        }

        private static ValueProviderDictionary GetAndPopulateDictionary() {
            return new ValueProviderDictionary(GetControllerContext());
        }

        private static ControllerContext GetControllerContext() {
            NameValueCollection form = new NameValueCollection() { { "foo", "fooFromForm" } };

            RouteData rd = new RouteData();
            rd.Values["foo"] = "fooFromRoute";
            rd.Values["bar"] = "barFromRoute";

            NameValueCollection queryString = new NameValueCollection() {
                { "foo", "fooFromQueryString" },
                { "bar", "barFromQueryString" },
                { "baz", "bazFromQueryString" },
                { null, "nullValue" },
                { "", "emptyStringValue" }
            };

            Mock<ControllerContext> mockControllerContext = new Mock<ControllerContext>();
            mockControllerContext.Expect(c => c.HttpContext.Request.Form).Returns(form);
            mockControllerContext.Expect(c => c.HttpContext.Request.QueryString).Returns(queryString);
            mockControllerContext.Expect(c => c.RouteData).Returns(rd);
            return mockControllerContext.Object;
        }

        public static IDisposable ReplaceCurrentCulture(string culture) {
            CultureInfo newCulture = CultureInfo.GetCultureInfo(culture);
            CultureInfo originalCulture = Thread.CurrentThread.CurrentCulture;
            Thread.CurrentThread.CurrentCulture = newCulture;
            return new CultureReplacement { OriginalCulture = originalCulture };
        }

        private class CultureReplacement : IDisposable {
            public CultureInfo OriginalCulture;
            public void Dispose() {
                Thread.CurrentThread.CurrentCulture = OriginalCulture;
            }
        }

    }
}
