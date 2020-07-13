// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows.Controls;

namespace MetroDemo.ExampleViews
{
    using MahApps.Metro.Controls;

    public sealed partial class HamburgerMenuCreatorsUpdate : UserControl
    {
        public HamburgerMenuCreatorsUpdate()
        {
            this.InitializeComponent();
        }

        private void HamburgerMenuControl_OnItemInvoked(object sender, HamburgerMenuItemInvokedEventArgs e)
        {
            HamburgerMenuControl.Content = e.InvokedItem;
        }
    }
}