// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using MahApps.Metro.ValueBoxes;

namespace MahApps.Metro.Controls
{
    public class FlipViewItem : ContentControl
    {
    }

    /// <summary>
    /// A control that imitate a slide show with back/forward buttons.
    /// </summary>
    [TemplatePart(Name = PART_Presenter, Type = typeof(TransitioningContentControl))]
    [TemplatePart(Name = PART_BackButton, Type = typeof(Button))]
    [TemplatePart(Name = PART_ForwardButton, Type = typeof(Button))]
    [TemplatePart(Name = PART_UpButton, Type = typeof(Button))]
    [TemplatePart(Name = PART_DownButton, Type = typeof(Button))]
    [TemplatePart(Name = PART_BannerGrid, Type = typeof(Grid))]
    [TemplatePart(Name = PART_BannerLabel, Type = typeof(Label))]
    public class FlipView : Selector
    {
        /// <summary>Identifies the <see cref="MouseHoverBorderBrush"/> dependency property.</summary>
        public static readonly DependencyProperty MouseHoverBorderBrushProperty = DependencyProperty.Register(nameof(MouseHoverBorderBrush), typeof(Brush), typeof(FlipView), new PropertyMetadata(Brushes.LightGray));

        /// <summary>
        /// Gets or sets the border brush of the mouse hover effect.
        /// </summary>
        public Brush MouseHoverBorderBrush
        {
            get => (Brush)this.GetValue(MouseHoverBorderBrushProperty);
            set => this.SetValue(MouseHoverBorderBrushProperty, value);
        }

        /// <summary>Identifies the <see cref="MouseHoverBorderEnabled"/> dependency property.</summary>
        public static readonly DependencyProperty MouseHoverBorderEnabledProperty = DependencyProperty.Register(nameof(MouseHoverBorderEnabled), typeof(bool), typeof(FlipView), new PropertyMetadata(BooleanBoxes.TrueBox));

        /// <summary>
        /// Gets or sets a value indicating whether the border for mouse over effect is enabled or not.
        /// </summary>
        public bool MouseHoverBorderEnabled
        {
            get => (bool)this.GetValue(MouseHoverBorderEnabledProperty);
            set => this.SetValue(MouseHoverBorderEnabledProperty, BooleanBoxes.Box(value));
        }

        /// <summary>Identifies the <see cref="MouseHoverBorderThickness"/> dependency property.</summary>
        public static readonly DependencyProperty MouseHoverBorderThicknessProperty = DependencyProperty.Register(nameof(MouseHoverBorderThickness), typeof(Thickness), typeof(FlipView), new PropertyMetadata(new Thickness(4)));

        /// <summary>
        /// Gets or sets the border thickness for the border of the mouse hover effect.
        /// </summary>
        public Thickness MouseHoverBorderThickness
        {
            get => (Thickness)this.GetValue(MouseHoverBorderThicknessProperty);
            set => this.SetValue(MouseHoverBorderThicknessProperty, value);
        }

        /// <summary>Identifies the <see cref="CircularNavigation"/> dependency property.</summary>
        public static readonly DependencyProperty CircularNavigationProperty = DependencyProperty.Register(nameof(CircularNavigation), typeof(bool), typeof(FlipView), new FrameworkPropertyMetadata(BooleanBoxes.FalseBox, FrameworkPropertyMetadataOptions.AffectsRender, (d, e) => ((FlipView)d).DetectControlButtonsStatus()));

        /// <summary>
        /// Gets or sets a value indicating whether the navigation is circular, so you get the first after last and the last before first.
        /// </summary>
        public bool CircularNavigation
        {
            get => (bool)this.GetValue(CircularNavigationProperty);
            set => this.SetValue(CircularNavigationProperty, BooleanBoxes.Box(value));
        }

        /// <summary>Identifies the <see cref="NavigationButtonsPosition"/> dependency property.</summary>
        public static readonly DependencyProperty NavigationButtonsPositionProperty = DependencyProperty.Register(nameof(NavigationButtonsPosition), typeof(NavigationButtonsPosition), typeof(FlipView), new FrameworkPropertyMetadata(NavigationButtonsPosition.Inside, FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsMeasure, (d, e) => ((FlipView)d).DetectControlButtonsStatus()));

