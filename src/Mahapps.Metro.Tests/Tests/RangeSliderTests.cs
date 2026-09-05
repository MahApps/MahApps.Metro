// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Threading.Tasks;
using System.Windows.Controls.Primitives;
using MahApps.Metro.Controls;
using MahApps.Metro.Tests.TestHelpers;
using MahApps.Metro.Tests.Views;
using NUnit.Framework;

namespace MahApps.Metro.Tests.Tests
{
    /// <summary>
    /// MinRange and MinRangeWidth sound like the same thing and are not: one is a distance between
    /// the values, the other a width in pixels. These tests hold both of them to what the
    /// documentation of the two properties says.
    /// </summary>
    [TestFixture]
    public class RangeSliderTests
    {
        private RangeSliderWindow? window;

        [OneTimeSetUp]
        public async Task OneTimeSetUp()
        {
            this.window = await WindowHelpers.CreateInvisibleWindowAsync<RangeSliderWindow>().ConfigureAwait(false);
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            this.window?.Close();
            this.window = null;
        }

        private static Thumb GetMiddleThumb(RangeSlider slider)
        {
            var thumb = slider.FindChild<Thumb>("PART_MiddleThumb");
            Assert.That(thumb, Is.Not.Null, "the template should carry a middle thumb");

            return thumb!;
        }

        [Test]
        public void MinRangeWidthShouldBeTheMinimumWidthOfTheMiddleThumb()
        {
            Assert.That(this.window, Is.Not.Null);

            var slider = this.window.TheRangeSlider;
            slider.UpdateLayout();

            Assert.That(slider.MinRangeWidth, Is.EqualTo(30d), "the default should be 30");
            Assert.That(GetMiddleThumb(slider).MinWidth, Is.EqualTo(slider.MinRangeWidth), "the template binds MinRangeWidth to the minimum width of the middle thumb");
        }

        [Test]
        public void MinRangeWidthShouldKeepTheThumbsApartWithoutMovingTheValues()
        {
            Assert.That(this.window, Is.Not.Null);

            var slider = this.window.TheRangeSlider;
            slider.UpdateLayout();

            Assert.That(slider.MinRange, Is.EqualTo(0d), "nothing should keep the values apart here");
            Assert.That(slider.LowerValue, Is.EqualTo(50d));
            Assert.That(slider.UpperValue, Is.EqualTo(50d), "MinRangeWidth is a width, so it must not move the values");
            Assert.That(GetMiddleThumb(slider).ActualWidth, Is.EqualTo(slider.MinRangeWidth).Within(0.5), "two equal values still draw a range as wide as MinRangeWidth");
        }

        [Test]
        public void MinRangeWidthShouldBeCoercedToHalfOfTheTrack()
        {
            Assert.That(this.window, Is.Not.Null);

            var slider = this.window.TheRangeSlider;
            slider.UpdateLayout();

            var leftThumb = slider.FindChild<Thumb>("PART_LeftThumb");
            var rightThumb = slider.FindChild<Thumb>("PART_RightThumb");
            Assert.That(leftThumb, Is.Not.Null);
            Assert.That(rightThumb, Is.Not.Null);

            var track = slider.ActualWidth - leftThumb!.ActualWidth - rightThumb!.ActualWidth;
            Assert.That(track, Is.GreaterThan(0), "the slider should be laid out, otherwise this test proves nothing");

            try
            {
                slider.SetCurrentValue(RangeSlider.MinRangeWidthProperty, 10000d);

                Assert.That(slider.MinRangeWidth, Is.EqualTo(track / 2).Within(0.5), "a value wider than the track should be cut down to half of it");
            }
            finally
            {
                slider.ClearValue(RangeSlider.MinRangeWidthProperty);
            }
        }

        [Test]
        public void MinRangeShouldBeTheMinimumDistanceBetweenTheValues()
        {
            Assert.That(this.window, Is.Not.Null);

            var slider = this.window.TheMinRangeSlider;
            slider.UpdateLayout();

            Assert.That(slider.MinRangeWidth, Is.EqualTo(0d), "nothing should keep the thumbs apart here");
            Assert.That(slider.MinRange, Is.EqualTo(10d));
            Assert.That(slider.UpperValue - slider.LowerValue, Is.GreaterThanOrEqualTo(slider.MinRange), "two equal values should be pushed apart by MinRange");
        }

        [Test]
        public void MinRangeShouldStopTheLowerValueFromReachingTheUpperOne()
        {
            Assert.That(this.window, Is.Not.Null);

            var slider = this.window.TheMinRangeSlider;
            slider.UpdateLayout();

            var upperValue = slider.UpperValue;

            try
            {
                slider.SetCurrentValue(RangeSlider.LowerValueProperty, upperValue);

                Assert.That(slider.LowerValue, Is.EqualTo(upperValue - slider.MinRange), "the lower value should stop MinRange short of the upper one");
            }
            finally
            {
                slider.SetCurrentValue(RangeSlider.LowerValueProperty, 50d);
            }
        }
    }
}
