using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace MahApps.Metro.Controls
{
    public class FlipViewItem : ContentControl
    { }

    /// <summary>
    /// A control that imitate a slideshow with back/forward buttons.
    /// </summary>
    [TemplatePart(Name = "PART_Presenter", Type = typeof(TransitioningContentControl))]
    [TemplatePart(Name = "PART_BackButton", Type = typeof(Button))]
    [TemplatePart(Name = "PART_ForwardButton", Type = typeof(Button))]
    [TemplatePart(Name = "PART_UpButton", Type = typeof(Button))]
    [TemplatePart(Name = "PART_DownButton", Type = typeof(Button))]
    [TemplatePart(Name = "PART_BannerGrid", Type = typeof(Grid))]
    [TemplatePart(Name = "PART_BannerLabel", Type = typeof(Label))]
    public class FlipView : Selector
    {
        private const string PART_Presenter = "PART_Presenter";
        private const string PART_BackButton = "PART_BackButton";
        private const string PART_ForwardButton = "PART_ForwardButton";
        private const string PART_UpButton = "PART_UpButton";
        private const string PART_DownButton = "PART_DownButton";
        private const string PART_BannerGrid = "PART_BannerGrid";
        private const string PART_BannerLabel = "PART_BannerLabel";

        private TransitioningContentControl presenter;
        private Button backButton;
        private Button forwardButton;
        private Button upButton;
        private Button downButton;
        private Grid bannerGrid;
        private Label bannerLabel;

        private Storyboard showBannerStoryboard;
        private Storyboard hideBannerStoryboard;
        private Storyboard hideControlStoryboard;
        private Storyboard showControlStoryboard;

        private EventHandler hideControlStoryboardCompletedHandler;
        /// <summary>
        /// To counteract the double Loaded event issue.
        /// </summary>
        private bool loaded;
        private bool controlsVisibilityOverride;

        static FlipView()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(FlipView), new FrameworkPropertyMetadata(typeof(FlipView)));
        }
        
        public FlipView()
        {
            this.Loaded += FlipView_Loaded;
            this.MouseLeftButtonDown += FlipView_MouseLeftButtonDown;
        }

        void FlipView_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.Focus();
        }

        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is FlipViewItem;
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new FlipViewItem() { HorizontalAlignment = HorizontalAlignment.Stretch };
        }

        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            if (element != item)
                element.SetCurrentValue(DataContextProperty, item); //dont want to set the datacontext to itself. taken from MetroTabControl.cs

            base.PrepareContainerForItemOverride(element, item);
        }

        void FlipView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DetectControlButtonsStatus();
        }

        private void DetectControlButtonsStatus()
        {
            if (controlsVisibilityOverride) return;

            var prevButton = Orientation == Orientation.Horizontal ? backButton : upButton;
            var nextButton = Orientation == Orientation.Horizontal ? forwardButton : downButton;

            if (prevButton == null || nextButton == null)
            {
                return;
            }

            prevButton.Visibility = this.Items.Count <= 0 || !this.CircularNavigation && this.SelectedIndex == 0 ? Visibility.Hidden : Visibility.Visible;
            nextButton.Visibility = this.Items.Count <= 0 || !this.CircularNavigation && this.SelectedIndex == (this.Items.Count - 1) ? Visibility.Hidden : Visibility.Visible;
        }

        void FlipView_Loaded(object sender, RoutedEventArgs e)
        {
            /* Loaded event fires twice if its a child of a TabControl.
             * Once because the TabControl seems to initiali(z|s)e everything.
             * And a second time when the Tab (housing the FlipView) is switched to. */

            // if OnApplyTemplate hasn't been called yet.
            if (backButton == null || forwardButton == null || upButton == null || downButton == null)
            {
                ApplyTemplate();
            }

            // Counteracts the double 'Loaded' event issue.
            if (loaded)
            {
                return;
            }

            this.Unloaded += FlipView_Unloaded;
            backButton.Click += this.PrevButtonClick;
            forwardButton.Click += this.NextButtonClick;
            upButton.Click += this.PrevButtonClick;
            downButton.Click += this.NextButtonClick;

            this.SelectionChanged += FlipView_SelectionChanged;
            this.KeyDown += FlipView_KeyDown;

            if (SelectedIndex < 0)
            {
                SelectedIndex = 0;
            }

            DetectControlButtonsStatus();

            ShowBanner();

            loaded = true;
        }

        void FlipView_Unloaded(object sender, RoutedEventArgs e)
        {
            this.Unloaded -= FlipView_Unloaded;
            this.MouseLeftButtonDown -= FlipView_MouseLeftButtonDown;
            this.SelectionChanged -= FlipView_SelectionChanged;

            this.KeyDown -= FlipView_KeyDown;
            backButton.Click -= this.PrevButtonClick;
            forwardButton.Click -= this.NextButtonClick;
            upButton.Click -= this.PrevButtonClick;
            downButton.Click -= this.NextButtonClick;

            if (hideControlStoryboard != null && hideControlStoryboardCompletedHandler != null)
            {
                hideControlStoryboard.Completed -= hideControlStoryboardCompletedHandler;
            }

            loaded = false;
        }

        void FlipView_KeyDown(object sender, KeyEventArgs e)
        {
            var canGoPrev = (e.Key == Key.Left && Orientation == Orientation.Horizontal && backButton != null && backButton.Visibility == Visibility.Visible && backButton.IsEnabled)
                         || (e.Key == Key.Up && Orientation == Orientation.Vertical && upButton != null && upButton.Visibility == Visibility.Visible && upButton.IsEnabled);
            var canGoNext = (e.Key == Key.Right && Orientation == Orientation.Horizontal && forwardButton != null && forwardButton.Visibility == Visibility.Visible && forwardButton.IsEnabled)
                         || (e.Key == Key.Down && Orientation == Orientation.Vertical && downButton != null && downButton.Visibility == Visibility.Visible && downButton.IsEnabled);
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

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            showBannerStoryboard = ((Storyboard)this.Template.Resources["ShowBannerStoryboard"]).Clone();
            hideBannerStoryboard = ((Storyboard)this.Template.Resources["HideBannerStoryboard"]).Clone();

            showControlStoryboard = ((Storyboard)this.Template.Resources["ShowControlStoryboard"]).Clone();
            hideControlStoryboard = ((Storyboard)this.Template.Resources["HideControlStoryboard"]).Clone();

            presenter = GetTemplateChild(PART_Presenter) as TransitioningContentControl;
            backButton = GetTemplateChild(PART_BackButton) as Button;
            forwardButton = GetTemplateChild(PART_ForwardButton) as Button;
            upButton = GetTemplateChild(PART_UpButton) as Button;
            downButton = GetTemplateChild(PART_DownButton) as Button;
            bannerGrid = GetTemplateChild(PART_BannerGrid) as Grid;
            bannerLabel = GetTemplateChild(PART_BannerLabel) as Label;

            bannerLabel.Opacity = IsBannerEnabled ? 1.0 : 0.0;
        }

        protected override void OnItemsSourceChanged(System.Collections.IEnumerable oldValue, System.Collections.IEnumerable newValue)
        {
            base.OnItemsSourceChanged(oldValue, newValue);

            SelectedIndex = 0;
        }

        protected override void OnItemsChanged(System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            base.OnItemsChanged(e);

            DetectControlButtonsStatus();
        }

        void NextButtonClick(object sender, RoutedEventArgs e)
        {
            GoForward();
        }

        void PrevButtonClick(object sender, RoutedEventArgs e)
        {
            GoBack();
        }

        /// <summary>
        /// Changes the current slide to the previous item.
        /// </summary>
        public void GoBack()
        {
            if (SelectedIndex > 0)
            {
                presenter.Transition = Orientation == Orientation.Horizontal ? RightTransition : UpTransition;
                SelectedIndex--;
            }
            else
            {
                if (this.CircularNavigation)
                {
                    SelectedIndex = Items.Count - 1;
                }
            }
        }

        /// <summary>
        /// Changes the current to the next item.
        /// </summary>
        public void GoForward()
        {
            if (SelectedIndex < Items.Count - 1)
            {
                presenter.Transition = Orientation == Orientation.Horizontal ? LeftTransition : DownTransition;
                SelectedIndex++;
            }
            else
            {
                if (this.CircularNavigation)
                {
                    SelectedIndex = 0;
                }
            }
        }

        /// <summary>
        /// Brings the control buttons (next/previous) into view.
        /// </summary>
        public void ShowControlButtons()
        {
            controlsVisibilityOverride = false;
            ExecuteWhenLoaded(this, () =>
                {
                    var prevButton = Orientation == Orientation.Horizontal ? backButton : upButton;
                    var nextButton = Orientation == Orientation.Horizontal ? forwardButton : downButton;
                    if (prevButton != null && nextButton != null)
                    {
                        prevButton.Visibility = Visibility.Visible;
                        nextButton.Visibility = Visibility.Visible;
                    }
                });
        }
        /// <summary>
        /// Removes the control buttons (next/previous) from view.
        /// </summary>
        public void HideControlButtons()
        {
            controlsVisibilityOverride = true;
            ExecuteWhenLoaded(this, () =>
                {
                    var prevButton = Orientation == Orientation.Horizontal ? backButton : upButton;
                    var nextButton = Orientation == Orientation.Horizontal ? forwardButton : downButton;
                    if (prevButton != null && nextButton != null)
                    {
                        prevButton.Visibility = Visibility.Hidden;
                        nextButton.Visibility = Visibility.Hidden;
                    }
                });
        }

        private void ShowBanner()
        {
            if (IsBannerEnabled)
            {
                bannerGrid.BeginStoryboard(showBannerStoryboard);
            }
        }

        private void HideBanner()
        {
            if (this.ActualHeight > 0.0)
            {
                bannerLabel.BeginStoryboard(hideControlStoryboard);
                bannerGrid.BeginStoryboard(hideBannerStoryboard);
            }
        }

        private static void ExecuteWhenLoaded(FlipView flipview, Action body)
        {
            if (flipview.IsLoaded)
            {
                System.Windows.Threading.Dispatcher.CurrentDispatcher.Invoke(body);
            }
            else
            {
                RoutedEventHandler handler = null;
                handler = (o, a) => {
                    flipview.Loaded -= handler;
                    System.Windows.Threading.Dispatcher.CurrentDispatcher.Invoke(body);
                };

                flipview.Loaded += handler;
            }
        }

        public static readonly DependencyProperty UpTransitionProperty = DependencyProperty.Register("UpTransition", typeof(TransitionType), typeof(FlipView), new PropertyMetadata(TransitionType.Up));
        public static readonly DependencyProperty DownTransitionProperty = DependencyProperty.Register("DownTransition", typeof(TransitionType), typeof(FlipView), new PropertyMetadata(TransitionType.Down));
        public static readonly DependencyProperty LeftTransitionProperty = DependencyProperty.Register("LeftTransition", typeof(TransitionType), typeof(FlipView), new PropertyMetadata(TransitionType.LeftReplace));
        public static readonly DependencyProperty RightTransitionProperty = DependencyProperty.Register("RightTransition", typeof(TransitionType), typeof(FlipView), new PropertyMetadata(TransitionType.RightReplace));
        [Obsolete(@"This property will be deleted in the next release. You should use now MouseHoverBorderEnabled instead.")]
        public static readonly DependencyProperty MouseOverGlowEnabledProperty = DependencyProperty.Register("MouseOverGlowEnabled", typeof(bool), typeof(FlipView), new PropertyMetadata(true, (o, e) => ((FlipView)o).MouseHoverBorderEnabled = (bool)e.NewValue));
        public static readonly DependencyProperty MouseHoverBorderEnabledProperty = DependencyProperty.Register("MouseHoverBorderEnabled", typeof(bool), typeof(FlipView), new PropertyMetadata(true));
        public static readonly DependencyProperty MouseHoverBorderBrushProperty = DependencyProperty.Register("MouseHoverBorderBrush", typeof(Brush), typeof(FlipView), new PropertyMetadata(Brushes.LightGray));
        public static readonly DependencyProperty MouseHoverBorderThicknessProperty = DependencyProperty.Register("MouseHoverBorderThickness", typeof(Thickness), typeof(FlipView), new PropertyMetadata(new Thickness(4)));
        public static readonly DependencyProperty OrientationProperty = DependencyProperty.Register("Orientation", typeof(Orientation), typeof(FlipView), new PropertyMetadata(Orientation.Horizontal));
        public static readonly DependencyProperty IsBannerEnabledProperty = DependencyProperty.Register("IsBannerEnabled", typeof(bool), typeof(FlipView), new UIPropertyMetadata(true, OnIsBannerEnabledPropertyChangedCallback));
        public static readonly DependencyProperty BannerTextProperty = DependencyProperty.Register("BannerText", typeof(string), typeof(FlipView), new FrameworkPropertyMetadata("Banner", FrameworkPropertyMetadataOptions.AffectsRender, (d, e) => ExecuteWhenLoaded(((FlipView)d), () => ((FlipView)d).ChangeBannerText((string)e.NewValue))));
        public static readonly DependencyProperty CircularNavigationProperty = DependencyProperty.Register("CircularNavigation", typeof(bool), typeof(FlipView), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsRender, (d, e) => ((FlipView)d).DetectControlButtonsStatus()));

        public TransitionType UpTransition
        {
            get { return (TransitionType)GetValue(UpTransitionProperty); }
            set { SetValue(UpTransitionProperty, value); }
        }

        public TransitionType DownTransition
        {
            get { return (TransitionType)GetValue(DownTransitionProperty); }
            set { SetValue(DownTransitionProperty, value); }
        }

        public TransitionType LeftTransition
        {
            get { return (TransitionType)GetValue(LeftTransitionProperty); }
            set { SetValue(LeftTransitionProperty, value); }
        }

        public TransitionType RightTransition
        {
            get { return (TransitionType)GetValue(RightTransitionProperty); }
            set { SetValue(RightTransitionProperty, value); }
        }

        public bool MouseOverGlowEnabled
        {
            get { return (bool)GetValue(MouseOverGlowEnabledProperty); }
            set { SetValue(MouseOverGlowEnabledProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the border for mouse over state is enabled or not.
        /// </summary>
        public bool MouseHoverBorderEnabled
        {
            get { return (bool)GetValue(MouseHoverBorderEnabledProperty); }
            set { SetValue(MouseHoverBorderEnabledProperty, value); }
        }

        /// <summary>
        /// Gets or sets the mouse hover border brush.
        /// </summary>
        public Brush MouseHoverBorderBrush
        {
            get { return (Brush)GetValue(MouseHoverBorderBrushProperty); }
            set { SetValue(MouseHoverBorderBrushProperty, value); }
        }

        /// <summary>
        /// Gets or sets the mouse hover border thickness.
        /// </summary>
        public Thickness MouseHoverBorderThickness
        {
            get { return (Thickness)GetValue(MouseHoverBorderThicknessProperty); }
            set { SetValue(MouseHoverBorderThicknessProperty, value); }
        }

        public Orientation Orientation
        {
            get { return (Orientation)GetValue(OrientationProperty); }
            set { SetValue(OrientationProperty, value); }
        }

        /// <summary>
        /// Gets/sets the text that is displayed in the FlipView's banner.
        /// </summary>
        public string BannerText
        {
            get { return (string)GetValue(BannerTextProperty); }
            set { SetValue(BannerTextProperty, value); }
        }

        /// <summary>
        /// Gets/sets whether the FlipView's banner is visible.
        /// </summary>
        public bool IsBannerEnabled
        {
            get { return (bool)GetValue(IsBannerEnabledProperty); }
            set { SetValue(IsBannerEnabledProperty, value); }
        }


        /// <summary>
        /// Gets or sets a value indicating whether the navigation is circular, so you get the first after last and the last before first.
        /// </summary>
        public bool CircularNavigation
        {
            get { return (bool)GetValue(CircularNavigationProperty); }
            set { SetValue(CircularNavigationProperty, value); }
        }

        private void ChangeBannerText(string value = null)
        {
            if (IsBannerEnabled)
            {
                var newValue = value ?? BannerText;

                if (newValue == null || hideControlStoryboard == null)
                {
                    return;
                }

                if (hideControlStoryboardCompletedHandler != null)
                {
                    hideControlStoryboard.Completed -= hideControlStoryboardCompletedHandler;
                }

                hideControlStoryboardCompletedHandler = (sender, e) => {
                    try
                    {
                        hideControlStoryboard.Completed -= hideControlStoryboardCompletedHandler;

                        bannerLabel.Content = newValue;

                        bannerLabel.BeginStoryboard(showControlStoryboard, HandoffBehavior.SnapshotAndReplace);
                    }
                    catch (Exception)
                    {
                    }
                };


                hideControlStoryboard.Completed += hideControlStoryboardCompletedHandler;

                bannerLabel.BeginStoryboard(hideControlStoryboard, HandoffBehavior.SnapshotAndReplace);
            }

            else
            {
                ExecuteWhenLoaded(this, () => {
                    bannerLabel.Content = value ?? BannerText;
                });
            }
        }

        private static void OnIsBannerEnabledPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var flipview = ((FlipView)d);

            if (!flipview.IsLoaded)
            {
                //wait to be loaded?
                ExecuteWhenLoaded(flipview, () => {
                    flipview.ApplyTemplate();

                    if ((bool)e.NewValue)
                    {
                        flipview.ChangeBannerText(flipview.BannerText);
                        flipview.ShowBanner();
                    }
                    else
                    {
                        flipview.HideBanner();
                    }
                });
            }
            else
            {
                if ((bool)e.NewValue)
                {
                    flipview.ChangeBannerText(flipview.BannerText);
                    flipview.ShowBanner();
                }
                else
                {
                    flipview.HideBanner();
                }
            }
        }

    }
}
