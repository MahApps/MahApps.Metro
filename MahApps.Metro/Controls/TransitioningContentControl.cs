// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

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
    public class TransitioningContentControl : ContentControl
    {
        internal const string PresentationGroup = "PresentationStates";
        internal const string NormalState = "Normal";
        internal const string PreviousContentPresentationSitePartName = "PreviousContentPresentationSite";
        internal const string CurrentContentPresentationSitePartName = "CurrentContentPresentationSite";

        private ContentPresenter currentContentPresentationSite;
        private ContentPresenter previousContentPresentationSite;
        private bool allowIsTransitioningPropertyWrite;
        private Storyboard currentTransition;

        public event RoutedEventHandler TransitionCompleted;

        public const TransitionType DefaultTransitionState = TransitionType.Default;

        public static readonly DependencyProperty IsTransitioningProperty = DependencyProperty.Register("IsTransitioning", typeof(bool), typeof(TransitioningContentControl), new PropertyMetadata(OnIsTransitioningPropertyChanged));
        public static readonly DependencyProperty TransitionProperty = DependencyProperty.Register("Transition", typeof(TransitionType), typeof(TransitioningContentControl), new FrameworkPropertyMetadata(TransitionType.Default, FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.Inherits, OnTransitionPropertyChanged));
        public static readonly DependencyProperty RestartTransitionOnContentChangeProperty = DependencyProperty.Register("RestartTransitionOnContentChange", typeof(bool), typeof(TransitioningContentControl), new PropertyMetadata(false, OnRestartTransitionOnContentChangePropertyChanged));
        public static readonly DependencyProperty CustomVisualStatesProperty = DependencyProperty.Register("CustomVisualStates", typeof(ObservableCollection<VisualState>), typeof(TransitioningContentControl), new PropertyMetadata(null));
        public static readonly DependencyProperty CustomVisualStatesNameProperty = DependencyProperty.Register("CustomVisualStatesName", typeof(string), typeof(TransitioningContentControl), new PropertyMetadata("CustomTransition"));

        public ObservableCollection<VisualState> CustomVisualStates
        {
            get { return (ObservableCollection<VisualState>)this.GetValue(CustomVisualStatesProperty); }
            set { this.SetValue(CustomVisualStatesProperty, value); }
        }

        /// <summary>
        /// Gets or sets the name of the custom transition visual state.
        /// </summary>
        public string CustomVisualStatesName
        {
            get { return (string)this.GetValue(CustomVisualStatesNameProperty); }
            set { this.SetValue(CustomVisualStatesNameProperty, value); }
        }

        /// <summary>
        /// Gets/sets if the content is transitioning.
        /// </summary>
        public bool IsTransitioning
        {
            get { return (bool)this.GetValue(IsTransitioningProperty); }
            private set
            {
                this.allowIsTransitioningPropertyWrite = true;
                this.SetValue(IsTransitioningProperty, value);
                this.allowIsTransitioningPropertyWrite = false;
            }
        }

        public TransitionType Transition
        {
            get { return (TransitionType)this.GetValue(TransitionProperty); }
            set { this.SetValue(TransitionProperty, value); }
        }

        public bool RestartTransitionOnContentChange
        {
            get { return (bool)this.GetValue(RestartTransitionOnContentChangeProperty); }
            set { this.SetValue(RestartTransitionOnContentChangeProperty, value); }
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

        private Storyboard CurrentTransition
        {
            get { return this.currentTransition; }
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
            Storyboard newStoryboard = source.GetStoryboard(newTransition);

            // unable to find the transition.
            if (newStoryboard == null)
            {
                // could be during initialization of xaml that presentationgroups was not yet defined
                if (VisualStates.TryGetVisualStateGroup(source, PresentationGroup) == null)
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

            if (this.currentContentPresentationSite != null)
            {
                if (this.ContentTemplateSelector != null)
                {
                    this.currentContentPresentationSite.ContentTemplate = this.ContentTemplateSelector.SelectTemplate(this.Content, this);
                }
                else
                {
                    this.currentContentPresentationSite.ContentTemplate = this.ContentTemplate;
                }
                this.currentContentPresentationSite.Content = this.Content;
            }

            // hookup currenttransition
            Storyboard transition = this.GetStoryboard(this.Transition);
            this.CurrentTransition = transition;
            if (transition == null)
            {
                var invalidTransition = this.Transition;
                // revert to default
                this.Transition = DefaultTransitionState;

                throw new ArgumentException(string.Format("'{0}' Transition could not be found!", invalidTransition), "Transition");
            }
            VisualStateManager.GoToState(this, NormalState, false);
        }

        protected override void OnContentChanged(object oldContent, object newContent)
        {
            base.OnContentChanged(oldContent, newContent);

            this.StartTransition(oldContent, newContent);
        }

        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "newContent", Justification = "Should be used in the future.")]
        private void StartTransition(object oldContent, object newContent)
        {
            // both presenters must be available, otherwise a transition is useless.
            if (this.currentContentPresentationSite != null && this.previousContentPresentationSite != null)
            {
                if (this.RestartTransitionOnContentChange)
                {
                    this.CurrentTransition.Completed -= this.OnTransitionCompleted;
                }

                if (this.ContentTemplateSelector != null)
                {
                    this.previousContentPresentationSite.ContentTemplate = this.ContentTemplateSelector.SelectTemplate(oldContent, this);
                    this.currentContentPresentationSite.ContentTemplate = this.ContentTemplateSelector.SelectTemplate(newContent, this);
                }
                else
                {
                    this.previousContentPresentationSite.ContentTemplate = this.ContentTemplate;
                    this.currentContentPresentationSite.ContentTemplate = this.ContentTemplate;
                }
                this.currentContentPresentationSite.Content = newContent;
                this.previousContentPresentationSite.Content = oldContent;

                // and start a new transition
                if (!this.IsTransitioning || this.RestartTransitionOnContentChange)
                {
                    if (this.RestartTransitionOnContentChange)
                    {
                        this.CurrentTransition.Completed += this.OnTransitionCompleted;
                    }
                    this.IsTransitioning = true;
                    VisualStateManager.GoToState(this, NormalState, false);
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
                if (this.RestartTransitionOnContentChange)
                {
                    this.CurrentTransition.Completed -= this.OnTransitionCompleted;
                }
                if (!this.IsTransitioning || this.RestartTransitionOnContentChange)
                {
                    if (this.RestartTransitionOnContentChange)
                    {
                        this.CurrentTransition.Completed += this.OnTransitionCompleted;
                    }
                    this.IsTransitioning = true;
                    VisualStateManager.GoToState(this, NormalState, false);
                    VisualStateManager.GoToState(this, this.GetTransitionName(this.Transition), true);
                }
            }
        }

        private void OnTransitionCompleted(object sender, EventArgs e)
        {
            this.AbortTransition();
            this.TransitionCompleted?.Invoke(this, new RoutedEventArgs());
        }

        public void AbortTransition()
        {
            // go to normal state and release our hold on the old content.
            VisualStateManager.GoToState(this, NormalState, false);
            this.IsTransitioning = false;
            if (this.previousContentPresentationSite != null)
            {
                this.previousContentPresentationSite.ContentTemplate = null;
                this.previousContentPresentationSite.Content = null;
            }
        }

        private Storyboard GetStoryboard(TransitionType newTransition)
        {
            VisualStateGroup presentationGroup = VisualStates.TryGetVisualStateGroup(this, PresentationGroup);
            Storyboard newStoryboard = null;
            if (presentationGroup != null)
            {
                var transitionName = this.GetTransitionName(newTransition);
                newStoryboard = presentationGroup.States
                                                 .OfType<VisualState>()
                                                 .Where(state => state.Name == transitionName)
                                                 .Select(state => state.Storyboard)
                                                 .FirstOrDefault();
            }
            return newStoryboard;
        }

        private string GetTransitionName(TransitionType transition)
        {
            switch (transition)
            {
                default:
                case TransitionType.Default:
                    return "DefaultTransition";
                case TransitionType.Normal:
                    return "Normal";
                case TransitionType.Up:
                    return "UpTransition";
                case TransitionType.Down:
                    return "DownTransition";
                case TransitionType.Right:
                    return "RightTransition";
                case TransitionType.RightReplace:
                    return "RightReplaceTransition";
                case TransitionType.Left:
                    return "LeftTransition";
                case TransitionType.LeftReplace:
                    return "LeftReplaceTransition";
                case TransitionType.Custom:
                    return this.CustomVisualStatesName;
            }
        }
    }
}