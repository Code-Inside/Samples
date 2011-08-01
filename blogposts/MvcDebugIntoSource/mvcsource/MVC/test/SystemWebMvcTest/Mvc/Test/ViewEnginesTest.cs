namespace System.Web.Mvc.Test {
    using System.Linq;
    using System.Web.Mvc;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class ViewEnginesTest {

        [TestMethod]
        public void EnginesProperty() {
            // Act
            ViewEngineCollection collection = ViewEngines.Engines;

            // Assert
            Assert.AreEqual(1, collection.Count);
            Assert.IsInstanceOfType(collection[0], typeof(WebFormViewEngine), "Collection should have contained a single WebFormViewEngine.");
        }

    }
}
