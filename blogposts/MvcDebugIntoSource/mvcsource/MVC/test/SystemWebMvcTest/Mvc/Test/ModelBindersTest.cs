namespace System.Web.Mvc.Test {
    using System;
    using System.Collections.Generic;
    using System.Web.Mvc;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class ModelBindersTest {

        [TestMethod]
        public void BindersProperty() {
            // Act
            ModelBinderDictionary binders = ModelBinders.Binders;

            // Assert
            Assert.AreEqual(1, binders.Count);
            Assert.IsTrue(binders.ContainsKey(typeof(HttpPostedFileBase)), "Did not contain entry for HttpPostedFileBase.");
            Assert.IsInstanceOfType(binders[typeof(HttpPostedFileBase)], typeof(HttpPostedFileBaseModelBinder), "Did not contain correct binder for HttpPostedFileBase.");
        }

    }
}
