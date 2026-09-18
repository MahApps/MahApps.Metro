// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows.Controls;
using MahApps.Metro.Controls;

namespace MahApps.Metro.Gallery.Pages
{
    /// <summary>
    /// Interaction logic for DropDownButtonPage.xaml
    /// </summary>
    public partial class DropDownButtonPage : UserControl
    {
        public DropDownButtonPage()
        {
            this.InitializeComponent();

            this.DropDownExample.Watch(this.DropDown,
                                       DropDownButton.ContentProperty,
                                       DropDownButton.ArrowVisibilityProperty,
                                       IsEnabledProperty);
            this.DropDownExample.Watch("Layout", this.DropDown, WidthProperty);

        }
    }
}
