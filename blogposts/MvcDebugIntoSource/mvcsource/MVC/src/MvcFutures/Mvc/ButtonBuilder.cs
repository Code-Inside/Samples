namespace Microsoft.Web.Mvc {
    using System;
    using System.Collections.Generic;
    using System.Web.Mvc;

    public static class ButtonBuilder {
        public static TagBuilder SubmitButton(string name, string buttonText, IDictionary<string, object> htmlAttributes) {
            TagBuilder buttonTag = new TagBuilder("input");
            
            buttonTag.MergeAttribute("type", "submit");
            if (!buttonTag.Attributes.ContainsKey("id") && name != null) {
                buttonTag.GenerateId(name);
            }

            if (!String.IsNullOrEmpty(name)) {
                buttonTag.MergeAttribute("name", name);
            }

            if (!String.IsNullOrEmpty(buttonText)) {
                buttonTag.MergeAttribute("value", buttonText);
            }
            
            buttonTag.MergeAttributes(htmlAttributes, true);
            return buttonTag;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1054:UriParametersShouldNotBeStrings", MessageId = "1#", Justification = "The return value is not a regular URL since it may contain ~/ ASP.NET-specific characters")]
        public static TagBuilder SubmitImage(string name, string sourceUrl, IDictionary<string, object> htmlAttributes) {
            TagBuilder buttonTag = new TagBuilder("input");

            buttonTag.MergeAttribute("type", "image");
            
            if (!buttonTag.Attributes.ContainsKey("id")) {
                buttonTag.GenerateId(name);
            }

            if (!String.IsNullOrEmpty(name)) {
                buttonTag.MergeAttribute("name", name);
            }

            if (!String.IsNullOrEmpty(sourceUrl)) {
                buttonTag.MergeAttribute("src", sourceUrl);
            }
            buttonTag.MergeAttributes(htmlAttributes, true);
            return buttonTag;
        }

        public static TagBuilder Button(string name, string buttonText, HtmlButtonType type, string onClickMethod, IDictionary<string, object> htmlAttributes) {
            if (name == null) {
                throw new ArgumentNullException("name");
            }
            
            TagBuilder buttonTag = new TagBuilder("button");

            if (!String.IsNullOrEmpty(name)) {
                buttonTag.MergeAttribute("name", name);
            }

            buttonTag.MergeAttribute("type", type.ToString().ToLowerInvariant());

            buttonTag.InnerHtml = buttonText;
            
            if (!String.IsNullOrEmpty(onClickMethod)) {
                buttonTag.MergeAttribute("onclick", onClickMethod);
            }

            buttonTag.MergeAttributes(htmlAttributes, true);
            return buttonTag;
        }
    }
}