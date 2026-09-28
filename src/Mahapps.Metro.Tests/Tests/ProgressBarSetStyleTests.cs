// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using MahApps.Metro.Controls;
using MahApps.Metro.Tests.TestHelpers;
using NUnit.Framework;

namespace MahApps.Metro.Tests.Tests
{
    /// <summary>
    /// The two Windows bars. Windows 10 sends five dots along one that cannot say how far it has
    /// got, WinUI sends two bars instead, and both of them answer work that has been held up or has
    /// gone wrong. GH-3328 asked for the Windows looks; these are the two of them side by side.
    /// </summary>
    [TestFixture]
    public class ProgressBarSetStyleTests : WindowTestFixture<TestWindow>
    {
        private const string Default = "MahApps.Styles.ProgressBar";
        private const string Win10 = "MahApps.Styles.ProgressBar.Win10";
        private const string WinUI = "MahApps.Styles.ProgressBar.WinUI";
        private const string MetroWin10 = "MahApps.Styles.MetroProgressBar.Win10";
        private const string MetroWinUI = "MahApps.Styles.MetroProgressBar.WinUI";

        [TestCase(Default, 10d)]
        [TestCase(Win10, 4d)]
        [TestCase(WinUI, 3d)]
        [Description("Each Windows look is thinner than the one before it, and both are thinner than the bar the library has always drawn.")]
        public void EachSetDrawsTheBarItsOwnHeight(string key, double height)
        {
            var bar = this.Show(key, 320);

            Assert.That(bar.MinHeight, Is.EqualTo(height));
        }

        [Test]
        [Description("Windows 10 sends five dots along a bar that cannot say how far it has got. WinUI has none of them and sends two bars instead.")]
        public void OnlyTheWindows10BarSendsDotsAlong()
        {
            Assert.Multiple(() =>
                {
                    Assert.That(this.Show(Win10, 320).FindChild<Ellipse>("E1"), Is.Not.Null);
                    Assert.That(this.Show(WinUI, 320).FindChild<Ellipse>("E1"), Is.Null);
                    Assert.That(this.Show(WinUI, 320).FindChild<Rectangle>("RunningBar1"), Is.Not.Null);
                });
        }

        [TestCase(160d, 4d, 4d)]
        [TestCase(240d, 5d, 7d)]
        [TestCase(320d, 6d, 9d)]
        [Description("A wider bar gets bigger dots with more room between them, in the three steps Windows 10 picks between.")]
        public void TheDotsAreSizedForTheBarTheyRunIn(double width, double diameter, double offset)
        {
            var bar = this.Show(Win10, width);

            Assert.Multiple(() =>
                {
                    Assert.That(ProgressBarHelper.GetEllipseDiameter(bar), Is.EqualTo(diameter));
                    Assert.That(ProgressBarHelper.GetEllipseOffset(bar), Is.EqualTo(offset));
                });
        }

        [Test]
        [Description("How far the dots travel depends on how wide the bar is, and a Storyboard carries no lookups, so the helper writes those numbers into a copy of the one the template brought.")]
        public void TheDotsRunTheLengthOfTheBarTheyAreIn()
        {
            var bar = this.Show(Win10, 320, isIndeterminate: true);

            var containingGrid = bar.FindChild<Grid>("ContainingGrid");
            Assert.That(containingGrid, Is.Not.Null);

            var storyboard = VisualStateManager.GetVisualStateGroups(containingGrid)
                                               .OfType<VisualStateGroup>()
                                               .SelectMany(group => group.States.OfType<VisualState>())
                                               .First(state => state.Name == "Indeterminate")
                                               .Storyboard;

            Assert.That(storyboard, Is.Not.Null, "the helper should have hung an adjusted Storyboard on the state");

            var travel = (DoubleAnimation)storyboard!.Children.First(child => child.Name == "MainDoubleAnim");

            Assert.Multiple(() =>
                {
                    Assert.That(travel.From, Is.EqualTo(-63d), "a bar this wide starts the dots furthest off the left edge");
                    Assert.That(travel.To, Is.EqualTo(0.4352 * 320 + 58.862).Within(0.001));
                });
        }

