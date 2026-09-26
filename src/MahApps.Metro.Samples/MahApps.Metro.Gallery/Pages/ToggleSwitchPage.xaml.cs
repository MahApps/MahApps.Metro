// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows.Controls;
using MahApps.Metro.Controls;

namespace MahApps.Metro.Gallery.Pages
{
    /// <summary>
    /// Interaction logic for ToggleSwitchPage.xaml
    /// </summary>
    public partial class ToggleSwitchPage : UserControl
    {
        public ToggleSwitchPage()
        {
            this.InitializeComponent();

            this.SwitchExample.Watch(this.Switch,
                                     ToggleSwitch.IsOnProperty,
                                     HeaderedContentControl.HeaderProperty,
                                     IsEnabledProperty);

            this.ContentExample.Watch(this.Sides,
                                      ToggleSwitch.IsOnProperty,
                                      ToggleSwitch.OnContentProperty,
                                      ToggleSwitch.OffContentProperty,
                                      ToggleSwitch.ContentDirectionProperty,
                                      ToggleSwitch.ContentPaddingProperty);

            this.WinUIExample.Watch(this.WinUI,
                                    ToggleSwitch.IsOnProperty,
                                    HeaderedContentControl.HeaderProperty,
                                    IsEnabledProperty);
        }
    }
}
