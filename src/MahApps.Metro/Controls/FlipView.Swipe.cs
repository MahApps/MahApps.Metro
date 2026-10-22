// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using MahApps.Metro.ValueBoxes;

namespace MahApps.Metro.Controls
{
    /// <summary>
    /// Flipping by dragging the item aside, with the mouse, a pen or a finger. Nothing moves until the
    /// pointer has gone the system drag distance along the orientation, the way a drag and drop waits,
    /// so a click or a tap on the item still reaches it. Let go past half the item, or with a flick in
    /// the same direction, and it flips; anything less and the item springs back.
    /// </summary>
    public partial class FlipView
    {
        /// <summary>Identifies the <see cref="IsSwipeEnabled"/> dependency property.</summary>
        public static readonly DependencyProperty IsSwipeEnabledProperty
            = DependencyProperty.Register(nameof(IsSwipeEnabled),
                                          typeof(bool),
                                          typeof(FlipView),
                                          new PropertyMetadata(BooleanBoxes.TrueBox, (d, _) => ((FlipView)d).CancelSwipe()));

        /// <summary>
        /// Gets or sets whether the item can be dragged aside to flip to the next or the previous one.
        /// </summary>
        [Bindable(true)]
        [Category(AppName.MahApps)]
        public bool IsSwipeEnabled
        {
            get => (bool)this.GetValue(IsSwipeEnabledProperty);
            set => this.SetValue(IsSwipeEnabledProperty, BooleanBoxes.Box(value));
        }

        /// <summary>The speed in device independent pixels per millisecond from which letting go counts as a flick.</summary>
        private const double SwipeFlickVelocity = 0.5;

        /// <summary>How far back the speed at the end of a swipe is taken from.</summary>
        private const int SwipeVelocityWindow = 100;

        /// <summary>How much of the drag shows where there is nothing to flip to.</summary>
        private const double SwipeEdgeResistance = 1d / 3d;

        private static readonly TimeSpan SwipeLeaveMinimumDuration = TimeSpan.FromMilliseconds(50);
        private static readonly TimeSpan SwipeLeaveMaximumDuration = TimeSpan.FromMilliseconds(200);
        private static readonly TimeSpan SwipeSettleDuration = TimeSpan.FromMilliseconds(300);

        private enum SwipeState
        {
            None,
            Pending,
            Dragging,
            Leaving
        }

        private readonly List<KeyValuePair<int, double>> swipeSamples = new();
        private TranslateTransform? swipeTransform;
        private SwipeState swipeState;
        private TouchDevice? swipeTouch;
        private Point swipeStart;
        private double swipeOrigin;
        private double swipeOffset;

        private DependencyProperty SwipeOffsetProperty => this.Orientation == Orientation.Horizontal ? TranslateTransform.XProperty : TranslateTransform.YProperty;

        private double SwipeExtent => this.presenter is null ? 0d : this.Orientation == Orientation.Horizontal ? this.presenter.ActualWidth : this.presenter.ActualHeight;

        /// <inheritdoc />
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);

