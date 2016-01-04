namespace MahApps.Metro.Controls
{
    using System;

    [Flags]
    public enum DatePartVisibility
    {
        Hour = 1 << 1,
        Minute = 1 << 2,
        Second = 1 << 3,
        HourMinute = Hour | Minute,
        All = HourMinute | Second
    }
}