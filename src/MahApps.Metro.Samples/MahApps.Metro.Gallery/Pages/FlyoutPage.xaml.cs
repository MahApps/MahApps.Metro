// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows.Controls;
using MahApps.Metro.Controls;

namespace MahApps.Metro.Gallery.Pages
{
    /// <summary>
    /// Interaction logic for FlyoutPage.xaml
    /// </summary>
    public partial class FlyoutPage : UserControl
    {
        public FlyoutPage()
        {
            this.InitializeComponent();

            this.FlyoutExample.Watch(this.Side,
                                     Flyout.IsOpenProperty,
                                     Flyout.PositionProperty,
                                     Flyout.HeaderProperty,
                                     Flyout.IsPinnedProperty,
                                     Flyout.IsModalProperty,
                                     Flyout.ThemeProperty,
                                     Flyout.CloseButtonVisibilityProperty,
                                     Flyout.TitleVisibilityProperty);
            this.FlyoutExample.Watch("Layout", this.Side, WidthProperty);
        }
    }
}
