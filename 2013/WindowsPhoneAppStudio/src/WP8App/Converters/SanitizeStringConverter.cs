using System;
using System.Text.RegularExpressions;
using System.Windows.Data;
using WPAppStudio.Entities.Base;

namespace WPAppStudio.Converters
{
    /// <summary>
    /// Sanitize string converter class.
    /// </summary>
    public class SanitizeStringConverter : IValueConverter
    {
        /// <summary>
        /// Sanitize a string.
        /// </summary>
        /// <param name="value">The binded string.</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">The converter parameter.</param>
        /// <param name="culture">The culture to be used in the conversion.</param>
        /// <returns>A regular string.</returns>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var valueLabel = string.Empty;
            if (value != null)
                valueLabel = HtmlUtil.CleanHtml(value.ToString());
            return valueLabel;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
