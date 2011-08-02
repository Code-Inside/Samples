namespace Microsoft.Web.Mvc.Test {
    using System;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.Web.Mvc;

    [TestClass]
    public class OutstandingAsyncOperationsTest {

        [TestMethod]
        public void CompletedEvent() {
            // Arrange
            bool premature = true;
            bool eventFired = false;
            OutstandingAsyncOperations ops = new OutstandingAsyncOperations();
            ops.Completed += (sender, eventArgs) => {
                if (premature) {
                    Assert.Fail("Event fired too early!");
                }
                if (eventFired) {
                    Assert.Fail("Event fired multiple times.");
                }

                Assert.AreEqual(ops, sender);
                Assert.AreEqual(eventArgs, EventArgs.Empty);
                eventFired = true;
            };

            // Act & assert
            ops.Increment(); // should not fire event (will throw exception)
            premature = false;

            ops.Decrement(); // should fire event
            Assert.IsTrue(eventFired);

            ops.Increment(); // should not fire event (will throw exception)
        }

        [TestMethod]
        public void CountStartsAtZero() {
            // Arrange
            OutstandingAsyncOperations ops = new OutstandingAsyncOperations();
            
            // Act & assert
            Assert.AreEqual(0, ops.Count);
        }

        [TestMethod]
        public void DecrementWithIntegerArgument() {
            // Arrange
            OutstandingAsyncOperations ops = new OutstandingAsyncOperations();

            // Act
            int returned = ops.Decrement(3);
            int newCount = ops.Count;

            // Assert
            Assert.AreEqual(-3, returned);
            Assert.AreEqual(-3, newCount);
        }

        [TestMethod]
        public void DecrementWithNoArguments() {
            // Arrange
            OutstandingAsyncOperations ops = new OutstandingAsyncOperations();

            // Act
            int returned = ops.Decrement();
            int newCount = ops.Count;

            // Assert
            Assert.AreEqual(-1, returned);
            Assert.AreEqual(-1, newCount);
        }

        [TestMethod]
        public void IncrementWithIntegerArgument() {
            // Arrange
            OutstandingAsyncOperations ops = new OutstandingAsyncOperations();

            // Act
            int returned = ops.Increment(3);
            int newCount = ops.Count;

            // Assert
            Assert.AreEqual(3, returned);
            Assert.AreEqual(3, newCount);
        }

        [TestMethod]
        public void IncrementWithNoArguments() {
            // Arrange
            OutstandingAsyncOperations ops = new OutstandingAsyncOperations();

            // Act
            int returned = ops.Increment();
            int newCount = ops.Count;

            // Assert
            Assert.AreEqual(1, returned);
            Assert.AreEqual(1, newCount);
        }

    }
}
