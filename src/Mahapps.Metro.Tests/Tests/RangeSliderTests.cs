// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Markup;
using MahApps.Metro.Controls;
using MahApps.Metro.Tests.TestHelpers;
using MahApps.Metro.Tests.Views;
using NUnit.Framework;

namespace MahApps.Metro.Tests.Tests
{
    /// <summary>
    /// MinRange is the smallest distance there may be between the two values. These tests hold it to
    /// what its documentation says, including the order it may be set in.
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

        /// <summary>
        /// What a thumb takes out of the track. The templates let it hang over the end by half its width
        /// through a negative margin, so what it occupies is the width and the margin put together.
        /// </summary>
        private static double RoomTakenBy(Thumb thumb)
        {
            return thumb.ActualWidth + thumb.Margin.Left + thumb.Margin.Right;
        }

        private static Thumb GetMiddleThumb(RangeSlider slider)
        {
            var thumb = slider.FindChild<Thumb>("PART_MiddleThumb");
            Assert.That(thumb, Is.Not.Null, "the template should carry a middle thumb");

            return thumb!;
        }

        [Test]
        [Description("MinRangeWidth is obsolete and does nothing at all: it may neither move a value nor change what is drawn, so that the attribute can be left in place until it is removed.")]
        public void MinRangeWidthShouldNoLongerDoAnything()
        {
            Assert.That(this.window, Is.Not.Null);

            var slider = this.window.TheRangeSlider;
            var band = GetMiddleThumb(slider);

            try
            {
                // a range with something to it, otherwise there is nothing to watch for a change
                slider.SetCurrentValue(RangeSlider.LowerValueProperty, 30d);
                slider.SetCurrentValue(RangeSlider.UpperValueProperty, 70d);
                slider.UpdateLayout();

                var drawn = band.ActualWidth;
                Assume.That(drawn, Is.GreaterThan(60), "the band has to be wider than what the test is about to ask for");

#pragma warning disable CS0618
                slider.SetCurrentValue(RangeSlider.MinRangeWidthProperty, 200d);
#pragma warning restore CS0618
                slider.UpdateLayout();

                Assert.That(slider.LowerValue, Is.EqualTo(30d), "the lower value should be where it was");
                Assert.That(slider.UpperValue, Is.EqualTo(70d), "and so should the upper one");
                Assert.That(band.ActualWidth, Is.EqualTo(drawn).Within(0.01), "and the range should be drawn the way it was");
            }
            finally
            {
#pragma warning disable CS0618
                slider.ClearValue(RangeSlider.MinRangeWidthProperty);
#pragma warning restore CS0618
                slider.SetCurrentValue(RangeSlider.LowerValueProperty, 50d);
                slider.SetCurrentValue(RangeSlider.UpperValueProperty, 50d);
                slider.UpdateLayout();
            }
        }

        [Test]
        [Description("XAML sets a property as it reads it, and the formatter sorts MinRange in front of Style, so MinRange is set while Maximum is still the 1 that RangeBase hands out. What the style brings afterwards has to count all the same.")]
        public void MinRangeSetBeforeTheStyleShouldNotSwallowItsValues()
        {
            var slider = TheSliderOf(@"<mah:RangeSlider MinRange='20' Style='{StaticResource TheRange}' />");

            Assert.That(slider.MinRange, Is.EqualTo(20d), "the distance asked for");
            Assert.That(slider.LowerValue, Is.EqualTo(30d), "the lower value asked for");
            Assert.That(slider.UpperValue, Is.EqualTo(70d), "the upper value asked for");
        }

        [Test]
        [Description("And the other way round, with MinRange written after the style, which has always worked.")]
        public void MinRangeSetAfterTheStyleShouldKeepItsValues()
        {
            var slider = TheSliderOf(@"<mah:RangeSlider Style='{StaticResource TheRange}' MinRange='20' />");

            Assert.That(slider.MinRange, Is.EqualTo(20d));
            Assert.That(slider.LowerValue, Is.EqualTo(30d));
            Assert.That(slider.UpperValue, Is.EqualTo(70d));
        }

        /// <summary>
        /// A slider built from markup the way the demo builds one, with its range in a style, so that
        /// the properties are set in the order the markup names them.
        /// </summary>
        private static RangeSlider TheSliderOf(string markup)
        {
            var panel = (StackPanel)XamlReader.Parse(
                @"<StackPanel xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation'
                              xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml'
                              xmlns:mah='http://metro.mahapps.com/winfx/xaml/controls'>
                    <StackPanel.Resources>
                      <Style x:Key='TheRange' TargetType='mah:RangeSlider'>
                        <Setter Property='Maximum' Value='100' />
                        <Setter Property='Minimum' Value='0' />
                        <Setter Property='LowerValue' Value='30' />
                        <Setter Property='UpperValue' Value='70' />
                      </Style>
                    </StackPanel.Resources>
                    " + markup + @"
                  </StackPanel>");

            return (RangeSlider)panel.Children[0];
        }

        [Test]
        public void MinRangeShouldBeTheMinimumDistanceBetweenTheValues()
        {
            Assert.That(this.window, Is.Not.Null);

            var slider = this.window.TheMinRangeSlider;
            slider.UpdateLayout();

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
