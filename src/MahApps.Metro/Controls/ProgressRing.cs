// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using MahApps.Metro.Automation.Peers;
using MahApps.Metro.ValueBoxes;

namespace MahApps.Metro.Controls
{
    [TemplatePart(Name = PART_TurningArc, Type = typeof(Shape))]
    [TemplatePart(Name = PART_TurningTrack, Type = typeof(Shape))]
    [TemplateVisualState(Name = "Large", GroupName = "SizeStates")]
    [TemplateVisualState(Name = "Small", GroupName = "SizeStates")]
    [TemplateVisualState(Name = "Inactive", GroupName = "ActiveStates")]
    [TemplateVisualState(Name = "Active", GroupName = "ActiveStates")]
    public class ProgressRing : RangeBase
    {
        private const string TurningStoryboardKey = "TurningStoryboard";
        private const string PART_TurningArc = "PART_TurningArc";
        private const string PART_TurningTrack = "PART_TurningTrack";

        /// <summary>Identifies the <see cref="IsIndeterminate"/> dependency property.</summary>
        public static readonly DependencyProperty IsIndeterminateProperty
            = DependencyProperty.Register(nameof(IsIndeterminate),
                                          typeof(bool),
                                          typeof(ProgressRing),
                                          new PropertyMetadata(BooleanBoxes.TrueBox, OnIsIndeterminatePropertyChanged));

        /// <summary>
        /// Gets or sets whether the ring only says that something is going on, rather than how far
        /// along it is. Windows 10 never drew a ring that could say how far, so only a set that
        /// draws an arc has anything to do with this.
        /// </summary>
        public bool IsIndeterminate
        {
            get => (bool)this.GetValue(IsIndeterminateProperty);
            set => this.SetValue(IsIndeterminateProperty, BooleanBoxes.Box(value));
        }

        /// <summary>Identifies the <see cref="ArcStartAngle"/> dependency property.</summary>
        public static readonly DependencyProperty ArcStartAngleProperty
            = DependencyProperty.Register(nameof(ArcStartAngle),
                                          typeof(double),
                                          typeof(ProgressRing),
                                          new PropertyMetadata(0d, OnTurningArcChanged));

        /// <summary>
        /// Gets or sets where the arc of a ring that only says something is going on begins, in
        /// degrees clockwise from twelve. A template animates this and the one that says where the
        /// arc ends, and the ring turns the two of them into the dash that draws it.
        /// </summary>
        public double ArcStartAngle
        {
            get => (double)this.GetValue(ArcStartAngleProperty);
            set => this.SetValue(ArcStartAngleProperty, value);
        }

        /// <summary>Identifies the <see cref="ArcEndAngle"/> dependency property.</summary>
        public static readonly DependencyProperty ArcEndAngleProperty
            = DependencyProperty.Register(nameof(ArcEndAngle),
                                          typeof(double),
                                          typeof(ProgressRing),
                                          new PropertyMetadata(0d, OnTurningArcChanged));

        /// <summary>
        /// Gets or sets where the arc of a ring that only says something is going on ends, in
        /// degrees clockwise from twelve. See <see cref="ArcStartAngle"/>.
        /// </summary>
        public double ArcEndAngle
        {
            get => (double)this.GetValue(ArcEndAngleProperty);
            set => this.SetValue(ArcEndAngleProperty, value);
        }

        /// <summary>Identifies the <see cref="ArcFraction"/> dependency property.</summary>
        private static readonly DependencyPropertyKey ArcFractionPropertyKey
            = DependencyProperty.RegisterReadOnly(nameof(ArcFraction),
                                                  typeof(double),
                                                  typeof(ProgressRing),
                                                  new PropertyMetadata(0d));

        /// <summary>Identifies the <see cref="ArcFraction"/> dependency property.</summary>
        public static readonly DependencyProperty ArcFractionProperty = ArcFractionPropertyKey.DependencyProperty;

        /// <summary>
        /// Gets how much of the way round a ring that says how far along it is has drawn, from 0
        /// for one with nothing to show up to 1 for a full one. A template that draws the rest of
        /// the circle behind that arc multiplies this by the circumference of its own ring, turns
        /// it negative and hands the result to StrokeDashOffset.
        /// </summary>
        public double ArcFraction
        {
            get => (double)this.GetValue(ArcFractionProperty);
            protected set => this.SetValue(ArcFractionPropertyKey, value);
        }

