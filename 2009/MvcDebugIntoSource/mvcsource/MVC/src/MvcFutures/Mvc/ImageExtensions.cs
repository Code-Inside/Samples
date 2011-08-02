namespace Microsoft.Web.Mvc {
    using System;
    using System.Collections.Generic;
    using System.Web.Mvc;
    using System.Web.Routing;
    using Microsoft.Web.Resources;

    public static class ImageExtensions {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1054:UriParametersShouldNotBeStrings", MessageId = "1#", Justification = "The return value is not a regular URL since it may contain ~/ ASP.NET-specific characters")]
        public static string Image(this HtmlHelper helper, string imageRelativeUrl) {
            return Image(helper, imageRelativeUrl, null, (IDictionary<string, object>)null);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1054:UriParametersShouldNotBeStrings", MessageId = "1#", Justification = "Required for Extension Method")]
        public static string Image(this HtmlHelper helper, string imageRelativeUrl, string alt) {
            return Image(helper, imageRelativeUrl, alt, (IDictionary<string, object>)null);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1054:UriParametersShouldNotBeStrings", MessageId = "1#", Justification = "The return value is not a regular URL since it may contain ~/ ASP.NET-specific characters")]
        public static string Image(this HtmlHelper helper, string imageRelativeUrl, string alt, object htmlAttributes) {
            return Image(helper, imageRelativeUrl, alt, new RouteValueDictionary(htmlAttributes));
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1054:UriParametersShouldNotBeStrings", MessageId = "1#", Justification = "The return value is not a regular URL since it may contain ~/ ASP.NET-specific characters")]
        public static string Image(this HtmlHelper helper, string imageRelativeUrl, object htmlAttributes) {
            return Image(helper, imageRelativeUrl, null, new RouteValueDictionary(htmlAttributes));
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1054:UriParametersShouldNotBeStrings", MessageId = "1#", Justification = "The return value is not a regular URL since it may contain ~/ ASP.NET-specific characters")]
        public static string Image(this HtmlHelper helper, string imageRelativeUrl, IDictionary<string, object> htmlAttributes) {
            return Image(helper, imageRelativeUrl, null, htmlAttributes);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1054:UriParametersShouldNotBeStrings", MessageId = "1#", Justification = "The return value is not a regular URL since it may contain ~/ ASP.NET-specific characters")]
        public static string Image(this HtmlHelper helper, string imageRelativeUrl, string alt, IDictionary<string, object> htmlAttributes) {
            if (String.IsNullOrEmpty(imageRelativeUrl)) {
                throw new ArgumentException(MvcResources.Common_NullOrEmpty, "imageRelativeUrl");
            }

            UrlHelper url = new UrlHelper(helper.ViewContext.RequestContext);
            string imageUrl = url.Content(imageRelativeUrl);
            return Image(imageUrl, alt, htmlAttributes).ToString(TagRenderMode.SelfClosing);
        }

        public static TagBuilder Image(string imageUrl, string alt, IDictionary<string, object> htmlAttributes) {
            if (String.IsNullOrEmpty(imageUrl)) {
                throw new ArgumentException(MvcResources.Common_NullOrEmpty, "imageUrl");
            }

            TagBuilder imageTag = new TagBuilder("img");

            if (!String.IsNullOrEmpty(imageUrl)) {
                imageTag.MergeAttribute("src", imageUrl);
            }

            if (!String.IsNullOrEmpty(alt)) {
                imageTag.MergeAttribute("alt", alt);
            }

            imageTag.MergeAttributes(htmlAttributes, true);

            if (imageTag.Attributes.ContainsKey("alt") && !imageTag.Attributes.ContainsKey("title")) {
                imageTag.MergeAttribute("title", (imageTag.Attributes["alt"] ?? "").ToString());
            }
            return imageTag;
        }
    }
}
