// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Linq;
using System.Windows;
using System.Windows.Media;
using ControlzEx.Theming;
using MahApps.Metro.Theming;
using NUnit.Framework;

namespace MahApps.Metro.Tests.Tests
{
    /// <summary>
    /// The shades of the accent in the themes. A theme that ships has them from GeneratorParameters.json, one made at
    /// run time from the provider, and both have to be what <see cref="AccentPalette"/> works out for the accent.
    /// </summary>
    [TestFixture]
    public class AccentRampThemeTests
    {
        private static readonly string[] Shades = { "Light1", "Light2", "Light3", "Dark1", "Dark2", "Dark3" };

        [TearDown]
        public void TearDown()
        {
            ThemeManager.Current.ChangeTheme(Application.Current, "Light.Blue");
        }

        [Test]
        [Description("Every theme that ships carries the shades worked out from its accent, so the generated themes and the calculation never part ways.")]
        public void EveryThemeCarriesTheShadesOfItsAccent()
        {
            var themes = ThemeManager.Current.Themes.Where(t => !t.IsRuntimeGenerated).ToList();
            Assume.That(themes, Is.Not.Empty);

            Assert.Multiple(() =>
                {
                    foreach (var theme in themes)
                    {
                        var resources = ResourcesOf(theme);
                        var expected = AccentPalette.Create((Color)resources["MahApps.Colors.AccentBase"]);

                        foreach (var shade in Shades)
                        {
                            Assert.That(resources[$"MahApps.Colors.SystemAccent{shade}"], Is.EqualTo(Pick(expected, shade)), $"{theme.Name} {shade}");
                        }
                    }
                });
        }

        [TestCase("Light")]
        [TestCase("Dark")]
        public void ARuntimeThemeCarriesTheShadesOfItsAccent(string baseColour)
        {
            var accent = Color.FromRgb(0x12, 0x8A, 0x56);
            var theme = RuntimeThemeGenerator.Current.GenerateRuntimeTheme(baseColour, accent);
            Assert.That(theme, Is.Not.Null);

            var resources = ResourcesOf(theme!);
            var expected = AccentPalette.Create(accent);

            Assert.Multiple(() =>
                {
                    foreach (var shade in Shades)
                    {
                        Assert.That(resources[$"MahApps.Colors.SystemAccent{shade}"], Is.EqualTo(Pick(expected, shade)), shade);
                        Assert.That(((SolidColorBrush)resources[$"MahApps.Brushes.SystemAccent{shade}"]).Color, Is.EqualTo(Pick(expected, shade)), shade);
                    }
                });
        }

        [Test]
        [Explicit("Prints the values for GeneratorParameters.json.")]
        public void PrintTheShadesForTheGenerator()
        {
            foreach (var theme in ThemeManager.Current.Themes.Where(t => !t.IsRuntimeGenerated && t.BaseColorScheme == "Light"))
            {
                var palette = AccentPalette.Create((Color)ResourcesOf(theme)["MahApps.Colors.AccentBase"]);
                TestContext.Out.WriteLine($"## {theme.ColorScheme}");

                foreach (var shade in Shades)
                {
                    TestContext.Out.WriteLine($"        \"MahApps.Colors.SystemAccent{shade}\": \"{Pick(palette, shade)}\",");
                }
            }
        }

        private static ResourceDictionary ResourcesOf(Theme theme)
        {
            return theme.LibraryThemes.First(l => l.Origin == "MahApps.Metro").Resources;
        }

        private static Color Pick(AccentPalette palette, string shade)
        {
            return shade switch
            {
                "Light1" => palette.Light1,
                "Light2" => palette.Light2,
                "Light3" => palette.Light3,
                "Dark1" => palette.Dark1,
                "Dark2" => palette.Dark2,
                _ => palette.Dark3,
            };
        }
    }
}
