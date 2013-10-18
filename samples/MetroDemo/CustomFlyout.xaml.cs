using System.Collections.Generic;
using System.Windows.Input;
using MahApps.Metro.Controls;
using MetroDemo.Models;
using System;
using System.ComponentModel;

namespace MetroDemo
{
    /// <summary>
    /// Interaction logic for CustomFlyout.xaml
    /// </summary>
    public partial class CustomFlyout : Flyout, INotifyPropertyChanged
    {
        public List<Artist> Artists { get; set; }

        public CustomFlyout()
        {
            Artists = SampleData.Artists;
            InitializeComponent();
        }

        private bool canCloseFlyout;

        public bool CanCloseFlyout
        {
            get { return this.canCloseFlyout; }
            set
            {
                if (Equals(value, canCloseFlyout)) {
                    return;
                }
                canCloseFlyout = value;
                RaisePropertyChanged("CanCloseFlyout");
            }
        }

        private ICommand closeCmd;

        public ICommand CloseCmd
        {
            get
            {
                return this.closeCmd ?? (closeCmd = new SimpleCommand {
                    CanExecuteDelegate = x => this.CanCloseFlyout,
                    ExecuteDelegate = x => this.IsOpen = false
                });
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void RaisePropertyChanged(string propertyName)
        {
            if (PropertyChanged != null) {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }

    public class SimpleCommand : ICommand
    {
        public Predicate<object> CanExecuteDelegate { get; set; }
        public Action<object> ExecuteDelegate { get; set; }

        #region ICommand Members

        public bool CanExecute(object parameter)
        {
            if (CanExecuteDelegate != null)
                return CanExecuteDelegate(parameter);
            return true;// if there is no can execute default to true
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public void Execute(object parameter)
        {
            if (ExecuteDelegate != null)
                ExecuteDelegate(parameter);
        }

        #endregion
    }
}
