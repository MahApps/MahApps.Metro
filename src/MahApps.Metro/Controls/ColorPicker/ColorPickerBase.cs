using System;
using System.Collections.Generic;
using System.ComponentModel;
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
        /// <summary>Identifies the <see cref="SelectedColor"/> dependency property.</summary>
        public static readonly DependencyProperty SelectedColorProperty = DependencyProperty.Register(nameof(SelectedColor), typeof(Color?), typeof(ColorPickerBase), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, ColorChanged));
        
        /// <summary>Identifies the <see cref="DefaultColor"/> dependency property.</summary>
        public static readonly DependencyProperty DefaultColorProperty = DependencyProperty.Register(nameof(DefaultColor), typeof(Color?), typeof(ColorPickerBase), new FrameworkPropertyMetadata(null, ColorChanged));

        /// <summary>
        /// Identifies the <see cref="SelectedHSVColor"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectedHSVColorProperty = DependencyProperty.Register(nameof(SelectedHSVColor), typeof(HSVColor), typeof(ColorPickerBase), new PropertyMetadata(new HSVColor(Colors.Black)));

        /// <summary>Identifies the <see cref="ColorName"/> dependency property.</summary>
        public static readonly DependencyProperty ColorNameProperty = DependencyProperty.Register(nameof(ColorName), typeof(string), typeof(ColorPickerBase), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, ColorNameChanged));
        
        /// <summary>Identifies the <see cref="ColorNamesDictionary"/> dependency property.</summary>
        public static readonly DependencyProperty ColorNamesDictionaryProperty = DependencyProperty.Register(nameof(ColorNamesDictionary), typeof(Dictionary<Color?, string>), typeof(ColorPickerBase), new PropertyMetadata(null));

        /// <summary>Identifies the <see cref="A"/> dependency property.</summary>
        public static readonly DependencyProperty AProperty = DependencyProperty.Register(nameof(A), typeof(byte), typeof(ColorPickerBase), new FrameworkPropertyMetadata((byte)255, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, ColorChannelChanged));
        
        /// <summary>Identifies the <see cref="R"/> dependency property.</summary>
        public static readonly DependencyProperty RProperty = DependencyProperty.Register(nameof(R), typeof(byte), typeof(ColorPickerBase), new FrameworkPropertyMetadata((byte)0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, ColorChannelChanged));
        
        /// <summary>Identifies the <see cref="G"/> dependency property.</summary>
        public static readonly DependencyProperty GProperty = DependencyProperty.Register(nameof(G), typeof(byte), typeof(ColorPickerBase), new FrameworkPropertyMetadata((byte)0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, ColorChannelChanged));
        
        /// <summary>Identifies the <see cref="B"/> dependency property.</summary>
        public static readonly DependencyProperty BProperty = DependencyProperty.Register(nameof(B), typeof(byte), typeof(ColorPickerBase), new FrameworkPropertyMetadata((byte)0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, ColorChannelChanged));

        /// <summary>Identifies the <see cref="Hue"/> dependency property.</summary>
        public static readonly DependencyProperty HueProperty = DependencyProperty.Register(nameof(Hue), typeof(double), typeof(ColorPickerBase), new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, HSV_Values_Changed));
        
        /// <summary>Identifies the <see cref="Saturation"/> dependency property.</summary>
        public static readonly DependencyProperty SaturationProperty = DependencyProperty.Register(nameof(Saturation), typeof(double), typeof(ColorPickerBase), new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, HSV_Values_Changed));
        
        /// <summary>Identifies the <see cref="Value"/> dependency property.</summary>
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(nameof(Value), typeof(double), typeof(ColorPickerBase), new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, HSV_Values_Changed));



        /// <summary>Identifies the <see cref="LabelAlphaChannel"/> dependency property.</summary>
        public static readonly DependencyProperty LabelAlphaChannelProperty = DependencyProperty.Register(nameof(LabelAlphaChannel), typeof(object), typeof(ColorPickerBase), new PropertyMetadata("A"));

        /// <summary>Identifies the <see cref="LabelRedChannel"/> dependency property.</summary>
        public static readonly DependencyProperty LabelRedChannelProperty = DependencyProperty.Register(nameof(LabelRedChannel), typeof(object), typeof(ColorPickerBase), new PropertyMetadata("R"));

        /// <summary>Identifies the <see cref="LabelGreenChannel"/> dependency property.</summary>
        public static readonly DependencyProperty LabelGreenChannelProperty = DependencyProperty.Register(nameof(LabelGreenChannel), typeof(object), typeof(ColorPickerBase), new PropertyMetadata("G"));

        /// <summary>Identifies the <see cref="LabelBlueChannel"/> dependency property.</summary>
        public static readonly DependencyProperty LabelBlueChannelProperty = DependencyProperty.Register(nameof(LabelBlueChannel), typeof(object), typeof(ColorPickerBase), new PropertyMetadata("B"));

        /// <summary>Identifies the <see cref="LabelColorPreview"/> dependency property.</summary>
        public static readonly DependencyProperty LabelColorPreviewProperty = DependencyProperty.Register(nameof(LabelColorPreview), typeof(object), typeof(ColorPickerBase), new PropertyMetadata("Preview"));

        /// <summary>Identifies the <see cref="LabelHueChannel"/> dependency property.</summary>
        public static readonly DependencyProperty LabelHueChannelProperty = DependencyProperty.Register(nameof(LabelHueChannel), typeof(object), typeof(ColorPickerBase), new PropertyMetadata("H"));

        /// <summary>Identifies the <see cref="LabelSaturationChannel"/> dependency property.</summary>
        public static readonly DependencyProperty LabelSaturationChannelProperty = DependencyProperty.Register(nameof(LabelSaturationChannel), typeof(object), typeof(ColorPickerBase), new PropertyMetadata("S"));

        /// <summary>Identifies the <see cref="LabelValueChannel"/> dependency property.</summary>
        public static readonly DependencyProperty LabelValueChannelProperty = DependencyProperty.Register(nameof(LabelValueChannel), typeof(object), typeof(ColorPickerBase), new PropertyMetadata("V"));

        /// <summary>Identifies the <see cref="LabelColorName"/> dependency property.</summary>
        public static readonly DependencyProperty LabelColorNameProperty = DependencyProperty.Register(nameof(LabelColorName), typeof(object), typeof(ColorPickerBase), new PropertyMetadata("Name"));

        #endregion

        #region ColorProperties

        /// <summary>
        /// Gets or Sets the selected <see cref="Color"/>
        /// </summary>
        public Color? SelectedColor
        {
            get { return (Color?)this.GetValue(SelectedColorProperty); }
            set { this.SetValue(SelectedColorProperty, value); }
        }
        
        /// <summary>
        /// Gets or Sets the selected <see cref="Color"/>
        /// </summary>
        public Color? DefaultColor
        {
            get { return (Color?)this.GetValue(DefaultColorProperty); }
            set { this.SetValue(DefaultColorProperty, value); }
        }


        /// <summary>
        /// Gets the <see cref="SelectedColor"/> as <see cref="HSVColor"/>. This property is read only.
        /// </summary>
        public HSVColor SelectedHSVColor
        {
            get { return (HSVColor)GetValue(SelectedHSVColorProperty); }
        }


        /// <summary>
        /// Gets or sets the ColorName
        /// </summary>
        public string ColorName
        {
            get { return (string)this.GetValue(ColorNameProperty); }
            set { this.SetValue(ColorNameProperty, value); }
        }


        /// <summary>
        /// Gets or sets a <see cref="Dictionary{TKey, TValue}"/> for looking up the ColorName
        /// </summary>
        public Dictionary<Color?, string> ColorNamesDictionary
        {
            get { return (Dictionary<Color?, string>)GetValue(ColorNamesDictionaryProperty); }
            set { SetValue(ColorNamesDictionaryProperty, value); }
        }


        #endregion

        #region Color changed
        /// <summary>Identifies the <see cref="SelectedColorChanged"/> routed event.</summary>
        public static readonly RoutedEvent SelectedColorChangedEvent = EventManager.RegisterRoutedEvent(
                                                                        nameof(SelectedColorChanged),
                                                                        RoutingStrategy.Bubble,
                                                                        typeof(RoutedPropertyChangedEventHandler<Color?>),
                                                                        typeof(ColorPickerBase));

        private static void ColorChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            if (dependencyObject is ColorPickerBase colorPicker)
            {
                colorPicker.OnSelectedColorChanged(e.OldValue as Color?, e.NewValue as Color?);
            }
        }

        internal virtual void OnSelectedColorChanged(Color? OldValue, Color? NewValue)
        {
            // don't do a second update
            if (ColorIsUpdating)
                return;

            ColorIsUpdating = true;

            if (SelectedColor == null && DefaultColor != null)
            {
                SetCurrentValue(SelectedColorProperty, DefaultColor);
            }

            SetCurrentValue(ColorNameProperty, ColorHelper.GetColorName(SelectedColor, ColorNamesDictionary));

            // We just update the following lines if we have a Color.
            if (SelectedColor != null)
            {
                if (UpdateHsvValues)
                {
                    var hsv = new HSVColor((Color)SelectedColor);
                    SetCurrentValue(HueProperty, hsv.Hue);
                    SetCurrentValue(SaturationProperty, hsv.Saturation);
                    SetCurrentValue(ValueProperty, hsv.Value);
                }

                SetCurrentValue(SelectedHSVColorProperty, new HSVColor(A / 255d, Hue, Saturation, Value));

                SetCurrentValue(AProperty, (byte)SelectedColor?.A);
                SetCurrentValue(RProperty, (byte)SelectedColor?.R);
                SetCurrentValue(GProperty, (byte)SelectedColor?.G);
                SetCurrentValue(BProperty, (byte)SelectedColor?.B);
            }

            ColorIsUpdating = false;

            RaiseEvent(new RoutedPropertyChangedEventArgs<Color?>(OldValue, NewValue, SelectedColorChangedEvent));
        }


        private static void ColorNameChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            if (dependencyObject is ColorPickerBase colorPicker)
            {
                if (!colorPicker.ColorIsUpdating)
                {
                    if (string.IsNullOrEmpty(e.NewValue?.ToString()))
                    {
                        colorPicker.SetCurrentValue(SelectedColorProperty, null);
                    }
                    else if (ColorHelper.ColorFromString(e.NewValue?.ToString(), colorPicker.ColorNamesDictionary) is Color color)
                    {
                        colorPicker.SetCurrentValue(SelectedColorProperty, color);
                    }
                    else
                    {
                        throw new InvalidCastException("Cannot convert the given input to a valid color");
                    }
                }
            }
        }

        /// <summary>
        ///     Occurs when the <see cref="SelectedColor" /> property is changed.
        /// </summary>
        public event RoutedPropertyChangedEventHandler<Color?> SelectedColorChanged
        {
            add { AddHandler(SelectedColorChangedEvent, value); }
            remove { RemoveHandler(SelectedColorChangedEvent, value); }
        }
        #endregion


        #region ARGB
        /// <summary>
        /// Gets or Sets the Alpha-Channel
        /// </summary>
        public byte A
        {
            get { return (byte)this.GetValue(AProperty); }
            set { this.SetValue(AProperty, value); }
        }

        /// <summary>
        /// Gets or Sets the Red-Channel
        /// </summary>
        public byte R
        {
            get { return (byte)this.GetValue(RProperty); }
            set { this.SetValue(RProperty, value); }
        }

        /// <summary>
        /// Gets or Sets the Green-Channel
        /// </summary>
        public byte G
        {
            get { return (byte)this.GetValue(GProperty); }
            set { this.SetValue(GProperty, value); }
        }

        /// <summary>
        /// Gets or Sets the Blue-Channel
        /// </summary>
        public byte B
        {
            get { return (byte)this.GetValue(BProperty); }
            set { this.SetValue(BProperty, value); }
        }

        internal static void ColorChannelChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            if (dependencyObject is ColorPickerBase colorPicker && !colorPicker.ColorIsUpdating)
            {
                colorPicker.SetCurrentValue(SelectedColorProperty, Color.FromArgb(colorPicker.A, colorPicker.R, colorPicker.G, colorPicker.B));
            }
        }

        #endregion

        #region HSV
        /// <summary>
        /// Gets or Sets the Hue-Channel
        /// </summary>
        public double Hue
        {
            get { return (double)this.GetValue(HueProperty); }
            set { this.SetValue(HueProperty, value); }
        }

        /// <summary>
        /// Gets or Sets the Saturation-Channel
        /// </summary>
        public double Saturation
        {
            get { return (double)this.GetValue(SaturationProperty); }
            set { this.SetValue(SaturationProperty, value); }
        }

        /// <summary>
        /// Gets or Sets the Value-Channel
        /// </summary>
        public double Value
        {
            get { return (double)this.GetValue(ValueProperty); }
            set { this.SetValue(ValueProperty, value); }
        }

        private static void HSV_Values_Changed(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            if (dependencyObject is ColorPickerBase colorPicker && !colorPicker.ColorIsUpdating)
            {
                var hsv = new HSVColor(colorPicker.A / 255d, colorPicker.Hue, colorPicker.Saturation, colorPicker.Value);

                colorPicker.UpdateHsvValues = false;
                colorPicker.SetCurrentValue(SelectedColorProperty, hsv.ToColor());
                colorPicker.UpdateHsvValues = true;
            }
        }
        #endregion

        #region Labels

        /// <summary>
        /// Gets or Sets the Alpha-Label in the GUI
        /// </summary>
        public object LabelAlphaChannel
        {
            get { return (object)GetValue(LabelAlphaChannelProperty); }
            set { SetValue(LabelAlphaChannelProperty, value); }
        }

        /// <summary>
        /// Gets or Sets the Red-Label in the GUI
        /// </summary>
        public object LabelRedChannel
        {
            get { return (object)GetValue(LabelRedChannelProperty); }
            set { SetValue(LabelRedChannelProperty, value); }
        }

        /// <summary>
        /// Gets or Sets the Green-Label in the GUI
        /// </summary>
        public object LabelGreenChannel
        {
            get { return (object)GetValue(LabelGreenChannelProperty); }
            set { SetValue(LabelGreenChannelProperty, value); }
        }

        /// <summary>
        /// Gets or Sets the Blue-Label in the GUI
        /// </summary>
        public object LabelBlueChannel
        {
            get { return (object)GetValue(LabelBlueChannelProperty); }
            set { SetValue(LabelBlueChannelProperty, value); }
        }



        /// <summary>
        /// Gets or Sets the Preview-Label in the GUI
        /// </summary>
        public object LabelColorPreview
        {
            get { return (object)GetValue(LabelColorPreviewProperty); }
            set { SetValue(LabelColorPreviewProperty, value); }
        }


        /// <summary>
        /// Gets or Sets the Hue-Label in the GUI
        /// </summary>
        public object LabelHueChannel
        {
            get { return (object)GetValue(LabelHueChannelProperty); }
            set { SetValue(LabelHueChannelProperty, value); }
        }

        /// <summary>
        /// Gets or Sets the Saturation-Label in the GUI
        /// </summary>
        public object LabelSaturationChannel
        {
            get { return (object)GetValue(LabelSaturationChannelProperty); }
            set { SetValue(LabelSaturationChannelProperty, value); }
        }

        /// <summary>
        /// Gets or Sets the Value-Label in the GUI
        /// </summary>
        public object LabelValueChannel
        {
            get { return (object)GetValue(LabelValueChannelProperty); }
            set { SetValue(LabelValueChannelProperty, value); }
        }

        /// <summary>
        /// Gets or Sets the ColorName-Label in the GUI
        /// </summary>
        public object LabelColorName
        {
            get { return (object)GetValue(LabelColorNameProperty); }
            set { SetValue(LabelColorNameProperty, value); }
        }
        
        #endregion

    }
}
