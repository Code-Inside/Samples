using System.Text;
using System.Windows;
using System.Windows.Media;

namespace WPAppStudio.Helpers
{
    public class WebBrowserHelper
    {       
        public static string AdjustHeight
        {
            get
            {
                return @"<script>
                    function AdjustHeight() {
                        var _offsetHeight=document.body.firstChild.offsetHeight* 0.43;
                        var _json = '{""offset_Height"" : '+_offsetHeight+'}';
                        window.external.notify(_json);
                    }
                    AdjustHeight();
                    </script>";
            }
        }
        public static string WrapHtml(string htmlSubString, double viewportWidth)
        {
            var html = new StringBuilder();
            html.Append("<html>");
            html.Append(HtmlHeader(viewportWidth));
            html.Append("<body><div>");
            html.Append(htmlSubString);
            html.Append("</body></div>");
            html.Append(AdjustHeight);
            html.Append("</html>");
            return html.ToString();
        }

        public static string HtmlHeader(double viewportWidth)
        {
            var head = new StringBuilder();

            head.Append("<head>");
            //head.Append(string.Format(
            //    "<meta name=\"viewport\" content=\"width={0}\" user-scalable=\"no\" />",
            //    viewportWidth));
            head.Append("<style>");
            head.Append("html { -ms-text-size-adjust:120% }");
            head.Append(string.Format(
                "body {{background:{0};color:{1};font-family:'{2}';font-size:{3}pt;margin:0;padding:0;}}",
                GetBrowserColor("PhoneBackgroundColor"),
                GetBrowserColor("PhoneForegroundColor"),
                Application.Current.Resources["PhoneFontFamilyNormal"].ToString(),
                (double)Application.Current.Resources["PhoneFontSizeLarge"])
                );
            head.Append(string.Format(
                "a {{color:{0}}}",
                GetBrowserColor("PhoneAccentColor")));
            head.Append("</style>");            
            head.Append("</head>");
            return head.ToString();
        }

        private static string GetBrowserColor(string sourceResource)
        {
            var color = (Color)Application.Current.Resources[sourceResource];
            return "#" + color.ToString().Substring(3, 6);
        }
    }
}

