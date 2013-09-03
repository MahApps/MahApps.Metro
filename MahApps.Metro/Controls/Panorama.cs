using System;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Threading;
using System.ComponentModel;
using System.Windows.Input;
using System.Windows.Media;

namespace MahApps.Metro.Controls
{
    [TemplatePart(Name = "PART_ScrollViewer", Type = typeof(ScrollViewer))]
    public class Panorama : ItemsControl
    {
        public static readonly DependencyProperty ItemBoxProperty = DependencyProperty.Register("ItemHeight", typeof(double), typeof(Panorama), new FrameworkPropertyMetadata(120.0));
        public static readonly DependencyProperty GroupHeightProperty = DependencyProperty.Register("GroupHeight", typeof(double), typeof(Panorama), new FrameworkPropertyMetadata(640.0));
        public static readonly DependencyProperty HeaderFontSizeProperty = DependencyProperty.Register("HeaderFontSize", typeof(double), typeof(Panorama), new FrameworkPropertyMetadata(40.0));
        public static readonly DependencyProperty HeaderFontColorProperty = DependencyProperty.Register("HeaderFontColor", typeof(Brush), typeof(Panorama), new FrameworkPropertyMetadata(Brushes.White));
        public static readonly DependencyProperty HeaderFontFamilyProperty = DependencyProperty.Register("HeaderFontFamily", typeof(FontFamily), typeof(Panorama), new FrameworkPropertyMetadata(new FontFamily("Segoe UI Light")));
        public static readonly DependencyProperty UseSnapBackScrollingProperty = DependencyProperty.Register("UseSnapBackScrolling", typeof(bool), typeof(Panorama), new FrameworkPropertyMetadata(true));
        public static readonly DependencyProperty MouseScrollEnabledProperty = DependencyProperty.Register("MouseScrollEnabled", typeof(bool), typeof(Panorama), new FrameworkPropertyMetadata(true));
        public static readonly DependencyProperty HorizontalScrollBarEnabledProperty = DependencyProperty.Register("HorizontalScrollBarEnabled", typeof(bool), typeof(Panorama), new FrameworkPropertyMetadata(true));
        public static readonly DependencyProperty DynamicGroupHeaderProperty = DependencyProperty.Register("DynamicGroupHeader", typeof(bool), typeof(Panorama), new FrameworkPropertyMetadata(true));

        public double Friction
        {
            get { return 1.0 - friction; }
            set { friction = Math.Min(Math.Max(1.0 - value, 0), 1.0); }
        }

        public double ItemBox
        {
            get { return (double)GetValue(ItemBoxProperty); }
            set { SetValue(ItemBoxProperty, value); }
        }

        public double GroupHeight
        {
            get { return (double)GetValue(GroupHeightProperty); }
            set { SetValue(GroupHeightProperty, value); }
        }

        public double HeaderFontSize
        {
            get { return (double)GetValue(HeaderFontSizeProperty); }
            set { SetValue(HeaderFontSizeProperty, value); }
        }

        public Brush HeaderFontColor
        {
            get { return (Brush)GetValue(HeaderFontColorProperty); }
            set { SetValue(HeaderFontColorProperty, value); }
        }

        public FontFamily HeaderFontFamily
        {
            get { return (FontFamily)GetValue(HeaderFontFamilyProperty); }
            set { SetValue(HeaderFontFamilyProperty, value); }
        }

        public bool UseSnapBackScrolling
        {
            get { return (bool)GetValue(UseSnapBackScrollingProperty); }
            set { SetValue(UseSnapBackScrollingProperty, value); }
        }

        public bool DynamicGroupHeader
        {
            get { return (bool)GetValue(DynamicGroupHeaderProperty); }
            set { SetValue(DynamicGroupHeaderProperty, value); }
        }

        public bool MouseScrollEnabled
        {
            get { return (bool)GetValue(MouseScrollEnabledProperty); }
            set { SetValue(MouseScrollEnabledProperty, value); }
        }

        public bool HorizontalScrollBarEnabled
        {
            get { return (bool)GetValue(HorizontalScrollBarEnabledProperty); }
            set { SetValue(HorizontalScrollBarEnabledProperty, value); }
        }

        private ScrollViewer sv;
        private Point scrollTarget;
        private Point scrollStartPoint;
        private Point scrollStartOffset;
        private Point previousPoint;
        private Vector velocity;
        private double friction;
        private readonly DispatcherTimer animationTimer = new DispatcherTimer(DispatcherPriority.DataBind);
        private readonly DispatcherTimer scrollBarTimer = new DispatcherTimer(DispatcherPriority.DataBind);
        private static int PixelsToMoveToBeConsideredScroll = 5;
        private static int PixelsToMoveToBeConsideredClick = 2;
        private IPanoramaTile tile;
        private bool touchCaptured;
        private Point lastTouchPosition;