        /// <summary>Identifies the <see cref="ArcRemainder"/> dependency property.</summary>
        private static readonly DependencyPropertyKey ArcRemainderPropertyKey
            = DependencyProperty.RegisterReadOnly(nameof(ArcRemainder),
                                                  typeof(double),
                                                  typeof(ProgressRing),
                                                  new PropertyMetadata(1d));

        /// <summary>Identifies the <see cref="ArcRemainder"/> dependency property.</summary>
        public static readonly DependencyProperty ArcRemainderProperty = ArcRemainderPropertyKey.DependencyProperty;

        /// <summary>
        /// Gets how much of the way round a ring that says how far along it is leaves undrawn,
        /// from 1 for one with nothing to show down to 0 for a full one. A template that draws
        /// that arc as one dashed circle multiplies this by the circumference of its own ring and
        /// hands the result to StrokeDashOffset.
        /// </summary>
        public double ArcRemainder
        {
            get => (double)this.GetValue(ArcRemainderProperty);
            protected set => this.SetValue(ArcRemainderPropertyKey, value);
        }

        /// <summary>Identifies the <see cref="BindableWidth"/> dependency property.</summary>
        private static readonly DependencyPropertyKey BindableWidthPropertyKey
            = DependencyProperty.RegisterReadOnly(nameof(BindableWidth),
                                                  typeof(double),
                                                  typeof(ProgressRing),
                                                  new PropertyMetadata(default(double), OnBindableWidthPropertyChanged));

        /// <summary>Identifies the <see cref="BindableWidth"/> dependency property.</summary>
        public static readonly DependencyProperty BindableWidthProperty = BindableWidthPropertyKey.DependencyProperty;

        public double BindableWidth
        {
            get => (double)this.GetValue(BindableWidthProperty);
            protected set => this.SetValue(BindableWidthPropertyKey, value);
        }

        /// <summary>Identifies the <see cref="IsActive"/> dependency property.</summary>
        public static readonly DependencyProperty IsActiveProperty
            = DependencyProperty.Register(nameof(IsActive),
                                          typeof(bool),
                                          typeof(ProgressRing),
                                          new PropertyMetadata(BooleanBoxes.TrueBox, OnIsActivePropertyChanged));

        public bool IsActive
        {
            get => (bool)this.GetValue(IsActiveProperty);
            set => this.SetValue(IsActiveProperty, BooleanBoxes.Box(value));
        }

        /// <summary>Identifies the <see cref="IsLarge"/> dependency property.</summary>
        public static readonly DependencyProperty IsLargeProperty
            = DependencyProperty.Register(nameof(IsLarge),
                                          typeof(bool),
                                          typeof(ProgressRing),
                                          new PropertyMetadata(BooleanBoxes.TrueBox, OnIsLargePropertyChanged));

        public bool IsLarge
        {
            get => (bool)this.GetValue(IsLargeProperty);
            set => this.SetValue(IsLargeProperty, BooleanBoxes.Box(value));
        }

        /// <summary>Identifies the <see cref="MaxSideLength"/> dependency property.</summary>
        private static readonly DependencyPropertyKey MaxSideLengthPropertyKey
            = DependencyProperty.RegisterReadOnly(nameof(MaxSideLength),
                                                  typeof(double),
                                                  typeof(ProgressRing),
                                                  new PropertyMetadata(default(double)));

        /// <summary>Identifies the <see cref="MaxSideLength"/> dependency property.</summary>
        public static readonly DependencyProperty MaxSideLengthProperty = MaxSideLengthPropertyKey.DependencyProperty;

        public double MaxSideLength
        {
            get => (double)this.GetValue(MaxSideLengthProperty);
            protected set => this.SetValue(MaxSideLengthPropertyKey, value);
        }

        /// <summary>Identifies the <see cref="EllipseDiameter"/> dependency property.</summary>
        private static readonly DependencyPropertyKey EllipseDiameterPropertyKey
            = DependencyProperty.RegisterReadOnly(nameof(EllipseDiameter),
                                                  typeof(double),
                                                  typeof(ProgressRing),
                                                  new PropertyMetadata(default(double)));

        /// <summary>Identifies the <see cref="EllipseDiameter"/> dependency property.</summary>
        public static readonly DependencyProperty EllipseDiameterProperty = EllipseDiameterPropertyKey.DependencyProperty;

        public double EllipseDiameter
        {
            get => (double)this.GetValue(EllipseDiameterProperty);
            protected set => this.SetValue(EllipseDiameterPropertyKey, value);
        }