        [TestCase(Win10, "MahApps.Brushes.ProgressBar.Win10.Paused")]
        [TestCase(WinUI, "MahApps.Brushes.ProgressBar.WinUI.Paused")]
        [TestCase(MetroWin10, "MahApps.Brushes.ProgressBar.Win10.Paused")]
        [TestCase(MetroWinUI, "MahApps.Brushes.ProgressBar.WinUI.Paused")]
        [Description("Work that has been held up: Windows 10 keeps the accent and only takes it back, WinUI goes over to amber.")]
        public void ABarThatIsHeldUpSaysSo(string key, string brush)
        {
            var bar = this.Show(key, 320);
            ProgressBarHelper.SetShowPaused(bar, true);
            this.Settle();

            Assert.That(bar.Foreground, Is.SameAs(Application.Current.FindResource(brush)));
        }

        [TestCase(Win10, "MahApps.Brushes.ProgressBar.Win10.Error")]
        [TestCase(WinUI, "MahApps.Brushes.ProgressBar.WinUI.Error")]
        [TestCase(MetroWin10, "MahApps.Brushes.ProgressBar.Win10.Error")]
        [TestCase(MetroWinUI, "MahApps.Brushes.ProgressBar.WinUI.Error")]
        [Description("And work that has gone wrong turns the bar red in both sets.")]
        public void ABarThatHasGoneWrongSaysSo(string key, string brush)
        {
            var bar = this.Show(key, 320);
            ProgressBarHelper.SetShowError(bar, true);
            this.Settle();

            Assert.That(bar.Foreground, Is.SameAs(Application.Current.FindResource(brush)));
        }

        [TestCase(Win10, "MahApps.Brushes.ProgressBar.Win10.Error")]
        [TestCase(WinUI, "MahApps.Brushes.ProgressBar.WinUI.Error")]
        [Description("A bar that is both held up and wrong reads as the worse of the two, because the error trigger comes after the pause.")]
        public void ABarThatIsBothReadsAsTheWorseOfTheTwo(string key, string brush)
        {
            var bar = this.Show(key, 320);
            ProgressBarHelper.SetShowPaused(bar, true);
            ProgressBarHelper.SetShowError(bar, true);
            this.Settle();

            Assert.That(bar.Foreground, Is.SameAs(Application.Current.FindResource(brush)));
        }

        [Test]
        [Description("A wait of unknown length that has been held up has nothing to show while it moves, so the WinUI bar takes the two that travel off and leaves one standing.")]
        public void TheWinUIBarStandsStillWhileItIsHeldUp()
        {
            var bar = this.Show(WinUI, 320, isIndeterminate: true);
            ProgressBarHelper.SetShowPaused(bar, true);
            this.Settle();

            Assert.Multiple(() =>
                {
                    Assert.That(bar.FindChild<Rectangle>("RunningBar1")!.Opacity, Is.EqualTo(0d));
                    Assert.That(bar.FindChild<Rectangle>("RunningBar2")!.Opacity, Is.EqualTo(0d));
                    Assert.That(bar.FindChild<Rectangle>("StoppedBar")!.Opacity, Is.EqualTo(1d));
                });
        }

        [Test]
        [Description("The WinUI track is a line under the bar rather than a fill behind it, and it goes out of the way while the two that travel are out.")]
        public void TheWinUITrackIsALineUnderTheBar()
        {
            var resting = this.Show(WinUI, 320);
            var track = resting.FindChild<Rectangle>("PART_Track");

            Assert.Multiple(() =>
                {
                    Assert.That(track, Is.Not.Null);
                    Assert.That(track!.Height, Is.EqualTo(1d));
                    Assert.That(track.Fill, Is.SameAs(Application.Current.FindResource("MahApps.Brushes.ProgressBar.WinUI.Background")));
                    Assert.That(this.Show(WinUI, 320, isIndeterminate: true).FindChild<Rectangle>("PART_Track")!.Opacity, Is.EqualTo(0d));
                });
        }

        private ProgressBar Show(string key, double width, bool isIndeterminate = false)
        {
            Assert.That(this.window, Is.Not.Null);

            var bar = key.Contains("MetroProgressBar") ? new MetroProgressBar() : new ProgressBar();
            bar.Style = (Style)Application.Current.FindResource(key);
            bar.Width = width;
            bar.Value = 42;
            bar.IsIndeterminate = isIndeterminate;
            bar.HorizontalAlignment = HorizontalAlignment.Left;
            bar.VerticalAlignment = VerticalAlignment.Top;

            this.window!.Content = bar;
            this.Settle();

            return bar;
        }

        private void Settle()
        {
            this.window!.UpdateLayout();
            ClipAssert.Pump();
        }
    }
}
