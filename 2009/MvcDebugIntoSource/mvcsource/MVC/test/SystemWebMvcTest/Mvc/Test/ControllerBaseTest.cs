namespace System.Web.Mvc.Test {
    using System;
    using System.Collections.Generic;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Routing;
    using System.Web.TestUtil;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

    [TestClass]
    public class ControllerBaseTest {

        [TestMethod]
        public void ExecuteCallsControllerBaseExecute() {
            // Arrange
            RequestContext requestContext = new RequestContext(new Mock<HttpContextBase>().Object, new RouteData());

            Mock<ControllerBaseHelper> mockController = new Mock<ControllerBaseHelper>() { CallBase = true };
            mockController.Expect(c => c.PublicInitialize(requestContext)).Verifiable();
            mockController.Expect(c => c.PublicExecuteCore()).Verifiable();
            IController controller = mockController.Object;

            // Act
            controller.Execute(requestContext);

            // Assert
            mockController.Verify();
        }

        [TestMethod]
        public void ExecuteThrowsIfRequestContextIsNull() {
            // Arrange
            IController controller = new ControllerBaseHelper();

            // Act & Assert
            ExceptionHelper.ExpectArgumentNullException(
                delegate {
                    controller.Execute(null);
                }, "requestContext");
        }

        [TestMethod]
        public void InitializeSetsControllerContext() {
            // Arrange
            ControllerBaseHelper helper = new ControllerBaseHelper();
            RequestContext requestContext = new RequestContext(new Mock<HttpContextBase>().Object, new RouteData());

            // Act
            helper.PublicInitialize(requestContext);

            // Assert
            Assert.AreSame(requestContext.HttpContext, helper.ControllerContext.HttpContext);
            Assert.AreSame(requestContext.RouteData, helper.ControllerContext.RouteData);
            Assert.AreSame(helper, helper.ControllerContext.Controller);
        }

        [TestMethod]
        public void TempDataProperty() {
            // Arrange
            ControllerBase controller = new ControllerBaseHelper();

            // Act & Assert
            MemberHelper.TestPropertyWithDefaultInstance(controller, "TempData", new TempDataDictionary());
        }

        [TestMethod]
        public void ValidateRequestProperty() {
            // Arrange
            ControllerBase controller = new ControllerBaseHelper();

            // Act & assert
            MemberHelper.TestBooleanProperty(controller, "ValidateRequest", true /* initialValue */, false /* testDefaultValue */);
        }

        [TestMethod]
        public void ValueProviderProperty() {
            // Arrange
            ControllerBase controller = new ControllerBaseHelper();
            Dictionary<string, ValueProviderResult> valueProvider = new Dictionary<string, ValueProviderResult>();

            // Act & assert
            MemberHelper.TestPropertyWithDefaultInstance(controller, "ValueProvider", valueProvider);
        }

        [TestMethod]
        public void ViewDataProperty() {
            // Arrange
            ControllerBase controller = new ControllerBaseHelper();

            // Act & Assert
            MemberHelper.TestPropertyWithDefaultInstance(controller, "ViewData", new ViewDataDictionary());
        }

        public class ControllerBaseHelper : ControllerBase {
            protected override void Initialize(RequestContext requestContext) {
                PublicInitialize(requestContext);
            }
            public virtual void PublicInitialize(RequestContext requestContext) {
                base.Initialize(requestContext);
            }
            protected override void ExecuteCore() {
                PublicExecuteCore();
            }
            public virtual void PublicExecuteCore() {
                throw new NotImplementedException();
            }
        }

    }
}