        /// <summary>
        /// Gets or sets the position of the navigation buttons.
        /// </summary>
        [Bindable(true)]
        [Category("Appearance")]
        [DefaultValue(NavigationButtonsPosition.Inside)]
        public NavigationButtonsPosition NavigationButtonsPosition
        {
            get => (NavigationButtonsPosition)GetValue(NavigationButtonsPositionProperty);
            set => SetValue(NavigationButtonsPositionProperty, value);
        }

        /// <summary>Identifies the <see cref="Orientation"/> dependency property.</summary>
        public static readonly DependencyProperty OrientationProperty = DependencyProperty.Register(nameof(Orientation), typeof(Orientation), typeof(FlipView), new PropertyMetadata(Orientation.Horizontal, (d, e) => ((FlipView)d).DetectControlButtonsStatus()));

        /// <summary>
        /// Gets or sets the orientation of the navigation.
        /// </summary>
        public Orientation Orientation
        {
            get => (Orientation)this.GetValue(OrientationProperty);
            set => this.SetValue(OrientationProperty, value);
        }

        /// <summary>Identifies the <see cref="LeftTransition"/> dependency property.</summary>
        public static readonly DependencyProperty LeftTransitionProperty = DependencyProperty.Register(nameof(LeftTransition), typeof(TransitionType), typeof(FlipView), new PropertyMetadata(TransitionType.LeftReplace));

        /// <summary>
        /// Gets or sets the transition of the left navigation.
        /// </summary>
        public TransitionType LeftTransition
        {
            get => (TransitionType)this.GetValue(LeftTransitionProperty);
            set => this.SetValue(LeftTransitionProperty, value);
        }

        /// <summary>Identifies the <see cref="RightTransition"/> dependency property.</summary>
        public static readonly DependencyProperty RightTransitionProperty = DependencyProperty.Register(nameof(RightTransition), typeof(TransitionType), typeof(FlipView), new PropertyMetadata(TransitionType.RightReplace));

        /// <summary>
        /// Gets or sets the transition of the right navigation.
        /// </summary>
        public TransitionType RightTransition
        {
            get => (TransitionType)this.GetValue(RightTransitionProperty);
            set => this.SetValue(RightTransitionProperty, value);
        }

        /// <summary>Identifies the <see cref="UpTransition"/> dependency property.</summary>
        public static readonly DependencyProperty UpTransitionProperty = DependencyProperty.Register(nameof(UpTransition), typeof(TransitionType), typeof(FlipView), new PropertyMetadata(TransitionType.Up));

        /// <summary>
        /// Gets or sets the transition of the up navigation.
        /// </summary>
        public TransitionType UpTransition
        {
            get => (TransitionType)this.GetValue(UpTransitionProperty);
            set => this.SetValue(UpTransitionProperty, value);
        }

        /// <summary>Identifies the <see cref="DownTransition"/> dependency property.</summary>
        public static readonly DependencyProperty DownTransitionProperty = DependencyProperty.Register(nameof(DownTransition), typeof(TransitionType), typeof(FlipView), new PropertyMetadata(TransitionType.Down));

        /// <summary>
        /// Gets or sets the transition of the down navigation.
        /// </summary>
        public TransitionType DownTransition
        {
            get => (TransitionType)this.GetValue(DownTransitionProperty);
            set => this.SetValue(DownTransitionProperty, value);
        }

        /// <summary>Identifies the <see cref="IsBannerEnabled"/> dependency property.</summary>
        public static readonly DependencyProperty IsBannerEnabledProperty = DependencyProperty.Register(nameof(IsBannerEnabled), typeof(bool), typeof(FlipView), new UIPropertyMetadata(BooleanBoxes.TrueBox, OnIsBannerEnabledPropertyChangedCallback));

        /// <summary>
        /// Gets or sets whether the banner is visible or not.
        /// </summary>
        public bool IsBannerEnabled
        {
            get => (bool)this.GetValue(IsBannerEnabledProperty);
            set => this.SetValue(IsBannerEnabledProperty, BooleanBoxes.Box(value));
        }

        private static void OnIsBannerEnabledPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var flipView = (FlipView)d;

            void ChangeFlipViewBannerVisibility()
            {
                if ((bool)e.NewValue)
                {
                    flipView.ChangeBannerText(flipView.BannerText);
                    flipView.ShowBanner();
                }
                else
                {
                    flipView.HideBanner();
                }
            }

