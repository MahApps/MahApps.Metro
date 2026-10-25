// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using MahApps.Metro.Controls;
using MahApps.Metro.Tests.TestHelpers;
using NUnit.Framework;

namespace MahApps.Metro.Tests.Tests
{
    /// <summary>
    /// GH-2508: a FlipView flips when its item is dragged aside, the way it does on a tablet. The tests
    /// drag with a finger of their own making, since a test cannot say where the mouse is; the mouse
    /// goes through the same swipe once it has been told where the pointer went.
    /// </summary>
    [TestFixture]
    public class FlipViewSwipeTests : WindowTestFixture<TestWindow>
    {
        [Test]
        [Description("Dragged past half its width and let go slowly, the item gives way to the next one, which comes in and settles where the old one was.")]
        public async Task DraggingPastHalfFlipsToTheNextItem()
        {
            var flipView = this.Show();

            var finger = new Finger(flipView);
            finger.Down(new Point(320, 150));
            finger.Move(new Point(310, 150));
            finger.Move(new Point(200, 150));
            finger.Move(new Point(60, 150));
            await Task.Delay(150);
            finger.Up();

            await WaitUntil(() => flipView.SelectedIndex == 1, "the second item should have been flipped to");
            await WaitUntil(() => Offset(flipView).X == 0d, "and it should have come to rest where the first one was");
        }

        [Test]
        [Description("A short drag let go of slowly is no flip, and the item springs back to where it was.")]
        public async Task AShortSlowDragSpringsBack()
        {
            var flipView = this.Show();

            var finger = new Finger(flipView);
            finger.Down(new Point(250, 150));
            finger.Move(new Point(240, 150));
            finger.Move(new Point(200, 150));

            Assert.That(Offset(flipView).X, Is.LessThan(0d), "the item should follow the finger");

            await Task.Delay(150);
            finger.Up();

            await WaitUntil(() => Offset(flipView).X == 0d, "the item should have sprung back");
            Assert.That(flipView.SelectedIndex, Is.EqualTo(0));
        }

        [Test]
        [Description("A quick flick flips even though the item went nowhere near half its width.")]
        public async Task AFlickFlipsAfterAShortDrag()
        {
            var flipView = this.Show();

            var finger = new Finger(flipView);
            finger.Down(new Point(250, 150));
            finger.Move(new Point(240, 150));
            finger.Move(new Point(200, 150));
            finger.Up();

            await WaitUntil(() => flipView.SelectedIndex == 1, "the flick should have flipped to the second item");
        }

        [Test]
        [Description("A flick back against the way the item was dragged is a change of mind, not a flip.")]
        public async Task AFlickBackIsNoFlip()
        {
            var flipView = this.Show();

            var finger = new Finger(flipView);
            finger.Down(new Point(250, 150));
            finger.Move(new Point(240, 150));
            finger.Move(new Point(140, 150));
            await Task.Delay(150);
            finger.Move(new Point(150, 150));
            finger.Move(new Point(200, 150));
            finger.Up();

            await WaitUntil(() => Offset(flipView).X == 0d, "the item should have sprung back");
            Assert.That(flipView.SelectedIndex, Is.EqualTo(0));
        }

        [Test]
        [Description("With nothing after the last item the drag only gives a little, and letting go never flips.")]
        public async Task TheLastItemOnlyGivesALittle()
        {
            var flipView = this.Show();
            flipView.SelectedIndex = 2;
            Settle(flipView);

            var finger = new Finger(flipView);
            finger.Down(new Point(320, 150));
            finger.Move(new Point(310, 150));
            finger.Move(new Point(10, 150));

            Assert.That(Offset(flipView).X, Is.EqualTo(-100d).Within(0.5), "a third of the 300 pixels dragged should show");

            finger.Up();

            await WaitUntil(() => Offset(flipView).X == 0d, "the item should have sprung back");
            Assert.That(flipView.SelectedIndex, Is.EqualTo(2));
        }

        [Test]
        [Description("Going round is going round, the drag included.")]
        public async Task CircularNavigationFlipsFromTheLastItemToTheFirst()
        {
            var flipView = this.Show();
            flipView.CircularNavigation = true;
            flipView.SelectedIndex = 2;
            Settle(flipView);

            var finger = new Finger(flipView);
            finger.Down(new Point(320, 150));
            finger.Move(new Point(310, 150));
            finger.Move(new Point(10, 150));
            finger.Up();

            await WaitUntil(() => flipView.SelectedIndex == 0, "the first item should follow the last one");
        }

        [Test]
        [Description("Dragging back from the second item flips to the first.")]
        public async Task DraggingTheOtherWayFlipsBack()
        {
            var flipView = this.Show();
            flipView.SelectedIndex = 1;
            Settle(flipView);

            var finger = new Finger(flipView);
            finger.Down(new Point(80, 150));
            finger.Move(new Point(90, 150));
            finger.Move(new Point(380, 150));
            finger.Up();

            await WaitUntil(() => flipView.SelectedIndex == 0, "the first item should have been flipped back to");
        }

