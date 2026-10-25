// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using MahApps.Metro.Controls;
using NUnit.Framework;

namespace MahApps.Metro.Tests.TestHelpers
{
    /// <summary>
    /// What a test does instead of the mouse and the keyboard it does not have.
    /// </summary>
    public static class FakeInput
    {
        // IsMouseOver reads the mouse, and a test has no mouse to put anywhere. Both of these are
        // read only to the outside and carry a key their own class keeps, which is the one handle
        // there is on the state the triggers are written against.
        public static readonly DependencyPropertyKey MouseOverKey = KeyOf(typeof(UIElement), "IsMouseOverPropertyKey");
        public static readonly DependencyPropertyKey SelectionActiveKey = KeyOf(typeof(Selector), "IsSelectionActivePropertyKey");

        private static DependencyPropertyKey KeyOf(Type owner, string field)
        {
            return (DependencyPropertyKey)owner.GetField(field, BindingFlags.Static | BindingFlags.NonPublic)!.GetValue(null)!;
        }

        public static void PutTheMouseOn(DataGridCell cell)
        {
            cell.GetVisualAncestor<DataGridRow>()?.SetValue(MouseOverKey, true);
            cell.SetValue(MouseOverKey, true);
            ClipAssert.Pump();
        }

        /// <summary>
        /// Puts the keyboard focus on the control the way a tab would, which is what the helper drawing
        /// the ring tells apart from a click: the last input before the focus came from the keyboard.
        /// </summary>
        public static void FocusWithTheKeyboard(Control control)
        {
            // a test has no keys to press, so it tells the input manager what a press would have told it
            typeof(InputManager).GetProperty(nameof(InputManager.MostRecentInputDevice))!.GetSetMethod(true)!.Invoke(InputManager.Current, new object[] { Keyboard.PrimaryDevice });
            Keyboard.Focus(control);
            Window.GetWindow(control).Settle();

            // a build agent hands the keyboard to one window at a time
            Assume.That(control.IsKeyboardFocusWithin, Is.True);
        }
    }
}
