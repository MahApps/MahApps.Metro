using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace MahApps.Metro.Controls
{
    [TemplatePart(Name = "PART_BackButton", Type = typeof(Button))]
    [TemplatePart(Name = "PART_Header", Type = typeof(ContentPresenter))]
    public class Flyout : ContentControl
    {
        
        public event EventHandler IsOpenChanged;
        
        public static readonly DependencyProperty HeaderProperty = DependencyProperty.Register("Header", typeof(string), typeof(Flyout), new PropertyMetadata(default(string)));
        public static readonly DependencyProperty PositionProperty = DependencyProperty.Register("Position", typeof(Position), typeof(Flyout), new PropertyMetadata(Position.Left, PositionChanged));
        public static readonly DependencyProperty IsPinnableProperty = DependencyProperty.Register("IsPinnable", typeof(bool), typeof(Flyout), new PropertyMetadata(default(bool)));
        public static readonly DependencyProperty IsOpenProperty = DependencyProperty.Register("IsOpen", typeof(bool), typeof(Flyout), new FrameworkPropertyMetadata(default(bool), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, IsOpenedChanged));
        public static readonly DependencyProperty HeaderTemplateProperty = DependencyProperty.Register("HeaderTemplate", typeof(DataTemplate), typeof(Flyout));
        public static readonly DependencyProperty CloseCommandProperty = DependencyProperty.RegisterAttached("CloseCommand", typeof(ICommand), typeof(Flyout), new UIPropertyMetadata(null));

        public ICommand CloseCommand
        {
            get { return (ICommand)GetValue(CloseCommandProperty); }
            set { SetValue(CloseCommandProperty, value); }
        }

        public DataTemplate HeaderTemplate
        {
            get { return (DataTemplate)GetValue(HeaderTemplateProperty); }
            set { SetValue(HeaderTemplateProperty, value); }
        }

        public bool IsOpen
        {
            get { return (bool)GetValue(IsOpenProperty); }
            set { SetValue(IsOpenProperty, value); }
        }

        public bool IsPinnable
        {
            get { return (bool)GetValue(IsPinnableProperty); }
            set { SetValue(IsPinnableProperty, value); }
        }

        public Position Position
        {
            get { return (Position)GetValue(PositionProperty); }
            set { SetValue(PositionProperty, value); }
        }

        public string Header
        {
            get { return (string)GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }

        private static void IsOpenedChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            var flyout = (Flyout)dependencyObject;

            if ((bool)e.NewValue)
            {
                var window = flyout.TryFindParent<MetroWindow>();
                if (window != null)
                {
                    var theme = ThemeManager.DetectTheme(window);
                    if (theme != null && theme.Item2 != null)
                    {
                        var accent = theme.Item2;
                        flyout.CheckForMainResourceDictionaries();
                        ThemeManager.ChangeTheme(flyout.Resources, accent, Theme.Dark);
                    }
                }
            }

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

        private bool resDictAlreadyChecked;

        private void CheckForMainResourceDictionaries()
        {
            if (resDictAlreadyChecked)
                return;

            this.Resources.BeginInit();
            try {
                var idx = -1;
                foreach (var rd in ThemeManager.MainResourceDictionaries)
                {
                    var md = this.Resources.MergedDictionaries.FirstOrDefault(d => d.Source == rd.Source);
                    if (md != null)
                        this.Resources.MergedDictionaries.Remove(md);
                    this.Resources.MergedDictionaries.Insert(++idx, rd);
                }
            }
            finally
            {
                this.Resources.EndInit();
                resDictAlreadyChecked = true;
            }
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
                    hideFrame.Value = -root.DesiredSize.Width;
                    root.RenderTransform = new TranslateTransform(-root.DesiredSize.Width, 0);
                    break;
                case Position.Right:
                    HorizontalAlignment = HorizontalAlignment.Right;
                    VerticalAlignment = VerticalAlignment.Stretch;
                    hideFrame.Value = root.DesiredSize.Width;
                    root.RenderTransform = new TranslateTransform(root.DesiredSize.Width, 0);
                    break;
                case Position.Top:
                    HorizontalAlignment = HorizontalAlignment.Stretch;
                    VerticalAlignment = VerticalAlignment.Top;
                    hideFrameY.Value = -root.DesiredSize.Height;
                    root.RenderTransform = new TranslateTransform(0, -root.DesiredSize.Height);
                    break;
                case Position.Bottom:
                    HorizontalAlignment = HorizontalAlignment.Stretch;
                    VerticalAlignment = VerticalAlignment.Bottom;
                    hideFrameY.Value = root.DesiredSize.Height;
                    root.RenderTransform = new TranslateTransform(0, root.DesiredSize.Height);
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
                    hideFrame.Value = -root.DesiredSize.Width;
                    break;
                case Position.Right:
                    hideFrame.Value = root.DesiredSize.Width;
                    break;
                case Position.Top:
                    hideFrameY.Value = -root.DesiredSize.Height;
                    break;
                case Position.Bottom:
                    hideFrameY.Value = root.DesiredSize.Height;
                    break;
            }
        }
    }
}
