using System.Linq;
using System.Windows;
using System.Windows.Media;
using MahApps.Metro;

namespace AccentColorRepro
{
    public partial class MainWindow
    {
        private string lastUsedAccent;

        public MainWindow()
        {
            InitializeComponent();
            lastUsedAccent = "Blue";
            ThemeManager.ChangeTheme(this, ThemeManager.DefaultAccents.First(x => x.Name == lastUsedAccent), Theme.Light);
            this.GlowBrush = (SolidColorBrush)(new BrushConverter().ConvertFrom("#CC119EDA"));
        }

        private void DarkButtonClick(object sender, RoutedEventArgs e)
        {
            ThemeManager.ChangeTheme(this, ThemeManager.DefaultAccents.First(x => x.Name == lastUsedAccent), Theme.Dark);
        }

        private void LightButtonClick(object sender, RoutedEventArgs e)
        {
            ThemeManager.ChangeTheme(this, ThemeManager.DefaultAccents.First(x => x.Name == lastUsedAccent), Theme.Light);
        }

        private void BlueButtonClick(object sender, RoutedEventArgs e)
        {
            lastUsedAccent = "Blue";
            ThemeManager.ChangeTheme(this, ThemeManager.DefaultAccents.First(x => x.Name == lastUsedAccent), ThemeManager.ThemeIsDark ? Theme.Dark : Theme.Light);
            this.GlowBrush = (SolidColorBrush)(new BrushConverter().ConvertFrom("#CC119EDA"));
        }

        private void RedButtonClick(object sender, RoutedEventArgs e)
        {
            lastUsedAccent = "Red";
            ThemeManager.ChangeTheme(this, ThemeManager.DefaultAccents.First(x => x.Name == lastUsedAccent), ThemeManager.ThemeIsDark ? Theme.Dark : Theme.Light);
            this.GlowBrush = (SolidColorBrush)(new BrushConverter().ConvertFrom("#CCcb0000"));
        }

        private void GreenButtonClick(object sender, RoutedEventArgs e)
        {
            lastUsedAccent = "Green";
            ThemeManager.ChangeTheme(this, ThemeManager.DefaultAccents.First(x => x.Name == lastUsedAccent), ThemeManager.ThemeIsDark ? Theme.Dark : Theme.Light);
            this.GlowBrush = (SolidColorBrush)(new BrushConverter().ConvertFrom("#CC48cb00"));
        }
    }
}
