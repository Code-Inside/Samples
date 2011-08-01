namespace Microsoft.Web.Mvc.Test {
    using System.Collections.Generic;
    using System.Globalization;
    using System.Web.Mvc;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.Web.Mvc;
    using Moq;

    [TestClass]
    public class ExpressionInputExtensionsTest {
        [TestMethod]
        public void TextboxForWithExpressionRendersInputTagUsingExpression() {
            // Arrange
            Mock<ViewContext> mockViewContext = new Mock<ViewContext>();
            ViewDataDictionary viewData = new ViewDataDictionary(new Product { ProductName = "ASP.NET MVC" });
            Mock<IViewDataContainer> mockIViewDataContainer = new Mock<IViewDataContainer>();
            mockIViewDataContainer.Expect(c => c.ViewData).Returns(viewData);
            HtmlHelper<Product> htmlHelper = new HtmlHelper<Product>(mockViewContext.Object, mockIViewDataContainer.Object);

            // Act
            string result = htmlHelper.TextBoxFor(p => p.ProductName);

            // Assert
            Assert.AreEqual("<input id=\"ProductName\" name=\"ProductName\" type=\"text\" value=\"ASP.NET MVC\" />", result);
        }

        [TestMethod]
        public void TextboxForWithExpressionRendersInputTagUsingExpressionUsingValueFromModelState() {
            // Arrange
            Mock<ViewContext> mockViewContext = new Mock<ViewContext>();
            ViewDataDictionary viewData = new ViewDataDictionary(new Product { ProductName = "ASP.NET MVC" });
            viewData.ModelState.SetModelValue("ProductName", new ValueProviderResult("Something Else", "Something Else", CultureInfo.InvariantCulture));
            Mock<IViewDataContainer> mockIViewDataContainer = new Mock<IViewDataContainer>();
            mockIViewDataContainer.Expect(c => c.ViewData).Returns(viewData);
            HtmlHelper<Product> htmlHelper = new HtmlHelper<Product>(mockViewContext.Object, mockIViewDataContainer.Object);

            // Act
            string result = htmlHelper.TextBoxFor(p => p.ProductName);

            // Assert
            Assert.AreEqual("<input id=\"ProductName\" name=\"ProductName\" type=\"text\" value=\"Something Else\" />", result);
        }

        [TestMethod]
        public void TextboxForWithExpressionRendersInputTagUsingExpressionUsingValueFromViewState() {
            // Arrange
            Mock<ViewContext> mockViewContext = new Mock<ViewContext>();
            ViewDataDictionary viewData = new ViewDataDictionary();
            Product productInViewData = new Product { ProductName = "Something Else" };
            viewData["ProductName"] = "Not Something Else";
            Mock<IViewDataContainer> mockIViewDataContainer = new Mock<IViewDataContainer>();
            mockIViewDataContainer.Expect(c => c.ViewData).Returns(viewData);
            HtmlHelper<Product> htmlHelper = new HtmlHelper<Product>(mockViewContext.Object, mockIViewDataContainer.Object);

            // Act
            string result = htmlHelper.TextBoxFor(p => p.ProductName);

            // Assert
            Assert.AreEqual("<input id=\"ProductName\" name=\"ProductName\" type=\"text\" value=\"Not Something Else\" />", result);
        }

        [TestMethod]
        public void TextboxForWithExpressionRendersInputTagUsingExpressionWithIntProperty() {
            // Arrange
            Mock<ViewContext> mockViewContext = new Mock<ViewContext>();
            ViewDataDictionary viewData = new ViewDataDictionary(new Product { Id = 123 });
            Mock<IViewDataContainer> mockIViewDataContainer = new Mock<IViewDataContainer>();
            mockIViewDataContainer.Expect(c => c.ViewData).Returns(viewData);
            HtmlHelper<Product> htmlHelper = new HtmlHelper<Product>(mockViewContext.Object, mockIViewDataContainer.Object);

            // Act
            string result = htmlHelper.TextBoxFor(p => p.Id);

            // Assert
            Assert.AreEqual("<input id=\"Id\" name=\"Id\" type=\"text\" value=\"123\" />", result);
        }

        [TestMethod]
        public void TextboxForWithExpressionContainingMethodCallRendersInputTagUsingExpressionProperty() {
            // Arrange
            Mock<ViewContext> mockViewContext = new Mock<ViewContext>();
            ViewDataDictionary viewData = new ViewDataDictionary(new Product { Id = 123 });
            Mock<IViewDataContainer> mockIViewDataContainer = new Mock<IViewDataContainer>();
            mockIViewDataContainer.Expect(c => c.ViewData).Returns(viewData);
            HtmlHelper<Product> htmlHelper = new HtmlHelper<Product>(mockViewContext.Object, mockIViewDataContainer.Object);

            // Act
            string result = htmlHelper.TextBoxFor(p => p.Id.ToString());

            // Assert
            Assert.AreEqual("<input id=\"Id\" name=\"Id\" type=\"text\" value=\"123\" />", result);
        }

        [TestMethod]
        public void TextboxForWithAttributesAndExpressionRendersAttributes() {
            // Arrange
            Mock<ViewContext> mockViewContext = new Mock<ViewContext>();
            ViewDataDictionary viewData = new ViewDataDictionary(new Product { ProductName = "ASP.NET MVC" });
            Mock<IViewDataContainer> mockIViewDataContainer = new Mock<IViewDataContainer>();
            mockIViewDataContainer.Expect(c => c.ViewData).Returns(viewData);
            HtmlHelper<Product> htmlHelper = new HtmlHelper<Product>(mockViewContext.Object, mockIViewDataContainer.Object);

            // Act
            string result = htmlHelper.TextBoxFor(p => p.ProductName, new { width = 123, maxlength = 99 });

            // Assert
            Assert.AreEqual("<input id=\"ProductName\" maxlength=\"99\" name=\"ProductName\" type=\"text\" value=\"ASP.NET MVC\" width=\"123\" />", result);
        }

        [TestMethod]
        public void HiddenForWithExpressionRendersInputTagUsingExpression() {
            // Arrange
            Mock<ViewContext> mockViewContext = new Mock<ViewContext>();
            ViewDataDictionary viewData = new ViewDataDictionary(new Product { ProductName = "ASP.NET MVC" });
            Mock<IViewDataContainer> mockIViewDataContainer = new Mock<IViewDataContainer>();
            mockIViewDataContainer.Expect(c => c.ViewData).Returns(viewData);
            HtmlHelper<Product> htmlHelper = new HtmlHelper<Product>(mockViewContext.Object, mockIViewDataContainer.Object);

            // Act
            string result = htmlHelper.HiddenFor(p => p.ProductName);

            // Assert
            Assert.AreEqual("<input id=\"ProductName\" name=\"ProductName\" type=\"hidden\" value=\"ASP.NET MVC\" />", result);
        }

        [TestMethod]
        public void HiddenForWithExpressionRendersInputTagUsingExpressionUsingValueFromModelState() {
            // Arrange
            Mock<ViewContext> mockViewContext = new Mock<ViewContext>();
            ViewDataDictionary viewData = new ViewDataDictionary(new Product { ProductName = "ASP.NET MVC" });
            viewData.ModelState.SetModelValue("ProductName", new ValueProviderResult("Something Else", "Something Else", CultureInfo.InvariantCulture));
            Mock<IViewDataContainer> mockIViewDataContainer = new Mock<IViewDataContainer>();
            mockIViewDataContainer.Expect(c => c.ViewData).Returns(viewData);
            HtmlHelper<Product> htmlHelper = new HtmlHelper<Product>(mockViewContext.Object, mockIViewDataContainer.Object);

            // Act
            string result = htmlHelper.HiddenFor(p => p.ProductName);

            // Assert
            Assert.AreEqual("<input id=\"ProductName\" name=\"ProductName\" type=\"hidden\" value=\"Something Else\" />", result);
        }


        [TestMethod]
        public void HiddenForWithAttributesAndExpressionRendersAttributes() {
            // Arrange
            Mock<ViewContext> mockViewContext = new Mock<ViewContext>();
            ViewDataDictionary viewData = new ViewDataDictionary(new Product { ProductName = "ASP.NET MVC" });
            Mock<IViewDataContainer> mockIViewDataContainer = new Mock<IViewDataContainer>();
            mockIViewDataContainer.Expect(c => c.ViewData).Returns(viewData);
            HtmlHelper<Product> htmlHelper = new HtmlHelper<Product>(mockViewContext.Object, mockIViewDataContainer.Object);

            // Act
            string result = htmlHelper.HiddenFor(p => p.ProductName, new { @class = "test-css-class" });

            // Assert
            Assert.AreEqual("<input class=\"test-css-class\" id=\"ProductName\" name=\"ProductName\" type=\"hidden\" value=\"ASP.NET MVC\" />", result);
        }

        [TestMethod]
        public void DropDownListSelectListItemWithSelectedTrueSelectedAttribute() {
            // Arrange
            Mock<ViewContext> mockViewContext = new Mock<ViewContext>();
            ViewDataDictionary viewData = new ViewDataDictionary(new Product { ProductName = null });
            Mock<IViewDataContainer> mockIViewDataContainer = new Mock<IViewDataContainer>();
            mockIViewDataContainer.Expect(c => c.ViewData).Returns(viewData);
            HtmlHelper<Product> htmlHelper = new HtmlHelper<Product>(mockViewContext.Object, mockIViewDataContainer.Object);
            IList<SelectListItem> selectList = new List<SelectListItem>();
            selectList.Add(new SelectListItem { Text = "option 1", Value = "option_1", Selected=true });
            selectList.Add(new SelectListItem { Text = "option 2", Value = "option_2", Selected=false });

            // Act
            string result = htmlHelper.DropDownListFor(p => p.ProductName, selectList);

            // Assert
            Assert.AreEqual(@"<select id=""ProductName"" name=""ProductName""><option selected=""selected"" value=""option_1"">option 1</option>
<option value=""option_2"">option 2</option>
</select>", result);
        }

        [TestMethod]
        public void DropDownListWithOptionLabelAndSelectedIdInModelStateRendersSelectedOption() {
            // Arrange
            Mock<ViewContext> mockViewContext = new Mock<ViewContext>();
            ViewDataDictionary viewData = new ViewDataDictionary(new Product { CategoryId = 2 });
            Mock<IViewDataContainer> mockIViewDataContainer = new Mock<IViewDataContainer>();
            mockIViewDataContainer.Expect(c => c.ViewData).Returns(viewData);
            HtmlHelper<Product> htmlHelper = new HtmlHelper<Product>(mockViewContext.Object, mockIViewDataContainer.Object);
            IList<SelectListItem> selectList = new List<SelectListItem>();
            selectList.Add(new SelectListItem { Text = "option 1", Value = "1"});
            selectList.Add(new SelectListItem { Text = "option 2", Value = "2"});

            // Act
            string result = htmlHelper.DropDownListFor(p => p.CategoryId, selectList, "Select One");

            // Assert
            Assert.AreEqual(@"<select id=""CategoryId"" name=""CategoryId""><option value="""">Select One</option>
<option value=""1"">option 1</option>
<option selected=""selected"" value=""2"">option 2</option>
</select>", result);
        }

        [TestMethod]
        public void DropDownListForWithAttributesAndExpressionRendersOptionsAndAttributes() {
            // Arrange
            Mock<ViewContext> mockViewContext = new Mock<ViewContext>();
            ViewDataDictionary viewData = new ViewDataDictionary(new Product { CategoryId = 123 });
            Mock<IViewDataContainer> mockIViewDataContainer = new Mock<IViewDataContainer>();
            mockIViewDataContainer.Expect(c => c.ViewData).Returns(viewData);
            HtmlHelper<Product> htmlHelper = new HtmlHelper<Product>(mockViewContext.Object, mockIViewDataContainer.Object);
            IList<SelectListItem> selectList = new List<SelectListItem>();
            selectList.Add(new SelectListItem { Text = "option 1", Value= "1", Selected= false });
            selectList.Add(new SelectListItem { Text = "option 2", Value = "2", Selected = false });

            // Act
            string result = htmlHelper.DropDownListFor(p => p.CategoryId, selectList, new { @class = "test-css-class" });

            // Assert
            Assert.AreEqual(@"<select class=""test-css-class"" id=""CategoryId"" name=""CategoryId""><option value=""1"">option 1</option>
<option value=""2"">option 2</option>
</select>", result);
        }

        [TestMethod]
        public void ValidationMessageForWithNoErrorReturnsNull() {
            // Arrange
            Mock<ViewContext> mockViewContext = new Mock<ViewContext>();
            ViewDataDictionary viewData = new ViewDataDictionary(new Product());
            Mock<IViewDataContainer> mockIViewDataContainer = new Mock<IViewDataContainer>();
            mockIViewDataContainer.Expect(c => c.ViewData).Returns(viewData);
            HtmlHelper<Product> htmlHelper = new HtmlHelper<Product>(mockViewContext.Object, mockIViewDataContainer.Object);

            // Act
            string html = htmlHelper.ValidationMessageFor(p => p.ProductName);

            // Assert
            Assert.IsNull(html);
        }

        [TestMethod]
        public void ValidationMessageForDisplaysErrorMessageInModelState() {
            // Arrange
            Mock<ViewContext> mockViewContext = new Mock<ViewContext>();
            ViewDataDictionary viewData = new ViewDataDictionary(new Product());
            viewData.ModelState.AddModelError("ProductName", "Error Will Robinson!");
            Mock<IViewDataContainer> mockIViewDataContainer = new Mock<IViewDataContainer>();
            mockIViewDataContainer.Expect(c => c.ViewData).Returns(viewData);
            HtmlHelper<Product> htmlHelper = new HtmlHelper<Product>(mockViewContext.Object, mockIViewDataContainer.Object);

            // Act
            string html = htmlHelper.ValidationMessageFor(p => p.ProductName);

            // Assert
            Assert.AreEqual(@"<span class=""field-validation-error"">Error Will Robinson!</span>", html);
        }

        [TestMethod]
        public void ValidationMessageForDisplaysSpecifiedErrorMessage() {
            // Arrange
            Mock<ViewContext> mockViewContext = new Mock<ViewContext>();
            ViewDataDictionary viewData = new ViewDataDictionary(new Product());
            viewData.ModelState.AddModelError("ProductName", "Error");
            Mock<IViewDataContainer> mockIViewDataContainer = new Mock<IViewDataContainer>();
            mockIViewDataContainer.Expect(c => c.ViewData).Returns(viewData);
            HtmlHelper<Product> htmlHelper = new HtmlHelper<Product>(mockViewContext.Object, mockIViewDataContainer.Object);

            // Act
            string html = htmlHelper.ValidationMessageFor(p => p.ProductName, "Error Will Robinson!");

            // Assert
            Assert.AreEqual(@"<span class=""field-validation-error"">Error Will Robinson!</span>", html);
        }

        [TestMethod]
        public void ValidationMessageForWithAttributesRendersAttribute() {
            // Arrange
            Mock<ViewContext> mockViewContext = new Mock<ViewContext>();
            ViewDataDictionary viewData = new ViewDataDictionary(new Product());
            viewData.ModelState.AddModelError("ProductName", "Error");
            Mock<IViewDataContainer> mockIViewDataContainer = new Mock<IViewDataContainer>();
            mockIViewDataContainer.Expect(c => c.ViewData).Returns(viewData);
            HtmlHelper<Product> htmlHelper = new HtmlHelper<Product>(mockViewContext.Object, mockIViewDataContainer.Object);

            // Act
            string html = htmlHelper.ValidationMessageFor(p => p.ProductName, "Error Will Robinson!", new { title = "Error!"});

            // Assert
            Assert.AreEqual(@"<span class=""field-validation-error"" title=""Error!"">Error Will Robinson!</span>", html);
        }


        internal class Product {
            public string ProductName {
                get;
                set;
            }

            public int Id {
                get;
                set;
            }

            public int CategoryId { 
                get; 
                set; 
            }
        }
    }
}
