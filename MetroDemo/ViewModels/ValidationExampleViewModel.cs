using System.ComponentModel;

namespace MetroDemo.ViewModels
{
    public class ValidationExampleViewModel : INotifyPropertyChanged, IDataErrorInfo
    {
        int? _integerProperty;

        public int? IntegerProperty
        {
            get { return _integerProperty; }
            set
            {
                if (Equals(value, _integerProperty))
                {
                    return;
                }
                _integerProperty = value;
                OnPropertyChanged("IntegerProperty");
            }
        }

        int? _integerGreater5Property;

        public int? IntegerGreater5Property
        {
            get { return _integerGreater5Property; }
            set
            {
                if (Equals(value, _integerGreater5Property))
                {
                    return;
                }
                _integerGreater5Property = value;
                OnPropertyChanged("IntegerGreater5Property");
            }
        }

        private string _comboBoxProperty;

        public string ComboBoxProperty
        {
            get { return _comboBoxProperty; }
            set
            {
                if (Equals(value, _comboBoxProperty))
                {
                    return;
                }
                _comboBoxProperty = value;
                OnPropertyChanged("ComboBoxProperty");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public string this[string columnName]
        {
            get
            {
                if (columnName.Equals("IntegerGreater5Property"))
                {
                    if (IntegerGreater5Property <= 5)
                    {
                        return "This number is not greater 5!";
                    }
                }
                if (columnName.Equals("ComboBoxProperty"))
                {
                    if (string.IsNullOrWhiteSpace(ComboBoxProperty))
                    {
                        return "Please choose a valid item!";
                    }
                }
                return null;
            }
        }

        public string Error { get; private set; }
    }
}