// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using ControlzEx.Theming;

namespace MahApps.Metro.Gallery.Pages
{
    /// <summary>
    /// Interaction logic for SettingsPage.xaml
    /// </summary>
    public partial class SettingsPage : UserControl
    {
        private bool isFilling;

        public SettingsPage()
        {
            this.InitializeComponent();

            this.BaseColors.ItemsSource = ThemeManager.Current.BaseColors;
            this.ColorSchemes.ItemsSource = ThemeManager.Current.ColorSchemes;

            this.Version.Text = "MahApps.Metro "
                                + typeof(MahApps.Metro.Controls.MetroWindow).Assembly
                                                                            .GetCustomAttribute<AssemblyInformationalVersionAttribute>()
                                                                            ?.InformationalVersion;

            this.Fill();

            ThemeManager.Current.ThemeChanged += (_, _) => this.Fill();
        }

        /// <summary>
        /// Puts what is currently on into the boxes, without taking that for somebody picking it.
        /// </summary>
        private void Fill()
        {
            var theme = ThemeManager.Current.DetectTheme(Application.Current);

            this.isFilling = true;
            try
            {
                this.FollowWindows.IsOn = ThemeManager.Current.ThemeSyncMode != ThemeSyncMode.DoNotSync;
                this.BaseColors.SelectedItem = theme?.BaseColorScheme;
                this.ColorSchemes.SelectedItem = theme?.ColorScheme;
            }
            finally
            {
                this.isFilling = false;
            }
        }

        private void OnFollowWindowsToggled(object sender, RoutedEventArgs e)
        {
            if (this.isFilling)
            {
                return;
            }

            if (this.FollowWindows.IsOn)
            {
                ThemeManager.Current.ThemeSyncMode = ThemeSyncMode.SyncAll;
                ThemeManager.Current.SyncTheme();
            }
            else
            {
                ThemeManager.Current.ThemeSyncMode = ThemeSyncMode.DoNotSync;
            }
        }

        private void OnBaseColorChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.isFilling || this.BaseColors.SelectedItem is not string baseColor)
            {
                return;
            }

            // picking one by hand means Windows no longer has the last word, otherwise the next
            // change out there would put this one back
            ThemeManager.Current.ThemeSyncMode = ThemeSyncMode.DoNotSync;
            ThemeManager.Current.ChangeThemeBaseColor(Application.Current, baseColor);
        }

        private void OnColorSchemeChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.isFilling || this.ColorSchemes.SelectedItem is not string colorScheme)
            {
                return;
            }

            ThemeManager.Current.ThemeSyncMode = ThemeSyncMode.DoNotSync;
            ThemeManager.Current.ChangeThemeColorScheme(Application.Current, colorScheme);
        }
    }
}
