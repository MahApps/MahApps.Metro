// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using MahApps.Metro.Controls;
using MahApps.Metro.Tests.TestHelpers;
using NUnit.Framework;

namespace MahApps.Metro.Tests.Tests
{
    /// <summary>
    /// GH-4392, GH-4113 and GH-4324: a thumb of a range slider did not stand over the tick its value
    /// belongs to, and the gap grew along the track. A thumb hangs over the ends of the track by half
    /// its width now, so its middle is the point the value stands for, and the ticks are drawn over the
    /// same track. Read the other way round, which is how GH-4324 came in, a place on the track is
    /// worth one value and it does not matter which of the two thumbs is dragged onto it.
    /// </summary>
    [TestFixture]
    public class RangeSliderThumbPositionTests
    {
        private const double SliderWidth = 400;
        private const double Lowest = 0;
        private const double Highest = 255;

        // what a pixel of the track is worth, twice over, which is as close as layout rounding lets a
        // value and the place it stands on come
        private const double APixelOrTwo = 2d * (Highest - Lowest) / SliderWidth;

        private TestWindow? window;
        private double reportedLower;
        private double reportedUpper;

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

        [TestCase(40, 130, 155.5, TestName = "APointOpensTowardsTheEndThatIsPulledTowards")]
        [TestCase(-40, 130, 130, TestName = "APointStaysShutWhenPulledTheWayItCannotGo")]
        [Description("With both values the same the thumbs sit on top of each other, and the upper one is the one on top, so that is what a drag takes hold of. Pulled towards its own end it opens the range, pulled the other way it has nowhere to go.")]
        public void APointOpensOnlyTowardsTheEndThatIsPulledTowards(double by, double lower, double upper)
        {
            var slider = this.Show();

            slider.IsSnapToTickEnabled = false;
            slider.LowerValue = 130;
            slider.UpperValue = 130;
            this.Settle();

            Assume.That(slider.LowerValue, Is.EqualTo(slider.UpperValue), "the two thumbs should be in the same place");

            Drag(PartOf(slider, "PART_RightThumb"), by);
            this.Settle();

            Assert.That(slider.LowerValue, Is.EqualTo(lower).Within(1));
            Assert.That(slider.UpperValue, Is.EqualTo(upper).Within(1));
        }

        [TestCase(-40, 130, 155.5, TestName = "AnUprightPointOpensUpwards")]
        [TestCase(40, 130, 130, TestName = "AnUprightPointStaysShutWhenPulledDownwards")]
        [Description("Standing upright a value grows towards the top, so the upper thumb opens the range on a drag upwards and has nowhere to go on one downwards.")]
        public void AnUprightPointOpensOnlyUpwards(double by, double lower, double upper)
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

        [TestCase(Orientation.Horizontal, -200, TestName = "TheUpperThumbStopsOnTheLowerOne")]
        [TestCase(Orientation.Vertical, 200, TestName = "AnUprightUpperThumbStopsOnTheLowerOne")]
        [Description("A thumb dragged against the other one comes to rest there. The mouse carries on but the other value stays where somebody put it, since nobody is dragging that one.")]
        public void TheUpperThumbDraggedAgainstTheLowerOneStopsThere(Orientation orientation, double by)
        {
            var slider = this.Show(orientation);

            slider.IsSnapToTickEnabled = false;
            slider.LowerValue = 100;
            slider.UpperValue = 150;
            this.Settle();

            // a mouse reports a drag in steps, and it takes one of them to push the thumb onto the
            // lower one and another to ask for more than it can give
            DragBy(PartOf(slider, "PART_RightThumb"), orientation, by / 2, by);
            this.Settle();

            Assert.That(slider.UpperValue, Is.EqualTo(100).Within(0.01), "the upper value comes to rest on the lower one");
            Assert.That(slider.LowerValue, Is.EqualTo(100).Within(0.01), "and the lower one stays where it was put");
        }

        [TestCase(Orientation.Horizontal, 200, TestName = "TheLowerThumbStopsOnTheUpperOne")]
        [TestCase(Orientation.Vertical, -200, TestName = "AnUprightLowerThumbStopsOnTheUpperOne")]
        [Description("And the same the other way round, with the lower thumb driven against the upper one.")]
        public void TheLowerThumbDraggedAgainstTheUpperOneStopsThere(Orientation orientation, double by)
        {
            var slider = this.Show(orientation);

            slider.IsSnapToTickEnabled = false;
            slider.LowerValue = 100;
            slider.UpperValue = 150;
            this.Settle();

            DragBy(PartOf(slider, "PART_LeftThumb"), orientation, by / 2, by);
            this.Settle();

            Assert.That(slider.LowerValue, Is.EqualTo(150).Within(0.01), "the lower value comes to rest on the upper one");
            Assert.That(slider.UpperValue, Is.EqualTo(150).Within(0.01), "and the upper one stays where it was put");
        }

