// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using ControlzEx.Theming;
using MahApps.Metro.Tests.TestHelpers;
using NUnit.Framework;

namespace MahApps.Metro.Tests.Tests
{
    /// <summary>
    /// The link in running text, in the Windows 10 look of generic.xaml of the Windows SDK and in the WinUI look of
    /// Hyperlink_themeresources.xaml. WinUI takes a darker or lighter step of the accent there; until the theme has
    /// those steps it is the accent fill.
    /// </summary>
    [TestFixture]
    public class HyperlinkStyleTests : WindowTestFixture<TestWindow>
    {
        private static readonly DependencyPropertyKey MouseOverKey
            = (DependencyPropertyKey)typeof(UIElement).GetField("IsMouseOverPropertyKey", BindingFlags.Static | BindingFlags.NonPublic)!.GetValue(null)!;

        [TestCase("Light.Blue", "MahApps.Brushes.Hyperlink.Win10.Foreground", "#FF0078D7")]
        [TestCase("Dark.Blue", "MahApps.Brushes.Hyperlink.Win10.Foreground", "#FF0078D7")]
        [TestCase("Light.Blue", "MahApps.Brushes.Hyperlink.Win10.ForegroundPointerOver", "#99000000")]
        [TestCase("Dark.Blue", "MahApps.Brushes.Hyperlink.Win10.ForegroundPointerOver", "#99FFFFFF")]
        [TestCase("Light.Blue", "MahApps.Brushes.Hyperlink.Win10.ForegroundPressed", "#66000000")]
        [TestCase("Dark.Blue", "MahApps.Brushes.Hyperlink.Win10.ForegroundPressed", "#66FFFFFF")]
        [TestCase("Light.Blue", "MahApps.Brushes.Hyperlink.Win10.ForegroundDisabled", "#66000000")]
        [TestCase("Dark.Blue", "MahApps.Brushes.Hyperlink.Win10.ForegroundDisabled", "#66FFFFFF")]
        [TestCase("Light.Blue", "MahApps.Brushes.Hyperlink.WinUI.Foreground", "#FF0078D7")]
        [TestCase("Light.Blue", "MahApps.Brushes.Hyperlink.WinUI.ForegroundDisabled", "#5C000000")]
        [TestCase("Dark.Blue", "MahApps.Brushes.Hyperlink.WinUI.ForegroundDisabled", "#5DFFFFFF")]
        public void TheColoursAreMicrosofts(string theme, string key, string expected)
        {
            ThemeManager.Current.ChangeTheme(this.window!, theme);

            try
            {
                var brush = (SolidColorBrush)this.window!.FindResource(key);

                Assert.That(brush.Color, Is.EqualTo((Color)ColorConverter.ConvertFromString(expected)));
            }
            finally
            {
                ThemeManager.Current.ChangeTheme(this.window!, "Light.Blue");
            }
        }

        [Test]
        public void TheWinUILinkTakesTheStepsOfTheAccentFill()
        {
            Assert.Multiple(() =>
                {
                    Assert.That(this.window!.FindResource("MahApps.Brushes.Hyperlink.WinUI.ForegroundPointerOver"), Is.SameAs(this.window.FindResource("MahApps.Brushes.WinUI.AccentFillSecondary")));
                    Assert.That(this.window.FindResource("MahApps.Brushes.Hyperlink.WinUI.ForegroundPressed"), Is.SameAs(this.window.FindResource("MahApps.Brushes.WinUI.AccentFillTertiary")));
                });
        }

        private Hyperlink Show(string styleKey)
        {
            var link = new Hyperlink(new Run("42")) { Style = (Style)this.window!.FindResource(styleKey) };
            this.window.Content = new TextBlock(link);
            this.window.UpdateLayout();
            return link;
        }

        [TestCase("MahApps.Styles.Hyperlink.Win10", "MahApps.Brushes.Hyperlink.Win10", true)]
        [TestCase("MahApps.Styles.Hyperlink.WinUI", "MahApps.Brushes.Hyperlink.WinUI", false)]
        public void TheLinkWearsItsColoursAndItsUnderline(string styleKey, string brushes, bool underlined)
        {
            var link = this.Show(styleKey);

            Assert.Multiple(() =>
                {
                    Assert.That(link.Foreground, Is.SameAs(this.window!.FindResource($"{brushes}.Foreground")), "at rest");
                    Assert.That(link.TextDecorations, underlined ? Is.EqualTo(TextDecorations.Underline) : Is.Null.Or.Empty, "the underline");
                    Assert.That(link.Cursor, Is.EqualTo(Cursors.Hand), "the hand of the Metro style it stands on");
                });

            link.SetValue(MouseOverKey, true);
            Assert.That(link.Foreground, Is.SameAs(this.window!.FindResource($"{brushes}.ForegroundPointerOver")), "under the mouse");

            link.ClearValue(MouseOverKey);
            link.IsEnabled = false;
            Assert.That(link.Foreground, Is.SameAs(this.window.FindResource($"{brushes}.ForegroundDisabled")), "disabled");
        }
    }
}
