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
            ThemeManager.Current.ThemeSyncMode = ThemeSyncMode.SyncWithAppMode;
            ThemeManager.Current.SyncTheme();

            ThemeManager.Current.ThemeChanged += HandleThemeChanged;

#pragma warning disable CS0618 // Type or member is obsolete
            AppModeHelper.SyncAppMode();
#pragma warning restore CS0618 // Type or member is obsolete

            base.OnStartup(e);

            return;

            void HandleThemeChanged(object? sender, ThemeChangedEventArgs themeChangedEventArgs)
            {
#pragma warning disable CS0618 // Type or member is obsolete
                AppModeHelper.SyncAppMode();
#pragma warning restore CS0618 // Type or member is obsolete
            }
        }
    }
}