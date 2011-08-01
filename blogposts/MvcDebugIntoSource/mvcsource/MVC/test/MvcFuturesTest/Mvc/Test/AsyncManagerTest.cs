namespace Microsoft.Web.Mvc.Test {
    using System;
    using System.Threading;
    using System.Web.TestUtil;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.Web.Mvc;
    using Moq;

    [TestClass]
    public class AsyncManagerTest {

        [TestMethod]
        public void OutstandingOperationsProperty() {
            // Act
            AsyncManager helper = new AsyncManager();

            // Assert
            Assert.IsNotNull(helper.OutstandingOperations);
        }

        [TestMethod]
        public void ParametersProperty() {
            // Act
            AsyncManager helper = new AsyncManager();

            // Assert
            Assert.IsNotNull(helper.Parameters);
        }

        [TestMethod]
        public void Post() {
            // Arrange
            Mock<SynchronizationContext> mockSyncContext = new Mock<SynchronizationContext>();
            mockSyncContext
                .Expect(c => c.Send(It.IsAny<SendOrPostCallback>(), null))
                .Callback(
                    delegate(SendOrPostCallback d, object state) {
                        d(state);
                    });

            AsyncManager helper = new AsyncManager(mockSyncContext.Object);
            bool wasCalled = false;

            // Act
            helper.Post(() => { wasCalled = true; });

            // Assert
            Assert.IsTrue(wasCalled);
        }

        [TestMethod]
        public void RegisterTaskWithAction() {
            // Arrange
            Func<int> numCallsFunc;
            AsyncManager helper = GetAsyncManagerForRegisterTask(out numCallsFunc);
            AsyncCallback storedCallback = null;

            int opCountDuringBeginDelegate = 0;

            Action<AsyncCallback> beginDelegate = innerCb => {
                storedCallback = innerCb;
                opCountDuringBeginDelegate = helper.OutstandingOperations.Count;
            };

            // Act
            int opCountBeforeBeginDelegate = helper.OutstandingOperations.Count;
            helper.RegisterTask(beginDelegate, null /* endDelegate */);
            storedCallback(null);
            int opCountAfterEndDelegate = helper.OutstandingOperations.Count;

            // Assert
            Assert.AreEqual(0, opCountBeforeBeginDelegate);
            Assert.AreEqual(1, opCountDuringBeginDelegate);
            Assert.AreEqual(0, opCountAfterEndDelegate);
            Assert.AreEqual(1, numCallsFunc(), "Send() was not called.");
        }

        [TestMethod]
        public void RegisterTaskWithActionThrowsIfBeginDelegateIsNull() {
            // Arrange
            AsyncManager helper = GetAsyncManagerForRegisterTask();

            // Act & assert
            ExceptionHelper.ExpectArgumentNullException(
                delegate {
                    helper.RegisterTask((Action<AsyncCallback>)null, null);
                }, "beginDelegate");
        }

        [TestMethod]
        public void RegisterTaskWithFunc() {
            // Arrange
            Func<int> numCallsFunc;
            AsyncManager helper = GetAsyncManagerForRegisterTask(out numCallsFunc);
            MockAsyncResult asyncResult = new MockAsyncResult();
            AsyncCallback storedCallback = null;

            int opCountDuringBeginDelegate = 0;
            int opCountDuringEndDelegate = 0;

            Func<AsyncCallback, IAsyncResult> beginDelegate = innerCb => {
                storedCallback = innerCb;
                opCountDuringBeginDelegate = helper.OutstandingOperations.Count;
                return asyncResult;
            };
            AsyncCallback endDelegate = ar => {
                Assert.AreEqual(asyncResult, ar);
                opCountDuringEndDelegate = helper.OutstandingOperations.Count;
            };

            // Act
            int opCountBeforeBeginDelegate = helper.OutstandingOperations.Count;
            IAsyncResult returnedAsyncResult = helper.RegisterTask(beginDelegate, endDelegate);
            storedCallback(returnedAsyncResult);
            int opCountAfterEndDelegate = helper.OutstandingOperations.Count;

            // Assert
            Assert.AreEqual(asyncResult, returnedAsyncResult);
            Assert.AreEqual(0, opCountBeforeBeginDelegate);
            Assert.AreEqual(1, opCountDuringBeginDelegate);
            Assert.AreEqual(1, opCountDuringEndDelegate);
            Assert.AreEqual(0, opCountAfterEndDelegate);
            Assert.AreEqual(1, numCallsFunc(), "Send() was not called.");
        }

        [TestMethod]
        public void RegisterTaskWithFuncThrowsIfBeginDelegateIsNull() {
            // Arrange
            AsyncManager helper = GetAsyncManagerForRegisterTask();

            // Act & assert
            ExceptionHelper.ExpectArgumentNullException(
                delegate {
                    helper.RegisterTask((Func<AsyncCallback, IAsyncResult>)null, null);
                }, "beginDelegate");
        }

        [TestMethod]
        public void Send() {
            // Arrange
            Mock<SynchronizationContext> mockSyncContext = new Mock<SynchronizationContext>();
            mockSyncContext
                .Expect(c => c.Send(It.IsAny<SendOrPostCallback>(), null))
                .Callback(
                    delegate(SendOrPostCallback d, object state) {
                        d(state);
                    });

            AsyncManager helper = new AsyncManager(mockSyncContext.Object);
            bool wasCalled = false;

            // Act
            helper.Send(() => { wasCalled = true; });

            // Assert
            Assert.IsTrue(wasCalled);
        }

        [TestMethod]
        public void SynchronizationContextProperty() {
            // Arrange
            SynchronizationContext syncContext = new SynchronizationContext();

            // Act
            AsyncManager helper = new AsyncManager(syncContext);

            // Assert
            Assert.AreEqual(syncContext, helper.SynchronizationContext);
        }

        [TestMethod]
        public void TimeoutProperty() {
            // Arrange
            int setValue = 50;
            AsyncManager helper = new AsyncManager();

            // Act
            int defaultTimeout = helper.Timeout;
            helper.Timeout = setValue;
            int newTimeout = helper.Timeout;

            // Assert
            Assert.AreEqual(30000, defaultTimeout);
            Assert.AreEqual(setValue, newTimeout);
        }

        [TestMethod]
        public void TimeoutPropertyThrowsIfDurationIsOutOfRangs() {
            // Arrange
            int timeout = -30;
            AsyncManager helper = new AsyncManager();

            // Act & assert
            ExceptionHelper.ExpectArgumentOutOfRangeException(
                delegate {
                    helper.Timeout = timeout;
                }, "value",
                @"The timeout period must be a non-negative number or Timeout.Infinite.
Parameter name: value");
        }

        private static AsyncManager GetAsyncManagerForRegisterTask() {
            Func<int> numCallsFunc;
            return GetAsyncManagerForRegisterTask(out numCallsFunc);
        }

        private static AsyncManager GetAsyncManagerForRegisterTask(out Func<int> numCallsFunc) {
            int numCalls = 0;
            numCallsFunc = () => numCalls;

            Mock<SynchronizationContext> mockSyncContext = new Mock<SynchronizationContext>();
            mockSyncContext
                .Expect(c => c.Send(It.IsAny<SendOrPostCallback>(), null))
                .Callback(
                    delegate(SendOrPostCallback d, object state) {
                        numCalls++;
                        d(state);
                    });

            return new AsyncManager(mockSyncContext.Object);
        }

    }
}
