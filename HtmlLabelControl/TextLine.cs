using System.Collections.ObjectModel;

namespace Common.Wpf.HtmlLabelControl
{
    public class TextLine
    {
        private double _height;
        public double Height
        {
            get
            {
                if (_height == 0)
                {
                    foreach (TextFragment textFragment in FragmentList)
                    {
                        if (textFragment.FormattedText.Height > _height)
                            _height = textFragment.FormattedText.Height;
                    }
                }

                return _height;
            }
        }

        private double _width;
        public double Width
        {
            get
            {
                if (_width == 0)
                {
                    foreach (TextFragment textFragment in FragmentList)
                    {
                        _width += textFragment.FormattedText.Width;
                    }
                }

                return _width;
            }
        }

        public Collection<TextFragment> FragmentList { get; private set; }

        public TextLine()
        {
            FragmentList = new Collection<TextFragment>();
        }
    }
}
