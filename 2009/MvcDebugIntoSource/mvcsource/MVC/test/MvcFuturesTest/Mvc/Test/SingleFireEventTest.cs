namespace Microsoft.Web.Mvc.Test {
    using System;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.Web.Mvc;

    [TestClass]
    public class SingleFireEventTest {

        [TestMethod]
        public void Signal() {
            // Arrange
            SingleFireEvent sfEvent = new SingleFireEvent();

            // Act
            bool firstCall = sfEvent.Signal();
            bool secondCall = sfEvent.Signal();

            // Assert
            Assert.IsTrue(firstCall, "Signal() should return TRUE on first call.");
            Assert.IsFalse(secondCall, "Signal() should return FALSE on each subsequent call.");
        }

    }
}
