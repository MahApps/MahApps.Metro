// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Shapes;
using MahApps.Metro.Controls;
using MahApps.Metro.Tests.TestHelpers;
using NUnit.Framework;

namespace MahApps.Metro.Tests.Tests
{
    /// <summary>
    /// The two Windows sliders. Windows 10 lays a bar of the accent across a line two high; WinUI
    /// turns that inside out and runs a ring along a line four high with a dot of the accent in the
    /// middle of it. GH-3328 asked for the Windows looks; the Windows 10 one has been the default
    /// all along, and these tests hold the WinUI one to what it is meant to draw.
    /// </summary>
    [TestFixture]
    public class SliderSetStyleTests : WindowTestFixture<TestWindow>
    {
        private const string Win10 = "MahApps.Styles.Slider.Win10";
        private const string WinUI = "MahApps.Styles.Slider.WinUI";
        private const string RangeWinUI = "MahApps.Styles.RangeSlider.WinUI";

        [TestCase(Win10, 8d, 24d)]
        [TestCase(WinUI, 18d, 18d)]
        [Description("Windows 10 stands a bar of eight by twenty four on the track. WinUI lays out a circle in a box of eighteen instead, which is why both sides of it are the same, and draws the ring two past that box either way.")]
        public void EachSetDrawsTheThumbItsOwnShape(string key, double width, double height)
        {
            var thumb = this.Show(key).FindChild<MetroThumb>("HorizontalThumb");

            Assert.That(thumb, Is.Not.Null);

            Assert.Multiple(() =>
                {
                    Assert.That(thumb!.Width, Is.EqualTo(width));
                    Assert.That(thumb.Height, Is.EqualTo(height));
                });
        }

        [TestCase(Win10, 2d, 0d)]
        [TestCase(WinUI, 4d, 2d)]
        [Description("The line the thumb runs along is twice as thick in WinUI, and its ends are rounded off rather than cut square.")]
        public void TheWinUITrackIsAThickerLineWithRoundedEnds(string key, double thickness, double radius)
        {
            var line = this.Show(key).FindChild<RepeatButton>("DecreaseLarge")?.FindChild<Rectangle>();

            Assert.That(line, Is.Not.Null);

            Assert.Multiple(() =>
                {
                    Assert.That(line!.Height, Is.EqualTo(thickness));
                    Assert.That(line.RadiusX, Is.EqualTo(radius));
                    Assert.That(line.RadiusY, Is.EqualTo(radius));
                });
        }

        [Test]
        [Description("Standing the slider on end turns the line the other way, so the four it is thick is measured across rather than up.")]
        public void AVerticalSliderLaysTheSameLineTheOtherWay()
        {
            var line = this.Show(WinUI, Orientation.Vertical).FindChild<RepeatButton>("DecreaseLarge")?.FindChild<Rectangle>();

            Assert.That(line, Is.Not.Null);
            Assert.That(line!.Width, Is.EqualTo(4d));
        }

        [Test]
        [Description("The colours the slider shows under the pointer do reach across the whole thirty two it takes up rather than only the line and the thumb, which takes a ground under all of it to land on.")]
        public void TheWholeOfTheSliderTakesThePointer()
        {
            var slider = this.Show(WinUI);
            var root = slider.FindChild<Grid>("HorizontalTemplate");

            Assert.That(root, Is.Not.Null);

            Assert.Multiple(() =>
                {
                    Assert.That(root!.Background, Is.Not.Null);
                    Assert.That(root.ActualHeight, Is.EqualTo(32d));
                });
        }

        [TestCase("MahApps.Thickness.Slider.Thumb.WinUI", 10.32d)]
        [TestCase("MahApps.Thickness.Slider.Thumb.WinUI.Hover", 14d)]
        [TestCase("MahApps.Thickness.Slider.Thumb.WinUI.Pressed", 8.52d)]
        [Description("The dot in the thumb is what answers the pointer. WinUI keeps it a twelve and scales it by 0.86 at rest, 1.167 under the pointer and 0.71 while the thumb is dragged, which comes to these three out of a ring of twenty two. The ring holds still, so here they are one padding rather than three sizes. The thumb snaps to whole device pixels, so on a screen that is not at a hundred percent the dot lands within one of them.")]
        public void TheDotInTheThumbIsSetByThePaddingAroundIt(string key, double dot)
        {
            var thumb = this.Show(WinUI).FindChild<MetroThumb>("HorizontalThumb");

            Assert.That(thumb, Is.Not.Null);

            thumb!.SetCurrentValue(Control.PaddingProperty, (Thickness)Application.Current.FindResource(key));
            this.Settle();

            var inner = thumb.FindChild<Ellipse>("ThumbDot");

            Assert.That(inner, Is.Not.Null);
            Assert.That(inner!.ActualWidth, Is.EqualTo(dot).Within(1d));
        }

