// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Threading.Tasks;
using System.Windows;
using System.Windows.Automation.Peers;
using MahApps.Metro.Controls;
using MahApps.Metro.Tests.TestHelpers;
using MahApps.Metro.Tests.Views;
using NUnit.Framework;

namespace MahApps.Metro.Tests.Tests
{
    /// <summary>
    /// GH-4454: the flyouts of a window are held in a control that lies over the whole of it. A client
    /// asking what sits under a point goes by the box an element occupies, so with nothing to keep it
    /// out of the way that control answers for the content underneath and an inspection tool cannot
    /// reach anything in the window.
    /// </summary>
    [TestFixture]
    public class FlyoutsAutomationTests
    {
        private FlyoutWindow? window;

        [OneTimeSetUp]
        public async Task OneTimeSetUp()
        {
            this.window = await WindowHelpers.CreateInvisibleWindowAsync<FlyoutWindow>().ConfigureAwait(false);
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
            foreach (var flyout in new[] { this.window?.DefaultFlyout, this.window?.LeftFlyout, this.window?.RightFlyout, this.window?.RightFlyout2 })
            {
                flyout?.SetCurrentValue(Flyout.IsOpenProperty, false);
            }

            ClipAssert.Pump();
        }

        [Test]
        [Description("With every flyout shut there is nothing here to answer for, so the question passes to what lies underneath.")]
        public void ShutFlyoutsAnswerNoQuestionAboutAPoint()
        {
            var peer = PeerOfTheFlyouts();

            Assert.That(peer.GetPeerFromPoint(MiddleOfTheFlyouts()), Is.Null);
        }

        [Test]
        [Description("An open flyout is there to be found, which is the whole point of answering at all.")]
        public void AnOpenFlyoutAnswersForItsOwnPoint()
        {
            Assert.That(this.window, Is.Not.Null);

            this.window!.RightFlyout.SetCurrentValue(Flyout.IsOpenProperty, true);
            ClipAssert.Pump();

            var flyout = this.window.RightFlyout;
            var middle = flyout.PointToScreen(new Point(flyout.ActualWidth / 2, flyout.ActualHeight / 2));

            Assert.That(PeerOfTheFlyouts().GetPeerFromPoint(middle), Is.Not.Null);
        }

        [Test]
        [Description("Nothing of the control is on screen while every flyout is shut.")]
        public void ShutFlyoutsAreNotOnScreen()
        {
            Assert.That(PeerOfTheFlyouts().IsOffscreen(), Is.True);

            this.window!.RightFlyout.SetCurrentValue(Flyout.IsOpenProperty, true);
            ClipAssert.Pump();

            Assert.That(PeerOfTheFlyouts().IsOffscreen(), Is.False);
        }

        private AutomationPeer PeerOfTheFlyouts()
        {
            Assert.That(this.window, Is.Not.Null);

            var flyouts = this.window!.Flyouts;
            Assert.That(flyouts, Is.Not.Null, "the window should carry a flyouts control");

            var peer = UIElementAutomationPeer.CreatePeerForElement(flyouts!);
            Assert.That(peer, Is.Not.Null, "and that control should have a peer");

            return peer!;
        }

        private Point MiddleOfTheFlyouts()
        {
            var flyouts = this.window!.Flyouts!;

            return flyouts.PointToScreen(new Point(flyouts.ActualWidth / 2, flyouts.ActualHeight / 2));
        }
    }
}
