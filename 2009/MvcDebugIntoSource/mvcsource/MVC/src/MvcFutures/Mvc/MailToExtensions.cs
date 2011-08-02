namespace Microsoft.Web.Mvc {
    using System;
    using System.Collections.Generic;
    using System.Web.Mvc;
    using System.Web.Routing;

    public static class MailToExtensions {
        public static string Mailto(this HtmlHelper helper, string linkText, string emailAddress) {
            return Mailto(helper, linkText, emailAddress, null, null, null, null, (IDictionary<string, object>)null);
        }

        public static string Mailto(this HtmlHelper helper, string linkText, string emailAddress, object htmlAttributes) {
            return Mailto(helper, linkText, emailAddress, null, null, null, null, htmlAttributes);
        }

        public static string Mailto(this HtmlHelper helper, string linkText, string emailAddress, IDictionary<string, object> htmlAttributes) {
            return Mailto(helper, linkText, emailAddress, null, null, null, null, htmlAttributes);
        }

        public static string Mailto(this HtmlHelper helper, string linkText, string emailAddress, string subject) {
            return Mailto(helper, linkText, emailAddress, subject, null, null, null, (IDictionary<string, object>)null);
        }

        public static string Mailto(this HtmlHelper helper, string linkText, string emailAddress, string subject, object htmlAttributes) {
            return Mailto(helper, linkText, emailAddress, subject, null, null, null, htmlAttributes);
        }

        public static string Mailto(this HtmlHelper helper, string linkText, string emailAddress, string subject, IDictionary<string, object> htmlAttributes) {
            return Mailto(helper, linkText, emailAddress, subject, null, null, null, htmlAttributes);
        }

        public static string Mailto(this HtmlHelper helper, string linkText, string emailAddress, string subject, string body, string cc, string bcc, object htmlAttributes) {
            return Mailto(helper, linkText, emailAddress, subject, body, cc, bcc, new RouteValueDictionary(htmlAttributes));
        }

        public static string Mailto(this HtmlHelper helper, string linkText, string emailAddress, string subject,
            string body, string cc, string bcc, IDictionary<string, object> htmlAttributes) {
            if (emailAddress == null) {
                throw new ArgumentNullException("emailAddress"); // TODO: Resource message
            }
            if (linkText == null) {
                throw new ArgumentNullException("linkText"); // TODO: Resource message
            }

            
            string mailToUrl = "mailto:" + emailAddress;

            List<string> mailQuery = new List<string>();
            if (!String.IsNullOrEmpty(subject)) {
                mailQuery.Add("subject=" + helper.Encode(subject));
            }

            if (!String.IsNullOrEmpty(cc)) {
                mailQuery.Add("cc=" + helper.Encode(cc));
            }

            if (!String.IsNullOrEmpty(bcc)) {
                mailQuery.Add("bcc=" + helper.Encode(bcc));
            }

            if (!String.IsNullOrEmpty(body)) {
                string encodedBody = helper.Encode(body);
                encodedBody = encodedBody.Replace(Environment.NewLine, "%0A");
                mailQuery.Add("body=" + encodedBody);
            }

            string query = string.Empty;
            for (int i = 0; i < mailQuery.Count; i++) {
                query += mailQuery[i];
                if (i < mailQuery.Count - 1) {
                    query += "&";
                }
            }
            if (query.Length > 0) {
                mailToUrl += "?" + query;
            }

            TagBuilder mailtoAnchor = new TagBuilder("a");
            mailtoAnchor.MergeAttribute("href", mailToUrl);
            mailtoAnchor.MergeAttributes(htmlAttributes, true);
            mailtoAnchor.InnerHtml = linkText;
            return mailtoAnchor.ToString();
        }

        
    }
}
