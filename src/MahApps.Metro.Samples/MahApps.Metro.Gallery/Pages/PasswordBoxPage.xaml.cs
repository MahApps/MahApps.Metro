// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows.Controls;
using MahApps.Metro.Controls;

namespace MahApps.Metro.Gallery.Pages
{
    /// <summary>
    /// Interaction logic for PasswordBoxPage.xaml
    /// </summary>
    public partial class PasswordBoxPage : UserControl
    {
        public PasswordBoxPage()
        {
            this.InitializeComponent();

            this.PlainExample.Watch(this.Plain,
                                    PasswordBox.PasswordCharProperty,
                                    PasswordBox.MaxLengthProperty,
                                    IsEnabledProperty);
            this.PlainExample.Watch("Attached",
                                    this.Plain,
                                    TextBoxHelper.WatermarkProperty,
                                    TextBoxHelper.ClearTextButtonProperty,
                                    TextBoxHelper.UseFloatingWatermarkProperty,
                                    PasswordBoxHelper.CapsLockWarningToolTipProperty,
                                    ControlsHelper.CornerRadiusProperty);
            this.PlainExample.Watch("Layout", this.Plain, WidthProperty);

            this.RevealExample.Watch(this.Reveal, PasswordBox.PasswordCharProperty);
            this.RevealExample.Watch("Attached",
                                     this.Reveal,
                                     TextBoxHelper.WatermarkProperty,
                                     PasswordBoxHelper.RevealButtonContentProperty);

            this.Win10Example.Watch(this.Win10, PasswordBox.PasswordCharProperty, IsEnabledProperty);
            this.Win10Example.Watch("Attached",
                                    this.Win10,
                                    TextBoxHelper.WatermarkProperty,
                                    TextBoxHelper.ClearTextButtonProperty,
                                    TextBoxHelper.UseFloatingWatermarkProperty,
                                    ControlsHelper.CornerRadiusProperty);

            this.WinUIExample.Watch(this.WinUI, PasswordBox.PasswordCharProperty, IsEnabledProperty);
            this.WinUIExample.Watch("Attached",
                                    this.WinUI,
                                    TextBoxHelper.WatermarkProperty,
                                    TextBoxHelper.ClearTextButtonProperty,
                                    TextBoxHelper.UseFloatingWatermarkProperty,
                                    ControlsHelper.CornerRadiusProperty);
        }
    }
}
