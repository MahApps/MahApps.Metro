namespace MahApps.Metro.Controls
{
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;

    public class SliderHelper
    {
        public static readonly DependencyProperty ChangeValueByProperty = DependencyProperty.RegisterAttached(
            "ChangeValueBy",
            typeof(MouseWheelChange),
            typeof(SliderHelper),
            new PropertyMetadata(MouseWheelChange.SmallChange));
        public static readonly DependencyProperty EnableMouseWheelProperty = DependencyProperty.RegisterAttached(
            "EnableMouseWheel",
            typeof(MouseWheelState),
            typeof(SliderHelper),
            new PropertyMetadata(MouseWheelState.None, OnEnableMouseWheelChanged));
        
        public static MouseWheelChange GetChangeValueBy(DependencyObject element)
        {
            return (MouseWheelChange)element.GetValue(ChangeValueByProperty);
        }

        public static MouseWheelState GetEnableMouseWheel(DependencyObject element)
        {
            return (MouseWheelState)element.GetValue(EnableMouseWheelProperty);
        }

        private static void OnEnableMouseWheelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var slider = ((Slider)d);
            if ((MouseWheelState)e.NewValue != MouseWheelState.None)
            {
                slider.Unloaded += OnUnloaded;
                slider.PreviewMouseWheel += OnPreviewMouseWheel;
            }
            else
            {
                UnregisterEvents(slider);
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
            }
        }

        private static void OnUnloaded(object sender, RoutedEventArgs e)
        {
            UnregisterEvents((Slider)sender);
        }

        public static void SetChangeValueBy(DependencyObject element, MouseWheelChange value)
        {
            element.SetValue(ChangeValueByProperty, value);
        }

        public static void SetEnableMouseWheel(DependencyObject element, MouseWheelState value)
        {
            element.SetValue(EnableMouseWheelProperty, value);
        }

        private static void UnregisterEvents(Slider slider)
        {
            slider.Unloaded -= OnUnloaded;
            slider.PreviewMouseWheel -= OnPreviewMouseWheel;
        }
    }
}
