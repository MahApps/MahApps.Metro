using System.ComponentModel;

namespace MetroDemo.ViewModels
{
    public class ValidationExampleViewModel : INotifyPropertyChanged, IDataErrorInfo
    {
        public ValidationExampleViewModel() {
        }

        private int? _integerProperty;

        public int? IntegerProperty {
            get { return this._integerProperty; }
            set {
                if (Equals(value, this._integerProperty)) {
                    return;
                }
                this._integerProperty = value;
                this.OnPropertyChanged("IntegerProperty");
            }
        }

        private int? _integerGreater5Property;

        public int? IntegerGreater5Property {
            get { return this._integerGreater5Property; }
            set {
                if (Equals(value, this._integerGreater5Property)) {
                    return;
                }
                this._integerGreater5Property = value;
                this.OnPropertyChanged("IntegerGreater5Property");
            }
        }

        private string comboBoxProperty;

        public string ComboBoxProperty {
            get { return this.comboBoxProperty; }
            set {
                if (Equals(value, this.comboBoxProperty)) {
                    return;
                }
                this.comboBoxProperty = value;
                this.OnPropertyChanged("ComboBoxProperty");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName) {
            var handler = this.PropertyChanged;
            if (handler != null) {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public string this[string columnName] {
            get {
                if (columnName.Equals("IntegerGreater5Property")) {
                    if (this.IntegerGreater5Property <= 5) {
                        return "This number is not greater 5!";
                    }
                }
                if (columnName.Equals("ComboBoxProperty")) {
                    if (string.IsNullOrWhiteSpace(this.ComboBoxProperty)) {
                        return "Please choose a valid item!";
                    }
                }
                return null;
            }
        }

        public string Error { get; private set; }
    }
}