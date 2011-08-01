namespace System.Web.Mvc.Test {
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Web.Routing;
    using System.Web.TestUtil;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class NameValueCollectionExtensionsTest {

        [TestMethod]
        public void CopyTo() {
            // Arrange
            NameValueCollection collection = GetCollection();
            IDictionary<string, object> dictionary = GetDictionary();

            // Act
            collection.CopyTo(dictionary);

            // Assert
            Assert.AreEqual(3, dictionary.Count);
            Assert.AreEqual("FooDictionary", dictionary["foo"]);
            Assert.AreEqual("BarDictionary", dictionary["bar"]);
            Assert.AreEqual("BazCollection", dictionary["baz"]);
        }

        public void CopyToReplaceExisting() {
            // Arrange
            NameValueCollection collection = GetCollection();
            IDictionary<string, object> dictionary = GetDictionary();

            // Act
            collection.CopyTo(dictionary, true /* replaceExisting */);

            // Assert
            Assert.AreEqual(3, dictionary.Count);
            Assert.AreEqual("FooCollection", dictionary["foo"]);
            Assert.AreEqual("BarDictionary", dictionary["bar"]);
            Assert.AreEqual("BazCollection", dictionary["baz"]);
        }

        [TestMethod]
        public void CopyToWithNullCollectionThrows() {
            ExceptionHelper.ExpectArgumentNullException(
            delegate {
                NameValueCollectionExtensions.CopyTo(null /* collection */, null /* destination */);
            }, "collection");
        }

        [TestMethod]
        public void CopyToWithNullDestinationThrows() {
            // Arrange
            NameValueCollection collection = GetCollection();

            // Act & Assert
            ExceptionHelper.ExpectArgumentNullException(
                delegate {
                    collection.CopyTo(null /* destination */);
                }, "destination");
        }

        private static NameValueCollection GetCollection() {
            return new NameValueCollection {
                { "Foo", "FooCollection" },
                { "Baz", "BazCollection" }
            };
        }

        private static IDictionary<string, object> GetDictionary() {
            return new RouteValueDictionary(new { Foo = "FooDictionary", Bar = "BarDictionary" });
        }
    }
}
