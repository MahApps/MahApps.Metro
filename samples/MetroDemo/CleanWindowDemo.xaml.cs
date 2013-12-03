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
    }
}
