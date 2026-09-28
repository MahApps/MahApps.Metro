// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using MahApps.Metro.Controls;
using MahApps.Metro.Tests.TestHelpers;
using NUnit.Framework;

namespace MahApps.Metro.Tests.Tests
{
    /// <summary>
    /// The two Windows rings. Windows 10 sends six dots round, which is what the library has drawn
    /// all along, and WinUI drops them for one arc that grows and is eaten away again while the
    /// whole thing turns. GH-3328 asked for the Windows looks; these are the two of them.
    /// </summary>
    [TestFixture]
    public class ProgressRingSetStyleTests : WindowTestFixture<TestWindow>
    {
        private const string Win10 = "MahApps.Styles.ProgressRing.Win10";
        private const string WinUI = "MahApps.Styles.ProgressRing.WinUI";

        [TestCase(Win10, 60d, 20d)]
        [TestCase(WinUI, 32d, 16d)]
        [Description("Windows 10 never said how big its ring should be, so the Win10 set keeps the size the library has always given it. WinUI does say, and asks for a smaller one.")]
        public void EachSetDrawsTheRingItsOwnSize(string key, double size, double smallest)
        {
            var ring = this.Show(key);

            Assert.Multiple(() =>
                {
                    Assert.That(ring.Width, Is.EqualTo(size));
                    Assert.That(ring.Height, Is.EqualTo(size));
                    Assert.That(ring.MinWidth, Is.EqualTo(smallest));
                });
        }

        [Test]
        [Description("Windows 10 sends six dots round the ring. WinUI has none of them and draws one arc instead.")]
        public void OnlyTheWindows10RingSendsDotsRound()
        {
            Assert.Multiple(() =>
                {
                    Assert.That(this.Show(Win10).FindChild<Ellipse>("E1"), Is.Not.Null);
                    Assert.That(this.Show(WinUI).FindChild<Ellipse>("E1"), Is.Null);
                    Assert.That(this.Show(WinUI).FindChild<Ellipse>("PART_TurningArc"), Is.Not.Null);
                });
        }

        [TestCase(32d, 3d)]
        [TestCase(80d, 7.5d)]
        [TestCase(160d, 15d)]
        [Description("The WinUI ring is drawn in a square of 80 and scaled to whatever size it is given, so the stroke stays in proportion instead of thinning out on a big ring.")]
        public void TheWinUIStrokeGrowsWithTheRing(double size, double thickness)
        {
            var ring = this.Show(WinUI, size);
            var square = ring.FindChild<Grid>("RingSquare");

            Assert.That(square, Is.Not.Null);

            var transform = square!.TransformToAncestor(ring);
            var scale = transform.Transform(new Point(1, 0)).X - transform.Transform(new Point(0, 0)).X;

            Assert.That(scale * 7.5, Is.EqualTo(thickness).Within(0.01));
        }

        [Test]
        [Description("Two seconds carry the arc round once and a half with a whole turn of growing and being eaten away inside it, and it ends where it started so the jump back to the next round has nothing to show.")]
        public void TheWinUIRingEndsItsRoundWhereItCanStartAgain()
        {
            var ring = this.Show(WinUI);
            var storyboard = ring.TryFindResource("TurningStoryboard") as Storyboard;

            Assert.That(storyboard, Is.Not.Null);

            var start = KeyFramesFor(storyboard!, ProgressRing.ArcStartAngleProperty);
            var end = KeyFramesFor(storyboard!, ProgressRing.ArcEndAngleProperty);

            Assert.Multiple(() =>
                {
                    Assert.That(start.First().Value, Is.EqualTo(0d));
                    Assert.That(end.First().Value, Is.EqualTo(0d));

                    // half way round the arc is at its longest, which is half the circle
                    Assert.That(end[1].Value - start[1].Value, Is.EqualTo(180d));

                    // and at the end the two meet again, a whole number of turns from where they began
                    Assert.That(start.Last().Value, Is.EqualTo(1080d));
                    Assert.That(end.Last().Value, Is.EqualTo(1080d));
                    Assert.That(start.Last().KeyTime.TimeSpan.TotalSeconds, Is.EqualTo(2d));
                });
        }

