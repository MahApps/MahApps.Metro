// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;

namespace MahApps.Metro.Controls
{
    /// <summary>
    /// The arguments of <see cref="ContentDialog.Closing"/>.
    /// </summary>
    public class ContentDialogClosingEventArgs : CancelEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ContentDialogClosingEventArgs"/> class.
        /// </summary>
        /// <param name="result">What the dialog closes with.</param>
        public ContentDialogClosingEventArgs(ContentDialogResult result)
        {
            this.Result = result;
        }

        /// <summary>
        /// Gets what the dialog closes with.
        /// </summary>
        public ContentDialogResult Result { get; }
    }
}
