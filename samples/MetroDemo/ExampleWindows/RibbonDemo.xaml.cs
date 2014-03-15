using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Ribbon;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

using MahApps.Metro;
using MahApps.Metro.Controls;

namespace MetroDemo.ExampleWindows
{
    /// <summary>
    /// Interaction logic for RibbonDemo.xaml
    /// </summary>
    public partial class RibbonDemo : MetroWindow
    {
        public RibbonDemo()
        {
            var vm = new MainWindowViewModel();
            DataContext = vm;
            InitializeComponent();

            var theme = ThemeManager.DetectTheme(Application.Current);
            var accent = ThemeManager.DefaultAccents.First(x => x.Name == theme.Item2.Name);

            foreach (var accentColorItem in vm.AccentColors.Where(accentColorItem => accentColorItem.Name == accent.Name))
            {
                this.accentsComboBox.SelectedItem = accentColorItem;
            }
        }
        
        private void DarkTheme(object sender, RoutedEventArgs e)
        {
            var theme = ThemeManager.DetectTheme(Application.Current);
            ThemeManager.ChangeTheme(Application.Current, theme.Item2, Theme.Dark);
        }

        private void LightTheme(object sender, RoutedEventArgs e)
        {
            var theme = ThemeManager.DetectTheme(Application.Current);
            ThemeManager.ChangeTheme(Application.Current, theme.Item2, Theme.Light);
        }

        private void Accents_SelectionChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            RibbonGallery source = e.OriginalSource as RibbonGallery;
            if (source == null) return;
            var accentItem = (AccentColorMenuData)source.SelectedItem;

            var theme = ThemeManager.DetectTheme(Application.Current);
            var accent = ThemeManager.DefaultAccents.First(x => x.Name == accentItem.Name);
            ThemeManager.ChangeTheme(Application.Current, accent, theme.Item1);
        }
    }
}
