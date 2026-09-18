// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;

namespace MetroDemo.Core
{
    /// <summary>
    /// What the buttons in the template of a XamlDisplay are wired to.
    /// </summary>
    public static class XamlDisplayCommands
    {
        /// <summary>
        /// Puts the shown XAML on the clipboard, which is the whole point of showing it.
        /// </summary>
        public static ICommand CopyXaml { get; } = new SimpleCommand<string>(
            xaml => !string.IsNullOrWhiteSpace(xaml),
            xaml =>
                {
                    try
                    {
                        Clipboard.SetDataObject(xaml, true);
                    }
                    catch (Exception exception)
                    {
                        // another program can have the clipboard open, and losing a copy is no
                        // reason to take the demo down with it
                        Debug.WriteLine(exception);
                    }
                });
    }
}