        /// <summary>Identifies the <see cref="EllipseOffset"/> dependency property.</summary>
        private static readonly DependencyPropertyKey EllipseOffsetPropertyKey
            = DependencyProperty.RegisterReadOnly(nameof(EllipseOffset),
                                                  typeof(Thickness),
                                                  typeof(ProgressRing),
                                                  new PropertyMetadata(default(Thickness)));

        /// <summary>Identifies the <see cref="EllipseOffset"/> dependency property.</summary>
        public static readonly DependencyProperty EllipseOffsetProperty = EllipseOffsetPropertyKey.DependencyProperty;

        public Thickness EllipseOffset
        {
            get => (Thickness)this.GetValue(EllipseOffsetProperty);
            protected set => this.SetValue(EllipseOffsetPropertyKey, value);
        }

        /// <summary>Identifies the <see cref="EllipseDiameterScale"/> dependency property.</summary>
        public static readonly DependencyProperty EllipseDiameterScaleProperty
            = DependencyProperty.Register(nameof(EllipseDiameterScale),
                                          typeof(double),
                                          typeof(ProgressRing),
                                          new PropertyMetadata(1D));

        public double EllipseDiameterScale
        {
            get => (double)this.GetValue(EllipseDiameterScaleProperty);
            set => this.SetValue(EllipseDiameterScaleProperty, value);
        }

        private List<Action>? deferredActions = new List<Action>();
        private Shape? turningArc;
        private Storyboard? turningStoryboard;
        private Shape? turningTrack;

