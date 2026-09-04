// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
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

        /// <summary>Identifies the <see cref="Transition"/> dependency property.</summary>
        /// <remarks>
        /// This is an attached property, so the transition can be set on any element above the control and
        /// every <see cref="TransitioningContentControl"/> underneath inherits it.
        /// </remarks>
        public static readonly DependencyProperty TransitionProperty
            = DependencyProperty.RegisterAttached(nameof(Transition),
                                                  typeof(TransitionType),
                                                  typeof(TransitioningContentControl),
                                                  new FrameworkPropertyMetadata(TransitionType.Default, FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.Inherits, OnTransitionPropertyChanged, CoerceTransition));

        /// <summary>Helper for getting <see cref="TransitionProperty"/> from <paramref name="element"/>.</summary>
        [AttachedPropertyBrowsableForType(typeof(DependencyObject))]
        public static TransitionType GetTransition(DependencyObject element)
        {
            return (TransitionType)element.GetValue(TransitionProperty);
        }

        /// <summary>Helper for setting <see cref="TransitionProperty"/> on <paramref name="element"/>.</summary>
        [AttachedPropertyBrowsableForType(typeof(DependencyObject))]
        public static void SetTransition(DependencyObject element, TransitionType value)
        {
            element.SetValue(TransitionProperty, value);
        }

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

        private static object CoerceTransition(DependencyObject d, object? baseValue)
        {
            if (baseValue is not TransitionType newTransition)
            {
                return DefaultTransitionState;
            }

            // The property is attached, so it can sit on any element above the control. Only a control
            // knows its visual states, everything else passes the value on to its children untouched.
            if (d is not TransitioningContentControl source)
            {
                return baseValue;
            }

            // Could be during initialization of xaml that the presentation group was not yet defined.
            // The value is checked again in OnApplyTemplate, so take it as it is for now.
            if (VisualStates.TryGetVisualStateGroup(source, PresentationGroup) is null)
            {
                return baseValue;
            }

            if (source.GetStoryboard(newTransition) is not null)
            {
                return baseValue;
            }

            // The transition could not be found, so keep the transition the control has right now.
            // Coercing instead of writing the old value back from inside the changed callback leaves
            // a binding on Transition alone and cannot recurse. If that transition cannot be resolved
            // either, which happens for a template without any of the known states, fall back to the
            // default one and let the control live without a transition.
            var currentTransition = (TransitionType)source.GetValue(TransitionProperty);

            return source.GetStoryboard(currentTransition) is not null
                ? currentTransition
                : DefaultTransitionState;
        }

        private static void OnTransitionPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is not TransitioningContentControl source)
            {
                return;
            }

            if (source.IsTransitioning)
            {
                source.AbortTransition();
            }

            // The value passed the coercion, so it is either a transition that could be found
            // or one that has to be checked again as soon as the template is applied.
            source.CurrentTransition = source.GetStoryboard((TransitionType)e.NewValue);
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
            // The states are known now, so let the coercion decide whether the current transition survives the new template.
            this.CoerceValue(TransitionProperty);
            this.CurrentTransition = this.GetStoryboard(this.Transition);

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