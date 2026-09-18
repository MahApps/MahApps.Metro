// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows;
using ControlzEx.Theming;
using ShowMeTheXAML;

namespace MahApps.Metro.Gallery
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        /// <inheritdoc />
        protected override void OnStartup(StartupEventArgs e)
        {
            // Hands the XamlDisplay controls the markup that the build wrote into this assembly.
            // Without it every card comes up with an empty code block.
            XamlDisplay.Init();

            base.OnStartup(e);

            ThemeManager.Current.ThemeSyncMode = ThemeSyncMode.SyncAll;
            ThemeManager.Current.SyncTheme();
        }
    }
}
