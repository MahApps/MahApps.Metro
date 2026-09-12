// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows;
using System.Windows.Controls;
using ControlzEx.Theming;

namespace MetroDemo.ExampleViews
{
    /// <summary>
    /// Interaction logic for HeaderExamples.xaml
    /// </summary>
    public partial class HeaderExamples : UserControl
    {
        public HeaderExamples()
        {
            this.InitializeComponent();

            this.Loaded += this.OnLoaded;
            this.Unloaded += this.OnUnloaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            ThemeManager.Current.ThemeChanged -= this.OnThemeChanged;
            ThemeManager.Current.ThemeChanged += this.OnThemeChanged;

            this.ReadTheBrushes();
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            ThemeManager.Current.ThemeChanged -= this.OnThemeChanged;
        }

        private void OnThemeChanged(object? sender, ThemeChangedEventArgs e)
        {
            this.ReadTheBrushes();
        }

        /// <summary>
        /// The entries of the two colour boxes stand for keys, not for colours, and another theme puts
        /// another brush behind a key. Asking from here rather than from the settings, because looking a
        /// resource up is a question to the tree this control hangs in.
        /// </summary>
        private void ReadTheBrushes()
        {
            if (this.DataContext is MainWindowViewModel viewModel)
            {
                viewModel.HeaderSettings.ReadTheBrushesFromTheTheme(this.TryFindResource);
            }
        }
    }
}
