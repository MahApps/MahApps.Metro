// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using MahApps.Metro.ValueBoxes;

namespace MahApps.Metro.Controls
{
    /// <summary>
    /// enumeration for the different transition types
    /// </summary>
    public enum TransitionType
    {
        /// <summary>
        /// Use the VisualState DefaultTransition
        /// </summary>
        Default,
        /// <summary>
        /// Use the VisualState Normal
        /// </summary>
        Normal,
        /// <summary>
        /// Use the VisualState UpTransition
        /// </summary>
        Up,
        /// <summary>
        /// Use the VisualState DownTransition
        /// </summary>
        Down,
        /// <summary>
        /// Use the VisualState RightTransition
        /// </summary>
        Right,
        /// <summary>
        /// Use the VisualState RightReplaceTransition
        /// </summary>
        RightReplace,
        /// <summary>
        /// Use the VisualState LeftTransition
        /// </summary>
        Left,
        /// <summary>
        /// Use the VisualState LeftReplaceTransition
        /// </summary>
        LeftReplace,
        /// <summary>
        /// Use a custom VisualState, the name must be set using CustomVisualStatesName property
        /// </summary>
        Custom
    }

    /// <summary>
    /// A ContentControl that animates content as it loads and unloads.
    /// </summary>
    [TemplatePart(Name = PreviousContentPresentationSitePartName, Type = typeof(ContentPresenter))]
    [TemplatePart(Name = CurrentContentPresentationSitePartName, Type = typeof(ContentPresenter))]
    public class TransitioningContentControl : ContentControl
    {
        internal const string PresentationGroup = "PresentationStates";
        internal const string HiddenState = "Hidden";
        internal const string PreviousContentPresentationSitePartName = "PreviousContentPresentationSite";
        internal const string CurrentContentPresentationSitePartName = "CurrentContentPresentationSite";

        private ContentPresenter? currentContentPresentationSite;
        private ContentPresenter? previousContentPresentationSite;
        private bool allowIsTransitioningPropertyWrite;
        private Storyboard? currentTransition;

        public event RoutedEventHandler? TransitionCompleted;

        public const TransitionType DefaultTransitionState = TransitionType.Default;

        public static readonly DependencyProperty IsTransitioningProperty
            = DependencyProperty.Register(nameof(IsTransitioning),
                                          typeof(bool),
                                          typeof(TransitioningContentControl),
                                          new PropertyMetadata(BooleanBoxes.FalseBox, OnIsTransitioningPropertyChanged));

        /// <summary>
        /// Gets whether if the content is transitioning.
        /// </summary>
        public bool IsTransitioning
        {
            get => (bool)this.GetValue(IsTransitioningProperty);
            private set
            {
                this.allowIsTransitioningPropertyWrite = true;
                try
                {
                    this.SetValue(IsTransitioningProperty, BooleanBoxes.Box(value));
                }
                finally
                {
                    this.allowIsTransitioningPropertyWrite = false;
                }
            }
        }

        public static readonly DependencyProperty TransitionProperty
            = DependencyProperty.Register(nameof(Transition),
                                          typeof(TransitionType),
                                          typeof(TransitioningContentControl),
                                          new FrameworkPropertyMetadata(TransitionType.Default, FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.Inherits, OnTransitionPropertyChanged));

        /// <summary>
        /// Gets or sets the transition type.
        /// </summary>
        public TransitionType Transition
        {
            get => (TransitionType)this.GetValue(TransitionProperty);
            set => this.SetValue(TransitionProperty, value);
        }

        public static readonly DependencyProperty RestartTransitionOnContentChangeProperty
            = DependencyProperty.Register(nameof(RestartTransitionOnContentChange),
                                          typeof(bool),
                                          typeof(TransitioningContentControl),
                                          new PropertyMetadata(BooleanBoxes.FalseBox, OnRestartTransitionOnContentChangePropertyChanged));

        /// <summary>
        /// Gets or sets whether if the transition should restart after the content change.
        /// </summary>
        public bool RestartTransitionOnContentChange
        {
            get => (bool)this.GetValue(RestartTransitionOnContentChangeProperty);
            set => this.SetValue(RestartTransitionOnContentChangeProperty, BooleanBoxes.Box(value));
        }

