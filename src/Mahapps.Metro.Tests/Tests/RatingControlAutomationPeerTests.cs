// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Globalization;
using System.Windows.Automation;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using MahApps.Metro.Automation.Peers;
using MahApps.Metro.Controls;
using MahApps.Metro.Tests.TestHelpers;
using NUnit.Framework;

namespace MahApps.Metro.Tests.Tests
{
    [TestFixture]
    public class RatingControlAutomationPeerTests : WindowTestFixture<TestWindow>
    {
        private CultureInfo? previousUICulture;
        private CultureInfo? previousCulture;

        [SetUp]
        public void UseEnglish()
        {
            this.previousUICulture = CultureInfo.CurrentUICulture;
            this.previousCulture = CultureInfo.CurrentCulture;
            CultureInfo.CurrentUICulture = new CultureInfo("en-US");
            CultureInfo.CurrentCulture = new CultureInfo("en-US");
        }

        [TearDown]
        public void RestoreCulture()
        {
            CultureInfo.CurrentUICulture = this.previousUICulture!;
            CultureInfo.CurrentCulture = this.previousCulture!;
        }

        private RatingControl Show(RatingControl rating)
        {
            this.window!.Content = rating;
            this.window.UpdateLayout();
            return rating;
        }

        private static RatingControlAutomationPeer PeerOf(RatingControl rating)
        {
            return (RatingControlAutomationPeer)UIElementAutomationPeer.CreatePeerForElement(rating);
        }

        [Test]
        public void ThePeerIsASlider()
        {
            var peer = PeerOf(this.Show(new RatingControl()));

            Assert.Multiple(() =>
                {
                    Assert.That(peer.GetAutomationControlType(), Is.EqualTo(AutomationControlType.Slider));
                    Assert.That(peer.GetLocalizedControlType(), Is.EqualTo("Rating Slider"));
                });
        }

        [TestCase(-1, -1, "Rating Unset")]
        [TestCase(-1, 3.5, "Community Rating, 3.5 of 5")]
        [TestCase(3, -1, "Rating, 3 of 5")]
        [TestCase(3.25, -1, "Rating, 3.25 of 5")]
        [TestCase(3.3, -1, "Rating, 3.3 of 5")]
        [TestCase(2.3, -1, "Rating, 2.3 of 5")]
        public void TheValueReads(double value, double placeholder, string expected)
        {
            var rating = this.Show(new RatingControl { Value = value, PlaceholderValue = placeholder });
            var provider = (IValueProvider)PeerOf(rating).GetPattern(PatternInterface.Value)!;

            Assert.That(provider.Value, Is.EqualTo(expected));
        }

        [Test]
        public void TheRangeIsZeroToMaxRating()
        {
            var rating = this.Show(new RatingControl());
            var provider = (IRangeValueProvider)PeerOf(rating).GetPattern(PatternInterface.RangeValue)!;

            Assert.Multiple(() =>
                {
                    Assert.That(provider.Minimum, Is.EqualTo(0));
                    Assert.That(provider.Maximum, Is.EqualTo(5));
                    Assert.That(provider.SmallChange, Is.EqualTo(1));
                    Assert.That(provider.LargeChange, Is.EqualTo(1));
                    Assert.That(provider.Value, Is.EqualTo(0), "no rating goes out as 0");
                });

            rating.Value = 4;
            Assert.That(provider.Value, Is.EqualTo(4));
        }

        [Test]
        public void TheChangesAreHalvesWithHalfSteps()
        {
            var rating = this.Show(new RatingControl { IsHalfStepEnabled = true });
            var provider = (IRangeValueProvider)PeerOf(rating).GetPattern(PatternInterface.RangeValue)!;

            Assert.Multiple(() =>
                {
                    Assert.That(provider.SmallChange, Is.EqualTo(0.5));
                    Assert.That(provider.LargeChange, Is.EqualTo(0.5));
                });
        }

        [Test]
        public void SettingThroughThePeerSetsTheValue()
        {
            var rating = this.Show(new RatingControl());
            var peer = PeerOf(rating);
            var range = (IRangeValueProvider)peer.GetPattern(PatternInterface.RangeValue)!;
            var text = (IValueProvider)peer.GetPattern(PatternInterface.Value)!;

            range.SetValue(4);
            Assert.That(rating.Value, Is.EqualTo(4));

            text.SetValue("2");
            Assert.That(rating.Value, Is.EqualTo(2));

            text.SetValue("x");
            Assert.That(rating.Value, Is.EqualTo(2), "an input that is no number changes nothing");
        }

        [Test]
        public void IsReadOnlyFollowsTheControl()
        {
            var rating = this.Show(new RatingControl());
            var peer = PeerOf(rating);
            var range = (IRangeValueProvider)peer.GetPattern(PatternInterface.RangeValue)!;
            var text = (IValueProvider)peer.GetPattern(PatternInterface.Value)!;

            Assert.Multiple(() =>
                {
                    Assert.That(range.IsReadOnly, Is.False);
                    Assert.That(text.IsReadOnly, Is.False);
                });

            rating.IsReadOnly = true;

            Assert.Multiple(() =>
                {
                    Assert.That(range.IsReadOnly, Is.True);
                    Assert.That(text.IsReadOnly, Is.True);
                });
        }
    }
}
