// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows;
using ControlzEx.Theming;

namespace MetroDemo
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        /// <inheritdoc />
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            ThemeManager.Current.ThemeSyncMode = ThemeSyncMode.SyncAll;

            ThemeManager.Current.SyncTheme();

            ThemeManager.Current.ThemeChanged += CurrentOnThemeChanged;

#pragma warning disable CS0618 // Type or member is obsolete
            AppModeHelper.SyncAppMode();

            void CurrentOnThemeChanged(object? sender, ThemeChangedEventArgs themeChangedEventArgs)
            {
                AppModeHelper.SyncAppMode();
            }
#pragma warning restore CS0618 // Type or member is obsolete
        }
    }
}