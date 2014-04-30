using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Common.Wpf.TextListControl
{
	public partial class TextList : ListBox
	{
		#region Events

		public static RoutedEvent ListItemMouseUpEvent = EventManager.RegisterRoutedEvent("ListItemMouseUp", RoutingStrategy.Bubble, typeof(MouseButtonEventArgs), typeof(TextList));
		public event MouseButtonEventHandler ListItemMouseUp;

		public void OnListItemMouseUp(object listBoxItem, MouseButtonEventArgs e)
		{
			MouseButtonEventHandler handler = ListItemMouseUp;
			if (handler != null) handler(listBoxItem, e);
		}

		public static RoutedEvent ListItemMouseDoubleClickEvent = EventManager.RegisterRoutedEvent("ListItemMouseDoubleClick", RoutingStrategy.Bubble, typeof(MouseButtonEventArgs), typeof(TextList));
		public event MouseButtonEventHandler ListItemMouseDoubleClick;

		public void OnListItemMouseDoubleClick(object listBoxItem, MouseButtonEventArgs e)
		{
			MouseButtonEventHandler handler = ListItemMouseDoubleClick;
			if (handler != null) handler(listBoxItem, e);
		}

		#endregion

		#region Constructor

		public TextList()
		{
			InitializeComponent();
		}

		#endregion

		#region Hover selection events

		private void handleListItemMouseEnter(object sender, MouseEventArgs e)
		{
			// Make sure the control has focus
			Focus();

			// Get the list box item
			ListBoxItem listBoxItem = (ListBoxItem) sender;

			// Select the data context
			SelectedItem = listBoxItem.DataContext;

			// Set the cursor
			listBoxItem.Cursor = Cursors.Hand;
		}

		private void handleListItemMouseLeave(object sender, MouseEventArgs e)
		{
			// Clear selection
			SelectedItem = null;
		}

		#endregion

		#region List item events

		private void handleListItemMouseUp(object sender, MouseButtonEventArgs mouseButtonEventArgs)
		{
			// Get the list box item
			ListBoxItem listBoxItem = (ListBoxItem) sender;

			// Build the event args
			MouseButtonEventArgs eventArgs = new MouseButtonEventArgs(mouseButtonEventArgs.MouseDevice, mouseButtonEventArgs.Timestamp, mouseButtonEventArgs.ChangedButton);
			eventArgs.RoutedEvent = ListItemMouseUpEvent;

			// Raise the event
			OnListItemMouseUp(listBoxItem, eventArgs);
		}

		private void handleListItemMouseDoubleClick(object sender, MouseButtonEventArgs mouseButtonEventArgs)
		{
			// Get the list box item
			ListBoxItem listBoxItem = (ListBoxItem) sender;

			// Build the event args
			MouseButtonEventArgs eventArgs = new MouseButtonEventArgs(mouseButtonEventArgs.MouseDevice, mouseButtonEventArgs.Timestamp, mouseButtonEventArgs.ChangedButton);
			eventArgs.RoutedEvent = ListItemMouseDoubleClickEvent;

			// Raise the event
			OnListItemMouseDoubleClick(listBoxItem, eventArgs);
		}

		#endregion
	}
}
