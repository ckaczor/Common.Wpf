using System.Windows;
using System.Windows.Media;

namespace Common.Wpf.HtmlLabelControl
{
    public class TextFragmentStyle
    {
        public Brush Color { get; set; }
        public FontStyle? Style { get; set; }
        public FontWeight? Weight { get; set; }
        public double? Size { get; set; }
        public bool? Underline { get; set; }

        public void Apply(TextFragment fragment)
        {
            if (Color != null)
                fragment.Color = Color;

            if (Style.HasValue)
                fragment.Style = Style.Value;

            if (Weight.HasValue)
                fragment.Weight = Weight.Value;

            if (Size.HasValue)
                fragment.Size = Size.Value;

            if (Underline.HasValue)
                fragment.Underline = Underline.Value;
        }
    }
}