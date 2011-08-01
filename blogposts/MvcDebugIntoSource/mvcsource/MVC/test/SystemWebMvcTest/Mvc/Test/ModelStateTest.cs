namespace System.Web.Mvc.Test {
    using System.Web.Mvc;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class ModelStateTest {
        
        [TestMethod]
        public void ErrorsProperty() {
            // Arrange
            ModelState modelState = new ModelState();

            // Act & Assert
            Assert.IsNotNull(modelState.Errors);
        }

    }
}
