namespace Microsoft.Web.Mvc.Test {
    using System;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.Web.Mvc;

    [TestClass]
    public class ObjectAsyncResultTest {

        [TestMethod]
        public void InvokeWithCallback() {
            // Arrange
            ObjectAsyncResult<int> oar = new ObjectAsyncResult<int>(42);

            SignalContainer<IAsyncResult> callbackContainer = new SignalContainer<IAsyncResult>();
            AsyncCallback callback = ar => {
                callbackContainer.Signal(ar);
            };

            // Act
            IAsyncResult asyncResult = oar.BeginInvoke(callback, "some state");
            IAsyncResult passedToCallback = callbackContainer.Wait();
            int returned = oar.EndInvoke(asyncResult);

            // Assert
            Assert.AreEqual(asyncResult, passedToCallback);
            Assert.AreEqual("some state", asyncResult.AsyncState);
            Assert.AreEqual(42, returned);
        }

        [TestMethod]
        public void InvokeWithoutCallback() {
            // Arrange
            ObjectAsyncResult<int> oar = new ObjectAsyncResult<int>(42);

            // Act
            IAsyncResult asyncResult = oar.BeginInvoke(null /* callback */, "some state");
            int returned = oar.EndInvoke(asyncResult);

            // Assert
            Assert.AreEqual("some state", asyncResult.AsyncState);
            Assert.AreEqual(42, returned);
        }

    }
}
