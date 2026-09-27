// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media.Animation;
using MahApps.Metro.ValueBoxes;

namespace MahApps.Metro.Controls
{
    /// <summary>
    /// A helper class that provides various attached properties for the ProgressBar control.
    /// <see cref="ProgressBar"/>
    /// </summary>
    public static class ProgressBarHelper
    {
        public static readonly DependencyProperty ShowPausedProperty
            = DependencyProperty.RegisterAttached("ShowPaused",
                                                  typeof(bool),
                                                  typeof(ProgressBarHelper),
                                                  new PropertyMetadata(BooleanBoxes.FalseBox));

        /// <summary>Helper for getting <see cref="ShowPausedProperty"/> from <paramref name="element"/>.</summary>
        /// <param name="element"><see cref="DependencyObject"/> to read <see cref="ShowPausedProperty"/> from.</param>
        /// <remarks>Gets the value whether the bar should show that the work behind it is on hold.</remarks>
        /// <returns>ShowPaused property value.</returns>
        [AttachedPropertyBrowsableForType(typeof(ProgressBar))]
        public static bool GetShowPaused(DependencyObject element)
        {
            return (bool)element.GetValue(ShowPausedProperty);
        }

        /// <summary>Helper for setting <see cref="ShowPausedProperty"/> on <paramref name="element"/>.</summary>
        /// <param name="element"><see cref="DependencyObject"/> to set <see cref="ShowPausedProperty"/> on.</param>
        /// <param name="value">ShowPaused property value.</param>
        /// <remarks>Sets the value whether the bar should show that the work behind it is on hold.</remarks>
        public static void SetShowPaused(DependencyObject element, bool value)
        {
            element.SetValue(ShowPausedProperty, BooleanBoxes.Box(value));
        }

        public static readonly DependencyProperty ShowErrorProperty
            = DependencyProperty.RegisterAttached("ShowError",
                                                  typeof(bool),
                                                  typeof(ProgressBarHelper),
                                                  new PropertyMetadata(BooleanBoxes.FalseBox));

        /// <summary>Helper for getting <see cref="ShowErrorProperty"/> from <paramref name="element"/>.</summary>
        /// <param name="element"><see cref="DependencyObject"/> to read <see cref="ShowErrorProperty"/> from.</param>
        /// <remarks>Gets the value whether the bar should show that the work behind it has gone wrong.</remarks>
        /// <returns>ShowError property value.</returns>
        [AttachedPropertyBrowsableForType(typeof(ProgressBar))]
        public static bool GetShowError(DependencyObject element)
        {
            return (bool)element.GetValue(ShowErrorProperty);
        }

        /// <summary>Helper for setting <see cref="ShowErrorProperty"/> on <paramref name="element"/>.</summary>
        /// <param name="element"><see cref="DependencyObject"/> to set <see cref="ShowErrorProperty"/> on.</param>
        /// <param name="value">ShowError property value.</param>
        /// <remarks>Sets the value whether the bar should show that the work behind it has gone wrong.</remarks>
        public static void SetShowError(DependencyObject element, bool value)
        {
            element.SetValue(ShowErrorProperty, BooleanBoxes.Box(value));
        }

        public static readonly DependencyProperty EllipseDiameterProperty
            = DependencyProperty.RegisterAttached("EllipseDiameter",
                                                  typeof(double),
                                                  typeof(ProgressBarHelper),
                                                  new PropertyMetadata(default(double)));

        /// <summary>Helper for getting <see cref="EllipseDiameterProperty"/> from <paramref name="element"/>.</summary>
        /// <param name="element"><see cref="DependencyObject"/> to read <see cref="EllipseDiameterProperty"/> from.</param>
        /// <remarks>Gets the diameter of the dots the bar sends along while it is indeterminate.</remarks>
        /// <returns>EllipseDiameter property value.</returns>
        [AttachedPropertyBrowsableForType(typeof(ProgressBar))]
        public static double GetEllipseDiameter(DependencyObject element)
        {
            return (double)element.GetValue(EllipseDiameterProperty);
        }

        /// <summary>Helper for setting <see cref="EllipseDiameterProperty"/> on <paramref name="element"/>.</summary>
        /// <param name="element"><see cref="DependencyObject"/> to set <see cref="EllipseDiameterProperty"/> on.</param>
        /// <param name="value">EllipseDiameter property value.</param>
        /// <remarks>Sets the diameter of the dots the bar sends along while it is indeterminate.</remarks>
        public static void SetEllipseDiameter(DependencyObject element, double value)
        {
            element.SetValue(EllipseDiameterProperty, value);
        }

        public static readonly DependencyProperty EllipseOffsetProperty
            = DependencyProperty.RegisterAttached("EllipseOffset",
                                                  typeof(double),
                                                  typeof(ProgressBarHelper),
                                                  new PropertyMetadata(default(double)));

        /// <summary>Helper for getting <see cref="EllipseOffsetProperty"/> from <paramref name="element"/>.</summary>
        /// <param name="element"><see cref="DependencyObject"/> to read <see cref="EllipseOffsetProperty"/> from.</param>
        /// <remarks>Gets the room between two of those dots.</remarks>
        /// <returns>EllipseOffset property value.</returns>
        [AttachedPropertyBrowsableForType(typeof(ProgressBar))]
        public static double GetEllipseOffset(DependencyObject element)
        {
            return (double)element.GetValue(EllipseOffsetProperty);
        }

        /// <summary>Helper for setting <see cref="EllipseOffsetProperty"/> on <paramref name="element"/>.</summary>
        /// <param name="element"><see cref="DependencyObject"/> to set <see cref="EllipseOffsetProperty"/> on.</param>
        /// <param name="value">EllipseOffset property value.</param>
        /// <remarks>Sets the room between two of those dots.</remarks>
        public static void SetEllipseOffset(DependencyObject element, double value)
        {
            element.SetValue(EllipseOffsetProperty, value);
        }

