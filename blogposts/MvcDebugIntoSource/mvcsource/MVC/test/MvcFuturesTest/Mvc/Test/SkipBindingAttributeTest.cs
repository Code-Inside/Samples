namespace Microsoft.Web.Mvc.Test {
    using System.Web.Mvc;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.Web.Mvc;

    [TestClass]
    public class SkipBindingAttributeTest {

        [TestMethod]
        public void GetBinderReturnsModelBinderWhichReturnsNull() {
            // Arrange
            CustomModelBinderAttribute attr = new SkipBindingAttribute();
            IModelBinder binder = attr.GetBinder();

            // Act
            object result = binder.BindModel(null, null);

            // Assert
            Assert.IsNull(result);
        }

    }
}
