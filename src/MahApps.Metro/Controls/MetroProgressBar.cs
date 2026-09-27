// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace MahApps.Metro.Controls
{
    /// <summary>
    /// A metrofied ProgressBar.
    /// <see cref="ProgressBar"/>
    /// </summary>
    public class MetroProgressBar : ProgressBar
    {
        /// <summary>Identifies the <see cref="EllipseDiameter"/> dependency property.</summary>
        public static readonly DependencyProperty EllipseDiameterProperty
            = DependencyProperty.Register(nameof(EllipseDiameter),
                                          typeof(double),
                                          typeof(MetroProgressBar),
                                          new PropertyMetadata(default(double)));

        /// <summary>
        /// Gets or sets the diameter of the ellipses used in the indeterminate animation.
        /// </summary>
        public double EllipseDiameter
        {
            get => (double)this.GetValue(EllipseDiameterProperty);
            set => this.SetValue(EllipseDiameterProperty, value);
        }

        /// <summary>Identifies the <see cref="EllipseOffset"/> dependency property.</summary>
        public static readonly DependencyProperty EllipseOffsetProperty =
            DependencyProperty.Register(nameof(EllipseOffset),
                                        typeof(double),
                                        typeof(MetroProgressBar),
                                        new PropertyMetadata(default(double)));

        /// <summary>
        /// Gets or sets the offset of the ellipses used in the indeterminate animation.
        /// </summary>
        public double EllipseOffset
        {
            get => (double)this.GetValue(EllipseOffsetProperty);
            set => this.SetValue(EllipseOffsetProperty, value);
        }

        private readonly object lockme = new object();
        private Storyboard? indeterminateStoryboard;

        static MetroProgressBar()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MetroProgressBar), new FrameworkPropertyMetadata(typeof(MetroProgressBar)));
            IsIndeterminateProperty.OverrideMetadata(typeof(MetroProgressBar), new FrameworkPropertyMetadata(OnIsIndeterminateChanged));
        }

        public MetroProgressBar()
        {
            this.IsVisibleChanged += this.VisibleChangedHandler;
        }

        private void VisibleChangedHandler(object sender, DependencyPropertyChangedEventArgs e)
        {
            // reset Storyboard if Visibility is set to Visible #1300
            if (this.IsIndeterminate)
            {
                ToggleIndeterminate(this, (bool)e.OldValue, (bool)e.NewValue);
            }
        }

        private static void OnIsIndeterminateChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            var bar = (MetroProgressBar)dependencyObject;
            if (bar.IsLoaded && bar.IsVisible)
            {
                ToggleIndeterminate(bar, (bool)e.OldValue, (bool)e.NewValue);
            }
        }

        private static void ToggleIndeterminate(MetroProgressBar bar, bool oldValue, bool newValue)
        {
            if (newValue == oldValue)
            {
                return;
            }

            var indeterminateState = bar.GetIndeterminate();
            var containingObject = bar.GetTemplateChild(ProgressBarEllipseAnimation.ContainingGridName) as FrameworkElement;
            if (indeterminateState != null && containingObject != null)
            {
                var resetAction = new Action(() =>
                    {
                        if (oldValue && indeterminateState.Storyboard != null)
                        {
                            // remove the previous storyboard from the Grid #1855
                            indeterminateState.Storyboard.Stop(containingObject);
                            indeterminateState.Storyboard.Remove(containingObject);
                        }

                        if (newValue)
                        {
                            bar.ResetStoryboard(bar.ActualSize(true), false);
                        }
                    });
                bar.Invoke(resetAction);
            }
        }

        private void SizeChangedHandler(object? sender, SizeChangedEventArgs? e)
        {
            var size = this.ActualSize(false);
            var bar = this;
            if (this.Visibility == Visibility.Visible && this.IsIndeterminate)
            {
                bar.ResetStoryboard(size, true);
            }
        }

        private double ActualSize(bool invalidateMeasureArrange)
        {
            if (invalidateMeasureArrange)
            {
                this.UpdateLayout();
                this.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                this.InvalidateArrange();
            }

            return this.Orientation == Orientation.Horizontal ? this.ActualWidth : this.ActualHeight;
        }

        private void ResetStoryboard(double width, bool removeOldStoryboard)
        {
            if (!this.IsIndeterminate)
            {
                return;
            }

            lock (this.lockme)
            {
                //reset the main double animation
                try
                {
                    var indeterminate = this.GetIndeterminate();

                    if (indeterminate != null && this.indeterminateStoryboard != null)
                    {
                        var newStoryboard = ProgressBarEllipseAnimation.Rebuild(this.indeterminateStoryboard, width);

                        var containingGrid = (FrameworkElement)this.GetTemplateChild(ProgressBarEllipseAnimation.ContainingGridName);

                        if (removeOldStoryboard && indeterminate.Storyboard != null)
                        {
                            // remove the previous storyboard from the Grid #1855
                            indeterminate.Storyboard.Stop(containingGrid);
                            indeterminate.Storyboard.Remove(containingGrid);
                        }

                        indeterminate.Storyboard = newStoryboard;

                        indeterminate.Storyboard?.Begin(containingGrid, true);
                    }
                }
                catch (Exception)
                {
                    //we just ignore 
                }
            }
        }

        private VisualState? GetIndeterminate()
        {
            var templateGrid = this.GetTemplateChild(ProgressBarEllipseAnimation.ContainingGridName) as FrameworkElement;
            if (templateGrid is null)
            {
                this.ApplyTemplate();
                templateGrid = this.GetTemplateChild(ProgressBarEllipseAnimation.ContainingGridName) as FrameworkElement;
                if (templateGrid is null)
                {
                    return null;
                }
            }

            return ProgressBarEllipseAnimation.FindIndeterminateState(templateGrid);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            lock (this.lockme)
            {
                this.indeterminateStoryboard = this.TryFindResource(ProgressBarEllipseAnimation.StoryboardKey) as Storyboard;
            }

            this.Loaded -= this.LoadedHandler;
            this.Loaded += this.LoadedHandler;
        }

        private void LoadedHandler(object sender, RoutedEventArgs routedEventArgs)
        {
            this.Loaded -= this.LoadedHandler;
            this.SizeChangedHandler(null, null);
            this.SizeChanged += this.SizeChangedHandler;
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);
            this.UpdateEllipseProperties();
        }

        private void UpdateEllipseProperties()
        {
            // Update the Ellipse properties to their default values
            // only if they haven't been user-set.
            var actualSize = this.ActualSize(true);
            if (actualSize > 0)
            {
                if (this.EllipseDiameter.Equals(0))
                {
                    this.SetCurrentValue(EllipseDiameterProperty, ProgressBarEllipseAnimation.DiameterFor(actualSize));
                }

                if (this.EllipseOffset.Equals(0))
                {
                    this.SetCurrentValue(EllipseOffsetProperty, ProgressBarEllipseAnimation.OffsetFor(actualSize));
                }
            }
        }
    }
}