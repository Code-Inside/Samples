namespace Sys.Mvc {
    using System;
    using System.DHTML;
    using Sys.Net;
    using Sys.UI;

    public static class AsyncHyperlink {
        public static void HandleClick(AnchorElement anchor, DomEvent evt, AjaxOptions ajaxOptions) {
            evt.PreventDefault();
            MvcHelpers.AsyncRequest(anchor.Href,
                                    "post",
                                    "",
                                    anchor,
                                    ajaxOptions);
        }
    }
}