        public static readonly DependencyProperty CustomVisualStatesProperty
            = DependencyProperty.Register(nameof(CustomVisualStates),
                                          typeof(ObservableCollection<VisualState>),
                                          typeof(TransitioningContentControl),
                                          new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets customized visual states to use as transition.
        /// </summary>
        public ObservableCollection<VisualState>? CustomVisualStates
        {
            get => (ObservableCollection<VisualState>?)this.GetValue(CustomVisualStatesProperty);
            set => this.SetValue(CustomVisualStatesProperty, value);
        }

        public static readonly DependencyProperty CustomVisualStatesNameProperty
            = DependencyProperty.Register(nameof(CustomVisualStatesName),
                                          typeof(string),
                                          typeof(TransitioningContentControl),
                                          new PropertyMetadata("CustomTransition"));

        /// <summary>
        /// Gets or sets the name of a custom transition visual state.
        /// </summary>
        public string CustomVisualStatesName
        {
            get => (string)this.GetValue(CustomVisualStatesNameProperty);
            set => this.SetValue(CustomVisualStatesNameProperty, value);
        }

        internal Storyboard? CurrentTransition
        {
            get => this.currentTransition;
            set
            {
                // decouple event
                if (this.currentTransition != null)
                {
                    this.currentTransition.Completed -= this.OnTransitionCompleted;
                }

                this.currentTransition = value;

                if (this.currentTransition != null)
                {
                    this.currentTransition.Completed += this.OnTransitionCompleted;
                }
            }
        }

        private static void OnIsTransitioningPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)

        {
            var source = (TransitioningContentControl)d;

            if (!source.allowIsTransitioningPropertyWrite)
            {
                source.IsTransitioning = (bool)e.OldValue;
                throw new InvalidOperationException();
            }
        }

        private static void OnTransitionPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var source = (TransitioningContentControl)d;
            var oldTransition = (TransitionType)e.OldValue;
            var newTransition = (TransitionType)e.NewValue;

            if (source.IsTransitioning)
            {
                source.AbortTransition();
            }

            // find new transition
            var newStoryboard = source.GetStoryboard(newTransition);

