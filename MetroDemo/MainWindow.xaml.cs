using System.Linq;
using System.Windows;
using System.Windows.Input;
using MahApps.Metro;

namespace MetroDemo
{
    public partial class MainWindow
    {
        public MainWindow()
        {
            DataContext = new MainWindowViewModel(Dispatcher);
            InitializeComponent();
        }

        private void ButtonClick(object sender, RoutedEventArgs e)
        {
            pb.IsIndeterminate = !pb.IsIndeterminate;
        }

        private void MiLightRed(object sender, RoutedEventArgs e)
        {
            ThemeManager.ChangeTheme(this, ThemeManager.DefaultAccents.First(a => a.Name == "Red"), Theme.Light);
        }

        private void MiDarkRed(object sender, RoutedEventArgs e)
        {
            ThemeManager.ChangeTheme(this, ThemeManager.DefaultAccents.First(a => a.Name == "Red"), Theme.Dark);
        }

        private void MiLightGreen(object sender, RoutedEventArgs e)
        {
            ThemeManager.ChangeTheme(this, ThemeManager.DefaultAccents.First(a => a.Name == "Green"), Theme.Light);
        }

        private void MiDarkGreen(object sender, RoutedEventArgs e)
        {
            ThemeManager.ChangeTheme(this, ThemeManager.DefaultAccents.First(a => a.Name == "Green"), Theme.Dark);
        }

        private void MiLightBlue(object sender, RoutedEventArgs e)
        {
            ThemeManager.ChangeTheme(this, ThemeManager.DefaultAccents.First(a => a.Name == "Blue"), Theme.Light);
        }

        private void MiDarkBlue(object sender, RoutedEventArgs e)
        {
            ThemeManager.ChangeTheme(this, ThemeManager.DefaultAccents.First(a => a.Name == "Blue"), Theme.Dark);
        }

        private void MiLightPurple(object sender, RoutedEventArgs e)
        {
            ThemeManager.ChangeTheme(this, ThemeManager.DefaultAccents.First(a => a.Name == "Purple"), Theme.Light);
        }

        private void MiDarkPurple(object sender, RoutedEventArgs e)
        {
            ThemeManager.ChangeTheme(this, ThemeManager.DefaultAccents.First(a => a.Name == "Purple"), Theme.Dark);
        }

        private void AppMiLightRed(object sender, RoutedEventArgs e)
        {
            ThemeManager.ChangeAppTheme(this, ThemeManager.DefaultAccents.First(a => a.Name == "Red"), Theme.Light);
        }

        private void AppMiDarkRed(object sender, RoutedEventArgs e)
        {
            ThemeManager.ChangeAppTheme(this, ThemeManager.DefaultAccents.First(a => a.Name == "Red"), Theme.Dark);
        }

        private void AppMiLightGreen(object sender, RoutedEventArgs e)
        {
            ThemeManager.ChangeAppTheme(this, ThemeManager.DefaultAccents.First(a => a.Name == "Green"), Theme.Light);
        }

        private void AppMiDarkGreen(object sender, RoutedEventArgs e)
        {
            ThemeManager.ChangeAppTheme(this, ThemeManager.DefaultAccents.First(a => a.Name == "Green"), Theme.Dark);
        }

        private void AppMiLightBlue(object sender, RoutedEventArgs e)
        {
            ThemeManager.ChangeAppTheme(this, ThemeManager.DefaultAccents.First(a => a.Name == "Blue"), Theme.Light);
        }

        private void AppMiDarkBlue(object sender, RoutedEventArgs e)
        {
            ThemeManager.ChangeAppTheme(this, ThemeManager.DefaultAccents.First(a => a.Name == "Blue"), Theme.Dark);
        }

        private void AppMiLightPurple(object sender, RoutedEventArgs e)
        {
            ThemeManager.ChangeAppTheme(this, ThemeManager.DefaultAccents.First(a => a.Name == "Purple"), Theme.Light);
        }

        private void AppMiDarkPurple(object sender, RoutedEventArgs e)
        {
            ThemeManager.ChangeAppTheme(this, ThemeManager.DefaultAccents.First(a => a.Name == "Purple"), Theme.Dark);
        }

        private void BtnPanoramaClick(object sender, RoutedEventArgs e)
        {
            //new ChildWindow().ShowDialog();
            new PanoramaDemo().Show();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            MahApps.Metro.Controls.MessageBox.DisplayMessage("Testing", "Testing the messagebox control", this);
        }
    }
}
