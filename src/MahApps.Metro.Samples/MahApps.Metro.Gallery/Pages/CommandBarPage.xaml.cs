// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using MahApps.Metro.Controls;

namespace MahApps.Metro.Gallery.Pages
{
    /// <summary>
    /// Interaction logic for CommandBarPage.xaml
    /// </summary>
    public partial class CommandBarPage : UserControl
    {
        public CommandBarPage()
        {
            this.InitializeComponent();

            this.BasicExample.Watch(this.BasicBar, IsEnabledProperty, FlowDirectionProperty);
            this.TitleExample.Watch("Layout", this.TitleRow, WidthProperty);
            this.FormatExample.Watch(this.Bold, ToggleButton.IsCheckedProperty, IsEnabledProperty);
            this.OverflowExample.Watch("Layout", this.OverflowBar, WidthProperty);
            this.CommandsExample.Watch(this.Note, TextBox.TextProperty);
            this.SearchExample.Watch(this.Search, TextBoxHelper.WatermarkProperty, TextBoxHelper.ClearTextButtonProperty);
            this.TrayExample.Watch(this.Tray, ToolBarTray.IsLockedProperty, ToolBarTray.OrientationProperty);
        }
    }
}
