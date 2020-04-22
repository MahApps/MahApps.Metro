using MahApps.Metro.Controls.ColorPicker;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace MahApps.Metro.Controls
{
    [DefaultEvent("SelectedColorChanged")]
    public class ColorPickerBase : Control
    {
        #region private Members
        protected bool ColorIsUpdating = false;
        protected bool UpdateHsvValues = true;
        #endregion

        #region Dependcy Properties
        public static readonly DependencyProperty SelectedColorProperty = DependencyProperty.Register(nameof(SelectedColor), typeof(Color), typeof(ColorPickerBase), new FrameworkPropertyMetadata(Colors.Black, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, ColorChanged));
        public static readonly DependencyProperty HexCodeProperty = DependencyProperty.Register(nameof(HexCode), typeof(string), typeof(ColorPickerBase), new FrameworkPropertyMetadata("#FF000000", FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, ColorHexcodeChanged), IsValidHexCodeOrName);
        public static readonly DependencyProperty ColorNameProperty = DependencyProperty.Register(nameof(ColorName), typeof(string), typeof(ColorPickerBase), new FrameworkPropertyMetadata(ColorHelper.GetColorName(Colors.Black), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, ColorNameChanged), IsValidHexCodeOrName);

        public static readonly DependencyProperty AProperty = DependencyProperty.Register("A", typeof(byte), typeof(ColorPickerBase), new FrameworkPropertyMetadata((byte)255, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, ColorChannelChanged));
        public static readonly DependencyProperty RProperty = DependencyProperty.Register("R", typeof(byte), typeof(ColorPickerBase), new FrameworkPropertyMetadata((byte)0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, ColorChannelChanged));
        public static readonly DependencyProperty GProperty = DependencyProperty.Register("G", typeof(byte), typeof(ColorPickerBase), new FrameworkPropertyMetadata((byte)0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, ColorChannelChanged));
        public static readonly DependencyProperty BProperty = DependencyProperty.Register("B", typeof(byte), typeof(ColorPickerBase), new FrameworkPropertyMetadata((byte)0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, ColorChannelChanged));

        public static readonly DependencyProperty HueProperty = DependencyProperty.Register("Hue", typeof(double), typeof(ColorPickerBase), new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, HSV_Values_Changed));
        public static readonly DependencyProperty SaturationProperty = DependencyProperty.Register("Saturation", typeof(double), typeof(ColorPickerBase), new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, HSV_Values_Changed));
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(double), typeof(ColorPickerBase), new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, HSV_Values_Changed));
        #endregion

        #region SelectedColorChangedEvent   

        #endregion

        #region ColorProperties
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
        #endregion

        #region Color changed

        public static readonly RoutedEvent SelectedColorChangedEvent = EventManager.RegisterRoutedEvent(
                                                                        nameof(SelectedColorChanged),
                                                                        RoutingStrategy.Bubble,
                                                                        typeof(EventHandler<TimePickerBaseSelectionChangedEventArgs<Color>>),
                                                                        typeof(ColorPickerBase));

        private static void ColorChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            if (dependencyObject is ColorPickerBase colorPicker)
            {
                // don't do a second update
                if (colorPicker.ColorIsUpdating)
                    return;

                colorPicker.ColorIsUpdating = true;

                colorPicker.SetCurrentValue(HexCodeProperty, colorPicker.SelectedColor.ToString());
                colorPicker.SetCurrentValue(ColorNameProperty, ColorHelper.GetColorName(colorPicker.SelectedColor));

                if (colorPicker.UpdateHsvValues)
                {
                    var hsv = new HSVColor(colorPicker.SelectedColor);
                    colorPicker.SetCurrentValue(HueProperty, hsv.Hue);
                    colorPicker.SetCurrentValue(SaturationProperty, hsv.Saturation);
                    colorPicker.SetCurrentValue(ValueProperty, hsv.Value);
                }

                colorPicker.SetCurrentValue(AProperty, colorPicker.SelectedColor.A);
                colorPicker.SetCurrentValue(RProperty, colorPicker.SelectedColor.R);
                colorPicker.SetCurrentValue(GProperty, colorPicker.SelectedColor.G);
                colorPicker.SetCurrentValue(BProperty, colorPicker.SelectedColor.B);

                colorPicker.ColorIsUpdating = false;

                colorPicker.RaiseEvent(new TimePickerBaseSelectionChangedEventArgs<Color>(SelectedColorChangedEvent, (Color)e.OldValue, (Color)e.NewValue));
            }
        }

        private static void ColorHexcodeChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            if (dependencyObject is ColorPickerBase colorPicker)
            {
                if (ColorHelper.ColorFromString(colorPicker.HexCode) is Color color && !colorPicker.ColorIsUpdating)
                {
                    colorPicker.SetCurrentValue(SelectedColorProperty, color);
                }
            }
        }

        private static void ColorNameChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            if (dependencyObject is ColorPickerBase colorPicker)
            {
                if (ColorHelper.ColorFromString(colorPicker.ColorName) is Color color && !colorPicker.ColorIsUpdating)
                {
                    colorPicker.SetCurrentValue(SelectedColorProperty, color);
                }
            }
        }

        /// <summary>
        ///     Occurs when the <see cref="SelectedColor" /> property is changed.
        /// </summary>
        public event EventHandler<TimePickerBaseSelectionChangedEventArgs<Color>> SelectedColorChanged
        {
            add { AddHandler(SelectedColorChangedEvent, value); }
            remove { RemoveHandler(SelectedColorChangedEvent, value); }
        }
        #endregion


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
            if (dependencyObject is ColorPickerBase colorPicker && !colorPicker.ColorIsUpdating)
            {
                colorPicker.SetCurrentValue(SelectedColorProperty, Color.FromArgb(colorPicker.A, colorPicker.R, colorPicker.G, colorPicker.B));
            }
        }

        #endregion

        #region HSV
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
            if (dependencyObject is ColorPickerBase colorPicker && !colorPicker.ColorIsUpdating)
            {
                var hsv = new HSVColor(colorPicker.Hue, colorPicker.Saturation, colorPicker.Value);

                colorPicker.UpdateHsvValues = false;
                colorPicker.SetCurrentValue(SelectedColorProperty, hsv.ToColor(colorPicker.A));
                colorPicker.UpdateHsvValues = true;
            }
        }
        #endregion

        #region Validation
        private static bool IsValidHexCodeOrName(object value)
        {
            return ColorHelper.ColorFromString(value?.ToString()).HasValue;
        }
        #endregion
    }
}
