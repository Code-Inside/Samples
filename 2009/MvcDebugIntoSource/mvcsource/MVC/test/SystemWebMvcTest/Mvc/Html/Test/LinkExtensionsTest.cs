namespace System.Web.Mvc.Html.Test {
    using System;
    using System.Web.Mvc.Test;
    using System.Web.Routing;
    using System.Web.TestUtil;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class LinkExtensionsTest {
        private const string AppPathModifier = HtmlHelperTest.AppPathModifier;

        [TestMethod]
        public void ActionLink() {
            // Arrange
            HtmlHelper htmlHelper = HtmlHelperTest.GetHtmlHelper();

            // Act
            string html = htmlHelper.ActionLink("linktext", "newaction");

            // Assert
            Assert.AreEqual<string>(@"<a href=""" + AppPathModifier + @"/app/home/newaction"">linktext</a>", html);
        }

        [TestMethod]
        public void ActionLinkDictionaryOverridesImplicitValues() {
            // Arrange
            HtmlHelper htmlHelper = HtmlHelperTest.GetHtmlHelper();

            // Act
            string html = htmlHelper.ActionLink("linktext", "newaction", null, new { href = "http://foo.com" });

            // Assert
            Assert.AreEqual<string>(@"<a href=""http://foo.com"">linktext</a>", html);
        }

        [TestMethod]
        public void ActionLinkExplictValuesOverrideDictionary() {
            // Arrange
            HtmlHelper htmlHelper = HtmlHelperTest.GetHtmlHelper();

            // Act
            string html = htmlHelper.ActionLink("linktext", "explicitAction", new { action = "dictionaryAction" }, null);

            // Assert
            Assert.AreEqual<string>(@"<a href=""" + AppPathModifier + @"/app/home/explicitAction"">linktext</a>", html);
        }

        [TestMethod]
        public void ActionLinkParametersNeedEscaping() {
            // Arrange
            HtmlHelper htmlHelper = HtmlHelperTest.GetHtmlHelper();

            // Act
            string html = htmlHelper.ActionLink("linktext<&>\"", "new action<&>\"");

            // Assert
            Assert.AreEqual<string>(@"<a href=""" + AppPathModifier + @"/app/home/new%20action%3C&amp;%3E%22"">linktext&lt;&amp;&gt;&quot;</a>", html);
        }

        [TestMethod]
        public void ActionLinkWithActionNameAndValueDictionary() {
            // Arrange
            HtmlHelper htmlHelper = HtmlHelperTest.GetHtmlHelper();

            // Act
            string html = htmlHelper.ActionLink("linktext", "newaction", new RouteValueDictionary(new { controller = "home2" }));

            // Assert
            Assert.AreEqual<string>(@"<a href=""" + AppPathModifier + @"/app/home2/newaction"">linktext</a>", html);
        }

        [TestMethod]
        public void ActionLinkWithActionNameAndValueObject() {
            // Arrange
            HtmlHelper htmlHelper = HtmlHelperTest.GetHtmlHelper();

            // Act
            string html = htmlHelper.ActionLink("linktext", "newaction", new { controller = "home2" });

            // Assert
            Assert.AreEqual<string>(@"<a href=""" + AppPathModifier + @"/app/home2/newaction"">linktext</a>", html);
        }

        [TestMethod]
        public void ActionLinkWithControllerName() {
            // Arrange
            HtmlHelper htmlHelper = HtmlHelperTest.GetHtmlHelper();

            // Act
            string html = htmlHelper.ActionLink("linktext", "newaction", "home2");

            // Assert
            Assert.AreEqual<string>(@"<a href=""" + AppPathModifier + @"/app/home2/newaction"">linktext</a>", html);
        }

        [TestMethod]
        public void ActionLinkWithControllerNameAndDictionary() {
            // Arrange
            HtmlHelper htmlHelper = HtmlHelperTest.GetHtmlHelper();

            // Act
            string html = htmlHelper.ActionLink("linktext", "newaction", "home2", new RouteValueDictionary(new { id = "someid" }), new RouteValueDictionary(new { baz = "baz" }));

            // Assert
            Assert.AreEqual<string>(@"<a baz=""baz"" href=""" + AppPathModifier + @"/app/home2/newaction/someid"">linktext</a>", html);
        }

        [TestMethod]
        public void ActionLinkWithControllerNameAndObjectProperties() {
            // Arrange
            HtmlHelper htmlHelper = HtmlHelperTest.GetHtmlHelper();

            // Act
            string html = htmlHelper.ActionLink("linktext", "newaction", "home2", new { id = "someid" }, new { baz = "baz" });

            // Assert
            Assert.AreEqual<string>(@"<a baz=""baz"" href=""" + AppPathModifier + @"/app/home2/newaction/someid"">linktext</a>", html);
        }

        [TestMethod]
        public void ActionLinkWithDictionary() {
            // Arrange
            HtmlHelper htmlHelper = HtmlHelperTest.GetHtmlHelper();

            // Act
            string html = htmlHelper.ActionLink("linktext", "newaction", new RouteValueDictionary(new { Controller = "home2", id = "someid" }), new RouteValueDictionary(new { baz = "baz" }));

            // Assert
            Assert.AreEqual<string>(@"<a baz=""baz"" href=""" + AppPathModifier + @"/app/home2/newaction/someid"">linktext</a>", html);
        }

        [TestMethod]
        public void ActionLinkWithFragment() {
            // Arrange
            HtmlHelper htmlHelper = HtmlHelperTest.GetHtmlHelper();

            // Act
            string html = htmlHelper.ActionLink("linktext", "newaction", "home2", "http", "foo.bar.com", "foo", new { id = "someid" }, new { baz = "baz" });

            // Assert
            Assert.AreEqual<string>(@"<a baz=""baz"" href=""http://foo.bar.com" + AppPathModifier + @"/app/home2/newaction/someid#foo"">linktext</a>", html);
        }

        [TestMethod]
        public void ActionLinkWithNullHostname() {
            // Arrange
            HtmlHelper htmlHelper = HtmlHelperTest.GetHtmlHelper();

            // Act
            string html = htmlHelper.ActionLink("linktext", "newaction", "home2", "https", null /* hostName */, "foo", new { id = "someid" }, new { baz = "baz" });

            // Assert
            Assert.AreEqual<string>(@"<a baz=""baz"" href=""https://localhost" + AppPathModifier + @"/app/home2/newaction/someid#foo"">linktext</a>", html);
        }

        [TestMethod]
        public void ActionLinkWithNullProtocolAndFragment() {
            // Arrange
            HtmlHelper htmlHelper = HtmlHelperTest.GetHtmlHelper();

            // Act
            string html = htmlHelper.ActionLink("linktext", "newaction", "home2", null /* protocol */, "foo.bar.com", null /* fragment */, new { id = "someid" }, new { baz = "baz" });

            // Assert
            Assert.AreEqual<string>(@"<a baz=""baz"" href=""http://foo.bar.com" + AppPathModifier + @"/app/home2/newaction/someid"">linktext</a>", html);
        }

        [TestMethod]
        public void ActionLinkWithNullProtocolNullHostNameAndNullFragment() {
            // Arrange
            HtmlHelper htmlHelper = HtmlHelperTest.GetHtmlHelper();

            // Act
            string html = htmlHelper.ActionLink("linktext", "newaction", "home2", null /* protocol */, null /* hostName */, null /* fragment */, new { id = "someid" }, new { baz = "baz" });

            // Assert
            Assert.AreEqual<string>(@"<a baz=""baz"" href=""" + AppPathModifier + @"/app/home2/newaction/someid"">linktext</a>", html);
        }

        [TestMethod]
        public void ActionLinkWithObjectProperties() {
            // Arrange
            HtmlHelper htmlHelper = HtmlHelperTest.GetHtmlHelper();

            // Act
            string html = htmlHelper.ActionLink("linktext", "newaction", new { Controller = "home2", id = "someid" }, new { baz = "baz" });

            // Assert
            Assert.AreEqual<string>(@"<a baz=""baz"" href=""" + AppPathModifier + @"/app/home2/newaction/someid"">linktext</a>", html);
        }

        [TestMethod]
        public void ActionLinkWithProtocol() {
            // Arrange
            HtmlHelper htmlHelper = HtmlHelperTest.GetHtmlHelper();

            // Act
            string html = htmlHelper.ActionLink("linktext", "newaction", "home2", "https", "foo.bar.com", null /* fragment */, new { id = "someid" }, new { baz = "baz" });

            // Assert
            Assert.AreEqual<string>(@"<a baz=""baz"" href=""https://foo.bar.com" + AppPathModifier + @"/app/home2/newaction/someid"">linktext</a>", html);
        }

        [TestMethod]
        public void ActionLinkWithProtocolAndFragment() {
            // Arrange
            HtmlHelper htmlHelper = HtmlHelperTest.GetHtmlHelper();

            // Act
            string html = htmlHelper.ActionLink("linktext", "newaction", "home2", "https", "foo.bar.com", "foo", new { id = "someid" }, new { baz = "baz" });

            // Assert
            Assert.AreEqual<string>(@"<a baz=""baz"" href=""https://foo.bar.com" + AppPathModifier + @"/app/home2/newaction/someid#foo"">linktext</a>", html);
        }

        [TestMethod]
        public void ActionLinkWithDefaultPort() {
            // Arrange
            HtmlHelper htmlHelper = HtmlHelperTest.GetHtmlHelper(Uri.UriSchemeHttps, -1);

            // Act
            string html = htmlHelper.ActionLink("linktext", "newaction", "home2", "https", "foo.bar.com", "foo", new { id = "someid" }, new { baz = "baz" });

            // Assert
            Assert.AreEqual<string>(@"<a baz=""baz"" href=""https://foo.bar.com" + AppPathModifier + @"/app/home2/newaction/someid#foo"">linktext</a>", html);
        }

        [TestMethod]
        public void ActionLinkWithDifferentPortProtocols() {
            // Arrange
            HtmlHelper htmlHelper = HtmlHelperTest.GetHtmlHelper(Uri.UriSchemeHttp, -1);

            // Act
            string html = htmlHelper.ActionLink("linktext", "newaction", "home2", "https", "foo.bar.com", "foo", new { id = "someid" }, new { baz = "baz" });

            // Assert
            Assert.AreEqual<string>(@"<a baz=""baz"" href=""https://foo.bar.com" + AppPathModifier + @"/app/home2/newaction/someid#foo"">linktext</a>", html);
        }

        [TestMethod]
        public void ActionLinkWithNonDefaultPortAndDifferentProtocol() {
            // Arrange
            HtmlHelper htmlHelper = HtmlHelperTest.GetHtmlHelper(Uri.UriSchemeHttp, 32768);

            // Act
            string html = htmlHelper.ActionLink("linktext", "newaction", "home2", "https", "foo.bar.com", "foo", new { id = "someid" }, new { baz = "baz" });

            // Assert
            Assert.AreEqual<string>(@"<a baz=""baz"" href=""https://foo.bar.com" + AppPathModifier + @"/app/home2/newaction/someid#foo"">linktext</a>", html);
        }

        [TestMethod]
        public void ActionLinkWithNonDefaultPortAndSameProtocol() {
            // Arrange
            HtmlHelper htmlHelper = HtmlHelperTest.GetHtmlHelper(Uri.UriSchemeHttp, 32768);

            // Act
            string html = htmlHelper.ActionLink("linktext", "newaction", "home2", "http", "foo.bar.com", "foo", new { id = "someid" }, new { baz = "baz" });

            // Assert
            Assert.AreEqual<string>(@"<a baz=""baz"" href=""http://foo.bar.com:32768" + AppPathModifier + @"/app/home2/newaction/someid#foo"">linktext</a>", html);
        }

        [TestMethod]
        public void LinkGenerationDoesNotChangeProvidedDictionary() {
            // Arrange
            HtmlHelper htmlHelper = HtmlHelperTest.GetHtmlHelper();
            RouteValueDictionary valuesDictionary = new RouteValueDictionary();

            // Act
            htmlHelper.ActionLink("linkText", "actionName", valuesDictionary, new RouteValueDictionary());

            // Assert
            Assert.AreEqual(0, valuesDictionary.Count);
            Assert.IsFalse(valuesDictionary.ContainsKey("action"));
        }

        [TestMethod]
        public void NullOrEmptyStringParameterThrows() {
            // Arrange
            HtmlHelper htmlHelper = HtmlHelperTest.GetHtmlHelper();
            Func<Action, GenericDelegate> Wrap = action => new GenericDelegate(() => action());
            var tests = new[] {
                // ActionLink(string linkText, string actionName)
                new { Parameter = "linkText", Action = Wrap(() => htmlHelper.ActionLink(String.Empty, "actionName")) },

                // ActionLink(string linkText, string actionName, object routeValues, object htmlAttributes)
                new { Parameter = "linkText", Action = Wrap(() => htmlHelper.ActionLink(String.Empty, "actionName", new Object(), null /* htmlAttributes */)) },

                // ActionLink(string linkText, string actionName, RouteValueDictionary routeValues, IDictionary<string, object> htmlAttributes)
                new { Parameter = "linkText", Action = Wrap(() => htmlHelper.ActionLink(String.Empty, "actionName", new RouteValueDictionary(), new RouteValueDictionary())) },

                // ActionLink(string linkText, string actionName, string controllerName)
                new { Parameter = "linkText", Action = Wrap(() => htmlHelper.ActionLink(String.Empty, "actionName", "controllerName")) },

                // ActionLink(string linkText, string actionName, string controllerName, object routeValues, object htmlAttributes)
                new { Parameter = "linkText", Action = Wrap(() => htmlHelper.ActionLink(String.Empty, "actionName", "controllerName", new Object(), null /* htmlAttributes */)) },

                // ActionLink(string linkText, string actionName, string controllerName, RouteValueDictionary routeValues, IDictionary<string, object> htmlAttributes)
                new { Parameter = "linkText", Action = Wrap(() => htmlHelper.ActionLink(String.Empty, "actionName", "controllerName", new RouteValueDictionary(), new RouteValueDictionary())) },

                // ActionLink(string linkText, string actionName, string controllerName, string protocol, string hostName, string fragment, RouteValueDictionary routeValues, IDictionary<string, object> htmlAttributes)
                new { Parameter = "linkText", Action = Wrap(() => htmlHelper.ActionLink(String.Empty, "actionName", "controllerName", null, null, null, new RouteValueDictionary(), new RouteValueDictionary())) },

                // RouteLink(string linkText, object routeValues, object htmlAttributes)
                new { Parameter = "linkText", Action = Wrap(() => htmlHelper.RouteLink(String.Empty, new Object(), null /* htmlAttributes */)) },

                // RouteLink(string linkText, RouteValueDictionary routeValues, IDictionary<string, object> htmlAttributes)
                new { Parameter = "linkText", Action = Wrap(() => htmlHelper.RouteLink(String.Empty, new RouteValueDictionary(), new RouteValueDictionary())) },

                // RouteLink(string linkText, string routeName, object routeValues)
                new { Parameter = "linkText", Action = Wrap(() => htmlHelper.RouteLink(String.Empty, "routeName", null /* routeValues */)) },

                // RouteLink(string linkText, string routeName, RouteValueDictionary routeValues)
                new { Parameter = "linkText", Action = Wrap(() => htmlHelper.RouteLink(String.Empty, "routeName", new RouteValueDictionary() /* routeValues */)) },

                // RouteLink(string linkText, object routeValues)
                new { Parameter = "linkText", Action = Wrap(() => htmlHelper.RouteLink(String.Empty, null /* routeValues */)) },

                // RouteLink(string linkText, RouteValueDictionary routeValues)
                new { Parameter = "linkText", Action = Wrap(() => htmlHelper.RouteLink(String.Empty, new RouteValueDictionary() /* routeValues */)) },

                // RouteLink(string linkText, string routeName, object routeValues, object htmlAttributes)
                new { Parameter = "linkText", Action = Wrap(() => htmlHelper.RouteLink(String.Empty, "routeName", new Object(), null /* htmlAttributes */)) },

                // RouteLink(string linkText, string routeName, RouteValueDictionary routeValues, IDictionary<string, object> htmlAttributes)
                new { Parameter = "linkText", Action = Wrap(() => htmlHelper.RouteLink(String.Empty, "routeName", new RouteValueDictionary(), new RouteValueDictionary())) },

                // RouteLink(string linkText, string routeName, string protocol, string hostName, string fragment, RouteValueDictionary routeValues, IDictionary<string, object> htmlAttributes)
                new { Parameter = "linkText", Action = Wrap(() => htmlHelper.RouteLink(String.Empty, "routeName", null, null, null, new RouteValueDictionary(), new RouteValueDictionary())) },
            };

            // Act & Assert
            foreach (var test in tests) {
                ExceptionHelper.ExpectArgumentExceptionNullOrEmpty(test.Action, test.Parameter);
            }
        }

        [TestMethod]
        public void RouteLinkCanUseNamedRouteWithoutSpecifyingDefaults() {
            // DevDiv 217072: Non-mvc specific helpers should not give default values for controller and action

            // Arrange
            HtmlHelper htmlHelper = HtmlHelperTest.GetHtmlHelper();
            htmlHelper.RouteCollection.MapRoute("MyRouteName", "any/url", new { controller = "Charlie" });

            // Act
            string html = htmlHelper.RouteLink("linktext", "MyRouteName", null /* routeValues */);

            // Assert
            Assert.AreEqual<string>(@"<a href=""" + AppPathModifier + @"/app/any/url"">linktext</a>", html);
        }

        [TestMethod]
        public void RouteLinkWithDictionary() {
            // Arrange
            HtmlHelper htmlHelper = HtmlHelperTest.GetHtmlHelper();

            // Act
            string html = htmlHelper.RouteLink("linktext", new RouteValueDictionary(new { Action = "newaction", Controller = "home2", id = "someid" }), new RouteValueDictionary(new { baz = "baz" }));

            // Assert
            Assert.AreEqual<string>(@"<a baz=""baz"" href=""" + AppPathModifier + @"/app/home2/newaction/someid"">linktext</a>", html);
        }

        [TestMethod]
        public void RouteLinkWithFragment() {
            // Arrange
            HtmlHelper htmlHelper = HtmlHelperTest.GetHtmlHelper();

            // Act
            string html = htmlHelper.RouteLink("linktext", "namedroute", "http", "foo.bar.com", "foo", new { Action = "newaction", Controller = "home2", id = "someid" }, new { baz = "baz" });

            // Assert
            Assert.AreEqual<string>(@"<a baz=""baz"" href=""http://foo.bar.com" + AppPathModifier + @"/app/named/home2/newaction/someid#foo"">linktext</a>", html);
        }

        [TestMethod]
        public void RouteLinkWithObjectProperties() {
            // Arrange
            HtmlHelper htmlHelper = HtmlHelperTest.GetHtmlHelper();

            // Act
            string html = htmlHelper.RouteLink("linktext", new { Action = "newaction", Controller = "home2", id = "someid" }, new { baz = "baz" });

            // Assert
            Assert.AreEqual<string>(@"<a baz=""baz"" href=""" + AppPathModifier + @"/app/home2/newaction/someid"">linktext</a>", html);
        }

        [TestMethod]
        public void RouteLinkWithProtocol() {
            // Arrange
            HtmlHelper htmlHelper = HtmlHelperTest.GetHtmlHelper();

            // Act
            string html = htmlHelper.RouteLink("linktext", "namedroute", "https", "foo.bar.com", null /* fragment */, new { Action = "newaction", Controller = "home2", id = "someid" }, new { baz = "baz" });

            // Assert
            Assert.AreEqual<string>(@"<a baz=""baz"" href=""https://foo.bar.com" + AppPathModifier + @"/app/named/home2/newaction/someid"">linktext</a>", html);
        }

        [TestMethod]
        public void RouteLinkWithProtocolAndFragment() {
            // Arrange
            HtmlHelper htmlHelper = HtmlHelperTest.GetHtmlHelper();

            // Act
            string html = htmlHelper.RouteLink("linktext", "namedroute", "https", "foo.bar.com", "foo", new { Action = "newaction", Controller = "home2", id = "someid" }, new { baz = "baz" });

            // Assert
            Assert.AreEqual<string>(@"<a baz=""baz"" href=""https://foo.bar.com" + AppPathModifier + @"/app/named/home2/newaction/someid#foo"">linktext</a>", html);
        }

        [TestMethod]
        public void RouteLinkWithRouteNameAndDefaults() {
            // Arrange
            HtmlHelper htmlHelper = HtmlHelperTest.GetHtmlHelper();

            // Act
            string html = htmlHelper.RouteLink("linktext", "namedroute", new { Action = "newaction" });

            // Assert
            Assert.AreEqual<string>(@"<a href=""" + AppPathModifier + @"/app/named/home/newaction"">linktext</a>", html);
        }

        [TestMethod]
        public void RouteLinkWithRouteNameAndDictionary() {
            // Arrange
            HtmlHelper htmlHelper = HtmlHelperTest.GetHtmlHelper();

            // Act
            string html = htmlHelper.RouteLink("linktext", "namedroute", new RouteValueDictionary(new { Action = "newaction", Controller = "home2", id = "someid" }), new RouteValueDictionary());

            // Assert
            Assert.AreEqual<string>(@"<a href=""" + AppPathModifier + @"/app/named/home2/newaction/someid"">linktext</a>", html);
        }

        [TestMethod]
        public void RouteLinkWithRouteNameAndObjectProperties() {
            // Arrange
            HtmlHelper htmlHelper = HtmlHelperTest.GetHtmlHelper();

            // Act
            string html = htmlHelper.RouteLink("linktext", "namedroute", new { Action = "newaction", Controller = "home2", id = "someid" }, new { baz = "baz" });

            // Assert
            Assert.AreEqual<string>(@"<a baz=""baz"" href=""" + AppPathModifier + @"/app/named/home2/newaction/someid"">linktext</a>", html);
        }
    }
}
