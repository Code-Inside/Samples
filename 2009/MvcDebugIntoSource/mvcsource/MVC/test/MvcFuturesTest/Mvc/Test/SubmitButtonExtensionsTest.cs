namespace Microsoft.Web.Mvc.Test {
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Routing;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.Web.Mvc;
    using Moq;

    [TestClass]
    public class SubmitButtonExtensionsTest {
        [TestMethod]
        public void SubmitButtonRendersWithJustTypeAttribute() {
            HtmlHelper html = TestHelper.GetHtmlHelper(new ViewDataDictionary());
            string button = html.SubmitButton();
            Assert.AreEqual("<input type=\"submit\" />", button);
        }
        
        [TestMethod]
        public void SubmitButtonWithNameRendersButtonWithNameAttribute() {
            HtmlHelper html = TestHelper.GetHtmlHelper(new ViewDataDictionary());
            string button = html.SubmitButton("button-name");
            Assert.AreEqual("<input id=\"button-name\" name=\"button-name\" type=\"submit\" />", button);
        }

        [TestMethod]
        public void SubmitButtonWithIdDifferentFromNameRendersButtonWithId() {
            HtmlHelper html = TestHelper.GetHtmlHelper(new ViewDataDictionary());
            string button = html.SubmitButton("button-name", "blah", new {id="foo" });
            Assert.AreEqual("<input id=\"foo\" name=\"button-name\" type=\"submit\" value=\"blah\" />", button);
        }

        [TestMethod]
        public void SubmitButtonWithNameAndTextRendersAttributes() {
            HtmlHelper html = TestHelper.GetHtmlHelper(new ViewDataDictionary());
            string button = html.SubmitButton("button-name", "button-text");
            Assert.AreEqual("<input id=\"button-name\" name=\"button-name\" type=\"submit\" value=\"button-text\" />", button);
        }

        [TestMethod]
        public void SubmitButtonWithNameAndValueRendersBothAttributes() {
            HtmlHelper html = TestHelper.GetHtmlHelper(new ViewDataDictionary());
            string button = html.SubmitButton("button-name", "button-value", new { id = "button-id" });
            Assert.AreEqual("<input id=\"button-id\" name=\"button-name\" type=\"submit\" value=\"button-value\" />", button);
        }

        [TestMethod]
        public void SubmitButtonWithNameAndIdRendersBothAttributesCorrectly() {
            HtmlHelper html = TestHelper.GetHtmlHelper(new ViewDataDictionary());
            string button = html.SubmitButton("button-name", "button-value", new { id = "button-id" });
            Assert.AreEqual("<input id=\"button-id\" name=\"button-name\" type=\"submit\" value=\"button-value\" />", button);
        }

        [TestMethod]
        public void SubmitButtonWithTypeAttributeRendersCorrectType() {
            HtmlHelper html = TestHelper.GetHtmlHelper(new ViewDataDictionary());
            string button = html.SubmitButton("specified-name", "button-value", new {type="not-submit"});
            Assert.AreEqual("<input id=\"specified-name\" name=\"specified-name\" type=\"not-submit\" value=\"button-value\" />", button);
        }

        [TestMethod]
        public void SubmitButtonWithNameAndValueSpecifiedAndPassedInAsAttributeChoosesSpecified() {
            HtmlHelper html = TestHelper.GetHtmlHelper(new ViewDataDictionary());
            string button = html.SubmitButton("specified-name", "button-value"
                , new RouteValueDictionary(new { name = "name-attribute-value", value="value-attribute" }));
            Assert.AreEqual("<input id=\"specified-name\" name=\"name-attribute-value\" type=\"submit\" value=\"value-attribute\" />", button);
        }
    }
}
