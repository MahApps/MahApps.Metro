// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

namespace MahApps.Metro.Controls
{
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