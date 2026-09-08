// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

namespace MahApps.Metro.Controls
{
    /// <summary>
    /// Comparing two doubles for equality fails on values that only differ by the last bit or two,
    /// which is what arithmetic on them leaves behind. These comparisons allow for that.
    /// </summary>
    public static class DoubleUtil
    {
        /// <summary>
        /// The smallest double where 1.0 + Epsilon is not 1.0 any more.
        /// </summary>
        private const double Epsilon = 2.2204460492503131e-016;

        /// <summary>
        /// Gets whether two doubles are close enough to each other to count as equal. How close that is
        /// grows with the values themselves, so the comparison holds up at any magnitude.
        /// </summary>
        /// <param name="value1">The first double to compare.</param>
        /// <param name="value2">The second double to compare.</param>
        public static bool AreClose(double value1, double value2)
        {
            // The epsilon check below cannot answer for infinities: subtracting one from itself gives
            // NaN, and every comparison against NaN is false. So they are answered here, on whether
            // they are the same value, which for two infinities means the same sign.
            if (double.IsInfinity(value1) || double.IsInfinity(value2))
            {
                return value1.CompareTo(value2) == 0;
            }

            // This computes (|value1-value2| / (|value1| + |value2| + 10.0)) < Epsilon
            var eps = (Math.Abs(value1) + Math.Abs(value2) + 10.0) * Epsilon;
            var delta = value1 - value2;

            return -eps < delta && eps > delta;
        }
    }
}
