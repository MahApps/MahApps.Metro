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
        public OwningWindowProbeDialog()
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
