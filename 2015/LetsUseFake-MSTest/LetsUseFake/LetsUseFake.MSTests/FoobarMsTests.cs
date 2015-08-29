using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LetsUseFake.MSTests
{
    [TestClass]
    public class FoobarMsTests
    {
        [TestMethod]
        public void DoAStupidTest()
        {
            Assert.IsTrue(true);
        }
    }
}
