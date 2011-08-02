namespace System.Web.Mvc.Ajax.Test {
    using System.Collections.Generic;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Mvc.Test;
    using System.Web.Routing;
    using System.Web.TestUtil;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

    [TestClass]
    public class AjaxExtensionsTest {
        private const string AjaxForm = @"<form action=""/rawUrl"" method=""post"" onsubmit=""Sys.Mvc.AsyncForm.handleSubmit(this, new Sys.UI.DomEvent(event), { insertionMode: Sys.Mvc.InsertionMode.replace });"">";
        private const string AjaxFormWithDefaultAction = @"<form action=""" + HtmlHelperTest.AppPathModifier + @"/app/home/oldaction"" method=""post"" onsubmit=""Sys.Mvc.AsyncForm.handleSubmit(this, new Sys.UI.DomEvent(event), { insertionMode: Sys.Mvc.InsertionMode.replace });"">";
        private const string AjaxFormWithDefaultController = @"<form action=""" + HtmlHelperTest.AppPathModifier + @"/app/home/Action"" method=""post"" onsubmit=""Sys.Mvc.AsyncForm.handleSubmit(this, new Sys.UI.DomEvent(event), { insertionMode: Sys.Mvc.InsertionMode.replace });"">";
        private const string AjaxFormWithId = @"<form action=""" + HtmlHelperTest.AppPathModifier + @"/app/Controller/Action/5"" method=""post"" onsubmit=""Sys.Mvc.AsyncForm.handleSubmit(this, new Sys.UI.DomEvent(event), { insertionMode: Sys.Mvc.InsertionMode.replace });"">";
        private const string AjaxFormWithIdAndHtmlAttributes = @"<form action=""" + HtmlHelperTest.AppPathModifier + @"/app/Controller/Action/5"" method=""get"" onsubmit=""Sys.Mvc.AsyncForm.handleSubmit(this, new Sys.UI.DomEvent(event), { insertionMode: Sys.Mvc.InsertionMode.replace });"">";
        private const string AjaxFormWithEmptyOptions = @"<form action=""" + HtmlHelperTest.AppPathModifier + @"/app/Controller/Action"" method=""post"" onsubmit=""Sys.Mvc.AsyncForm.handleSubmit(this, new Sys.UI.DomEvent(event), { insertionMode: Sys.Mvc.InsertionMode.replace });"">";
        private const string AjaxFormWithTargetId = @"<form action=""" + HtmlHelperTest.AppPathModifier + @"/app/Controller/Action"" method=""post"" onsubmit=""Sys.Mvc.AsyncForm.handleSubmit(this, new Sys.UI.DomEvent(event), { insertionMode: Sys.Mvc.InsertionMode.replace, updateTargetId: 'some-id' });"">";
        private const string AjaxFormWithHtmlAttributes = @"<form action=""" + HtmlHelperTest.AppPathModifier + @"/app/Controller/Action"" method=""get"" onsubmit=""Sys.Mvc.AsyncForm.handleSubmit(this, new Sys.UI.DomEvent(event), { insertionMode: Sys.Mvc.InsertionMode.replace, updateTargetId: 'some-id' });"">";
        private const string AjaxFormClose = "</form>";
        private const string AjaxRouteFormWithNamedRoute = @"<form action=""" + HtmlHelperTest.AppPathModifier + @"/app/named/home/oldaction"" method=""post"" onsubmit=""Sys.Mvc.AsyncForm.handleSubmit(this, new Sys.UI.DomEvent(event), { insertionMode: Sys.Mvc.InsertionMode.replace });"">";
        private const string AjaxRouteFormWithNamedRouteNoDefaults = @"<form action=""" + HtmlHelperTest.AppPathModifier + @"/app/any/url"" method=""post"" onsubmit=""Sys.Mvc.AsyncForm.handleSubmit(this, new Sys.UI.DomEvent(event), { insertionMode: Sys.Mvc.InsertionMode.replace });"">";
        private const string AjaxRouteFormWithEmptyOptions = @"<form action=""" + HtmlHelperTest.AppPathModifier + @"/app/named/home/oldaction"" method=""post"" onsubmit=""Sys.Mvc.AsyncForm.handleSubmit(this, new Sys.UI.DomEvent(event), { insertionMode: Sys.Mvc.InsertionMode.replace });"">";
        private const string AjaxRouteFormWithHtmlAttributes = @"<form action=""" + HtmlHelperTest.AppPathModifier + @"/app/named/home/oldaction"" method=""get"" onsubmit=""Sys.Mvc.AsyncForm.handleSubmit(this, new Sys.UI.DomEvent(event), { insertionMode: Sys.Mvc.InsertionMode.replace, updateTargetId: 'some-id' });"">";

        private static readonly object _valuesObjectDictionary = new { id = 5 };
        private static readonly object _attributesObjectDictionary = new { method = "post" };

        [TestMethod]
        public void ActionLinkWithNullActionName() {
            // Arrange
            AjaxHelper ajaxHelper = GetAjaxHelper();

            // Act
            string actionLink = ajaxHelper.ActionLink("linkText", null, GetEmptyOptions());

            // Assert
            Assert.AreEqual(@"<a href=""" + HtmlHelperTest.AppPathModifier + @"/app/home/oldaction"" onclick=""Sys.Mvc.AsyncHyperlink.handleClick(this, new Sys.UI.DomEvent(event), { insertionMode: Sys.Mvc.InsertionMode.replace });"">linkText</a>", actionLink);
        }

        [TestMethod]
        public void ActionLinkWithNullActionNameAndNullOptions() {
            // Arrange
            AjaxHelper ajaxHelper = GetAjaxHelper();

            // Act
            string actionLink = ajaxHelper.ActionLink("linkText", null, null);

            // Assert
            Assert.AreEqual(@"<a href=""" + HtmlHelperTest.AppPathModifier + @"/app/home/oldaction"" onclick=""Sys.Mvc.AsyncHyperlink.handleClick(this, new Sys.UI.DomEvent(event), { insertionMode: Sys.Mvc.InsertionMode.replace });"">linkText</a>", actionLink);
        }

        [TestMethod]
        public void ActionLinkWithNullOrEmptyLinkTextThrows() {
            // Arrange
            AjaxHelper ajaxHelper = GetAjaxHelper();

            // Act & Assert
            ExceptionHelper.ExpectArgumentExceptionNullOrEmpty(
                delegate {
                    string actionLink = ajaxHelper.ActionLink(String.Empty, String.Empty, null, null, null, null);
                },
                "linkText");
        }

        [TestMethod]
        public void ActionLink() {
            // Arrange
            AjaxHelper ajaxHelper = GetAjaxHelper();

            // Act
            string actionLink = ajaxHelper.ActionLink("linkText", "Action", GetEmptyOptions());

            // Assert
            Assert.AreEqual(@"<a href=""" + HtmlHelperTest.AppPathModifier + @"/app/home/Action"" onclick=""Sys.Mvc.AsyncHyperlink.handleClick(this, new Sys.UI.DomEvent(event), { insertionMode: Sys.Mvc.InsertionMode.replace });"">linkText</a>", actionLink);
        }

        [TestMethod]
        public void ActionLinkAnonymousValues() {
            // Arrange
            AjaxHelper helper = GetAjaxHelper();
            object values = new { controller = "Controller" };
            AjaxOptions options = new AjaxOptions { UpdateTargetId = "update-div" };

            // Act
            string actionLink = helper.ActionLink("Some Text", "Action", values, options);

            // Assert
            Assert.AreEqual(@"<a href=""" + HtmlHelperTest.AppPathModifier + @"/app/Controller/Action"" onclick=""Sys.Mvc.AsyncHyperlink.handleClick(this, new Sys.UI.DomEvent(event), { insertionMode: Sys.Mvc.InsertionMode.replace, updateTargetId: 'update-div' });"">Some Text</a>", actionLink);
        }

        [TestMethod]
        public void ActionLinkAnonymousValuesAndAttributes() {
            // Arrange
            AjaxHelper helper = GetAjaxHelper();
            object htmlAttributes = new { foo = "bar", baz = "quux" };
            object values = new { controller = "Controller" };
            AjaxOptions options = new AjaxOptions { UpdateTargetId = "update-div" };

            // Act
            string actionLink = helper.ActionLink("Some Text", "Action", values, options, htmlAttributes);

            // Assert
            Assert.AreEqual(@"<a baz=""quux"" foo=""bar"" href=""" + HtmlHelperTest.AppPathModifier + @"/app/Controller/Action"" onclick=""Sys.Mvc.AsyncHyperlink.handleClick(this, new Sys.UI.DomEvent(event), { insertionMode: Sys.Mvc.InsertionMode.replace, updateTargetId: 'update-div' });"">Some Text</a>", actionLink);
        }

        [TestMethod]
        public void ActionLinkTypedValues() {
            // Arrange
            AjaxHelper helper = GetAjaxHelper();
            RouteValueDictionary values = new RouteValueDictionary {
                { "controller", "Controller" }
            };

            AjaxOptions options = new AjaxOptions { UpdateTargetId = "update-div" };

            // Act
            string actionLink = helper.ActionLink("Some Text", "Action", values, options);

            // Assert
            Assert.AreEqual(@"<a href=""" + HtmlHelperTest.AppPathModifier + @"/app/Controller/Action"" onclick=""Sys.Mvc.AsyncHyperlink.handleClick(this, new Sys.UI.DomEvent(event), { insertionMode: Sys.Mvc.InsertionMode.replace, updateTargetId: 'update-div' });"">Some Text</a>", actionLink);
        }

        [TestMethod]
        public void ActionLinkTypedValuesAndAttributes() {
            // Arrange
            AjaxHelper helper = GetAjaxHelper();
            RouteValueDictionary values = new RouteValueDictionary {
                { "controller", "Controller" }
            };
            Dictionary<string, object> htmlAttributes = new Dictionary<string, object> {
                { "foo", "bar" },
                { "baz", "quux" }
            };
            AjaxOptions options = new AjaxOptions { UpdateTargetId = "update-div" };

            // Act
            string actionLink = helper.ActionLink("Some Text", "Action", values, options, htmlAttributes);

            // Assert
            Assert.AreEqual(@"<a baz=""quux"" foo=""bar"" href=""" + HtmlHelperTest.AppPathModifier + @"/app/Controller/Action"" onclick=""Sys.Mvc.AsyncHyperlink.handleClick(this, new Sys.UI.DomEvent(event), { insertionMode: Sys.Mvc.InsertionMode.replace, updateTargetId: 'update-div' });"">Some Text</a>", actionLink);
        }

        [TestMethod]
        public void ActionLinkController() {
            // Arrange
            AjaxHelper ajaxHelper = GetAjaxHelper();

            // Act
            string actionLink = ajaxHelper.ActionLink("linkText", "Action", "Controller", GetEmptyOptions());

            // Assert
            Assert.AreEqual(@"<a href=""" + HtmlHelperTest.AppPathModifier + @"/app/Controller/Action"" onclick=""Sys.Mvc.AsyncHyperlink.handleClick(this, new Sys.UI.DomEvent(event), { insertionMode: Sys.Mvc.InsertionMode.replace });"">linkText</a>", actionLink);
        }

        [TestMethod]
        public void ActionLinkControllerAnonymousValues() {
            // Arrange
            AjaxHelper helper = GetAjaxHelper();
            object values = new { id = 5 };
            AjaxOptions options = new AjaxOptions { UpdateTargetId = "update-div" };

            // Act
            string actionLink = helper.ActionLink("Some Text", "Action", "Controller", values, options);

            // Assert
            Assert.AreEqual(@"<a href=""" + HtmlHelperTest.AppPathModifier + @"/app/Controller/Action/5"" onclick=""Sys.Mvc.AsyncHyperlink.handleClick(this, new Sys.UI.DomEvent(event), { insertionMode: Sys.Mvc.InsertionMode.replace, updateTargetId: 'update-div' });"">Some Text</a>", actionLink);
        }

        [TestMethod]
        public void ActionLinkControllerAnonymousValuesAndAttributes() {
            // Arrange
            AjaxHelper helper = GetAjaxHelper();
            object htmlAttributes = new { foo = "bar", baz = "quux" };
            object values = new { id = 5 };
            AjaxOptions options = new AjaxOptions { UpdateTargetId = "update-div" };

            // Act
            string actionLink = helper.ActionLink("Some Text", "Action", "Controller", values, options, htmlAttributes);

            // Assert
            Assert.AreEqual(@"<a baz=""quux"" foo=""bar"" href=""" + HtmlHelperTest.AppPathModifier + @"/app/Controller/Action/5"" onclick=""Sys.Mvc.AsyncHyperlink.handleClick(this, new Sys.UI.DomEvent(event), { insertionMode: Sys.Mvc.InsertionMode.replace, updateTargetId: 'update-div' });"">Some Text</a>", actionLink);
        }

        [TestMethod]
        public void ActionLinkControllerTypedValues() {
            // Arrange
            AjaxHelper helper = GetAjaxHelper();
            RouteValueDictionary values = new RouteValueDictionary {
                { "id", 5 }
            };
            AjaxOptions options = new AjaxOptions { UpdateTargetId = "update-div" };

            // Act
            string actionLink = helper.ActionLink("Some Text", "Action", "Controller", values, options);

            // Assert
            Assert.AreEqual(@"<a href=""" + HtmlHelperTest.AppPathModifier + @"/app/Controller/Action/5"" onclick=""Sys.Mvc.AsyncHyperlink.handleClick(this, new Sys.UI.DomEvent(event), { insertionMode: Sys.Mvc.InsertionMode.replace, updateTargetId: 'update-div' });"">Some Text</a>", actionLink);
        }

        [TestMethod]
        public void ActionLinkControllerTypedValuesAndAttributes() {
            // Arrange
            AjaxHelper helper = GetAjaxHelper();
            RouteValueDictionary values = new RouteValueDictionary {
                { "id",5}
            };
            Dictionary<string, object> htmlAttributes = new Dictionary<string, object> {
                { "foo", "bar" },
                { "baz", "quux" }
            };
            AjaxOptions options = new AjaxOptions { UpdateTargetId = "update-div" };

            // Act
            string actionLink = helper.ActionLink("Some Text", "Action", "Controller", values, options, htmlAttributes);

            // Assert
            Assert.AreEqual(@"<a baz=""quux"" foo=""bar"" href=""" + HtmlHelperTest.AppPathModifier + @"/app/Controller/Action/5"" onclick=""Sys.Mvc.AsyncHyperlink.handleClick(this, new Sys.UI.DomEvent(event), { insertionMode: Sys.Mvc.InsertionMode.replace, updateTargetId: 'update-div' });"">Some Text</a>", actionLink);
        }

        [TestMethod]
        public void ActionLinkWithOptions() {
            // Arrange
            AjaxHelper ajaxHelper = GetAjaxHelper();

            // Act
            string actionLink = ajaxHelper.ActionLink("linkText", "Action", "Controller", new AjaxOptions { UpdateTargetId = "some-id" });

            // Assert
            Assert.AreEqual(@"<a href=""" + HtmlHelperTest.AppPathModifier + @"/app/Controller/Action"" onclick=""Sys.Mvc.AsyncHyperlink.handleClick(this, new Sys.UI.DomEvent(event), { insertionMode: Sys.Mvc.InsertionMode.replace, updateTargetId: 'some-id' });"">linkText</a>", actionLink);
        }

        [TestMethod]
        public void ActionLinkWithNullHostName() {
            // Arrange
            AjaxHelper ajaxHelper = GetAjaxHelper();

            // Act
            string actionLink = ajaxHelper.ActionLink("linkText", "Action", "Controller",
                null, null, null, null, new AjaxOptions { UpdateTargetId = "some-id" }, null);

            // Assert
            Assert.AreEqual(@"<a href=""" + HtmlHelperTest.AppPathModifier + @"/app/Controller/Action"" onclick=""Sys.Mvc.AsyncHyperlink.handleClick(this, new Sys.UI.DomEvent(event), { insertionMode: Sys.Mvc.InsertionMode.replace, updateTargetId: 'some-id' });"">linkText</a>", actionLink);
        }

        [TestMethod]
        public void ActionLinkWithProtocol() {
            // Arrange
            AjaxHelper ajaxHelper = GetAjaxHelper();

            // Act
            string actionLink = ajaxHelper.ActionLink("linkText", "Action", "Controller", "https", null, null, null, new AjaxOptions { UpdateTargetId = "some-id" }, null);

            // Assert
            Assert.AreEqual(@"<a href=""https://foo.bar.baz" + HtmlHelperTest.AppPathModifier + @"/app/Controller/Action"" onclick=""Sys.Mvc.AsyncHyperlink.handleClick(this, new Sys.UI.DomEvent(event), { insertionMode: Sys.Mvc.InsertionMode.replace, updateTargetId: 'some-id' });"">linkText</a>", actionLink);
        }

        [TestMethod]
        public void RouteLinkWithNullOrEmptyLinkTextThrows() {
            // Arrange
            AjaxHelper ajaxHelper = GetAjaxHelper();

            // Act & Assert
            ExceptionHelper.ExpectArgumentExceptionNullOrEmpty(
                delegate {
                    string actionLink = ajaxHelper.RouteLink(String.Empty, String.Empty, null, null, null);
                },
                "linkText");
        }

        [TestMethod]
        public void RouteLinkWithNullOptions() {
            // Arrange
            AjaxHelper ajaxHelper = GetAjaxHelper();

            // Act
            string routeLink = ajaxHelper.RouteLink("Some Text", new RouteValueDictionary(), null);

            // Assert
            Assert.AreEqual(@"<a href=""" + HtmlHelperTest.AppPathModifier + @"/app/home/oldaction"" onclick=""Sys.Mvc.AsyncHyperlink.handleClick(this, new Sys.UI.DomEvent(event), { insertionMode: Sys.Mvc.InsertionMode.replace });"">Some Text</a>", routeLink);
        }

        [TestMethod]
        public void RouteLinkAnonymousValues() {
            // Arrange
            AjaxHelper helper = GetAjaxHelper();
            object values = new {
                action = "Action",
                controller = "Controller"
            };
            AjaxOptions options = new AjaxOptions { UpdateTargetId = "update-div" };

            // Act
            string routeLink = helper.RouteLink("Some Text", values, options);

            // Assert
            Assert.AreEqual(@"<a href=""" + HtmlHelperTest.AppPathModifier + @"/app/Controller/Action"" onclick=""Sys.Mvc.AsyncHyperlink.handleClick(this, new Sys.UI.DomEvent(event), { insertionMode: Sys.Mvc.InsertionMode.replace, updateTargetId: 'update-div' });"">Some Text</a>", routeLink);
        }

        [TestMethod]
        public void RouteLinkAnonymousValuesAndAttributes() {
            // Arrange
            AjaxHelper helper = GetAjaxHelper();
            object htmlAttributes = new {
                foo = "bar",
                baz = "quux"
            };
            object values = new {
                action = "Action",
                controller = "Controller"
            };
            AjaxOptions options = new AjaxOptions { UpdateTargetId = "update-div" };

            // Act
            string actionLink = helper.RouteLink("Some Text", values, options, htmlAttributes);

            // Assert
            Assert.AreEqual(@"<a baz=""quux"" foo=""bar"" href=""" + HtmlHelperTest.AppPathModifier + @"/app/Controller/Action"" onclick=""Sys.Mvc.AsyncHyperlink.handleClick(this, new Sys.UI.DomEvent(event), { insertionMode: Sys.Mvc.InsertionMode.replace, updateTargetId: 'update-div' });"">Some Text</a>", actionLink);
        }

        [TestMethod]
        public void RouteLinkTypedValues() {
            // Arrange
            AjaxHelper helper = GetAjaxHelper();
            RouteValueDictionary values = new RouteValueDictionary {
                { "controller", "Controller" },
                { "action", "Action" }
            };

            AjaxOptions options = new AjaxOptions { UpdateTargetId = "update-div" };

            // Act
            string actionLink = helper.RouteLink("Some Text", values, options);

            // Assert
            Assert.AreEqual(@"<a href=""" + HtmlHelperTest.AppPathModifier + @"/app/Controller/Action"" onclick=""Sys.Mvc.AsyncHyperlink.handleClick(this, new Sys.UI.DomEvent(event), { insertionMode: Sys.Mvc.InsertionMode.replace, updateTargetId: 'update-div' });"">Some Text</a>", actionLink);
        }

        [TestMethod]
        public void RouteLinkTypedValuesAndAttributes() {
            // Arrange
            AjaxHelper helper = GetAjaxHelper();
            RouteValueDictionary values = new RouteValueDictionary {
                { "controller", "Controller" },
                { "action", "Action" }
            };
            Dictionary<string, object> htmlAttributes = new Dictionary<string, object> {
                { "foo", "bar" },
                { "baz", "quux" }
            };
            AjaxOptions options = new AjaxOptions { UpdateTargetId = "update-div" };

            // Act
            string actionLink = helper.RouteLink("Some Text", values, options, htmlAttributes);

            // Assert
            Assert.AreEqual(@"<a baz=""quux"" foo=""bar"" href=""" + HtmlHelperTest.AppPathModifier + @"/app/Controller/Action"" onclick=""Sys.Mvc.AsyncHyperlink.handleClick(this, new Sys.UI.DomEvent(event), { insertionMode: Sys.Mvc.InsertionMode.replace, updateTargetId: 'update-div' });"">Some Text</a>", actionLink);
        }

        [TestMethod]
        public void RouteLinkNamedRoute() {
            // Arrange
            AjaxHelper ajaxHelper = GetAjaxHelper();

            // Act
            string actionLink = ajaxHelper.RouteLink("linkText", "namedroute", GetEmptyOptions());

            // Assert
            Assert.AreEqual(@"<a href=""" + HtmlHelperTest.AppPathModifier + @"/app/named/home/oldaction"" onclick=""Sys.Mvc.AsyncHyperlink.handleClick(this, new Sys.UI.DomEvent(event), { insertionMode: Sys.Mvc.InsertionMode.replace });"">linkText</a>", actionLink);
        }

        [TestMethod]
        public void RouteLinkNamedRouteAnonymousAttributes() {
            // Arrange
            AjaxHelper ajaxHelper = GetAjaxHelper();
            object htmlAttributes = new {
                foo = "bar",
                baz = "quux"
            };
            AjaxOptions options = new AjaxOptions { UpdateTargetId = "update-div" };

            // Act
            string actionLink = ajaxHelper.RouteLink("Some Text", "namedroute", options, htmlAttributes);

            // Assert
            Assert.AreEqual(@"<a baz=""quux"" foo=""bar"" href=""" + HtmlHelperTest.AppPathModifier + @"/app/named/home/oldaction"" onclick=""Sys.Mvc.AsyncHyperlink.handleClick(this, new Sys.UI.DomEvent(event), { insertionMode: Sys.Mvc.InsertionMode.replace, updateTargetId: 'update-div' });"">Some Text</a>", actionLink);
        }

        [TestMethod]
        public void RouteLinkNamedRouteTypedAttributes() {
            // Arrange
            AjaxHelper ajaxHelper = GetAjaxHelper();
            Dictionary<string, object> htmlAttributes = new Dictionary<string, object> { { "foo", "bar" }, { "baz", "quux" } };
            AjaxOptions options = new AjaxOptions { UpdateTargetId = "update-div" };

            // Act
            string actionLink = ajaxHelper.RouteLink("Some Text", "namedroute", options, htmlAttributes);

            // Assert
            Assert.AreEqual(@"<a baz=""quux"" foo=""bar"" href=""" + HtmlHelperTest.AppPathModifier + @"/app/named/home/oldaction"" onclick=""Sys.Mvc.AsyncHyperlink.handleClick(this, new Sys.UI.DomEvent(event), { insertionMode: Sys.Mvc.InsertionMode.replace, updateTargetId: 'update-div' });"">Some Text</a>", actionLink);
        }

        [TestMethod]
        public void RouteLinkNamedRouteWithAnonymousValues() {
            // Arrange
            AjaxHelper ajaxHelper = GetAjaxHelper();
            object values = new {
                action = "Action",
                controller = "Controller"
            };

            // Act
            string actionLink = ajaxHelper.RouteLink("linkText", "namedroute", values, GetEmptyOptions());

            // Assert
            Assert.AreEqual(@"<a href=""" + HtmlHelperTest.AppPathModifier + @"/app/named/Controller/Action"" onclick=""Sys.Mvc.AsyncHyperlink.handleClick(this, new Sys.UI.DomEvent(event), { insertionMode: Sys.Mvc.InsertionMode.replace });"">linkText</a>", actionLink);
        }

        [TestMethod]
        public void RouteLinkNamedRouteAnonymousValuesAndAttributes() {
            // Arrange
            AjaxHelper ajaxHelper = GetAjaxHelper();
            object values = new {
                action = "Action",
                controller = "Controller"
            };

            object htmlAttributes = new {
                foo = "bar",
                baz = "quux"
            };
            AjaxOptions options = new AjaxOptions { UpdateTargetId = "update-div" };

            // Act
            string actionLink = ajaxHelper.RouteLink("Some Text", "namedroute", values, options, htmlAttributes);

            // Assert
            Assert.AreEqual(@"<a baz=""quux"" foo=""bar"" href=""" + HtmlHelperTest.AppPathModifier + @"/app/named/Controller/Action"" onclick=""Sys.Mvc.AsyncHyperlink.handleClick(this, new Sys.UI.DomEvent(event), { insertionMode: Sys.Mvc.InsertionMode.replace, updateTargetId: 'update-div' });"">Some Text</a>", actionLink);
        }

        [TestMethod]
        public void RouteLinkNamedRouteWithTypedValues() {
            // Arrange
            AjaxHelper ajaxHelper = GetAjaxHelper();
            RouteValueDictionary values = new RouteValueDictionary {
                { "controller", "Controller" },
                { "action", "Action" }
            };

            // Act
            string actionLink = ajaxHelper.RouteLink("linkText", "namedroute", values, GetEmptyOptions());

            // Assert
            Assert.AreEqual(@"<a href=""" + HtmlHelperTest.AppPathModifier + @"/app/named/Controller/Action"" onclick=""Sys.Mvc.AsyncHyperlink.handleClick(this, new Sys.UI.DomEvent(event), { insertionMode: Sys.Mvc.InsertionMode.replace });"">linkText</a>", actionLink);
        }

        [TestMethod]
        public void RouteLinkNamedRouteTypedValuesAndAttributes() {
            // Arrange
            AjaxHelper ajaxHelper = GetAjaxHelper();
            RouteValueDictionary values = new RouteValueDictionary {
                { "controller", "Controller" },
                { "action", "Action" }
            };

            Dictionary<string, object> htmlAttributes = new Dictionary<string, object> { { "foo", "bar" }, { "baz", "quux" } };
            AjaxOptions options = new AjaxOptions { UpdateTargetId = "update-div" };

            // Act
            string actionLink = ajaxHelper.RouteLink("Some Text", "namedroute", values, options, htmlAttributes);

            // Assert
            Assert.AreEqual(@"<a baz=""quux"" foo=""bar"" href=""" + HtmlHelperTest.AppPathModifier + @"/app/named/Controller/Action"" onclick=""Sys.Mvc.AsyncHyperlink.handleClick(this, new Sys.UI.DomEvent(event), { insertionMode: Sys.Mvc.InsertionMode.replace, updateTargetId: 'update-div' });"">Some Text</a>", actionLink);
        }

        [TestMethod]
        public void RouteLinkNamedRouteNullValuesAndAttributes() {
            // Arrange
            AjaxHelper ajaxHelper = GetAjaxHelper();
            Dictionary<string, object> htmlAttributes = new Dictionary<string, object> { { "foo", "bar" }, { "baz", "quux" } };
            AjaxOptions options = new AjaxOptions { UpdateTargetId = "update-div" };

            // Act
            string actionLink = ajaxHelper.RouteLink("Some Text", "namedroute", null, options, htmlAttributes);

            // Assert
            Assert.AreEqual(@"<a baz=""quux"" foo=""bar"" href=""" + HtmlHelperTest.AppPathModifier + @"/app/named/home/oldaction"" onclick=""Sys.Mvc.AsyncHyperlink.handleClick(this, new Sys.UI.DomEvent(event), { insertionMode: Sys.Mvc.InsertionMode.replace, updateTargetId: 'update-div' });"">Some Text</a>", actionLink);
        }

        [TestMethod]
        public void RouteLinkWithHostName() {
            // Arrange
            AjaxHelper ajaxHelper = GetAjaxHelper();
            Dictionary<string, object> htmlAttributes = new Dictionary<string, object> { { "foo", "bar" }, { "baz", "quux" } };
            AjaxOptions options = new AjaxOptions { UpdateTargetId = "update-div" };

            // Act
            string actionLink = ajaxHelper.RouteLink("Some Text", "namedroute", null, "baz.bar.foo", null, null, options, htmlAttributes);

            // Assert
            Assert.AreEqual(@"<a baz=""quux"" foo=""bar"" href=""http://baz.bar.foo" + HtmlHelperTest.AppPathModifier + @"/app/named/home/oldaction"" onclick=""Sys.Mvc.AsyncHyperlink.handleClick(this, new Sys.UI.DomEvent(event), { insertionMode: Sys.Mvc.InsertionMode.replace, updateTargetId: 'update-div' });"">Some Text</a>", actionLink);
        }

        [TestMethod]
        public void FormOnlyWithNullOptions() {
            // Arrange
            Mock<HttpResponseBase> mockResponse = new Mock<HttpResponseBase>(MockBehavior.Strict);
            AjaxHelper ajaxHelper = GetAjaxHelper(mockResponse);
            AjaxOptions ajaxOptions = GetEmptyOptions();

            // Arrange expectations
            mockResponse.Expect(response => response.Write(AjaxForm)).Verifiable();

            // Act
            IDisposable form = ajaxHelper.BeginForm(null);

            // Assert
            mockResponse.Verify();
        }

        [TestMethod]
        public void FormWithNullActionName() {
            // Arrange
            Mock<HttpResponseBase> mockResponse = new Mock<HttpResponseBase>(MockBehavior.Strict);
            AjaxHelper ajaxHelper = GetAjaxHelper(mockResponse);
            AjaxOptions ajaxOptions = GetEmptyOptions();

            // Arrange expectations
            mockResponse.Expect(response => response.Write(AjaxFormWithDefaultAction)).Verifiable();

            // Act
            IDisposable form = ajaxHelper.BeginForm(null, ajaxOptions);

            // Assert
            mockResponse.Verify();
        }

        [TestMethod]
        public void FormWithNullOptions() {
            // Arrange
            Mock<HttpResponseBase> mockResponse = new Mock<HttpResponseBase>(MockBehavior.Strict);
            AjaxHelper ajaxHelper = GetAjaxHelper(mockResponse);
            AjaxOptions ajaxOptions = GetEmptyOptions();

            // Arrange expectations
            mockResponse.Expect(response => response.Write(AjaxFormWithEmptyOptions)).Verifiable();

            // Act
            IDisposable form = ajaxHelper.BeginForm("Action", "Controller", null);

            // Assert
            mockResponse.Verify();
        }

        [TestMethod]
        public void Form() {
            // Arrange
            Mock<HttpResponseBase> mockResponse = new Mock<HttpResponseBase>(MockBehavior.Strict);
            AjaxHelper ajaxHelper = GetAjaxHelper(mockResponse);
            AjaxOptions ajaxOptions = GetEmptyOptions();

            // Arrange expectations
            mockResponse.Expect(response => response.Write(AjaxForm)).Verifiable();

            // Act
            IDisposable form = ajaxHelper.BeginForm(ajaxOptions);

            // Assert
            mockResponse.Verify();
        }

        [TestMethod]
        public void FormAction() {
            // Arrange
            Mock<HttpResponseBase> mockResponse = new Mock<HttpResponseBase>(MockBehavior.Strict);
            AjaxHelper ajaxHelper = GetAjaxHelper(mockResponse);
            AjaxOptions ajaxOptions = GetEmptyOptions();

            // Arrange expectations
            mockResponse.Expect(response => response.Write(AjaxFormWithDefaultController)).Verifiable();

            // Act
            IDisposable form = ajaxHelper.BeginForm("Action", ajaxOptions);

            // Assert
            mockResponse.Verify();
        }

        [TestMethod]
        public void FormAnonymousValues() {
            // Arrange
            Mock<HttpResponseBase> mockResponse = new Mock<HttpResponseBase>(MockBehavior.Strict);
            AjaxHelper ajaxHelper = GetAjaxHelper(mockResponse);
            AjaxOptions ajaxOptions = GetEmptyOptions();
            object values = new { controller = "Controller" };

            // Arrange expectations
            mockResponse.Expect(response => response.Write(AjaxFormWithEmptyOptions)).Verifiable();

            // Act
            IDisposable form = ajaxHelper.BeginForm("Action", values, ajaxOptions);

            // Assert
            mockResponse.Verify();
        }

        [TestMethod]
        public void FormAnonymousValuesAndAttributes() {
            // Arrange
            Mock<HttpResponseBase> mockResponse = new Mock<HttpResponseBase>(MockBehavior.Strict);
            AjaxHelper ajaxHelper = GetAjaxHelper(mockResponse);
            AjaxOptions ajaxOptions = new AjaxOptions { UpdateTargetId = "some-id" };
            object values = new { controller = "Controller" };
            object htmlAttributes = new { method = "get" };

            // Arrange expectations
            mockResponse.Expect(response => response.Write(AjaxFormWithHtmlAttributes)).Verifiable();

            // Act
            IDisposable form = ajaxHelper.BeginForm("Action", values, ajaxOptions, htmlAttributes);

            // Assert
            mockResponse.Verify();
        }

        [TestMethod]
        public void FormTypedValues() {
            // Arrange
            Mock<HttpResponseBase> mockResponse = new Mock<HttpResponseBase>(MockBehavior.Strict);
            AjaxHelper ajaxHelper = GetAjaxHelper(mockResponse);
            AjaxOptions ajaxOptions = GetEmptyOptions();
            RouteValueDictionary values = new RouteValueDictionary {
                { "controller", "Controller" }
            };

            // Arrange expectations
            mockResponse.Expect(response => response.Write(AjaxFormWithEmptyOptions)).Verifiable();

            // Act
            IDisposable form = ajaxHelper.BeginForm("Action", values, ajaxOptions);

            // Assert
            mockResponse.Verify();
        }

        [TestMethod]
        public void FormTypedValuesAndAttributes() {
            // Arrange
            Mock<HttpResponseBase> mockResponse = new Mock<HttpResponseBase>(MockBehavior.Strict);
            AjaxHelper ajaxHelper = GetAjaxHelper(mockResponse);
            AjaxOptions ajaxOptions = new AjaxOptions { UpdateTargetId = "some-id" };
            RouteValueDictionary values = new RouteValueDictionary {
                { "controller", "Controller" }
            };
            Dictionary<string, object> htmlAttributes = new Dictionary<string, object> {
                { "method", "get"}
            };

            // Arrange expectations
            mockResponse.Expect(response => response.Write(AjaxFormWithHtmlAttributes)).Verifiable();

            // Act
            IDisposable form = ajaxHelper.BeginForm("Action", values, ajaxOptions, htmlAttributes);

            // Assert
            mockResponse.Verify();
        }

        [TestMethod]
        public void FormController() {
            // Arrange
            Mock<HttpResponseBase> mockResponse = new Mock<HttpResponseBase>(MockBehavior.Strict);
            AjaxHelper ajaxHelper = GetAjaxHelper(mockResponse);
            AjaxOptions ajaxOptions = GetEmptyOptions();

            // Arrange expectations
            mockResponse.Expect(response => response.Write(AjaxFormWithEmptyOptions)).Verifiable();

            // Act
            IDisposable form = ajaxHelper.BeginForm("Action", "Controller", ajaxOptions);

            // Assert
            mockResponse.Verify();
        }

        [TestMethod]
        public void FormControllerAnonymousValues() {
            // Arrange
            Mock<HttpResponseBase> mockResponse = new Mock<HttpResponseBase>(MockBehavior.Strict);
            AjaxHelper ajaxHelper = GetAjaxHelper(mockResponse);
            AjaxOptions ajaxOptions = GetEmptyOptions();
            object values = new { id = 5 };

            // Arrange expectations
            mockResponse.Expect(response => response.Write(AjaxFormWithId)).Verifiable();

            // Act
            IDisposable form = ajaxHelper.BeginForm("Action", "Controller", values, ajaxOptions);

            // Assert
            mockResponse.Verify();
        }

        [TestMethod]
        public void FormControllerAnonymousValuesAndAttributes() {
            // Arrange
            Mock<HttpResponseBase> mockResponse = new Mock<HttpResponseBase>(MockBehavior.Strict);
            AjaxHelper ajaxHelper = GetAjaxHelper(mockResponse);
            AjaxOptions ajaxOptions = GetEmptyOptions();
            object values = new { id = 5 };
            object htmlAttributes = new { method = "get" };

            // Arrange expectations
            mockResponse.Expect(response => response.Write(AjaxFormWithIdAndHtmlAttributes)).Verifiable();

            // Act
            IDisposable form = ajaxHelper.BeginForm("Action", "Controller", values, ajaxOptions, htmlAttributes);

            // Assert
            mockResponse.Verify();
        }

        [TestMethod]
        public void FormControllerTypedValues() {
            // Arrange
            Mock<HttpResponseBase> mockResponse = new Mock<HttpResponseBase>(MockBehavior.Strict);
            AjaxHelper ajaxHelper = GetAjaxHelper(mockResponse);
            AjaxOptions ajaxOptions = GetEmptyOptions();
            RouteValueDictionary values = new RouteValueDictionary {
                { "id", 5 }
            };

            // Arrange expectations
            mockResponse.Expect(response => response.Write(AjaxFormWithId)).Verifiable();

            // Act
            IDisposable form = ajaxHelper.BeginForm("Action", "Controller", values, ajaxOptions);

            // Assert
            mockResponse.Verify();
        }

        [TestMethod]
        public void FormControllerTypedValuesAndAttributes() {
            // Arrange
            Mock<HttpResponseBase> mockResponse = new Mock<HttpResponseBase>(MockBehavior.Strict);
            AjaxHelper ajaxHelper = GetAjaxHelper(mockResponse);
            AjaxOptions ajaxOptions = GetEmptyOptions();
            RouteValueDictionary values = new RouteValueDictionary {
                { "id", 5 }
            };
            Dictionary<string, object> htmlAttributes = new Dictionary<string, object> {
                { "method", "get"}
            };

            // Arrange expectations
            mockResponse.Expect(response => response.Write(AjaxFormWithIdAndHtmlAttributes)).Verifiable();

            // Act
            IDisposable form = ajaxHelper.BeginForm("Action", "Controller", values, ajaxOptions, htmlAttributes);

            // Assert
            mockResponse.Verify();
        }

        [TestMethod]
        public void FormWithTargetId() {
            // Arrange
            Mock<HttpResponseBase> mockResponse = new Mock<HttpResponseBase>(MockBehavior.Strict);
            AjaxHelper ajaxHelper = GetAjaxHelper(mockResponse);
            AjaxOptions ajaxOptions = new AjaxOptions { UpdateTargetId = "some-id" };

            // Arrange expectations
            mockResponse.Expect(response => response.Write(AjaxFormWithTargetId)).Verifiable();

            // Act
            IDisposable form = ajaxHelper.BeginForm("Action", "Controller", ajaxOptions);

            // Assert
            mockResponse.Verify();
        }

        [TestMethod]
        public void DisposeWritesClosingFormTag() {
            // Arrange
            Mock<HttpResponseBase> mockResponse = new Mock<HttpResponseBase>(MockBehavior.Strict);
            AjaxHelper ajaxHelper = GetAjaxHelper(mockResponse);
            AjaxOptions ajaxOptions = new AjaxOptions { UpdateTargetId = "some-id" };

            // Arrange expectations
            mockResponse.Expect(response => response.Write(AjaxFormWithTargetId)).Verifiable();
            mockResponse.Expect(response => response.Write(AjaxFormClose)).Verifiable();

            // Act
            IDisposable form = ajaxHelper.BeginForm("Action", "Controller", ajaxOptions);
            form.Dispose();

            // Assert
            mockResponse.Verify();
        }

        [TestMethod]
        public void InsertionModeToString() {
            // Act & Assert
            Assert.AreEqual(AjaxExtensions.InsertionModeToString(InsertionMode.Replace), "Sys.Mvc.InsertionMode.replace");
            Assert.AreEqual(AjaxExtensions.InsertionModeToString(InsertionMode.InsertAfter), "Sys.Mvc.InsertionMode.insertAfter");
            Assert.AreEqual(AjaxExtensions.InsertionModeToString(InsertionMode.InsertBefore), "Sys.Mvc.InsertionMode.insertBefore");
            Assert.AreEqual(AjaxExtensions.InsertionModeToString((InsertionMode)4), "4");
        }

        [TestMethod]
        public void RouteForm() {
            // Arrange
            Mock<HttpResponseBase> mockResponse = new Mock<HttpResponseBase>(MockBehavior.Strict);
            AjaxHelper ajaxHelper = GetAjaxHelper(mockResponse);
            AjaxOptions ajaxOptions = GetEmptyOptions();

            // Arrange expectations
            mockResponse.Expect(response => response.Write(AjaxRouteFormWithNamedRoute)).Verifiable();

            // Act
            IDisposable form = ajaxHelper.BeginRouteForm("namedroute", ajaxOptions);

            // Assert
            mockResponse.Verify();
        }

        [TestMethod]
        public void RouteFormAnonymousValues() {
            // Arrange
            Mock<HttpResponseBase> mockResponse = new Mock<HttpResponseBase>(MockBehavior.Strict);
            AjaxHelper ajaxHelper = GetAjaxHelper(mockResponse);
            AjaxOptions ajaxOptions = GetEmptyOptions();
            AjaxHelper poes = GetAjaxHelper();
            
            // Arrange expectations
            string x = ajaxHelper.RouteLink("a", "namedroute", GetEmptyOptions());
            string y = poes.RouteLink("a", "namedroute", null, ajaxOptions, new RouteValueDictionary());
            mockResponse.Expect(response => response.Write(AjaxRouteFormWithEmptyOptions)).Verifiable();

            // Act
            IDisposable form = ajaxHelper.BeginRouteForm("namedroute", null, ajaxOptions);

            // Assert
            mockResponse.Verify();
        }

        [TestMethod]
        public void RouteFormAnonymousValuesAndAttributes() {
            // Arrange
            Mock<HttpResponseBase> mockResponse = new Mock<HttpResponseBase>(MockBehavior.Strict);
            AjaxHelper ajaxHelper = GetAjaxHelper(mockResponse);
            AjaxOptions ajaxOptions = new AjaxOptions { UpdateTargetId = "some-id" };
            object htmlAttributes = new { method = "get" };

            // Arrange expectations
            mockResponse.Expect(response => response.Write(AjaxRouteFormWithHtmlAttributes)).Verifiable();

            // Act
            IDisposable form = ajaxHelper.BeginRouteForm("namedroute", null, ajaxOptions, htmlAttributes);

            // Assert
            mockResponse.Verify();
        }

        [TestMethod]
        public void RouteFormCanUseNamedRouteWithoutSpecifyingDefaults() {
            // DevDiv 217072: Non-mvc specific helpers should not give default values for controller and action

            // Arrange
            Mock<HttpResponseBase> mockResponse = new Mock<HttpResponseBase>(MockBehavior.Strict);
            AjaxHelper ajaxHelper = GetAjaxHelper(mockResponse);
            ajaxHelper.RouteCollection.MapRoute("MyRouteName", "any/url", new { controller = "Charlie" });

            // Arrange expectations
            mockResponse.Expect(response => response.Write(AjaxRouteFormWithNamedRouteNoDefaults)).Verifiable();

            // Act
            IDisposable form = ajaxHelper.BeginRouteForm("MyRouteName", new AjaxOptions());

            // Assert
            mockResponse.Verify();
        }

        [TestMethod]
        public void RouteFormTypedValues() {
            // Arrange
            Mock<HttpResponseBase> mockResponse = new Mock<HttpResponseBase>(MockBehavior.Strict);
            AjaxHelper ajaxHelper = GetAjaxHelper(mockResponse);
            AjaxOptions ajaxOptions = GetEmptyOptions();
            RouteValueDictionary values = new RouteValueDictionary();
            
            // Arrange expectations
            mockResponse.Expect(response => response.Write(AjaxRouteFormWithEmptyOptions)).Verifiable();

            // Act
            IDisposable form = ajaxHelper.BeginRouteForm("namedroute", values, ajaxOptions);

            // Assert
            mockResponse.Verify();
        }

        [TestMethod]
        public void RouteFormTypedValuesAndAttributes() {
            // Arrange
            Mock<HttpResponseBase> mockResponse = new Mock<HttpResponseBase>(MockBehavior.Strict);
            AjaxHelper ajaxHelper = GetAjaxHelper(mockResponse);
            Dictionary<string, object> htmlAttributes = new Dictionary<string, object> { { "method", "get" } };
            AjaxOptions ajaxOptions = new AjaxOptions { UpdateTargetId = "some-id" };
            RouteValueDictionary values = new RouteValueDictionary ();

            // Arrange expectations
            mockResponse.Expect(response => response.Write(AjaxRouteFormWithHtmlAttributes)).Verifiable();

            // Act
            IDisposable form = ajaxHelper.BeginRouteForm("namedroute", values, ajaxOptions, htmlAttributes);

            // Assert
            mockResponse.Verify();
        }

        private static AjaxHelper GetAjaxHelper() {
            return GetAjaxHelper(new Mock<HttpResponseBase>());
        }

        private static AjaxHelper GetAjaxHelper(Mock<HttpResponseBase> mockResponse) {
            HttpContextBase httpcontext = GetHttpContext("/app/", mockResponse);
            RouteCollection rt = new RouteCollection();
            rt.Add(new Route("{controller}/{action}/{id}", null) { Defaults = new RouteValueDictionary(new { id = "defaultid" }) });
            rt.Add("namedroute", new Route("named/{controller}/{action}/{id}", null) { Defaults = new RouteValueDictionary(new { id = "defaultid" }) });
            RouteData rd = new RouteData();
            rd.Values.Add("controller", "home");
            rd.Values.Add("action", "oldaction");

            Mock<ViewContext> mockViewContext = new Mock<ViewContext>();
            mockViewContext.Expect(c => c.HttpContext).Returns(httpcontext);
            mockViewContext.Expect(c => c.RouteData).Returns(rd);
            AjaxHelper ajaxHelper = new AjaxHelper(mockViewContext.Object, new Mock<IViewDataContainer>().Object, rt);
            return ajaxHelper;
        }

        private static AjaxOptions GetEmptyOptions() {
            return new AjaxOptions();
        }

        private static HttpContextBase GetHttpContext(string appPath, Mock<HttpResponseBase> mockResponse) {
            Mock<HttpContextBase> mockContext = new Mock<HttpContextBase>();
            Mock<HttpRequestBase> mockRequest = new Mock<HttpRequestBase>();
            if (!String.IsNullOrEmpty(appPath)) {
                mockRequest.Expect(o => o.ApplicationPath).Returns(appPath);
            }
            mockRequest.Expect(o => o.Url).Returns(new Uri("http://foo.bar.baz"));
            mockRequest.Expect(o => o.RawUrl).Returns("/rawUrl");
            mockRequest.Expect(o => o.PathInfo).Returns(String.Empty);
            mockContext.Expect(o => o.Request).Returns(mockRequest.Object);
            mockContext.Expect(o => o.Session).Returns((HttpSessionStateBase)null);

            mockResponse.Expect(o => o.ApplyAppPathModifier(It.IsAny<string>())).Returns<string>(r => HtmlHelperTest.AppPathModifier + r);
            mockContext.Expect(o => o.Response).Returns(mockResponse.Object);

            return mockContext.Object;
        }
    }
}
