using System;
using System.Windows;
using System.Windows.Media;

namespace ReactNative.Framework.Views
{
    public static class VisualTreeExtensions
    {
        public static T FindChild<T>(this FrameworkElement obj, string name)
        {
            var dep = obj as DependencyObject;
            T ret = default(T);

            if (dep != null)
            {
                int childcount = VisualTreeHelper.GetChildrenCount(dep);
                for (int i = 0; i < childcount; i++)
                {
                    DependencyObject childDep = VisualTreeHelper.GetChild(dep, i);
                    FrameworkElement child = childDep as FrameworkElement;

                    if (child.GetType() == typeof(T) && child.Name == name)
                    {
                        ret = (T)Convert.ChangeType(child, typeof(T));
                        break;
                    }

                    ret = child.FindChild<T>(name);
                    if (ret != null)
                        break;
                }
            }
            return ret;
        }
    }
}