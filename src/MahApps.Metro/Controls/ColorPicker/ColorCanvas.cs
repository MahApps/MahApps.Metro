using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using MahApps.Metro.Controls.ColorPicker;

namespace MahApps.Metro.Controls
{
    [TemplatePart(Name = "PART_SaturationValueBox", Type = typeof(Control))]
    [TemplatePart(Name = "PART_SaturationValueBox_Background", Type = typeof(SolidColorBrush))]
    [TemplatePart(Name = "PART_PickColorFromScreen", Type = typeof(Button))]
    public class ColorCanvas : Control
    {
        #region private Members

        SolidColorBrush PART_SaturationValueBox_Background;
        FrameworkElement PART_SaturationValueBox;
        Button PART_PickColorFromScreen;

        bool ColorIsUpdating = false;

        #endregion

        // Depency Properties
        public static readonly DependencyProperty SelectedColorProperty = DependencyProperty.Register("SelectedColor", typeof(Color), typeof(ColorCanvas), new FrameworkPropertyMetadata(Colors.Black, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, ColorChanged));
        public static readonly DependencyProperty HexCodeProperty = DependencyProperty.Register("HexCode", typeof(string), typeof(ColorCanvas), new FrameworkPropertyMetadata("#FF000000", FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, ColorNameChanged), IsValidHexCode);
        public static readonly DependencyProperty ColorNameProperty = DependencyProperty.Register("ColorName", typeof(string), typeof(ColorCanvas), new PropertyMetadata(ColorHelper.GetColorName(Colors.Black)));

        public static readonly DependencyProperty AProperty = DependencyProperty.Register("A", typeof(byte), typeof(ColorCanvas), new FrameworkPropertyMetadata((byte)255, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, ColorChannelChanged));
        public static readonly DependencyProperty RProperty = DependencyProperty.Register("R", typeof(byte), typeof(ColorCanvas), new FrameworkPropertyMetadata((byte)0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, ColorChannelChanged));
        public static readonly DependencyProperty GProperty = DependencyProperty.Register("G", typeof(byte), typeof(ColorCanvas), new FrameworkPropertyMetadata((byte)0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, ColorChannelChanged));
        public static readonly DependencyProperty BProperty = DependencyProperty.Register("B", typeof(byte), typeof(ColorCanvas), new FrameworkPropertyMetadata((byte)0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, ColorChannelChanged));

