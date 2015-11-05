using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace ReactNative.Framework.Converters
{
    public class ColorToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));

            if (value is Color)
                return new SolidColorBrush((Color)value);

            if (value is string)
            {
                var converter = new BrushConverter();
                return converter.ConvertFromString((string)value);
            }

            throw new NotSupportedException("ColorToBrushConverter only supports converting from Color and String");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }

        private static Color Parse(string color)
        {
            var offset = color.StartsWith("#") ? 1 : 0;

            var a = Byte.Parse(color.Substring(0 + offset, 2), NumberStyles.HexNumber);
            var r = Byte.Parse(color.Substring(2 + offset, 2), NumberStyles.HexNumber);
            var g = Byte.Parse(color.Substring(4 + offset, 2), NumberStyles.HexNumber);
            var b = Byte.Parse(color.Substring(6 + offset, 2), NumberStyles.HexNumber);

            return Color.FromArgb(a, r, g, b);
        }
    }
}
