using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using MahApps.Metro.Controls;

namespace MetroDemo
{
    /// <summary>
    /// Interaction logic for IconsWindow.xaml
    /// </summary>
    public partial class IconsWindow : MetroWindow
    {
        public IconsWindow()
        {
            InitializeComponent();
            Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            var dict = Resources.MergedDictionaries.First();
            List<MetroIcon> foundIcons = new List<MetroIcon>(dict.Count);
            foreach (DictionaryEntry resource in dict)
            {
                Canvas visual = resource.Value as Canvas;
                if (visual != null)
                {
                    foundIcons.Add(new MetroIcon((string)resource.Key, visual));
                }
            }

            foundIcons.Sort((a, b) => string.Compare(a.Name, b.Name, StringComparison.InvariantCulture));
            IconsListBox.ItemsSource = foundIcons;
        }

        public sealed class MetroIcon
        {
            public MetroIcon(string name, Visual visual)
            {
                Name = name;
                Visual = visual;
            }

            public string Name { get; private set; }
            public Visual Visual { get; set; }
        }

    }
}
