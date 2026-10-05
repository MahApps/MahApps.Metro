// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using ControlzEx.Theming;
using ICSharpCode.AvalonEdit;
using MahApps.Metro.Controls;
using MahApps.Metro.Gallery.Core;
using MahApps.Metro.IconPacks;
using Microsoft.Xaml.Behaviors;
using ShowMeTheXAML;

namespace MahApps.Metro.Gallery.Pages
{
    /// <summary>
    /// Interaction logic for SettingsPage.xaml
    /// </summary>
    public partial class SettingsPage : UserControl
    {
        private const string UseWindows = "Use Windows setting";

        private bool isFilling;

        public SettingsPage()
        {
            this.InitializeComponent();

            this.BaseColors.ItemsSource = ThemeManager.Current.BaseColors.Concat(new[] { UseWindows }).ToList();

            // one swatch per accent that ships, in the order the themes come in
            this.Accent.CustomColorPalette01ItemsSource = ThemeManager.Current.Themes
                                                                      .Where(t => !t.IsHighContrast && !t.IsRuntimeGenerated && t.BaseColorScheme == ThemeManager.BaseColorLight)
                                                                      .Select(t => t.PrimaryAccentColor)
                                                                      .Distinct()
                                                                      .ToList();

            this.Version.Text = GalleryLink.VersionOf(typeof(MetroWindow).Assembly);

            this.Dependencies.ItemsSource = new[]
                                            {
                                                GalleryLink.For("MahApps.Metro", typeof(MetroWindow), "https://github.com/MahApps/MahApps.Metro"),
                                                GalleryLink.For("ControlzEx", typeof(ThemeManager), "https://github.com/ControlzEx/ControlzEx"),
                                                GalleryLink.For("Microsoft.Xaml.Behaviors.Wpf", typeof(Interaction), "https://github.com/microsoft/XamlBehaviorsWpf"),
                                                GalleryLink.For("MahApps.Metro.IconPacks", typeof(PackIconMaterial), "https://github.com/MahApps/MahApps.Metro.IconPacks"),
                                                GalleryLink.For("ShowMeTheXAML", typeof(XamlDisplay), "https://github.com/Keboo/ShowMeTheXAML"),
                                                GalleryLink.For("AvalonEdit", typeof(TextEditor), "https://github.com/icsharpcode/AvalonEdit")
                                            };

            this.Fill();

            ThemeManager.Current.ThemeChanged += (_, _) => this.Fill();
        }

        /// <summary>
        /// Puts what is currently on into the controls, without taking that for somebody picking it.
        /// </summary>
        private void Fill()
        {
            var theme = ThemeManager.Current.DetectTheme(Application.Current);
            var mode = ThemeManager.Current.ThemeSyncMode;

            this.isFilling = true;
            try
            {
                this.BaseColors.SelectedItem = mode.HasFlag(ThemeSyncMode.SyncWithAppMode) ? UseWindows : theme?.BaseColorScheme;
                this.Accent.SelectedColor = theme?.PrimaryAccentColor;
                this.FollowWindowsAccent.IsOn = mode.HasFlag(ThemeSyncMode.SyncWithAccent);
            }
            finally
            {
                this.isFilling = false;
            }
        }

        private void OnBaseColorChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.isFilling || this.BaseColors.SelectedItem is not string baseColor)
            {
                return;
            }

            if (baseColor == UseWindows)
            {
                Follow(ThemeSyncMode.SyncWithAppMode);
                return;
            }

            // picking one by hand means Windows no longer has the last word, otherwise the next
            // change out there would put this one back
            LetGo(ThemeSyncMode.SyncWithAppMode);

            var theme = ThemeManager.Current.DetectTheme(Application.Current);
            if (theme is not null)
            {
                Apply(baseColor, theme.PrimaryAccentColor);
            }
        }

        private void OnAccentChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            // the picker also reports the colour it was handed by Fill, once its template is on,
            // and that one is no pick, so only a colour other than the one on counts
            var theme = ThemeManager.Current.DetectTheme(Application.Current);
            if (this.isFilling || e.NewValue is not { } accent || accent == theme?.PrimaryAccentColor)
            {
                return;
            }

            LetGo(ThemeSyncMode.SyncWithAccent);
            Apply(theme?.BaseColorScheme ?? ThemeManager.BaseColorLight, accent);
        }

        private void OnFollowWindowsAccentToggled(object sender, RoutedEventArgs e)
        {
            if (this.isFilling)
            {
                return;
            }

            if (this.FollowWindowsAccent.IsOn)
            {
                Follow(ThemeSyncMode.SyncWithAccent);
            }
            else
            {
                LetGo(ThemeSyncMode.SyncWithAccent);
            }
        }

        private static void Follow(ThemeSyncMode part)
        {
            ThemeManager.Current.ThemeSyncMode |= part | ThemeSyncMode.SyncWithHighContrast;
            ThemeManager.Current.SyncTheme();
        }

        private static void LetGo(ThemeSyncMode part)
        {
            var mode = ThemeManager.Current.ThemeSyncMode & ~part;

            // high contrast only goes along with something else that follows Windows
            if (mode == ThemeSyncMode.SyncWithHighContrast)
            {
                mode = ThemeSyncMode.DoNotSync;
            }

            ThemeManager.Current.ThemeSyncMode = mode;
        }

        /// <summary>
        /// Puts on the theme for the base colour and the accent, the one that ships where there is one,
        /// so the accents under MahApps keep their names, and one made up at run time otherwise.
        /// </summary>
        private static void Apply(string baseColor, Color accent)
        {
            var theme = ThemeManager.Current.Themes.FirstOrDefault(t => !t.IsHighContrast
                                                                        && !t.IsRuntimeGenerated
                                                                        && t.BaseColorScheme == baseColor
                                                                        && t.PrimaryAccentColor == accent)
                        ?? RuntimeThemeGenerator.Current.GenerateRuntimeTheme(baseColor, accent);

            if (theme is not null)
            {
                ThemeManager.Current.ChangeTheme(Application.Current, theme);
            }
        }
    }
}