        static ProgressRing()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ProgressRing), new FrameworkPropertyMetadata(typeof(ProgressRing)));
            MaximumProperty.OverrideMetadata(typeof(ProgressRing), new FrameworkPropertyMetadata(100d));
            VisibilityProperty.OverrideMetadata(
                typeof(ProgressRing),
                new FrameworkPropertyMetadata(
                    (ringObject, e) =>
                        {
                            if (e.NewValue != e.OldValue)
                            {
                                var ring = ringObject as ProgressRing;

                                ring?.SetCurrentValue(IsActiveProperty, BooleanBoxes.Box((Visibility)e.NewValue == Visibility.Visible));
                            }
                        }));
        }

        public ProgressRing()
        {
            this.SizeChanged += this.OnSizeChanged;
        }

        private static void OnBindableWidthPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            if (!(dependencyObject is ProgressRing ring))
            {
                return;
            }

            var action = new Action(
                () =>
                    {
                        ring.SetEllipseDiameter((double)dependencyPropertyChangedEventArgs.NewValue);
                        ring.SetEllipseOffset((double)dependencyPropertyChangedEventArgs.NewValue);
                        ring.SetMaxSideLength((double)dependencyPropertyChangedEventArgs.NewValue);
                    });

            if (ring.deferredActions != null)
            {
                ring.deferredActions.Add(action);
            }
            else
            {
                action();
            }
        }

        private void SetMaxSideLength(double width)
        {
            this.SetValue(MaxSideLengthPropertyKey, width <= 20d ? 20d : width);
        }

        private void SetEllipseDiameter(double width)
        {
            this.SetValue(EllipseDiameterPropertyKey, (width / 8) * this.EllipseDiameterScale);
        }

        private void SetEllipseOffset(double width)
        {
            this.SetValue(EllipseOffsetPropertyKey, new Thickness(0, width / 2, 0, 0));
        }

        private static void OnIsLargePropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var ring = dependencyObject as ProgressRing;

            ring?.UpdateLargeState();
        }

        private void UpdateLargeState()
        {
            Action action;

            if (this.IsLarge)
            {
                action = () => VisualStateManager.GoToState(this, "Large", true);
            }
            else
            {
                action = () => VisualStateManager.GoToState(this, "Small", true);
            }

            if (this.deferredActions != null)
            {
                this.deferredActions.Add(action);
            }
            else
            {
                action();
            }
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs sizeChangedEventArgs)
        {
            this.SetValue(BindableWidthPropertyKey, this.ActualWidth);
        }

        private static void OnIsActivePropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var ring = dependencyObject as ProgressRing;

            ring?.UpdateActiveState();
        }

        private static void OnIsIndeterminatePropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            (dependencyObject as ProgressRing)?.UpdateTurning();
        }

        /// <summary>
        /// Runs the storyboard that moves the two angles of a turning arc, for as long as there is
        /// a turning arc to move. A storyboard from a visual state would run against the template
        /// rather than against the ring the angles belong to, so the ring starts this one itself.
        /// </summary>
        private void UpdateTurning()
        {
            if (this.turningStoryboard is null)
            {
                return;
            }

            if (this.IsActive && this.IsIndeterminate)
            {
                this.turningStoryboard.Begin(this, true);
            }
            else
            {
                this.turningStoryboard.Stop(this);
            }
        }

        private void UpdateActiveState()
        {
            this.UpdateTurning();

            Action action;

            if (this.IsActive)
            {
                action = () => VisualStateManager.GoToState(this, "Active", true);
            }
            else
            {
                action = () => VisualStateManager.GoToState(this, "Inactive", true);
            }

            if (this.deferredActions != null)
            {
                this.deferredActions.Add(action);
            }
            else
            {
                action();
            }
        }

        public override void OnApplyTemplate()
        {
            // make sure the states get updated
            this.UpdateLargeState();
            this.UpdateActiveState();

            base.OnApplyTemplate();

            this.turningArc = this.GetTemplateChild(PART_TurningArc) as Shape;
            this.turningTrack = this.GetTemplateChild(PART_TurningTrack) as Shape;
            this.turningStoryboard = this.TryFindResource(TurningStoryboardKey) as Storyboard;
            this.UpdateTurningArc();
            this.UpdateTurning();

            if (this.deferredActions != null)
            {
                foreach (var action in this.deferredActions)
                {
                    action();
                }
            }

            this.deferredActions = null;
        }

        /// <inheritdoc />
        protected override void OnValueChanged(double oldValue, double newValue)
        {
            base.OnValueChanged(oldValue, newValue);
            this.UpdateArcRemainder();
        }

        /// <inheritdoc />
        protected override void OnMinimumChanged(double oldMinimum, double newMinimum)
        {
            base.OnMinimumChanged(oldMinimum, newMinimum);
            this.UpdateArcRemainder();
        }

        /// <inheritdoc />
        protected override void OnMaximumChanged(double oldMaximum, double newMaximum)
        {
            base.OnMaximumChanged(oldMaximum, newMaximum);
            this.UpdateArcRemainder();
        }

        private static void OnTurningArcChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            (dependencyObject as ProgressRing)?.UpdateTurningArc();
        }

        /// <summary>
        /// Writes the arc between the two angles onto the shape that draws it, as a dash one turn
        /// of the ring long pushed round to where the arc begins. A dash cannot be animated the way
        /// an angle can, which is why the angles are what a template animates and this is what
        /// turns them into something to draw. The track takes the rest of the same turn, so that
        /// neither of the two ever runs under the other.
        /// </summary>
        private void UpdateTurningArc()
        {
            if (this.turningArc is null)
            {
                return;
            }

            // a dash is measured in strokes rather than in units, so one turn of the ring is its
            // own circumference over its own stroke. The ring is the one the shape is drawn on,
            // which keeps the arithmetic here free of whatever size a template chose.
            var stroke = this.turningArc.StrokeThickness;
            var radius = (this.turningArc.Width - stroke) / 2;

            if (stroke <= 0d || radius <= 0d)
            {
                return;
            }

            var turn = 2 * Math.PI * radius / stroke;
            var drawn = Math.Max(0d, Math.Min(360d, this.ArcEndAngle - this.ArcStartAngle)) / 360d;

            Write(this.turningArc, turn * drawn, turn * (1d - drawn), -turn * this.ArcStartAngle / 360d);
            Write(this.turningTrack, turn * (1d - drawn), turn * drawn, -turn * this.ArcEndAngle / 360d);
        }

        private static void Write(Shape? shape, double on, double off, double offset)
        {
            if (shape is null)
            {
                return;
            }

            // the collection is changed in place rather than swapped out, so that a ring that has
            // been turning for a while has not left a new one behind on every frame
            if (shape.StrokeDashArray is { Count: 2 } dashes && !dashes.IsFrozen)
            {
                dashes[0] = on;
                dashes[1] = off;
            }
            else
            {
                shape.StrokeDashArray = new DoubleCollection { on, off };
            }

            shape.SetCurrentValue(Shape.StrokeDashOffsetProperty, offset);
        }

        private void UpdateArcRemainder()
        {
            var range = this.Maximum - this.Minimum;
            var part = range > 0d ? (this.Value - this.Minimum) / range : 0d;

            var drawn = Math.Max(0d, Math.Min(1d, part));

            this.SetValue(ArcFractionPropertyKey, drawn);
            this.SetValue(ArcRemainderPropertyKey, 1d - drawn);
        }

        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new ProgressRingAutomationPeer(this);
        }
    }
}