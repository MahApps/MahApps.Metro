// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows.Input;
using Microsoft.Xaml.Behaviors.Core;

namespace MetroDemo.ExampleWindows
{
    public partial class VSDemo
    {
        public VSDemo()
        {
            this.InitializeComponent();
        }

        public ICommand QuickLaunchBarFocusCommand => new ActionCommand(this.FocusQuickLaunchBar);

        private void FocusQuickLaunchBar()
        {
            this.QuickLaunchBar.Focus();
        }
    }
}