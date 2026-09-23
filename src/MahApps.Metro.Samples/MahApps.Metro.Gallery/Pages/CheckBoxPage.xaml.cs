// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace MahApps.Metro.Gallery.Pages
{
    /// <summary>
    /// Interaction logic for CheckBoxPage.xaml
    /// </summary>
    public partial class CheckBoxPage : UserControl
    {
        public CheckBoxPage()
        {
            this.InitializeComponent();

            this.PlainExample.Watch(this.Plain,
                                    ToggleButton.IsCheckedProperty,
                                    ToggleButton.IsThreeStateProperty,
                                    ContentControl.ContentProperty,
                                    IsEnabledProperty,
                                    FlowDirectionProperty);

            this.TenExample.Watch(this.Ten,
                                  ToggleButton.IsCheckedProperty,
                                  ToggleButton.IsThreeStateProperty,
                                  ContentControl.ContentProperty,
                                  IsEnabledProperty);

            this.WinUIExample.Watch(this.WinUI,
                                    ToggleButton.IsCheckedProperty,
                                    ToggleButton.IsThreeStateProperty,
                                    ContentControl.ContentProperty,
                                    IsEnabledProperty);
        }
    }
}
