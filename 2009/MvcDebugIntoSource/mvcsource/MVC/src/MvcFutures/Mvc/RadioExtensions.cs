namespace Microsoft.Web.Mvc {
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Web.Mvc;
    using System.Web.Mvc.Html;
    using System.Web.Routing;
    using Microsoft.Web.Resources;

    public static class RadioListExtensions {
        public static string[] RadioButtonList(this HtmlHelper htmlHelper, string name) {
            return RadioButtonList(htmlHelper, name, (IDictionary<string, object>)null);
        }

        public static string[] RadioButtonList(this HtmlHelper htmlHelper, string name, object htmlAttributes) {
            return RadioButtonList(htmlHelper, name, new RouteValueDictionary(htmlAttributes));
        }

        public static string[] RadioButtonList(this HtmlHelper htmlHelper, string name, IDictionary<string, object> htmlAttributes) {
            IEnumerable<SelectListItem> selectList = htmlHelper.GetSelectData(name);
            return htmlHelper.RadioButtonListInternal(name, selectList, true /* usedViewData */, htmlAttributes);
        }

        public static string[] RadioButtonList(this HtmlHelper htmlHelper, string name, IEnumerable<SelectListItem> selectList) {
            return RadioButtonList(htmlHelper, name, selectList, (IDictionary<string, object>)null);
        }

        public static string[] RadioButtonList(this HtmlHelper htmlHelper, string name, IEnumerable<SelectListItem> selectList, object htmlAttributes) {
            return RadioButtonList(htmlHelper, name, selectList, new RouteValueDictionary(htmlAttributes));
        }

        public static string[] RadioButtonList(this HtmlHelper htmlHelper, string name, IEnumerable<SelectListItem> selectList, IDictionary<string, object> htmlAttributes) {
            return htmlHelper.RadioButtonListInternal(name, selectList, false /* usedViewData */, htmlAttributes);
        }

        private static IEnumerable<SelectListItem> GetSelectData(this HtmlHelper htmlHelper, string name) {
            object o = null;
            if (htmlHelper.ViewData != null) {
                o = htmlHelper.ViewData.Eval(name);
            }
            if (o == null) {
                throw new InvalidOperationException(
                    String.Format(
                        CultureInfo.CurrentUICulture,
                        MvcResources.HtmlHelper_MissingSelectData,
                        name,
                        typeof(IEnumerable<SelectListItem>)));
            }
            IEnumerable<SelectListItem> selectList = o as IEnumerable<SelectListItem>;
            if (selectList == null) {
                throw new InvalidOperationException(
                    String.Format(
                        CultureInfo.CurrentUICulture,
                        MvcResources.HtmlHelper_WrongSelectDataType,
                        name,
                        o.GetType().FullName,
                        typeof(IEnumerable<SelectListItem>)));
            }
            return selectList;
        }

        private static string[] RadioButtonListInternal(this HtmlHelper htmlHelper, string name, IEnumerable<SelectListItem> selectList, bool usedViewData, IDictionary<string, object> htmlAttributes) {
            if (String.IsNullOrEmpty(name)) {
                throw new ArgumentException(MvcResources.Common_NullOrEmpty, "name");
            }
            if (selectList == null) {
                throw new ArgumentNullException("selectList");
            }

            // If we haven't already used ViewData to get the entire list of items then we need to
            // use the ViewData-supplied value before using the parameter-supplied value.
            if (!usedViewData) {
                object defaultValue = htmlHelper.ViewData.Eval(name);

                if (defaultValue != null) {
                    IEnumerable defaultValues = new[] { defaultValue };
                    IEnumerable<string> values = from object value in defaultValues select Convert.ToString(value, CultureInfo.CurrentCulture);
                    HashSet<string> selectedValues = new HashSet<string>(values, StringComparer.OrdinalIgnoreCase);
                    List<SelectListItem> newSelectList = new List<SelectListItem>();

                    foreach (SelectListItem item in selectList) {
                        item.Selected = (item.Value != null) ? selectedValues.Contains(item.Value) : selectedValues.Contains(item.Text);
                        newSelectList.Add(item);
                    }

                    selectList = newSelectList;
                }
            }

            IEnumerable<string> radioButtons = selectList.Select<SelectListItem, string>(item => htmlHelper.RadioButton(name, item.Value, item.Selected, htmlAttributes));

            return radioButtons.ToArray();
        }
    }
}
