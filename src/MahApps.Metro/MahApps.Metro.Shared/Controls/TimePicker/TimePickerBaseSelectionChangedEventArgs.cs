namespace MahApps.Metro.Controls
{
    using System.Windows;

    public class TimePickerBaseSelectionChangedEventArgs<T> : RoutedEventArgs
    {
        public TimePickerBaseSelectionChangedEventArgs(RoutedEvent eventId, T oldValue, T newValue) :
            base(eventId)
        {
            this.OldValue = oldValue;
            this.NewValue = newValue;
        }

        public T OldValue { get; }
        public T NewValue { get; }
    }
}