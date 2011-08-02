namespace Microsoft.Web.Mvc.Test {
    using System;
    using System.Globalization;
    using System.Linq;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.Web.Mvc;

    [TestClass]
    public class DescriptorUtilTest {

        [TestMethod]
        public void LazilyFetchOrCreateDescriptors() {
            // Arrange
            string[] cache = null;
            int[] ints = new int[] { 0, 1, 2, 3 };

            // Act
            string[] returned = DescriptorUtil.LazilyFetchOrCreate(ref cache, () => ints, x => x.ToString(CultureInfo.InvariantCulture));
            string[] returned2 = DescriptorUtil.LazilyFetchOrCreate(ref cache, () => ints, x => x.ToString(CultureInfo.InvariantCulture));

            // Assert
            Assert.AreSame(cache, returned, "Cache and returned value should point to same array.");
            Assert.AreSame(returned, returned2, "Returned value was not correctly cached.");
            Assert.IsTrue(returned.SequenceEqual(new string[] { "0", "1", "2", "3" }), "Returned sequence was incorrect.");
        }

    }
}