            // unable to find the transition.
            if (newStoryboard is null)
            {
                // could be during initialization of xaml that presentationgroups was not yet defined
                if (VisualStates.TryGetVisualStateGroup(source, PresentationGroup) is null)
                {
                    // will delay check
                    source.CurrentTransition = null;
                }
                else
                {
                    // revert to old value
                    source.SetValue(TransitionProperty, oldTransition);

                    throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, "Temporary removed exception message", newTransition));
                }
            }
            else
            {
                source.CurrentTransition = newStoryboard;
            }
        }

        private static void OnRestartTransitionOnContentChangePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((TransitioningContentControl)d).OnRestartTransitionOnContentChangeChanged((bool)e.OldValue, (bool)e.NewValue);
        }

        protected virtual void OnRestartTransitionOnContentChangeChanged(bool oldValue, bool newValue)
        {
        }

        public TransitioningContentControl()
        {
            this.CustomVisualStates = new ObservableCollection<VisualState>();
            this.DefaultStyleKey = typeof(TransitioningContentControl);
        }

        public override void OnApplyTemplate()
        {
            if (this.IsTransitioning)
            {
                this.AbortTransition();
            }

            if (this.CustomVisualStates != null && this.CustomVisualStates.Any())
            {
                var presentationGroup = VisualStates.TryGetVisualStateGroup(this, PresentationGroup);
                if (presentationGroup != null)
                {
                    foreach (var state in this.CustomVisualStates)
                    {
                        presentationGroup.States.Add(state);
                    }
                }
            }

            base.OnApplyTemplate();

            this.previousContentPresentationSite = this.GetTemplateChild(PreviousContentPresentationSitePartName) as ContentPresenter;
            this.currentContentPresentationSite = this.GetTemplateChild(CurrentContentPresentationSitePartName) as ContentPresenter;

            // hookup currenttransition
            var transition = this.GetStoryboard(this.Transition);
            this.CurrentTransition = transition;
            if (transition is null)
            {
                var invalidTransition = this.Transition;
                // revert to default
                this.Transition = DefaultTransitionState;

                throw new MahAppsException($"'{invalidTransition}' transition could not be found!");
            }

            VisualStateManager.GoToState(this, HiddenState, false);
        }

        protected override void OnContentChanged(object oldContent, object newContent)
        {
            base.OnContentChanged(oldContent, newContent);

            if (oldContent != newContent)
            {
                this.StartTransition(oldContent, newContent);
            }
        }

        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "newContent", Justification = "Should be used in the future.")]
        private void StartTransition(object oldContent, object newContent)
        {
            // both presenters must be available, otherwise a transition is useless.
            if (this.currentContentPresentationSite != null && this.previousContentPresentationSite != null)
            {
                if (this.RestartTransitionOnContentChange
                    && this.CurrentTransition is not null)
                {
                    this.CurrentTransition.Completed -= this.OnTransitionCompleted;
                }

                this.currentContentPresentationSite.SetCurrentValue(ContentPresenter.ContentProperty, newContent);
                this.previousContentPresentationSite.SetCurrentValue(ContentPresenter.ContentProperty, oldContent);

                // and start a new transition
                if (!this.IsTransitioning || this.RestartTransitionOnContentChange)
                {
                    if (this.RestartTransitionOnContentChange
                        && this.CurrentTransition is not null)
                    {
                        this.CurrentTransition.Completed += this.OnTransitionCompleted;
                    }

                    this.IsTransitioning = true;
                    VisualStateManager.GoToState(this, HiddenState, false);
                    VisualStateManager.GoToState(this, this.GetTransitionName(this.Transition), true);
                }
            }
        }

        /// <summary>
        /// Reload the current transition if the content is the same.
        /// </summary>
        public void ReloadTransition()
        {
            // both presenters must be available, otherwise a transition is useless.
            if (this.currentContentPresentationSite != null && this.previousContentPresentationSite != null)
            {
                if (this.RestartTransitionOnContentChange
                    && this.CurrentTransition is not null)
                {
                    this.CurrentTransition.Completed -= this.OnTransitionCompleted;
                }

                if (!this.IsTransitioning || this.RestartTransitionOnContentChange)
                {
                    if (this.RestartTransitionOnContentChange
                        && this.CurrentTransition is not null)
                    {
                        this.CurrentTransition.Completed += this.OnTransitionCompleted;
                    }

                    this.IsTransitioning = true;
                    VisualStateManager.GoToState(this, HiddenState, false);
                    VisualStateManager.GoToState(this, this.GetTransitionName(this.Transition), true);
                }
            }
        }

        private void OnTransitionCompleted(object? sender, EventArgs e)
        {
            this.AbortTransition();
            var clockGroup = sender as ClockGroup;
            if (clockGroup is null || clockGroup.CurrentState == ClockState.Stopped)
            {
                this.TransitionCompleted?.Invoke(this, new RoutedEventArgs());
            }
        }

        public void AbortTransition()
        {
            // go to normal state and release our hold on the old content.
            VisualStateManager.GoToState(this, HiddenState, false);
            this.IsTransitioning = false;
            this.previousContentPresentationSite?.SetCurrentValue(ContentPresenter.ContentProperty, null);
        }

        private Storyboard? GetStoryboard(TransitionType newTransition)
        {
            var presentationGroup = VisualStates.TryGetVisualStateGroup(this, PresentationGroup);
            if (presentationGroup != null)
            {
                var transitionName = this.GetTransitionName(newTransition);
                return presentationGroup.States
                                        .OfType<VisualState>()
                                        .Where(state => state.Name == transitionName)
                                        .Select(state => state.Storyboard)
                                        .FirstOrDefault();
            }

            return null;
        }

        private string GetTransitionName(TransitionType transition)
        {
            return transition switch
            {
                TransitionType.Default => "DefaultTransition",
                TransitionType.Normal => "Normal",
                TransitionType.Up => "UpTransition",
                TransitionType.Down => "DownTransition",
                TransitionType.Right => "RightTransition",
                TransitionType.RightReplace => "RightReplaceTransition",
                TransitionType.Left => "LeftTransition",
                TransitionType.LeftReplace => "LeftReplaceTransition",
                TransitionType.Custom => this.CustomVisualStatesName,
                _ => "DefaultTransition"
            };
        }
    }
}