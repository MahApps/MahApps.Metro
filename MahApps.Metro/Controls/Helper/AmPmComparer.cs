namespace MahApps.Metro.Controls
{
    using System.Collections.Generic;

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