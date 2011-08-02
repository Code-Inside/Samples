namespace System.Web.Mvc.Html.Test {
    using System.IO;
    using System.Web.Mvc;
    using System.Web.TestUtil;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

    [TestClass]
    public class RenderPartialExtensionsTest {
        const string _parentViewName = "parent-view";
        const string _partialViewName = "partial-view";

        [TestMethod]
        public void RenderPartialWithViewName() {
            // Arrange
            SpyHtmlHelper helper = SpyHtmlHelper.Create();
            Mock<IViewEngine> engine = new Mock<IViewEngine>(MockBehavior.Strict);
            Mock<IView> view = new Mock<IView>(MockBehavior.Strict);

            helper.ViewData["Foo"] = "Foo";
            helper.SpiedEngine = engine.Object;

            engine
                .Expect(e => e.FindPartialView(It.IsAny<ControllerContext>(), _partialViewName, It.IsAny<bool>()))
                .Returns(new ViewEngineResult(view.Object, engine.Object))
                .Verifiable();
            view
                .Expect(v => v.Render(It.IsAny<ViewContext>(), helper.ViewContext.HttpContext.Response.Output))
                .Callback<ViewContext, TextWriter>(
                    (viewContext, writer) => {
                        Assert.AreSame(helper.ViewContext.View, viewContext.View);
                        Assert.AreNotSame(helper.ViewData, viewContext.ViewData);
                        Assert.AreSame(helper.ViewContext.TempData, viewContext.TempData);
                        Assert.AreEqual("Foo", viewContext.ViewData["Foo"]);
                    })
                .Verifiable();

            // Act
            helper.RenderPartial(_partialViewName);

            // Assert
            engine.Verify();
            view.Verify();
        }

        [TestMethod]
        public void RenderPartialWithViewNameAndViewData() {
            // Arrange
            SpyHtmlHelper helper = SpyHtmlHelper.Create();
            ViewDataDictionary viewData = new ViewDataDictionary();
            Mock<IViewEngine> engine = new Mock<IViewEngine>(MockBehavior.Strict);
            Mock<IView> view = new Mock<IView>(MockBehavior.Strict);

            helper.SpiedEngine = engine.Object;
            helper.ViewData["Foo"] = "Foo";
            viewData["Bar"] = "Bar";

            engine
                .Expect(e => e.FindPartialView(It.IsAny<ControllerContext>(), _partialViewName, It.IsAny<bool>()))
                .Returns(new ViewEngineResult(view.Object, engine.Object))
                .Verifiable();
            view
                .Expect(v => v.Render(It.IsAny<ViewContext>(), helper.ViewContext.HttpContext.Response.Output))
                .Callback<ViewContext, TextWriter>(
                    (viewContext, writer) => {
                        Assert.AreSame(helper.ViewContext.View, viewContext.View);
                        Assert.AreNotSame(helper.ViewData, viewContext.ViewData);
                        Assert.AreSame(helper.ViewContext.TempData, viewContext.TempData);
                        Assert.AreEqual("Bar", viewContext.ViewData["Bar"]);
                        Assert.IsFalse(viewContext.ViewData.ContainsKey("Foo"));
                    })
                .Verifiable();

            // Act
            helper.RenderPartial(_partialViewName, viewData);

            // Assert
            engine.Verify();
            view.Verify();
        }

        [TestMethod]
        public void RenderPartialWithViewNameAndModel() {
            // Arrange
            SpyHtmlHelper helper = SpyHtmlHelper.Create();
            Mock<IViewEngine> engine = new Mock<IViewEngine>(MockBehavior.Strict);
            Mock<IView> view = new Mock<IView>(MockBehavior.Strict);

            helper.SpiedEngine = engine.Object;
            helper.ViewData["Foo"] = "Foo";
            object model = new object();

            engine
                .Expect(e => e.FindPartialView(It.IsAny<ControllerContext>(), _partialViewName, It.IsAny<bool>()))
                .Returns(new ViewEngineResult(view.Object, engine.Object))
                .Verifiable();
            view
                .Expect(v => v.Render(It.IsAny<ViewContext>(), helper.ViewContext.HttpContext.Response.Output))
                .Callback<ViewContext, TextWriter>(
                    (viewContext, writer) => {
                        Assert.AreSame(helper.ViewContext.View, viewContext.View);
                        Assert.AreNotSame(helper.ViewData, viewContext.ViewData);
                        Assert.AreSame(helper.ViewContext.TempData, viewContext.TempData);
                        Assert.IsTrue(viewContext.ViewData.ContainsKey("Foo"));
                        Assert.AreSame(model, viewContext.ViewData.Model);
                    })
                .Verifiable();

            // Act
            helper.RenderPartial(_partialViewName, model);

            // Assert
            engine.Verify();
            view.Verify();
        }

        [TestMethod]
        public void RenderPartialWithViewNameAndModelAndViewData() {
            // Arrange
            SpyHtmlHelper helper = SpyHtmlHelper.Create();
            ViewDataDictionary viewData = new ViewDataDictionary();
            Mock<IViewEngine> engine = new Mock<IViewEngine>(MockBehavior.Strict);
            Mock<IView> view = new Mock<IView>(MockBehavior.Strict);

            helper.SpiedEngine = engine.Object;
            helper.ViewData["Foo"] = "Foo";
            object model = new object();
            viewData.Model = new object();
            viewData["Bar"] = "Bar";

            engine
                .Expect(e => e.FindPartialView(It.IsAny<ControllerContext>(), _partialViewName, It.IsAny<bool>()))
                .Returns(new ViewEngineResult(view.Object, engine.Object))
                .Verifiable();
            view
                .Expect(v => v.Render(It.IsAny<ViewContext>(), helper.ViewContext.HttpContext.Response.Output))
                .Callback<ViewContext, TextWriter>(
                    (viewContext, writer) => {
                        Assert.AreSame(helper.ViewContext.View, viewContext.View);
                        Assert.AreNotSame(helper.ViewData, viewContext.ViewData);
                        Assert.AreSame(helper.ViewContext.TempData, viewContext.TempData);
                        Assert.IsFalse(viewContext.ViewData.ContainsKey("Foo"));
                        Assert.AreEqual("Bar", viewContext.ViewData["Bar"]);
                        Assert.AreSame(model, viewContext.ViewData.Model);
                    })
                .Verifiable();

            // Act
            helper.RenderPartial(_partialViewName, model, viewData);

            // Assert
            engine.Verify();
            view.Verify();
        }

        // RenderPartialInternal tests

        [TestMethod]
        public void NullPartialViewNameThrows() {
            // Arrange
            TestableHtmlHelper helper = TestableHtmlHelper.Create();
            ViewDataDictionary viewData = new ViewDataDictionary();

            // Act & Assert
            ExceptionHelper.ExpectArgumentExceptionNullOrEmpty(
                () => helper.RenderPartialInternal(null, viewData),
                "partialViewName");
        }

        [TestMethod]
        public void EmptyPartialViewNameThrows() {
            // Arrange
            TestableHtmlHelper helper = TestableHtmlHelper.Create();
            ViewDataDictionary viewData = new ViewDataDictionary();

            // Act & Assert
            ExceptionHelper.ExpectArgumentExceptionNullOrEmpty(
                () => helper.RenderPartialInternal(String.Empty, viewData),
                "partialViewName");
        }

        [TestMethod]
        public void EngineLookupSuccessCallsRender() {
            // Arrange
            TestableHtmlHelper helper = TestableHtmlHelper.Create();
            ViewDataDictionary viewData = new ViewDataDictionary();
            Mock<IViewEngine> engine = new Mock<IViewEngine>(MockBehavior.Strict);
            Mock<IView> view = new Mock<IView>(MockBehavior.Strict);
            engine
                .Expect(e => e.FindPartialView(It.IsAny<ControllerContext>(), _partialViewName, It.IsAny<bool>()))
                .Returns(new ViewEngineResult(view.Object, engine.Object))
                .Verifiable();
            view
                .Expect(v => v.Render(It.IsAny<ViewContext>(), helper.ViewContext.HttpContext.Response.Output))
                .Callback<ViewContext, TextWriter>(
                    (viewContext, writer) => {
                        Assert.AreSame(helper.ViewContext.View, viewContext.View);
                        Assert.AreNotSame(viewData, viewContext.ViewData);
                        Assert.AreSame(helper.ViewContext.TempData, viewContext.TempData);
                    })
                .Verifiable();

            // Act
            helper.RenderPartialInternal(_partialViewName, viewData, null, engine.Object);

            // Assert
            engine.Verify();
            view.Verify();
        }

        [TestMethod]
        public void EngineLookupFailureThrows() {
            // Arrange
            TestableHtmlHelper helper = TestableHtmlHelper.Create();
            ViewDataDictionary viewData = new ViewDataDictionary();
            Mock<IViewEngine> engine = new Mock<IViewEngine>(MockBehavior.Strict);
            engine
                .Expect(e => e.FindPartialView(It.IsAny<ControllerContext>(), _partialViewName, It.IsAny<bool>()))
                .Returns(new ViewEngineResult(new[] { "location1", "location2" }))
                .Verifiable();

            // Act & Assert
            ExceptionHelper.ExpectInvalidOperationException(
                () => helper.RenderPartialInternal(_partialViewName, viewData, null /* model */, engine.Object),
                "The partial view '" + _partialViewName + "' could not be found. The following locations were searched:\r\nlocation1\r\nlocation2");

            engine.Verify();
        }

        private class SpyHtmlHelper : HtmlHelper {
            public SpyHtmlHelper(ViewContext viewContext, IViewDataContainer viewDataContainer)
                : base(viewContext, viewDataContainer) { }

            public new ViewDataDictionary ViewData {
                get { return base.ViewData; }
            }

            public IViewEngine SpiedEngine {
                get;
                set;
            }

            public string SpiedPartialViewName {
                get;
                set;
            }

            public ViewDataDictionary SpiedViewData {
                get;
                set;
            }

            public static SpyHtmlHelper Create() {
                ViewDataDictionary viewData = new ViewDataDictionary();

                Mock<ViewContext> mockViewContext = new Mock<ViewContext>() { DefaultValue = DefaultValue.Mock };
                mockViewContext.Expect(c => c.HttpContext.Response.Output).Returns(TextWriter.Null);

                Mock<IViewDataContainer> container = new Mock<IViewDataContainer>();
                container.Expect(c => c.ViewData).Returns(viewData);

                return new SpyHtmlHelper(mockViewContext.Object, container.Object);
            }

            internal override void RenderPartialInternal(string partialViewName, ViewDataDictionary viewData,
                                                         object model, ViewEngineCollection viewEngineCollection) {
                base.RenderPartialInternal(partialViewName, viewData, model, new ViewEngineCollection(new IViewEngine[] { SpiedEngine }));
            }
        }

        private class TestableHtmlHelper : HtmlHelper {
            TestableHtmlHelper(ViewContext viewContext, IViewDataContainer viewDataContainer)
                : base(viewContext, viewDataContainer) { }

            public static TestableHtmlHelper Create() {
                ViewDataDictionary viewData = new ViewDataDictionary();

                Mock<ViewContext> mockViewContext = new Mock<ViewContext>() { DefaultValue = DefaultValue.Mock };
                mockViewContext.Expect(c => c.HttpContext.Response.Output).Returns(TextWriter.Null);
                mockViewContext.Expect(c => c.ViewData).Returns(viewData);

                Mock<IViewDataContainer> container = new Mock<IViewDataContainer>();
                container.Expect(c => c.ViewData).Returns(viewData);

                return new TestableHtmlHelper(mockViewContext.Object, container.Object);
            }

            public void RenderPartialInternal(string partialViewName,
                                              ViewDataDictionary viewData,
                                              params IViewEngine[] engines) {
                base.RenderPartialInternal(partialViewName, viewData, null, new ViewEngineCollection(engines));
            }
        }

    }
}
