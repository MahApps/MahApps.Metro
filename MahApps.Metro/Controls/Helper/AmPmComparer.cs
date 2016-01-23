namespace MahApps.Metro.Controls
{
    using System.Collections.Generic;

    /// <summary>
    /// Represents an hour comparison operation that ensures that 12 is smaller than 1.
    /// This ensures that in the <see cref="DateTimePicker"/> control the first hour that is selectable is 12 (AM/PM). 
    /// </summary>
    ///<remarks>This ensures that the first hour that is selectable is 12 (AM/PM). <br></br>
    /// This comparer is used only if in the corresponding <see cref="DateTimePicker"/> the value for <see cref="DateTimePicker.IsMilitaryTime"/> is false.</remarks>
    /// 
    internal class AmPmComparer : IComparer<int>
    {
        public int Compare(int x, int y)
        {
            if (x == 12 && y == 12)
            {
                return 0;
            }
            if (x == 12)
            {
                return -1;
            }
            if (y == 12)
            {
                return 1;
            }
            return x.CompareTo(y);
        }
    }
}