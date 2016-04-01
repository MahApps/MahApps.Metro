﻿using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace MahApps.Metro.Controls
{
    /// <summary>
    /// A sliding panel control that is hosted in a MetroWindow via a FlyoutsControl.
    /// <see cref="MetroWindow"/>
    /// <seealso cref="FlyoutsControl"/>
    /// </summary>
    [TemplatePart(Name = "PART_BackButton", Type = typeof(Button))]
    [TemplatePart(Name = "PART_BackHeaderText", Type = typeof(TextBlock))]
    [TemplatePart(Name = "PART_WindowTitleThumb", Type = typeof(Thumb))]
    [TemplatePart(Name = "PART_Root", Type = typeof(Grid))]
    [TemplatePart(Name = "PART_Header", Type = typeof(ContentPresenter))]
    [TemplatePart(Name = "PART_Content", Type = typeof(ContentPresenter))]
    public class Flyout : ContentControl
    {
        /// <summary>
        /// An event that is raised when IsOpen changes.
        /// </summary>
        public static readonly RoutedEvent IsOpenChangedEvent =
            EventManager.RegisterRoutedEvent("IsOpenChanged", RoutingStrategy.Bubble,
                typeof(RoutedEventHandler), typeof(Flyout));

        public event RoutedEventHandler IsOpenChanged
        {
            add { AddHandler(IsOpenChangedEvent, value); }
            remove { RemoveHandler(IsOpenChangedEvent, value); }
        }

        /// <summary>
        /// An event that is raised when the closing animation has finished.
        /// </summary>
        public static readonly RoutedEvent ClosingFinishedEvent =
            EventManager.RegisterRoutedEvent("ClosingFinished", RoutingStrategy.Bubble,
                typeof(RoutedEventHandler), typeof(Flyout));

        public event RoutedEventHandler ClosingFinished
        {
            add { AddHandler(ClosingFinishedEvent, value); }
            remove { RemoveHandler(ClosingFinishedEvent, value); }
        }

        public static readonly DependencyProperty HeaderProperty = DependencyProperty.Register("Header", typeof(string), typeof(Flyout), new PropertyMetadata(default(string)));
        public static readonly DependencyProperty PositionProperty = DependencyProperty.Register("Position", typeof(Position), typeof(Flyout), new PropertyMetadata(Position.Left, PositionChanged));
        public static readonly DependencyProperty IsPinnedProperty = DependencyProperty.Register("IsPinned", typeof(bool), typeof(Flyout), new PropertyMetadata(true));
        public static readonly DependencyProperty IsOpenProperty = DependencyProperty.Register("IsOpen", typeof(bool), typeof(Flyout), new FrameworkPropertyMetadata(default(bool), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, IsOpenedChanged));
        public static readonly DependencyProperty AnimateOnPositionChangeProperty = DependencyProperty.Register("AnimateOnPositionChange", typeof(bool), typeof(Flyout), new PropertyMetadata(true));
        public static readonly DependencyProperty AnimateOpacityProperty = DependencyProperty.Register("AnimateOpacity", typeof(bool), typeof(Flyout), new FrameworkPropertyMetadata(false, AnimateOpacityChanged));
        public static readonly DependencyProperty IsModalProperty = DependencyProperty.Register("IsModal", typeof(bool), typeof(Flyout));
        public static readonly DependencyProperty HeaderTemplateProperty = DependencyProperty.Register("HeaderTemplate", typeof(DataTemplate), typeof(Flyout));

        public static readonly DependencyProperty CloseCommandProperty = DependencyProperty.RegisterAttached("CloseCommand", typeof(ICommand), typeof(Flyout), new UIPropertyMetadata(null));
        public static readonly DependencyProperty CloseCommandParameterProperty = DependencyProperty.Register("CloseCommandParameter", typeof(object), typeof(Flyout), new PropertyMetadata(null));

        public static readonly DependencyProperty ThemeProperty = DependencyProperty.Register("Theme", typeof(FlyoutTheme), typeof(Flyout), new FrameworkPropertyMetadata(FlyoutTheme.Dark, ThemeChanged));
        public static readonly DependencyProperty ExternalCloseButtonProperty = DependencyProperty.Register("ExternalCloseButton", typeof(MouseButton), typeof(Flyout), new PropertyMetadata(MouseButton.Left));
        public static readonly DependencyProperty CloseButtonVisibilityProperty = DependencyProperty.Register("CloseButtonVisibility", typeof(Visibility), typeof(Flyout), new FrameworkPropertyMetadata(Visibility.Visible));
        public static readonly DependencyProperty CloseButtonIsCancelProperty = DependencyProperty.Register("CloseButtonIsCancel", typeof(bool), typeof(Flyout), new FrameworkPropertyMetadata(false));
        public static readonly DependencyProperty TitleVisibilityProperty = DependencyProperty.Register("TitleVisibility", typeof(Visibility), typeof(Flyout), new FrameworkPropertyMetadata(Visibility.Visible));
        public static readonly DependencyProperty AreAnimationsEnabledProperty = DependencyProperty.Register("AreAnimationsEnabled", typeof(bool), typeof(Flyout), new PropertyMetadata(true));
        public static readonly DependencyProperty FocusedElementProperty = DependencyProperty.Register("FocusedElement", typeof(FrameworkElement), typeof(Flyout), new UIPropertyMetadata(null));
        public static readonly DependencyProperty AllowFocusElementProperty = DependencyProperty.Register("AllowFocusElement", typeof(bool), typeof(Flyout), new PropertyMetadata(true));
        public static readonly DependencyProperty IsAutoCloseEnabledProperty = DependencyProperty.Register("IsAutoCloseEnabled", typeof(bool), typeof(Flyout), new FrameworkPropertyMetadata(false, IsAutoCloseEnabledChanged));
        public static readonly DependencyProperty AutoCloseIntervalProperty = DependencyProperty.Register("AutoCloseInterval", typeof(long), typeof(Flyout), new FrameworkPropertyMetadata(5000L, AutoCloseIntervalChanged));

        internal PropertyChangeNotifier IsOpenPropertyChangeNotifier { get; set; }
        internal PropertyChangeNotifier ThemePropertyChangeNotifier { get; set; }

        public bool AreAnimationsEnabled
        {
            get { return (bool)GetValue(AreAnimationsEnabledProperty); }
            set { SetValue(AreAnimationsEnabledProperty, value); }
        }

        /// <summary>
        /// Gets/sets if the title is visible in this flyout.
        /// </summary>
        public Visibility TitleVisibility
        {
            get { return (Visibility)GetValue(TitleVisibilityProperty); }
            set { SetValue(TitleVisibilityProperty, value); }
        }

        /// <summary>
        /// Gets/sets if the close button is visible in this flyout.
        /// </summary>
        public Visibility CloseButtonVisibility
        {
            get { return (Visibility)GetValue(CloseButtonVisibilityProperty); }
            set { SetValue(CloseButtonVisibilityProperty, value); }
        }

        /// <summary>
        /// Gets/sets if the close button is a cancel button in this flyout.
        /// </summary>
        public bool CloseButtonIsCancel
        {
            get { return (bool)GetValue(CloseButtonIsCancelProperty); }
            set { SetValue(CloseButtonIsCancelProperty, value); }
        }

        /// <summary>
        /// Gets/sets a command which will be executed if the close button was clicked.
        /// Note that this won't execute when <see cref="IsOpen"/> is set to <c>false</c>.
        /// </summary>
        public ICommand CloseCommand
        {
            get { return (ICommand)GetValue(CloseCommandProperty); }
            set { SetValue(CloseCommandProperty, value); }
        }

        /// <summary>
        /// Gets/sets the command parameter which will be passed by the CloseCommand.
        /// </summary>
        public object CloseCommandParameter
        {
            get { return (object)GetValue(CloseCommandParameterProperty); }
            set { SetValue(CloseCommandParameterProperty, value); }
        }

        /// <summary>
        /// A DataTemplate for the flyout's header.
        /// </summary>
        public DataTemplate HeaderTemplate
        {
            get { return (DataTemplate)GetValue(HeaderTemplateProperty); }
            set { SetValue(HeaderTemplateProperty, value); }
        }

        /// <summary>
        /// Gets/sets whether this flyout is visible.
        /// </summary>
        public bool IsOpen
        {
            get { return (bool)GetValue(IsOpenProperty); }
            set { SetValue(IsOpenProperty, value); }
        }

        /// <summary>
        /// Gets/sets whether this flyout uses the open/close animation when changing the <see cref="Position"/> property. (default is true)
        /// </summary>
        public bool AnimateOnPositionChange
        {
            get { return (bool)GetValue(AnimateOnPositionChangeProperty); }
            set { SetValue(AnimateOnPositionChangeProperty, value); }
        }

        /// <summary>
        /// Gets/sets whether this flyout animates the opacity of the flyout when opening/closing.
        /// </summary>
        public bool AnimateOpacity
        {
            get { return (bool)GetValue(AnimateOpacityProperty); }
            set { SetValue(AnimateOpacityProperty, value); }
        }

        /// <summary>
        /// Gets/sets whether this flyout stays open when the user clicks outside of it.
        /// </summary>
        public bool IsPinned
        {
            get { return (bool)GetValue(IsPinnedProperty); }
            set { SetValue(IsPinnedProperty, value); }
        }

        /// <summary>
        /// Gets/sets the mouse button that closes the flyout on an external mouse click.
        /// </summary>
        public MouseButton ExternalCloseButton
        {
            get { return (MouseButton)GetValue(ExternalCloseButtonProperty); }
            set { SetValue(ExternalCloseButtonProperty, value); }
        }

        /// <summary>
        /// Gets/sets whether this flyout is modal.
        /// </summary>
        public bool IsModal
        {
            get { return (bool)GetValue(IsModalProperty); }
            set { SetValue(IsModalProperty, value); }
        }

        /// <summary>
        /// Gets/sets this flyout's position in the FlyoutsControl/MetroWindow.
        /// </summary>
        public Position Position
        {
            get { return (Position)GetValue(PositionProperty); }
            set { SetValue(PositionProperty, value); }
        }

        /// <summary>
        /// Gets/sets the flyout's header.
        /// </summary>
        public string Header
        {
            get { return (string)GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }

        /// <summary>
        /// Gets or sets the theme of this flyout.
        /// </summary>
        public FlyoutTheme Theme
        {
            get { return (FlyoutTheme)GetValue(ThemeProperty); }
            set { SetValue(ThemeProperty, value); }
        }

        /// <summary>
        /// Gets or sets the focused element.
        /// </summary>
        public FrameworkElement FocusedElement
        {
            get { return (FrameworkElement)this.GetValue(FocusedElementProperty); }
            set { this.SetValue(FocusedElementProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the flyout should auto close after AutoCloseInterval has passed.
        /// </summary>
        public bool IsAutoCloseEnabled
        {
            get { return (bool)this.GetValue(IsAutoCloseEnabledProperty); }
            set { this.SetValue(IsAutoCloseEnabledProperty, value); }
        }

        /// <summary>
        /// Gets or sets the time in milliseconds when the flyout should auto close.
        /// </summary>
        public long AutoCloseInterval
        {
            get { return (long)this.GetValue(AutoCloseIntervalProperty); }
            set { this.SetValue(AutoCloseIntervalProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the flyout should try focus an element.
        /// </summary>
        public bool AllowFocusElement
        {
            get { return (bool)this.GetValue(AllowFocusElementProperty); }
            set { this.SetValue(AllowFocusElementProperty, value); }
        }

        public Flyout()
        {
            this.Loaded += (sender, args) => this.UpdateFlyoutTheme();
            this.InitializeAutoCloseTimer();
        }

        private void InitializeAutoCloseTimer()
        {
            this.StopAutoCloseTimer();

            this.autoCloseTimer = new DispatcherTimer();
            this.autoCloseTimer.Tick += this.AutoCloseTimerCallback;
            this.autoCloseTimer.Interval = TimeSpan.FromMilliseconds(this.AutoCloseInterval);
        }

        private MetroWindow parentWindow;

        private MetroWindow ParentWindow => this.parentWindow ?? (this.parentWindow = this.TryFindParent<MetroWindow>());

        private void UpdateFlyoutTheme()
        {
            var flyoutsControl = this.TryFindParent<FlyoutsControl>();

            if (System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
            {
                this.Visibility = flyoutsControl != null ? Visibility.Collapsed : Visibility.Visible;
            }

            var window = this.ParentWindow;
            if (window != null)
            {
                var windowTheme = DetectTheme(this);

                if (windowTheme?.Item2 != null)
                {
                    var accent = windowTheme.Item2;
                    this.ChangeFlyoutTheme(accent, windowTheme.Item1);
                }

                // we must certain to get the right foreground for window commands and buttons
                if (flyoutsControl != null && this.IsOpen)
                {
                    flyoutsControl.HandleFlyoutStatusChange(this, window);
                }
            }
        }

        internal void ChangeFlyoutTheme(Accent windowAccent, AppTheme windowTheme)
        {
            // Beware: Über-dumb code ahead!
            switch (this.Theme)
            {
                case FlyoutTheme.Accent:
                    ThemeManager.ChangeAppStyle(this.Resources, windowAccent, windowTheme);
                    this.SetResourceReference(BackgroundProperty, "HighlightBrush");
                    this.SetResourceReference(ForegroundProperty, "IdealForegroundColorBrush");
                    break;

                case FlyoutTheme.Adapt:
                    ThemeManager.ChangeAppStyle(this.Resources, windowAccent, windowTheme);
                    break;

                case FlyoutTheme.Inverse:
                    AppTheme inverseTheme = ThemeManager.GetInverseAppTheme(windowTheme);

                    if (inverseTheme == null)
                        throw new InvalidOperationException("The inverse flyout theme only works if the window theme abides the naming convention. " +
                                                            "See ThemeManager.GetInverseAppTheme for more infos");

                    ThemeManager.ChangeAppStyle(this.Resources, windowAccent, inverseTheme);
                    break;

                case FlyoutTheme.Dark:
                    ThemeManager.ChangeAppStyle(this.Resources, windowAccent, ThemeManager.GetAppTheme("BaseDark"));
                    break;

                case FlyoutTheme.Light:
                    ThemeManager.ChangeAppStyle(this.Resources, windowAccent, ThemeManager.GetAppTheme("BaseLight"));
                    break;
            }
        }

        private static Tuple<AppTheme, Accent> DetectTheme(Flyout flyout)
        {
            if (flyout == null)
                return null;

            // first look for owner
            var window = flyout.ParentWindow;
            var theme = window != null ? ThemeManager.DetectAppStyle(window) : null;
            if (theme?.Item2 != null)
            {
                return theme;
            }

            // second try, look for main window
            if (Application.Current != null)
            {
                var mainWindow = Application.Current.MainWindow as MetroWindow;
                theme = mainWindow != null ? ThemeManager.DetectAppStyle(mainWindow) : null;
                if (theme?.Item2 != null)
                {
                    return theme;
                }

                // oh no, now look at application resource
                theme = ThemeManager.DetectAppStyle(Application.Current);
                if (theme?.Item2 != null)
                {
                    return theme;
                }
            }
            return null;
        }

        private void UpdateOpacityChange()
        {
            if (this.flyoutRoot == null || this.fadeOutFrame == null || System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
            {
                return;
            }
            if (!this.AnimateOpacity)
            {
                this.fadeOutFrame.Value = 1;
                this.flyoutRoot.Opacity = 1;
            }
            else
            {
                this.fadeOutFrame.Value = 0;
                if (!this.IsOpen) this.flyoutRoot.Opacity = 0;
            }
        }

        private static void IsOpenedChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            var flyout = (Flyout)dependencyObject;

            Action openedChangedAction = () => {
                if (e.NewValue != e.OldValue)
                {
                    if (flyout.AreAnimationsEnabled)
                    {
                        if ((bool)e.NewValue)
                        {
                            if (flyout.hideStoryboard != null)
                            {
                                // don't let the storyboard end it's completed event
                                // otherwise it could be hidden on start
                                flyout.hideStoryboard.Completed -= flyout.HideStoryboardCompleted;
                            }
                            flyout.Visibility = Visibility.Visible;
                            flyout.ApplyAnimation(flyout.Position, flyout.AnimateOpacity);
                            flyout.TryFocusElement();
                            if (flyout.IsAutoCloseEnabled)
                            {
                                flyout.StartAutoCloseTimer();
                            }
                        }
                        else
                        {
                            // focus the Flyout itself to avoid nasty FocusVisual painting (it's visible until the Flyout is closed)
                            flyout.Focus();
                            flyout.StopAutoCloseTimer();
                            if (flyout.hideStoryboard != null)
                            {
                                flyout.hideStoryboard.Completed += flyout.HideStoryboardCompleted;
                            }
                            else
                            {
                                flyout.Hide();
                            }
                        }
                        VisualStateManager.GoToState(flyout, (bool)e.NewValue == false ? "Hide" : "Show", true);
                    }
                    else
                    {
                        if ((bool)e.NewValue)
                        {
                            flyout.Visibility = Visibility.Visible;
                            flyout.TryFocusElement();
                            if (flyout.IsAutoCloseEnabled)
                            {
                                flyout.StartAutoCloseTimer();
                            }
                        }
                        else
                        {
                            // focus the Flyout itself to avoid nasty FocusVisual painting (it's visible until the Flyout is closed)
                            flyout.Focus();
                            flyout.StopAutoCloseTimer();
                            flyout.Hide();
                        }
                        VisualStateManager.GoToState(flyout, (bool)e.NewValue == false ? "HideDirect" : "ShowDirect", true);
                    }
                }

                flyout.RaiseEvent(new RoutedEventArgs(IsOpenChangedEvent));
            };

            flyout.Dispatcher.BeginInvoke(DispatcherPriority.Background, openedChangedAction);
        }

        private static void IsAutoCloseEnabledChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            var flyout = (Flyout)dependencyObject;

            Action autoCloseEnabledChangedAction = () => {
                if (e.NewValue != e.OldValue)
                {
                    if ((bool)e.NewValue)
                    {
                        if (flyout.IsOpen)
                        {
                            flyout.StartAutoCloseTimer();
                        }
                    }
                    else
                    {
                        flyout.StopAutoCloseTimer();
                    }
                }
            };

            flyout.Dispatcher.BeginInvoke(DispatcherPriority.Background, autoCloseEnabledChangedAction);
        }

        private static void AutoCloseIntervalChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            var flyout = (Flyout)dependencyObject;

            Action autoCloseIntervalChangedAction = () => { 
                if (e.NewValue != e.OldValue)
                {
                    flyout.InitializeAutoCloseTimer();
                    if (flyout.IsAutoCloseEnabled && flyout.IsOpen)
                    {
                        flyout.StartAutoCloseTimer();
                    }
                }
            };

            flyout.Dispatcher.BeginInvoke(DispatcherPriority.Background, autoCloseIntervalChangedAction);
        }

        private void StartAutoCloseTimer()
        {
            //in case it is already running
            this.StopAutoCloseTimer();
            if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
            {
                this.autoCloseTimer.Start();
            }
        }

        private void StopAutoCloseTimer()
        {
            if ((this.autoCloseTimer != null) && (this.autoCloseTimer.IsEnabled))
            {
                this.autoCloseTimer.Stop();
            }
        }

        private void AutoCloseTimerCallback(Object sender, EventArgs e)
        {
            this.StopAutoCloseTimer();

            //if the flyout is open and autoclose is still enabled then close the flyout
            if ((this.IsOpen) && (this.IsAutoCloseEnabled))
            {
                this.IsOpen = false;
            }
        }

        private void HideStoryboardCompleted(object sender, EventArgs e)
        {
            this.hideStoryboard.Completed -= this.HideStoryboardCompleted;
            this.Hide();
        }

        private void Hide()
        {
            // hide the flyout, we should get better performance and prevent showing the flyout on any resizing events
            this.Visibility = Visibility.Hidden;
            this.RaiseEvent(new RoutedEventArgs(ClosingFinishedEvent));
        }

        private void TryFocusElement()
        {
            if (this.AllowFocusElement)
            {
                // first focus itself
                this.Focus();
                
                if (this.FocusedElement != null)
                {
                    this.FocusedElement.Focus();
                }
                else if (this.flyoutContent == null || !this.flyoutContent.MoveFocus(new TraversalRequest(FocusNavigationDirection.First)))
                {
                    this.flyoutHeader?.MoveFocus(new TraversalRequest(FocusNavigationDirection.First));
                }
            }
        }

        private static void ThemeChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            var flyout = (Flyout)dependencyObject;
            flyout.UpdateFlyoutTheme();
        }

        private static void AnimateOpacityChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            var flyout = (Flyout)dependencyObject;
            flyout.UpdateOpacityChange();
        }

        private static void PositionChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            var flyout = (Flyout)dependencyObject;
            var wasOpen = flyout.IsOpen;
            if (wasOpen && flyout.AnimateOnPositionChange)
            {
                flyout.ApplyAnimation((Position)e.NewValue, flyout.AnimateOpacity);
                VisualStateManager.GoToState(flyout, "Hide", true);
            }
            else
            {
                flyout.ApplyAnimation((Position)e.NewValue, flyout.AnimateOpacity, false);
            }

            if (wasOpen && flyout.AnimateOnPositionChange)
            {
                flyout.ApplyAnimation((Position)e.NewValue, flyout.AnimateOpacity);
                VisualStateManager.GoToState(flyout, "Show", true);
            }
        }

        static Flyout()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Flyout), new FrameworkPropertyMetadata(typeof(Flyout)));
        }

        DispatcherTimer autoCloseTimer;
        Grid flyoutRoot;
        Storyboard hideStoryboard;
        SplineDoubleKeyFrame hideFrame;
        SplineDoubleKeyFrame hideFrameY;
        SplineDoubleKeyFrame showFrame;
        SplineDoubleKeyFrame showFrameY;
        SplineDoubleKeyFrame fadeOutFrame;
        ContentPresenter flyoutHeader;
        ContentPresenter flyoutContent;
        Thumb windowTitleThumb;
        Button backButton;

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            this.flyoutRoot = this.GetTemplateChild("PART_Root") as Grid;
            if (this.flyoutRoot == null)
            {
                return;
            }

            this.flyoutHeader = this.GetTemplateChild("PART_Header") as ContentPresenter;
            this.flyoutContent = this.GetTemplateChild("PART_Content") as ContentPresenter;

            this.flyoutHeader?.ApplyTemplate();
            this.backButton = this.flyoutHeader?.FindChild<Button>("PART_BackButton");
            if (this.backButton != null)
            {
                this.backButton.Click -= this.BackButtonClick;
                this.backButton.Click += this.BackButtonClick;
            }

            this.windowTitleThumb = this.GetTemplateChild("PART_WindowTitleThumb") as Thumb;
            if (this.windowTitleThumb != null)
            {
                this.windowTitleThumb.PreviewMouseLeftButtonUp -= this.WindowTitleThumbOnPreviewMouseLeftButtonUp;
                this.windowTitleThumb.DragDelta -= this.WindowTitleThumbMoveOnDragDelta;
                this.windowTitleThumb.MouseDoubleClick -= this.WindowTitleThumbChangeWindowStateOnMouseDoubleClick;
                this.windowTitleThumb.MouseRightButtonUp -= this.WindowTitleThumbSystemMenuOnMouseRightButtonUp;

                this.windowTitleThumb.PreviewMouseLeftButtonUp += this.WindowTitleThumbOnPreviewMouseLeftButtonUp;
                this.windowTitleThumb.DragDelta += this.WindowTitleThumbMoveOnDragDelta;
                this.windowTitleThumb.MouseDoubleClick += this.WindowTitleThumbChangeWindowStateOnMouseDoubleClick;
                this.windowTitleThumb.MouseRightButtonUp += this.WindowTitleThumbSystemMenuOnMouseRightButtonUp;
            }

            this.hideStoryboard = this.GetTemplateChild("HideStoryboard") as Storyboard;
            this.hideFrame = this.GetTemplateChild("hideFrame") as SplineDoubleKeyFrame;
            this.hideFrameY = this.GetTemplateChild("hideFrameY") as SplineDoubleKeyFrame;
            this.showFrame = this.GetTemplateChild("showFrame") as SplineDoubleKeyFrame;
            this.showFrameY = this.GetTemplateChild("showFrameY") as SplineDoubleKeyFrame;
            this.fadeOutFrame = this.GetTemplateChild("fadeOutFrame") as SplineDoubleKeyFrame;

            if (this.hideFrame == null || this.showFrame == null || this.hideFrameY == null || this.showFrameY == null || this.fadeOutFrame == null)
            {
                return;
            }

            this.ApplyAnimation(this.Position, this.AnimateOpacity);
        }

        protected internal void CleanUp(FlyoutsControl flyoutsControl)
        {
            if (this.backButton != null)
            {
                this.backButton.Click -= this.BackButtonClick;
            }
            if (this.windowTitleThumb != null)
            {
                this.windowTitleThumb.PreviewMouseLeftButtonUp -= this.WindowTitleThumbOnPreviewMouseLeftButtonUp;
                this.windowTitleThumb.DragDelta -= this.WindowTitleThumbMoveOnDragDelta;
                this.windowTitleThumb.MouseDoubleClick -= this.WindowTitleThumbChangeWindowStateOnMouseDoubleClick;
                this.windowTitleThumb.MouseRightButtonUp -= this.WindowTitleThumbSystemMenuOnMouseRightButtonUp;
            }
            this.parentWindow = null;
        }

        private void BackButtonClick(object sender, RoutedEventArgs e)
        {
            // close the Flyout only if there is no command
            var closeCommand = this.CloseCommand;
            if (closeCommand == null)
            {
                this.IsOpen = false;
            }
        }

        private void WindowTitleThumbOnPreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var window = this.ParentWindow;
            if (window != null && this.Position != Position.Bottom)
            {
                MetroWindow.DoWindowTitleThumbOnPreviewMouseLeftButtonUp(window, e);
            }
        }

        private void WindowTitleThumbMoveOnDragDelta(object sender, DragDeltaEventArgs dragDeltaEventArgs)
        {
            var window = this.ParentWindow;
            if (window != null && this.Position != Position.Bottom)
            {
                MetroWindow.DoWindowTitleThumbMoveOnDragDelta((Thumb)sender, window, dragDeltaEventArgs);
            }
        }

        private void WindowTitleThumbChangeWindowStateOnMouseDoubleClick(object sender, MouseButtonEventArgs mouseButtonEventArgs)
        {
            var window = this.ParentWindow;
            if (window != null && this.Position != Position.Bottom)
            {
                MetroWindow.DoWindowTitleThumbChangeWindowStateOnMouseDoubleClick(window, mouseButtonEventArgs);
            }
        }

        private void WindowTitleThumbSystemMenuOnMouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            var window = this.ParentWindow;
            if (window != null && this.Position != Position.Bottom)
            {
                MetroWindow.DoWindowTitleThumbSystemMenuOnMouseRightButtonUp(window, e);
            }
        }

        internal void ApplyAnimation(Position position, bool animateOpacity, bool resetShowFrame = true)
        {
            if (this.flyoutRoot == null || this.hideFrame == null || this.showFrame == null || this.hideFrameY == null || this.showFrameY == null || this.fadeOutFrame == null)
                return;

            if (this.Position == Position.Left || this.Position == Position.Right)
                this.showFrame.Value = 0;
            if (this.Position == Position.Top || this.Position == Position.Bottom)
                this.showFrameY.Value = 0;

            // I mean, we don't need this anymore, because we use ActualWidth and ActualHeight of the flyoutRoot
            //this.flyoutRoot.Measure(new Size(Double.PositiveInfinity, Double.PositiveInfinity));

            if (!animateOpacity)
            {
                this.fadeOutFrame.Value = 1;
                this.flyoutRoot.Opacity = 1;
            }
            else
            {
                this.fadeOutFrame.Value = 0;
                if (!this.IsOpen) this.flyoutRoot.Opacity = 0;
            }

            switch (position)
            {
                default:
                    this.HorizontalAlignment = HorizontalAlignment.Left;
                    this.VerticalAlignment = VerticalAlignment.Stretch;
                    this.hideFrame.Value = -this.flyoutRoot.ActualWidth;
                    if (resetShowFrame)
                        this.flyoutRoot.RenderTransform = new TranslateTransform(-this.flyoutRoot.ActualWidth, 0);
                    break;
                case Position.Right:
                    this.HorizontalAlignment = HorizontalAlignment.Right;
                    this.VerticalAlignment = VerticalAlignment.Stretch;
                    this.hideFrame.Value = this.flyoutRoot.ActualWidth;
                    if (resetShowFrame)
                        this.flyoutRoot.RenderTransform = new TranslateTransform(this.flyoutRoot.ActualWidth, 0);
                    break;
                case Position.Top:
                    this.HorizontalAlignment = HorizontalAlignment.Stretch;
                    this.VerticalAlignment = VerticalAlignment.Top;
                    this.hideFrameY.Value = -this.flyoutRoot.ActualHeight - 1;
                    if (resetShowFrame)
                        this.flyoutRoot.RenderTransform = new TranslateTransform(0, -this.flyoutRoot.ActualHeight - 1);
                    break;
                case Position.Bottom:
                    this.HorizontalAlignment = HorizontalAlignment.Stretch;
                    this.VerticalAlignment = VerticalAlignment.Bottom;
                    this.hideFrameY.Value = this.flyoutRoot.ActualHeight;
                    if (resetShowFrame)
                        this.flyoutRoot.RenderTransform = new TranslateTransform(0, this.flyoutRoot.ActualHeight);
                    break;
            }
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);

            if (!this.IsOpen) return; // no changes for invisible flyouts, ApplyAnimation is called now in visible changed event
            if (!sizeInfo.WidthChanged && !sizeInfo.HeightChanged) return;
            if (this.flyoutRoot == null || this.hideFrame == null || this.showFrame == null || this.hideFrameY == null || this.showFrameY == null)
                return; // don't bother checking IsOpen and calling ApplyAnimation

            if (this.Position == Position.Left || this.Position == Position.Right)
                this.showFrame.Value = 0;
            if (this.Position == Position.Top || this.Position == Position.Bottom)
                this.showFrameY.Value = 0;

            switch (this.Position)
            {
                default:
                    this.hideFrame.Value = -this.flyoutRoot.ActualWidth;
                    break;
                case Position.Right:
                    this.hideFrame.Value = this.flyoutRoot.ActualWidth;
                    break;
                case Position.Top:
                    this.hideFrameY.Value = -this.flyoutRoot.ActualHeight - 1;
                    break;
                case Position.Bottom:
                    this.hideFrameY.Value = this.flyoutRoot.ActualHeight;
                    break;
            }
        }
    }
}