        public static readonly DependencyProperty AdjustIndeterminateAnimationProperty
            = DependencyProperty.RegisterAttached("AdjustIndeterminateAnimation",
                                                  typeof(bool),
                                                  typeof(ProgressBarHelper),
                                                  new PropertyMetadata(BooleanBoxes.FalseBox, OnAdjustIndeterminateAnimationChanged));

        /// <summary>Helper for getting <see cref="AdjustIndeterminateAnimationProperty"/> from <paramref name="element"/>.</summary>
        /// <param name="element"><see cref="DependencyObject"/> to read <see cref="AdjustIndeterminateAnimationProperty"/> from.</param>
        /// <remarks>Gets the value whether the indeterminate animation of the template should be fitted to the size of the bar.</remarks>
        /// <returns>AdjustIndeterminateAnimation property value.</returns>
        [AttachedPropertyBrowsableForType(typeof(ProgressBar))]
        public static bool GetAdjustIndeterminateAnimation(DependencyObject element)
        {
            return (bool)element.GetValue(AdjustIndeterminateAnimationProperty);
        }

        /// <summary>Helper for setting <see cref="AdjustIndeterminateAnimationProperty"/> on <paramref name="element"/>.</summary>
        /// <param name="element"><see cref="DependencyObject"/> to set <see cref="AdjustIndeterminateAnimationProperty"/> on.</param>
        /// <param name="value">AdjustIndeterminateAnimation property value.</param>
        /// <remarks>
        /// Sets the value whether the indeterminate animation of the template should be fitted to the
        /// size of the bar. The template has to name the part the dots live in ContainingGrid and hand
        /// its unadjusted Storyboard over under the key IndeterminateStoryboard, the way the Windows 10
        /// style does.
        /// </remarks>
        public static void SetAdjustIndeterminateAnimation(DependencyObject element, bool value)
        {
            element.SetValue(AdjustIndeterminateAnimationProperty, BooleanBoxes.Box(value));
        }

        /// <summary>
        /// Reading IsIndeterminate through a binding rather than through a descriptor is what keeps the
        /// bar collectable, since a descriptor would hold on to it for as long as the application runs.
        /// </summary>
        private static readonly DependencyProperty IsIndeterminateWatcherProperty
            = DependencyProperty.RegisterAttached("IsIndeterminateWatcher",
                                                  typeof(bool),
                                                  typeof(ProgressBarHelper),
                                                  new PropertyMetadata(BooleanBoxes.FalseBox, OnWatchedValueChanged));

        private static void OnAdjustIndeterminateAnimationChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            if (dependencyObject is not ProgressBar bar || Equals(e.OldValue, e.NewValue))
            {
                return;
            }

            bar.Loaded -= OnLoaded;
            bar.SizeChanged -= OnSizeChanged;
            bar.IsVisibleChanged -= OnWatchedValueChanged;
            BindingOperations.ClearBinding(bar, IsIndeterminateWatcherProperty);

            if ((bool)e.NewValue)
            {
                bar.Loaded += OnLoaded;
                bar.SizeChanged += OnSizeChanged;
                bar.IsVisibleChanged += OnWatchedValueChanged;
                BindingOperations.SetBinding(bar, IsIndeterminateWatcherProperty, new Binding { Path = new PropertyPath(ProgressBar.IsIndeterminateProperty), Source = bar });
            }

            Reset(bar);
        }

        private static void OnLoaded(object sender, RoutedEventArgs e)
        {
            Reset((ProgressBar)sender);
        }

        private static void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            Reset((ProgressBar)sender);
        }

        private static void OnWatchedValueChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (sender is ProgressBar bar)
            {
                Reset(bar);
            }
        }

        /// <summary>
        /// Writes the numbers for the size the bar has now into a copy of the Storyboard the template
        /// brought and starts that copy, or takes the running one off again where there is nothing left
        /// to show.
        /// </summary>
        private static void Reset(ProgressBar bar)
        {
            bar.ApplyTemplate();

            if (bar.Template?.FindName(ProgressBarEllipseAnimation.ContainingGridName, bar) is not FrameworkElement containingGrid)
            {
                return;
            }

            var state = ProgressBarEllipseAnimation.FindIndeterminateState(containingGrid);
            if (state is null)
            {
                return;
            }

            if (state.Storyboard is not null)
            {
                state.Storyboard.Stop(containingGrid);
                state.Storyboard.Remove(containingGrid);
                state.Storyboard = null;
            }

            var size = bar.Orientation == Orientation.Horizontal ? bar.ActualWidth : bar.ActualHeight;
            if (size <= 0)
            {
                return;
            }

            // a size somebody set by hand stays, everything else follows how wide the bar turned out to be
            if (bar.ReadLocalValue(EllipseDiameterProperty) == DependencyProperty.UnsetValue)
            {
                bar.SetCurrentValue(EllipseDiameterProperty, ProgressBarEllipseAnimation.DiameterFor(size));
            }

            if (bar.ReadLocalValue(EllipseOffsetProperty) == DependencyProperty.UnsetValue)
            {
                bar.SetCurrentValue(EllipseOffsetProperty, ProgressBarEllipseAnimation.OffsetFor(size));
            }

            if (!bar.IsIndeterminate
                || !bar.IsVisible
                || !GetAdjustIndeterminateAnimation(bar)
                || bar.TryFindResource(ProgressBarEllipseAnimation.StoryboardKey) is not Storyboard template)
            {
                return;
            }

            state.Storyboard = ProgressBarEllipseAnimation.Rebuild(template, size);
            state.Storyboard.Begin(containingGrid, true);
        }
    }
}
