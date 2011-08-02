namespace System.Web.Mvc.Test {
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class DictionaryHelpersTest {

        [TestMethod]
        public void DoesAnyKeyHavePrefixFailure() {
            // Arrange
            Dictionary<string, object> dict = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase) {
                { "FOOBAR", 42 }
            };

            // Act
            bool wasPrefixFound = DictionaryHelpers.DoesAnyKeyHavePrefix(dict, "foo");

            // Assert
            Assert.IsFalse(wasPrefixFound);
        }

        [TestMethod]
        public void DoesAnyKeyHavePrefixSuccess() {
            // Arrange
            Dictionary<string, object> dict = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase) {
                { "FOO.BAR", 42 }
            };

            // Act
            bool wasPrefixFound = DictionaryHelpers.DoesAnyKeyHavePrefix(dict, "foo");

            // Assert
            Assert.IsTrue(wasPrefixFound);
        }

        [TestMethod]
        public void FindKeysWithPrefix() {
            // Arrange
            Dictionary<string, string> dict = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase) {
                { "FOO", "fooValue" },
                { "FOOBAR", "foobarValue" },
                { "FOO.BAR", "foo.barValue" },
                { "FOO[0]", "foo[0]Value" },
                { "BAR", "barValue" }
            };

            // Act
            var matchingEntries = DictionaryHelpers.FindKeysWithPrefix(dict, "foo");

            // Assert
            var matchingEntriesList = matchingEntries.OrderBy(entry => entry.Key).ToList();
            Assert.AreEqual(3, matchingEntriesList.Count);
            Assert.AreEqual("foo", matchingEntriesList[0].Key);
            Assert.AreEqual("fooValue", matchingEntriesList[0].Value);
            Assert.AreEqual("FOO.BAR", matchingEntriesList[1].Key);
            Assert.AreEqual("foo.barValue", matchingEntriesList[1].Value);
            Assert.AreEqual("FOO[0]", matchingEntriesList[2].Key);
            Assert.AreEqual("foo[0]Value", matchingEntriesList[2].Value);
        }

    }
}
