namespace Sys.Mvc {
    using System.DHTML;
    using Sys.UI;

    public static class AsyncForm {
        public static void HandleSubmit(FormElement form, DomEvent evt, AjaxOptions ajaxOptions) {
            evt.PreventDefault();
            string body = MvcHelpers.SerializeForm(form);
            MvcHelpers.AsyncRequest(form.Action, 
                                    form.Method ?? "post",
                                    body,
                                    form,
                                    ajaxOptions);

        }
    }
}
