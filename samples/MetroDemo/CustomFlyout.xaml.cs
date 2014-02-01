using System.Collections.Generic;
using System.Windows.Input;
using MahApps.Metro.Controls;
using MetroDemo.Models;
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
}
