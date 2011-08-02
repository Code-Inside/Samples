namespace Microsoft.Web.Mvc.Controls.Test {
    using System.Collections.Generic;
    using System.Web.Mvc;
    using System.Web.UI;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.Web.Mvc.Controls;

    [TestClass]
    public class MvcControlTest {
        [TestMethod]
        public void AttributesProperty() {
            // Setup
            DummyMvcControl c = new DummyMvcControl();

            // Execute
            IDictionary<string, string> attrs = c.Attributes;

            // Verify
            Assert.IsNotNull(attrs);
            Assert.AreEqual<int>(0, attrs.Count);
        }

        [TestMethod]
        public void GetSetAttributes() {
            // Setup
            DummyMvcControl c = new DummyMvcControl();
            IAttributeAccessor attrAccessor = (IAttributeAccessor)c;
            IDictionary<string, string> attrs = c.Attributes;

            // Execute and Verify
            string value;
            value = attrAccessor.GetAttribute("xyz");
            Assert.IsNull(value);

            attrAccessor.SetAttribute("a1", "v1");
            value = attrAccessor.GetAttribute("a1");
            Assert.AreEqual<string>("v1", value);
            Assert.AreEqual<int>(1, attrs.Count);
            value = c.Attributes["a1"];
            Assert.AreEqual<string>("v1", value);
        }

        [TestMethod]
        public void EnableViewStateProperty() {
            DummyMvcControl c = new DummyMvcControl();
            Assert.IsTrue(c.EnableViewState);
            Assert.IsTrue(((Control)c).EnableViewState);

            c.EnableViewState = false;
            Assert.IsFalse(c.EnableViewState);
            Assert.IsFalse(((Control)c).EnableViewState);

            c.EnableViewState = true;
            Assert.IsTrue(c.EnableViewState);
            Assert.IsTrue(((Control)c).EnableViewState);
        }

        [TestMethod]
        public void ViewContextWithNoPageIsNull() {
            // Setup
            DummyMvcControl c = new DummyMvcControl();
            Control c1 = new Control();
            c1.Controls.Add(c);

            // Execute
            ViewContext vc = c.ViewContext;

            // Verify
            Assert.IsNull(vc);
        }

        private sealed class DummyMvcControl : MvcControl {
        }
    }
}
