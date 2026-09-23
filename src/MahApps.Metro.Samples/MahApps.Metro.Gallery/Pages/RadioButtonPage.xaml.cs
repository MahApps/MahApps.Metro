// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using MahApps.Metro.Controls;

namespace MahApps.Metro.Gallery.Pages
{
    /// <summary>
    /// Interaction logic for RadioButtonPage.xaml
    /// </summary>
    public partial class RadioButtonPage : UserControl
    {
        public RadioButtonPage()
        {
            this.InitializeComponent();

            this.GroupExample.Watch(this.First,
                                    ToggleButton.IsCheckedProperty,
                                    ContentControl.ContentProperty,
                                    IsEnabledProperty,
                                    FlowDirectionProperty);
            this.GroupExample.Watch("Attached",
                                    this.First,
                                    RadioButtonHelper.RadioSizeProperty,
                                    RadioButtonHelper.RadioCheckSizeProperty);

            this.TenExample.Watch(this.Ten,
                                  ToggleButton.IsCheckedProperty,
                                  ContentControl.ContentProperty,
                                  IsEnabledProperty);
            this.TenExample.Watch("Attached",
                                  this.Ten,
                                  RadioButtonHelper.RadioSizeProperty,
                                  RadioButtonHelper.RadioCheckSizeProperty);

            this.WinUIExample.Watch(this.WinUI,
                                    ToggleButton.IsCheckedProperty,
                                    ContentControl.ContentProperty,
                                    IsEnabledProperty);
            this.WinUIExample.Watch("Attached",
                                    this.WinUI,
                                    RadioButtonHelper.RadioSizeProperty,
                                    RadioButtonHelper.RadioCheckSizeProperty);
        }
    }
}
