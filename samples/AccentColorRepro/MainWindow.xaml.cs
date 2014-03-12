using System.Linq;
using System.Windows;
using System.Windows.Media;
using MahApps.Metro;

namespace AccentColorRepro
{
    public partial class MainWindow
    {
        private Theme currentTheme = Theme.Light;
        private Accent currentAccent = ThemeManager.DefaultAccents.First(x => x.Name == "Blue");

        public MainWindow()
        {
            InitializeComponent();
            ThemeManager.ChangeTheme(Application.Current, currentAccent, currentTheme);
        }

        private void DarkButtonClick(object sender, RoutedEventArgs e)
        {
            currentTheme = Theme.Dark;
            ThemeManager.ChangeTheme(Application.Current, currentAccent, currentTheme);
        }

        private void LightButtonClick(object sender, RoutedEventArgs e)
        {
            currentTheme = Theme.Light;
            ThemeManager.ChangeTheme(Application.Current, currentAccent, currentTheme);
        }

        private void BlueButtonClick(object sender, RoutedEventArgs e)
        {
            currentAccent = ThemeManager.DefaultAccents.First(x => x.Name == "Blue");
            ThemeManager.ChangeTheme(Application.Current, currentAccent, currentTheme);
        }

        private void RedButtonClick(object sender, RoutedEventArgs e)
        {
            currentAccent = ThemeManager.DefaultAccents.First(x => x.Name == "Red");
            ThemeManager.ChangeTheme(Application.Current, currentAccent, currentTheme);
        }

        private void GreenButtonClick(object sender, RoutedEventArgs e)
        {
            currentAccent = ThemeManager.DefaultAccents.First(x => x.Name == "Green");
            ThemeManager.ChangeTheme(Application.Current, currentAccent, currentTheme);
        }
    }
}
