// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using MahApps.Metro.Controls;
using MahApps.Metro.Tests.TestHelpers;
using NUnit.Framework;

namespace MahApps.Metro.Tests.Tests
{
    /// <summary>
    /// The switch the library hands out is the Windows 10 one already, which is why only the WinUI
    /// set brings one of its own. GH-3328 asked for the Windows looks; these are the two of them
    /// side by side.
    /// </summary>
    [TestFixture]
    public class ToggleSwitchSetStyleTests
    {
        private const string Default = "MahApps.Styles.ToggleSwitch";
        private const string WinUI = "MahApps.Styles.ToggleSwitch.WinUI";

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

        [TearDown]
        public void TearDown()
        {
            if (this.window is not null)
            {
                this.window.Content = null;
            }
        }

        [TestCase(Default, 44d, 10d)]
        [TestCase(WinUI, 40d, 12d)]
        [Description("Windows 10 gives the switch a track of 44 by 20 with a 10 unit knob in it, WinUI a shorter track with a bigger knob.")]
        public void EachSetDrawsTheTrackAndTheKnobItsOwnSize(string key, double trackWidth, double knobSize)
        {
            var toggleSwitch = this.Show(key, false);

            var track = toggleSwitch.FindChild<Grid>("Switch");
            var knob = toggleSwitch.FindChild<Ellipse>("SwitchKnobOff");

            Assert.Multiple(() =>
                {
                    Assert.That(track, Is.Not.Null);
                    Assert.That(track!.Width, Is.EqualTo(trackWidth));
                    Assert.That(track.Height, Is.EqualTo(20d), "both are as tall as each other");
                    Assert.That(knob, Is.Not.Null);
                    Assert.That(knob!.Width, Is.EqualTo(knobSize));
                    Assert.That(knob.Height, Is.EqualTo(knobSize));
                });
        }

        [TestCase(Default, 24d)]
        [TestCase(WinUI, 20d)]
        [Description("How far the knob travels is the track less the cell it sits in, so a shorter track means a shorter way.")]
        public void TheKnobTravelsTheLengthOfItsOwnTrack(string key, double travel)
        {
            var toggleSwitch = this.Show(key, true);

            var knob = toggleSwitch.FindChild<Grid>("SwitchKnob");

            Assert.That(knob, Is.Not.Null);
            Assert.That(((TranslateTransform)knob!.RenderTransform).X, Is.EqualTo(travel));
        }

        [Test]
        [Description("A Windows 10 switch that is off is a hollow track with a line round it, so there is nothing behind the knob.")]
        public void TheWindows10TrackIsHollowWhileItIsOff()
        {
            var toggleSwitch = this.Show(Default, false);

            var track = toggleSwitch.FindChild<Rectangle>("OuterBorder");

            Assert.Multiple(() =>
                {
                    Assert.That(track, Is.Not.Null);
                    Assert.That(track!.Fill, Is.SameAs(Application.Current.FindResource("MahApps.Brushes.SystemControlTransparent")));
                    Assert.That(track.StrokeThickness, Is.EqualTo(2d), "and the line round it is the thicker of the two");
                });
        }

        [Test]
        [Description("A WinUI switch that is off is filled instead, and the line round it is half as thick.")]
        public void TheWinUITrackIsFilledWhileItIsOff()
        {
            var toggleSwitch = this.Show(WinUI, false);

            var track = toggleSwitch.FindChild<Rectangle>("OuterBorder");

            Assert.Multiple(() =>
                {
                    Assert.That(track, Is.Not.Null);
                    Assert.That(track!.Fill, Is.SameAs(Application.Current.FindResource("MahApps.Brushes.WinUI.ControlAltFillSecondary")));
                    Assert.That(track.StrokeThickness, Is.EqualTo(1d));
                });
        }

        [TestCase(Default, "MahApps.Brushes.ToggleSwitch.FillOn")]
        [TestCase(WinUI, "MahApps.Brushes.ToggleSwitch.WinUI.FillOn")]
        [Description("A switch that is on fills with the accent in both looks.")]
        public void ASwitchThatIsOnFillsWithTheAccent(string key, string fill)
        {
            var toggleSwitch = this.Show(key, true);

            var track = toggleSwitch.FindChild<Rectangle>("SwitchKnobBounds");

            Assert.Multiple(() =>
                {
                    Assert.That(track, Is.Not.Null);
                    Assert.That(track!.Fill, Is.SameAs(Application.Current.FindResource(fill)));
                    Assert.That(track.Opacity, Is.EqualTo(1d), "and it is the fill that shows rather than the hollow track");
                });
        }

        [Test]
        [Description("Pressing a switch puts it into the dragging state, and a WinUI switch that is on has to keep looking on while it is there, so what it looks like hangs on IsOn rather than on the state.")]
        public void TheWinUISwitchKeepsItsOnLookWhileItIsDragged()
        {
            var toggleSwitch = this.Show(WinUI, true);

            VisualStateManager.GoToState(toggleSwitch, "Dragging", false);
            this.window!.UpdateLayout();
            ClipAssert.Pump();

            Assert.Multiple(() =>
                {
                    Assert.That(toggleSwitch.FindChild<Rectangle>("SwitchKnobBounds")!.Opacity, Is.EqualTo(1d), "the accent stays");
                    Assert.That(toggleSwitch.FindChild<Rectangle>("OuterBorder")!.Opacity, Is.EqualTo(0d), "and the track it is off on stays out of the way");
                });
        }

        [Test]
        [Description("The knob of a WinUI switch keeps a lit edge while it is on, which is what holds it against a light accent. The Windows 10 one has none.")]
        public void OnlyTheWinUIKnobCarriesALitEdge()
        {
            Assert.Multiple(() =>
                {
                    Assert.That(this.Show(WinUI, true).FindChild<Ellipse>("SwitchKnobOn")!.Stroke,
                                Is.SameAs(Application.Current.FindResource("MahApps.Brushes.ToggleSwitch.WinUI.KnobStrokeOn")));
                    Assert.That(this.Show(Default, true).FindChild<Ellipse>("SwitchKnobOn")!.Stroke, Is.Null);
                });
        }

        private ToggleSwitch Show(string key, bool isOn)
        {
            Assert.That(this.window, Is.Not.Null);

            var toggleSwitch = new ToggleSwitch
                               {
                                   Style = (Style)Application.Current.FindResource(key),
                                   OffContent = "Off",
                                   OnContent = "On",
                                   IsOn = isOn,
                                   HorizontalAlignment = HorizontalAlignment.Left,
                                   VerticalAlignment = VerticalAlignment.Top
                               };

            this.window!.Content = toggleSwitch;
            this.window.UpdateLayout();
            ClipAssert.Pump();

            return toggleSwitch;
        }
    }
}
