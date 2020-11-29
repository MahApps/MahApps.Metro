// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using ControlzEx;
using ControlzEx.Theming;
using MahApps.Metro.Automation.Peers;
using MahApps.Metro.ValueBoxes;

namespace MahApps.Metro.Controls
{
    /// <summary>
    /// A sliding panel control that is hosted in a <see cref="MetroWindow"/> via a <see cref="FlyoutsControl"/>.
    /// </summary>
    [TemplatePart(Name = "PART_Root", Type = typeof(FrameworkElement))]
    [TemplatePart(Name = "PART_Header", Type = typeof(FrameworkElement))]
    [TemplatePart(Name = "PART_Content", Type = typeof(FrameworkElement))]
    public class Flyout : HeaderedContentControl
    {
        public static readonly RoutedEvent IsOpenChangedEvent =
            EventManager.RegisterRoutedEvent(nameof(IsOpenChanged),
                                             RoutingStrategy.Bubble,
                                             typeof(RoutedEventHandler),
                                             typeof(Flyout));

        /// <summary>
        /// An event that is raised when <see cref="IsOpen"/> property changes.
        /// </summary>
        public event RoutedEventHandler IsOpenChanged
        {
            add => this.AddHandler(IsOpenChangedEvent, value);
            remove => this.RemoveHandler(IsOpenChangedEvent, value);
        }

        public static readonly RoutedEvent OpeningFinishedEvent =
            EventManager.RegisterRoutedEvent(nameof(OpeningFinished),
                                             RoutingStrategy.Bubble,
                                             typeof(RoutedEventHandler),
                                             typeof(Flyout));

        /// <summary>
        /// An event that is raised when the opening animation has finished.
        /// </summary>
        public event RoutedEventHandler OpeningFinished
        {
            add => this.AddHandler(OpeningFinishedEvent, value);
            remove => this.RemoveHandler(OpeningFinishedEvent, value);
        }

        public static readonly RoutedEvent ClosingFinishedEvent =
            EventManager.RegisterRoutedEvent(nameof(ClosingFinished),
                                             RoutingStrategy.Bubble,
                                             typeof(RoutedEventHandler),
                                             typeof(Flyout));

        /// <summary>
        /// An event that is raised when the closing animation has finished.
        /// </summary>
        public event RoutedEventHandler ClosingFinished
        {
            add => this.AddHandler(ClosingFinishedEvent, value);
            remove => this.RemoveHandler(ClosingFinishedEvent, value);
        }

        public static readonly DependencyProperty PositionProperty =
            DependencyProperty.Register(nameof(Position),
                                        typeof(Position),
                                        typeof(Flyout),
                                        new PropertyMetadata(Position.Left, OnPositionPropertyChanged));

        private static void OnPositionPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
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

        /// <summary>
        /// Gets or sets the position of this <see cref="Flyout"/> inside the <see cref="FlyoutsControl"/>.
        /// </summary>
        public Position Position
        {
            get => (Position)this.GetValue(PositionProperty);
            set => this.SetValue(PositionProperty, value);
        }

        public static readonly DependencyProperty IsPinnedProperty
            = DependencyProperty.Register(nameof(IsPinned),
                                          typeof(bool),
                                          typeof(Flyout),
                                          new PropertyMetadata(BooleanBoxes.TrueBox));

        /// <summary>
        /// Gets or sets whether this <see cref="Flyout"/> stays open when the user clicks somewhere outside of it.
        /// </summary>
        public bool IsPinned
        {
            get => (bool)this.GetValue(IsPinnedProperty);
            set => this.SetValue(IsPinnedProperty, BooleanBoxes.Box(value));
        }

        public static readonly DependencyProperty IsOpenProperty
            = DependencyProperty.Register(nameof(IsOpen),
                                          typeof(bool),
                                          typeof(Flyout),
                                          new FrameworkPropertyMetadata(BooleanBoxes.FalseBox, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnIsOpenPropertyChanged));

        private static void OnIsOpenPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            var flyout = (Flyout)dependencyObject;

