using System;
using System.Windows.Controls;
using System.Windows.Threading;

namespace MetroDemo
{
    public partial class MainWindow
    {
        public MainWindow()
        {
            DataContext = new MainWindowViewModel();
            InitializeComponent();
            var t = new DispatcherTimer(TimeSpan.FromSeconds(2), DispatcherPriority.Normal, (sender, args) => this.TransitionTick(), this.Dispatcher);
        }

        private void TransitionTick()
        {
            var dateTime = DateTime.Now;
            transitioning.Content = new TextBlock {Text = "Transitioning Content! " + dateTime, SnapsToDevicePixels = true};
            customTransitioning.Content = new TextBlock {Text = "Custom transistion! " + dateTime, SnapsToDevicePixels = true};
        }
    }
}
