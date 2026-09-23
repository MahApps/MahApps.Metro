// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows.Controls;
using MahApps.Metro.Controls;

namespace MahApps.Metro.Gallery.Pages
{
    /// <summary>
    /// Interaction logic for TextBoxPage.xaml
    /// </summary>
    public partial class TextBoxPage : UserControl
    {
        public TextBoxPage()
        {
            this.InitializeComponent();

            this.PlainExample.Watch(this.Plain, TextBox.TextProperty, TextBox.TextAlignmentProperty, IsEnabledProperty);
            this.PlainExample.Watch("Attached",
                                    this.Plain,
                                    TextBoxHelper.WatermarkProperty,
                                    TextBoxHelper.UseFloatingWatermarkProperty,
                                    TextBoxHelper.WatermarkAlignmentProperty,
                                    TextBoxHelper.ClearTextButtonProperty,
                                    TextBoxHelper.ButtonsAlignmentProperty,
                                    TextBoxHelper.ButtonWidthProperty,
                                    TextBoxHelper.SelectAllOnFocusProperty,
                                    ControlsHelper.CornerRadiusProperty);
            this.PlainExample.Watch("Layout", this.Plain, WidthProperty);

            this.SearchExample.Watch(this.Search, TextBox.TextProperty, IsEnabledProperty);
            this.SearchExample.Watch("Attached",
                                     this.Search,
                                     TextBoxHelper.WatermarkProperty,
                                     TextBoxHelper.ButtonsAlignmentProperty,
                                     TextBoxHelper.ButtonWidthProperty);

            this.Win10Example.Watch(this.Win10, TextBox.TextProperty, IsEnabledProperty);
            this.Win10Example.Watch("Attached",
                                    this.Win10,
                                    TextBoxHelper.WatermarkProperty,
                                    TextBoxHelper.UseFloatingWatermarkProperty,
                                    TextBoxHelper.ClearTextButtonProperty);

            this.WinUIExample.Watch(this.WinUI, TextBox.TextProperty, IsEnabledProperty);
            this.WinUIExample.Watch("Attached",
                                    this.WinUI,
                                    TextBoxHelper.WatermarkProperty,
                                    TextBoxHelper.UseFloatingWatermarkProperty,
                                    TextBoxHelper.ClearTextButtonProperty);

            this.RichExample.Watch(this.Rich, IsEnabledProperty, PaddingProperty);
            this.RichExample.Watch("Attached",
                                   this.Rich,
                                   TextBoxHelper.WatermarkProperty,
                                   TextBoxHelper.ClearTextButtonProperty,
                                   TextBoxHelper.ButtonsAlignmentProperty,
                                   ControlsHelper.CornerRadiusProperty);

            this.RichWin10Example.Watch(this.RichWin10, IsEnabledProperty, PaddingProperty);
            this.RichWin10Example.Watch("Attached",
                                        this.RichWin10,
                                        TextBoxHelper.WatermarkProperty,
                                        TextBoxHelper.ClearTextButtonProperty,
                                        ControlsHelper.CornerRadiusProperty);

            this.RichWinUIExample.Watch(this.RichWinUI, IsEnabledProperty, PaddingProperty);
            this.RichWinUIExample.Watch("Attached",
                                        this.RichWinUI,
                                        TextBoxHelper.WatermarkProperty,
                                        TextBoxHelper.ClearTextButtonProperty,
                                        ControlsHelper.CornerRadiusProperty);
        }
    }
}
