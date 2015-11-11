using System;
using System.Windows.Controls;

namespace Common.Wpf.Windows
{
    public class CategoryPanel : UserControl
    {
        private object Data { get; set; }

        public virtual void LoadPanel(object data)
        {
            Data = data;
        }

        public virtual bool ValidatePanel()
        {
            throw new NotImplementedException();
        }

        public virtual void SavePanel()
        {
            throw new NotImplementedException();
        }

        public virtual string CategoryName => null;
    }
}
