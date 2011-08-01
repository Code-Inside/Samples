namespace Microsoft.Web.Mvc.Test {
    using System.Web.Mvc;
    using System.Web.TestUtil;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.Web.Mvc;
    using System;
    using System.Web.Routing;

    [TestClass]
    public class MailToExtensionsTest {
        [TestMethod]
        public void MailToWithoutEmailThrowsArgumentNullException() {
            HtmlHelper html = TestHelper.GetHtmlHelper(new ViewDataDictionary());
            ExceptionHelper.ExpectArgumentNullException(() => html.Mailto("link text", null), "emailAddress");
        }

        [TestMethod]
        public void MailToWithoutLinkTextThrowsArgumentNullException() {
            HtmlHelper html = TestHelper.GetHtmlHelper(new ViewDataDictionary());
            ExceptionHelper.ExpectArgumentNullException(() => html.Mailto(null, "somebody@example.com"), "linkText");
        }

        [TestMethod]
        public void MailToWithLinkTextAndEmailRendersProperElement() {
            HtmlHelper html = TestHelper.GetHtmlHelper(new ViewDataDictionary());
            string result = html.Mailto("This is a test", "test@example.com");
            Assert.AreEqual("<a href=\"mailto:test@example.com\">This is a test</a>", result);
        }

        [TestMethod]
        public void MailToWithLinkTextEmailAndHtmlAttributesRendersAttributes() {
            HtmlHelper html = TestHelper.GetHtmlHelper(new ViewDataDictionary());
            string result = html.Mailto("This is a test", "test@example.com", new {title="this is a test"});
            Assert.AreEqual("<a href=\"mailto:test@example.com\" title=\"this is a test\">This is a test</a>", result);
        }

        [TestMethod]
        public void MailToWithLinkTextEmailAndHtmlAttributesDictionaryRendersAttributes() {
            HtmlHelper html = TestHelper.GetHtmlHelper(new ViewDataDictionary());
            string result = html.Mailto("This is a test", "test@example.com", new RouteValueDictionary(new { title = "this is a test" }));
            Assert.AreEqual("<a href=\"mailto:test@example.com\" title=\"this is a test\">This is a test</a>", result);
        }

        [TestMethod]
        public void MailToWithSubjectAndHtmlAttributesRendersAttributes() {
            HtmlHelper html = TestHelper.GetHtmlHelper(new ViewDataDictionary());
            string result = html.Mailto("This is a test", "test@example.com", "The subject", new { title = "this is a test" });
            Assert.AreEqual("<a href=\"mailto:test@example.com?subject=The subject\" title=\"this is a test\">This is a test</a>", result);
        }

        [TestMethod]
        public void MailToWithSubjectAndHtmlAttributesDictionaryRendersAttributes() {
            HtmlHelper html = TestHelper.GetHtmlHelper(new ViewDataDictionary());
            string result = html.Mailto("This is a test", "test@example.com", "The subject", new RouteValueDictionary(new { title = "this is a test" }));
            Assert.AreEqual("<a href=\"mailto:test@example.com?subject=The subject\" title=\"this is a test\">This is a test</a>", result);
        }

        [TestMethod]
        public void MailToAttributeEncodesEmail() {
            HtmlHelper html = TestHelper.GetHtmlHelper(new ViewDataDictionary());
            string result = html.Mailto("This is a test", "te\">st@example.com");
            Assert.AreEqual("<a href=\"mailto:te&quot;>st@example.com\">This is a test</a>", result);
        }

        [TestMethod]
        public void MailToWithMultipleRecipientsRendersWithCommas() {
            HtmlHelper html = TestHelper.GetHtmlHelper(new ViewDataDictionary());
            string result = html.Mailto("This is a test", "te\">st@example.com,test2@example.com");
            Assert.AreEqual("<a href=\"mailto:te&quot;>st@example.com,test2@example.com\">This is a test</a>", result);
        }

        [TestMethod]
        public void MailToWithSubjectAppendsSubjectQuery() {
            HtmlHelper html = TestHelper.GetHtmlHelper(new ViewDataDictionary());
            string result = html.Mailto("This is a test", "test@example.com", "This is the subject");
            Assert.AreEqual("<a href=\"mailto:test@example.com?subject=This is the subject\">This is a test</a>", result);
        }

        [TestMethod]
        public void MailToWithCopyOnlyAppendsCopyQuery() {
            HtmlHelper html = TestHelper.GetHtmlHelper(new ViewDataDictionary());
            string result = html.Mailto("This is a test", "test@example.com", null, null, "cctest@example.com", null, null);
            Assert.AreEqual("<a href=\"mailto:test@example.com?cc=cctest@example.com\">This is a test</a>", result);
        }

        [TestMethod]
        public void MailToWithMultipartBodyRendersProperMailtoEncoding() {
            HtmlHelper html = TestHelper.GetHtmlHelper(new ViewDataDictionary());
            string body = @"Line one
Line two
Line three";
            string result = html.Mailto("email me", "test@example.com", null, body, null, null, null);
            Assert.AreEqual("<a href=\"mailto:test@example.com?body=Line one%0ALine two%0ALine three\">email me</a>", result);
        }

        [TestMethod]
        public void MailToWithAllValuesProvidedRendersCorrectTag() {
            HtmlHelper html = TestHelper.GetHtmlHelper(new ViewDataDictionary());
            string body = @"Line one
Line two
Line three";
            string result = html.Mailto("email me", "test@example.com", "the subject", body, "cc@example.com", "bcc@example.com", new { title="email test" });
            string expected = @"<a href=""mailto:test@example.com?subject=the subject&amp;cc=cc@example.com&amp;bcc=bcc@example.com&amp;body=Line one%0ALine two%0ALine three"" title=""email test"">email me</a>";
            Assert.AreEqual(expected, result);
        }
    }
}
