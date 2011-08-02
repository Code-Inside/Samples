namespace System.Web.Mvc.Test {
    using System;
    using System.Collections.Specialized;
    using System.Web;
    using System.Web.Mvc;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

    [TestClass]
    public class PathHelpersTest {

        [TestMethod]
        public void GenerateClientUrlWithAbsoluteContentPathAndRewritingDisabled() {
            // Arrange
            Mock<HttpContextBase> mockHttpContext = GetMockHttpContext(false /* includeServerRewriterVar */);

            // Act
            string returnedUrl = PathHelpers.GenerateClientUrl(mockHttpContext.Object, "should remain unchanged");

            // Assert
            Assert.AreEqual("should remain unchanged", returnedUrl);
        }

        [TestMethod]
        public void GenerateClientUrlWithAbsoluteContentPathAndRewritingEnabled() {
            // Arrange
            Mock<HttpContextBase> mockHttpContext = GetMockHttpContext(true /* includeServerRewriterVar */);
            mockHttpContext.Expect(c => c.Request.RawUrl).Returns("/quux/foo/bar/baz");
            mockHttpContext.Expect(c => c.Request.Path).Returns("/myapp/foo/bar/baz");

            // Act
            string returnedUrl = PathHelpers.GenerateClientUrl(mockHttpContext.Object, "/myapp/some/absolute/path?alpha=bravo");

            // Assert
            Assert.AreEqual("/quux/some/absolute/path?alpha=bravo", returnedUrl);
        }

        [TestMethod]
        public void GenerateClientUrlWithAppRelativeContentPathAndRewritingDisabled() {
            // Arrange
            Mock<HttpContextBase> mockHttpContext = GetMockHttpContext(false /* includeServerRewriterVar */);

            // Act
            string returnedUrl = PathHelpers.GenerateClientUrl(mockHttpContext.Object, "~/foo/bar?alpha=bravo");

            // Assert
            Assert.AreEqual("/myapp/(S(session))/foo/bar?alpha=bravo", returnedUrl);
        }

        [TestMethod]
        public void GenerateClientUrlWithAppRelativeContentPathAndRewritingEnabled() {
            // Arrange
            Mock<HttpContextBase> mockHttpContext = GetMockHttpContext(true /* includeServerRewriterVar */);
            mockHttpContext.Expect(c => c.Request.RawUrl).Returns("/quux/foo/baz");
            mockHttpContext.Expect(c => c.Request.Path).Returns("/myapp/foo/baz");

            // Act
            string returnedUrl = PathHelpers.GenerateClientUrl(mockHttpContext.Object, "~/foo/bar?alpha=bravo");

            // Assert
            Assert.AreEqual("/quux/foo/bar?alpha=bravo", returnedUrl);
        }

        [TestMethod]
        public void GenerateClientUrlWithEmptyContentPathReturnsEmptyString() {
            // Act
            string returnedUrl = PathHelpers.GenerateClientUrl(null, "");

            // Assert
            Assert.AreEqual("", returnedUrl);
        }

        [TestMethod]
        public void GenerateClientUrlWithNullContentPathReturnsNull() {
            // Act
            string returnedUrl = PathHelpers.GenerateClientUrl(null, null);

            // Assert
            Assert.IsNull(returnedUrl);
        }

        [TestMethod]
        public void GenerateClientUrlWithOnlyQueryStringForContentPathReturnsOriginalContentPath() {
            // Act
            string returnedUrl = PathHelpers.GenerateClientUrl(null, "?foo=bar");

            // Assert
            Assert.AreEqual("?foo=bar", returnedUrl);
        }

        [TestMethod]
        public void MakeAbsoluteFromDirectoryToParent() {
            // Act
            string returnedUrl = PathHelpers.MakeAbsolute("/Account/Register", "../Account");

            // Assert
            Assert.AreEqual("/Account", returnedUrl);
        }

        [TestMethod]
        public void MakeAbsoluteFromDirectoryToSelf() {
            // Act
            string returnedUrl = PathHelpers.MakeAbsolute("/foo/", "./");

            // Assert
            Assert.AreEqual("/foo/", returnedUrl);
        }

        [TestMethod]
        public void MakeAbsoluteFromFileToFile() {
            // Act
            string returnedUrl = PathHelpers.MakeAbsolute("/foo", "bar");

            // Assert
            Assert.AreEqual("/bar", returnedUrl);
        }

        [TestMethod]
        public void MakeAbsoluteFromFileWithQueryToFile() {
            // Act
            string returnedUrl = PathHelpers.MakeAbsolute("/foo/bar?alpha=bravo", "baz");

            // Assert
            Assert.AreEqual("/foo/baz", returnedUrl);
        }

        [TestMethod]
        public void MakeAbsoluteFromRootToSelf() {
            // Act
            string returnedUrl = PathHelpers.MakeAbsolute("/", "./");

            // Assert
            Assert.AreEqual("/", returnedUrl);
        }

        [TestMethod]
        public void MakeRelativeFromFileToDirectory() {
            // Act
            string returnedUrl = PathHelpers.MakeRelative("/foo/bar", "/foo/");

            // Assert
            Assert.AreEqual("./", returnedUrl);
        }

        [TestMethod]
        public void MakeRelativeFromFileToDirectoryWithQueryString() {
            // Act
            string returnedUrl = PathHelpers.MakeRelative("/foo/bar", "/foo/?alpha=bravo");

            // Assert
            Assert.AreEqual("./?alpha=bravo", returnedUrl);
        }

        [TestMethod]
        public void MakeRelativeFromFileToFile() {
            // Act
            string returnedUrl = PathHelpers.MakeRelative("/foo/bar", "/baz/quux");

            // Assert
            Assert.AreEqual("../baz/quux", returnedUrl);
        }

        [TestMethod]
        public void MakeRelativeFromFileToFileWithQuery() {
            // Act
            string returnedUrl = PathHelpers.MakeRelative("/foo/bar", "/baz/quux?alpha=bravo");

            // Assert
            Assert.AreEqual("../baz/quux?alpha=bravo", returnedUrl);
        }

        [TestMethod]
        public void MakeRelativeFromFileWithQueryToFileWithQuery() {
            // Act
            string returnedUrl = PathHelpers.MakeRelative("/foo/bar?charlie=delta", "/baz/quux?alpha=bravo");

            // Assert
            Assert.AreEqual("../baz/quux?alpha=bravo", returnedUrl);
        }

        [TestMethod]
        public void MakeRelativeFromRootToRoot() {
            // Act
            string returnedUrl = PathHelpers.MakeRelative("/", "/");

            // Assert
            Assert.AreEqual("./", returnedUrl);
        }

        [TestMethod]
        public void MakeRelativeFromRootToRootWithQueryString() {
            // Act
            string returnedUrl = PathHelpers.MakeRelative("/", "/?foo=bar");

            // Assert
            Assert.AreEqual("./?foo=bar", returnedUrl);
        }

        private static Mock<HttpContextBase> GetMockHttpContext(bool includeRewriterServerVar) {
            Mock<HttpContextBase> mockContext = new Mock<HttpContextBase>();

            NameValueCollection serverVars = new NameValueCollection();
            mockContext.Expect(c => c.Request.ServerVariables).Returns(serverVars);
            mockContext.Expect(c => c.Request.ApplicationPath).Returns("/myapp");

            if (includeRewriterServerVar) {
                serverVars["HTTP_X_ORIGINAL_URL"] = "I exist!";
                mockContext
                    .Expect(c => c.Response.ApplyAppPathModifier(It.IsAny<string>()))
                    .Returns(
                        delegate(string input) {
                            return input;
                        });
            }
            else {
                mockContext
                    .Expect(c => c.Response.ApplyAppPathModifier(It.IsAny<string>()))
                    .Returns(
                        delegate(string input) {
                            return "/myapp/(S(session))" + input.Substring("/myapp".Length);
                        });
            }

            return mockContext;
        }

    }
}
