using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Common.Wpf.Validation
{
    public class ValidationModelBase : INotifyPropertyChanged, INotifyDataErrorInfo
    {
        #region Property changed
        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        protected void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        #region Notify data error
        private readonly Dictionary<string, List<string>> _errors = new Dictionary<string, List<string>>();
        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged = delegate { };

        // get errors by property
        public IEnumerable GetErrors(string propertyName)
        {
            if (_errors.ContainsKey(propertyName))
                return _errors[propertyName];
            return null;
        }

        // has errors
        public bool HasErrors
        {
            get { return (_errors.Count > 0); }
        }

        // object is valid
        public bool IsValid
        {
            get { return !HasErrors; }

        }

        public void AddError(string error, [CallerMemberName] string propertyName = null)
        {
            if (propertyName == null)
                return;

            // Add error to list
            _errors[propertyName] = new List<string> { error };
            NotifyErrorsChanged(propertyName);
        }

        public void RemoveError([CallerMemberName] string propertyName = null)
        {
            if (propertyName == null)
                return;

            // remove error
            if (_errors.ContainsKey(propertyName))
                _errors.Remove(propertyName);

            NotifyErrorsChanged(propertyName);
        }

        private void NotifyErrorsChanged(string propertyName)
        {
            ErrorsChanged(this, new DataErrorsChangedEventArgs(propertyName));
        }

        #endregion

        protected void SetProperty<T>(ref T propertyValue, T newValue, [CallerMemberName] string propertyName = null)
        {
            if (Equals(propertyValue, newValue))
                return;

            propertyValue = newValue;
            NotifyPropertyChanged(propertyName);
            NotifyErrorsChanged(propertyName);
        }


    }
}
