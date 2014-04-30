using System.Collections.ObjectModel;

namespace Common.Wpf.HtmlTextBlock
{
    public class TextLine
    {
        public Collection<TextFragment> FragmentList { get; private set; }

        public TextLine()
        {
            FragmentList = new Collection<TextFragment>();
        }
    }
}
