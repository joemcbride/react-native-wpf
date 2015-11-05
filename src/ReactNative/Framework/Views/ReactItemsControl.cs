using System.Windows.Controls;
using Caliburn.Micro;

namespace ReactNative.Framework.Views
{
    public class ReactItemsControl : ItemsControl
    {
        public ReactItemsControl()
        {
            SetBinding(ItemsSourceProperty, "Views");
            ItemTemplate = ConventionManager.DefaultItemTemplate;
        }
    }
}