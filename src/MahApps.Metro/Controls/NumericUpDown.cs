// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows;

namespace MahApps.Metro.Controls
{
    /// <summary>
    /// A control for entering a <see cref="double"/>, with buttons to step it up and down.
    /// </summary>
    /// <remarks>
    /// This is the control MahApps has always had, and it holds its value as a <see cref="double"/>.
    /// A double cannot hold 0.1 exactly, and the error shows up once such values are added up, so a
    /// price or an amount of money is better off in a control that keeps every digit it was given.
    /// </remarks>
    public class NumericUpDown : NumericUpDownBase
    {
        static NumericUpDown()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(NumericUpDown), new FrameworkPropertyMetadata(typeof(NumericUpDown)));
        }
    }
}
