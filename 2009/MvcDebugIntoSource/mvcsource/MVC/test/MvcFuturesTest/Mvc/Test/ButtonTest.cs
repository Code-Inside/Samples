namespace Microsoft.Web.Mvc.Test {
    using System.Web.Mvc;
    using System.Web.Routing;
    using System.Web.TestUtil;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.Web.Mvc;

    [TestClass]
    public class ButtonTest {
        [TestMethod]
        public void ButtonWithNullNameThrowsArgumentNullException() {
            HtmlHelper html = TestHelper.GetHtmlHelper(new ViewDataDictionary());
            ExceptionHelper.ExpectArgumentNullException(() => html.Button(null, "text", HtmlButtonType.Button), "name");
        }

        [TestMethod]
        public void ButtonRendersBaseAttributes() {
            HtmlHelper html = TestHelper.GetHtmlHelper(new ViewDataDictionary());
            string result = html.Button("nameAttr", "buttonText", HtmlButtonType.Reset, "onclickAttr");
            Assert.AreEqual("<button name=\"nameAttr\" onclick=\"onclickAttr\" type=\"reset\">buttonText</button>", result);
        }

        [TestMethod]
        public void ButtonWithoutOnClickDoesNotRenderOnclickAttribute() {
            HtmlHelper html = TestHelper.GetHtmlHelper(new ViewDataDictionary());
            string result = html.Button("nameAttr", "buttonText", HtmlButtonType.Reset);
            Assert.AreEqual("<button name=\"nameAttr\" type=\"reset\">buttonText</button>", result);
        }

        [TestMethod]
        public void ButtonAllowsInnerHtml() {
            HtmlHelper html = TestHelper.GetHtmlHelper(new ViewDataDictionary());
            string result = html.Button("nameAttr", "<img src=\"puppy.jpg\" />", HtmlButtonType.Submit, "onclickAttr");
            Assert.AreEqual("<button name=\"nameAttr\" onclick=\"onclickAttr\" type=\"submit\"><img src=\"puppy.jpg\" /></button>", result);
        }
        
        [TestMethod]
        public void ButtonRendersExplicitAttributes() {
            HtmlHelper html = TestHelper.GetHtmlHelper(new ViewDataDictionary());
            string result = html.Button("nameAttr", "buttonText", HtmlButtonType.Reset, "onclickAttr", new { title = "the-title" });
            Assert.AreEqual("<button name=\"nameAttr\" onclick=\"onclickAttr\" title=\"the-title\" type=\"reset\">buttonText</button>", result);
        }

        [TestMethod]
        public void ButtonRendersExplicitDictionaryAttributes() {
            HtmlHelper html = TestHelper.GetHtmlHelper(new ViewDataDictionary());
            string result = html.Button("nameAttr", "buttonText", HtmlButtonType.Button, "onclickAttr", new RouteValueDictionary(new { title = "the-title" }));
            Assert.AreEqual("<button name=\"nameAttr\" onclick=\"onclickAttr\" title=\"the-title\" type=\"button\">buttonText</button>", result);
        }
    }
}
