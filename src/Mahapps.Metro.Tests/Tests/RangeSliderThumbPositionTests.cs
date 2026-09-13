// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using MahApps.Metro.Controls;
using MahApps.Metro.Tests.TestHelpers;
using NUnit.Framework;

namespace MahApps.Metro.Tests.Tests
{
    /// <summary>
    /// GH-4392 and GH-4113: a thumb of a range slider did not stand over the tick its value belongs to,
    /// and the gap grew along the track. A thumb hangs over the ends of the track by half its width now,
    /// so its middle is the point the value stands for, and the ticks are drawn over the same track.
    /// </summary>
    [TestFixture]
    public class RangeSliderThumbPositionTests
    {
        private const double SliderWidth = 400;
        private const double Lowest = 0;
        private const double Highest = 255;

        private TestWindow? window;

        [OneTimeSetUp]
        public async Task OneTimeSetUp()
        {
            this.window = await WindowHelpers.CreateInvisibleWindowAsync<TestWindow>().ConfigureAwait(false);
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            this.window?.Close();
            this.window = null;
        }

        [TestCase(0)]
        [TestCase(50)]
        [TestCase(130)]
        [TestCase(200)]
        [TestCase(255)]
        [Description("Wherever the lower value stands, its thumb is over the tick that value belongs to.")]
        public void TheLowerThumbStandsWhereItsValueSays(double value)
        {
            var slider = this.Show();

            slider.LowerValue = value;
            slider.UpperValue = Highest;
            this.Settle();

            var thumb = PartOf(slider, "PART_LeftThumb");
            var middle = MiddleOf(thumb, slider);
            var expected = WhereTheTickIs(value);

            Assert.That(middle,
                        Is.EqualTo(expected).Within(0.5),
                        $"the thumb sits at {middle:0.0} while the tick for {value} is at {expected:0.0}");
        }

        [TestCase(0)]
        [TestCase(50)]
        [TestCase(130)]
        [TestCase(200)]
        [TestCase(255)]
        [Description("And the same from the other end, where the upper thumb used to be the one that drifted.")]
        public void TheUpperThumbStandsWhereItsValueSays(double value)
        {
            var slider = this.Show();

            slider.LowerValue = Lowest;
            slider.UpperValue = value;
            this.Settle();

            var thumb = PartOf(slider, "PART_RightThumb");
            var middle = MiddleOf(thumb, slider);
            var expected = WhereTheTickIs(value);

            Assert.That(middle,
                        Is.EqualTo(expected).Within(0.5),
                        $"the thumb sits at {middle:0.0} while the tick for {value} is at {expected:0.0}");
        }

        [TestCase(-40, 104.5, 130, TestName = "PullingAPointApartToTheLeftMovesTheLowerValue")]
        [TestCase(40, 130, 155.5, TestName = "PullingAPointApartToTheRightMovesTheUpperValue")]
        [Description("With both values the same the thumbs sit on top of each other, and whichever one is grabbed has to open the range the way it is pulled.")]
        public void APointCanBePulledApartInEitherDirection(double by, double lower, double upper)
        {
            var slider = this.Show();

            slider.IsSnapToTickEnabled = false;
            slider.LowerValue = 130;
            slider.UpperValue = 130;
            this.Settle();

            Assume.That(slider.LowerValue, Is.EqualTo(slider.UpperValue), "the two thumbs should be in the same place");

            // the upper thumb is the one on top, so that is the one a click lands on
            Drag(PartOf(slider, "PART_RightThumb"), by);
            this.Settle();

            Assert.That(slider.LowerValue, Is.EqualTo(lower).Within(1));
            Assert.That(slider.UpperValue, Is.EqualTo(upper).Within(1));
        }

        [TestCase(-40, 130, 155.5, TestName = "PullingAPointApartUpwardsMovesTheUpperValue")]
        [TestCase(40, 104.5, 130, TestName = "PullingAPointApartDownwardsMovesTheLowerValue")]
        [Description("Standing upright a value grows towards the top, so a drag downwards is the one that opens the range at the lower end.")]
        public void AnUprightPointCanBePulledApartInEitherDirection(double by, double lower, double upper)
        {
            var slider = this.Show(Orientation.Vertical);

            slider.IsSnapToTickEnabled = false;
            slider.LowerValue = 130;
            slider.UpperValue = 130;
            this.Settle();

            Assume.That(slider.LowerValue, Is.EqualTo(slider.UpperValue), "the two thumbs should be in the same place");

            DragUpright(PartOf(slider, "PART_RightThumb"), by);
            this.Settle();

            Assert.That(slider.LowerValue, Is.EqualTo(lower).Within(1));
            Assert.That(slider.UpperValue, Is.EqualTo(upper).Within(1));
        }

