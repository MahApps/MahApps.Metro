// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using MahApps.Metro.Controls;

namespace MahApps.Metro.Gallery.Pages
{
    /// <summary>
    /// Interaction logic for ToggleButtonPage.xaml
    /// </summary>
    public partial class ToggleButtonPage : UserControl
    {
        public ToggleButtonPage()
        {
            this.InitializeComponent();

            this.PlainExample.Watch(this.Plain,
                                    ToggleButton.IsCheckedProperty,
                                    ToggleButton.IsThreeStateProperty,
                                    ContentControl.ContentProperty,
                                    IsEnabledProperty);
            this.PlainExample.Watch("Attached",
                                    this.Plain,
                                    ControlsHelper.ContentCharacterCasingProperty,
                                    ControlsHelper.CornerRadiusProperty);
            this.PlainExample.Watch("Layout", this.Plain, MinWidthProperty);

            this.CircleExample.Watch(this.Circle, ToggleButton.IsCheckedProperty, ForegroundProperty, IsEnabledProperty);
            this.CircleExample.Watch("Layout", this.Circle, WidthProperty, HeightProperty);

            this.FlatExample.Watch(this.Flat,
                                   ToggleButton.IsCheckedProperty,
                                   ContentControl.ContentProperty,
                                   IsEnabledProperty);
            this.FlatExample.Watch("Attached", this.Flat, ControlsHelper.ContentCharacterCasingProperty);
        }
    }
}
