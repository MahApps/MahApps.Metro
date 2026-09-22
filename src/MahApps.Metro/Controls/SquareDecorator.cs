// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Windows.Controls;

namespace MahApps.Metro.Controls
{
    /// <summary>
    /// Asks for as much width as its child needs height, so a round thing stays round and a wider
    /// one keeps its own width.
    /// </summary>
    /// <remarks>
    /// The obvious way to write this is a binding from MinWidth to ActualHeight, and that is a
    /// layout property fed by a layout result. It settles as long as nothing else moves, and it
    /// oscillates as soon as something does: every pass hands out a width, the width moves the box,
    /// the move changes what the height rounds to, and the next pass hands out a different width.
    /// Measuring the child once and answering from that is the same shape with no way back.
    /// </remarks>
    public class SquareDecorator : Decorator
    {
        /// <inheritdoc />
        protected override Size MeasureOverride(Size constraint)
        {
            var child = this.Child;
            if (child is null)
            {
                return default;
            }

            child.Measure(constraint);

            var desired = child.DesiredSize;
            return new Size(Math.Max(desired.Width, desired.Height), desired.Height);
        }
    }
}
