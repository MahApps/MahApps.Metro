// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;

namespace MahApps.Metro.Controls
{
    /// <summary>
    /// Represents an hour comparison operation that ensures that 12 is smaller than 1.
    /// This ensures that in the <see cref="TimePickerBase"/> control the first hour that is selectable is 12 (AM/PM). 
    /// </summary>
    ///<remarks>
    /// This ensures that the first hour that is selectable is 12 (AM/PM). <br></br>
    /// This comparer is used only if in the corresponding <see cref="TimePickerBase"/> the value for <see cref="TimePickerBase.IsMilitaryTime"/> is false.
    /// </remarks>
    internal class AmPmComparer : IComparer<int>
    {
        public int Compare(int x, int y)
        {
            if (x == 12 && y == 12)
            {
                return 0;
            }
            else if (x == 12)
            {
                return -1;
            }
            else if (y == 12)
            {
                return 1;
            }

            return x.CompareTo(y);
        }
    }
}