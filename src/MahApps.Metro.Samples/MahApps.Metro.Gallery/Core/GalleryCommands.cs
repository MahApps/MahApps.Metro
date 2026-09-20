// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;

namespace MahApps.Metro.Gallery.Core
{
    /// <summary>
    /// What the buttons in the templates of the gallery are wired to.
    /// </summary>
    public static class GalleryCommands
    {
        /// <summary>
        /// Opens a link in whatever the reader browses with.
        /// </summary>
        public static ICommand OpenLink { get; } = new RelayCommand(
            parameter => Process.Start(new ProcessStartInfo(parameter!.ToString()!) { UseShellExecute = true }),
            parameter => !string.IsNullOrWhiteSpace(parameter?.ToString()));

        /// <summary>
        /// Puts text on the clipboard, which for the XAML of a sample is the point of showing it.
        /// </summary>
        public static ICommand CopyText { get; } = new RelayCommand(
            parameter =>
                {
                    try
                    {
                        Clipboard.SetDataObject(parameter!.ToString(), true);
                    }
                    catch (Exception exception)
                    {
                        // another program can have the clipboard open, and losing a copy is no
                        // reason to take the gallery down with it
                        Debug.WriteLine(exception);
                    }
                },
            parameter => !string.IsNullOrWhiteSpace(parameter?.ToString()));
    }
}
