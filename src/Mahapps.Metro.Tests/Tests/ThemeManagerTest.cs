// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using ControlzEx.Theming;
using NUnit.Framework;

namespace MahApps.Metro.Tests.Tests
{
    [TestFixture]
    public class ThemeManagerTest
    {
        [SetUp]
        public void SetUp()
        {
            ThemeManager.Current.ClearThemes();
        }

        [OneTimeTearDown]
        public void Dispose()
        {
            ThemeManager.Current.ClearThemes();
        }

        [Test]
        public Task GetThemes()
        {
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

            var actual = CollectionViewSource.GetDefaultView(ThemeManager.Current.Themes).Cast<Theme>().Select(x => x.DisplayName).ToList();
            Assert.That(actual, Is.EquivalentTo(expectedThemes));

            return Task.CompletedTask;
        }

        [Test]
        public Task CreateDynamicAccentWithColorAndChangeBaseColorScheme()
        {
            var themeManager = ThemeManager.Current;

            var applicationTheme = themeManager.DetectTheme(Application.Current);
            Assert.That(applicationTheme, Is.Not.Null);

            Assert.DoesNotThrow(() => { themeManager.AddTheme(RuntimeThemeGenerator.Current.GenerateRuntimeTheme("Dark", Colors.Red)); });

            Assert.DoesNotThrow(() => { themeManager.AddTheme(themeManager.ChangeTheme(Application.Current, RuntimeThemeGenerator.Current.GenerateRuntimeTheme("Light", Colors.Red))); });

            var detected = themeManager.DetectTheme(Application.Current);
            Assert.That(detected, Is.Not.Null);
            Assert.That(detected.ColorScheme, Is.EqualTo(Colors.Red.ToString()));

            var newTheme = themeManager.ChangeThemeBaseColor(Application.Current, "Dark");
            Assert.That(newTheme, Is.Not.Null);

            newTheme = themeManager.ChangeThemeBaseColor(Application.Current, "Light");
            Assert.That(newTheme, Is.Not.Null);

            themeManager.ChangeTheme(Application.Current, applicationTheme);

            return Task.CompletedTask;
        }
    }
}