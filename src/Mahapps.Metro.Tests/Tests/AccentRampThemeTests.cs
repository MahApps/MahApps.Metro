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

                        Assert.That(resources["MahApps.Colors.IdealForeground.SystemAccentDark1"], Is.EqualTo(RuntimeThemeGenerator.GetIdealTextColor(expected.Dark1)), $"{theme.Name} on Dark1");
                        Assert.That(resources["MahApps.Colors.IdealForeground.SystemAccentLight2"], Is.EqualTo(RuntimeThemeGenerator.GetIdealTextColor(expected.Light2)), $"{theme.Name} on Light2");
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

        [TestCase("Light.Blue", "WinUI.AccentFillDefault", "Dark1")]
        [TestCase("Dark.Blue", "WinUI.AccentFillDefault", "Light2")]
        [TestCase("Light.Blue", "WinUI.AccentFillSecondary", "Dark1")]
        [TestCase("Dark.Blue", "WinUI.AccentFillSecondary", "Light2")]
        [TestCase("Light.Blue", "WinUI.AccentFillTertiary", "Dark1")]
        [TestCase("Dark.Blue", "WinUI.AccentFillTertiary", "Light2")]
        [TestCase("Light.Blue", "WinUI.AccentTextFillPrimary", "Dark2")]
        [TestCase("Dark.Blue", "WinUI.AccentTextFillPrimary", "Light3")]
        [TestCase("Light.Blue", "WinUI.AccentTextFillSecondary", "Dark3")]
        [TestCase("Dark.Blue", "WinUI.AccentTextFillSecondary", "Light3")]
        [TestCase("Light.Blue", "WinUI.AccentTextFillTertiary", "Dark1")]
        [TestCase("Dark.Blue", "WinUI.AccentTextFillTertiary", "Light2")]
        [TestCase("Light.Blue", "WinUI.SystemFillAttention", "")]
        [TestCase("Dark.Blue", "WinUI.SystemFillAttention", "Light2")]
        [TestCase("Light.Blue", "TextControl.WinUI.BorderBrushBottomFocused", "Dark1")]
        [TestCase("Dark.Blue", "TextControl.WinUI.BorderBrushBottomFocused", "Light2")]
        [Description("Common_themeresources_any.xaml of microsoft-ui-xaml: the light theme takes the darker shades, the dark theme the lighter ones.")]
        public void TheWinUIBrushesTakeTheShadesWinUITakes(string theme, string brush, string shade)
        {
            ThemeManager.Current.ChangeTheme(Application.Current, theme);

            var actual = (SolidColorBrush)Application.Current.FindResource($"MahApps.Brushes.{brush}");

            Assert.That(actual.Color, Is.EqualTo(Application.Current.FindResource($"MahApps.Colors.SystemAccent{shade}")));
        }

        [TestCase("WinUI.AccentFillDefault", 1.0)]
        [TestCase("WinUI.AccentFillSecondary", 0.9)]
        [TestCase("WinUI.AccentFillTertiary", 0.8)]
        public void TheStepsOfTheFillKeepTheirOpacity(string brush, double opacity)
        {
            var actual = (SolidColorBrush)Application.Current.FindResource($"MahApps.Brushes.{brush}");

            Assert.That(actual.Opacity, Is.EqualTo(opacity).Within(0.001));
        }

        [TestCase("Light")]
        [TestCase("Dark")]
        public void ARuntimeThemeMapsTheShadesTheSameWay(string baseColour)
        {
            var accent = Color.FromRgb(0x12, 0x8A, 0x56);
            var theme = RuntimeThemeGenerator.Current.GenerateRuntimeTheme(baseColour, accent);
            Assert.That(theme, Is.Not.Null);

            var expected = AccentPalette.Create(accent);
            var fill = (SolidColorBrush)ResourcesOf(theme!)["MahApps.Brushes.WinUI.AccentFillDefault"];

            Assert.That(fill.Color, Is.EqualTo(baseColour == "Light" ? expected.Dark1 : expected.Light2));
        }

        [TestCase("Light.Blue")]
        [TestCase("Dark.Blue")]
        [TestCase("Light.Yellow")]
        [TestCase("Dark.Yellow")]
        [Description("What sits on a fill, the tick or the text of an accent button, is written in the foreground that reads on that shade, not on the accent: the light shade of a dark blue wants black.")]
        public void TheTextOnTheFillReadsOnTheShadeOfTheFill(string theme)
        {
            ThemeManager.Current.ChangeTheme(Application.Current, theme);

            var fill = ((SolidColorBrush)Application.Current.FindResource("MahApps.Brushes.WinUI.AccentFillDefault")).Color;
            var expected = RuntimeThemeGenerator.GetIdealTextColor(fill);

            Assert.Multiple(() =>
                {
                    Assert.That(((SolidColorBrush)Application.Current.FindResource("MahApps.Brushes.WinUI.TextOnAccentPrimary")).Color, Is.EqualTo(expected), "primary");
                    Assert.That(((SolidColorBrush)Application.Current.FindResource("MahApps.Brushes.WinUI.TextOnAccentSecondary")).Color, Is.EqualTo(expected), "secondary");
                });
        }

        [TestCase("Light")]
        [TestCase("Dark")]
        public void ARuntimeThemeWritesOnTheFillTheSameWay(string baseColour)
        {
            var accent = Color.FromRgb(0x00, 0x78, 0xD7);
            var theme = RuntimeThemeGenerator.Current.GenerateRuntimeTheme(baseColour, accent);
            Assert.That(theme, Is.Not.Null);

            var resources = ResourcesOf(theme!);
            var fill = ((SolidColorBrush)resources["MahApps.Brushes.WinUI.AccentFillDefault"]).Color;

            Assert.That(((SolidColorBrush)resources["MahApps.Brushes.WinUI.TextOnAccentPrimary"]).Color, Is.EqualTo(RuntimeThemeGenerator.GetIdealTextColor(fill)));
        }

        [Test]
        public void TheLinksAndTheInfoBarTakeTheirWinUIBrushes()
        {
            var resources = Application.Current;

            Assert.Multiple(() =>
                {
                    Assert.That(resources.FindResource("MahApps.Brushes.Hyperlink.WinUI.Foreground"), Is.SameAs(resources.FindResource("MahApps.Brushes.WinUI.AccentTextFillPrimary")));
                    Assert.That(resources.FindResource("MahApps.Brushes.Hyperlink.WinUI.ForegroundPointerOver"), Is.SameAs(resources.FindResource("MahApps.Brushes.WinUI.AccentTextFillSecondary")));
                    Assert.That(resources.FindResource("MahApps.Brushes.Hyperlink.WinUI.ForegroundPressed"), Is.SameAs(resources.FindResource("MahApps.Brushes.WinUI.AccentTextFillTertiary")));
                    Assert.That(resources.FindResource("MahApps.Brushes.InfoBar.Informational.IconBackground"), Is.SameAs(resources.FindResource("MahApps.Brushes.WinUI.SystemFillAttention")));
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

                TestContext.Out.WriteLine($"        \"MahApps.Colors.IdealForeground.SystemAccentDark1\": \"{RuntimeThemeGenerator.GetIdealTextColor(palette.Dark1)}\",");
                TestContext.Out.WriteLine($"        \"MahApps.Colors.IdealForeground.SystemAccentLight2\": \"{RuntimeThemeGenerator.GetIdealTextColor(palette.Light2)}\",");
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