            Action openedChangedAction = () =>
                {
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
                                if (flyout.showStoryboard != null)
                                {
                                    flyout.showStoryboard.Completed += flyout.ShowStoryboardCompleted;
                                }
                                else
                                {
                                    flyout.Shown();
                                }

                                if (flyout.IsAutoCloseEnabled)
                                {
                                    flyout.StartAutoCloseTimer();
                                }
                            }
                            else
                            {
                                if (flyout.showStoryboard != null)
                                {
                                    flyout.showStoryboard.Completed -= flyout.ShowStoryboardCompleted;
                                }

                                flyout.StopAutoCloseTimer();
                                flyout.SetValue(IsShownPropertyKey, BooleanBoxes.FalseBox);
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
                                flyout.Shown();
                                if (flyout.IsAutoCloseEnabled)
                                {
                                    flyout.StartAutoCloseTimer();
                                }
                            }
                            else
                            {
                                flyout.StopAutoCloseTimer();
                                flyout.SetValue(IsShownPropertyKey, BooleanBoxes.FalseBox);
                                flyout.Hide();
                            }

                            VisualStateManager.GoToState(flyout, (bool)e.NewValue == false ? "HideDirect" : "ShowDirect", true);
                        }
                    }

                    flyout.RaiseEvent(new RoutedEventArgs(IsOpenChangedEvent));
                };

            flyout.Dispatcher.BeginInvoke(DispatcherPriority.Background, openedChangedAction);
        }

        /// <summary>
        /// Gets or sets whether this <see cref="Flyout"/> should be visible or not.
        /// </summary>
        public bool IsOpen
        {
            get => (bool)this.GetValue(IsOpenProperty);
            set => this.SetValue(IsOpenProperty, BooleanBoxes.Box(value));
        }

        /// <summary>Identifies the <see cref="IsShown"/> dependency property.</summary>
        private static readonly DependencyPropertyKey IsShownPropertyKey
            = DependencyProperty.RegisterReadOnly(nameof(IsShown),
                                                  typeof(bool),
                                                  typeof(Flyout),
                                                  new PropertyMetadata(BooleanBoxes.FalseBox));

        /// <summary>Identifies the <see cref="IsShown"/> dependency property.</summary>
        public static readonly DependencyProperty IsShownProperty = IsShownPropertyKey.DependencyProperty;

        /// <summary>
        /// Gets whether the <see cref="Flyout"/> is completely shown (after <see cref="IsOpen"/> was set to true).
        /// </summary>
        public bool IsShown
        {
            get => (bool)this.GetValue(IsShownProperty);
            protected set => this.SetValue(IsShownPropertyKey, BooleanBoxes.Box(value));
        }

        public static readonly DependencyProperty AnimateOnPositionChangeProperty
            = DependencyProperty.Register(nameof(AnimateOnPositionChange),
                                          typeof(bool),
                                          typeof(Flyout),
                                          new PropertyMetadata(BooleanBoxes.TrueBox));

        /// <summary>
        /// Gets or sets whether this <see cref="Flyout"/> uses the open/close animation when changing the <see cref="Position"/> property (default is true).
        /// </summary>
        public bool AnimateOnPositionChange
        {
            get => (bool)this.GetValue(AnimateOnPositionChangeProperty);
            set => this.SetValue(AnimateOnPositionChangeProperty, BooleanBoxes.Box(value));
        }

        public static readonly DependencyProperty AnimateOpacityProperty
            = DependencyProperty.Register(nameof(AnimateOpacity),
                                          typeof(bool),
                                          typeof(Flyout),
                                          new FrameworkPropertyMetadata(BooleanBoxes.FalseBox, (d, args) => (d as Flyout)?.UpdateOpacityChange()));

        /// <summary>
        /// Gets or sets whether this <see cref="Flyout"/> animates the opacity when opening/closing the <see cref="Flyout"/>.
        /// </summary>
        public bool AnimateOpacity
        {
            get => (bool)this.GetValue(AnimateOpacityProperty);
            set => this.SetValue(AnimateOpacityProperty, BooleanBoxes.Box(value));
        }

        public static readonly DependencyProperty IsModalProperty
            = DependencyProperty.Register(nameof(IsModal),
                                          typeof(bool),
                                          typeof(Flyout),
                                          new PropertyMetadata(BooleanBoxes.FalseBox));

        /// <summary>
        /// Gets or sets whether this <see cref="Flyout"/> is modal.
        /// </summary>
        public bool IsModal
        {
            get => (bool)this.GetValue(IsModalProperty);
            set => this.SetValue(IsModalProperty, BooleanBoxes.Box(value));
        }

        public static readonly DependencyProperty CloseCommandProperty
            = DependencyProperty.RegisterAttached(nameof(CloseCommand),
                                                  typeof(ICommand),
                                                  typeof(Flyout),
                                                  new UIPropertyMetadata(null));

        /// <summary>
        /// Gets or sets a <see cref="ICommand"/> which will be executed if the close button was clicked.
        /// </summary>
        /// <remarks>
        /// The <see cref="ICommand"/> won't be executed when <see cref="IsOpen"/> property will be set to false/true.
        /// </remarks>
        public ICommand CloseCommand
        {
            get => (ICommand)this.GetValue(CloseCommandProperty);
            set => this.SetValue(CloseCommandProperty, value);
        }

        public static readonly DependencyProperty CloseCommandParameterProperty
            = DependencyProperty.Register(nameof(CloseCommandParameter),
                                          typeof(object),
                                          typeof(Flyout),
                                          new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the parameter for the <see cref="CloseCommand"/>.
        /// </summary>
        public object CloseCommandParameter
        {
            get => (object)this.GetValue(CloseCommandParameterProperty);
            set => this.SetValue(CloseCommandParameterProperty, value);
        }

        public static readonly DependencyProperty ThemeProperty
            = DependencyProperty.Register(nameof(Theme),
                                          typeof(FlyoutTheme),
                                          typeof(Flyout),
                                          new FrameworkPropertyMetadata(FlyoutTheme.Dark, (d, args) => (d as Flyout)?.UpdateFlyoutTheme()));

        /// <summary>
        /// Gets or sets the theme for the <see cref="Flyout"/>.
        /// </summary>
        public FlyoutTheme Theme
        {
            get => (FlyoutTheme)this.GetValue(ThemeProperty);
            set => this.SetValue(ThemeProperty, value);
        }

        public static readonly DependencyProperty ExternalCloseButtonProperty
            = DependencyProperty.Register(nameof(ExternalCloseButton),
                                          typeof(MouseButton),
                                          typeof(Flyout),
                                          new PropertyMetadata(MouseButton.Left));

        /// <summary>
        /// Gets or sets the mouse button that closes the <see cref="Flyout"/> when the user clicks somewhere outside of it.
        /// </summary>
        public MouseButton ExternalCloseButton
        {
            get => (MouseButton)this.GetValue(ExternalCloseButtonProperty);
            set => this.SetValue(ExternalCloseButtonProperty, value);
        }

        public static readonly DependencyProperty CloseButtonVisibilityProperty
            = DependencyProperty.Register(nameof(CloseButtonVisibility),
                                          typeof(Visibility),
                                          typeof(Flyout),
                                          new FrameworkPropertyMetadata(Visibility.Visible));

        /// <summary>
        /// Gets or sets the visibility of the close button for this <see cref="Flyout"/>.
        /// </summary>
        public Visibility CloseButtonVisibility
        {
            get => (Visibility)this.GetValue(CloseButtonVisibilityProperty);
            set => this.SetValue(CloseButtonVisibilityProperty, value);
        }

        public static readonly DependencyProperty CloseButtonIsCancelProperty
            = DependencyProperty.Register(nameof(CloseButtonIsCancel),
                                          typeof(bool),
                                          typeof(Flyout),
                                          new PropertyMetadata(BooleanBoxes.FalseBox));

        /// <summary>
        /// Gets or sets a value that indicates whether the close button is a Cancel button. A user can activate the Cancel button by pressing the ESC key.
        /// </summary>
        public bool CloseButtonIsCancel
        {
            get => (bool)this.GetValue(CloseButtonIsCancelProperty);
            set => this.SetValue(CloseButtonIsCancelProperty, BooleanBoxes.Box(value));
        }

        public static readonly DependencyProperty TitleVisibilityProperty
            = DependencyProperty.Register(nameof(TitleVisibility),
                                          typeof(Visibility),
                                          typeof(Flyout),
                                          new FrameworkPropertyMetadata(Visibility.Visible));

        /// <summary>
        /// Gets or sets the visibility of the title.
        /// </summary>
        public Visibility TitleVisibility
        {
            get => (Visibility)this.GetValue(TitleVisibilityProperty);
            set => this.SetValue(TitleVisibilityProperty, value);
        }

        public static readonly DependencyProperty AreAnimationsEnabledProperty
            = DependencyProperty.Register(nameof(AreAnimationsEnabled),
                                          typeof(bool),
                                          typeof(Flyout),
                                          new PropertyMetadata(BooleanBoxes.TrueBox));

        /// <summary>
        /// Gets or sets a value that indicates whether the <see cref="Flyout"/> uses animations for open/close.
        /// </summary>
        public bool AreAnimationsEnabled
        {
            get => (bool)this.GetValue(AreAnimationsEnabledProperty);
            set => this.SetValue(AreAnimationsEnabledProperty, BooleanBoxes.Box(value));
        }

        public static readonly DependencyProperty FocusedElementProperty
            = DependencyProperty.Register(nameof(FocusedElement),
                                          typeof(FrameworkElement),
                                          typeof(Flyout),
                                          new UIPropertyMetadata(null));

        /// <summary>
        /// Gets or sets the focused element.
        /// </summary>
        public FrameworkElement FocusedElement
        {
            get => (FrameworkElement)this.GetValue(FocusedElementProperty);
            set => this.SetValue(FocusedElementProperty, value);
        }

        public static readonly DependencyProperty AllowFocusElementProperty
            = DependencyProperty.Register(nameof(AllowFocusElement),
                                          typeof(bool),
                                          typeof(Flyout),
                                          new PropertyMetadata(BooleanBoxes.TrueBox));

        /// <summary>
        /// Gets or sets a value indicating whether the <see cref="Flyout"/> should try focus an element.
        /// </summary>
        public bool AllowFocusElement
        {
            get => (bool)this.GetValue(AllowFocusElementProperty);
            set => this.SetValue(AllowFocusElementProperty, BooleanBoxes.Box(value));
        }

        public static readonly DependencyProperty IsAutoCloseEnabledProperty
            = DependencyProperty.Register(nameof(IsAutoCloseEnabled),
                                          typeof(bool),
                                          typeof(Flyout),
                                          new FrameworkPropertyMetadata(BooleanBoxes.FalseBox, OnIsAutoCloseEnabledPropertyChanged));

        private static void OnIsAutoCloseEnabledPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            var flyout = (Flyout)dependencyObject;

            Action autoCloseEnabledChangedAction = () =>
                {
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

        /// <summary>
        /// Gets or sets a value indicating whether the <see cref="Flyout"/> should auto close after the <see cref="AutoCloseInterval"/> has passed.
        /// </summary>
        public bool IsAutoCloseEnabled
        {
            get => (bool)this.GetValue(IsAutoCloseEnabledProperty);
            set => this.SetValue(IsAutoCloseEnabledProperty, BooleanBoxes.Box(value));
        }

        public static readonly DependencyProperty AutoCloseIntervalProperty
            = DependencyProperty.Register(nameof(AutoCloseInterval),
                                          typeof(long),
                                          typeof(Flyout),
                                          new FrameworkPropertyMetadata(5000L, AutoCloseIntervalChanged));

        private static void AutoCloseIntervalChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            var flyout = (Flyout)dependencyObject;

            Action autoCloseIntervalChangedAction = () =>
                {
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

        /// <summary>
        /// Gets or sets the time in milliseconds when the <see cref="Flyout"/> should auto close.
        /// </summary>
        public long AutoCloseInterval
        {
            get => (long)this.GetValue(AutoCloseIntervalProperty);
            set => this.SetValue(AutoCloseIntervalProperty, value);
        }

        /// <summary>Identifies the <see cref="Owner"/> dependency property.</summary>
        private static readonly DependencyPropertyKey OwnerPropertyKey =
            DependencyProperty.RegisterReadOnly(nameof(Owner),
                                                typeof(FlyoutsControl),
                                                typeof(Flyout),
                                                new PropertyMetadata(null));

        /// <summary>Identifies the <see cref="Owner"/> dependency property.</summary>
        public static readonly DependencyProperty OwnerProperty = OwnerPropertyKey.DependencyProperty;

        public FlyoutsControl Owner
        {
            get => (FlyoutsControl)this.GetValue(OwnerProperty);
            protected set => this.SetValue(OwnerPropertyKey, value);
        }

        private DispatcherTimer autoCloseTimer;
        private FrameworkElement flyoutRoot;
        private Storyboard showStoryboard;
        private Storyboard hideStoryboard;
        private SplineDoubleKeyFrame hideFrame;
        private SplineDoubleKeyFrame hideFrameY;
        private SplineDoubleKeyFrame showFrame;
        private SplineDoubleKeyFrame showFrameY;
        private SplineDoubleKeyFrame fadeOutFrame;
        private FrameworkElement flyoutHeader;
        private FrameworkElement flyoutContent;
        private MetroWindow parentWindow;

        private MetroWindow ParentWindow => this.parentWindow ??= this.TryFindParent<MetroWindow>();

        /// <summary>
        /// <see cref="IsOpen"/> property changed notifier used in <see cref="FlyoutsControl"/>.
        /// </summary>
        internal PropertyChangeNotifier IsOpenPropertyChangeNotifier { get; set; }

        /// <summary>
        /// <see cref="Theme"/> property changed notifier used in <see cref="FlyoutsControl"/>.
        /// </summary>
        internal PropertyChangeNotifier ThemePropertyChangeNotifier { get; set; }

        static Flyout()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Flyout), new FrameworkPropertyMetadata(typeof(Flyout)));
        }

        public Flyout()
        {
            this.Loaded += (sender, args) => this.UpdateFlyoutTheme();
            this.InitializeAutoCloseTimer();
        }

        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new FlyoutAutomationPeer(this);
        }

        private void InitializeAutoCloseTimer()
        {
            this.StopAutoCloseTimer();

            this.autoCloseTimer = new DispatcherTimer();
            this.autoCloseTimer.Tick += this.AutoCloseTimerCallback;
            this.autoCloseTimer.Interval = TimeSpan.FromMilliseconds(this.AutoCloseInterval);
        }

        private void StartAutoCloseTimer()
        {
            // in case it is already running
            this.StopAutoCloseTimer();

            if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
            {
                this.autoCloseTimer.Start();
            }
        }

        private void StopAutoCloseTimer()
        {
            if (this.autoCloseTimer != null && this.autoCloseTimer.IsEnabled)
            {
                this.autoCloseTimer.Stop();
            }
        }

        private void AutoCloseTimerCallback(object sender, EventArgs e)
        {
            this.StopAutoCloseTimer();

            // if the flyout is open and auto close is still enabled then close the flyout
            if (this.IsOpen && this.IsAutoCloseEnabled)
            {
                this.SetCurrentValue(IsOpenProperty, BooleanBoxes.FalseBox);
            }
        }

        private void UpdateFlyoutTheme()
        {
            var flyoutsControl = this.Owner ?? this.TryFindParent<FlyoutsControl>();

            if (System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
            {
                this.SetCurrentValue(VisibilityProperty, flyoutsControl != null ? Visibility.Collapsed : Visibility.Visible);
            }

            var window = this.ParentWindow;
            if (window != null)
            {
                var windowTheme = DetectTheme(this);

                if (windowTheme != null)
                {
                    this.ChangeFlyoutTheme(windowTheme);
                }

                // we must certain to get the right foreground for window commands and buttons
                if (flyoutsControl != null && this.IsOpen)
                {
                    flyoutsControl.HandleFlyoutStatusChange(this, window);
                }
            }
        }

        private static ControlzEx.Theming.Theme DetectTheme(Flyout flyout)
        {
            if (flyout is null)
            {
                return null;
            }

            // first look for owner
            var window = flyout.ParentWindow;
            var theme = window != null ? ThemeManager.Current.DetectTheme(window) : null;
            if (theme != null)
            {
                return theme;
            }

            // second try, look for main window and then for current application
            if (Application.Current != null)
            {
                theme = Application.Current.MainWindow is null
                    ? ThemeManager.Current.DetectTheme(Application.Current)
                    : ThemeManager.Current.DetectTheme(Application.Current.MainWindow);
                if (theme != null)
                {
                    return theme;
                }
            }

            return null;
        }

        internal void ChangeFlyoutTheme(ControlzEx.Theming.Theme windowTheme)
        {
            switch (this.Theme)
            {
                case FlyoutTheme.Accent:
                    ThemeManager.Current.ApplyThemeResourcesFromTheme(this.Resources, windowTheme);
                    this.OverrideFlyoutResources(this.Resources, true);
                    break;

                case FlyoutTheme.Adapt:
                    ThemeManager.Current.ApplyThemeResourcesFromTheme(this.Resources, windowTheme);
                    this.OverrideFlyoutResources(this.Resources);
                    break;

                case FlyoutTheme.Inverse:
                    var inverseTheme = ThemeManager.Current.GetInverseTheme(windowTheme);
                    if (inverseTheme is null)
                    {
                        throw new InvalidOperationException("The inverse Flyout theme only works if the window theme abides the naming convention. " +
                                                            "See ThemeManager.GetInverseAppTheme for more infos");
                    }

                    ThemeManager.Current.ApplyThemeResourcesFromTheme(this.Resources, inverseTheme);
                    this.OverrideFlyoutResources(this.Resources);
                    break;

                case FlyoutTheme.Dark:
                    var darkTheme = windowTheme.BaseColorScheme == ThemeManager.BaseColorDark ? windowTheme : ThemeManager.Current.GetInverseTheme(windowTheme);
                    if (darkTheme is null)
                    {
                        throw new InvalidOperationException("The Dark Flyout theme only works if the window theme abides the naming convention. " +
                                                            "See ThemeManager.GetInverseAppTheme for more infos");
                    }

                    ThemeManager.Current.ApplyThemeResourcesFromTheme(this.Resources, darkTheme);
                    this.OverrideFlyoutResources(this.Resources);
                    break;

                case FlyoutTheme.Light:
                    var lightTheme = windowTheme.BaseColorScheme == ThemeManager.BaseColorLight ? windowTheme : ThemeManager.Current.GetInverseTheme(windowTheme);
                    if (lightTheme is null)
                    {
                        throw new InvalidOperationException("The Light Flyout theme only works if the window theme abides the naming convention. " +
                                                            "See ThemeManager.GetInverseAppTheme for more infos");
                    }

                    ThemeManager.Current.ApplyThemeResourcesFromTheme(this.Resources, lightTheme);
                    this.OverrideFlyoutResources(this.Resources);
                    break;
            }
        }

        protected virtual void OverrideFlyoutResources(ResourceDictionary resources, bool accent = false)
        {
            var fromColorKey = accent ? "MahApps.Colors.Highlight" : "MahApps.Colors.Flyout";

            resources.BeginInit();

            var fromColor = (Color)resources[fromColorKey];
            resources["MahApps.Colors.ThemeBackground"] = fromColor;
            resources["MahApps.Colors.Flyout"] = fromColor;

            var newBrush = new SolidColorBrush(fromColor);
            newBrush.Freeze();
            resources["MahApps.Brushes.Flyout.Background"] = newBrush;
            resources["MahApps.Brushes.Control.Background"] = newBrush;
            resources["MahApps.Brushes.ThemeBackground"] = newBrush;
            resources["MahApps.Brushes.Window.Background"] = newBrush;
            resources[SystemColors.WindowBrushKey] = newBrush;

            if (accent)
            {
                fromColor = (Color)resources["MahApps.Colors.IdealForeground"];
                newBrush = new SolidColorBrush(fromColor);
                newBrush.Freeze();
                resources["MahApps.Brushes.Flyout.Foreground"] = newBrush;
                resources["MahApps.Brushes.Text"] = newBrush;

                if (resources.Contains("MahApps.Colors.AccentBase"))
                {
                    fromColor = (Color)resources["MahApps.Colors.AccentBase"];
                }
                else
                {
                    var accentColor = (Color)resources["MahApps.Colors.Accent"];
                    fromColor = Color.FromArgb(255, accentColor.R, accentColor.G, accentColor.B);
                }

                newBrush = new SolidColorBrush(fromColor);
                newBrush.Freeze();
                resources["MahApps.Colors.Highlight"] = fromColor;
                resources["MahApps.Brushes.Highlight"] = newBrush;
            }

            resources.EndInit();
        }

        private void UpdateOpacityChange()
        {
            if (this.flyoutRoot is null || this.fadeOutFrame is null || System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
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
                if (!this.IsOpen)
                {
                    this.flyoutRoot.Opacity = 0;
                }
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

        private void ShowStoryboardCompleted(object sender, EventArgs e)
        {
            this.showStoryboard.Completed -= this.ShowStoryboardCompleted;
            this.Shown();
        }

        private void Shown()
        {
            this.SetValue(IsShownPropertyKey, BooleanBoxes.TrueBox);
            this.RaiseEvent(new RoutedEventArgs(OpeningFinishedEvent));
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
                else if (this.flyoutContent is null || !this.flyoutContent.MoveFocus(new TraversalRequest(FocusNavigationDirection.First)))
                {
                    this.flyoutHeader?.MoveFocus(new TraversalRequest(FocusNavigationDirection.First));
                }
            }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            var flyoutsControl = ItemsControl.ItemsControlFromItemContainer(this) as FlyoutsControl ?? this.TryFindParent<FlyoutsControl>();
            this.SetValue(OwnerPropertyKey, flyoutsControl);

            this.flyoutRoot = this.GetTemplateChild("PART_Root") as FrameworkElement;
            if (this.flyoutRoot is null)
            {
                return;
            }

            this.flyoutHeader = this.GetTemplateChild("PART_Header") as FrameworkElement;
            this.flyoutHeader?.ApplyTemplate();

            this.flyoutContent = this.GetTemplateChild("PART_Content") as FrameworkElement;

            if (this.flyoutHeader is IMetroThumb thumb)
            {
                thumb.PreviewMouseLeftButtonUp -= this.HeaderThumbOnPreviewMouseLeftButtonUp;
                thumb.DragDelta -= this.HeaderThumbMoveOnDragDelta;
                thumb.MouseDoubleClick -= this.HeaderThumbChangeWindowStateOnMouseDoubleClick;
                thumb.MouseRightButtonUp -= this.HeaderThumbSystemMenuOnMouseRightButtonUp;

                thumb.PreviewMouseLeftButtonUp += this.HeaderThumbOnPreviewMouseLeftButtonUp;
                thumb.DragDelta += this.HeaderThumbMoveOnDragDelta;
                thumb.MouseDoubleClick += this.HeaderThumbChangeWindowStateOnMouseDoubleClick;
                thumb.MouseRightButtonUp += this.HeaderThumbSystemMenuOnMouseRightButtonUp;
            }

#pragma warning disable WPF0130 // Add [TemplatePart] to the type.
            this.showStoryboard = this.GetTemplateChild("ShowStoryboard") as Storyboard;
            this.hideStoryboard = this.GetTemplateChild("HideStoryboard") as Storyboard;
            this.hideFrame = this.GetTemplateChild("hideFrame") as SplineDoubleKeyFrame;
            this.hideFrameY = this.GetTemplateChild("hideFrameY") as SplineDoubleKeyFrame;
            this.showFrame = this.GetTemplateChild("showFrame") as SplineDoubleKeyFrame;
            this.showFrameY = this.GetTemplateChild("showFrameY") as SplineDoubleKeyFrame;
            this.fadeOutFrame = this.GetTemplateChild("fadeOutFrame") as SplineDoubleKeyFrame;
#pragma warning restore WPF0130 // Add [TemplatePart] to the type.

            if (this.hideFrame is null || this.showFrame is null || this.hideFrameY is null || this.showFrameY is null || this.fadeOutFrame is null)
            {
                return;
            }

            this.ApplyAnimation(this.Position, this.AnimateOpacity);
        }

        internal void CleanUp()
        {
            if (this.flyoutHeader is IMetroThumb thumb)
            {
                thumb.PreviewMouseLeftButtonUp -= this.HeaderThumbOnPreviewMouseLeftButtonUp;
                thumb.DragDelta -= this.HeaderThumbMoveOnDragDelta;
                thumb.MouseDoubleClick -= this.HeaderThumbChangeWindowStateOnMouseDoubleClick;
                thumb.MouseRightButtonUp -= this.HeaderThumbSystemMenuOnMouseRightButtonUp;
            }

            this.parentWindow = null;
        }

        private void HeaderThumbOnPreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var window = this.ParentWindow;
            if (window != null && this.Position != Position.Bottom)
            {
                MetroWindow.DoWindowTitleThumbOnPreviewMouseLeftButtonUp(window, e);
            }
        }

        private void HeaderThumbMoveOnDragDelta(object sender, DragDeltaEventArgs dragDeltaEventArgs)
        {
            var window = this.ParentWindow;
            if (window != null && this.Position != Position.Bottom)
            {
                MetroWindow.DoWindowTitleThumbMoveOnDragDelta(sender as IMetroThumb, window, dragDeltaEventArgs);
            }
        }

        private void HeaderThumbChangeWindowStateOnMouseDoubleClick(object sender, MouseButtonEventArgs mouseButtonEventArgs)
        {
            var window = this.ParentWindow;
            if (window != null && this.Position != Position.Bottom && Mouse.GetPosition((IInputElement)sender).Y <= window.TitleBarHeight && window.TitleBarHeight > 0)
            {
                MetroWindow.DoWindowTitleThumbChangeWindowStateOnMouseDoubleClick(window, mouseButtonEventArgs);
            }
        }

        private void HeaderThumbSystemMenuOnMouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            var window = this.ParentWindow;
            if (window != null && this.Position != Position.Bottom && Mouse.GetPosition((IInputElement)sender).Y <= window.TitleBarHeight && window.TitleBarHeight > 0)
            {
                MetroWindow.DoWindowTitleThumbSystemMenuOnMouseRightButtonUp(window, e);
            }
        }

        internal void ApplyAnimation(Position position, bool animateOpacity, bool resetShowFrame = true)
        {
            if (this.flyoutRoot is null || this.hideFrame is null || this.showFrame is null || this.hideFrameY is null || this.showFrameY is null || this.fadeOutFrame is null)
            {
                return;
            }

            if (this.Position == Position.Left || this.Position == Position.Right)
            {
                this.showFrame.Value = 0;
            }

            if (this.Position == Position.Top || this.Position == Position.Bottom)
            {
                this.showFrameY.Value = 0;
            }

            if (!animateOpacity)
            {
                this.fadeOutFrame.Value = 1;
                this.flyoutRoot.Opacity = 1;
            }
            else
            {
                this.fadeOutFrame.Value = 0;
                if (!this.IsOpen)
                {
                    this.flyoutRoot.Opacity = 0;
                }
            }

            switch (position)
            {
                default:
                    this.HorizontalAlignment = this.Margin.Right <= 0 ? this.HorizontalContentAlignment != HorizontalAlignment.Stretch ? HorizontalAlignment.Left : this.HorizontalContentAlignment : HorizontalAlignment.Stretch;
                    this.VerticalAlignment = VerticalAlignment.Stretch;
                    this.hideFrame.Value = -this.flyoutRoot.ActualWidth - this.Margin.Left;
                    if (resetShowFrame)
                    {
                        this.flyoutRoot.RenderTransform = new TranslateTransform(-this.flyoutRoot.ActualWidth, 0);
                    }

                    break;
                case Position.Right:
                    this.HorizontalAlignment = this.Margin.Left <= 0 ? this.HorizontalContentAlignment != HorizontalAlignment.Stretch ? HorizontalAlignment.Right : this.HorizontalContentAlignment : HorizontalAlignment.Stretch;
                    this.VerticalAlignment = VerticalAlignment.Stretch;
                    this.hideFrame.Value = this.flyoutRoot.ActualWidth + this.Margin.Right;
                    if (resetShowFrame)
                    {
                        this.flyoutRoot.RenderTransform = new TranslateTransform(this.flyoutRoot.ActualWidth, 0);
                    }

                    break;
                case Position.Top:
                    this.HorizontalAlignment = HorizontalAlignment.Stretch;
                    this.VerticalAlignment = this.Margin.Bottom <= 0 ? this.VerticalContentAlignment != VerticalAlignment.Stretch ? VerticalAlignment.Top : this.VerticalContentAlignment : VerticalAlignment.Stretch;
                    this.hideFrameY.Value = -this.flyoutRoot.ActualHeight - 1 - this.Margin.Top;
                    if (resetShowFrame)
                    {
                        this.flyoutRoot.RenderTransform = new TranslateTransform(0, -this.flyoutRoot.ActualHeight - 1);
                    }

                    break;
                case Position.Bottom:
                    this.HorizontalAlignment = HorizontalAlignment.Stretch;
                    this.VerticalAlignment = this.Margin.Top <= 0 ? this.VerticalContentAlignment != VerticalAlignment.Stretch ? VerticalAlignment.Bottom : this.VerticalContentAlignment : VerticalAlignment.Stretch;
                    this.hideFrameY.Value = this.flyoutRoot.ActualHeight + this.Margin.Bottom;
                    if (resetShowFrame)
                    {
                        this.flyoutRoot.RenderTransform = new TranslateTransform(0, this.flyoutRoot.ActualHeight);
                    }

                    break;
            }
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);

            if (!this.IsOpen)
            {
                return; // no changes for invisible flyouts, ApplyAnimation is called now in visible changed event
            }

            if (!sizeInfo.WidthChanged && !sizeInfo.HeightChanged)
            {
                return;
            }

            if (this.flyoutRoot is null || this.hideFrame is null || this.showFrame is null || this.hideFrameY is null || this.showFrameY is null)
            {
                return; // don't bother checking IsOpen and calling ApplyAnimation
            }

            if (this.Position == Position.Left || this.Position == Position.Right)
            {
                this.showFrame.Value = 0;
            }

            if (this.Position == Position.Top || this.Position == Position.Bottom)
            {
                this.showFrameY.Value = 0;
            }

            switch (this.Position)
            {
                default:
                    this.hideFrame.Value = -this.flyoutRoot.ActualWidth - this.Margin.Left;
                    break;
                case Position.Right:
                    this.hideFrame.Value = this.flyoutRoot.ActualWidth + this.Margin.Right;
                    break;
                case Position.Top:
                    this.hideFrameY.Value = -this.flyoutRoot.ActualHeight - 1 - this.Margin.Top;
                    break;
                case Position.Bottom:
                    this.hideFrameY.Value = this.flyoutRoot.ActualHeight + this.Margin.Bottom;
                    break;
            }
        }
    }
}