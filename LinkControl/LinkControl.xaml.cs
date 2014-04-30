using System.Windows;

namespace Common.Wpf.LinkControl
{
    public partial class LinkControl
    {
        public event RoutedEventHandler Click;

        public LinkControl()
        {
            InitializeComponent();

            HyperlinkControl.Click += HandleHyperlinkControlClick;
        }

        private void HandleHyperlinkControlClick(object sender, RoutedEventArgs e)
        {
            if (Click != null)
                Click.Invoke(sender, e);
        }

        public string Text
        {
            get { return ContentControl.Text; }
            set { ContentControl.Text = value; }
        }

        public new bool IsEnabled
        {
            get { return base.IsEnabled; }
            set
            {
                base.IsEnabled = value;
                HyperlinkControl.IsEnabled = value;
            }
        }
    }
}
