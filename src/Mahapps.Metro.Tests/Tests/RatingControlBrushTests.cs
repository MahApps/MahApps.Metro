// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using ControlzEx.Theming;
using MahApps.Metro.Controls;
using MahApps.Metro.Tests.TestHelpers;
using NUnit.Framework;

namespace MahApps.Metro.Tests.Tests
{
    /// <summary>
    /// The colours of the rating, taken from RatingControl_themeresources.xaml of microsoft-ui-xaml: WinUI 3 for the
    /// WinUI look, WinUI 2 v2.4.2 for the Windows 10 look. The Metro look puts the gray steps of the other Metro
    /// styles in for the Windows 10 levels, the empty stars in the unfilled track colour of the Metro slider.
    /// </summary>
    [TestFixture]
    public class RatingControlBrushTests : WindowTestFixture<TestWindow>
    {
        [TestCase("Light.Blue", "MahApps.Brushes.RatingControl.WinUI.Unselected", "#9E000000")]
        [TestCase("Dark.Blue", "MahApps.Brushes.RatingControl.WinUI.Unselected", "#C5FFFFFF")]
        [TestCase("Light.Blue", "MahApps.Brushes.RatingControl.WinUI.Selected", "#FF0067C3")]
        [TestCase("Dark.Blue", "MahApps.Brushes.RatingControl.WinUI.Selected", "#FF4DC1FF")]
        [TestCase("Light.Blue", "MahApps.Brushes.RatingControl.WinUI.Placeholder", "#E4000000")]
        [TestCase("Dark.Blue", "MahApps.Brushes.RatingControl.WinUI.Placeholder", "#FFFFFFFF")]
        [TestCase("Light.Blue", "MahApps.Brushes.RatingControl.WinUI.PointerOverPlaceholder", "#0F000000")]
        [TestCase("Dark.Blue", "MahApps.Brushes.RatingControl.WinUI.PointerOverUnselected", "#0BFFFFFF")]
        [TestCase("Light.Blue", "MahApps.Brushes.RatingControl.WinUI.PointerOverSelected", "#FF0067C3")]
        [TestCase("Light.Blue", "MahApps.Brushes.RatingControl.WinUI.DisabledSelected", "#5C000000")]
        [TestCase("Dark.Blue", "MahApps.Brushes.RatingControl.WinUI.Caption", "#C5FFFFFF")]
        [TestCase("Light.Blue", "MahApps.Brushes.RatingControl.Win10.Unselected", "#33000000")]
        [TestCase("Dark.Blue", "MahApps.Brushes.RatingControl.Win10.Unselected", "#33FFFFFF")]
        [TestCase("Light.Blue", "MahApps.Brushes.RatingControl.Win10.Selected", "#FF0078D7")]
        [TestCase("Dark.Blue", "MahApps.Brushes.RatingControl.Win10.Selected", "#FF0078D7")]
        [TestCase("Light.Blue", "MahApps.Brushes.RatingControl.Win10.Placeholder", "#FF000000")]
        [TestCase("Dark.Blue", "MahApps.Brushes.RatingControl.Win10.Placeholder", "#FFFFFFFF")]
        [TestCase("Light.Blue", "MahApps.Brushes.RatingControl.Win10.PointerOverPlaceholder", "#99000000")]
        [TestCase("Dark.Blue", "MahApps.Brushes.RatingControl.Win10.PointerOverUnselected", "#99FFFFFF")]
        [TestCase("Dark.Blue", "MahApps.Brushes.RatingControl.Win10.PointerOverSelected", "#FF0078D7")]
        [TestCase("Light.Blue", "MahApps.Brushes.RatingControl.Win10.DisabledSelected", "#66000000")]
        [TestCase("Light.Blue", "MahApps.Brushes.RatingControl.Win10.Caption", "#99000000")]
        [TestCase("Light.Blue", "MahApps.Brushes.RatingControl.Unselected", "#FF9D9D9D")]
        [TestCase("Dark.Blue", "MahApps.Brushes.RatingControl.Unselected", "#FF9D9D9D")]
        [TestCase("Light.Blue", "MahApps.Brushes.RatingControl.Selected", "#CC0078D7")]
        [TestCase("Dark.Blue", "MahApps.Brushes.RatingControl.Selected", "#CC0078D7")]
        [TestCase("Light.Blue", "MahApps.Brushes.RatingControl.Placeholder", "#FF000000")]
        [TestCase("Dark.Blue", "MahApps.Brushes.RatingControl.Placeholder", "#FFFFFFFF")]
        [TestCase("Light.Blue", "MahApps.Brushes.RatingControl.PointerOverPlaceholder", "#FF7F7F7F")]
        [TestCase("Dark.Blue", "MahApps.Brushes.RatingControl.PointerOverUnselected", "#FF7F7F7F")]
        [TestCase("Light.Blue", "MahApps.Brushes.RatingControl.PointerOverSelected", "#CC0078D7")]
        [TestCase("Light.Blue", "MahApps.Brushes.RatingControl.DisabledSelected", "#FFCCCCCC")]
        [TestCase("Dark.Blue", "MahApps.Brushes.RatingControl.Caption", "#FF7F7F7F")]
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
        public void TheStarsFollowAThemeChange()
        {
            // the empty stars are the same gray in both themes, the placeholder ones are the text colour
            var rating = new RatingControl { PlaceholderValue = 3 };
            this.window!.Content = rating;
            this.window.UpdateLayout();

            var foreground = rating.FindChild<StackPanel>("PART_RatingForegroundStackPanel")!;

            try
            {
                var light = ((SolidColorBrush)TextElement.GetForeground(foreground)).Color;

                ThemeManager.Current.ChangeTheme(this.window!, "Dark.Blue");
                var dark = ((SolidColorBrush)TextElement.GetForeground(foreground)).Color;

                Assert.Multiple(() =>
                    {
                        Assert.That(light, Is.EqualTo((Color)ColorConverter.ConvertFromString("#FF000000")));
                        Assert.That(dark, Is.EqualTo((Color)ColorConverter.ConvertFromString("#FFFFFFFF")));
                    });
            }
            finally
            {
                ThemeManager.Current.ChangeTheme(this.window!, "Light.Blue");
            }
        }
    }
}
