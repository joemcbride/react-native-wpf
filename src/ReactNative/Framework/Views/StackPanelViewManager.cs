using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Markup;
using ReactNative.Framework.Converters;

namespace ReactNative.Framework.Views
{
    public class ReactStackPanelView : ReactItemsControl
    {
        public ReactStackPanelView()
        {
            var template =
                "<ItemsPanelTemplate xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation' " +
                    "xmlns:cal='clr-namespace:Caliburn.Micro;assembly=Caliburn.Micro.Platform'> " +
                    "<StackPanel Name=\"PART_Panel\"/>" +
                "</ItemsPanelTemplate>";

            ItemsPanel = XamlReader.Parse(template) as ItemsPanelTemplate;
            Loaded += StackPanel_Loaded;
        }

        private void StackPanel_Loaded(object sender, RoutedEventArgs e)
        {
            var panel = this.FindChild<StackPanel>("PART_Panel");
            var binding = new Binding("Orientation")
            {
                Converter = new StringToOrientationConverter()
            };
            panel.SetBinding(StackPanel.OrientationProperty, binding);
        }
    }

    public class ReactStackPanelViewModel : ReactContainerComponentBase
    {
        private string _orientation;

        public string Orientation
        {
            get { return _orientation; }
            set
            {
                _orientation = value;
                NotifyOfPropertyChange(() => Orientation);
            }
        }
    }

    [ReactModule("ReactStackPanel")]
    public class StackPanelViewManager : ViewManager
    {
        public override IReactComponent View()
        {
            return new ReactStackPanelViewModel();
        }
    }
}