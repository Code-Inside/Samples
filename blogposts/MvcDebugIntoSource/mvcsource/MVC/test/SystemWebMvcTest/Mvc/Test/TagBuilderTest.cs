namespace System.Web.Mvc.Test {
    using System;
    using System.Collections.Generic;
    using System.Web.Mvc;
    using System.Web.TestUtil;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class TagBuilderTest {

        [TestMethod]
        public void AddCssClassPrepends() {
            // Arrange
            TagBuilder builder = new TagBuilder("SomeTag");
            builder.MergeAttribute("class", "oldA");

            // Act
            builder.AddCssClass("newA");

            // Assert
            Assert.AreEqual("newA oldA", builder.Attributes["class"]);
        }

        [TestMethod]
        public void AttributesProperty() {
            // Arrange
            TagBuilder builder = new TagBuilder("SomeTag");

            // Act
            SortedDictionary<string, string> attributes = builder.Attributes as SortedDictionary<string, string>;

            // Assert
            Assert.IsNotNull(attributes, "Attributes was not a SortedDictionary<string, string>.");
            Assert.AreEqual(StringComparer.Ordinal, attributes.Comparer, "Attributes was not Ordinal case-sensitive.");
        }

        [TestMethod]
        public void ConstructorSetsTagNameProperty() {
            // Arrange
            TagBuilder builder = new TagBuilder("SomeTag");

            // Act
            string tagName = builder.TagName;

            // Assert
            Assert.AreEqual("SomeTag", tagName);
        }

        [TestMethod]
        public void ConstructorWithEmptyTagNameThrows() {
            ExceptionHelper.ExpectArgumentExceptionNullOrEmpty(
                delegate {
                    new TagBuilder(String.Empty);
                }, "tagName");
        }

        [TestMethod]
        public void ConstructorWithNullTagNameThrows() {
            ExceptionHelper.ExpectArgumentExceptionNullOrEmpty(
                delegate {
                    new TagBuilder(null /* tagName */);
                }, "tagName");
        }

        [TestMethod]
        public void InnerHtmlProperty() {
            // Arrange
            TagBuilder builder = new TagBuilder("SomeTag");

            // Act & Assert
            MemberHelper.TestStringProperty(builder, "InnerHtml", String.Empty, false /* testDefaultValue */, true /* allowNullAndEmpty */);
        }

        [TestMethod]
        public void MergeAttributeDoesNotOverwriteExistingValuesByDefault() {
            // Arrange
            TagBuilder builder = new TagBuilder("SomeTag");
            builder.MergeAttribute("a", "oldA");

            // Act
            builder.MergeAttribute("a", "newA");

            // Assert
            Assert.AreEqual("oldA", builder.Attributes["a"]);
        }

        [TestMethod]
        public void MergeAttributeOverwritesExistingValueIfAsked() {
            // Arrange
            TagBuilder builder = new TagBuilder("SomeTag");
            builder.MergeAttribute("a", "oldA");

            // Act
            builder.MergeAttribute("a", "newA", true);

            // Assert
            Assert.AreEqual("newA", builder.Attributes["a"]);
        }

        [TestMethod]
        public void MergeAttributeWithEmptyKeyThrows() {
            // Arrange
            TagBuilder builder = new TagBuilder("SomeTag");

            // Act & Assert
            ExceptionHelper.ExpectArgumentExceptionNullOrEmpty(
                delegate {
                    builder.MergeAttribute(String.Empty, "value");
                }, "key");
        }

        [TestMethod]
        public void MergeAttributeWithNullKeyThrows() {
            // Arrange
            TagBuilder builder = new TagBuilder("SomeTag");

            // Act & Assert
            ExceptionHelper.ExpectArgumentExceptionNullOrEmpty(
                delegate {
                    builder.MergeAttribute(null, "value");
                }, "key");
        }

        [TestMethod]
        public void MergeAttributesDoesNotOverwriteExistingValuesByDefault() {
            // Arrange
            TagBuilder builder = new TagBuilder("SomeTag");
            builder.Attributes["a"] = "oldA";

            Dictionary<string, string> newAttrs = new Dictionary<string, string> {
                { "a", "newA" },
                { "b", "newB" }
            };

            // Act
            builder.MergeAttributes(newAttrs);

            // Assert
            Assert.AreEqual(2, builder.Attributes.Count);
            Assert.AreEqual("oldA", builder.Attributes["a"]);
            Assert.AreEqual("newB", builder.Attributes["b"]);
        }

        [TestMethod]
        public void MergeAttributesOverwritesExistingValueIfAsked() {
            // Arrange
            TagBuilder builder = new TagBuilder("SomeTag");
            builder.Attributes["a"] = "oldA";

            Dictionary<string, string> newAttrs = new Dictionary<string, string> {
                { "a", "newA" },
                { "b", "newB" }
            };

            // Act
            builder.MergeAttributes(newAttrs, true);

            // Assert
            Assert.AreEqual(2, builder.Attributes.Count);
            Assert.AreEqual("newA", builder.Attributes["a"]);
            Assert.AreEqual("newB", builder.Attributes["b"]);
        }

        [TestMethod]
        public void MergeAttributesWithNullAttributesDoesNothing() {
            // Arrange
            TagBuilder builder = new TagBuilder("SomeTag");

            // Act
            builder.MergeAttributes<string, string>(null);

            // Assert
            Assert.AreEqual(0, builder.Attributes.Count);
        }

        [TestMethod]
        public void SetInnerTextEncodes() {
            // Arrange
            TagBuilder builder = new TagBuilder("SomeTag");

            // Act
            builder.SetInnerText("<>");

            // Assert
            Assert.AreEqual("&lt;&gt;", builder.InnerHtml);
        }

        [TestMethod]
        public void ToStringDefaultsToNormal() {
            // Arrange
            TagBuilder builder = new TagBuilder("SomeTag") {
                InnerHtml = "<x&y>"
            };
            builder.MergeAttributes(GetAttributesDictionary());

            // Act
            string output = builder.ToString();

            // Assert
            Assert.AreEqual(@"<SomeTag a=""Foo"" b=""Bar&amp;Baz"" c=""&lt;&quot;Quux&quot;>""><x&y></SomeTag>", output);
        }

        [TestMethod]
        public void ToStringEndTag() {
            // Arrange
            TagBuilder builder = new TagBuilder("SomeTag") {
                InnerHtml = "<x&y>"
            };
            builder.MergeAttributes(GetAttributesDictionary());

            // Act
            string output = builder.ToString(TagRenderMode.EndTag);

            // Assert
            Assert.AreEqual(@"</SomeTag>", output);
        }

        [TestMethod]
        public void ToStringNormal() {
            // Arrange
            TagBuilder builder = new TagBuilder("SomeTag") {
                InnerHtml = "<x&y>"
            };
            builder.MergeAttributes(GetAttributesDictionary());

            // Act
            string output = builder.ToString(TagRenderMode.Normal);

            // Assert
            Assert.AreEqual(@"<SomeTag a=""Foo"" b=""Bar&amp;Baz"" c=""&lt;&quot;Quux&quot;>""><x&y></SomeTag>", output);
        }

        [TestMethod]
        public void ToStringSelfClosing() {
            // Arrange
            TagBuilder builder = new TagBuilder("SomeTag") {
                InnerHtml = "<x&y>"
            };
            builder.MergeAttributes(GetAttributesDictionary());

            // Act
            string output = builder.ToString(TagRenderMode.SelfClosing);

            // Assert
            Assert.AreEqual(@"<SomeTag a=""Foo"" b=""Bar&amp;Baz"" c=""&lt;&quot;Quux&quot;>"" />", output);
        }

        [TestMethod]
        public void ToStringStartTag() {
            // Arrange
            TagBuilder builder = new TagBuilder("SomeTag") {
                InnerHtml = "<x&y>"
            };
            builder.MergeAttributes(GetAttributesDictionary());

            // Act
            string output = builder.ToString(TagRenderMode.StartTag);

            // Assert
            Assert.AreEqual(@"<SomeTag a=""Foo"" b=""Bar&amp;Baz"" c=""&lt;&quot;Quux&quot;>"">", output);
        }

        private static IDictionary<string, string> GetAttributesDictionary() {
            return new SortedDictionary<string, string> {
                { "a", "Foo" },
                { "b", "Bar&Baz" },
                { "c", @"<""Quux"">" }
            };
        }

    }
}
