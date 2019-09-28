using MahApps.Metro.Controls.ColorPicker;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace MahApps.Metro.Controls
{
    [TemplatePart(Name = "PART_SaturationValueBox", Type = typeof(Control))]
    [TemplatePart(Name = "PART_SaturationValueBox_Background", Type = typeof(SolidColorBrush))]
    [TemplatePart(Name = "PART_PickColorFromScreen", Type = typeof(Button))]
    public class ColorCanvas: Control
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
            get { return (Color)GetValue(SelectedColorProperty); }
            set { SetValue(SelectedColorProperty, value); }
        }

        public string HexCode
        {
            get { return (string)GetValue(HexCodeProperty); }
            set { SetValue(HexCodeProperty, value); }
        }


        public string ColorName
        {
            get { return (string)GetValue(ColorNameProperty); }
            set { SetValue(ColorNameProperty, value); }
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
                colorCanvas.SetCurrentValue(SaturationProperty, hsv.Saturation * 100);
                colorCanvas.SetCurrentValue(ValueProperty, hsv.Value * 100);

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
            get { return (byte)GetValue(AProperty); }
            set { SetValue(AProperty, value); }
        }

        public byte R
        {
            get { return (byte)GetValue(RProperty); }
            set { SetValue(RProperty, value); }
        }

        public byte G
        {
            get { return (byte)GetValue(GProperty); }
            set { SetValue(GProperty, value); }
        }

        public byte B
        {
            get { return (byte)GetValue(BProperty); }
            set { SetValue(BProperty, value); }
        }


        public static void ColorChannelChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            if (dependencyObject is ColorCanvas colorCanvas && ! colorCanvas.ColorIsUpdating)
            {
                colorCanvas.SetCurrentValue(SelectedColorProperty, Color.FromArgb(colorCanvas.A, colorCanvas.R, colorCanvas.G, colorCanvas.B));
            }
        }
        #endregion

        public double Hue
        {
            get { return (double)GetValue(HueProperty); }
            set { SetValue(HueProperty, value); }
        }

        public double Saturation
        {
            get { return (double)GetValue(SaturationProperty); }
            set { SetValue(SaturationProperty, value); }
        }

        public double Value
        {
            get { return (double)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        private static void HSV_Values_Changed(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            if (dependencyObject is ColorCanvas colorCanvas && !colorCanvas.ColorIsUpdating)
            {
                var hsv = new HSVColor(colorCanvas.Hue, colorCanvas.Saturation / 100, colorCanvas.Value / 100);
                colorCanvas.SetCurrentValue(SelectedColorProperty, hsv.ToColor(colorCanvas.A));
            }
        }

        private void PART_SaturationValueBox_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Mouse.Capture(null);
            PART_SaturationValueBox.MouseMove -= PART_SaturationValueBox_MouseMove;
        }

        private void PART_SaturationValueBox_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Mouse.Capture(PART_SaturationValueBox);
            PART_SaturationValueBox.MouseMove += PART_SaturationValueBox_MouseMove;

            PART_SaturationValueBox_UpdateValues(e.GetPosition(PART_SaturationValueBox));
        }

        private void PART_SaturationValueBox_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton != MouseButtonState.Pressed)
            {
                PART_SaturationValueBox.MouseMove -= PART_SaturationValueBox_MouseMove;
            }
            else
            {
                PART_SaturationValueBox_UpdateValues(e.GetPosition(PART_SaturationValueBox));
            }
        }

        private void PART_SaturationValueBox_UpdateValues(Point position)
        {
            if (PART_SaturationValueBox.ActualWidth < 1 || PART_SaturationValueBox.ActualHeight < 1)
                return;

            var s = position.X / PART_SaturationValueBox.ActualWidth;
            var v = 1 - (position.Y / PART_SaturationValueBox.ActualHeight);

            if (s > 1) s = 1;
            if (v > 1) v = 1;

            if (s < 0) s = 0;
            if (v < 0) v = 0;

            SetCurrentValue (SaturationProperty, s * 100);
            SetCurrentValue (ValueProperty, v * 100);
        }


        #region Overrides

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            PART_SaturationValueBox_Background = (SolidColorBrush)base.GetTemplateChild("PART_SaturationValueBox_Background");

            PART_SaturationValueBox = (FrameworkElement)base.GetTemplateChild("PART_SaturationValueBox");
            PART_SaturationValueBox.MouseLeftButtonDown += PART_SaturationValueBox_MouseLeftButtonDown;
            PART_SaturationValueBox.MouseLeftButtonUp += PART_SaturationValueBox_MouseLeftButtonUp;

            PART_PickColorFromScreen = (Button)base.GetTemplateChild("PART_PickColorFromScreen");
            PART_PickColorFromScreen.PreviewMouseLeftButtonDown += PART_PickColorFromScreen_PreviewMouseDown; ;
            PART_PickColorFromScreen.PreviewMouseLeftButtonUp += PART_PickColorFromScreen_PreviewMouseUp; ;

        }

        private void PART_PickColorFromScreen_PreviewMouseUp(object sender, MouseEventArgs e)
        {
            Mouse.Capture(null);
            PART_PickColorFromScreen.MouseMove -= PART_PickColorFromScreen_PreviewMouseMove;

            PART_PickColorFromScreen.Cursor = Cursors.Arrow;

            if (PART_PickColorFromScreen.ToolTip is ToolTip toolTip)
            {
                toolTip.IsOpen = false;
            }

            if (!PART_PickColorFromScreen.IsMouseOver)
            {
                Point pointToWindow = Mouse.GetPosition(this);
                Point pointToScreen = PointToScreen(pointToWindow);
                SelectedColor = ColorHelper.GetPixelColor(pointToScreen);
            }

            UpdateTooltip_Timer = null;
        }

        private void PART_PickColorFromScreen_PreviewMouseDown(object sender, MouseEventArgs e)
        {
            PART_PickColorFromScreen.PreviewMouseMove += PART_PickColorFromScreen_PreviewMouseMove;
            PART_PickColorFromScreen.Cursor = (Cursor)PART_PickColorFromScreen.Resources["MahApps.Cursors.EyeDropper"];
            Mouse.Capture(PART_PickColorFromScreen);

            if (PART_PickColorFromScreen.ToolTip is ToolTip toolTip)
            {
                toolTip.IsOpen = true;
                toolTip.StaysOpen = true;
                toolTip.Placement = PlacementMode.MousePoint;
                toolTip.HorizontalOffset = Mouse.GetPosition(PART_PickColorFromScreen).X + 18;
                toolTip.VerticalOffset = Mouse.GetPosition(PART_PickColorFromScreen).Y - 18;

                UpdateTooltip_Timer = new DispatcherTimer();
                UpdateTooltip_Timer.Interval = TimeSpan.FromSeconds(0.1);
                UpdateTooltip_Timer.Tick += PART_PickColorFromScreen_UpdateTooltip;
                UpdateTooltip_Timer.Start();
            }

            PART_PickColorFromScreen_UpdateTooltip(this, new EventArgs());

            e.Handled = true;
        }

        private void PART_PickColorFromScreen_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (PART_PickColorFromScreen.ToolTip is ToolTip toolTip)
            {
                toolTip.IsOpen = true;
                toolTip.StaysOpen = true;
                toolTip.Placement = PlacementMode.MousePoint;
                toolTip.HorizontalOffset = Mouse.GetPosition(PART_PickColorFromScreen).X + 18;
                toolTip.VerticalOffset = Mouse.GetPosition(PART_PickColorFromScreen).Y - 18;
            }
        }

        private DispatcherTimer UpdateTooltip_Timer;
        private void PART_PickColorFromScreen_UpdateTooltip(object sender, EventArgs e)
        {
            if (PART_PickColorFromScreen.ToolTip is ToolTip toolTip)
            {
                Point pointToWindow = Mouse.GetPosition(this);
                Point pointToScreen = PointToScreen(pointToWindow);
                toolTip.DataContext = ColorHelper.GetPixelColor(pointToScreen);
            }
        }
        #endregion

    }
}
