// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace MahApps.Metro.Converters
{
    public enum RadiusType
    {
        /// <summary>
        /// Use the radius of all corners.
        /// </summary>
        None,
        /// <summary>
        /// Ignore the radius of the top-left and bottom-left corner.
        /// </summary>
        Left,
        /// <summary>
        /// Ignore the radius of the top-left and top-right corner.
        /// </summary>
        Top,
        /// <summary>
        /// Ignore the radius of the top-right and bottom-right corner.
        /// </summary>
        Right,
        /// <summary>
        /// Ignore the radius of the bottom-left and bottom-right corner.
        /// </summary>
        Bottom,
        /// <summary>
        /// Ignore the radius of the top-left corner.
        /// </summary>
        TopLeft,
        /// <summary>
        /// Ignore radius of the top-right corner.
        /// </summary>
        TopRight,
        /// <summary>
        /// Ignore the radius of the bottom-right corner.
        /// </summary>
        BottomRight,
        /// <summary>
        /// Ignore the radius of the bottom-left corner.
        /// </summary>
        BottomLeft
    }
}