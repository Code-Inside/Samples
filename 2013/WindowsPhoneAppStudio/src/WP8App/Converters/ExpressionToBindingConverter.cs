using System;
using System.Text.RegularExpressions;
using System.Windows.Data;
using WPAppStudio.Entities.Base;

namespace WPAppStudio.Converters
{
    /// <summary>
    /// Expression with bindings to regular string converter class.
    /// </summary>
    public class ExpressionToBindingConverter : IValueConverter
    {
        /// <summary>
        /// Converts an expression with bindings to a regular string.
        /// </summary>
        /// <param name="value">The binded object from a repository.</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">The binding expression.</param>
        /// <param name="culture">The culture to be used in the conversion.</param>
        /// <returns>String with resolved bindings.</returns>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var parameterLabel = parameter as string;
            if (!string.IsNullOrEmpty(parameterLabel) && value != null)
                parameterLabel = BindingExpressionUtil.ResolveBindingExpression(value, parameterLabel);
            return parameterLabel;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
