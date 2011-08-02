namespace System.Web.Mvc.Test {
    using System.Web.TestUtil;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class ActionFilterAttributeTest {

        [TestMethod]
        public void DefaultOrderIsNegativeOne() {
            // Act
            var attr = new EmptyActionFilterAttribute();

            // Assert
            Assert.AreEqual(-1, attr.Order);
        }

        [TestMethod]
        public void OrderIsSetCorrectly() {
            // Act
            var attr = new EmptyActionFilterAttribute() { Order = 98052 };

            // Assert
            Assert.AreEqual(98052, attr.Order);
        }

        [TestMethod]
        public void SpecifyingInvalidOrderThrows() {
            // Act & Assert
            ExceptionHelper.ExpectArgumentOutOfRangeException(
                delegate {
                    new EmptyActionFilterAttribute() { Order = -2 };
                },
                "value",
                "Order must be greater than or equal to -1.\r\nParameter name: value");
        }

        private class EmptyActionFilterAttribute : ActionFilterAttribute {
        }

    }
}
