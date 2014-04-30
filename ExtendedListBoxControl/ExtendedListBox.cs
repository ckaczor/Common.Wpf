using System.Windows;
using System.Windows.Controls;

namespace Common.Wpf.ExtendedListBoxControl
{
    public class ExtendedListBox : ListBox
    {
        protected override DependencyObject GetContainerForItemOverride()
        {
            return new ExtendedListBoxItem();
        }

        private int _lastSelectedIndex = -1;

        protected override void OnSelectionChanged(SelectionChangedEventArgs e)
        {
            base.OnSelectionChanged(e);

            if (SelectedIndex == -1 && _lastSelectedIndex != -1)
            {
                int itemCount = Items.Count;

                if (_lastSelectedIndex >= itemCount)
                    _lastSelectedIndex = itemCount - 1;

                SelectedIndex = _lastSelectedIndex;
            }

            _lastSelectedIndex = SelectedIndex;
        }
    }
}
