using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Common.Wpf.HtmlLabelControl
{
    /// <summary>
    /// Interaction logic for HtmlLabel.xaml
    /// </summary>
    public partial class HtmlLabel : UserControl
    {
        private Collection<TextLine> _textLines;

        public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(HtmlLabel), new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.AffectsRender));

        public HtmlLabel()
        {
            InitializeComponent();
        }

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);

            switch (e.Property.Name)
            {
                case "FontFamily":
                case "FontSize":
                case "Foreground":
                    // Force the control to re-parse
                    Text = Text;
                    break;

                case "HorizontalContentAlignment":
                case "VerticalContentAlignment":
                case "Padding":
                    InvalidateVisual();
                    break;
            }
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            if (string.IsNullOrEmpty(Text))
                return;

            Point drawPoint = new Point(Padding.Left, Padding.Top);

            switch (VerticalContentAlignment)
            {
                case VerticalAlignment.Bottom:
                    drawPoint.Y = ActualHeight - Padding.Bottom;

                    foreach (TextLine line in _textLines)
                    {
                        drawPoint.Y -= line.Height;
                    }

                    break;

                case VerticalAlignment.Center:
                    double totalHeight = 0;

                    foreach (TextLine line in _textLines)
                    {
                        totalHeight += line.Height;
                    }

                    drawPoint.Y = ((ActualHeight - (Padding.Top + Padding.Bottom)) / 2) - (totalHeight / 2);

                    break;
            }

            foreach (TextLine line in _textLines)
            {
                if (drawPoint.Y < 0)
                {
                    drawPoint.Y += line.Height;
                    continue;
                }

                drawPoint.X = Padding.Left;

                switch (HorizontalContentAlignment)
                {
                    case HorizontalAlignment.Right:
                        drawPoint.X = ActualWidth - line.Width - Padding.Right;
                        break;

                    case HorizontalAlignment.Center:
                        drawPoint.X = ((ActualWidth - (Padding.Left + Padding.Right)) / 2) - (line.Width / 2);
                        break;
                }

                foreach (TextFragment fragment in line.FragmentList)
                {
                    drawingContext.DrawText(fragment.FormattedText, drawPoint);

                    drawPoint.X += fragment.FormattedText.WidthIncludingTrailingWhitespace;
                }

                drawPoint.Y += line.Height;
            }
        }

        public string Text
        {
            get
            {
                return (string) GetValue(TextProperty);
            }
            set
            {
                TextParser textParser = new TextParser();
                _textLines = textParser.Parse(this, value);

                SetValue(TextProperty, value);
            }
        }
    }
}