using System;
using System.Linq;
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
        public static readonly DependencyProperty EllipseDiameterProperty
            = DependencyProperty.Register(nameof(EllipseDiameter),
                                          typeof(double),
                                          typeof(MetroProgressBar),
                                          new PropertyMetadata(default(double)));

        public static readonly DependencyProperty EllipseOffsetProperty =
            DependencyProperty.Register(nameof(EllipseOffset),
                                        typeof(double),
                                        typeof(MetroProgressBar),
                                        new PropertyMetadata(default(double)));

        private readonly object lockme = new object();
        private Storyboard indeterminateStoryboard;

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
            var containingObject = bar.GetTemplateChild("ContainingGrid") as FrameworkElement;
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

        /// <summary>
        /// Gets/sets the diameter of the ellipses used in the indeterminate animation.
        /// </summary>
        public double EllipseDiameter
        {
            get { return (double)this.GetValue(EllipseDiameterProperty); }
            set { this.SetValue(EllipseDiameterProperty, value); }
        }

        /// <summary>
        /// Gets/sets the offset of the ellipses used in the indeterminate animation.
        /// </summary>
        public double EllipseOffset
        {
            get { return (double)this.GetValue(EllipseOffsetProperty); }
            set { this.SetValue(EllipseOffsetProperty, value); }
        }

        private void SizeChangedHandler(object sender, SizeChangedEventArgs e)
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
                //perform calculations
                var containerAnimStart = this.CalcContainerAnimStart(width);
                var containerAnimEnd = this.CalcContainerAnimEnd(width);
                var ellipseAnimWell = this.CalcEllipseAnimWell(width);
                var ellipseAnimEnd = this.CalcEllipseAnimEnd(width);
                //reset the main double animation
                try
                {
                    var indeterminate = this.GetIndeterminate();

                    if (indeterminate != null && this.indeterminateStoryboard != null)
                    {
                        var newStoryboard = this.indeterminateStoryboard.Clone();
                        var doubleAnim = newStoryboard.Children.First(t => t.Name == "MainDoubleAnim");
                        doubleAnim.SetValue(DoubleAnimation.FromProperty, containerAnimStart);
                        doubleAnim.SetValue(DoubleAnimation.ToProperty, containerAnimEnd);

                        var namesOfElements = new[] { "E1", "E2", "E3", "E4", "E5" };
                        foreach (var elemName in namesOfElements)
                        {
                            var doubleAnimParent = (DoubleAnimationUsingKeyFrames)newStoryboard.Children.First(t => t.Name == elemName + "Anim");
                            DoubleKeyFrame first,
                                           second,
                                           third;
                            if (elemName == "E1")
                            {
                                first = doubleAnimParent.KeyFrames[1];
                                second = doubleAnimParent.KeyFrames[2];
                                third = doubleAnimParent.KeyFrames[3];
                            }
                            else
                            {
                                first = doubleAnimParent.KeyFrames[2];
                                second = doubleAnimParent.KeyFrames[3];
                                third = doubleAnimParent.KeyFrames[4];
                            }

                            first.Value = ellipseAnimWell;
                            second.Value = ellipseAnimWell;
                            third.Value = ellipseAnimEnd;
                            first.InvalidateProperty(DoubleKeyFrame.ValueProperty);
                            second.InvalidateProperty(DoubleKeyFrame.ValueProperty);
                            third.InvalidateProperty(DoubleKeyFrame.ValueProperty);

                            doubleAnimParent.InvalidateProperty(Storyboard.TargetPropertyProperty);
                            doubleAnimParent.InvalidateProperty(Storyboard.TargetNameProperty);
                        }

                        var containingGrid = (FrameworkElement)this.GetTemplateChild("ContainingGrid");

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

        private VisualState GetIndeterminate()
        {
            var templateGrid = this.GetTemplateChild("ContainingGrid") as FrameworkElement;
            if (templateGrid == null)
            {
                this.ApplyTemplate();
                templateGrid = this.GetTemplateChild("ContainingGrid") as FrameworkElement;
                if (templateGrid == null) return null;
            }
            var groups = VisualStateManager.GetVisualStateGroups(templateGrid);
            return groups?.OfType<VisualStateGroup>()
                         .SelectMany(group => group.States.OfType<VisualState>())
                         .FirstOrDefault(state => state.Name == "Indeterminate");
        }

        private void SetEllipseDiameter(double width)
        {
            this.SetCurrentValue(EllipseDiameterProperty, width <= 180 ? 4d : (width <= 280 ? 5d : 6d));
        }

        private void SetEllipseOffset(double width)
        {
            this.SetCurrentValue(EllipseOffsetProperty, width <= 180 ? 4d : (width <= 280 ? 7d : 9d));
        }

        private double CalcContainerAnimStart(double width)
        {
            return width <= 180 ? -34 : (width <= 280 ? -50.5 : -63);
        }

        private double CalcContainerAnimEnd(double width)
        {
            var firstPart = 0.4352 * width;
            return width <= 180 ? firstPart - 25.731 : (width <= 280 ? firstPart + 27.84 : firstPart + 58.862);
        }

        private double CalcEllipseAnimWell(double width)
        {
            return width * 1.0 / 3.0;
        }

        private double CalcEllipseAnimEnd(double width)
        {
            return width * 2.0 / 3.0;
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            lock (this.lockme)
            {
                this.indeterminateStoryboard = this.TryFindResource("IndeterminateStoryboard") as Storyboard;
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
                    this.SetEllipseDiameter(actualSize);
                }
                if (this.EllipseOffset.Equals(0))
                {
                    this.SetEllipseOffset(actualSize);
                }
            }
        }
    }
}