            if (flipView.IsLoaded)
            {
                ChangeFlipViewBannerVisibility();
            }
            else
            {
                //wait to be loaded?
                flipView.ExecuteWhenLoaded(() =>
                    {
                        flipView.ApplyTemplate();
                        ChangeFlipViewBannerVisibility();
                    });
            }
        }

        /// <summary>Identifies the <see cref="IsNavigationEnabled"/> dependency property.</summary>
        public static readonly DependencyProperty IsNavigationEnabledProperty = DependencyProperty.Register(nameof(IsNavigationEnabled), typeof(bool), typeof(FlipView), new PropertyMetadata(BooleanBoxes.TrueBox, (d, e) => ((FlipView)d).DetectControlButtonsStatus()));

        /// <summary>
        /// Gets or sets whether the navigation button are visible or not.
        /// </summary>
        public bool IsNavigationEnabled
        {
            get => (bool)this.GetValue(IsNavigationEnabledProperty);
            set => this.SetValue(IsNavigationEnabledProperty, BooleanBoxes.Box(value));
        }

        /// <summary>Identifies the <see cref="BannerText"/> dependency property.</summary>
        public static readonly DependencyProperty BannerTextProperty = DependencyProperty.Register(nameof(BannerText), typeof(string), typeof(FlipView), new FrameworkPropertyMetadata("Banner", FrameworkPropertyMetadataOptions.AffectsRender, (d, e) => ((FlipView)d).ExecuteWhenLoaded(() => ((FlipView)d).ChangeBannerText((string)e.NewValue))));

        /// <summary>
        /// Gets or sets the banner text.
        /// </summary>
        public string BannerText
        {
            get => (string)this.GetValue(BannerTextProperty);
            set => this.SetValue(BannerTextProperty, value);
        }

        private const string PART_BackButton = "PART_BackButton";
        private const string PART_BannerGrid = "PART_BannerGrid";
        private const string PART_BannerLabel = "PART_BannerLabel";
        private const string PART_DownButton = "PART_DownButton";
        private const string PART_ForwardButton = "PART_ForwardButton";
        private const string PART_Presenter = "PART_Presenter";
        private const string PART_UpButton = "PART_UpButton";
        private bool allowSelectedIndexChangedCallback = true;
        private Button backButton;
        private Grid bannerGrid;
        private Label bannerLabel;
        private Button downButton;
        private Button forwardButton;
        private Storyboard hideBannerStoryboard;
        private Storyboard hideControlStoryboard;
        private EventHandler hideControlStoryboardCompletedHandler;
        /// <summary>
        /// To counteract the double Loaded event issue.
        /// </summary>
        private bool loaded;
        private TransitioningContentControl presenter;
        private Storyboard showBannerStoryboard;
        private Storyboard showControlStoryboard;
        private Button upButton;