        [TestCase(0d, 180d, 0.5d)]
        [TestCase(90d, 180d, 0.25d)]
        [TestCase(450d, 630d, 0.5d)]
        [Description("The two angles a template animates become the dash that draws the turning arc, and the track takes the rest of the same turn so that neither is ever under the other.")]
        public void TheTwoAnglesBecomeTheDashThatDrawsTheArc(double start, double end, double share)
        {
            // the storyboard would otherwise write over the two angles on its next frame
            var ring = this.Show(WinUI, size: 80, isActive: false);

            ring.ArcStartAngle = start;
            ring.ArcEndAngle = end;
            this.Settle();

            var arc = ring.FindChild<Ellipse>("PART_TurningArc");
            var track = ring.FindChild<Ellipse>("PART_TurningTrack");

            Assert.Multiple(() =>
                {
                    Assert.That(arc, Is.Not.Null);
                    Assert.That(track, Is.Not.Null);
                });

            var radius = (arc!.Width - arc.StrokeThickness) / 2;
            var turn = 2 * Math.PI * radius / arc.StrokeThickness;

            Assert.Multiple(() =>
                {
                    Assert.That(arc.StrokeDashArray[0], Is.EqualTo(turn * share).Within(0.001));
                    Assert.That(arc.StrokeDashArray[1], Is.EqualTo(turn * (1 - share)).Within(0.001));
                    Assert.That(arc.StrokeDashOffset, Is.EqualTo(-turn * start / 360).Within(0.001));

                    Assert.That(track!.StrokeDashArray[0], Is.EqualTo(turn * (1 - share)).Within(0.001));
                    Assert.That(track.StrokeDashOffset, Is.EqualTo(-turn * end / 360).Within(0.001));
                });
        }

        [Test]
        [Description("A ring that has been switched off keeps its place in the layout and shows nothing, which is what makes it usable in a binding.")]
        public void ARingThatIsOffShowsNothing()
        {
            var ring = this.Show(WinUI, isActive: false);
            var root = (FrameworkElement)VisualTreeHelper.GetChild(ring, 0);

            Assert.Multiple(() =>
                {
                    Assert.That(root.Opacity, Is.EqualTo(0d));
                    Assert.That(ring.ActualWidth, Is.EqualTo(32d));
                });
        }

        [TestCase(0d, 1d)]
        [TestCase(25d, 0.75d)]
        [TestCase(50d, 0.5d)]
        [TestCase(100d, 0d)]
        [Description("What the ring leaves undrawn is what the dash gap has to cover, and it runs from the whole way round down to none of it.")]
        public void WhatIsLeftUndrawnFollowsTheValue(double value, double remainder)
        {
            var ring = this.Show(WinUI, isIndeterminate: false, value: value);

            Assert.That(ring.ArcRemainder, Is.EqualTo(remainder));
        }

        [Test]
        [Description("A ring takes its range from RangeBase, and counts to a hundred rather than to one the way RangeBase otherwise would.")]
        public void ARingCountsToAHundred()
        {
            var ring = new ProgressRing();

            Assert.Multiple(() =>
                {
                    Assert.That(ring.Maximum, Is.EqualTo(100d));
                    Assert.That(ring.Minimum, Is.EqualTo(0d));
                    Assert.That(ring.IsIndeterminate, Is.True);
                });
        }

        [Test]
        [Description("A value outside the range does not send the arc past the end of the ring or behind its start.")]
        public void AValueOutsideTheRangeStaysOnTheRing()
        {
            var ring = this.Show(WinUI, isIndeterminate: false, value: 100);

            ring.Minimum = 20;
            ring.Maximum = 80;
            this.Settle();

            Assert.That(ring.ArcRemainder, Is.EqualTo(0d));
        }

        [Test]
        [Description("The two ways of drawing the ring never show at once: one is the turning arc, the other the one that says how far along it is.")]
        public void OnlyOneOfTheTwoArcsIsEverOnShow()
        {
            var turning = this.Show(WinUI);
            var telling = this.Show(WinUI, isIndeterminate: false, value: 42);

            Assert.Multiple(() =>
                {
                    Assert.That(turning.FindChild<Canvas>("IndeterminateArc")?.Visibility, Is.EqualTo(Visibility.Visible));
                    Assert.That(turning.FindChild<Canvas>("DeterminateArc")?.Visibility, Is.EqualTo(Visibility.Collapsed));
                    Assert.That(telling.FindChild<Canvas>("IndeterminateArc")?.Visibility, Is.EqualTo(Visibility.Collapsed));
                    Assert.That(telling.FindChild<Canvas>("DeterminateArc")?.Visibility, Is.EqualTo(Visibility.Visible));
                });
        }

        [Test]
        [Description("A ring at the bottom of its range draws nothing at all, since the path of a circle comes out a hair longer than the dash gap meant to cover it and a round cap would leave a dot behind.")]
        public void ARingAtTheBottomOfItsRangeDrawsNothing()
        {
            var empty = this.Show(WinUI, isIndeterminate: false, value: 0);
            var started = this.Show(WinUI, isIndeterminate: false, value: 1);

            Assert.Multiple(() =>
                {
                    Assert.That(empty.FindChild<Canvas>("DeterminateArc")?.Opacity, Is.EqualTo(0d));
                    Assert.That(started.FindChild<Canvas>("DeterminateArc")?.Opacity, Is.EqualTo(1d));
                });
        }

