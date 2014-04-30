using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Common.Wpf.Toolbar
{
    public partial class ImageButton
    {
        public ImageButton()
        {
            InitializeComponent();

            Loaded += HandleImageButtonLoaded;
        }

        void HandleImageButtonLoaded(object sender, RoutedEventArgs e)
        {
            if (Style == null && Parent is ToolBar)
            {
                Style = (Style) FindResource(ToolBar.ButtonStyleKey);
            }
        }        

        public ImageSource ImageSource
        {
            get { return image.Source; }
            set { image.Source = value; }
        }
    }
}
