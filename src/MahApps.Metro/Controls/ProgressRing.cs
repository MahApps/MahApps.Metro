using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace MahApps.Metro.Controls
{
    [TemplateVisualState(Name = "Large", GroupName = "SizeStates")]
    [TemplateVisualState(Name = "Small", GroupName = "SizeStates")]
    [TemplateVisualState(Name = "Inactive", GroupName = "ActiveStates")]
    [TemplateVisualState(Name = "Active", GroupName = "ActiveStates")]
    public class ProgressRing : Control
    {
        /// <summary>Identifies the <see cref="BindableWidth"/> dependency property.</summary>
        public static readonly DependencyPropertyKey BindableWidthPropertyKey
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
                                          new PropertyMetadata(true, OnIsActivePropertyChanged));

        public bool IsActive
        {
            get => (bool)this.GetValue(IsActiveProperty);
            set => this.SetValue(IsActiveProperty, value);
        }

        /// <summary>Identifies the <see cref="IsLarge"/> dependency property.</summary>
        public static readonly DependencyProperty IsLargeProperty
            = DependencyProperty.Register(nameof(IsLarge),
                                          typeof(bool),
                                          typeof(ProgressRing),
                                          new PropertyMetadata(true, OnIsLargePropertyChanged));

        public bool IsLarge
        {
            get => (bool)this.GetValue(IsLargeProperty);
            set => this.SetValue(IsLargeProperty, value);
        }

        /// <summary>Identifies the <see cref="MaxSideLength"/> dependency property.</summary>
        public static readonly DependencyPropertyKey MaxSideLengthPropertyKey
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
        public static readonly DependencyPropertyKey EllipseDiameterPropertyKey
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
        public static readonly DependencyPropertyKey EllipseOffsetPropertyKey
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

        private List<Action> deferredActions = new List<Action>();

        static ProgressRing()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ProgressRing), new FrameworkPropertyMetadata(typeof(ProgressRing)));
            VisibilityProperty.OverrideMetadata(
                typeof(ProgressRing),
                new FrameworkPropertyMetadata(
                    (ringObject, e) =>
                        {
                            if (e.NewValue != e.OldValue)
                            {
                                var ring = ringObject as ProgressRing;

                                ring?.SetCurrentValue(IsActiveProperty, (Visibility)e.NewValue == Visibility.Visible);
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

        private void UpdateActiveState()
        {
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
            if (this.deferredActions != null)
            {
                foreach (var action in this.deferredActions)
                {
                    action();
                }
            }

            this.deferredActions = null;
        }
    }
}