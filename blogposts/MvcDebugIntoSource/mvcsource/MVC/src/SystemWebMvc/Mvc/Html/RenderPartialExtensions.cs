namespace System.Web.Mvc.Html {

    public static class RenderPartialExtensions {
        // Renders the partial view with the parent's view data
        public static void RenderPartial(this HtmlHelper htmlHelper, string partialViewName) {
            htmlHelper.RenderPartialInternal(partialViewName, htmlHelper.ViewData, null, ViewEngines.Engines);
        }

        // Renders the partial view with the given view data
        public static void RenderPartial(this HtmlHelper htmlHelper, string partialViewName, ViewDataDictionary viewData) {
            htmlHelper.RenderPartialInternal(partialViewName, viewData, null, ViewEngines.Engines);
        }

        // Renders the partial view with an empty view data and the given model
        public static void RenderPartial(this HtmlHelper htmlHelper, string partialViewName, object model) {
            htmlHelper.RenderPartialInternal(partialViewName, htmlHelper.ViewData, model, ViewEngines.Engines);
        }

        // Renders the partial view with a copy of the given view data plus the given model
        public static void RenderPartial(this HtmlHelper htmlHelper, string partialViewName, object model, ViewDataDictionary viewData) {
            htmlHelper.RenderPartialInternal(partialViewName, viewData, model, ViewEngines.Engines);
        }
    }
}