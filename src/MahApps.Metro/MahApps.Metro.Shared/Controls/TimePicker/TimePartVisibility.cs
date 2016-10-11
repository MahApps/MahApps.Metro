namespace MahApps.Metro.Controls
{
    using System;

    /// <summary>
    /// Defines the visibility for time-parts that are visible for the <see cref="DateTimePicker"/>. 
    /// </summary>
    [Flags]
    public enum TimePartVisibility
    {
        Hour = 1 << 1,
        Minute = 1 << 2,
        Second = 1 << 3,
        HourMinute = Hour | Minute,
        All = HourMinute | Second
    }
}