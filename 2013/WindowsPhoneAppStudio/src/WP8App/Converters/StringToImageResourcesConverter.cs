using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;

namespace WPAppStudio.Converters
{
    public class StringToImageResourcesConverter : IValueConverter
    {
        private const string LocalImageRootPath = @"..\Images";
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var imagePath = value as string;
            if (!string.IsNullOrEmpty(imagePath) && !imagePath.StartsWith("http"))
                return Path.Combine(LocalImageRootPath, imagePath);
            return imagePath;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
