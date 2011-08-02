namespace Microsoft.Web.Mvc.Test {
    using System;
    using System.Web.TestUtil;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.Web.Mvc;
    using System.Threading;

    [TestClass]
    public class AsyncResultWrapperTest {

        [TestMethod]
        public void UnwrapAndContinueExecutesStoredDelegateAndReturnsValue() {
            // Arrange
            IAsyncResult result = AsyncResultWrapper.Wrap(null, null,
                (callback, state) => new MockAsyncResult(),
                ar => 42);

            // Act
            int returned = AsyncResultWrapper.UnwrapAndContinue<int>(result);

            // Assert
            Assert.AreEqual(42, returned);
        }

        [TestMethod]
        public void UnwrapAndContinueThrowsIfAsyncResultIsIncorrectType() {
            // Arrange
            IAsyncResult result = AsyncResultWrapper.Wrap(null, null,
                (callback, state) => new MockAsyncResult(),
                ar => { });

            // Act & assert
            ExceptionHelper.ExpectArgumentException(
                delegate {
                    AsyncResultWrapper.UnwrapAndContinue<int>(result);
                },
                @"The provided IAsyncResult is not valid for this method.
Parameter name: asyncResult");
        }

        [TestMethod]
        public void UnwrapAndContinueThrowsIfAsyncResultIsNull() {
            // Act & assert
            ExceptionHelper.ExpectArgumentNullException(
                delegate {
                    AsyncResultWrapper.UnwrapAndContinue(null);
                }, "asyncResult");
        }

        [TestMethod]
        public void UnwrapAndContinueThrowsIfAsyncResultTagMismatch() {
            // Arrange
            IAsyncResult result = AsyncResultWrapper.Wrap(null, null,
                (callback, state) => new MockAsyncResult(),
                ar => { },
                "some tag");

            // Act & assert
            ExceptionHelper.ExpectArgumentException(
                delegate {
                    AsyncResultWrapper.UnwrapAndContinue(result, "some other tag");
                },
                @"The provided IAsyncResult is not valid for this method.
Parameter name: asyncResult");
        }

        [TestMethod]
        public void UnwrapAndContinueThrowsIfCalledTwiceOnSameAsyncResult() {
            // Arrange
            IAsyncResult result = AsyncResultWrapper.Wrap(null, null,
                (callback, state) => new MockAsyncResult(),
                ar => { });

            // Act & assert
            AsyncResultWrapper.UnwrapAndContinue(result);
            ExceptionHelper.ExpectArgumentException(
                delegate {
                    AsyncResultWrapper.UnwrapAndContinue(result);
                },
                @"The provided IAsyncResult has already been consumed.
Parameter name: asyncResult");
        }

        [TestMethod]
        public void WrapReturnsAsyncResultWhichWrapsInnerResult() {
            // Arrange
            object outerAsyncState = new object();

            IAsyncResult innerResult = new MockAsyncResult() {
                CompletedSynchronously = true,
                IsCompleted = true
            };

            // Act
            IAsyncResult outerResult = AsyncResultWrapper.Wrap(null, outerAsyncState,
                (callback, state) => innerResult,
                ar => { });

            // Assert
            Assert.AreEqual(outerAsyncState, outerResult.AsyncState);
            Assert.AreEqual(innerResult.AsyncWaitHandle, outerResult.AsyncWaitHandle);
            Assert.AreEqual(innerResult.CompletedSynchronously, outerResult.CompletedSynchronously);
            Assert.AreEqual(innerResult.IsCompleted, outerResult.IsCompleted);
        }

        [TestMethod]
        public void WrapThrowsIfBeginCallbackIsNull() {
            // Act & assert
            ExceptionHelper.ExpectArgumentNullException(
                delegate {
                    AsyncResultWrapper.Wrap(null, null, null, null);
                }, "beginCallback");
        }

        [TestMethod]
        public void WrapThrowsIfEndCallbackIsNull() {
            // Arrange
            BeginInvokeCallback beginCallback = (callback, state) => null;

            // Act & assert
            ExceptionHelper.ExpectArgumentNullException(
                delegate {
                    AsyncResultWrapper.Wrap(null, null, beginCallback, null);
                }, "endCallback");
        }

        [TestMethod]
        public void WrapThrowsIfTimeoutIsOutOfRange() {
            // Arrange
            BeginInvokeCallback beginCallback = (callback, state) => null;
            AsyncCallback endCallback = ar => { };

            // Act & assert
            ExceptionHelper.ExpectArgumentOutOfRangeException(
                delegate {
                    AsyncResultWrapper.WrapWithTimeout(null, null, beginCallback, endCallback, -1000);
                }, "timeout",
                @"The timeout period must be a non-negative number or Timeout.Infinite.
Parameter name: timeout");
        }

        [TestMethod]
        public void WrapCompletedAsynchronously() {
            // Arrange
            AsyncCallback capturedCallback = null;
            IAsyncResult resultGivenToCallback = null;
            IAsyncResult innerResult = new MockAsyncResult();

            // Act
            IAsyncResult outerResult = AsyncResultWrapper.Wrap(
                ar => { resultGivenToCallback = ar; },
                null,
                (callback, state) => {
                    capturedCallback = callback;
                    return innerResult;
                },
                ar => { });

            capturedCallback(innerResult);

            // Assert
            Assert.AreEqual(outerResult, resultGivenToCallback);
        }

        [TestMethod]
        public void WrapCompletedSynchronously() {
            // Arrange
            IAsyncResult resultGivenToCallback = null;
            IAsyncResult innerResult = new MockAsyncResult();

            // Act
            IAsyncResult outerResult = AsyncResultWrapper.Wrap(
                ar => { resultGivenToCallback = ar; },
                null,
                (callback, state) => {
                    callback(innerResult);
                    return innerResult;
                },
                ar => { });

            // Assert
            Assert.AreEqual(outerResult, resultGivenToCallback);
        }

        [TestMethod]
        public void WrapTimedOut() {
            // Arrange
            ManualResetEvent waitHandle = new ManualResetEvent(false /* initialState */);

            AsyncCallback callback = ar => {
                waitHandle.Set();
            };

            // Act & assert
            IAsyncResult asyncResult = AsyncResultWrapper.WrapWithTimeout(callback, null,
                (innerCallback, innerState) => new MockAsyncResult(),
                ar => {
                    Assert.Fail("This callback should never execute since we timed out.");
                },
                0);

            // wait for the timeout
            waitHandle.WaitOne();

            ExceptionHelper.ExpectException<TimeoutException>(
                delegate {
                    AsyncResultWrapper.UnwrapAndContinue(asyncResult);
                });
        }

    }
}
