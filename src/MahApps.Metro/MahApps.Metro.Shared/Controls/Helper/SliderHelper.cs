using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace MahApps.Metro.Controls
{
    public class SliderHelper
    {
        /// <summary>
        /// Gets or sets the brush of the thumb.
        /// </summary>
        public static readonly DependencyProperty ThumbFillBrushProperty
            = DependencyProperty.RegisterAttached("ThumbFillBrush",
                                                  typeof(Brush),
                                                  typeof(SliderHelper),
                                                  new FrameworkPropertyMetadata(default(Brush), FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.Inherits));

        /// <summary>
        /// Gets the brush of the thumb.
        /// </summary>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(Slider))]
        [AttachedPropertyBrowsableForType(typeof(RangeSlider))]
        public static Brush GetThumbFillBrush(UIElement element)
        {
            return (Brush)element.GetValue(ThumbFillBrushProperty);
        }

        /// <summary>
        /// Sets the brush of the thumb.
        /// </summary>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(Slider))]
        [AttachedPropertyBrowsableForType(typeof(RangeSlider))]
        public static void SetThumbFillBrush(UIElement element, Brush value)
        {
            element.SetValue(ThumbFillBrushProperty, value);
        }

        /// <summary>
        /// Gets or sets the brush of the thumb if the mouse is over the slider.
        /// </summary>
        public static readonly DependencyProperty ThumbFillHoverBrushProperty
            = DependencyProperty.RegisterAttached("ThumbFillHoverBrush",
                                                  typeof(Brush),
                                                  typeof(SliderHelper),
                                                  new FrameworkPropertyMetadata(default(Brush), FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.Inherits));

        /// <summary>
        /// Gets the brush of the thumb if the mouse is over the slider.
        /// </summary>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(Slider))]
        [AttachedPropertyBrowsableForType(typeof(RangeSlider))]
        public static Brush GetThumbFillHoverBrush(UIElement element)
        {
            return (Brush)element.GetValue(ThumbFillHoverBrushProperty);
        }

        /// <summary>
        /// Sets the brush of the thumb if the mouse is over the slider.
        /// </summary>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(Slider))]
        [AttachedPropertyBrowsableForType(typeof(RangeSlider))]
        public static void SetThumbFillHoverBrush(UIElement element, Brush value)
        {
            element.SetValue(ThumbFillHoverBrushProperty, value);
        }

        /// <summary>
        /// Gets or sets the brush of the thumb if the mouse button is pressed.
        /// </summary>
        public static readonly DependencyProperty ThumbFillPressedBrushProperty
            = DependencyProperty.RegisterAttached("ThumbFillPressedBrush",
                                                  typeof(Brush),
                                                  typeof(SliderHelper),
                                                  new FrameworkPropertyMetadata(default(Brush), FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.Inherits));

        /// <summary>
        /// Gets the brush of the thumb if the mouse button is pressed.
        /// </summary>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(Slider))]
        [AttachedPropertyBrowsableForType(typeof(RangeSlider))]
        public static Brush GetThumbFillPressedBrush(UIElement element)
        {
            return (Brush)element.GetValue(ThumbFillPressedBrushProperty);
        }

        /// <summary>
        /// Sets the brush of the thumb if the mouse button is pressed.
        /// </summary>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(Slider))]
        [AttachedPropertyBrowsableForType(typeof(RangeSlider))]
        public static void SetThumbFillPressedBrush(UIElement element, Brush value)
        {
            element.SetValue(ThumbFillPressedBrushProperty, value);
        }

        /// <summary>
        /// Gets or sets the brush of the thumb if the slider is disabled.
        /// </summary>
        public static readonly DependencyProperty ThumbFillDisabledBrushProperty
            = DependencyProperty.RegisterAttached("ThumbFillDisabledBrush",
                                                  typeof(Brush),
                                                  typeof(SliderHelper),
                                                  new FrameworkPropertyMetadata(default(Brush), FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.Inherits));

        /// <summary>
        /// Gets the brush of the thumb if the slider is disabled.
        /// </summary>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(Slider))]
        [AttachedPropertyBrowsableForType(typeof(RangeSlider))]
        public static Brush GetThumbFillDisabledBrush(UIElement element)
        {
            return (Brush)element.GetValue(ThumbFillDisabledBrushProperty);
        }

        /// <summary>
        /// Sets the brush of the thumb if the slider is disabled.
        /// </summary>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(Slider))]
        [AttachedPropertyBrowsableForType(typeof(RangeSlider))]
        public static void SetThumbFillDisabledBrush(UIElement element, Brush value)
        {
            element.SetValue(ThumbFillDisabledBrushProperty, value);
        }

        /// <summary>
        /// Gets or sets the brush of the track.
        /// </summary>
        public static readonly DependencyProperty TrackFillBrushProperty
            = DependencyProperty.RegisterAttached("TrackFillBrush",
                                                  typeof(Brush),
                                                  typeof(SliderHelper),
                                                  new FrameworkPropertyMetadata(default(Brush), FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.Inherits));

        /// <summary>
        /// Gets the brush of the track.
        /// </summary>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(Slider))]
        [AttachedPropertyBrowsableForType(typeof(RangeSlider))]
        public static Brush GetTrackFillBrush(UIElement element)
        {
            return (Brush)element.GetValue(TrackFillBrushProperty);
        }

        /// <summary>
        /// Sets the brush of the track.
        /// </summary>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(Slider))]
        [AttachedPropertyBrowsableForType(typeof(RangeSlider))]
        public static void SetTrackFillBrush(UIElement element, Brush value)
        {
            element.SetValue(TrackFillBrushProperty, value);
        }

        /// <summary>
        /// Gets or sets the brush of the track if the mouse is over the slider.
        /// </summary>
        public static readonly DependencyProperty TrackFillHoverBrushProperty
            = DependencyProperty.RegisterAttached("TrackFillHoverBrush",
                                                  typeof(Brush),
                                                  typeof(SliderHelper),
                                                  new FrameworkPropertyMetadata(default(Brush), FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.Inherits));

        /// <summary>
        /// Gets the brush of the track if the mouse is over the slider.
        /// </summary>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(Slider))]
        [AttachedPropertyBrowsableForType(typeof(RangeSlider))]
        public static Brush GetTrackFillHoverBrush(UIElement element)
        {
            return (Brush)element.GetValue(TrackFillHoverBrushProperty);
        }

        /// <summary>
        /// Sets the brush of the track if the mouse is over the slider.
        /// </summary>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(Slider))]
        [AttachedPropertyBrowsableForType(typeof(RangeSlider))]
        public static void SetTrackFillHoverBrush(UIElement element, Brush value)
        {
            element.SetValue(TrackFillHoverBrushProperty, value);
        }

        /// <summary>
        /// Gets or sets the brush of the track if the mouse button is pressed.
        /// </summary>
        public static readonly DependencyProperty TrackFillPressedBrushProperty
            = DependencyProperty.RegisterAttached("TrackFillPressedBrush",
                                                  typeof(Brush),
                                                  typeof(SliderHelper),
                                                  new FrameworkPropertyMetadata(default(Brush), FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.Inherits));

        /// <summary>
        /// Gets the brush of the track if the mouse button is pressed.
        /// </summary>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(Slider))]
        [AttachedPropertyBrowsableForType(typeof(RangeSlider))]
        public static Brush GetTrackFillPressedBrush(UIElement element)
        {
            return (Brush)element.GetValue(TrackFillPressedBrushProperty);
        }

        /// <summary>
        /// Sets the brush of the track if the mouse button is pressed.
        /// </summary>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(Slider))]
        [AttachedPropertyBrowsableForType(typeof(RangeSlider))]
        public static void SetTrackFillPressedBrush(UIElement element, Brush value)
        {
            element.SetValue(TrackFillPressedBrushProperty, value);
        }

        /// <summary>
        /// Gets or sets the brush of the track if the slider is disabled.
        /// </summary>
        public static readonly DependencyProperty TrackFillDisabledBrushProperty
            = DependencyProperty.RegisterAttached("TrackFillDisabledBrush",
                                                  typeof(Brush),
                                                  typeof(SliderHelper),
                                                  new FrameworkPropertyMetadata(default(Brush), FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.Inherits));

        /// <summary>
        /// Gets the brush of the track if the slider is disabled.
        /// </summary>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(Slider))]
        [AttachedPropertyBrowsableForType(typeof(RangeSlider))]
        public static Brush GetTrackFillDisabledBrush(UIElement element)
        {
            return (Brush)element.GetValue(TrackFillDisabledBrushProperty);
        }

        /// <summary>
        /// Sets the brush of the track if the slider is disabled.
        /// </summary>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(Slider))]
        [AttachedPropertyBrowsableForType(typeof(RangeSlider))]
        public static void SetTrackFillDisabledBrush(UIElement element, Brush value)
        {
            element.SetValue(TrackFillDisabledBrushProperty, value);
        }

        /// <summary>
        /// Gets or sets the brush of the track value.
        /// </summary>
        public static readonly DependencyProperty TrackValueFillBrushProperty
            = DependencyProperty.RegisterAttached("TrackValueFillBrush",
                                                  typeof(Brush),
                                                  typeof(SliderHelper),
                                                  new FrameworkPropertyMetadata(default(Brush), FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.Inherits));

        /// <summary>
        /// Gets the brush of the track value.
        /// </summary>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(Slider))]
        [AttachedPropertyBrowsableForType(typeof(RangeSlider))]
        public static Brush GetTrackValueFillBrush(UIElement element)
        {
            return (Brush)element.GetValue(TrackValueFillBrushProperty);
        }

        /// <summary>
        /// Sets the brush of the track value.
        /// </summary>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(Slider))]
        [AttachedPropertyBrowsableForType(typeof(RangeSlider))]
        public static void SetTrackValueFillBrush(UIElement element, Brush value)
        {
            element.SetValue(TrackValueFillBrushProperty, value);
        }

        /// <summary>
        /// Gets or sets the brush of the track value if the mouse is over the slider.
        /// </summary>
        public static readonly DependencyProperty TrackValueFillHoverBrushProperty
            = DependencyProperty.RegisterAttached("TrackValueFillHoverBrush",
                                                  typeof(Brush),
                                                  typeof(SliderHelper),
                                                  new FrameworkPropertyMetadata(default(Brush), FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.Inherits));

        /// <summary>
        /// Gets the brush of the track value if the mouse is over the slider.
        /// </summary>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(Slider))]
        [AttachedPropertyBrowsableForType(typeof(RangeSlider))]
        public static Brush GetTrackValueFillHoverBrush(UIElement element)
        {
            return (Brush)element.GetValue(TrackValueFillHoverBrushProperty);
        }

        /// <summary>
        /// Sets the brush of the track value if the mouse is over the slider.
        /// </summary>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(Slider))]
        [AttachedPropertyBrowsableForType(typeof(RangeSlider))]
        public static void SetTrackValueFillHoverBrush(UIElement element, Brush value)
        {
            element.SetValue(TrackValueFillHoverBrushProperty, value);
        }

        /// <summary>
        /// Gets or sets the brush of the track value if the mouse button is pressed.
        /// </summary>
        public static readonly DependencyProperty TrackValueFillPressedBrushProperty
            = DependencyProperty.RegisterAttached("TrackValueFillPressedBrush",
                                                  typeof(Brush),
                                                  typeof(SliderHelper),
                                                  new FrameworkPropertyMetadata(default(Brush), FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.Inherits));

        /// <summary>
        /// Gets the brush of the track value if the mouse button is pressed.
        /// </summary>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(Slider))]
        [AttachedPropertyBrowsableForType(typeof(RangeSlider))]
        public static Brush GetTrackValueFillPressedBrush(UIElement element)
        {
            return (Brush)element.GetValue(TrackValueFillPressedBrushProperty);
        }

        /// <summary>
        /// Sets the brush of the track value if the mouse button is pressed.
        /// </summary>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(Slider))]
        [AttachedPropertyBrowsableForType(typeof(RangeSlider))]
        public static void SetTrackValueFillPressedBrush(UIElement element, Brush value)
        {
            element.SetValue(TrackValueFillPressedBrushProperty, value);
        }

        /// <summary>
        /// Gets or sets the brush of the track value if the slider is disabled.
        /// </summary>
        public static readonly DependencyProperty TrackValueFillDisabledBrushProperty
            = DependencyProperty.RegisterAttached("TrackValueFillDisabledBrush",
                                                  typeof(Brush),
                                                  typeof(SliderHelper),
                                                  new FrameworkPropertyMetadata(default(Brush), FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.Inherits));

        /// <summary>
        /// Gets the brush of the track value if the slider is disabled.
        /// </summary>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(Slider))]
        [AttachedPropertyBrowsableForType(typeof(RangeSlider))]
        public static Brush GetTrackValueFillDisabledBrush(UIElement element)
        {
            return (Brush)element.GetValue(TrackValueFillDisabledBrushProperty);
        }

        /// <summary>
        /// Sets the brush of the track value if the slider is disabled.
        /// </summary>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(Slider))]
        [AttachedPropertyBrowsableForType(typeof(RangeSlider))]
        public static void SetTrackValueFillDisabledBrush(UIElement element, Brush value)
        {
            element.SetValue(TrackValueFillDisabledBrushProperty, value);
        }

        public static readonly DependencyProperty ChangeValueByProperty
            = DependencyProperty.RegisterAttached("ChangeValueBy",
                                                  typeof(MouseWheelChange),
                                                  typeof(SliderHelper),
                                                  new PropertyMetadata(MouseWheelChange.SmallChange));

        /// <summary>
        /// Gets/Sets the type how the value will be changed if the user rotates the mouse wheel.
        /// </summary>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(Slider))]
        public static MouseWheelChange GetChangeValueBy(Slider element)
        {
            return (MouseWheelChange)element.GetValue(ChangeValueByProperty);
        }

        /// <summary>
        /// Gets/Sets the type how the value will be changed if the user rotates the mouse wheel.
        /// </summary>
        public static void SetChangeValueBy(Slider element, MouseWheelChange value)
        {
            element.SetValue(ChangeValueByProperty, value);
        }

        public static readonly DependencyProperty EnableMouseWheelProperty
            = DependencyProperty.RegisterAttached("EnableMouseWheel",
                                                  typeof(MouseWheelState),
                                                  typeof(SliderHelper),
                                                  new PropertyMetadata(MouseWheelState.None, OnEnableMouseWheelChanged));

        /// <summary>
        /// Gets/Sets the value when the slider will be changed. Possible values are if the slider is focused or if the mouse is over the slider.
        /// </summary>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(Slider))]
        public static MouseWheelState GetEnableMouseWheel(Slider element)
        {
            return (MouseWheelState)element.GetValue(EnableMouseWheelProperty);
        }

        /// <summary>
        /// Gets/Sets the value when the slider will be changed. Possible values are if the slider is focused or if the mouse is over the slider.
        /// </summary>
        public static void SetEnableMouseWheel(Slider element, MouseWheelState value)
        {
            element.SetValue(EnableMouseWheelProperty, value);
        }

        private static void OnEnableMouseWheelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != e.OldValue)
            {
                var slider = d as Slider;
                if (slider != null)
                {
                    slider.PreviewMouseWheel -= OnPreviewMouseWheel;
                    if ((MouseWheelState)e.NewValue != MouseWheelState.None)
                    {
                        slider.PreviewMouseWheel += OnPreviewMouseWheel;
                    }
                }
            }
        }

        private static void OnPreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            var slider = ((Slider)sender);
            if (slider.IsFocused || MouseWheelState.MouseHover.Equals(slider.GetValue(EnableMouseWheelProperty)))
            {
                var changeType = (MouseWheelChange)slider.GetValue(ChangeValueByProperty);
                var difference = changeType == MouseWheelChange.LargeChange ? slider.LargeChange : slider.SmallChange;

                if (e.Delta > 0)
                {
                    slider.Value += difference;
                }
                else
                {
                    slider.Value -= difference;
                }

                e.Handled = true;
            }
        }
    }
}