        [TestCase(0.05)]
        [TestCase(0.142857)]
        [TestCase(0.25)]
        [TestCase(0.5)]
        [TestCase(0.9)]
        [Description("A place on the track is worth one value, and dragging either thumb onto it lands on that value. The two used to disagree by a thumb's width, which is a couple of hundred on a long range.")]
        public void EitherThumbDraggedOntoAPlaceLandsOnTheSameValue(double fraction)
        {
            var slider = this.Show(snapToTicks: false);
            var place = SliderWidth * fraction;
            var worth = WhatThePlaceIsWorth(place);

            this.DragOnto(slider, "PART_RightThumb", place);
            var upper = this.reportedUpper;

            this.DragOnto(slider, "PART_LeftThumb", place);
            var lower = this.reportedLower;

            Assert.Multiple(() =>
                {
                    Assert.That(upper, Is.EqualTo(worth).Within(APixelOrTwo), $"the upper value came to {upper:0.0} where the place it stands on is worth {worth:0.0}");
                    Assert.That(lower, Is.EqualTo(worth).Within(APixelOrTwo), $"the lower value came to {lower:0.0} where the place it stands on is worth {worth:0.0}");
                });
        }

        [Test]
        [Description("A mouse reports a drag in steps, and the value stays under the mouse over all of them instead of losing a little on every one.")]
        public void ALongDragStaysUnderTheMouse()
        {
            var slider = this.Show(snapToTicks: false);

            slider.LowerValue = Lowest;
            slider.UpperValue = Highest;
            this.Settle();

            var thumb = PartOf(slider, "PART_RightThumb");

            thumb.RaiseEvent(new DragStartedEventArgs(0, 0) { RoutedEvent = Thumb.DragStartedEvent, Source = thumb });

            for (var step = 1; step <= 20; step++)
            {
                thumb.RaiseEvent(new DragDeltaEventArgs(-15, 0) { RoutedEvent = Thumb.DragDeltaEvent, Source = thumb });
                this.Settle();

                var worth = WhatThePlaceIsWorth(MiddleOf(thumb, slider));

                Assert.That(this.reportedUpper,
                            Is.EqualTo(worth).Within(APixelOrTwo),
                            $"after {step} steps of the mouse the value is {this.reportedUpper:0.0} and the thumb stands on {worth:0.0}");
            }

            thumb.RaiseEvent(new DragCompletedEventArgs(-300, 0, false) { RoutedEvent = Thumb.DragCompletedEvent, Source = thumb });
        }

        /// <summary>
        /// Opens the range all the way and has the mouse carry one thumb onto a place on the track.
        /// What it landed on is in <see cref="reportedLower"/> and <see cref="reportedUpper"/>, the
        /// values the range slider hands over in its own event, which is where GH-4324 read them.
        /// </summary>
        private void DragOnto(RangeSlider slider, string part, double place)
        {
            slider.LowerValue = Lowest;
            slider.UpperValue = Highest;
            this.Settle();

            var thumb = PartOf(slider, part);

            Drag(thumb, place - MiddleOf(thumb, slider));
            this.Settle();

            Assume.That(MiddleOf(thumb, slider), Is.EqualTo(place).Within(1), "the mouse should have taken the thumb where it was going");
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

        /// <summary>
        /// And the same the other way round: what the value at a place on the track is.
        /// </summary>
        private static double WhatThePlaceIsWorth(double place)
        {
            return Lowest + ((Highest - Lowest) * place / SliderWidth);
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

        private RangeSlider Show(Orientation orientation = Orientation.Horizontal, bool snapToTicks = true)
        {
            Assert.That(this.window, Is.Not.Null);

            var slider = new RangeSlider
                         {
                             Orientation = orientation,
                             Minimum = Lowest,
                             Maximum = Highest,
                             TickFrequency = 5,
                             IsSnapToTickEnabled = snapToTicks
                         };

            this.reportedLower = double.NaN;
            this.reportedUpper = double.NaN;
            slider.RangeSelectionChanged += (_, e) =>
                {
                    this.reportedLower = e.NewLowerValue;
                    this.reportedUpper = e.NewUpperValue;
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
