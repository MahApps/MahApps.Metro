using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Input;
using MahApps.Metro.Controls;
using MetroDemo.Models;

namespace MetroDemo.ExampleWindows
{
    /// <summary>
    /// Interaction logic for CustomFlyout.xaml
    /// </summary>
    public partial class CustomFlyout : Flyout, INotifyPropertyChanged
    {
        public List<Artist> Artists { get; set; }

        public CustomFlyout()
        {
            this.Artists = SampleData.Artists;
            InitializeComponent();
        }

        private bool canCloseFlyout;

        public bool CanCloseFlyout
        {
            get { return this.canCloseFlyout; }
            set
            {
                if (Equals(value, this.canCloseFlyout)) {
                    return;
                }
                this.canCloseFlyout = value;
                this.RaisePropertyChanged("CanCloseFlyout");
            }
        }

        private ICommand closeCmd;

        public ICommand CloseCmd
        {
            get
            {
                return this.closeCmd ?? (this.closeCmd = new SimpleCommand {
                    CanExecuteDelegate = x => this.CanCloseFlyout,
                    ExecuteDelegate = x => this.IsOpen = false
                });
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void RaisePropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null) {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
