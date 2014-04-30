using System.Windows.Controls;
using System.Windows.Input;

using Common.Wpf.Extensions;

namespace Common.Wpf.ExtendedListBoxControl
{
    public class ExtendedListBoxItem : ListBoxItem
    {
        #region Helper properties

        public ExtendedListBox ParentListBox
        {
            get
            {
                return this.GetAncestor<ExtendedListBox>();
            }
        }

        #endregion

        #region Fix selection handling for multiple selection

        private bool _fireMouseDownOnMouseUp;
        private SelectionMode? _selectionMode;

        private SelectionMode SelectionMode
        {
            get
            {
                if (_selectionMode == null)
                {
                    // Get the parent list box
                    ListBox listBox = ParentListBox;

                    // Cache the selection mode
                    _selectionMode = listBox.SelectionMode;
                }

                return _selectionMode.Value;
            }
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            // If the item is already selected we want to ignore the mouse down now and raise it on mouse up instead
            if (SelectionMode != SelectionMode.Single && IsSelected)
            {
                _fireMouseDownOnMouseUp = true;
                return;
            }

            // Call the normal mouse down
            base.OnMouseLeftButtonDown(e);
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            // If we ignored the earlier mouse down we need to fire it now
            if (SelectionMode != SelectionMode.Single && _fireMouseDownOnMouseUp)
            {
                // Call the normal mouse down
                base.OnMouseLeftButtonDown(e);

                // Clear the flag
                _fireMouseDownOnMouseUp = false;
            }

            // Call the normal mouse up
            base.OnMouseLeftButtonUp(e);
        }

        #endregion
    }
}