        [TestCase(-40, TestName = "AnUprightUpperThumbFollowsADragUpwards")]
        [TestCase(40, TestName = "AnUprightUpperThumbFollowsADragDownwards")]
        [Description("With room on both sides the upper thumb simply follows the mouse, upwards to a larger value and downwards to a smaller one.")]
        public void AnUprightThumbFollowsTheDrag(double by)
        {
            var slider = this.Show(Orientation.Vertical);

            slider.IsSnapToTickEnabled = false;
            slider.LowerValue = 50;
            slider.UpperValue = 130;
            this.Settle();

            DragUpright(PartOf(slider, "PART_RightThumb"), by);
            this.Settle();

            var expected = 130 - (by * (Highest - Lowest) / SliderWidth);

            Assert.That(slider.UpperValue, Is.EqualTo(expected).Within(1), $"a drag of {by} should move the upper value to {expected:0.0}");
            Assert.That(slider.LowerValue, Is.EqualTo(50).Within(0.01), "the lower value has nothing to do with it");
        }

        [TestCase(0)]
        [TestCase(130)]
        [TestCase(255)]
        [Description("Two values on the same spot put the two thumbs on the same spot, wherever on the track that is, and leave no range between them to draw.")]
        public void TwoValuesOnTheSameSpotLeaveBothThumbsThere(double value)
        {
            var slider = this.Show();

            slider.LowerValue = value;
            slider.UpperValue = value;
            this.Settle();

            var left = MiddleOf(PartOf(slider, "PART_LeftThumb"), slider);
            var right = MiddleOf(PartOf(slider, "PART_RightThumb"), slider);
            var band = slider.Template?.FindName("PART_MiddleThumb", slider) as FrameworkElement;

            Assert.That(left, Is.EqualTo(WhereTheTickIs(value)).Within(0.5), "the lower thumb stays on its tick");
            Assert.That(right, Is.EqualTo(WhereTheTickIs(value)).Within(0.5), "and so does the upper one");
            Assert.That(band!.ActualWidth, Is.EqualTo(0).Within(0.5), "and an empty range is drawn as nothing");
        }

        [TestCase(0, 205, TestName = "TheThumbsStandWhereTheirValuesSayWithAMinRange")]
        [TestCase(50, 255, TestName = "AndAtTheOtherEndOfTheTrack")]
        [Description("MinRange is a distance between the two values, not a piece of the track. It says how close the values may get, and it may not change where either of them is drawn.")]
        public void MinRangeDoesNotChangeWhereAThumbStands(double lower, double upper)
        {
            var slider = this.Show();

            slider.MinRange = 50;
            slider.LowerValue = lower;
            slider.UpperValue = upper;
            this.Settle();

            Assume.That(slider.LowerValue, Is.EqualTo(lower).Within(0.01), "the values should be the ones this test asked for");
            Assume.That(slider.UpperValue, Is.EqualTo(upper).Within(0.01));

            var left = MiddleOf(PartOf(slider, "PART_LeftThumb"), slider);
            var right = MiddleOf(PartOf(slider, "PART_RightThumb"), slider);

            Assert.That(left,
                        Is.EqualTo(WhereTheTickIs(lower)).Within(0.5),
                        $"the lower thumb sits at {left:0.0} while the tick for {lower} is at {WhereTheTickIs(lower):0.0}");
            Assert.That(right,
                        Is.EqualTo(WhereTheTickIs(upper)).Within(0.5),
                        $"the upper thumb sits at {right:0.0} while the tick for {upper} is at {WhereTheTickIs(upper):0.0}");
        }

