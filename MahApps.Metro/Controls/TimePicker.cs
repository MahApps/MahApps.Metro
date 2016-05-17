namespace MahApps.Metro.Controls
{
    using System.Windows;

    public class TimePicker : TimePartPickerBase
    {
        static TimePicker()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TimePicker), new FrameworkPropertyMetadata(typeof(TimePicker)));
            DatePickerVisibilityProperty.OverrideMetadata(typeof(TimePicker), new FrameworkPropertyMetadata(Visibility.Collapsed), DatePickerVisibilityPropertyKey);
        }
    }
}