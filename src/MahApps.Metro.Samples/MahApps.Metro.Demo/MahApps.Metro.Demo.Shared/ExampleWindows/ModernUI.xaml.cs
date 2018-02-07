using System;
using MahApps.Metro;
using MahApps.Metro.Controls;

namespace MetroDemo.ExampleWindows
{
    public partial class ModernUI : MetroWindow
    {
        public ModernUI()
        {
            this.InitializeComponent();

            ThemeManager.AddAppTheme("ModernUI.Light", new Uri("pack://application:,,,/MahApps.Metro;component/Styles/ModernUI/ModernUI.Light.xaml", UriKind.RelativeOrAbsolute));
            ThemeManager.AddAppTheme("ModernUI.Dark", new Uri("pack://application:,,,/MahApps.Metro;component/Styles/ModernUI/ModernUI.Dark.xaml", UriKind.RelativeOrAbsolute));
            ThemeManager.IsThemeChanged += ThemeManager_IsThemeChanged;
        }

        private void ThemeManager_IsThemeChanged(object sender, OnThemeChangedEventArgs e)
        {
            ThemeManager.IsThemeChanged -= ThemeManager_IsThemeChanged;

            var newTheme = e.AppTheme.Name.Contains("Light") ? ThemeManager.GetAppTheme("ModernUI.Light") : ThemeManager.GetAppTheme("ModernUI.Dark");
            ThemeManager.ChangeAppStyle(this, e.Accent, newTheme);

            ThemeManager.IsThemeChanged += ThemeManager_IsThemeChanged;
        }
    }
}