// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Threading.Tasks;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;

namespace MahApps.Metro.Tests.Views
{
    /// <summary>
    /// A dialog the way a caller writes one, in code or in XAML. It reaches its window through
    /// <see cref="BaseMetroDialog.OwningWindow"/>, which is protected and therefore only available
    /// from within a dialog of your own.
    /// </summary>
    public class OwningWindowProbeDialog : CustomDialog
    {
        /// <summary>
        /// A dialog nobody handed a window, which is the case these tests are about.
        /// </summary>
        /// <remarks>
        /// The animations are off, and they have to be said here: a dialog built without settings
        /// keeps a set of its own, and showing it on a window does not hand it the ones of that
        /// window. Left on, hiding it waits for a closing storyboard to run to its end, and on a
        /// build agent that clock does not always tick.
        /// </remarks>
        public OwningWindowProbeDialog()
            : base(null, new MetroDialogSettings { AnimateShow = false, AnimateHide = false })
        {
        }

        public OwningWindowProbeDialog(MetroWindow? owningWindow, MetroDialogSettings? settings)
            : base(owningWindow, settings)
        {
        }

        public MetroWindow? Owner => this.OwningWindow;

        public Task CloseItselfAsync()
        {
            return this.OwningWindow!.HideMetroDialogAsync(this);
        }
    }
}
