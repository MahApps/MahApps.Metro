// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace MahApps.Metro.ValueBoxes
{
    /// <summary>
    /// Helps boxing Boolean values.
    /// </summary>
    public static class BooleanBoxes
    {
        /// <summary>
        /// Gets a boxed representation for <see cref="bool"/> "true" value.
        /// </summary>
        public static readonly object TrueBox = true;

        /// <summary>
        /// Gets a boxed representation for <see cref="bool"/> "false" value.
        /// </summary>
        public static readonly object FalseBox = false;

        /// <summary>
        /// Returns a boxed representation for the specified Boolean value.
        /// </summary>
        /// <param name="value">The value to box.</param>
        /// <returns>A boxed <see cref="bool"/> value.</returns>
        public static object Box(bool value) => value ? TrueBox : FalseBox;

        /// <summary>
        /// Returns a boxed value for the specified nullable <paramref name="value"/>.
        /// </summary>
        /// <returns>A boxed nullable <see cref="bool"/> value.</returns>
        public static object? Box(bool? value)
        {
            if (value.HasValue)
            {
                return value.Value
                    ? TrueBox
                    : FalseBox;
            }

            return null;
        }
    }
}