        [TestCase(Orientation.Horizontal, -200, TestName = "AThumbPushedAgainstTheOtherOneTakesItAlong")]
        [TestCase(Orientation.Vertical, 200, TestName = "AnUprightThumbPushedAgainstTheOtherOneTakesItAlong")]
        [Description("Pushed onto the other thumb rather than set to its value, a thumb must still take it along instead of stopping there. The two values are then equal only down to the last bits of a double.")]
        public void AThumbPushedOntoTheOtherOneTakesItAlong(Orientation orientation, double by)
        {
            var slider = this.Show(orientation);

            slider.IsSnapToTickEnabled = false;
            slider.LowerValue = 100;
            slider.UpperValue = 150;
            this.Settle();

            var thumb = PartOf(slider, "PART_RightThumb");

            // a mouse reports a drag in steps, and it takes one of them to push the thumb onto the
            // lower one and another to ask for more than it can give
            DragBy(thumb, orientation, by / 2, by);
            this.Settle();

            Assert.That(slider.UpperValue, Is.EqualTo(100).Within(0.01), "the upper thumb comes to rest on the lower one and stays there");
            Assert.That(slider.LowerValue, Is.LessThan(100), $"and the drag carries on with the lower one, but it stayed at {slider.LowerValue:0.00}");
        }

        private static void DragBy(Thumb thumb, Orientation orientation, params double[] steps)
        {
            if (orientation == Orientation.Horizontal)
            {
                Drag(thumb, steps);
            }
            else
            {
                DragUpright(thumb, steps);
            }
        }

        private static void Drag(Thumb thumb, params double[] steps)
        {
            thumb.RaiseEvent(new DragStartedEventArgs(0, 0) { RoutedEvent = Thumb.DragStartedEvent, Source = thumb });
            foreach (var step in steps)
            {
                thumb.RaiseEvent(new DragDeltaEventArgs(step, 0) { RoutedEvent = Thumb.DragDeltaEvent, Source = thumb });
            }

            thumb.RaiseEvent(new DragCompletedEventArgs(steps[steps.Length - 1], 0, false) { RoutedEvent = Thumb.DragCompletedEvent, Source = thumb });
        }

        private static void DragUpright(Thumb thumb, params double[] steps)
        {
            thumb.RaiseEvent(new DragStartedEventArgs(0, 0) { RoutedEvent = Thumb.DragStartedEvent, Source = thumb });
            foreach (var step in steps)
            {
                thumb.RaiseEvent(new DragDeltaEventArgs(0, step) { RoutedEvent = Thumb.DragDeltaEvent, Source = thumb });
            }

            thumb.RaiseEvent(new DragCompletedEventArgs(0, steps[steps.Length - 1], false) { RoutedEvent = Thumb.DragCompletedEvent, Source = thumb });
        }

        /// <summary>
        /// Where the tick for that value is drawn. The tick bars of the template run over the whole
        /// track, without room set aside for a thumb, so a value is simply its share of the width.
        /// </summary>
        private static double WhereTheTickIs(double value)
        {
            var ratio = (value - Lowest) / (Highest - Lowest);

            return SliderWidth * ratio;
        }

        private static double MiddleOf(FrameworkElement thumb, FrameworkElement slider)
        {
            var origin = thumb.TransformToAncestor(slider).Transform(new Point(0, 0));

            return origin.X + (thumb.ActualWidth / 2);
        }

        private static Thumb PartOf(RangeSlider slider, string part)
        {
            var thumb = slider.Template?.FindName(part, slider) as Thumb;

            Assert.That(thumb, Is.Not.Null, $"the template should carry {part}");
            Assert.That(thumb!.ActualWidth, Is.GreaterThan(0), "the thumb should be laid out, otherwise this test proves nothing");
            Assert.That(thumb.ActualHeight, Is.GreaterThan(0), "and the same the other way round");

            return thumb;
        }

        private RangeSlider Show(Orientation orientation = Orientation.Horizontal)
        {
            Assert.That(this.window, Is.Not.Null);

            var slider = new RangeSlider
                         {
                             Orientation = orientation,
                             Minimum = Lowest,
                             Maximum = Highest,
                             TickFrequency = 5,
                             IsSnapToTickEnabled = true
                         };

            if (orientation == Orientation.Horizontal)
            {
                slider.Width = SliderWidth;
            }
            else
            {
                slider.Height = SliderWidth;
            }

            this.window!.Content = slider;
            this.Settle();

            Assert.That(slider.IsLoaded, Is.True, "the slider should be up before a test looks at it");

            return slider;
        }

        private void Settle()
        {
            this.window!.UpdateLayout();
            ClipAssert.Pump();
        }
    }
}