        [Test]
        [Description("A FlipView standing on end flips by dragging up and down.")]
        public async Task AVerticalFlipViewFlipsByDraggingUp()
        {
            var flipView = this.Show();
            flipView.Orientation = Orientation.Vertical;
            Settle(flipView);

            var finger = new Finger(flipView);
            finger.Down(new Point(200, 240));
            finger.Move(new Point(200, 230));
            finger.Move(new Point(200, 0));
            finger.Up();

            await WaitUntil(() => flipView.SelectedIndex == 1, "the second item should have been flipped to");
            Assert.That(Offset(flipView).X, Is.EqualTo(0d), "nothing should have moved sideways");
        }

        [Test]
        [Description("Nothing moves until the finger has gone the drag distance, so a tap stays a tap.")]
        public void NothingMovesBeforeTheDragDistance()
        {
            var flipView = this.Show();

            var finger = new Finger(flipView);
            finger.Down(new Point(250, 150));
            finger.Move(new Point(250 - (SystemParameters.MinimumHorizontalDragDistance / 2), 150));

            Assert.That(Offset(flipView).X, Is.EqualTo(0d));

            finger.Up();

            Assert.That(flipView.SelectedIndex, Is.EqualTo(0));
        }

        [Test]
        [Description("A finger going up or down across a FlipView that flips sideways is scrolling something, not flipping.")]
        public void MovingAcrossIsNoSwipe()
        {
            var flipView = this.Show();

            var finger = new Finger(flipView);
            finger.Down(new Point(250, 150));
            finger.Move(new Point(248, 120));
            finger.Move(new Point(100, 120));

            Assert.That(Offset(flipView).X, Is.EqualTo(0d), "the item should not have moved");

            finger.Up();

            Assert.That(flipView.SelectedIndex, Is.EqualTo(0));
        }

        [Test]
        [Description("With swiping switched off the item stays put however far it is dragged.")]
        public async Task SwipingSwitchedOffLeavesTheItemAlone()
        {
            var flipView = this.Show();
            flipView.IsSwipeEnabled = false;

            var finger = new Finger(flipView);
            finger.Down(new Point(320, 150));
            finger.Move(new Point(310, 150));
            finger.Move(new Point(10, 150));

            Assert.That(Offset(flipView).X, Is.EqualTo(0d));

            finger.Up();
            await Task.Delay(400);

            Assert.That(flipView.SelectedIndex, Is.EqualTo(0));
        }

        private FlipView Show()
        {
            Assert.That(this.window, Is.Not.Null);

            var flipView = new FlipView { Width = 400, Height = 300, IsBannerEnabled = false };
            foreach (var brush in new[] { Brushes.SteelBlue, Brushes.Goldenrod, Brushes.SeaGreen })
            {
                flipView.Items.Add(new FlipViewItem { Content = new Border { Background = brush } });
            }

            this.window!.Content = flipView;
            Settle(flipView);

            Assert.That(flipView.IsLoaded, Is.True, "the control should be up before a test looks at it");
            Assert.That(flipView.SelectedIndex, Is.EqualTo(0));

            return flipView;
        }

        private static void Settle(FrameworkElement element)
        {
            element.UpdateLayout();
            ClipAssert.Pump();
        }

        private static TranslateTransform Offset(FlipView flipView)
        {
            var presenter = flipView.FindChild<TransitioningContentControl>("PART_Presenter")!;
            var root = (UIElement)VisualTreeHelper.GetChild(presenter, 0);
            Assert.That(root.RenderTransform, Is.InstanceOf<TranslateTransform>(), "the root of the presenter should carry the offset of the swipe");
            return (TranslateTransform)root.RenderTransform;
        }

        private static async Task WaitUntil(Func<bool> condition, string what)
        {
            var watch = Stopwatch.StartNew();
            while (!condition())
            {
                if (watch.ElapsedMilliseconds > 3000)
                {
                    Assert.Fail(what);
                }

                await Task.Delay(20);
            }
        }

        /// <summary>A touch the test moves itself, at points given in the coordinates of the flip view.</summary>
        private sealed class Finger : TouchDevice
        {
            private readonly FlipView flipView;
            private Point position;

            public Finger(FlipView flipView)
                : base(4711)
            {
                this.flipView = flipView;
            }

            public void Down(Point point)
            {
                this.position = point;
                this.SetActiveSource(PresentationSource.FromVisual(this.flipView));
                this.Activate();
                this.ReportDown();
            }

            public void Move(Point point)
            {
                this.position = point;
                this.ReportMove();
            }

            public void Up()
            {
                this.ReportUp();
                this.Deactivate();
            }

            public override TouchPoint GetTouchPoint(IInputElement? relativeTo)
            {
                var root = (UIElement)PresentationSource.FromVisual(this.flipView)!.RootVisual;
                var point = this.flipView.TranslatePoint(this.position, relativeTo as UIElement ?? root);
                return new TouchPoint(this, point, new Rect(point, new Size(1, 1)), TouchAction.Move);
            }

            public override TouchPointCollection GetIntermediateTouchPoints(IInputElement? relativeTo)
            {
                // the finger jumps from point to point, so the one it is at now is all there is between
                return new TouchPointCollection { this.GetTouchPoint(relativeTo) };
            }
        }
    }
}
