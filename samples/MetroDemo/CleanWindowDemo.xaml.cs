using MahApps.Metro.Controls.Dialogs;

namespace MetroDemo
{
    /// <summary>
    /// Interaction logic for CleanWindowDemo.xaml
    /// </summary>
    public partial class CleanWindowDemo
    {
        public CleanWindowDemo()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            settingsFlyout.IsOpen = !settingsFlyout.IsOpen;
        }

        private void Button_Click_1(object sender, System.Windows.RoutedEventArgs e)
        {
            this.ShowMessageAsync("Something", "Something should be displayed here.");
        }
    }
}
