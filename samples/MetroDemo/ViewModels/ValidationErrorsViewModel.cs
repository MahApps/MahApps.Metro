using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace MetroDemo.ViewModels
{
#if NET_4_5
    internal class ValidationErrorsViewModel : INotifyPropertyChanged, INotifyDataErrorInfo
    {
        private string _someText;

        public string SomeText
        {
            get { return _someText; }
            set
            {
                if (value != _someText)
                {
                    _someText = value;

                    OnPropertyChanged();

                    ClearPropertyValidationErrors();

                    if (_someText == "one")
                    {
                        AddPropertyValidationError("First message");
                    }
                    else if (_someText == "two")
                    {
                        AddPropertyValidationError("First message");
                        AddPropertyValidationError("Second message");
                    }
                }
            }
        }

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;

            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion

        #region INotifyDataErrorInfo

        private readonly Dictionary<string, HashSet<string>> _validationErrors =
            new Dictionary<string, HashSet<string>>(StringComparer.OrdinalIgnoreCase);

        public IEnumerable GetErrors(string propertyName)
        {
            if (propertyName != null && _validationErrors.ContainsKey(propertyName))
            {
                return _validationErrors[propertyName].AsEnumerable();
            }

            return Enumerable.Empty<string>();
        }

        public bool HasErrors
        {
            get { return _validationErrors.Any(kvp => kvp.Value.Any()); }
        }

        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        private void NotifyOfDataError(string propertyName)
        {
            EventHandler<DataErrorsChangedEventArgs> handler = ErrorsChanged;

            if (handler != null)
            {
                handler(this, new DataErrorsChangedEventArgs(propertyName));
            }
        }

        protected void AddPropertyValidationError(string message, [CallerMemberName] string propertyName = null)
        {
            AddValidationError(message, propertyName);
        }

        private void AddValidationError(string message, string propertyName)
        {
            if (propertyName == null)
            {
                return;
            }

            if (_validationErrors.ContainsKey(propertyName))
            {
                _validationErrors[propertyName].Add(message);
            }
            else
            {
                _validationErrors.Add(propertyName, new HashSet<string> { message });
            }

            NotifyOfDataError(propertyName);
        }

        protected void ClearAllValidationErrors()
        {
            _validationErrors.Clear();
            NotifyOfDataError(null);
        }

        protected void ClearPropertyValidationErrors([CallerMemberName] string propertyName = null)
        {
            ClearValidationErrors(propertyName);
        }

        private void ClearValidationErrors(string propertyName)
        {
            if (propertyName != null)
            {
                _validationErrors.Remove(propertyName);
                NotifyOfDataError(propertyName);
            }
        }

        #endregion
    }
#endif
}