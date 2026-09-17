// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using MahApps.Metro.Controls;
using MahApps.Metro.Tests.TestHelpers;
using NUnit.Framework;

namespace MahApps.Metro.Tests.Tests
{
    /// <summary>
    /// The WinUI text box takes its colours from the WinUI set and not from the Win10 one it
    /// inherits its template from. These tests hold the wiring rather than the palette: what they
    /// compare against are the colour resources themselves, so a value may be corrected in the
    /// generator parameters without a test having to be edited.
    /// </summary>
    [TestFixture]
    public class TextBoxWinUIColourTests
    {
        private TestWindow? window;

        [OneTimeSetUp]
        public async Task OneTimeSetUp()
        {
            this.window = await WindowHelpers.CreateInvisibleWindowAsync<TestWindow>().ConfigureAwait(true);
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            this.window?.Close();
            this.window = null;
        }

        [Test]
        [Description("A box nobody has touched: the WinUI fill, the WinUI text and a border that is drawn a little stronger along its bottom edge.")]
        public void AnIdleBoxCarriesTheWinUIFill()
        {
            var box = this.Show();

            Assert.Multiple(() =>
                {
                    Assert.That(ColourOf(box.Background), Is.EqualTo(Colour("MahApps.Colors.WinUI.ControlFillDefault")), "the background of an idle box");
                    Assert.That(ColourOf(box.Foreground), Is.EqualTo(Colour("MahApps.Colors.WinUI.TextPrimary")), "the text of an idle box");
                    Assert.That(ColourOf(Watermark(box).Foreground), Is.EqualTo(Colour("MahApps.Colors.WinUI.TextSecondary")), "the watermark of an idle box");

                    var stops = Stops(box.BorderBrush);
                    Assert.That(stops.First(), Is.EqualTo(Colour("MahApps.Colors.WinUI.ControlStrokeDefault")), "the border above");
                    Assert.That(stops.Last(), Is.EqualTo(Colour("MahApps.Colors.WinUI.ControlStrokeSecondary")), "and along the bottom edge");
                });
        }

        [Test]
        [Description("With the caret in it the fill turns solid and the bottom edge becomes the accent, two pixels of it.")]
        public void AFocusedBoxTurnsSolidAndUnderlinesItself()
        {
            var box = this.Show();

            box.Focus();
            Keyboard.Focus(box);
            ClipAssert.Pump();

            // a build agent hands the keyboard to one window at a time
            Assume.That(box.IsFocused, Is.True);

            var border = Border(box);

            Assert.Multiple(() =>
                {
                    Assert.That(ColourOf(box.Background), Is.EqualTo(Colour("MahApps.Colors.WinUI.ControlFillInputActive")), "the background of a focused box");
                    Assert.That(Stops(border.BorderBrush).Last(), Is.EqualTo(Colour("MahApps.Colors.SystemAccent")), "the bottom edge of a focused box");
                    Assert.That(Stops(border.BorderBrush).First(), Is.EqualTo(Colour("MahApps.Colors.WinUI.ControlStrokeDefault")), "while the rest of the border stays what it was");
                    Assert.That(border.BorderThickness, Is.EqualTo(new Thickness(1, 1, 1, 2)), "and the line along the bottom is the thick one");
                });
        }

        [Test]
        [Description("A box that is switched off says so.")]
        public void ABoxThatIsOffCarriesTheDisabledColours()
        {
            var box = this.Show();

            box.IsEnabled = false;
            this.Settle();

            Assert.Multiple(() =>
                {
                    Assert.That(ColourOf(box.Background), Is.EqualTo(Colour("MahApps.Colors.WinUI.ControlFillDisabled")), "the background of a box that is off");
                    Assert.That(ColourOf(box.Foreground), Is.EqualTo(Colour("MahApps.Colors.WinUI.TextDisabled")), "the text of a box that is off");
                    Assert.That(ColourOf(Watermark(box).Foreground), Is.EqualTo(Colour("MahApps.Colors.WinUI.TextDisabled")), "the watermark of a box that is off");
                    Assert.That(ColourOf(Border(box).BorderBrush), Is.EqualTo(Colour("MahApps.Colors.WinUI.ControlStrokeDefault")), "the border of a box that is off");
                });
        }

        [Test]
        [Description("And it says so even when it is switched off while the caret is in it, which leaves IsFocused standing.")]
        public void ABoxSwitchedOffWhileItHadTheCaretStillLooksOff()
        {
            var box = this.Show();

            box.Focus();
            Keyboard.Focus(box);
            ClipAssert.Pump();

            Assume.That(box.IsFocused, Is.True);

            box.IsEnabled = false;
            this.Settle();

            Assume.That(box.IsFocused, Is.True, "this test is about the state where both hold at once");

            Assert.Multiple(() =>
                {
                    Assert.That(ColourOf(box.Background), Is.EqualTo(Colour("MahApps.Colors.WinUI.ControlFillDisabled")), "the background of a box that is off");
                    Assert.That(ColourOf(Border(box).BorderBrush), Is.EqualTo(Colour("MahApps.Colors.WinUI.ControlStrokeDefault")), "the border of a box that is off");
                });
        }

        private static Color Colour(string key)
        {
            return (Color)Application.Current.FindResource(key);
        }

        private static Color ColourOf(Brush? brush)
        {
            Assert.That(brush, Is.InstanceOf<SolidColorBrush>(), "this one should be a brush of a single colour");

            return ((SolidColorBrush)brush!).Color;
        }

        private static Color[] Stops(Brush? brush)
        {
            Assert.That(brush, Is.InstanceOf<GradientBrush>(), "this one should be a gradient, since it carries two colours");

            return ((GradientBrush)brush!).GradientStops.Select(stop => stop.Color).ToArray();
        }

        private static Border Border(TextBox box)
        {
            var border = box.FindChild<Border>("BorderElement");
            Assert.That(border, Is.Not.Null, "the template should carry the border");

            return border!;
        }

        private static TextBlock Watermark(TextBox box)
        {
            var watermark = box.FindChild<TextBlock>("PART_Message");
            Assert.That(watermark, Is.Not.Null, "the template should carry the watermark");

            return watermark!;
        }

        private TextBox Show()
        {
            Assert.That(this.window, Is.Not.Null);

            var box = new TextBox
                      {
                          Style = (Style)Application.Current.FindResource("MahApps.Styles.TextBox.WinUI"),
                          Width = 200
                      };

            TextBoxHelper.SetWatermark(box, "Name");

            this.window!.Content = box;
            this.Settle();

            return box;
        }

        private void Settle()
        {
            this.window!.UpdateLayout();
            ClipAssert.Pump();
        }
    }
}
