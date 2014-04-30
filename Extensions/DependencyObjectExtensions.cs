using System.Windows;
using System.Windows.Media;

namespace Common.Wpf.Extensions
{
    public static class DependencyObjectExtensions
    {
        public static T GetAncestor<T>(this DependencyObject referenceObject) where T : class
        {
            DependencyObject parent = VisualTreeHelper.GetParent(referenceObject);

            while (parent != null && !parent.GetType().Equals(typeof(T)))
            {
                parent = VisualTreeHelper.GetParent(parent);
            }

            return parent as T;
        }
    }
}