        static FlipView()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(FlipView), new FrameworkPropertyMetadata(typeof(FlipView)));

            /* Hook SelectedIndexProperty's coercion.
             * Coercion is called whenever the value of the DependencyProperty is being re-evaluated or coercion is specifically requested.
             * Coercion has access to current and proposed value to enforce compliance.
             * It is called after ValidateCallback.
             * So one can ultimately use this callback like a value is changing event.
             * As this control doesn't implement System.ComponentModel.INotifyPropertyChanging,
             * it's the only way to determine from/to index and ensure Transition consistency in any use case of the control.
             */
            var previousSelectedIndexPropertyMetadata = SelectedIndexProperty.GetMetadata(typeof(FlipView));
            SelectedIndexProperty.OverrideMetadata(typeof(FlipView),
                                                   new FrameworkPropertyMetadata
                                                   {
                                                       /* Coercion being behavior critical, we don't want to replace inherited or added callbacks.
                                                        * They must be called before ours and most of all : their result must be our input.
                                                        * But since delegates are multicast (meaning they can have more than on target), each target would sequentially be executed with the same original input
                                                        * thus not chaining invocations' inputs/outputs. So the caller would retrieve the sole last target's return value, ignoring previous computations.
                                                        * Hence, we chain coerions inputs/outputs until our callback to preserve the behavior of the control
                                                        * and be sure the value won't change anymore before being actually set.
                                                        */
                                                       CoerceValueCallback = (d, value) =>
                                                           {
                                                               /* Chain actual coercions... */
                                                               if (previousSelectedIndexPropertyMetadata.CoerceValueCallback != null)
                                                               {
                                                                   foreach (var item in previousSelectedIndexPropertyMetadata.CoerceValueCallback.GetInvocationList())
                                                                   {
                                                                       value = ((CoerceValueCallback)item)(d, value);
                                                                   }
                                                               }

                                                               /* ...'til our new one. */
                                                               return CoerceSelectedIndexProperty(d, value);
                                                           }
                                                   });
        }

        public FlipView()
        {
            this.Loaded += this.FlipViewLoaded;
        }

        /// <summary>
        /// Coerce SelectedIndexProperty's value.
        /// </summary>
        /// <param name="d">The object that the property exists on.</param>
        /// <param name="value">The new value of the property, prior to any coercion attempt.</param>
        /// <returns>The coerced value (with appropriate type). </returns>
        private static object CoerceSelectedIndexProperty(DependencyObject d, object value)
        {
            // call ComputeTransition only if SelectedIndex is changed from outside and not from GoBack or GoForward
            if (d is FlipView flipView && flipView.allowSelectedIndexChangedCallback)
            {
                flipView.ComputeTransition(flipView.SelectedIndex, value as int? ?? flipView.SelectedIndex);
            }

            return value;
        }

        /// <summary>
        /// Changes the current slide to the previous item.
        /// </summary>
        public void GoBack()
        {
            this.allowSelectedIndexChangedCallback = false;
            this.presenter.Transition = this.Orientation == Orientation.Horizontal ? this.RightTransition : this.UpTransition;
            if (this.SelectedIndex > 0)
            {
                this.SelectedIndex--;
            }
            else
            {
                if (this.CircularNavigation)
                {
                    this.SelectedIndex = this.Items.Count - 1;
                }
            }

            this.allowSelectedIndexChangedCallback = true;
        }

        /// <summary>
        /// Changes the current to the next item.
        /// </summary>
        public void GoForward()
        {
            this.allowSelectedIndexChangedCallback = false;
            this.presenter.Transition = this.Orientation == Orientation.Horizontal ? this.LeftTransition : this.DownTransition;
            if (this.SelectedIndex < this.Items.Count - 1)
            {
                this.SelectedIndex++;
            }
            else
            {
                if (this.CircularNavigation)
                {
                    this.SelectedIndex = 0;
                }
            }

            this.allowSelectedIndexChangedCallback = true;
        }

        /// <summary>
        /// Brings the control buttons (next/previous) into view.
        /// </summary>
        public void ShowControlButtons()
        {
            this.ExecuteWhenLoaded(() => this.DetectControlButtonsStatus());
        }

        /// <summary>
        /// Removes the control buttons (next/previous) from view.
        /// </summary>
        public void HideControlButtons()
        {
            this.ExecuteWhenLoaded(() => this.DetectControlButtonsStatus(Visibility.Hidden));
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            this.showBannerStoryboard = ((Storyboard)this.Template.Resources["ShowBannerStoryboard"]).Clone();
            this.hideBannerStoryboard = ((Storyboard)this.Template.Resources["HideBannerStoryboard"]).Clone();

            this.showControlStoryboard = ((Storyboard)this.Template.Resources["ShowControlStoryboard"]).Clone();
            this.hideControlStoryboard = ((Storyboard)this.Template.Resources["HideControlStoryboard"]).Clone();

            this.presenter = this.GetTemplateChild(PART_Presenter) as TransitioningContentControl;

            if (this.forwardButton != null)
            {
                this.forwardButton.Click -= this.NextButtonClick;
            }

            if (this.backButton != null)
            {
                this.backButton.Click -= this.PrevButtonClick;
            }

            if (this.upButton != null)
            {
                this.upButton.Click -= this.PrevButtonClick;
            }

            if (this.downButton != null)
            {
                this.downButton.Click -= this.NextButtonClick;
            }

            this.forwardButton = this.GetTemplateChild(PART_ForwardButton) as Button;
            this.backButton = this.GetTemplateChild(PART_BackButton) as Button;
            this.upButton = this.GetTemplateChild(PART_UpButton) as Button;
            this.downButton = this.GetTemplateChild(PART_DownButton) as Button;

            this.bannerGrid = this.GetTemplateChild(PART_BannerGrid) as Grid;
            this.bannerLabel = this.GetTemplateChild(PART_BannerLabel) as Label;

            if (this.forwardButton != null)
            {
                this.forwardButton.Click += this.NextButtonClick;
            }

            if (this.backButton != null)
            {
                this.backButton.Click += this.PrevButtonClick;
            }

            if (this.upButton != null)
            {
                this.upButton.Click += this.PrevButtonClick;
            }

            if (this.downButton != null)
            {
                this.downButton.Click += this.NextButtonClick;
            }

            if (this.bannerLabel != null)
            {
                this.bannerLabel.Opacity = this.IsBannerEnabled ? 1d : 0d;
            }
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new FlipViewItem() { HorizontalAlignment = HorizontalAlignment.Stretch };
        }

        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is FlipViewItem;
        }

        protected override void OnItemsChanged(System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            base.OnItemsChanged(e);
            this.DetectControlButtonsStatus();
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            var isHorizontal = this.Orientation == Orientation.Horizontal;
            var isVertical = this.Orientation == Orientation.Vertical;
            var canGoPrev = (e.Key == Key.Left && isHorizontal && this.backButton != null && this.backButton.Visibility == Visibility.Visible && this.backButton.IsEnabled)
                            || (e.Key == Key.Up && isVertical && this.upButton != null && this.upButton.Visibility == Visibility.Visible && this.upButton.IsEnabled);
            var canGoNext = (e.Key == Key.Right && isHorizontal && this.forwardButton != null && this.forwardButton.Visibility == Visibility.Visible && this.forwardButton.IsEnabled)
                            || (e.Key == Key.Down && isVertical && this.downButton != null && this.downButton.Visibility == Visibility.Visible && this.downButton.IsEnabled);

            if (canGoPrev)
            {
                this.GoBack();
                e.Handled = true;
                this.Focus();
            }
            else if (canGoNext)
            {
                this.GoForward();
                e.Handled = true;
                this.Focus();
            }
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);
            this.Focus();
        }

        protected override void OnSelectionChanged(SelectionChangedEventArgs e)
        {
            base.OnSelectionChanged(e);
            this.DetectControlButtonsStatus();
        }

        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            if (element != item)
            {
                element.SetValue(DataContextProperty, item); // don't want to set the DataContext to itself.
            }

            base.PrepareContainerForItemOverride(element, item);
        }

        /// <summary>
        /// Applies actions to navigation buttons.
        /// </summary>
        /// <param name="prevButtonApply">Action applied to the previous button.</param>
        /// <param name="nextButtonApply">Action applied to the next button.</param>
        /// <param name="inactiveButtonsApply">Action applied to the inactive buttons.</param>
        /// <exception cref="ArgumentNullException">Any action is null.</exception>
        private void ApplyToNavigationButtons(Action<Button> prevButtonApply, Action<Button> nextButtonApply, Action<Button> inactiveButtonsApply)
        {
            if (prevButtonApply == null)
            {
                throw new ArgumentNullException(nameof(prevButtonApply));
            }

            if (nextButtonApply == null)
            {
                throw new ArgumentNullException(nameof(nextButtonApply));
            }

            if (inactiveButtonsApply == null)
            {
                throw new ArgumentNullException(nameof(inactiveButtonsApply));
            }

            this.GetNavigationButtons(out var prevButton, out var nextButton, out var inactiveButtons);

            foreach (var button in inactiveButtons.Where(b => !(b is null)))
            {
                inactiveButtonsApply(button);
            }

            if (prevButton != null)
            {
                prevButtonApply(prevButton);
            }

            if (nextButton != null)
            {
                nextButtonApply(nextButton);
            }
        }

        private void ChangeBannerText(string value = null)
        {
            if (this.IsBannerEnabled)
            {
                var newValue = value ?? this.BannerText;
                if (newValue == null || this.hideControlStoryboard == null)
                {
                    return;
                }

                if (this.hideControlStoryboardCompletedHandler != null)
                {
                    this.hideControlStoryboard.Completed -= this.hideControlStoryboardCompletedHandler;
                }

                this.hideControlStoryboardCompletedHandler = (sender, e) =>
                    {
                        try
                        {
                            this.hideControlStoryboard.Completed -= this.hideControlStoryboardCompletedHandler;

                            this.bannerLabel.Content = newValue;

                            this.bannerLabel.BeginStoryboard(this.showControlStoryboard, HandoffBehavior.SnapshotAndReplace);
                        }
                        catch (Exception)
                        {
                        }
                    };

                this.hideControlStoryboard.Completed += this.hideControlStoryboardCompletedHandler;

                this.bannerLabel.BeginStoryboard(this.hideControlStoryboard, HandoffBehavior.SnapshotAndReplace);
            }
            else
            {
                this.ExecuteWhenLoaded(() => { this.bannerLabel.Content = value ?? this.BannerText; });
            }
        }

        /// <summary>
        /// Computes the transition when changing selected index.
        /// </summary>
        /// <param name="fromIndex">Previous selected index.</param>
        /// <param name="toIndex">New selected index.</param>
        private void ComputeTransition(int fromIndex, int toIndex)
        {
            if (this.presenter != null)
            {
                if (fromIndex < toIndex)
                {
                    this.presenter.Transition = this.Orientation == Orientation.Horizontal ? this.LeftTransition : this.DownTransition;
                }
                else if (fromIndex > toIndex)
                {
                    this.presenter.Transition = this.Orientation == Orientation.Horizontal ? this.RightTransition : this.UpTransition;
                }
                else
                {
                    this.presenter.Transition = TransitionType.Default;
                }
            }
        }

        /// <summary>
        /// Sets the visibility of navigation buttons.
        /// </summary>
        /// <param name="activeButtonsVisibility">Visibility of active buttons.</param>
        private void DetectControlButtonsStatus(Visibility activeButtonsVisibility = Visibility.Visible)
        {
            if (!this.IsNavigationEnabled)
            {
                activeButtonsVisibility = Visibility.Hidden;
            }

            this.ApplyToNavigationButtons(
                prev => prev.Visibility = this.CircularNavigation || (this.Items.Count > 0 && this.SelectedIndex > 0) ? activeButtonsVisibility : Visibility.Hidden,
                next => next.Visibility = this.CircularNavigation || (this.Items.Count > 0 && this.SelectedIndex < this.Items.Count - 1) ? activeButtonsVisibility : Visibility.Hidden,
                inactive => inactive.Visibility = Visibility.Hidden);
        }

        private void FlipViewLoaded(object sender, RoutedEventArgs e)
        {
            /* Loaded event fires twice if its a child of a TabControl.
             * Once because the TabControl seems to initiali(z|s)e everything.
             * And a second time when the Tab (housing the FlipView) is switched to. */

            // if OnApplyTemplate hasn't been called yet.
            if (this.backButton == null || this.forwardButton == null || this.upButton == null || this.downButton == null)
            {
                this.ApplyTemplate();
            }

            // Counteracts the double 'Loaded' event issue.
            if (this.loaded)
            {
                return;
            }

            this.Unloaded += this.FlipViewUnloaded;

            if (this.SelectedIndex < 0)
            {
                this.SelectedIndex = 0;
            }

            this.DetectControlButtonsStatus();

            this.ShowBanner();

            this.loaded = true;
        }

        private void FlipViewUnloaded(object sender, RoutedEventArgs e)
        {
            this.Unloaded -= this.FlipViewUnloaded;

            if (this.hideControlStoryboard != null && this.hideControlStoryboardCompletedHandler != null)
            {
                this.hideControlStoryboard.Completed -= this.hideControlStoryboardCompletedHandler;
            }

            this.loaded = false;
        }

        /// <summary>
        /// Gets the navigation buttons.
        /// </summary>
        /// <param name="prevButton">Previous button.</param>
        /// <param name="nextButton">Next button.</param>
        /// <param name="inactiveButtons">Inactive buttons.</param>
        private void GetNavigationButtons(out Button prevButton, out Button nextButton, out IEnumerable<Button> inactiveButtons)
        {
            if (this.Orientation == Orientation.Horizontal)
            {
                prevButton = this.backButton;
                nextButton = this.forwardButton;
                inactiveButtons = new[] { this.upButton, this.downButton };
            }
            else
            {
                prevButton = this.upButton;
                nextButton = this.downButton;
                inactiveButtons = new[] { this.backButton, this.forwardButton };
            }
        }

        private void HideBanner()
        {
            if (this.ActualHeight > 0.0)
            {
                this.bannerLabel?.BeginStoryboard(this.hideControlStoryboard);
                this.bannerGrid?.BeginStoryboard(this.hideBannerStoryboard);
            }
        }

        private void NextButtonClick(object sender, RoutedEventArgs e)
        {
            this.GoForward();
        }

        private void PrevButtonClick(object sender, RoutedEventArgs e)
        {
            this.GoBack();
        }

        private void ShowBanner()
        {
            if (this.IsBannerEnabled)
            {
                this.bannerGrid?.BeginStoryboard(this.showBannerStoryboard);
            }
        }
    }
}