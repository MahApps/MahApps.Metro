using MahApps.Metro.Controls.ColorPicker;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace MahApps.Metro.Controls
{
    [TemplatePart(Name = "PART_SaturationValueBox", Type = typeof(Control))]
    [TemplatePart(Name = "PART_SaturationValueBox_Background", Type = typeof(SolidColorBrush))]
    public class ColorCanvas: Control
    {

        #region private Members

        SolidColorBrush PART_SaturationValueBox_Background;
        FrameworkElement PART_SaturationValueBox;
        bool ColorIsUpdating = false;
        
        #endregion

        public static readonly DependencyProperty SelectedColorProperty = DependencyProperty.Register("SelectedColor", typeof(Color), typeof(ColorCanvas), new FrameworkPropertyMetadata(Colors.Black, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public static readonly DependencyProperty ColorNumberProperty = DependencyProperty.Register("ColorNumber", typeof(int), typeof(ColorCanvas), new PropertyMetadata(0));


        public int ColorNumber
        {
            get { return (int)GetValue(ColorNumberProperty); }
            set { SetValue(ColorNumberProperty, value); }
        }


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
            if (dependencyObject is ColorCanvas colorCanvas)
            {
                // don't do a second update
                if (colorCanvas.ColorIsUpdating)
                    return;

                colorCanvas.ColorIsUpdating = true;

                colorCanvas.SetCurrentValue(SelectedColorProperty, Color.FromArgb(colorCanvas.A, colorCanvas.R, colorCanvas.G, colorCanvas.B));

                var hsv = new HSVColor(colorCanvas.SelectedColor);
                colorCanvas.SetCurrentValue(HueProperty, hsv.Hue);
                colorCanvas.SetCurrentValue(SaturationProperty, hsv.Saturation * 100);
                colorCanvas.SetCurrentValue(ValueProperty, hsv.Value * 100);

                colorCanvas.PART_SaturationValueBox_Background.Color = new HSVColor(hsv.Hue, 1, 1).ToColor();

                colorCanvas.ColorIsUpdating = false;
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
            if (dependencyObject is ColorCanvas colorCanvas)
            {
                // don't do a second update
                if (colorCanvas.ColorIsUpdating)
                    return;

                colorCanvas.ColorIsUpdating = true;

                var hsv = new HSVColor(colorCanvas.Hue, colorCanvas.Saturation / 100, colorCanvas.Value / 100);

                var color = hsv.ToColor();

                colorCanvas.SetCurrentValue(SelectedColorProperty, color);
                colorCanvas.SetCurrentValue(RProperty, color.R);
                colorCanvas.SetCurrentValue(GProperty, color.G);
                colorCanvas.SetCurrentValue(BProperty, color.B);

                colorCanvas.PART_SaturationValueBox_Background.Color = new HSVColor(hsv.Hue, 1, 1).ToColor();

                colorCanvas.ColorIsUpdating = false;
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
        }


        #endregion

    }
}
