using System.Windows;

namespace MetroDemo
{
    public partial class PanoramaDemo
    {
        public PanoramaDemo()
        {
            InitializeComponent();
            Loaded += MainWindowLoaded;
        }

        void MainWindowLoaded(object sender, RoutedEventArgs e)
        {
            DataContext = new MainWindowViewModel();
        }
    }
}
