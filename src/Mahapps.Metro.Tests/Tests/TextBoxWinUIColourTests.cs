// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
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

                    Assert.That(ColourOf(Border(box).BorderBrush), Is.EqualTo(Colour("MahApps.Colors.WinUI.ControlStrokeDefault")), "the border of an idle box");
                    Assert.That(ColourOf(BottomEdge(box).BorderBrush), Is.EqualTo(Colour("MahApps.Colors.WinUI.ControlStrokeSecondary")), "and along the bottom edge, the stronger stroke");
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
                    Assert.That(ColourOf(BottomEdge(box).BorderBrush), Is.EqualTo(Colour("MahApps.Colors.SystemAccent")), "the bottom edge of a focused box");
                    Assert.That(ColourOf(border.BorderBrush), Is.EqualTo(Colour("MahApps.Colors.WinUI.ControlStrokeDefault")), "while the rest of the border stays what it was");
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
                    Assert.That(ColourOf(BottomEdge(box).BorderBrush), Is.EqualTo(Colour("MahApps.Colors.WinUI.ControlStrokeDefault")), "and its bottom edge, which is no longer the stronger one");
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

        [Test]
        [Description("Two boxes of different heights each keep their own edge: the brush belongs to the control it paints, and a shared one would draw the edge of the first across the middle of the second.")]
        public void EachBoxKeepsItsOwnEdgeHoweverTallItsNeighbourIs()
        {
            Assert.That(this.window, Is.Not.Null);

            var shortOne = new RichTextBox { Width = 200, Height = 40, HorizontalAlignment = HorizontalAlignment.Left };
            var tallOne = new RichTextBox { Width = 200, Height = 120, HorizontalAlignment = HorizontalAlignment.Left };

            var page = new StackPanel { HorizontalAlignment = HorizontalAlignment.Left, VerticalAlignment = VerticalAlignment.Top };
            foreach (var box in new[] { shortOne, tallOne })
            {
                box.Style = (Style)Application.Current.FindResource("MahApps.Styles.RichTextBox.WinUI");
                page.Children.Add(box);
            }

            this.window!.Content = page;
            this.Settle();

            // the strokes are translucent, so how much of one a pixel carries is its alpha, and a
            // pixel landing between two device pixels carries a little less of it
            var halfwayDownTheShortOne = SideOf(page, shortOne).A;
            var halfwayDownTheTallOne = SideOf(page, tallOne).A;

            Assert.Multiple(() =>
                {
                    Assert.That(halfwayDownTheTallOne, Is.EqualTo(halfwayDownTheShortOne).Within(2), "halfway down, both sides should be the ordinary stroke");
                    Assert.That(BottomOf(page, tallOne).A, Is.GreaterThan(halfwayDownTheTallOne + 8), "and the bottom edge of the taller one is the stronger stroke, not the ordinary one");
                    Assert.That(BottomOf(page, shortOne).A, Is.GreaterThan(halfwayDownTheShortOne + 8), "as is the bottom edge of the shorter one");
                });
        }

        /// <summary>
        /// The colour the left edge of a box carries that far down it, read off a render of the box
        /// rather than off the brush, since what is asked here is what a viewer ends up seeing.
        /// </summary>
        [TestCase("MahApps.Styles.TimePicker.WinUI", true)]
        [TestCase("MahApps.Styles.TimePicker.Win10", false)]
        [Description("The picker shares its template with the other sets, so the edge is a brush the style hands it and the sets without one draw nothing there.")]
        public void OnlyTheWinUIPickerCarriesAnEdge(string key, bool hasAnEdge)
        {
            Assert.That(this.window, Is.Not.Null);

            var picker = new TimePicker { Width = 200, Style = (Style)Application.Current.FindResource(key) };

            this.window!.Content = picker;
            this.Settle();

            var edge = BottomEdge(picker);

            if (hasAnEdge)
            {
                Assert.That(ColourOf(edge.BorderBrush), Is.EqualTo(Colour("MahApps.Colors.WinUI.ControlStrokeSecondary")), "the WinUI picker should carry the stronger stroke along its bottom edge");
            }
            else
            {
                Assert.That(edge.BorderBrush, Is.Null, "and a set that draws no such edge should leave it unpainted");
            }
        }

        /// <summary>
        /// The colour of the left side of a box halfway down it, read off a render rather than off a
        /// brush, since what is asked here is what a viewer ends up seeing.
        /// </summary>
        private static Color SideOf(FrameworkElement page, FrameworkElement box)
        {
            var picture = Render(page);
            var where = box.TransformToAncestor(page).Transform(new Point(0, box.ActualHeight / 2));
            var origin = VisualTreeHelper.GetOffset(page);

            // a side is a unit wide and rounding may put it either side of a whole pixel, so the
            // first column carrying any colour is the one asked about
            for (var x = 0; x < 4; x++)
            {
                var colour = At(picture, (int)(where.X + origin.X) + x, (int)(where.Y + origin.Y));
                if (colour.A > 0)
                {
                    return colour;
                }
            }

            return Colors.Transparent;
        }

        /// <summary>
        /// And the colour of its bottom edge, taken in the middle of the box so that the rounded
        /// corners stay out of it.
        /// </summary>
        private static Color BottomOf(FrameworkElement page, FrameworkElement box)
        {
            var picture = Render(page);
            var where = box.TransformToAncestor(page).Transform(new Point(box.ActualWidth / 2, box.ActualHeight));
            var origin = VisualTreeHelper.GetOffset(page);

            for (var y = 1; y <= 4; y++)
            {
                var colour = At(picture, (int)(where.X + origin.X), (int)(where.Y + origin.Y) - y);
                if (colour.A > 0)
                {
                    return colour;
                }
            }

            return Colors.Transparent;
        }

        private static RenderTargetBitmap Render(FrameworkElement page)
        {
            // a render carries the offset the visual has in its own parent, so the page sits at its
            // margin and everything on it that much further along
            var origin = VisualTreeHelper.GetOffset(page);

            var bitmap = new RenderTargetBitmap((int)(page.ActualWidth + origin.X), (int)(page.ActualHeight + origin.Y), 96, 96, PixelFormats.Pbgra32);
            bitmap.Render(page);

            return bitmap;
        }

        private static Color At(BitmapSource picture, int x, int y)
        {
            var pixel = new byte[4];
            picture.CopyPixels(new Int32Rect(x, y, 1, 1), pixel, 4, 0);

            return Color.FromArgb(pixel[3], pixel[2], pixel[1], pixel[0]);
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

        private static Border BottomEdge(Control box)
        {
            var edge = box.FindChild<Border>("BottomEdge");
            Assert.That(edge, Is.Not.Null, "the template should carry the bottom edge");

            return edge!;
        }

        private static Border Border(Control box)
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
