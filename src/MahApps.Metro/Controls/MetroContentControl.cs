// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using MahApps.Metro.ValueBoxes;

namespace MahApps.Metro.Controls
{
    /// <summary>
    /// A ContentControl which use a transition to slide in the content.
    /// </summary>
    public class MetroContentControl : ContentControl
    {
        private Storyboard afterLoadedStoryboard;
        private Storyboard afterLoadedReverseStoryboard;
        private bool transitionLoaded;

        /// <summary>Identifies the <see cref="ReverseTransition"/> dependency property.</summary>
        public static readonly DependencyProperty ReverseTransitionProperty
            = DependencyProperty.Register(nameof(ReverseTransition),
                                          typeof(bool),
                                          typeof(MetroContentControl),
                                          new FrameworkPropertyMetadata(BooleanBoxes.FalseBox));

        /// <summary>
        /// Gets or sets whether the reverse version of the transition should be used.
        /// </summary>
        public bool ReverseTransition
        {
            get => (bool)this.GetValue(ReverseTransitionProperty);
            set => this.SetValue(ReverseTransitionProperty, BooleanBoxes.Box(value));
        }

        /// <summary>Identifies the <see cref="TransitionsEnabled"/> dependency property.</summary>
        public static readonly DependencyProperty TransitionsEnabledProperty
            = DependencyProperty.Register(nameof(TransitionsEnabled),
                                          typeof(bool),
                                          typeof(MetroContentControl),
                                          new FrameworkPropertyMetadata(BooleanBoxes.TrueBox));

        /// <summary>
        /// Gets or sets the value if a transition should be used or not.
        /// </summary>
        public bool TransitionsEnabled
        {
            get => (bool)this.GetValue(TransitionsEnabledProperty);
            set => this.SetValue(TransitionsEnabledProperty, BooleanBoxes.Box(value));
        }

        /// <summary>Identifies the <see cref="OnlyLoadTransition"/> dependency property.</summary>
        public static readonly DependencyProperty OnlyLoadTransitionProperty
            = DependencyProperty.Register(nameof(OnlyLoadTransition),
                                          typeof(bool),
                                          typeof(MetroContentControl),
                                          new FrameworkPropertyMetadata(BooleanBoxes.FalseBox));

        /// <summary>
        /// Gets or sets whether the transition should be used only at the loaded event of the control.
        /// </summary>
        public bool OnlyLoadTransition
        {
            get => (bool)this.GetValue(OnlyLoadTransitionProperty);
            set => this.SetValue(OnlyLoadTransitionProperty, BooleanBoxes.Box(value));
        }

        /// <summary>Identifies the <see cref="TransitionStarted"/> routed event.</summary>
        public static readonly RoutedEvent TransitionStartedEvent
            = EventManager.RegisterRoutedEvent(nameof(TransitionStarted),
                                               RoutingStrategy.Bubble,
                                               typeof(RoutedEventHandler),
                                               typeof(MetroContentControl));

        /// <summary>
        /// The event which will be fired when the transition starts.
        /// </summary>
        public event RoutedEventHandler TransitionStarted
        {
            add => this.AddHandler(TransitionStartedEvent, value);
            remove => this.RemoveHandler(TransitionStartedEvent, value);
        }

        /// <summary>Identifies the <see cref="TransitionCompleted"/> routed event.</summary>
        public static readonly RoutedEvent TransitionCompletedEvent
            = EventManager.RegisterRoutedEvent(nameof(TransitionCompleted),
                                               RoutingStrategy.Bubble,
                                               typeof(RoutedEventHandler),
                                               typeof(MetroContentControl));

        /// <summary>
        /// The event which will be fired when the transition ends.
        /// </summary>
        public event RoutedEventHandler TransitionCompleted
        {
            add => this.AddHandler(TransitionCompletedEvent, value);
            remove => this.RemoveHandler(TransitionCompletedEvent, value);
        }

        /// <summary>Identifies the <see cref="IsTransitioning"/> dependency property.</summary>
        private static readonly DependencyPropertyKey IsTransitioningPropertyKey
            = DependencyProperty.RegisterReadOnly(nameof(IsTransitioning),
                                                  typeof(bool),
                                                  typeof(MetroContentControl),
                                                  new PropertyMetadata(BooleanBoxes.FalseBox));

        /// <summary>Identifies the <see cref="IsTransitioning"/> dependency property.</summary>
        public static readonly DependencyProperty IsTransitioningProperty = IsTransitioningPropertyKey.DependencyProperty;

        /// <summary>
        /// Gets whether if the content is transitioning.
        /// </summary>
        public bool IsTransitioning
        {
            get => (bool)this.GetValue(IsTransitioningProperty);
            protected set => this.SetValue(IsTransitioningPropertyKey, BooleanBoxes.Box(value));
        }

        public MetroContentControl()
        {
            this.DefaultStyleKey = typeof(MetroContentControl);

            this.Loaded += this.MetroContentControlLoaded;
            this.Unloaded += this.MetroContentControlUnloaded;
        }

