using System.Linq;
using System.Windows;
using MahApps.Metro;

namespace AccentColorRepro
{
    public partial class MainWindow
    {
        private AppTheme currentTheme = ThemeManager.DefaultAppThemes.First(x => x.Name == "BaseLight");
        private Accent currentAccent = ThemeManager.DefaultAccents.First(x => x.Name == "Blue");

        public MainWindow()
        {
            InitializeComponent();
            ThemeManager.ChangeAppStyle(Application.Current, currentAccent, currentTheme);
        }

        private void DarkButtonClick(object sender, RoutedEventArgs e)
        {
            currentTheme = ThemeManager.DefaultAppThemes.First(x => x.Name == "BaseDark");
            ThemeManager.ChangeAppStyle(Application.Current, currentAccent, currentTheme);
        }

        private void LightButtonClick(object sender, RoutedEventArgs e)
        {
            currentTheme = ThemeManager.DefaultAppThemes.First(x => x.Name == "BaseLight");
            ThemeManager.ChangeAppStyle(Application.Current, currentAccent, currentTheme);
        }

        private void BlueButtonClick(object sender, RoutedEventArgs e)
        {
            currentAccent = ThemeManager.DefaultAccents.First(x => x.Name == "Blue");
            ThemeManager.ChangeAppStyle(Application.Current, currentAccent, currentTheme);
        }

        private void RedButtonClick(object sender, RoutedEventArgs e)
        {
            currentAccent = ThemeManager.DefaultAccents.First(x => x.Name == "Red");
            ThemeManager.ChangeAppStyle(Application.Current, currentAccent, currentTheme);
        }

        private void GreenButtonClick(object sender, RoutedEventArgs e)
        {
            currentAccent = ThemeManager.DefaultAccents.First(x => x.Name == "Green");
            ThemeManager.ChangeAppStyle(Application.Current, currentAccent, currentTheme);
        }
    }
}
