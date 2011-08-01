namespace Microsoft.Web.Mvc {
    using System.Web;
    using System.Web.Mvc;

    public static class CacheExtensions {
        public static object Substitute(this HtmlHelper html, MvcSubstitutionCallback substitutionCallback) {
            html.ViewContext.HttpContext.Response.WriteSubstitution(httpContext => substitutionCallback(new HttpContextWrapper(httpContext)));
            return null;
        }
    }
}
