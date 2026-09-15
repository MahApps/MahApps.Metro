// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Linq;
using System.Threading.Tasks;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Controls.Primitives;
using MahApps.Metro.Controls;
using MahApps.Metro.Tests.TestHelpers;
using MahApps.Metro.Tests.Views;
using NUnit.Framework;

namespace MahApps.Metro.Tests.Tests
{
    /// <summary>
    /// GH-4454: a range slider held two values and told a client about neither. It was not in the
    /// automation tree at all, only the three thumbs of its template were, so a tool found a handle to
    /// drag and nothing that says what it stands for.
    /// </summary>
    [TestFixture]
    public class RangeSliderAutomationTests
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

        [SetUp]
        public void SetUp()
        {
            var slider = this.window?.TheRangeSlider;

            slider?.SetCurrentValue(RangeSlider.MinimumProperty, 0d);
            slider?.SetCurrentValue(RangeSlider.MaximumProperty, 100d);
            slider?.SetCurrentValue(RangeSlider.LowerValueProperty, 20d);
            slider?.SetCurrentValue(RangeSlider.UpperValueProperty, 80d);

            ClipAssert.Pump();
        }

        [Test]
        [Description("The control is in the tree and says what it is, rather than leaving a client with three loose handles.")]
        public void TheSliderSaysWhatItIs()
        {
            var peer = PeerOfTheSlider();

            Assert.That(peer.GetAutomationControlType(), Is.EqualTo(AutomationControlType.Slider));
            Assert.That(peer.GetClassName(), Is.EqualTo("RangeSlider"));
        }

        [Test]
        [Description("Both ends are read out at once, which is what somebody listening wants to know.")]
        public void TheSliderTellsBothEnds()
        {
            Assert.That(PeerOfTheSlider().GetItemStatus(), Is.EqualTo("20 - 80"));
        }

        [Test]
        [Description("The handle at the lower end carries that value, and hands it over to be read and set.")]
        public void TheLowerHandleCarriesTheLowerValue()
        {
            var range = RangeOf("PART_LeftThumb");

            Assert.That(range.Value, Is.EqualTo(20d));
            Assert.That(range.Minimum, Is.EqualTo(0d));
            Assert.That(range.Maximum, Is.EqualTo(100d));

            range.SetValue(35d);
            ClipAssert.Pump();

            Assert.That(this.window!.TheRangeSlider.LowerValue, Is.EqualTo(35d));
        }

        [Test]
        [Description("And the one at the upper end carries the other.")]
        public void TheUpperHandleCarriesTheUpperValue()
        {
            var range = RangeOf("PART_RightThumb");

            Assert.That(range.Value, Is.EqualTo(80d));

            range.SetValue(65d);
            ClipAssert.Pump();

            Assert.That(this.window!.TheRangeSlider.UpperValue, Is.EqualTo(65d));
        }

        [Test]
        [Description("Neither end may be pushed past the other, the way dragging cannot push it either.")]
        public void OneEndIsNotPushedPastTheOther()
        {
            RangeOf("PART_LeftThumb").SetValue(120d);
            ClipAssert.Pump();

            Assert.That(this.window!.TheRangeSlider.LowerValue, Is.LessThanOrEqualTo(this.window.TheRangeSlider.UpperValue));
        }

        [Test]
        [Description("The handle in the middle moves the whole range, so it stands for no value of its own.")]
        public void TheHandleInTheMiddleCarriesNoValue()
        {
            var thumb = this.window!.TheRangeSlider.FindChild<Thumb>("PART_MiddleThumb");
            var peer = UIElementAutomationPeer.CreatePeerForElement(thumb!);

            Assert.That(peer!.GetPattern(PatternInterface.RangeValue), Is.Null);
        }

        private AutomationPeer PeerOfTheSlider()
        {
            Assert.That(this.window, Is.Not.Null);

            var peer = UIElementAutomationPeer.CreatePeerForElement(this.window!.TheRangeSlider);
            Assert.That(peer, Is.Not.Null, "the slider should make a peer of its own");

            return peer!;
        }

        private IRangeValueProvider RangeOf(string part)
        {
            var thumb = this.window!.TheRangeSlider.FindChild<Thumb>(part);
            Assert.That(thumb, Is.Not.Null, $"the template should carry {part}");

            var peer = PeerOfTheSlider().GetChildren()?
                                        .OfType<UIElementAutomationPeer>()
                                        .FirstOrDefault(child => ReferenceEquals(child.Owner, thumb));

            Assert.That(peer, Is.Not.Null, $"{part} should be among the children of the slider");

            var range = peer!.GetPattern(PatternInterface.RangeValue) as IRangeValueProvider;
            Assert.That(range, Is.Not.Null, $"{part} should hand over the value it stands for");

            return range!;
        }
    }
}