        [Test]
        [Description("WinUI keeps two sets of states, one on the slider and one inside the thumb. The colours are the slider's and follow the pointer across the whole of it; the dot is the thumb's own and grows only once the pointer is on the thumb itself, so the padding belongs to the thumb rather than to either template.")]
        public void TheDotFollowsTheThumbRatherThanTheSlider()
        {
            var thumb = (Style)Application.Current.FindResource("MahApps.Styles.Thumb.Slider.WinUI");
            var states = thumb.Triggers
                              .OfType<Trigger>()
                              .Where(trigger => trigger.Setters.OfType<Setter>().Any(setter => setter.Property == Control.PaddingProperty))
                              .Select(trigger => trigger.Property)
                              .ToArray();

            Assert.That(states, Is.EquivalentTo(new DependencyProperty[] { UIElement.IsMouseOverProperty, Thumb.IsDraggingProperty, UIElement.IsEnabledProperty }));

            foreach (var key in new[] { "MahApps.Templates.Slider.Horizontal.WinUI", "MahApps.Templates.Slider.Vertical.WinUI" })
            {
                var template = (ControlTemplate)Application.Current.FindResource(key);

                Assert.That(template.Triggers
                                    .OfType<Trigger>()
                                    .SelectMany(trigger => trigger.Setters.OfType<Setter>())
                                    .Any(setter => setter.Property == Control.PaddingProperty),
                            Is.False,
                            $"{key} still moves the dot from the slider");
            }
        }

        [Test]
        [Description("A range slider wears the same thumb and the same line, since WinUI has no range slider of its own to copy.")]
        public void TheRangeSliderWearsTheSameThumbAndTheSameLine()
        {
            var slider = this.ShowRange();
            var left = slider.FindChild<MetroThumb>("PART_LeftThumb");
            var middle = slider.FindChild<MetroThumb>("PART_MiddleThumb");

            Assert.Multiple(() =>
                {
                    Assert.That(left, Is.Not.Null);
                    Assert.That(middle, Is.Not.Null);
                });

            var bar = middle!.FindChild<Rectangle>();

            Assert.Multiple(() =>
                {
                    Assert.That(left!.Width, Is.EqualTo(18d));
                    Assert.That(left.Height, Is.EqualTo(18d));
                    Assert.That(bar, Is.Not.Null);
                    Assert.That(bar!.Height, Is.EqualTo(4d));
                    Assert.That(bar.RadiusX, Is.EqualTo(2d));
                });
        }

        [Test]
        [Description("The two thumbs stand in a row with the bar between them rather than on a track of their own, so each has to hang half the box it is laid out in back over the line for its middle to be the point its value stands for.")]
        public void TheThumbsOfARangeSliderHangHalfOverTheLine()
        {
            var slider = this.ShowRange();
            var left = slider.FindChild<MetroThumb>("PART_LeftThumb");
            var right = slider.FindChild<MetroThumb>("PART_RightThumb");

            Assert.Multiple(() =>
                {
                    Assert.That(left?.Margin, Is.EqualTo(new Thickness(-9, 0, -9, 0)));
                    Assert.That(right?.Margin, Is.EqualTo(new Thickness(-9, 0, -9, 0)));
                });
        }

        private Slider Show(string key, Orientation orientation = Orientation.Horizontal)
        {
            Assert.That(this.window, Is.Not.Null);

            var slider = new Slider
                         {
                             Style = (Style)Application.Current.FindResource(key),
                             Orientation = orientation,
                             Value = 42,
                             Width = orientation == Orientation.Horizontal ? 200 : double.NaN,
                             Height = orientation == Orientation.Vertical ? 200 : double.NaN,
                             HorizontalAlignment = HorizontalAlignment.Left,
                             VerticalAlignment = VerticalAlignment.Top
                         };

            this.window!.Content = slider;
            this.Settle();

            return slider;
        }

        private RangeSlider ShowRange()
        {
            Assert.That(this.window, Is.Not.Null);

            var slider = new RangeSlider
                         {
                             Style = (Style)Application.Current.FindResource(RangeWinUI),
                             Minimum = 0,
                             Maximum = 100,
                             LowerValue = 20,
                             UpperValue = 65,
                             Width = 200,
                             HorizontalAlignment = HorizontalAlignment.Left,
                             VerticalAlignment = VerticalAlignment.Top
                         };

            this.window!.Content = slider;
            this.Settle();

            return slider;
        }

        private void Settle()
        {
            this.window!.UpdateLayout();
            ClipAssert.Pump();
        }
    }
}
