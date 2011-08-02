namespace System.Web.Mvc.Test {
    using System;
    using System.Text;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Caching;
    using System.Web.TestUtil;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

    [TestClass]
    public class DefaultViewLocationCacheTest {

        [TestMethod]
        public void TimeSpanProperty() {
            // Arrange
            TimeSpan timeSpan = new TimeSpan(0, 20, 0);
            DefaultViewLocationCache viewCache = new DefaultViewLocationCache(timeSpan);

            // Assert
            Assert.AreEqual(timeSpan.Ticks, viewCache.TimeSpan.Ticks);
        }

        [TestMethod]
        public void ConstructorAssignsDefaultTimeSpan() {
            // Arrange
            DefaultViewLocationCache viewLocationCache = new DefaultViewLocationCache();
            TimeSpan timeSpan = new TimeSpan(0, 15, 0);

            // Assert
            Assert.AreEqual(timeSpan.Ticks, viewLocationCache.TimeSpan.Ticks);
        }

        [TestMethod]
        public void ConstructorWithNegativeTimeSpanThrows() {
            // Assert
            ExceptionHelper.ExpectInvalidOperationException(
                delegate {
                    new DefaultViewLocationCache(new TimeSpan(-1, 0, 0));
                },
                "The total number of ticks for the TimeSpan must be greater than 0.");
        }

        [TestMethod]
        public void GetViewLocationThrowsWithNullHttpContext() {
            // Arrange
            DefaultViewLocationCache viewLocationCache = new DefaultViewLocationCache();

            // Act & Assert
            ExceptionHelper.ExpectArgumentNullException(
                delegate {
                    string viewLocation = viewLocationCache.GetViewLocation(null /* httpContext */, "foo");
                },
                "httpContext");
        }

        [TestMethod]
        public void InsertViewLocationThrowsWithNullHttpContext() {
            // Arrange
            DefaultViewLocationCache viewLocationCache = new DefaultViewLocationCache();

            // Act & Assert
            ExceptionHelper.ExpectArgumentNullException(
                delegate {
                    viewLocationCache.InsertViewLocation(null /* httpContext */, "foo", "fooPath");
                },
                "httpContext");
        }

        [TestMethod]
        public void NullViewLocationCacheReturnsNullLocations() {
            // Act
            DefaultViewLocationCache.Null.InsertViewLocation(null /* httpContext */, "foo", "fooPath");

            // Assert
            Assert.AreEqual(null, DefaultViewLocationCache.Null.GetViewLocation(null /* httpContext */, "foo"));
        }        
    }
}
