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

        private ScrollViewer sv;
        private Point scrollTarget;
        private Point scrollStartPoint;
        private Point scrollStartOffset;
        private Point previousPoint;
        private Vector velocity;
        private double friction;
        private DispatcherTimer animationTimer = new DispatcherTimer(DispatcherPriority.DataBind);
        private static int PixelsToMoveToBeConsideredScroll = 5;
        private static int PixelsToMoveToBeConsideredClick = 2;
        private IPanoramaTile tile;

        public Panorama()
        {
            friction = 0.85;

            animationTimer.Interval = new TimeSpan(0, 0, 0, 0, 20);
            animationTimer.Tick += HandleWorldTimerTick;
            animationTimer.Start();
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

            if (IsMouseCaptured)
            {
                Point currentPoint = Mouse.GetPosition(this);
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

        public override void OnApplyTemplate()
        {
            sv = (ScrollViewer)Template.FindName("PART_ScrollViewer", this);
            base.OnApplyTemplate();
        }

        protected override void OnPreviewMouseDown(MouseButtonEventArgs e)
        {
            if (sv.IsMouseOver)
            {
                tile = null;

                // Save starting point, used later when determining how much to scroll.
                scrollStartPoint = e.GetPosition(this);
                scrollStartOffset.X = sv.HorizontalOffset;
                scrollStartOffset.Y = sv.VerticalOffset;

                // Update the cursor if can scroll or not.
                Cursor = (sv.ExtentWidth > sv.ViewportWidth) || (sv.ExtentHeight > sv.ViewportHeight) ? Cursors.ScrollAll : Cursors.Arrow;

                //store Control if one was found, so we can call its command later
                var x = TreeHelper.TryFindFromPoint<ListBoxItem>(this, scrollStartPoint);
                if (x != null)
                {
                    x.IsSelected = true;
                    ItemsControl tiles = ItemsControlFromItemContainer(x);
                    var data = tiles.ItemContainerGenerator.ItemFromContainer(x);
                    if (data != null && data is IPanoramaTile)
                    {
                        tile = (IPanoramaTile)data;
                    }
                }

                CaptureMouse();
            }

            base.OnPreviewMouseDown(e);
        }

        protected override void OnPreviewMouseMove(MouseEventArgs e)
        {
            if (IsMouseCaptured)
            {
                Point currentPoint = e.GetPosition(this);

                // Determine the new amount to scroll.
                var delta = new Point(scrollStartPoint.X - currentPoint.X, scrollStartPoint.Y - currentPoint.Y);

                if (Math.Abs(delta.X) < PixelsToMoveToBeConsideredScroll &&
                    Math.Abs(delta.Y) < PixelsToMoveToBeConsideredScroll)
                    return;

                scrollTarget.X = scrollStartOffset.X + delta.X;
                scrollTarget.Y = scrollStartOffset.Y + delta.Y;

                // Scroll to the new position.
                sv.ScrollToHorizontalOffset(scrollTarget.X);
                sv.ScrollToVerticalOffset(scrollTarget.Y);
            }

            base.OnPreviewMouseMove(e);
        }

        protected override void OnPreviewMouseUp(MouseButtonEventArgs e)
        {
            if (IsMouseCaptured)
            {
                Cursor = Cursors.Arrow;
                ReleaseMouseCapture();
            }

            Point currentPoint = e.GetPosition(this);

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

            base.OnPreviewMouseUp(e);
        }

    }
}
