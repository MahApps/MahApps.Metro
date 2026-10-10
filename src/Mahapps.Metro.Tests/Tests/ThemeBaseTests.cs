// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using ControlzEx.Theming;
using MahApps.Metro.Theming;
using NUnit.Framework;

namespace MahApps.Metro.Tests.Tests
{
    /// <summary>
    /// What a theme holds that does not depend on its accent, the Windows 10 and WinUI resources
    /// above all, is the same in every theme of one base colour. So it lives once per base colour, in
    /// Styles/ThemeBase/Light.xaml and Dark.xaml, and each theme merges the one of its base colour
    /// rather than carrying a copy of its own.
    /// </summary>
    [TestFixture]
    public class ThemeBaseTests
    {
        [TearDown]
        public void TearDown()
        {
            ThemeManager.Current.ChangeTheme(Application.Current, "Light.Blue");
        }

        [Test]
        [Description("Every theme that ships merges the base of its own base colour.")]
        public void EveryThemeTakesTheBaseOfItsBaseColour()
        {
            var themes = ThemeManager.Current.Themes.Where(t => !t.IsRuntimeGenerated).ToList();
            Assume.That(themes, Is.Not.Empty);

            Assert.Multiple(() =>
                {
                    foreach (var theme in themes)
                    {
                        Assert.That(BaseOf(ThemeDictionaryOf(theme)), Is.EqualTo(theme.BaseColorScheme), theme.Name);
                    }
                });
        }

        [TestCase("Dark")]
        [TestCase("Light")]
        [Description("A theme made at run time from the template merges the base of its base colour as well.")]
        public void ARuntimeThemeTakesTheBaseOfItsBaseColour(string baseColour)
        {
            var theme = RuntimeThemeGenerator.Current.GenerateRuntimeTheme(baseColour, Color.FromRgb(0x12, 0x8A, 0x56));
            Assert.That(theme, Is.Not.Null);

            Assert.That(BaseOf(ThemeDictionaryOf(theme!)), Is.EqualTo(baseColour));
        }

        [Test]
        [Description("A theme carries only what its accent changes, the rest it takes from its base, which is what keeps the assembly small.")]
        public void AThemeCarriesOnlyWhatItsAccentChanges()
        {
            var theme = ThemeManager.Current.GetTheme("Light.Blue");
            Assert.That(theme, Is.Not.Null);

            var dictionary = ThemeDictionaryOf(theme!);
            var own = dictionary.Keys.Count;
            var all = dictionary.MergedDictionaries.Sum(d => d.Keys.Count) + own;

            Assert.That(own, Is.LessThan(all / 4), $"the theme carries {own} of the {all} keys itself");
        }

        [Test]
        [Description("Changing the base colour changes what comes from the base, and the theme is still told by its name.")]
        public void ChangingTheBaseColourChangesTheBase()
        {
            ThemeManager.Current.ChangeTheme(Application.Current, "Light.Blue");
            var light = (Brush)Application.Current.FindResource("MahApps.Brushes.WinUI.TextPrimary");

            ThemeManager.Current.ChangeTheme(Application.Current, "Dark.Blue");
            var dark = (Brush)Application.Current.FindResource("MahApps.Brushes.WinUI.TextPrimary");

            Assert.Multiple(() =>
                {
                    Assert.That(ThemeManager.Current.DetectTheme(Application.Current)?.Name, Is.EqualTo("Dark.Blue"));
                    Assert.That(((SolidColorBrush)dark).Color, Is.Not.EqualTo(((SolidColorBrush)light).Color));
                    Assert.That(((SolidColorBrush)dark).Color, Is.EqualTo(Colors.White), "the primary text of a dark theme");
                });
        }

        [TestCase("styles/themes/light.blue.baml", true)]
        [TestCase("styles/themes/dark.yellow.baml", true)]
        [TestCase("themes/styles.baml", false)]
        [TestCase("themes/analogclock.baml", false)]
        [TestCase("styles/themebase/light.baml", false)]
        [Description("Only the folder the themes are generated into is looked through for themes. The bases have a folder of their own, so they are never loaded only to be turned down as no theme.")]
        public void OnlyTheThemeFolderIsLookedThrough(string resource, bool looked)
        {
            Assert.That(new Provider().Looks(resource), Is.EqualTo(looked));
        }

        private static ResourceDictionary ThemeDictionaryOf(Theme theme)
        {
            var library = theme.LibraryThemes.First(l => l.Origin == "MahApps.Metro");
            var dictionary = Flatten(library.Resources).FirstOrDefault(d => d.Keys.Cast<object>().Contains("Theme.Name"));
            Assert.That(dictionary, Is.Not.Null, $"{theme.Name} should hold a dictionary that names it");

            return dictionary!;
        }

        private static System.Collections.Generic.IEnumerable<ResourceDictionary> Flatten(ResourceDictionary dictionary)
        {
            yield return dictionary;

            foreach (var merged in dictionary.MergedDictionaries)
            {
                foreach (var inner in Flatten(merged))
                {
                    yield return inner;
                }
            }
        }

        private static string? BaseOf(ResourceDictionary theme)
        {
            var source = theme.MergedDictionaries.Select(d => d.Source?.ToString()).FirstOrDefault(s => s?.IndexOf("/Styles/ThemeBase/", StringComparison.OrdinalIgnoreCase) >= 0);

            return source is null ? null : System.IO.Path.GetFileNameWithoutExtension(source);
        }

        private sealed class Provider : MahAppsLibraryThemeProvider
        {
            public bool Looks(string resource) => this.IsPotentialThemeResourceDictionary(new DictionaryEntry(resource, null));
        }
    }
}
