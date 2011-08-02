namespace Microsoft.Web.Mvc.Test {
    using System;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.Web.Mvc;

    [TestClass]
    public class ContinuationListenerTest {

        [TestMethod]
        public void Signal() {
            // Arrange
            object theValue = null;
            ContinuationListener listener = new ContinuationListener();

            // Act
            listener.Signal();
            listener.SetContinuation(() => {
                theValue = 42;
            });

            // Assert
            Assert.AreEqual(42, theValue);
        }

    }
}
