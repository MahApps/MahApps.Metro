using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using JetBrains.Annotations;

namespace MetroDemo.Models
{
    public class CustomDialogExampleContent : INotifyPropertyChanged
    {
        private readonly ICommand _closeCommand;

        private string _firstName;
        private string _lastName;

        public CustomDialogExampleContent(Action<CustomDialogExampleContent> closeHandler)
        {
            _closeCommand = new SimpleCommand
            {
                ExecuteDelegate = o => closeHandler(this)
            };
        }

        public string FirstName
        {
            get { return _firstName; }
            set
            {
                _firstName = value;
                OnPropertyChanged();
            }
        }

        public string LastName
        {
            get { return _lastName; }
            set
            {
                _lastName = value;
                OnPropertyChanged();
            }
        }

        public ICommand CloseCommand
        {
            get { return _closeCommand; }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