        [Test]
        [Description("The turn that brings the start of a determinate arc up to twelve has to go about the middle of the ring. A render transform reads its centre off the element it sits on, and that ellipse is inset from the corner of the square, so a centre given in the coordinates of the square swings the arc off the track entirely.")]
        public void TheArcSitsOnTheTrack()
        {
            var ring = this.Show(WinUI, size: 80, isIndeterminate: false, value: 42);
            var arc = ring.FindChild<Ellipse>("DeterminateArcRing");
            var track = ring.FindChild<Ellipse>("DeterminateTrackRing");

            Assert.Multiple(() =>
                {
                    Assert.That(arc, Is.Not.Null);
                    Assert.That(track, Is.Not.Null);
                });

            var arcBounds = arc!.TransformToAncestor(ring).TransformBounds(new Rect(arc.RenderSize));
            var trackBounds = track!.TransformToAncestor(ring).TransformBounds(new Rect(track.RenderSize));

            Assert.Multiple(() =>
                {
                    Assert.That(arcBounds.X + (arcBounds.Width / 2), Is.EqualTo(trackBounds.X + (trackBounds.Width / 2)).Within(0.01));
                    Assert.That(arcBounds.Y + (arcBounds.Height / 2), Is.EqualTo(trackBounds.Y + (trackBounds.Height / 2)).Within(0.01));
                });
        }

        [TestCase(0d, 0d)]
        [TestCase(25d, -0.25d)]
        [TestCase(100d, -1d)]
        [Description("The rest of the circle is drawn with the same dash pushed the other way, so it picks up where the arc stops instead of running underneath it, where two strokes on one line come out mixed along their edges.")]
        public void TheRestOfTheCirclePicksUpWhereTheArcStops(double value, double share)
        {
            var ring = this.Show(WinUI, size: 80, isIndeterminate: false, value: value);
            var rest = ring.FindChild<Ellipse>("DeterminateTrackRing");

            Assert.That(rest, Is.Not.Null);

            var radius = (rest!.Width - rest.StrokeThickness) / 2;
            var turn = 2 * Math.PI * radius / rest.StrokeThickness;

            Assert.That(rest.StrokeDashOffset, Is.EqualTo(turn * share).Within(0.001));
        }

        [Test]
        [Description("The dash that draws a determinate arc has to be exactly one turn of the circle it runs on, or a full ring would not close and an empty one would not be empty.")]
        public void TheDashIsOneWholeTurnOfTheRing()
        {
            var ring = this.Show(WinUI, size: 80, isIndeterminate: false, value: 42);
            var dashed = ring.FindChild<Ellipse>("DeterminateArcRing");

            Assert.That(dashed, Is.Not.Null);

            var radius = (dashed!.Width - dashed.StrokeThickness) / 2;
            var turn = 2 * Math.PI * radius / dashed.StrokeThickness;

            Assert.Multiple(() =>
                {
                    Assert.That(dashed.StrokeDashArray, Has.Count.EqualTo(2));
                    Assert.That(dashed.StrokeDashArray[0], Is.EqualTo(turn).Within(0.001));
                    Assert.That(dashed.StrokeDashArray[1], Is.EqualTo(turn).Within(0.001));
                    Assert.That(dashed.StrokeDashOffset, Is.EqualTo(turn * 0.58).Within(0.001));
                });
        }

        private static VisualState? ActiveState(ProgressRing ring)
        {
            var root = VisualTreeHelper.GetChild(ring, 0) as FrameworkElement;

            return VisualStateManager.GetVisualStateGroups(root)
                                     .OfType<VisualStateGroup>()
                                     .SelectMany(group => group.States.OfType<VisualState>())
                                     .FirstOrDefault(state => state.Name == "Active");
        }

        private static DoubleKeyFrame[] KeyFramesFor(Storyboard storyboard, DependencyProperty property)
        {
            return storyboard.Children
                             .OfType<DoubleAnimationUsingKeyFrames>()
                             .Where(animation => Storyboard.GetTargetProperty(animation)?.PathParameters.Contains(property) == true)
                             .SelectMany(animation => animation.KeyFrames.OfType<DoubleKeyFrame>())
                             .ToArray();
        }

        private ProgressRing Show(string key, double size = double.NaN, bool isActive = true, bool isIndeterminate = true, double value = 0d)
        {
            Assert.That(this.window, Is.Not.Null);

            var ring = new ProgressRing
                       {
                           Style = (Style)Application.Current.FindResource(key),
                           IsActive = isActive,
                           IsIndeterminate = isIndeterminate,
                           Value = value,
                           HorizontalAlignment = HorizontalAlignment.Left,
                           VerticalAlignment = VerticalAlignment.Top
                       };

            if (!double.IsNaN(size))
            {
                ring.Width = size;
                ring.Height = size;
            }

            this.window!.Content = ring;
            this.Settle();

            return ring;
        }

        private void Settle()
        {
            this.window!.UpdateLayout();
            ClipAssert.Pump();
        }
    }
}
