namespace System.Web.Mvc.Test {
    using System;
    using System.Collections.Generic;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class ReaderWriterCacheTest {

        [TestMethod]
        public void PublicFetchOrCreateItemCreatesItemIfNotAlreadyInCache() {
            // Arrange
            ReaderWriterCacheHelper<int, string> helper = new ReaderWriterCacheHelper<int, string>();
            Dictionary<int, string> cache = helper.PublicCache;

            // Act
            string item = helper.PublicFetchOrCreateItem(42, () => "new");

            // Assert
            Assert.AreEqual("new", cache[42], "Cache should have been updated.");
            Assert.AreEqual("new", item);
        }

        [TestMethod]
        public void PublicFetchOrCreateItemReturnsExistingItemIfFound() {
            // Arrange
            ReaderWriterCacheHelper<int, string> helper = new ReaderWriterCacheHelper<int,string>();
            Dictionary<int, string> cache = helper.PublicCache;
            helper.PublicCache[42] = "original";

            // Act
            string item = helper.PublicFetchOrCreateItem(42, () => "new");

            // Assert
            Assert.AreEqual("original", cache[42], "Cache should not have been modified.");
            Assert.AreEqual("original", item);
        }

        [TestMethod]
        public void PublicFetchOrCreateItemReturnsFirstItemIfTwoThreadsUpdateCacheSimultaneously() {
            // Arrange
            ReaderWriterCacheHelper<int, string> helper = new ReaderWriterCacheHelper<int, string>();
            Dictionary<int, string> cache = helper.PublicCache;
            Func<string> creator = delegate() {
                // fake a second thread coming along when we weren't looking
                string firstItem = helper.PublicFetchOrCreateItem(42, () => "original");

                Assert.AreEqual("original", cache[42], "Cache should have been updated.");
                Assert.AreEqual("original", firstItem);
                return "new";
            };

            // Act
            string secondItem = helper.PublicFetchOrCreateItem(42, creator);

            // Assert
            Assert.AreEqual("original", cache[42], "Cache should not have been updated by outer thread.");
            Assert.AreEqual("original", secondItem);
        }

        private class ReaderWriterCacheHelper<TKey, TValue> : ReaderWriterCache<TKey, TValue> {

            public Dictionary<TKey, TValue> PublicCache {
                get {
                    return Cache;
                }
            }

            public TValue PublicFetchOrCreateItem(TKey key, Func<TValue> creator) {
                return FetchOrCreateItem(key, creator);
            }

        }

    }
}
