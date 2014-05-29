using System;
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
        public static readonly DependencyProperty IsPinnedProperty = DependencyProperty.Register("IsPinned", typeof(bool), typeof(Flyout), new PropertyMetadata(true));
        public static readonly DependencyProperty IsOpenProperty = DependencyProperty.Register("IsOpen", typeof(bool), typeof(Flyout), new FrameworkPropertyMetadata(default(bool), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, IsOpenedChanged));
        public static readonly DependencyProperty AnimateOnPositionChangeProperty = DependencyProperty.Register("AnimateOnPositionChange", typeof(bool), typeof(Flyout), new PropertyMetadata(true));
        public static readonly DependencyProperty IsModalProperty = DependencyProperty.Register("IsModal", typeof(bool), typeof(Flyout));
        public static readonly DependencyProperty HeaderTemplateProperty = DependencyProperty.Register("HeaderTemplate", typeof(DataTemplate), typeof(Flyout));
        public static readonly DependencyProperty CloseCommandProperty = DependencyProperty.RegisterAttached("CloseCommand", typeof(ICommand), typeof(Flyout), new UIPropertyMetadata(null));
        public static readonly DependencyProperty ThemeProperty = DependencyProperty.Register("Theme", typeof(FlyoutTheme), typeof(Flyout), new FrameworkPropertyMetadata(FlyoutTheme.Dark, ThemeChanged));
        public static readonly DependencyProperty ExternalCloseButtonProperty = DependencyProperty.Register("ExternalCloseButton", typeof(MouseButton), typeof(Flyout), new PropertyMetadata(MouseButton.Left));

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
        /// Gets/sets whether this flyout uses the open/close animation when changing the <see cref="Position"/> property. (default is true)
        /// </summary>
        public bool AnimateOnPositionChange
        {
            get { return (bool)GetValue(AnimateOnPositionChangeProperty); }
            set { SetValue(AnimateOnPositionChangeProperty, value); }
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
            get { return (MouseButton) GetValue(ExternalCloseButtonProperty); }
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

        public Flyout()
        {
            this.Loaded += (sender, args) => UpdateFlyoutTheme();
        }

        private void UpdateFlyoutTheme()
        {
            if (System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
            {
                this.Visibility = this.TryFindParent<FlyoutsControl>() != null ? Visibility.Collapsed : Visibility.Visible;
            }

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

                    if(inverseTheme == null)
                        throw new InvalidOperationException("The inverse flyout theme only works if the window theme abides the naming convention. " +
                                                            "See ThemeManager.GetInverseAppTheme for more infos");

                    ThemeManager.ChangeAppStyle(this.Resources, windowAccent, inverseTheme);
                    break;
                
                case FlyoutTheme.Dark: {
                    ThemeManager.ChangeAppStyle(this.Resources, windowAccent, ThemeManager.GetAppTheme("BaseDark"));
                    break;
                }

                case FlyoutTheme.Light: {
                    ThemeManager.ChangeAppStyle(this.Resources, windowAccent, ThemeManager.GetAppTheme("BaseLight"));
                    break;
                }
            }
        }

        private static Tuple<AppTheme, Accent> DetectTheme(Flyout flyout)
        {
            if (flyout == null)
                return null;

            // first look for owner
            var window = flyout.TryFindParent<MetroWindow>();
            var theme = window != null ? ThemeManager.DetectAppStyle(window) : null;
            if (theme != null && theme.Item2 != null)
                return theme;

            // second try, look for main window
            if (Application.Current != null) {
                var mainWindow = Application.Current.MainWindow as MetroWindow;
                theme = mainWindow != null ? ThemeManager.DetectAppStyle(mainWindow) : null;
                if (theme != null && theme.Item2 != null)
                    return theme;

                // oh no, now look at application resource
                theme = ThemeManager.DetectAppStyle(Application.Current);
                if (theme != null && theme.Item2 != null)
                    return theme;
            }
            return null;
        }

        private static void IsOpenedChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            var flyout = (Flyout)dependencyObject;

            if (e.NewValue != e.OldValue)
            {
                if ((bool)e.NewValue)
                {
                    if (flyout.hideStoryboard != null)
                    {
                        // don't set visibility to hidden on show :-)
                        flyout.hideStoryboard.Completed -= flyout.HideStoryboard_Completed;
                    }
                    flyout.Visibility = Visibility.Visible;
                    flyout.ApplyAnimation(flyout.Position);
                }
                else
                {
                    if (flyout.hideStoryboard != null)
                    {
                        // after finished hide story board set the visibility to hidden
                        flyout.hideStoryboard.Completed += flyout.HideStoryboard_Completed;
                    }
                }

                VisualStateManager.GoToState(flyout, (bool)e.NewValue == false ? "Hide" : "Show", true);
            }

            var eh = flyout.IsOpenChanged;
            if (eh != null)
            {
                eh(flyout, EventArgs.Empty);
            }
        }

        private void HideStoryboard_Completed(object sender, EventArgs e)
        {
            // hide the flyout, we should get better performance and prevent showing the flyout on any resizing events
            this.Visibility = Visibility.Hidden;
        }

        private static void ThemeChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            var flyout = (Flyout) dependencyObject;
            flyout.UpdateFlyoutTheme();
        }

        private static void PositionChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            var flyout = (Flyout) dependencyObject;
            var wasOpen = flyout.IsOpen;
            if (wasOpen && flyout.AnimateOnPositionChange)
            {
                flyout.ApplyAnimation((Position)e.NewValue);
                VisualStateManager.GoToState(flyout, "Hide", true);
            }
            else
            {
                flyout.ApplyAnimation((Position)e.NewValue, false);
            }

            if (wasOpen && flyout.AnimateOnPositionChange)
            {
                flyout.ApplyAnimation((Position)e.NewValue);
                VisualStateManager.GoToState(flyout, "Show", true);
            }
        }

        static Flyout()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Flyout), new FrameworkPropertyMetadata(typeof(Flyout)));
        }

        Grid root;
        Storyboard hideStoryboard;
        EasingDoubleKeyFrame hideFrame;
        EasingDoubleKeyFrame hideFrameY;
        EasingDoubleKeyFrame showFrame;
        EasingDoubleKeyFrame showFrameY;

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            
            root = (Grid)GetTemplateChild("root");
            if (root == null)
                return;

            hideStoryboard = (Storyboard)GetTemplateChild("HideStoryboard");
            hideFrame = (EasingDoubleKeyFrame)GetTemplateChild("hideFrame");
            hideFrameY = (EasingDoubleKeyFrame)GetTemplateChild("hideFrameY");
            showFrame = (EasingDoubleKeyFrame)GetTemplateChild("showFrame");
            showFrameY = (EasingDoubleKeyFrame)GetTemplateChild("showFrameY");

            if (hideFrame == null || showFrame == null || hideFrameY == null || showFrameY == null)
                return;
            
            ApplyAnimation(Position);
        }

        internal void ApplyAnimation(Position position, bool resetShowFrame = true)
        {
            if (root == null || hideFrame == null || showFrame == null || hideFrameY == null || showFrameY == null)
                return;

            if (Position == Position.Left || Position == Position.Right)
                showFrame.Value = 0;
            if (Position == Position.Top || Position == Position.Bottom)
                showFrameY.Value = 0;

            // I mean, we don't need this anymore, because we use ActualWidth and ActualHeight of the root
            //root.Measure(new Size(Double.PositiveInfinity, Double.PositiveInfinity));

            switch (position)
            {
                default:
                    HorizontalAlignment = HorizontalAlignment.Left;
                    VerticalAlignment = VerticalAlignment.Stretch;
                    hideFrame.Value = -root.ActualWidth;
                    if (resetShowFrame)
                        root.RenderTransform = new TranslateTransform(-root.ActualWidth, 0);
                    break;
                case Position.Right:
                    HorizontalAlignment = HorizontalAlignment.Right;
                    VerticalAlignment = VerticalAlignment.Stretch;
                    hideFrame.Value = root.ActualWidth;
                    if (resetShowFrame)
                        root.RenderTransform = new TranslateTransform(root.ActualWidth, 0);
                    break;
                case Position.Top:
                    HorizontalAlignment = HorizontalAlignment.Stretch;
                    VerticalAlignment = VerticalAlignment.Top;
                    hideFrameY.Value = -root.ActualHeight - 1;
                    if (resetShowFrame)
                        root.RenderTransform = new TranslateTransform(0, -root.ActualHeight - 1);
                    break;
                case Position.Bottom:
                    HorizontalAlignment = HorizontalAlignment.Stretch;
                    VerticalAlignment = VerticalAlignment.Bottom;
                    hideFrameY.Value = root.ActualHeight;
                    if (resetShowFrame)
                        root.RenderTransform = new TranslateTransform(0, root.ActualHeight);
                    break;
            }
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);

            if (!IsOpen) return; // no changes for invisible flyouts, ApplyAnimation is called now in visible changed event
            if (!sizeInfo.WidthChanged && !sizeInfo.HeightChanged) return;
            if (root == null || hideFrame == null || showFrame == null || hideFrameY == null || showFrameY == null)
                return; // don't bother checking IsOpen and calling ApplyAnimation

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
