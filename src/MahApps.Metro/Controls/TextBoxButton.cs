// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows;
using System.Windows.Controls;

namespace MahApps.Metro.Controls
{
    /// <summary>
    /// A button for the <see cref="TextBoxHelper.ButtonsProperty"/> of a text control. It is a
    /// <see cref="Button"/> and nothing more, so everything a button has is there: content,
    /// command, tool tip, visibility. What it brings of its own is a style that takes the width,
    /// the font and the chrome from the control it stands in, so a button beside a Windows 10 box
    /// is drawn the way that set draws the clear button.
    /// </summary>
    /// <remarks>
    /// Pressing one leaves the caret in the box, the way it does for the clear button and for every
    /// other button a text control draws: the Windows 10 and the WinUI box show their clear button
    /// only while the box has the keyboard. Say Focusable and IsTabStop on a button to put it in the
    /// tab order instead.
    /// </remarks>
    public class TextBoxButton : Button
    {
        static TextBoxButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TextBoxButton), new FrameworkPropertyMetadata(typeof(TextBoxButton)));
        }
    }
}
