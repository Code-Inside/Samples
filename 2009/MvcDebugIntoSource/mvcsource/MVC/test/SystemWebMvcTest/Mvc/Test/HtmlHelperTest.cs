namespace System.Web.Mvc.Test {
    using System;
    using System.Globalization;
    using System.Text.RegularExpressions;
    using System.Threading;
    using System.Web.Routing;
    using System.Web.TestUtil;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

    [TestClass]
    public class HtmlHelperTest {
        public const string AppPathModifier = "/$(SESSION)";
        public static readonly RouteValueDictionary AttributesDictionary = new RouteValueDictionary(new { baz = "BazValue" });
        public static readonly object AttributesObjectDictionary = new { baz = "BazObjValue" };

        // AntiForgeryToken test helpers
        private static string _antiForgeryTokenCookieName = AntiForgeryData.GetAntiForgeryTokenName("/SomeAppPath");
        private const string _serializedValuePrefix = @"<input name=""__RequestVerificationToken"" type=""hidden"" value=""Creation: ";
        private const string _someValueSuffix = @", Value: some value, Salt: some other salt"" />";
        private readonly Regex _randomFormValueSuffixRegex = new Regex(@", Value: (?<value>[A-Za-z0-9/\+=]{24}), Salt: some other salt"" />$");
        private readonly Regex _randomCookieValueSuffixRegex = new Regex(@", Value: (?<value>[A-Za-z0-9/\+=]{24}), Salt: $");

        [TestMethod]
        public void SerializerProperty() {
            // Arrange
            HtmlHelper helper = GetHtmlHelperForAntiForgeryToken(null);
            AntiForgeryDataSerializer newSerializer = new AntiForgeryDataSerializer();

            // Act & Assert
            MemberHelper.TestPropertyWithDefaultInstance(helper, "Serializer", newSerializer);
        }

        [TestMethod]
        public void ViewContextProperty() {
            // Arrange
            ViewContext viewContext = new Mock<ViewContext>().Object;
            HtmlHelper htmlHelper = new HtmlHelper(viewContext, new Mock<IViewDataContainer>().Object);

            // Act
            ViewContext value = htmlHelper.ViewContext;

            // Assert
            Assert.AreEqual(viewContext, value);
        }

        [TestMethod]
        public void ViewDataContainerProperty() {
            // Arrange
            ViewContext viewContext = new Mock<ViewContext>().Object;
            IViewDataContainer container = new Mock<IViewDataContainer>().Object;
            HtmlHelper htmlHelper = new HtmlHelper(viewContext, container);

            // Act
            IViewDataContainer value = htmlHelper.ViewDataContainer;

            // Assert
            Assert.AreEqual(container, value);
        }

        [TestMethod]
        public void ConstructorWithNullRouteCollectionThrows() {
            // Assert
            ExceptionHelper.ExpectArgumentNullException(
                delegate {
                    new HtmlHelper(new Mock<ViewContext>().Object, GetViewDataContainer(null), null);
                },
                "routeCollection");
        }

        [TestMethod]
        public void ConstructorWithNullViewContextThrows() {
            // Assert
            ExceptionHelper.ExpectArgumentNullException(
                delegate {
                    new HtmlHelper(null, null);
                },
                "viewContext");
        }

        [TestMethod]
        public void ConstructorWithNullViewDataContainerThrows() {
            // Assert
            ExceptionHelper.ExpectArgumentNullException(
                delegate {
                    new HtmlHelper(new Mock<ViewContext>().Object, null);
                },
                "viewDataContainer");
        }

        [TestMethod]
        public void AntiForgeryTokenSetsCookieValueIfDoesNotExist() {
            // Arrange
            HtmlHelper htmlHelper = GetHtmlHelperForAntiForgeryToken(null);

            // Act
            string formValue = htmlHelper.AntiForgeryToken("some other salt");

            // Assert
            Assert.IsTrue(formValue.StartsWith(_serializedValuePrefix), "Form value prefix did not match.");

            Match formMatch = _randomFormValueSuffixRegex.Match(formValue);
            string formTokenValue = formMatch.Groups["value"].Value;

            HttpCookie cookie = htmlHelper.ViewContext.HttpContext.Response.Cookies[_antiForgeryTokenCookieName];
            Assert.IsNotNull(cookie, "Cookie was not set correctly.");
            Assert.IsTrue(cookie.HttpOnly, "Cookie should have HTTP-only flag set.");
            Assert.IsTrue(String.IsNullOrEmpty(cookie.Domain), "Domain should not have been set.");
            Assert.AreEqual("/", cookie.Path, "Path should have remained at '/' by default.");

            Match cookieMatch = _randomCookieValueSuffixRegex.Match(cookie.Value);
            string cookieTokenValue = cookieMatch.Groups["value"].Value;

            Assert.AreEqual(formTokenValue, cookieTokenValue, "Form and cookie token values did not match.");
        }

        [TestMethod]
        public void AntiForgeryTokenSetsDomainAndPathIfSpecified() {
            // Arrange
            HtmlHelper htmlHelper = GetHtmlHelperForAntiForgeryToken(null);

            // Act
            string formValue = htmlHelper.AntiForgeryToken("some other salt", "theDomain", "thePath");

            // Assert
            Assert.IsTrue(formValue.StartsWith(_serializedValuePrefix), "Form value prefix did not match.");

            Match formMatch = _randomFormValueSuffixRegex.Match(formValue);
            string formTokenValue = formMatch.Groups["value"].Value;

            HttpCookie cookie = htmlHelper.ViewContext.HttpContext.Response.Cookies[_antiForgeryTokenCookieName];
            Assert.IsNotNull(cookie, "Cookie was not set correctly.");
            Assert.IsTrue(cookie.HttpOnly, "Cookie should have HTTP-only flag set.");
            Assert.AreEqual("theDomain", cookie.Domain);
            Assert.AreEqual("thePath", cookie.Path);

            Match cookieMatch = _randomCookieValueSuffixRegex.Match(cookie.Value);
            string cookieTokenValue = cookieMatch.Groups["value"].Value;

            Assert.AreEqual(formTokenValue, cookieTokenValue, "Form and cookie token values did not match.");
        }

        [TestMethod]
        public void AntiForgeryTokenUsesCookieValueIfExists() {
            // Arrange
            HtmlHelper htmlHelper = GetHtmlHelperForAntiForgeryToken("2001-01-01:some value:some salt");

            // Act
            string formValue = htmlHelper.AntiForgeryToken("some other salt");

            // Assert
            Assert.IsTrue(formValue.StartsWith(_serializedValuePrefix), "Form value prefix did not match.");
            Assert.IsTrue(formValue.EndsWith(_someValueSuffix), "Form value suffix did not match.");
            Assert.AreEqual(0, htmlHelper.ViewContext.HttpContext.Response.Cookies.Count, "Cookie should not have been added to response.");
        }

        [TestMethod]
        public void AttributeEncodeObject() {
            // Arrange
            HtmlHelper htmlHelper = GetHtmlHelper();

            // Act
            string encodedHtml = htmlHelper.AttributeEncode((object)@"<"">");

            // Assert
            Assert.AreEqual(encodedHtml, "&lt;&quot;>", "Text is not being properly HTML attribute-encoded.");
        }

        [TestMethod]
        public void AttributeEncodeObjectNull() {
            // Arrange
            HtmlHelper htmlHelper = GetHtmlHelper();

            // Act
            string encodedHtml = htmlHelper.AttributeEncode((object)null);

            // Assert
            Assert.AreEqual("", encodedHtml);
        }

        [TestMethod]
        public void AttributeEncodeString() {
            // Arrange
            HtmlHelper htmlHelper = GetHtmlHelper();

            // Act
            string encodedHtml = htmlHelper.AttributeEncode(@"<"">");

            // Assert
            Assert.AreEqual(encodedHtml, "&lt;&quot;>", "Text is not being properly HTML attribute-encoded.");
        }

        [TestMethod]
        public void AttributeEncodeStringNull() {
            // Arrange
            HtmlHelper htmlHelper = GetHtmlHelper();

            // Act
            string encodedHtml = htmlHelper.AttributeEncode((string)null);

            // Assert
            Assert.AreEqual("", encodedHtml);
        }

        [TestMethod]
        public void EncodeObject() {
            // Arrange
            HtmlHelper htmlHelper = GetHtmlHelper();

            // Act
            string encodedHtml = htmlHelper.Encode((object)"<br />");

            // Assert
            Assert.AreEqual(encodedHtml, "&lt;br /&gt;", "Text is not being properly HTML-encoded.");
        }

        [TestMethod]
        public void EncodeObjectNull() {
            // Arrange
            HtmlHelper htmlHelper = GetHtmlHelper();

            // Act
            string encodedHtml = htmlHelper.Encode((object)null);

            // Assert
            Assert.AreEqual("", encodedHtml);
        }

        [TestMethod]
        public void EncodeString() {
            // Arrange
            HtmlHelper htmlHelper = GetHtmlHelper();

            // Act
            string encodedHtml = htmlHelper.Encode("<br />");

            // Assert
            Assert.AreEqual(encodedHtml, "&lt;br /&gt;", "Text is not being properly HTML-encoded.");
        }

        [TestMethod]
        public void EncodeStringNull() {
            // Arrange
            HtmlHelper htmlHelper = GetHtmlHelper();

            // Act
            string encodedHtml = htmlHelper.Encode((string)null);

            // Assert
            Assert.AreEqual("", encodedHtml);
        }

        [TestMethod]
        public void GenericHelperConstructorSetsProperties1() {
            // Arrange
            ViewContext viewContext = new Mock<ViewContext>().Object;
            ViewDataDictionary<Controller> vdd = new ViewDataDictionary<Controller>(new Mock<Controller>().Object);
            Mock<IViewDataContainer> vdc = new Mock<IViewDataContainer>();
            vdc.Expect(v => v.ViewData).Returns(vdd);

            // Act
            HtmlHelper<Controller> htmlHelper = new HtmlHelper<Controller>(viewContext, vdc.Object);

            // Assert
            Assert.AreEqual(viewContext, htmlHelper.ViewContext);
            Assert.AreEqual(vdc.Object, htmlHelper.ViewDataContainer);
            Assert.AreEqual(RouteTable.Routes, htmlHelper.RouteCollection);
            Assert.AreEqual(vdd.Model, htmlHelper.ViewData.Model);
        }

        [TestMethod]
        public void GenericHelperConstructorSetsProperties2() {
            // Arrange
            ViewContext viewContext = new Mock<ViewContext>().Object;
            ViewDataDictionary<Controller> vdd = new ViewDataDictionary<Controller>(new Mock<Controller>().Object);
            Mock<IViewDataContainer> vdc = new Mock<IViewDataContainer>();
            vdc.Expect(v => v.ViewData).Returns(vdd);
            RouteCollection rc = new RouteCollection();

            // Act
            HtmlHelper<Controller> htmlHelper = new HtmlHelper<Controller>(viewContext, vdc.Object, rc);

            // Assert
            Assert.AreEqual(viewContext, htmlHelper.ViewContext);
            Assert.AreEqual(vdc.Object, htmlHelper.ViewDataContainer);
            Assert.AreEqual(rc, htmlHelper.RouteCollection);
            Assert.AreEqual(vdd.Model, htmlHelper.ViewData.Model);
        }

        public static HttpContextBase GetHttpContext(string appPath, string requestPath, string httpMethod, string protocol, int port) {
            Mock<HttpContextBase> mockHttpContext = new Mock<HttpContextBase>();

            if (!String.IsNullOrEmpty(appPath)) {
                mockHttpContext.Expect(o => o.Request.ApplicationPath).Returns(appPath);
            }
            if (!String.IsNullOrEmpty(requestPath)) {
                mockHttpContext.Expect(o => o.Request.AppRelativeCurrentExecutionFilePath).Returns(requestPath);
            }

            Uri uri;

            if (port >= 0) {
                uri = new Uri(protocol + "://localhost" + ":" + Convert.ToString(port));
            }
            else {
                uri = new Uri(protocol + "://localhost");
            }
            mockHttpContext.Expect(o => o.Request.Url).Returns(uri);

            mockHttpContext.Expect(o => o.Request.PathInfo).Returns(String.Empty);
            if (!String.IsNullOrEmpty(httpMethod)) {
                mockHttpContext.Expect(o => o.Request.HttpMethod).Returns(httpMethod);
            }

            mockHttpContext.Expect(o => o.Session).Returns((HttpSessionStateBase)null);
            mockHttpContext.Expect(o => o.Response.ApplyAppPathModifier(It.IsAny<string>())).Returns<string>(r => AppPathModifier + r);
            return mockHttpContext.Object;
        }

        public static HttpContextBase GetHttpContext(string appPath, string requestPath, string httpMethod) {
            return GetHttpContext(appPath, requestPath, httpMethod, Uri.UriSchemeHttp.ToString(), -1);
        }

        internal static HtmlHelper GetHtmlHelper() {
            HttpContextBase httpcontext = GetHttpContext("/app/", null, null);
            RouteCollection rt = new RouteCollection();
            rt.Add(new Route("{controller}/{action}/{id}", null) { Defaults = new RouteValueDictionary(new { id = "defaultid" }) });
            rt.Add("namedroute", new Route("named/{controller}/{action}/{id}", null) { Defaults = new RouteValueDictionary(new { id = "defaultid" }) });
            RouteData rd = new RouteData();
            rd.Values.Add("controller", "home");
            rd.Values.Add("action", "oldaction");

            ViewDataDictionary vdd = new ViewDataDictionary();

            Mock<ViewContext> mockViewContext = new Mock<ViewContext>();
            mockViewContext.Expect(c => c.HttpContext).Returns(httpcontext);
            mockViewContext.Expect(c => c.RouteData).Returns(rd);
            mockViewContext.Expect(c => c.ViewData).Returns(vdd);
            Mock<IViewDataContainer> mockVdc = new Mock<IViewDataContainer>();
            mockVdc.Expect(vdc => vdc.ViewData).Returns(vdd);

            HtmlHelper htmlHelper = new HtmlHelper(mockViewContext.Object, mockVdc.Object, rt);
            return htmlHelper;
        }

        internal static HtmlHelper GetHtmlHelper(string protocol, int port) {
            HttpContextBase httpcontext = GetHttpContext("/app/", null, null, protocol, port);
            RouteCollection rt = new RouteCollection();
            rt.Add(new Route("{controller}/{action}/{id}", null) { Defaults = new RouteValueDictionary(new { id = "defaultid" }) });
            rt.Add("namedroute", new Route("named/{controller}/{action}/{id}", null) { Defaults = new RouteValueDictionary(new { id = "defaultid" }) });
            RouteData rd = new RouteData();
            rd.Values.Add("controller", "home");
            rd.Values.Add("action", "oldaction");

            ViewDataDictionary vdd = new ViewDataDictionary();

            Mock<ViewContext> mockViewContext = new Mock<ViewContext>();
            mockViewContext.Expect(c => c.HttpContext).Returns(httpcontext);
            mockViewContext.Expect(c => c.RouteData).Returns(rd);
            mockViewContext.Expect(c => c.ViewData).Returns(vdd);
            Mock<IViewDataContainer> mockVdc = new Mock<IViewDataContainer>();
            mockVdc.Expect(vdc => vdc.ViewData).Returns(vdd);

            HtmlHelper htmlHelper = new HtmlHelper(mockViewContext.Object, mockVdc.Object, rt);
            return htmlHelper;
        }

        internal static HtmlHelper GetHtmlHelper(ViewDataDictionary viewData) {
            Mock<ViewContext> mockViewContext = new Mock<ViewContext>();
            mockViewContext.Expect(c => c.ViewData).Returns(viewData);
            IViewDataContainer container = GetViewDataContainer(viewData);
            return new HtmlHelper(mockViewContext.Object, container);
        }

        private static HtmlHelper GetHtmlHelperForAntiForgeryToken(string cookieValue) {
            HttpCookieCollection requestCookies = new HttpCookieCollection();
            HttpCookieCollection responseCookies = new HttpCookieCollection();
            if (!String.IsNullOrEmpty(cookieValue)) {
                requestCookies.Set(new HttpCookie(AntiForgeryData.GetAntiForgeryTokenName("/SomeAppPath"), cookieValue));
            }

            Mock<ViewContext> mockViewContext = new Mock<ViewContext>();
            mockViewContext.Expect(c => c.HttpContext.Request.Cookies).Returns(requestCookies);
            mockViewContext.Expect(c => c.HttpContext.Request.ApplicationPath).Returns("/SomeAppPath");
            mockViewContext.Expect(c => c.HttpContext.Response.Cookies).Returns(responseCookies);

            return new HtmlHelper(mockViewContext.Object, new Mock<IViewDataContainer>().Object) {
                Serializer = new SubclassedAntiForgeryTokenSerializer()
            };
        }

        internal static ValueProviderResult GetValueProviderResult(object rawValue, string attemptedValue) {
            return new ValueProviderResult(rawValue, attemptedValue, CultureInfo.InvariantCulture);
        }

        private static IViewDataContainer GetViewDataContainer(ViewDataDictionary viewData) {
            Mock<IViewDataContainer> mockContainer = new Mock<IViewDataContainer>();
            mockContainer.Expect(c => c.ViewData).Returns(viewData);
            return mockContainer.Object;
        }

        public static IDisposable ReplaceCulture(string currentCulture, string currentUICulture) {
            CultureInfo newCulture = CultureInfo.GetCultureInfo(currentCulture);
            CultureInfo newUICulture = CultureInfo.GetCultureInfo(currentUICulture);
            CultureInfo originalCulture = Thread.CurrentThread.CurrentCulture;
            CultureInfo originalUICulture = Thread.CurrentThread.CurrentUICulture;
            Thread.CurrentThread.CurrentCulture = newCulture;
            Thread.CurrentThread.CurrentUICulture = newUICulture;
            return new CultureReplacement { OriginalCulture = originalCulture, OriginalUICulture = originalUICulture };
        }

        private class CultureReplacement : IDisposable {
            public CultureInfo OriginalCulture;
            public CultureInfo OriginalUICulture;
            public void Dispose() {
                Thread.CurrentThread.CurrentCulture = OriginalCulture;
                Thread.CurrentThread.CurrentUICulture = OriginalUICulture;
            }
        }

        internal class SubclassedAntiForgeryTokenSerializer : AntiForgeryDataSerializer {
            public override string Serialize(AntiForgeryData token) {
                return String.Format(CultureInfo.InvariantCulture, "Creation: {0}, Value: {1}, Salt: {2}",
                        token.CreationDate, token.Value, token.Salt);
            }
            public override AntiForgeryData Deserialize(string serializedToken) {
                string[] parts = serializedToken.Split(':');
                return new AntiForgeryData() {
                    CreationDate = DateTime.Parse(parts[0], CultureInfo.InvariantCulture),
                    Value = parts[1],
                    Salt = parts[2]
                };
            }
        }

    }
}
