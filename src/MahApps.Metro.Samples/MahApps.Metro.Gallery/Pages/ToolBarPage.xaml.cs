// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace MahApps.Metro.Gallery.Pages
{
    /// <summary>
    /// Interaction logic for ToolBarPage.xaml
    /// </summary>
    public partial class ToolBarPage : UserControl
    {
        public ToolBarPage()
        {
            this.InitializeComponent();

            this.TrayExample.Watch(this.Tray,
                                   ToolBarTray.IsLockedProperty,
                                   ToolBarTray.OrientationProperty,
                                   BackgroundProperty);
            this.TrayExample.Watch("The bar", this.Bar, BackgroundProperty, IsEnabledProperty);
            this.TrayExample.Watch("Layout", this.Tray, WidthProperty);

            this.ItemsExample.Watch(this.Tick, ToggleButton.IsCheckedProperty, ContentControl.ContentProperty);

            this.OverflowExample.Watch("Layout", this.Narrow, WidthProperty);
        }
    }
}
