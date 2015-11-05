using ReactNative.Framework.Converters;
using System.Windows.Controls;
using System.Windows.Data;

namespace ReactNative.Framework.Views
{
    public class ReactTextBlockView : TextBlock
    {
        public ReactTextBlockView()
        {
            SetBinding(TextProperty, "Text");

            var backgroundBinding = new Binding("BackgroundColor");
            backgroundBinding.Converter = new ColorToBrushConverter();
            SetBinding(BackgroundProperty, backgroundBinding);
        }
    }
}