        public static readonly DependencyProperty HueProperty = DependencyProperty.Register("Hue", typeof(double), typeof(ColorCanvas), new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, HSV_Values_Changed));
        public static readonly DependencyProperty SaturationProperty = DependencyProperty.Register("Saturation", typeof(double), typeof(ColorCanvas), new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, HSV_Values_Changed));
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(double), typeof(ColorCanvas), new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, HSV_Values_Changed));

        public Color SelectedColor
        {
            get { return (Color)this.GetValue(SelectedColorProperty); }
            set { this.SetValue(SelectedColorProperty, value); }
        }

        public string HexCode
        {
            get { return (string)this.GetValue(HexCodeProperty); }
            set { this.SetValue(HexCodeProperty, value); }
        }

        public string ColorName
        {
            get { return (string)this.GetValue(ColorNameProperty); }
            set { this.SetValue(ColorNameProperty, value); }
        }

        private static void ColorChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            if (dependencyObject is ColorCanvas colorCanvas)
            {
                // don't do a second update
                if (colorCanvas.ColorIsUpdating)
                    return;

                colorCanvas.ColorIsUpdating = true;

                colorCanvas.SetCurrentValue(HexCodeProperty, colorCanvas.SelectedColor.ToString());
                colorCanvas.SetCurrentValue(ColorNameProperty, ColorHelper.GetColorName(colorCanvas.SelectedColor));

                var hsv = new HSVColor(colorCanvas.SelectedColor);
                colorCanvas.SetCurrentValue(HueProperty, hsv.Hue);
                colorCanvas.SetCurrentValue(SaturationProperty, hsv.Saturation);
                colorCanvas.SetCurrentValue(ValueProperty, hsv.Value);

                colorCanvas.SetCurrentValue(AProperty, colorCanvas.SelectedColor.A);
                colorCanvas.SetCurrentValue(RProperty, colorCanvas.SelectedColor.R);
                colorCanvas.SetCurrentValue(GProperty, colorCanvas.SelectedColor.G);
                colorCanvas.SetCurrentValue(BProperty, colorCanvas.SelectedColor.B);

                colorCanvas.PART_SaturationValueBox_Background.Color = new HSVColor(hsv.Hue, 1, 1).ToColor();

                colorCanvas.ColorIsUpdating = false;
            }
        }

        private static void ColorNameChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            if (dependencyObject is ColorCanvas colorCanvas)
            {
                if (ColorHelper.ColorFromString(colorCanvas.HexCode) is Color color && !colorCanvas.ColorIsUpdating)
                {
                    colorCanvas.SetCurrentValue(SelectedColorProperty, color);
                }
            }
        }

        public static bool IsValidHexCode(object value)
        {
            return ColorHelper.ColorFromString(value.ToString()).HasValue;
        }

        #region ARGB

        public byte A
        {
            get { return (byte)this.GetValue(AProperty); }
            set { this.SetValue(AProperty, value); }
        }

        public byte R
        {
            get { return (byte)this.GetValue(RProperty); }
            set { this.SetValue(RProperty, value); }
        }

        public byte G
        {
            get { return (byte)this.GetValue(GProperty); }
            set { this.SetValue(GProperty, value); }
        }

        public byte B
        {
            get { return (byte)this.GetValue(BProperty); }
            set { this.SetValue(BProperty, value); }
        }

        public static void ColorChannelChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            if (dependencyObject is ColorCanvas colorCanvas && !colorCanvas.ColorIsUpdating)
            {
                colorCanvas.SetCurrentValue(SelectedColorProperty, Color.FromArgb(colorCanvas.A, colorCanvas.R, colorCanvas.G, colorCanvas.B));
            }
        }

        #endregion

        public double Hue
        {
            get { return (double)this.GetValue(HueProperty); }
            set { this.SetValue(HueProperty, value); }
        }

        public double Saturation
        {
            get { return (double)this.GetValue(SaturationProperty); }
            set { this.SetValue(SaturationProperty, value); }
        }

        public double Value
        {
            get { return (double)this.GetValue(ValueProperty); }
            set { this.SetValue(ValueProperty, value); }
        }

        private static void HSV_Values_Changed(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            if (dependencyObject is ColorCanvas colorCanvas && !colorCanvas.ColorIsUpdating)
            {
                var hsv = new HSVColor(colorCanvas.Hue, colorCanvas.Saturation, colorCanvas.Value);
                colorCanvas.SetCurrentValue(SelectedColorProperty, hsv.ToColor(colorCanvas.A));
            }
        }

        private void PART_SaturationValueBox_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Mouse.Capture(null);
            this.PART_SaturationValueBox.MouseMove -= this.PART_SaturationValueBox_MouseMove;
        }

        private void PART_SaturationValueBox_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Mouse.Capture(this.PART_SaturationValueBox);
            this.PART_SaturationValueBox.MouseMove += this.PART_SaturationValueBox_MouseMove;

            this.PART_SaturationValueBox_UpdateValues(e.GetPosition(this.PART_SaturationValueBox));
        }

        private void PART_SaturationValueBox_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton != MouseButtonState.Pressed)
            {
                this.PART_SaturationValueBox.MouseMove -= this.PART_SaturationValueBox_MouseMove;
            }
            else
            {
                this.PART_SaturationValueBox_UpdateValues(e.GetPosition(this.PART_SaturationValueBox));
            }
        }

        private void PART_SaturationValueBox_UpdateValues(Point position)
        {
            if (this.PART_SaturationValueBox.ActualWidth < 1 || this.PART_SaturationValueBox.ActualHeight < 1)
                return;

            var s = position.X / this.PART_SaturationValueBox.ActualWidth;
            var v = 1 - (position.Y / this.PART_SaturationValueBox.ActualHeight);

            if (s > 1) s = 1;
            if (v > 1) v = 1;

            if (s < 0) s = 0;
            if (v < 0) v = 0;

            this.SetCurrentValue(SaturationProperty, s);
            this.SetCurrentValue(ValueProperty, v);
        }

        #region Overrides

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            this.PART_SaturationValueBox_Background = (SolidColorBrush)this.GetTemplateChild("PART_SaturationValueBox_Background");

            this.PART_SaturationValueBox = (FrameworkElement)this.GetTemplateChild("PART_SaturationValueBox");
            this.PART_SaturationValueBox.MouseLeftButtonDown += this.PART_SaturationValueBox_MouseLeftButtonDown;
            this.PART_SaturationValueBox.MouseLeftButtonUp += this.PART_SaturationValueBox_MouseLeftButtonUp;

            this.PART_PickColorFromScreen = (Button)this.GetTemplateChild("PART_PickColorFromScreen");
            this.PART_PickColorFromScreen.PreviewMouseLeftButtonDown += this.PART_PickColorFromScreen_PreviewMouseDown;
            ;
            this.PART_PickColorFromScreen.PreviewMouseLeftButtonUp += this.PART_PickColorFromScreen_PreviewMouseUp;
            ;
        }

        private void PART_PickColorFromScreen_PreviewMouseUp(object sender, MouseEventArgs e)
        {
            Mouse.Capture(null);
            this.PART_PickColorFromScreen.MouseMove -= this.PART_PickColorFromScreen_PreviewMouseMove;

            this.PART_PickColorFromScreen.Cursor = Cursors.Arrow;

            if (this.PART_PickColorFromScreen.ToolTip is ToolTip toolTip)
            {
                toolTip.IsOpen = false;
            }

            if (!this.PART_PickColorFromScreen.IsMouseOver)
            {
                Point pointToWindow = Mouse.GetPosition(this);
                Point pointToScreen = this.PointToScreen(pointToWindow);
                this.SelectedColor = ColorHelper.GetPixelColor(pointToScreen);
            }

            this.UpdateTooltip_Timer = null;
        }

        private void PART_PickColorFromScreen_PreviewMouseDown(object sender, MouseEventArgs e)
        {
            this.PART_PickColorFromScreen.PreviewMouseMove += this.PART_PickColorFromScreen_PreviewMouseMove;
            this.PART_PickColorFromScreen.Cursor = (Cursor)this.PART_PickColorFromScreen.Resources["MahApps.Cursors.EyeDropper"];
            Mouse.Capture(this.PART_PickColorFromScreen);

            if (this.PART_PickColorFromScreen.ToolTip is ToolTip toolTip)
            {
                toolTip.IsOpen = true;
                toolTip.StaysOpen = true;
                toolTip.Placement = PlacementMode.MousePoint;
                toolTip.HorizontalOffset = Mouse.GetPosition(this.PART_PickColorFromScreen).X + 18;
                toolTip.VerticalOffset = Mouse.GetPosition(this.PART_PickColorFromScreen).Y - 18;

                this.UpdateTooltip_Timer = new DispatcherTimer();
                this.UpdateTooltip_Timer.Interval = TimeSpan.FromSeconds(0.1);
                this.UpdateTooltip_Timer.Tick += this.PART_PickColorFromScreen_UpdateTooltip;
                this.UpdateTooltip_Timer.Start();
            }

            this.PART_PickColorFromScreen_UpdateTooltip(this, new EventArgs());

            e.Handled = true;
        }

        private void PART_PickColorFromScreen_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (this.PART_PickColorFromScreen.ToolTip is ToolTip toolTip)
            {
                toolTip.IsOpen = true;
                toolTip.StaysOpen = true;
                toolTip.Placement = PlacementMode.MousePoint;
                toolTip.HorizontalOffset = Mouse.GetPosition(this.PART_PickColorFromScreen).X + 18;
                toolTip.VerticalOffset = Mouse.GetPosition(this.PART_PickColorFromScreen).Y - 18;
            }
        }

        private DispatcherTimer UpdateTooltip_Timer;

        private void PART_PickColorFromScreen_UpdateTooltip(object sender, EventArgs e)
        {
            if (this.PART_PickColorFromScreen.ToolTip is ToolTip toolTip)
            {
                Point pointToWindow = Mouse.GetPosition(this);
                Point pointToScreen = this.PointToScreen(pointToWindow);
                toolTip.DataContext = ColorHelper.GetPixelColor(pointToScreen);
            }
        }

        #endregion
    }
}