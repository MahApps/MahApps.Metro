namespace MahApps.Metro.Controls
{
    using System;
    using System.Windows;
    using System.Windows.Data;

    [ValueConversion(typeof(TimeSpan?), typeof(string))]
    public class TimePicker : TimePartPickerBase
    {
        static TimePicker()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TimePicker), new FrameworkPropertyMetadata(typeof(TimePicker)));
            IsDatePickerVisibleProperty.OverrideMetadata(typeof(TimePicker), new PropertyMetadata(false), IsDatePickerVisiblePropertyKey);
        }
    }
}