﻿using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace MahApps.Metro.Controls
{
    /// <summary>
    /// A sliding panel control that is hosted in a MetroWindow via a FlyoutsControl.
    /// <see cref="MetroWindow"/>
    /// <seealso cref="FlyoutsControl"/>
    /// </summary>
    [TemplatePart(Name = "PART_BackButton", Type = typeof(Button))]
    [TemplatePart(Name = "PART_Header", Type = typeof(ContentPresenter))]
    public class Flyout : ContentControl
    {
        
        /// <summary>
        /// An event that is raised when IsOpen changes.
        /// </summary>
        public event EventHandler IsOpenChanged;
        
        public static readonly DependencyProperty HeaderProperty = DependencyProperty.Register("Header", typeof(string), typeof(Flyout), new PropertyMetadata(default(string)));
        public static readonly DependencyProperty PositionProperty = DependencyProperty.Register("Position", typeof(Position), typeof(Flyout), new PropertyMetadata(Position.Left, PositionChanged));
        public static readonly DependencyProperty IsPinnableProperty = DependencyProperty.Register("IsPinnable", typeof(bool), typeof(Flyout), new PropertyMetadata(default(bool)));
        public static readonly DependencyProperty IsOpenProperty = DependencyProperty.Register("IsOpen", typeof(bool), typeof(Flyout), new FrameworkPropertyMetadata(default(bool), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, IsOpenedChanged));
        public static readonly DependencyProperty IsModalProperty = DependencyProperty.Register("IsModal", typeof(bool), typeof(Flyout));
        public static readonly DependencyProperty HeaderTemplateProperty = DependencyProperty.Register("HeaderTemplate", typeof(DataTemplate), typeof(Flyout));
        public static readonly DependencyProperty CloseCommandProperty = DependencyProperty.RegisterAttached("CloseCommand", typeof(ICommand), typeof(Flyout), new UIPropertyMetadata(null));
        public static readonly DependencyProperty ThemeProperty = DependencyProperty.Register("Theme", typeof(FlyoutTheme), typeof(Flyout), new PropertyMetadata(FlyoutTheme.Dark));

        /// <summary>
        /// Gets the actual theme (dark/light) of this flyout.
        /// Used to handle the WindowCommands overlay in the MetroWindow.
        /// </summary>
        internal Theme ActualTheme { get; private set; }

        /// <summary>
        /// An ICommand that executes when the flyout's close button is clicked.
        /// </summary>
        public ICommand CloseCommand
        {
            get { return (ICommand)GetValue(CloseCommandProperty); }
            set { SetValue(CloseCommandProperty, value); }
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
        /// Gets/sets whether this flyout is pinnable.
        /// </summary>
        public bool IsPinnable
        {
            get { return (bool)GetValue(IsPinnableProperty); }
            set { SetValue(IsPinnableProperty, value); }
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

        public Flyout()
        {
            this.Loaded += (sender, args) => 
            {
                var window = this.TryFindParent<MetroWindow>();
                if (window != null)
                {
                    var windowTheme = DetectTheme(this);

                    if (windowTheme != null && windowTheme.Item2 != null)
                    {
                        var accent = windowTheme.Item2;

                        this.ChangeFlyoutTheme(accent, windowTheme.Item1);
                    }
                }

                ThemeManager.IsThemeChanged += this.ThemeManager_IsThemeChanged;
            };
            this.Unloaded += (sender, args) => ThemeManager.IsThemeChanged -= this.ThemeManager_IsThemeChanged;
        }

        private void ChangeFlyoutTheme(Accent windowAccent, Theme windowTheme)
        {
            // Beware: Über-dumb code ahead!
            switch (this.Theme)
            {
                case FlyoutTheme.Accent:
                    ThemeManager.ChangeTheme(this.Resources, windowAccent, windowTheme);
                    this.SetResourceReference(BackgroundProperty, "HighlightBrush");
                    this.SetResourceReference(ForegroundProperty, "IdealForegroundColorBrush");
                    this.ActualTheme = windowTheme;
                break;

                case FlyoutTheme.Adapt:
                    ThemeManager.ChangeTheme(this.Resources, windowAccent, windowTheme);
                    switch (windowTheme)
                    {
                        case Metro.Theme.Dark:
                            this.SetResourceReference(ForegroundProperty, "BlackColorBrush");
                            this.SetResourceReference(BackgroundProperty, "FlyoutDarkBrush");
                            break;

                        case Metro.Theme.Light:
                            this.SetResourceReference(ForegroundProperty, "BlackColorBrush");
                            this.SetResourceReference(BackgroundProperty, "FlyoutLightBrush");
                            break;
                    }
                    this.ActualTheme = windowTheme;
                    break;

                case FlyoutTheme.Inverse:
                    switch (windowTheme)
                    {
                        case Metro.Theme.Dark:
                            ThemeManager.ChangeTheme(this.Resources, windowAccent, Metro.Theme.Light);
                            this.Background = (Brush) ThemeManager.DarkResource["FlyoutLightBrush"];
                            this.Foreground = (Brush) ThemeManager.DarkResource["WhiteColorBrush"];
                            this.ActualTheme = Metro.Theme.Light;
                            break;

                        case Metro.Theme.Light:
                            ThemeManager.ChangeTheme(this.Resources, windowAccent, Metro.Theme.Dark);
                            this.Background = (Brush) ThemeManager.LightResource["FlyoutDarkBrush"];
                            this.Foreground = (Brush)ThemeManager.LightResource["WhiteColorBrush"];
                            this.ActualTheme = Metro.Theme.Dark;
                            break;
                    }
                    break;
                
                case FlyoutTheme.Dark:
                    ThemeManager.ChangeTheme(this.Resources, windowAccent, Metro.Theme.Dark);
                    this.SetResourceReference(BackgroundProperty, "FlyoutDarkBrush");
                    this.SetResourceReference(ForegroundProperty, "BlackColorBrush");
                    this.ActualTheme = Metro.Theme.Dark;
                    break;

                case FlyoutTheme.Light:
                    ThemeManager.ChangeTheme(this.Resources, windowAccent, Metro.Theme.Light);
                    this.SetResourceReference(BackgroundProperty, "FlyoutLightBrush");
                    this.SetResourceReference(ForegroundProperty, "BlackColorBrush");
                    this.ActualTheme = Metro.Theme.Light;
                    break;
            }
        }

        private void ThemeManager_IsThemeChanged(object sender, OnThemeChangedEventArgs e)
        {
            if (e.Accent != null)
            {
                this.ChangeFlyoutTheme(e.Accent, e.Theme);
            }
        }

        private static Tuple<Theme, Accent> DetectTheme(Flyout flyout)
        {
            if (flyout == null)
                return null;

            // first look for owner
            var window = flyout.TryFindParent<MetroWindow>();
            var theme = window != null ? ThemeManager.DetectTheme(window) : null;
            if (theme != null && theme.Item2 != null)
                return theme;

            // second try, look for main window
            if (Application.Current != null) {
                var mainWindow = Application.Current.MainWindow as MetroWindow;
                theme = mainWindow != null ? ThemeManager.DetectTheme(mainWindow) : null;
                if (theme != null && theme.Item2 != null)
                    return theme;

                // oh no, now look at application resource
                theme = ThemeManager.DetectTheme(Application.Current);
                if (theme != null && theme.Item2 != null)
                    return theme;
            }
            return null;
        }

        private static void IsOpenedChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            var flyout = (Flyout)dependencyObject;

            VisualStateManager.GoToState(flyout, (bool) e.NewValue == false ? "Hide" : "Show", true);
            if (flyout.IsOpenChanged != null)
            {
                flyout.IsOpenChanged(flyout, EventArgs.Empty);
            }
        }

        private static void PositionChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            var flyout = (Flyout) dependencyObject;
            flyout.ApplyAnimation((Position)e.NewValue);
        }

        static Flyout()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Flyout), new FrameworkPropertyMetadata(typeof(Flyout)));
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            ApplyAnimation(Position);
        }

        internal void ApplyAnimation(Position position)
        {
            var root = (Grid)GetTemplateChild("root");
            if (root == null)
                return;

            var hideFrame = (EasingDoubleKeyFrame)GetTemplateChild("hideFrame");
            var hideFrameY = (EasingDoubleKeyFrame)GetTemplateChild("hideFrameY");
            var showFrame = (EasingDoubleKeyFrame)GetTemplateChild("showFrame");
            var showFrameY = (EasingDoubleKeyFrame)GetTemplateChild("showFrameY");

            if (hideFrame == null || showFrame == null || hideFrameY == null || showFrameY == null)
                return;

            if (Position == Position.Left || Position == Position.Right)
                showFrame.Value = 0;
            if (Position == Position.Top || Position == Position.Bottom)
                showFrameY.Value = 0;
            root.Measure(new Size(Double.PositiveInfinity, Double.PositiveInfinity));

            switch (position)
            {
                default:
                    HorizontalAlignment = HorizontalAlignment.Left;
                    VerticalAlignment = VerticalAlignment.Stretch;
                    hideFrame.Value = -root.ActualWidth;
                    root.RenderTransform = new TranslateTransform(-root.ActualWidth, 0);
                    break;
                case Position.Right:
                    HorizontalAlignment = HorizontalAlignment.Right;
                    VerticalAlignment = VerticalAlignment.Stretch;
                    hideFrame.Value = root.ActualWidth;
                    root.RenderTransform = new TranslateTransform(root.ActualWidth, 0);
                    break;
                case Position.Top:
                    HorizontalAlignment = HorizontalAlignment.Stretch;
                    VerticalAlignment = VerticalAlignment.Top;
                    hideFrameY.Value = -root.ActualHeight - 1;
                    root.RenderTransform = new TranslateTransform(0, -root.ActualHeight - 1);
                    break;
                case Position.Bottom:
                    HorizontalAlignment = HorizontalAlignment.Stretch;
                    VerticalAlignment = VerticalAlignment.Bottom;
                    hideFrameY.Value = root.ActualHeight;
                    root.RenderTransform = new TranslateTransform(0, root.ActualHeight);
                    break;
            }
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);

            if (!sizeInfo.WidthChanged && !sizeInfo.HeightChanged) return;

            if (!IsOpen)
            {
                ApplyAnimation(Position);
                return;
            }

            var root = (Grid)GetTemplateChild("root");
            if (root == null)
                return;

            var hideFrame = (EasingDoubleKeyFrame)GetTemplateChild("hideFrame");
            var hideFrameY = (EasingDoubleKeyFrame)GetTemplateChild("hideFrameY");
            var showFrame = (EasingDoubleKeyFrame)GetTemplateChild("showFrame");
            var showFrameY = (EasingDoubleKeyFrame)GetTemplateChild("showFrameY");

            if (hideFrame == null || showFrame == null || hideFrameY == null || showFrameY == null)
                return;

            if (Position == Position.Left || Position == Position.Right)
                showFrame.Value = 0;
            if (Position == Position.Top || Position == Position.Bottom) 
                showFrameY.Value = 0;

            switch (Position)
            {
                default:
                    hideFrame.Value = -root.ActualWidth;
                    break;
                case Position.Right:
                    hideFrame.Value = root.ActualWidth;
                    break;
                case Position.Top:
                    hideFrameY.Value = -root.ActualHeight - 1;
                    break;
                case Position.Bottom:
                    hideFrameY.Value = root.ActualHeight;
                    break;
            }
        }
    }
}
