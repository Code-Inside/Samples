namespace Microsoft.Web.Mvc {
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Runtime.Serialization.Formatters.Binary;
    using System.Web;
    using System.Web.Mvc;

    public class CookieTempDataProvider : ITempDataProvider {
        internal const string TempDataCookieKey = "__ControllerTempData";
        HttpContextBase _httpContext;

        public CookieTempDataProvider(HttpContextBase httpContext) {
            if (httpContext == null) {
                throw new ArgumentNullException("httpContext");
            }
            _httpContext = httpContext;
        }

        public HttpContextBase HttpContext {
            get {
                return _httpContext;
            }
        }

        protected virtual IDictionary<string, object> LoadTempData(ControllerContext controllerContext) {
            HttpCookie cookie = _httpContext.Request.Cookies[TempDataCookieKey];
            if(cookie != null && !string.IsNullOrEmpty(cookie.Value)) {
                IDictionary<string, object> deserializedTempData = DeserializeTempData(cookie.Value);

                cookie.Expires = DateTime.MinValue;
                cookie.Value = string.Empty;

                if (_httpContext.Response != null && _httpContext.Response.Cookies != null) {
                    HttpCookie responseCookie = _httpContext.Response.Cookies[TempDataCookieKey];
                    if (responseCookie != null) {
                        cookie.Expires = DateTime.MinValue;
                        cookie.Value = string.Empty;
                    }
                }

                return deserializedTempData;
            }

            return new Dictionary<string, object>();
        }

        protected virtual void SaveTempData(ControllerContext controllerContext, IDictionary<string, object> values) {
            string cookieValue = SerializeToBase64EncodedString(values);
            
            var cookie = new HttpCookie(TempDataCookieKey);
            cookie.HttpOnly = true;
            cookie.Value = cookieValue;
            
            _httpContext.Response.Cookies.Add(cookie);
        }

        public static IDictionary<string, object> DeserializeTempData(string base64EncodedSerializedTempData) {
            byte[] bytes = Convert.FromBase64String(base64EncodedSerializedTempData);
            var memStream = new MemoryStream(bytes);
            var binFormatter = new BinaryFormatter();
            return binFormatter.Deserialize(memStream, null) as TempDataDictionary;
        }

        public static string SerializeToBase64EncodedString(IDictionary<string, object> values) {
            MemoryStream memStream = new MemoryStream();
            memStream.Seek(0, SeekOrigin.Begin);
            var binFormatter = new BinaryFormatter();
            binFormatter.Serialize(memStream, values);
            memStream.Seek(0, SeekOrigin.Begin);
            byte[] bytes = memStream.ToArray();
            return Convert.ToBase64String(bytes);
        }
        
        IDictionary<string, object> ITempDataProvider.LoadTempData(ControllerContext controllerContext) {
            return LoadTempData(controllerContext);
        }

        void ITempDataProvider.SaveTempData(ControllerContext controllerContext, IDictionary<string, object> values) {
            SaveTempData(controllerContext, values);
        }
    }
}