        public Panorama()
        {
            friction = 0.85;

            animationTimer.Interval = new TimeSpan(0, 0, 0, 0, 20);
            animationTimer.Tick += HandleWorldTimerTick;

            scrollBarTimer.Interval = TimeSpan.FromSeconds(1);
            scrollBarTimer.Tick += (s, e) => HideHorizontalScrollBar();

            this.Loaded += (sender, e) => animationTimer.Start();
            this.Unloaded += (sender, e) => animationTimer.Stop();

            lastTouchPosition = new Point();
        }

        static Panorama()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Panorama), new FrameworkPropertyMetadata(typeof(Panorama)));
        }

        private void DoStandardScrolling()
        {
            sv.ScrollToHorizontalOffset(scrollTarget.X);
            sv.ScrollToVerticalOffset(scrollTarget.Y);
            scrollTarget.X += velocity.X;
            scrollTarget.Y += velocity.Y;
            velocity *= friction;
        }

        private void HandleWorldTimerTick(object sender, EventArgs e)
        {
            if (sv == null)
                return;
            var prop = DesignerProperties.IsInDesignModeProperty;
            var isInDesignMode = (bool)DependencyPropertyDescriptor.FromProperty(prop, typeof(FrameworkElement)).Metadata.DefaultValue;

            if (isInDesignMode)
                return;

            if (IsMouseCaptured || touchCaptured)
            {
                Point currentPoint = IsMouseCaptured ? Mouse.GetPosition(this) : lastTouchPosition;

                velocity = previousPoint - currentPoint;
                previousPoint = currentPoint;
            }
            else
            {
                if (velocity.Length > 1)
                {
                    DoStandardScrolling();
                }
                else
                {
                    if (UseSnapBackScrolling)
                    {
                        int mx = (int)sv.HorizontalOffset % (int)ActualWidth;
                        if (mx == 0)
                            return;
                        int ix = (int)sv.HorizontalOffset / (int)ActualWidth;
                        double snapBackX = mx > ActualWidth / 2 ? (ix + 1) * ActualWidth : ix * ActualWidth;
                        sv.ScrollToHorizontalOffset(sv.HorizontalOffset + (snapBackX - sv.HorizontalOffset) / 4.0);
                    }
                    else
                    {
                        DoStandardScrolling();
                    }
                }
            }
        }

        private void HideHorizontalScrollBar()
        {
            // Ignore when scroll happen with mouse drag or not to be viewed
            if (!HorizontalScrollBarEnabled || Mouse.LeftButton == MouseButtonState.Pressed) return;

            // Hide the scrollbar
            scrollBarTimer.Stop();
            if (sv.HorizontalScrollBarVisibility == ScrollBarVisibility.Visible)
                sv.HorizontalScrollBarVisibility = ScrollBarVisibility.Hidden;
        }

        private void ShowHorizontalScrollBar()
        {
            // Ignore if scrollbar is visible yet or not to be viewed
            if (!HorizontalScrollBarEnabled || sv.HorizontalScrollBarVisibility == ScrollBarVisibility.Visible) return;

            // Restart the timer and show the scrollbar
            scrollBarTimer.Stop();
            if (sv.HorizontalScrollBarVisibility == ScrollBarVisibility.Hidden)
                sv.HorizontalScrollBarVisibility = ScrollBarVisibility.Visible;
            scrollBarTimer.Start();
        }

        public override void OnApplyTemplate()
        {
            sv = (ScrollViewer)Template.FindName("PART_ScrollViewer", this);

            // Apply the handler for scrollbar visibility
            sv.ScrollChanged += (s, e) =>
            {
                if (HorizontalScrollBarEnabled && Math.Abs(e.HorizontalChange) > PixelsToMoveToBeConsideredScroll)
                    ShowHorizontalScrollBar();
            };

            base.OnApplyTemplate();
        }

        protected override void OnPreviewMouseDown(MouseButtonEventArgs e)
        {
            if (sv.IsMouseOver)
            {
                // Save starting point, used later when determining how much to scroll.
                scrollStartPoint = e.GetPosition(this);
                // Update the cursor if can scroll or not.
                Cursor = (sv.ExtentWidth > sv.ViewportWidth) || (sv.ExtentHeight > sv.ViewportHeight) ? Cursors.ScrollAll : Cursors.Arrow;
                HandleMouseDown();
            }

            base.OnPreviewMouseDown(e);
        }

        protected override void OnPreviewTouchDown(TouchEventArgs e)
        {
            scrollStartPoint = e.GetTouchPoint(this).Position;
            HandleMouseDown();
        }

        private void HandleMouseDown()
        {
            tile = null;

            scrollStartOffset.X = sv.HorizontalOffset;
            scrollStartOffset.Y = sv.VerticalOffset;

            //store Control if one was found, so we can call its command later
            var x = TreeHelper.TryFindFromPoint<ListBoxItem>(this, scrollStartPoint);
            if (x != null)
            {
                x.IsSelected = !x.IsSelected;
                ItemsControl tiles = ItemsControlFromItemContainer(x);
                var data = tiles.ItemContainerGenerator.ItemFromContainer(x);
                if (data != null && data is IPanoramaTile)
                {
                    tile = (IPanoramaTile)data;
                }
            }
        }

        protected override void OnPreviewMouseMove(MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                Point currentPoint = e.GetPosition(this);
                if (HandleMouseMove(currentPoint))
                {
                    CaptureMouse();
                }
            }

            base.OnPreviewMouseMove(e);
        }

        protected override void OnPreviewTouchMove(TouchEventArgs e)
        {
            Point currentPoint = e.GetTouchPoint(this).Position;
            if (HandleMouseMove(currentPoint))
            {
                touchCaptured = true;
                lastTouchPosition = currentPoint;
            }
        }

        private bool HandleMouseMove(Point currentPoint)
        {
            // Determine the new amount to scroll.
            var delta = new Point(scrollStartPoint.X - currentPoint.X, scrollStartPoint.Y - currentPoint.Y);

            if (Math.Abs(delta.X) < PixelsToMoveToBeConsideredScroll &&
                Math.Abs(delta.Y) < PixelsToMoveToBeConsideredScroll)
                return false;

            scrollTarget.X = scrollStartOffset.X + delta.X;
            scrollTarget.Y = scrollStartOffset.Y + delta.Y;

            // Scroll to the new position.
            sv.ScrollToHorizontalOffset(scrollTarget.X);
            sv.ScrollToVerticalOffset(scrollTarget.Y);
            return true;
        }

        protected override void OnPreviewMouseUp(MouseButtonEventArgs e)
        {
            Point currentPoint = e.GetPosition(this);

            if (IsMouseCaptured)
            {
                ReleaseMouseCapture();
            }
            Cursor = Cursors.Arrow;
            HandleMouseUp(currentPoint);

            base.OnPreviewMouseUp(e);
        }

        protected override void OnPreviewTouchUp(TouchEventArgs e)
        {
            if (touchCaptured)
            {
                touchCaptured = false;
            }
            Point currentPoint = e.GetTouchPoint(this).Position;
            HandleMouseUp(currentPoint);

            base.OnPreviewTouchUp(e);
        }

        private void HandleMouseUp(Point currentPoint)
        {

            // Determine the new amount to scroll.
            var delta = new Point(scrollStartPoint.X - currentPoint.X, scrollStartPoint.Y - currentPoint.Y);

            if (Math.Abs(delta.X) < PixelsToMoveToBeConsideredClick &&
                Math.Abs(delta.Y) < PixelsToMoveToBeConsideredClick && tile != null)
            {
                if (tile.TileClickedCommand != null)
                    //Ok, its a click ask the tile to do its job
                    if (tile.TileClickedCommand.CanExecute(null))
                    {
                        tile.TileClickedCommand.Execute(null);
                    }
            }
        }

        protected override void OnPreviewMouseWheel(MouseWheelEventArgs e)
        {
            if (!MouseScrollEnabled) return;

            // Pause the scrollbar timer
            if (scrollBarTimer.IsEnabled)
                scrollBarTimer.Stop();

            // Determine the new amount to scroll.
            scrollTarget.X = sv.HorizontalOffset + ((e.Delta * -1) / 3);

            // Scroll to the new position.
            sv.ScrollToHorizontalOffset(scrollTarget.X);
            CaptureMouse();

            // Save starting point, used later when determining how much to scroll.
            scrollStartPoint = e.GetPosition(this);
            scrollStartOffset.X = sv.HorizontalOffset;

            // Restart the scrollbar timer
            if (HorizontalScrollBarEnabled)
                scrollBarTimer.Start();

            base.OnPreviewMouseWheel(e);
        }

    }
}
