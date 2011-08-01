namespace System.Web.Mvc.Test {
    using System.Collections.Generic;
    using System.Collections;
    using System.Web.Routing;
    using System.Web.TestUtil;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;
    
    [TestClass]
    public class SessionStateTempDataProviderTest {

        [TestMethod]
        public void SessionProviderLoadThrowsOnDisabledSessionState() {
            // Arrange
            SessionStateTempDataProvider testProvider = new SessionStateTempDataProvider();

            // Act & Assert
            ExceptionHelper.ExpectInvalidOperationException(
                delegate { 
                    IDictionary<string, object> tempDataDictionary = testProvider.LoadTempData(GetControllerContext()); 
                },
                "The SessionStateTempDataProvider requires SessionState to be enabled.");
        }

        [TestMethod]
        public void SessionProviderSaveThrowsOnDisabledSessionState() {
            // Arrange
            SessionStateTempDataProvider testProvider = new SessionStateTempDataProvider();

            // Act & Assert
            ExceptionHelper.ExpectInvalidOperationException(
                delegate {
                    testProvider.SaveTempData(GetControllerContext(), new Dictionary<string, object>());
                },
                "The SessionStateTempDataProvider requires SessionState to be enabled.");
        }

        private static ControllerContext GetControllerContext() {
            Mock<ControllerContext> mockControllerContext = new Mock<ControllerContext>();
            mockControllerContext.Expect(c => c.HttpContext.Session).Returns((HttpSessionStateBase)null);
            return mockControllerContext.Object;
        }
    }
}
