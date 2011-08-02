namespace Microsoft.Web.Mvc {
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq.Expressions;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Mvc.Html;
    using System.Web.Routing;

    public static class FormExtensions {
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "This is an Extension Method which allows the user to provide a strongly-typed argument via Expression")]
        public static MvcForm BeginForm<TController>(this HtmlHelper helper, Expression<Action<TController>> action) where TController : Controller {
            return BeginForm<TController>(helper, action, FormMethod.Post, null);
        }

        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "This is an Extension Method which allows the user to provide a strongly-typed argument via Expression")]
        public static MvcForm BeginForm<TController>(this HtmlHelper helper, Expression<Action<TController>> action, FormMethod method) where TController : Controller {
            return BeginForm<TController>(helper, action, method, null);
        }

        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "This is an Extension Method which allows the user to provide a strongly-typed argument via Expression")]
        public static MvcForm BeginForm<TController>(this HtmlHelper helper, Expression<Action<TController>> action, FormMethod method, object htmlAttributes) where TController : Controller {
            return BeginForm<TController>(helper, action, method, new RouteValueDictionary(htmlAttributes));
        }

        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "This is an Extension Method which allows the user to provide a strongly-typed argument via Expression")]
        public static MvcForm BeginForm<TController>(this HtmlHelper helper, Expression<Action<TController>> action, FormMethod method, IDictionary<string, object> htmlAttributes) where TController : Controller {
            TagBuilder tagBuilder = new TagBuilder("form");
            tagBuilder.MergeAttributes(htmlAttributes);
            string formAction = LinkExtensions.BuildUrlFromExpression(helper, action);
            tagBuilder.MergeAttribute("action", formAction);
            tagBuilder.MergeAttribute("method", HtmlHelper.GetFormMethodString(method));
         
            HttpResponseBase httpResponse = helper.ViewContext.HttpContext.Response;
            httpResponse.Write(tagBuilder.ToString(TagRenderMode.StartTag));
            return new MvcForm(helper.ViewContext.HttpContext.Response);
        }
    }
}
