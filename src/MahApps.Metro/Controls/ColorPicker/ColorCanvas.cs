using MahApps.Metro.Controls.ColorPicker;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace MahApps.Metro.Controls
{
    [TemplatePart(Name = "PART_SaturationValueBox", Type = typeof(Control))]
    [TemplatePart(Name = "PART_SaturationValueBox_Background", Type = typeof(SolidColorBrush))]
    public class ColorCanvas: Control
    {
        SolidColorBrush PART_SaturationValueBox_Background;

        public static readonly DependencyProperty SelectedColorProperty = DependencyProperty.Register("SelectedColor", typeof(Color), typeof(ColorCanvas), new FrameworkPropertyMetadata(Colors.Black, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public static readonly DependencyProperty AProperty = DependencyProperty.Register("A", typeof(byte), typeof(ColorCanvas), new FrameworkPropertyMetadata((byte)255, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, ColorChannelChanged));
        public static readonly DependencyProperty RProperty = DependencyProperty.Register("R", typeof(byte), typeof(ColorCanvas), new FrameworkPropertyMetadata((byte)0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, ColorChannelChanged));
        public static readonly DependencyProperty GProperty = DependencyProperty.Register("G", typeof(byte), typeof(ColorCanvas), new FrameworkPropertyMetadata((byte)0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, ColorChannelChanged));
        public static readonly DependencyProperty BProperty = DependencyProperty.Register("B", typeof(byte), typeof(ColorCanvas), new FrameworkPropertyMetadata((byte)0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, ColorChannelChanged));

        public static readonly DependencyProperty HueProperty = DependencyProperty.Register("Hue", typeof(double), typeof(ColorCanvas), new PropertyMetadata(0d));
        public static readonly DependencyProperty SaturationProperty = DependencyProperty.Register("Saturation", typeof(double), typeof(ColorCanvas), new PropertyMetadata(0d));
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(double), typeof(ColorCanvas), new PropertyMetadata(0d));

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

        public static void ColorChannelChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs eventArgs)
        {
            if (dependencyObject is ColorCanvas colorCanvas)
            {
                colorCanvas.SetCurrentValue(SelectedColorProperty, Color.FromArgb(colorCanvas.A, colorCanvas.R, colorCanvas.G, colorCanvas.B));

                var hsv = new HSVColor(colorCanvas.SelectedColor);
                colorCanvas.SetCurrentValue(HueProperty, hsv.Hue);
                colorCanvas.SetCurrentValue(SaturationProperty, hsv.Saturation);
                colorCanvas.SetCurrentValue(ValueProperty, hsv.Value);

                colorCanvas.PART_SaturationValueBox_Background.Color = new HSVColor(hsv.Hue, 1, 1).GetColor();
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



        #region Overrides

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            PART_SaturationValueBox_Background = (SolidColorBrush)base.GetTemplateChild("PART_SaturationValueBox_Background");
        }

        #endregion

    }
}
