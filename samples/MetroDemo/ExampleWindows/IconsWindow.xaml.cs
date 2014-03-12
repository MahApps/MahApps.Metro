using System;
using System.Collections;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using MahApps.Metro.Controls;

namespace MetroDemo.ExampleWindows
{
    /// <summary>
    /// Interaction logic for IconsWindow.xaml
    /// </summary>
    public partial class IconsWindow : MetroWindow
    {
        public IconsWindow()
        {
            InitializeComponent();
            this.Loaded += this.OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            var dict = new ResourceDictionary {Source = new Uri("pack://application:,,,/MahApps.Metro.Resources;component/Icons.xaml")};
            var foundIcons = dict
                .OfType<DictionaryEntry>()
                .Where(de => de.Value is Canvas)
                .Select(de => new MetroIcon((string)de.Key, (Canvas)de.Value))
                .OrderBy(mi => mi.Name)
                .ToList();
            this.IconsListBox.ItemsSource = foundIcons;
        }

        public sealed class MetroIcon
        {
            public MetroIcon(string name, Visual visual)
            {
                this.Name = name;
                this.Visual = visual;
            }

            public string Name { get; private set; }
            public Visual Visual { get; set; }
        }

    }
}
