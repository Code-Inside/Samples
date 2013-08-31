using System;
using System.Collections.Generic;
using System.Windows;
using Microsoft.Phone.Controls;
using Newtonsoft.Json;

namespace WPAppStudio.Helpers
{
    public static class WebBrowserBindingHelper
    {
        public static readonly DependencyProperty HtmlProperty = DependencyProperty.RegisterAttached(
        "Html", typeof(string), typeof(WebBrowserHelper), new PropertyMetadata(OnHtmlChanged));

        public static string GetHtml(DependencyObject dependencyObject)
        {
            return (string)dependencyObject.GetValue(HtmlProperty);
        }

        public static void SetHtml(DependencyObject dependencyObject, string value)
        {
            dependencyObject.SetValue(HtmlProperty, value);
        }

        private static void OnHtmlChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var browser = d as WebBrowser;

            if (browser == null)
                return;

            var html = e.NewValue.ToString();
            browser.ScriptNotify += browser_ScriptNotify;
            browser.NavigateToString(WebBrowserHelper.WrapHtml(html, Application.Current.RootVisual.RenderSize.Width));
        }

        static void browser_ScriptNotify(object sender, NotifyEventArgs e)
        {
            var wb = sender as WebBrowser;
            if (wb == null) return;
            var json =
                (Dictionary<String, String>)
                JsonConvert.DeserializeObject(e.Value, typeof(Dictionary<String, String>));
            if (json.ContainsKey("offset_Height"))
                wb.Height = Convert.ToInt32(Math.Abs(Convert.ToDouble(json["offset_Height"])));
        }
    }
}
