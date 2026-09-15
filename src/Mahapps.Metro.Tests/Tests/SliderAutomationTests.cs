// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Threading.Tasks;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using MahApps.Metro.Controls;
using MahApps.Metro.Tests.TestHelpers;
using MahApps.Metro.Tests.Views;
using NUnit.Framework;

namespace MahApps.Metro.Tests.Tests
{
    /// <summary>
    /// GH-4450: the two repeat buttons beside the thumb are what an automation tool presses to move a
    /// slider by a page. It looks them up by the id WPF gives them, so every style has to hand out the
    /// same one.
    /// </summary>
    [TestFixture]
    public class SliderAutomationTests
    {
        private SliderWindow? window;

        [OneTimeSetUp]
        public async Task OneTimeSetUp()
        {
            this.window = await WindowHelpers.CreateInvisibleWindowAsync<SliderWindow>().ConfigureAwait(false);
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            this.window?.Close();
            this.window = null;
        }

        [Test]
        [Description("The styles this library ships hand out the ids the stock template does.")]
        public void EveryStyleHandsOutTheIdsAnAutomationToolLooksFor()
        {
            Assert.That(this.window, Is.Not.Null);

            AssertTheIds(this.window!.TheSlider, "MahApps.Styles.Slider");
            AssertTheIds(this.window.TheWin10Slider, "MahApps.Styles.Slider.Win10");
            AssertTheIds(this.window.TheFlatSlider, "MahApps.Styles.Slider.Flat");
        }

        [Test]
        [Description("An upright slider has a template of its own, and it hands out the same ids.")]
        public void AnUprightSliderHandsOutTheSameIds()
        {
            Assert.That(this.window, Is.Not.Null);

            AssertTheIds(this.window!.TheUprightSlider, "MahApps.Styles.Slider");
            AssertTheIds(this.window.TheUprightWin10Slider, "MahApps.Styles.Slider.Win10");
            AssertTheIds(this.window.TheUprightFlatSlider, "MahApps.Styles.Slider.Flat");
        }

        private static void AssertTheIds(Slider slider, string what)
        {
            var track = slider.FindChild<Track>("PART_Track");
            Assert.That(track, Is.Not.Null, $"{what} should carry a track");

            Assert.That(IdOf(track!.DecreaseRepeatButton), Is.EqualTo("DecreaseLarge"), $"the button below the thumb of {what}");
            Assert.That(IdOf(track.IncreaseRepeatButton), Is.EqualTo("IncreaseLarge"), $"the button above the thumb of {what}");
        }

        private static string? IdOf(RepeatButton button)
        {
            Assert.That(button, Is.Not.Null);

            return UIElementAutomationPeer.CreatePeerForElement(button)?.GetAutomationId();
        }
    }
}