            if (!e.Handled && !IsPromotedFromTouch(e))
            {
                this.TryBeginSwipe(e.OriginalSource as DependencyObject, e.GetPosition(this), null);
            }
        }

        /// <inheritdoc />
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (this.swipeTouch is not null || IsPromotedFromTouch(e))
            {
                return;
            }

            if (e.LeftButton != MouseButtonState.Pressed)
            {
                // the button went up somewhere we never heard of
                if (this.swipeState == SwipeState.Pending)
                {
                    this.swipeState = SwipeState.None;
                }

                return;
            }

            var wasDragging = this.swipeState == SwipeState.Dragging;
            if (this.MoveSwipe(e.GetPosition(this), e.Timestamp))
            {
                if (!wasDragging)
                {
                    this.CaptureMouse();
                }

                e.Handled = true;
            }
        }

        /// <inheritdoc />
        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonUp(e);

            if (this.swipeTouch is null && !IsPromotedFromTouch(e) && this.EndSwipe(e.GetPosition(this), e.Timestamp))
            {
                this.ReleaseMouseCapture();
                e.Handled = true;
            }
        }

        /// <inheritdoc />
        protected override void OnLostMouseCapture(MouseEventArgs e)
        {
            base.OnLostMouseCapture(e);

            // the event comes up from the item as well, when something in there lets go of the mouse
            if (ReferenceEquals(e.OriginalSource, this) && this.swipeTouch is null)
            {
                this.CancelSwipe();
            }
        }

        /// <inheritdoc />
        protected override void OnTouchDown(TouchEventArgs e)
        {
            base.OnTouchDown(e);

            // the touch is left alone until it turns into a swipe, so it still becomes a click on whatever it went down on
            if (!e.Handled)
            {
                this.TryBeginSwipe(e.OriginalSource as DependencyObject, e.GetTouchPoint(this).Position, e.TouchDevice);
            }
        }

        /// <inheritdoc />
        protected override void OnTouchMove(TouchEventArgs e)
        {
            base.OnTouchMove(e);

            if (!ReferenceEquals(e.TouchDevice, this.swipeTouch))
            {
                return;
            }

            var wasDragging = this.swipeState == SwipeState.Dragging;
            if (this.MoveSwipe(e.GetTouchPoint(this).Position, e.Timestamp))
            {
                if (!wasDragging)
                {
                    this.CaptureTouch(e.TouchDevice);

                    // whatever the touch went down on took the mouse it was promoted to, and would
                    // take the end of the swipe for a click
                    if (Mouse.Captured is DependencyObject captured && this.presenter is not null && captured.IsDescendantOf(this.presenter))
                    {
                        Mouse.Capture(null);
                    }
                }

                e.Handled = true;
            }
        }

        /// <inheritdoc />
        protected override void OnTouchUp(TouchEventArgs e)
        {
            base.OnTouchUp(e);

            if (!ReferenceEquals(e.TouchDevice, this.swipeTouch))
            {
                return;
            }

            var touch = e.TouchDevice;
            if (this.EndSwipe(e.GetTouchPoint(this).Position, e.Timestamp))
            {
                this.ReleaseTouchCapture(touch);
                e.Handled = true;
            }
        }

        /// <inheritdoc />
        protected override void OnLostTouchCapture(TouchEventArgs e)
        {
            base.OnLostTouchCapture(e);

            if (ReferenceEquals(e.OriginalSource, this) && ReferenceEquals(e.TouchDevice, this.swipeTouch))
            {
                this.CancelSwipe();
            }
        }

        private static bool IsPromotedFromTouch(MouseEventArgs e)
        {
            return e.StylusDevice?.TabletDevice?.Type == TabletDeviceType.Touch;
        }

        /// <summary>
        /// The presenter stays where it is and cuts off what goes past its edges, while the root of its
        /// template carries the drag. Cut off any further out, the index of the Metro look would go too.
        /// </summary>
        private void AttachSwipeTransform()
        {
            this.swipeTransform = null;

            if (this.presenter is null)
            {
                return;
            }

            this.presenter.ApplyTemplate();
            if (VisualTreeHelper.GetChildrenCount(this.presenter) > 0 && VisualTreeHelper.GetChild(this.presenter, 0) is UIElement root)
            {
                this.swipeTransform = new TranslateTransform();
                root.RenderTransform = this.swipeTransform;
                this.presenter.SetCurrentValue(ClipToBoundsProperty, BooleanBoxes.TrueBox);
            }
        }

        private bool CanFlip(bool forward)
        {
            var count = this.Items.Count;
            return count > 1 && (this.CircularNavigation || (forward ? this.SelectedIndex < count - 1 : this.SelectedIndex > 0));
        }

        private void TryBeginSwipe(DependencyObject? source, Point position, TouchDevice? touch)
        {
            if (this.swipeState is SwipeState.Dragging or SwipeState.Leaving)
            {
                return;
            }

            this.swipeState = SwipeState.None;
            this.swipeTouch = null;

            if (!this.IsSwipeEnabled
                || this.presenter is null
                || this.swipeTransform is null
                || this.Items.Count < 2
                || source is null
                || !source.IsDescendantOf(this.presenter)
                || this.IsInsideScrollableContent(source))
            {
                return;
            }

            this.swipeState = SwipeState.Pending;
            this.swipeTouch = touch;
            this.swipeStart = position;
        }

        /// <summary>
        /// Content that scrolls along the same direction itself keeps the drag, as a list in the item
        /// that is longer than the item would otherwise never get scrolled by hand.
        /// </summary>
        private bool IsInsideScrollableContent(DependencyObject source)
        {
            var horizontal = this.Orientation == Orientation.Horizontal;

            for (var current = source; current is not null && !ReferenceEquals(current, this.presenter); current = current.GetParentObject())
            {
                if (current is ScrollViewer scrollViewer
                    && (horizontal ? scrollViewer.ScrollableWidth : scrollViewer.ScrollableHeight) > 0
                    && scrollViewer.PanningMode != (horizontal ? PanningMode.VerticalOnly : PanningMode.HorizontalOnly)
                    && scrollViewer.PanningMode != PanningMode.None)
                {
                    return true;
                }
            }

            return false;
        }

        /// <returns><see langword="true"/> while the item is being dragged, so the move belongs to the swipe.</returns>
        private bool MoveSwipe(Point position, int timestamp)
        {
            if (this.swipeState == SwipeState.Pending)
            {
                var delta = position - this.swipeStart;
                var horizontal = this.Orientation == Orientation.Horizontal;
                var along = Math.Abs(horizontal ? delta.X : delta.Y);
                var across = Math.Abs(horizontal ? delta.Y : delta.X);
                var alongThreshold = horizontal ? SystemParameters.MinimumHorizontalDragDistance : SystemParameters.MinimumVerticalDragDistance;
                var acrossThreshold = horizontal ? SystemParameters.MinimumVerticalDragDistance : SystemParameters.MinimumHorizontalDragDistance;

                if (across >= acrossThreshold && across > along)
                {
                    // that is somebody scrolling or dragging something else, not flipping
                    this.swipeState = SwipeState.None;
                    this.swipeTouch = null;
                    return false;
                }

                if (along < alongThreshold)
                {
                    return false;
                }

                // starting from here rather than from where the pointer went down keeps the item from jumping by the threshold
                this.swipeState = SwipeState.Dragging;
                this.swipeStart = position;
                this.swipeOrigin = this.StopSwipeAnimation();
                this.swipeSamples.Clear();
            }

            if (this.swipeState != SwipeState.Dragging)
            {
                return false;
            }

            this.DragSwipe(position, timestamp);
            return true;
        }

        private void DragSwipe(Point position, int timestamp)
        {
            var travelled = this.Orientation == Orientation.Horizontal ? position.X - this.swipeStart.X : position.Y - this.swipeStart.Y;
            var offset = this.swipeOrigin + travelled;
            var extent = this.SwipeExtent;

            if (!this.CanFlip(offset < 0))
            {
                offset *= SwipeEdgeResistance;
            }

            this.swipeOffset = Math.Max(-extent, Math.Min(extent, offset));
            this.swipeTransform!.SetValue(this.SwipeOffsetProperty, this.swipeOffset);

            this.swipeSamples.Add(new KeyValuePair<int, double>(timestamp, this.swipeOffset));
            while (this.swipeSamples.Count > 2 && timestamp - this.swipeSamples[0].Key > SwipeVelocityWindow)
            {
                this.swipeSamples.RemoveAt(0);
            }
        }

        /// <returns><see langword="true"/> if the item was being dragged, so letting go belongs to the swipe.</returns>
        private bool EndSwipe(Point position, int timestamp)
        {
            if (this.swipeState == SwipeState.Pending)
            {
                this.swipeState = SwipeState.None;
                this.swipeTouch = null;
                return false;
            }

            if (this.swipeState != SwipeState.Dragging)
            {
                return false;
            }

            this.DragSwipe(position, timestamp);

            var velocity = this.SwipeVelocity(timestamp);
            var offset = this.swipeOffset;
            var forward = offset < 0;
            var flicked = Math.Abs(velocity) >= SwipeFlickVelocity && Math.Sign(velocity) == Math.Sign(offset);

            // the state goes first, since giving the capture back comes round as a lost capture
            this.swipeTouch = null;

            // an item that never moved reaches no half and has no flick in its direction, so it stays
            var extent = this.SwipeExtent;
            if (extent > 0 && this.CanFlip(forward) && (Math.Abs(offset) >= extent / 2 || flicked))
            {
                this.swipeState = SwipeState.Leaving;
                this.LeaveSwipe(forward, Math.Abs(velocity));
            }
            else
            {
                this.swipeState = SwipeState.None;
                this.AnimateSwipeOffset(offset, 0d, SwipeSettleDuration, null);
            }

            return true;
        }

        private double SwipeVelocity(int timestamp)
        {
            if (this.swipeSamples.Count < 2)
            {
                return 0d;
            }

            var first = this.swipeSamples[0];
            var last = this.swipeSamples[this.swipeSamples.Count - 1];

            // a pointer that stood still before letting go has no speed left
            if (timestamp - this.swipeSamples[this.swipeSamples.Count - 2].Key > SwipeVelocityWindow)
            {
                return 0d;
            }

            return (last.Value - first.Value) / Math.Max(1, last.Key - first.Key);
        }

        /// <summary>
        /// Sends the item off the side it was dragged to, then brings the next one in from the other
        /// side. The template shows one item at a time, so the two cannot travel side by side.
        /// </summary>
        private void LeaveSwipe(bool forward, double speed)
        {
            var extent = this.SwipeExtent;
            var target = forward ? -extent : extent;
            var remaining = Math.Abs(target - this.swipeOffset);
            var milliseconds = remaining / Math.Max(speed, extent / SwipeLeaveMaximumDuration.TotalMilliseconds);

            // a fast flick would be gone at once, and on .NET Framework that rounds down to an animation of no length, which never came to an end there
            var duration = TimeSpan.FromMilliseconds(Math.Max(SwipeLeaveMinimumDuration.TotalMilliseconds, Math.Min(SwipeLeaveMaximumDuration.TotalMilliseconds, milliseconds)));

            this.AnimateSwipeOffset(this.swipeOffset, target, duration, () =>
                {
                    this.swipeState = SwipeState.None;

                    // the item is already gone, so the new one simply takes its place
                    if (forward)
                    {
                        this.GoForward(TransitionType.Normal);
                    }
                    else
                    {
                        this.GoBack(TransitionType.Normal);
                    }

                    this.AnimateSwipeOffset(-target, 0d, SwipeSettleDuration, null);
                });
        }

        private void AnimateSwipeOffset(double from, double to, TimeSpan duration, Action? completed)
        {
            if (this.swipeTransform is null)
            {
                completed?.Invoke();
                return;
            }

            var property = this.SwipeOffsetProperty;
            var animation = new DoubleAnimation(from, to, new Duration(duration), FillBehavior.Stop);

            // leaving keeps its pace, arriving and springing back slow down towards the end
            if (completed is null)
            {
                animation.EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut };
            }

            var transform = this.swipeTransform;
            animation.Completed += (_, _) =>
                {
                    // a newer swipe or template has taken over in the meantime
                    if (ReferenceEquals(transform, this.swipeTransform) && this.swipeState != SwipeState.Dragging)
                    {
                        completed?.Invoke();
                    }
                };

            transform.SetValue(property, to);
            transform.BeginAnimation(property, animation);
        }

        /// <returns>Where the item was when the animation got stopped.</returns>
        private double StopSwipeAnimation()
        {
            if (this.swipeTransform is null)
            {
                return 0d;
            }

            var property = this.SwipeOffsetProperty;
            var current = (double)this.swipeTransform.GetValue(property);
            this.swipeTransform.BeginAnimation(property, null);
            this.swipeTransform.SetValue(property, current);
            return current;
        }

        /// <summary>Lets go of a swipe that was interrupted, and the item springs back.</summary>
        private void CancelSwipe()
        {
            if (this.swipeState == SwipeState.Dragging)
            {
                this.swipeState = SwipeState.None;
                this.swipeTouch = null;
                this.AnimateSwipeOffset(this.swipeOffset, 0d, SwipeSettleDuration, null);
            }
            else if (this.swipeState == SwipeState.Pending)
            {
                this.swipeState = SwipeState.None;
                this.swipeTouch = null;
            }
        }

        /// <summary>Puts the item straight back, for a new template or a change of direction.</summary>
        private void ResetSwipe()
        {
            this.swipeState = SwipeState.None;
            this.swipeTouch = null;

            if (this.swipeTransform is not null)
            {
                this.swipeTransform.BeginAnimation(TranslateTransform.XProperty, null);
                this.swipeTransform.BeginAnimation(TranslateTransform.YProperty, null);
                this.swipeTransform.X = 0d;
                this.swipeTransform.Y = 0d;
            }
        }
    }
}
