using System.ComponentModel;

namespace Common.Wpf
{
    public class CheckedListItem<T> : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private bool _isChecked;
        private T _item;

        public CheckedListItem() { }

        public CheckedListItem(T item, bool isChecked = false)
        {
            _item = item;
            _isChecked = isChecked;
        }

        public T Item
        {
            get
            {
                return _item;
            }
            set
            {
                _item = value;

                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("Item"));
            }
        }

        public bool IsChecked
        {
            get
            {
                return _isChecked;
            }
            set
            {
                _isChecked = value;

                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("IsChecked"));
            }
        }
    }
}
