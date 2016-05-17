namespace MahApps.Metro.Controls
{
    using System;
    using System.Windows;

    public class TimePicker : TimePartPickerBase<TimeSpan?>
    {
        static TimePicker()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TimePicker), new FrameworkPropertyMetadata(typeof(TimePicker)));
        }
    }
}