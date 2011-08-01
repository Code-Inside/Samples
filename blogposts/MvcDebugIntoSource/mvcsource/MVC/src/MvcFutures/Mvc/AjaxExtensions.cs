namespace Microsoft.Web.Mvc {
    using System;
    using System.Text;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Script.Serialization;

    public static class AjaxExtensions {

        public static string JavaScriptStringEncode(this AjaxHelper helper, string message) {
            if (String.IsNullOrEmpty(message)) {
                return message;
            }

            StringBuilder builder = new StringBuilder();
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            serializer.Serialize(message, builder);
            return builder.ToString(1, builder.Length - 2); // remove first + last quote
        }

    }
}
