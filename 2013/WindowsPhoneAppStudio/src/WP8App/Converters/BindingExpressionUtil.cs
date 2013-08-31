using System.Text.RegularExpressions;
using WPAppStudio.Entities.Base;

namespace WPAppStudio.Converters
{
    /// <summary>
    /// A helper to resolve expressions with bindings
    /// </summary>
    public class BindingExpressionUtil
    {
        private const string Regexp = @"\{(?<binding>[^\}]+)\}";

        /// <summary>
        /// Resolve an expression with bindings
        /// </summary>
        /// <param name="value">Binding data source</param>
        /// <param name="parameterLabel">Expression with bindings</param>
        /// <returns></returns>
        public static string ResolveBindingExpression(object value, string parameterLabel)
        {
            var matches = Regex.Matches(parameterLabel, Regexp);
            foreach (Match match in matches)
            {
                var bindingMatch = match.Groups["binding"];
                var prop = value.GetType().GetProperty(bindingMatch.Value);
                string replaceValue = string.Empty;
                if (prop != null)
                    replaceValue = prop.GetValue(value).ToString();
                parameterLabel = parameterLabel.Replace("{" + bindingMatch.Value + "}", replaceValue);
                parameterLabel = HtmlUtil.CleanHtml(parameterLabel);
                parameterLabel = parameterLabel.Trim();
            }
            return parameterLabel;
        }
    }
}