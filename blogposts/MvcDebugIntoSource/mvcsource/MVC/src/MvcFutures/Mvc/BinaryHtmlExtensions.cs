namespace Microsoft.Web.Mvc {
    using System;
    using System.Data.Linq;
    using System.Linq.Expressions;
    using System.Web.Mvc;
    using System.Web.Mvc.Html;
    using Microsoft.Web.Mvc.Internal;

    public static class BinaryHtmlExtensions {
        public static string Hidden(this HtmlHelper htmlHelper, string name, byte[] value) {
            return htmlHelper.Hidden(name, Convert.ToBase64String(value));
        }

        public static string Hidden(this HtmlHelper htmlHelper, string name, Binary value) {
            return htmlHelper.Hidden(name, value.ToArray());
        }

        public static string HiddenFor<TModel>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, byte[]>> expression) where TModel : class {
            string inputName = ExpressionHelper.GetInputName(expression);
            byte[] value = ExpressionInputExtensions.GetValue(htmlHelper, expression);
            return htmlHelper.Hidden(inputName, value);
        }

        public static string HiddenFor<TModel>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, Binary>> expression) where TModel : class {
            string inputName = ExpressionHelper.GetInputName(expression);
            Binary value = ExpressionInputExtensions.GetValue(htmlHelper, expression);
            return htmlHelper.Hidden(inputName, value.ToArray());
        }
    }
}
