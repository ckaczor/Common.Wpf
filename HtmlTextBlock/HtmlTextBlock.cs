using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace Common.Wpf.HtmlTextBlock
{
    public class HtmlTextBlock : TextBlock
    {
        public static readonly DependencyProperty HtmlProperty = DependencyProperty.Register("Html", typeof(string), typeof(HtmlTextBlock), new UIPropertyMetadata("Html", OnHtmlChanged));

        public static void OnHtmlChanged(DependencyObject s, DependencyPropertyChangedEventArgs e)
        {
            HtmlTextBlock htmlTextBlock = (HtmlTextBlock) s;

            Parse(htmlTextBlock, e.NewValue as string);
        }

        private static void Parse(HtmlTextBlock control, string value)
        {
            try
            {
                control.Inlines.Clear();

                TextParser parser = new TextParser();
                var lines = parser.Parse(control, value);

                foreach (TextLine line in lines)
                {
                    foreach (TextFragment fragment in line.FragmentList)
                    {
                        Run run = new Run(fragment.Text);

                        run.FontStyle = fragment.Style;
                        run.FontWeight = fragment.Weight;
                        run.FontSize = fragment.Size;
                        run.Foreground = fragment.Color;

                        control.Inlines.Add(run);
                    }
                }
            }
            catch (Exception)
            {
                control.Inlines.Clear();

                control.Text = value;
            }
        }

        public string Html
        {
            get { return (string) GetValue(HtmlProperty); }
            set { SetValue(HtmlProperty, value); }
        }
    }
}
