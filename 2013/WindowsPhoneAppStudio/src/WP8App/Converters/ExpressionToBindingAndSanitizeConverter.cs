using System;
using System.Text.RegularExpressions;
using System.Windows.Data;
using WPAppStudio.Entities.Base;

namespace WPAppStudio.Converters
{
    /// <summary>
    /// Expression with bindings to sanitized string converter class.
    /// </summary>
    public class ExpressionToBindingAndSanitizeConverter : IValueConverter
    {
        /// <summary>
        /// Converts a regular expression into a binded property and sanitize it.
        /// </summary>
        /// <param name="value">The binded object from a repository.</param>
        /// <param name="targetType">The type of the conversion.</param>
        /// <param name="parameter">The binding expression.</param>
        /// <param name="culture">The culture to be used in the conversion.</param>
        /// <returns>String with resolved bindings.</returns>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var parameterLabel = parameter as string;
            if (!string.IsNullOrEmpty(parameterLabel) && value != null)
            {
                parameterLabel = BindingExpressionUtil.ResolveBindingExpression(value, parameterLabel);
                parameterLabel = HtmlUtil.CleanHtml(parameterLabel);
            }
            return parameterLabel;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
