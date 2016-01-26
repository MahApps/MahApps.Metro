using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using MahApps.Metro;
using MahApps.Metro.Controls;

namespace MetroDemo.ExampleWindows
{
    /// <summary>
    /// Interaction logic for IconPacksWindow.xaml
    /// </summary>
    public partial class IconPacksWindow : MetroWindow
    {
        public static readonly DependencyProperty MaterialKindsProperty = DependencyProperty.Register("MaterialKinds", typeof(IEnumerable<PackIconMaterialKind>), typeof(IconPacksWindow), new PropertyMetadata(default(IEnumerable<PackIconMaterialKind>)));

        public IEnumerable<PackIconMaterialKind> MaterialKinds
        {
            get { return (IEnumerable<PackIconMaterialKind>)GetValue(MaterialKindsProperty); }
            set { SetValue(MaterialKindsProperty, value); }
        }

        public static readonly DependencyProperty ModernKindsProperty = DependencyProperty.Register("ModernKinds", typeof(IEnumerable<PackIconModernKind>), typeof(IconPacksWindow), new PropertyMetadata(default(IEnumerable<PackIconModernKind>)));

        public IEnumerable<PackIconModernKind> ModernKinds
        {
            get { return (IEnumerable<PackIconModernKind>)GetValue(ModernKindsProperty); }
            set { SetValue(ModernKindsProperty, value); }
        }

        public IconPacksWindow()
        {
            DataContext = this;
            InitializeComponent();
            this.Loaded += IconPacksWindow_Loaded;
        }

        private void IconPacksWindow_Loaded(object sender, RoutedEventArgs e)
        {
            MaterialKinds = new Lazy<IEnumerable<PackIconMaterialKind>>(
                () => Enum.GetValues(typeof(PackIconMaterialKind)).OfType<PackIconMaterialKind>()
                          .OrderBy(k => k.ToString(), StringComparer.InvariantCultureIgnoreCase).ToList()
                ).Value;

            ModernKinds = new Lazy<IEnumerable<PackIconModernKind>>(
                () => Enum.GetValues(typeof(PackIconModernKind)).OfType<PackIconModernKind>()
                          .OrderBy(k => k.ToString(), StringComparer.InvariantCultureIgnoreCase).ToList()
                ).Value;
        }

        private void HyperlinkMaterialOnClick(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://materialdesignicons.com/");
        }

        private void HyperlinkModernOnClick(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("http://modernuiicons.com/");
        }

        private void TextBoxOnGotFocus(object sender, RoutedEventArgs e)
        {
            var textBox = (TextBox)sender;
            textBox.Dispatcher.BeginInvoke(new Action(textBox.SelectAll));
        }
    }
}