        private void MetroContentControlIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (this.TransitionsEnabled && !this.transitionLoaded)
            {
                if (!this.IsVisible)
                {
                    VisualStateManager.GoToState(this, this.ReverseTransition ? "AfterUnLoadedReverse" : "AfterUnLoaded", false);
                }
                else
                {
                    VisualStateManager.GoToState(this, this.ReverseTransition ? "AfterLoadedReverse" : "AfterLoaded", true);
                }
            }
        }

        private void MetroContentControlUnloaded(object sender, RoutedEventArgs e)
        {
            if (this.TransitionsEnabled)
            {
                this.UnsetStoryboardEvents();
                if (this.transitionLoaded)
                {
                    VisualStateManager.GoToState(this, this.ReverseTransition ? "AfterUnLoadedReverse" : "AfterUnLoaded", false);
                }

                this.IsVisibleChanged -= this.MetroContentControlIsVisibleChanged;
            }
        }

        private void MetroContentControlLoaded(object sender, RoutedEventArgs e)
        {
            if (this.TransitionsEnabled)
            {
                if (!this.transitionLoaded)
                {
                    this.SetStoryboardEvents();
                    this.transitionLoaded = this.OnlyLoadTransition;
                    VisualStateManager.GoToState(this, this.ReverseTransition ? "AfterLoadedReverse" : "AfterLoaded", true);
                }

                this.IsVisibleChanged -= this.MetroContentControlIsVisibleChanged;
                this.IsVisibleChanged += this.MetroContentControlIsVisibleChanged;
            }
            else
            {
                if (this.GetTemplateChild("RootGrid") is Grid rootGrid)
                {
                    rootGrid.Opacity = 1.0;
                    var transform = ((System.Windows.Media.TranslateTransform)rootGrid.RenderTransform);
                    if (transform.IsFrozen)
                    {
                        var modifiedTransform = transform.Clone();
                        modifiedTransform.X = 0;
                        rootGrid.RenderTransform = modifiedTransform;
                    }
                    else
                    {
                        transform.X = 0;
                    }
                }
            }
        }

        /// <summary>
        /// Execute the transition again.
        /// </summary>
        public void Reload()
        {
            if (!this.TransitionsEnabled || this.transitionLoaded)
            {
                return;
            }

            if (this.ReverseTransition)
            {
                VisualStateManager.GoToState(this, "BeforeLoaded", true);
                VisualStateManager.GoToState(this, "AfterUnLoadedReverse", true);
            }
            else
            {
                VisualStateManager.GoToState(this, "BeforeLoaded", true);
                VisualStateManager.GoToState(this, "AfterLoaded", true);
            }
        }

        /// <inheritdoc />
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            this.afterLoadedStoryboard = this.GetTemplateChild("AfterLoadedStoryboard") as Storyboard;
            this.afterLoadedReverseStoryboard = this.GetTemplateChild("AfterLoadedReverseStoryboard") as Storyboard;
        }

        private void AfterLoadedStoryboardCurrentTimeInvalidated(object sender, System.EventArgs e)
        {
            if (sender is Clock clock)
            {
                if (clock.CurrentState == ClockState.Active)
                {
                    this.SetValue(IsTransitioningPropertyKey, BooleanBoxes.TrueBox);
                    this.RaiseEvent(new RoutedEventArgs(TransitionStartedEvent));
                }
            }
        }

        private void AfterLoadedStoryboardCompleted(object sender, System.EventArgs e)
        {
            if (this.transitionLoaded)
            {
                this.UnsetStoryboardEvents();
            }

            this.InvalidateVisual();
            this.SetValue(IsTransitioningPropertyKey, BooleanBoxes.FalseBox);
            this.RaiseEvent(new RoutedEventArgs(TransitionCompletedEvent));
        }

        private void SetStoryboardEvents()
        {
            if (this.afterLoadedStoryboard != null)
            {
                this.afterLoadedStoryboard.CurrentTimeInvalidated += this.AfterLoadedStoryboardCurrentTimeInvalidated;
                this.afterLoadedStoryboard.Completed += this.AfterLoadedStoryboardCompleted;
            }

            if (this.afterLoadedReverseStoryboard != null)
            {
                this.afterLoadedReverseStoryboard.CurrentTimeInvalidated += this.AfterLoadedStoryboardCurrentTimeInvalidated;
                this.afterLoadedReverseStoryboard.Completed += this.AfterLoadedStoryboardCompleted;
            }
        }

        private void UnsetStoryboardEvents()
        {
            if (this.afterLoadedStoryboard != null)
            {
                this.afterLoadedStoryboard.CurrentTimeInvalidated -= this.AfterLoadedStoryboardCurrentTimeInvalidated;
                this.afterLoadedStoryboard.Completed -= this.AfterLoadedStoryboardCompleted;
            }

            if (this.afterLoadedReverseStoryboard != null)
            {
                this.afterLoadedReverseStoryboard.CurrentTimeInvalidated -= this.AfterLoadedStoryboardCurrentTimeInvalidated;
                this.afterLoadedReverseStoryboard.Completed -= this.AfterLoadedStoryboardCompleted;
            }
        }
    }
}