using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace MSBuildNUnit.Tests
{
    [TestFixture]
    public class FooBarTest
    {
        [Test]
        public void FooBar()
        {
            Assert.IsTrue(true);
        }

        [TestFixtureTearDown]
        public void TearDown()
        {

        }
    }
}
