// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace MahApps.Metro.Tests
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Media;
    using ControlzEx.Theming;
    using MahApps.Metro.Controls;
    using MahApps.Metro.Tests.TestHelpers;
    using Xunit;

    public class ThemeManagerTest : AutomationTestBase
    {
        public override void Dispose()
        {
            Application.Current.Dispatcher.Invoke(ThemeManager.Current.ClearThemes);

            base.Dispose();
        }

        [Fact]
        [DisplayTestMethodName]
        public async Task GetThemes()
        {
            await TestHost.SwitchToAppThread();

            var expectedThemes = new[]
                                 {
                                     "Amber (Dark)",
                                     "Amber (Light)",
                                     "Blue (Dark)",
                                     "Blue (Light)",
                                     "Brown (Dark)",
                                     "Brown (Light)",
                                     "Cobalt (Dark)",
                                     "Cobalt (Light)",
                                     "Crimson (Dark)",
                                     "Crimson (Light)",
                                     "Cyan (Dark)",
                                     "Cyan (Light)",
                                     "Emerald (Dark)",
                                     "Emerald (Light)",
                                     "Green (Dark)",
                                     "Green (Light)",
                                     "Indigo (Dark)",
                                     "Indigo (Light)",
                                     "Lime (Dark)",
                                     "Lime (Light)",
                                     "Magenta (Dark)",
                                     "Magenta (Light)",
                                     "Mauve (Dark)",
                                     "Mauve (Light)",
                                     "Olive (Dark)",
                                     "Olive (Light)",
                                     "Orange (Dark)",
                                     "Orange (Light)",
                                     "Pink (Dark)",
                                     "Pink (Light)",
                                     "Purple (Dark)",
                                     "Purple (Light)",
                                     "Red (Dark)",
                                     "Red (Light)",
                                     "Sienna (Dark)",
                                     "Sienna (Light)",
                                     "Steel (Dark)",
                                     "Steel (Light)",
                                     "Taupe (Dark)",
                                     "Taupe (Light)",
                                     "Teal (Dark)",
                                     "Teal (Light)",
                                     "Violet (Dark)",
                                     "Violet (Light)",
                                     "Yellow (Dark)",
                                     "Yellow (Light)"
                                 };
            Assert.Equal(expectedThemes, CollectionViewSource.GetDefaultView(ThemeManager.Current.Themes).Cast<Theme>().Select(x => x.DisplayName).ToList());
        }

        [Fact]
        [DisplayTestMethodName]
        public async Task CreateDynamicAccentWithColorAndChangeBaseColorScheme()
        {
            await TestHost.SwitchToAppThread();

            var applicationTheme = ThemeManager.Current.DetectTheme(Application.Current);

            var ex = Record.Exception(() => ThemeManager.Current.AddTheme(RuntimeThemeGenerator.Current.GenerateRuntimeTheme("Dark", Colors.Red)));
            Assert.Null(ex);
            ex = Record.Exception(() => ThemeManager.Current.AddTheme(ThemeManager.Current.ChangeTheme(Application.Current, RuntimeThemeGenerator.Current.GenerateRuntimeTheme("Light", Colors.Red))));
            Assert.Null(ex);

            var detected = ThemeManager.Current.DetectTheme(Application.Current);
            Assert.NotNull(detected);
            Assert.Equal(Colors.Red.ToString(), detected.ColorScheme);

            var newTheme = ThemeManager.Current.ChangeThemeBaseColor(Application.Current, "Dark");
            Assert.NotNull(newTheme);

            newTheme = ThemeManager.Current.ChangeThemeBaseColor(Application.Current, "Light");
            Assert.NotNull(newTheme);

            ThemeManager.Current.ChangeTheme(Application.Current, applicationTheme);
        }
    }
}