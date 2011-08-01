namespace System.Web.Mvc.Test {
    using System.Web.Mvc;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class NonActionAttributeTest {

        [TestMethod]
        public void InValidActionForRequestReturnsFalse() {
            // Arrange
            NonActionAttribute attr = new NonActionAttribute();

            // Act & Assert
            Assert.IsFalse(attr.IsValidForRequest(null, null), "[NonAction] should not be valid for any request.");
        }

    }
}
