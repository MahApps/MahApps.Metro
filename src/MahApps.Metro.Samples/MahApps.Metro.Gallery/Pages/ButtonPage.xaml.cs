// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows.Controls;
using MahApps.Metro.Controls;

namespace MahApps.Metro.Gallery.Pages
{
    /// <summary>
    /// Interaction logic for ButtonPage.xaml
    /// </summary>
    public partial class ButtonPage : UserControl
    {
        public ButtonPage()
        {
            this.InitializeComponent();

            this.PlainExample.Watch(this.Plain, ContentControl.ContentProperty, IsEnabledProperty);
            this.PlainExample.Watch("Attached",
                                    this.Plain,
                                    ControlsHelper.ContentCharacterCasingProperty,
                                    ControlsHelper.CornerRadiusProperty);
            this.PlainExample.Watch("Layout", this.Plain, WidthProperty, HeightProperty);

            this.SquareExample.Watch(this.Square, ContentControl.ContentProperty, IsEnabledProperty);
            this.SquareExample.Watch("Attached", this.Square, ControlsHelper.ContentCharacterCasingProperty);

            this.AccentExample.Watch(this.Accent, ContentControl.ContentProperty, IsEnabledProperty);
            this.AccentExample.Watch("Layout", this.Accent, WidthProperty, HeightProperty);

            this.CircleExample.Watch(this.Circle, IsEnabledProperty);
            this.CircleExample.Watch("Layout", this.Circle, WidthProperty, HeightProperty);

            this.FlatExample.Watch(this.Flat, ContentControl.ContentProperty, IsEnabledProperty);
            this.FlatExample.Watch("Attached", this.Flat, ControlsHelper.ContentCharacterCasingProperty, ControlsHelper.CornerRadiusProperty);

            this.ChromelessExample.Watch(this.Chromeless, ContentControl.ContentProperty, IsEnabledProperty);

            this.Win10Example.Watch("The one on the left", this.Win10, ContentControl.ContentProperty, IsEnabledProperty);
            this.Win10Example.Watch("The one on the right", this.AccentWin10, ContentControl.ContentProperty, IsEnabledProperty);

            this.FlatAccentExample.Watch(this.FlatAccent, ContentControl.ContentProperty, IsEnabledProperty);
        }
    }
}
