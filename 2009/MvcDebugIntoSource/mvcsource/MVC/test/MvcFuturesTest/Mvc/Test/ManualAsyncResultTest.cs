namespace Microsoft.Web.Mvc.Test {
    using System;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.Web.Mvc;

    [TestClass]
    public class ManualAsyncResultTest {

        [TestMethod]
        public void CompletedSynchronouslyPropertySetToFalse() {
            // Arrange
            ManualAsyncResult asyncResult = new ManualAsyncResult();

            // Act
            asyncResult.MarkCompleted(false /* completedSynchronously */, null);

            // Assert
            Assert.IsFalse(asyncResult.CompletedSynchronously);
        }

        [TestMethod]
        public void CompletedSynchronouslyPropertySetToTrue() {
            // Arrange
            ManualAsyncResult asyncResult = new ManualAsyncResult();

            // Act
            asyncResult.MarkCompleted(true /* completedSynchronously */, null);

            // Assert
            Assert.IsTrue(asyncResult.CompletedSynchronously);
        }

        [TestMethod]
        public void IsCompletedProperty() {
            // Arrange
            ManualAsyncResult asyncResult = new ManualAsyncResult();

            // Act
            bool isCompletedBefore = asyncResult.IsCompleted;
            asyncResult.MarkCompleted(false, null);
            bool isCompletedAfter = asyncResult.IsCompleted;

            // Assert
            Assert.IsFalse(isCompletedBefore);
            Assert.IsTrue(isCompletedAfter);
        }

        [TestMethod]
        public void MarkCompleted() {
            // Arrange
            ManualAsyncResult asyncResult = new ManualAsyncResult();
            bool callbackWasCalled = false;

            AsyncCallback callback = ar => {
                callbackWasCalled = true;
                Assert.AreEqual(asyncResult, ar);
                Assert.IsTrue(ar.CompletedSynchronously);
                Assert.IsTrue(ar.IsCompleted);

                bool wasSignaledBefore = ar.AsyncWaitHandle.WaitOne(0, false /* exitContext */);
                Assert.IsFalse(wasSignaledBefore, "The WaitHandle should not yet have been signaled.");
            };

            // Act
            asyncResult.MarkCompleted(true, callback);
            bool wasSignaledAfter = asyncResult.AsyncWaitHandle.WaitOne(0, false /* exitContext */);

            // Assert
            Assert.IsTrue(callbackWasCalled);
            Assert.IsTrue(wasSignaledAfter);
        }

        [TestMethod]
        public void WaitHandlePropertyIsNotSetByDefault() {
            // Arrange
            ManualAsyncResult asyncResult = new ManualAsyncResult();

            // Act
            bool wasSet = asyncResult.AsyncWaitHandle.WaitOne(0, false /* exitContext */);

            // Assert
            Assert.IsFalse(wasSet);
        }

        [TestMethod]
        public void WaitHandlePropertyIsActivelySetOnCompletion() {
            // Arrange
            ManualAsyncResult asyncResult = new ManualAsyncResult();

            // Act
            bool wasSet1 = asyncResult.AsyncWaitHandle.WaitOne(0, false /* exitContext */);
            asyncResult.MarkCompleted(false, null);
            bool wasSet2 = asyncResult.AsyncWaitHandle.WaitOne(0, false /* exitContext */);

            // Assert
            Assert.IsFalse(wasSet1);
            Assert.IsTrue(wasSet2);
        }

        [TestMethod]
        public void WaitHandlePropertyIsLazilySetOnCompletion() {
            // Arrange
            ManualAsyncResult asyncResult = new ManualAsyncResult();
            asyncResult.MarkCompleted(false, null);

            // Act
            bool wasSet = asyncResult.AsyncWaitHandle.WaitOne(0, false /* exitContext */);

            // Assert
            Assert.IsTrue(wasSet);
        }

    }
}
