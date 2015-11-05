using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;

namespace ReactNative.Framework.Converters
{
    public class StringToOrientationConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var result = Orientation.Vertical;
            var orientation = value?.ToString();
            Enum.TryParse(orientation, true, out result);
            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
