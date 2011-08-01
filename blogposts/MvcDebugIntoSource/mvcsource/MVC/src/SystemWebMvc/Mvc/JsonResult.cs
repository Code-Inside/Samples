namespace System.Web.Mvc {
    using System;
    using System.Text;
    using System.Web;
    using System.Web.Script.Serialization;

    public class JsonResult : ActionResult {

        public Encoding ContentEncoding {
            get;
            set;
        }

        public string ContentType {
            get;
            set;
        }

        public object Data {
            get;
            set;
        }

        public override void ExecuteResult(ControllerContext context) {
            if (context == null) {
                throw new ArgumentNullException("context");
            }

            HttpResponseBase response = context.HttpContext.Response;

            if (!String.IsNullOrEmpty(ContentType)) {
                response.ContentType = ContentType;
            }
            else {
                response.ContentType = "application/json";
            }
            if (ContentEncoding != null) {
                response.ContentEncoding = ContentEncoding;
            }
            if (Data != null) {
                // The JavaScriptSerializer type was marked as obsolete prior to .NET Framework 3.5 SP1
#pragma warning disable 0618
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                response.Write(serializer.Serialize(Data));
#pragma warning restore 0618
            }
        }
    }
}
