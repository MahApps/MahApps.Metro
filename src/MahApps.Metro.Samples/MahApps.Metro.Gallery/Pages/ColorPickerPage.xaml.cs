// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows.Controls;
using MahApps.Metro.Controls;

namespace MahApps.Metro.Gallery.Pages
{
    /// <summary>
    /// Interaction logic for ColorPickerPage.xaml
    /// </summary>
    public partial class ColorPickerPage : UserControl
    {
        public ColorPickerPage()
        {
            this.InitializeComponent();

            this.PickerExample.Watch(this.Picker,
                                     ColorPicker.SelectedColorProperty,
                                     ColorPicker.DefaultColorProperty,
                                     ColorPicker.IsAvailableColorPaletteVisibleProperty,
                                     ColorPicker.IsStandardColorPaletteVisibleProperty,
                                     ColorPicker.IsRecentColorPaletteVisibleProperty);
            this.PickerExample.Watch("Attached", this.Picker, TextBoxHelper.WatermarkProperty);
            this.PickerExample.Watch("Layout", this.Picker, WidthProperty);

            this.CanvasExample.Watch(this.Canvas, ColorCanvas.SelectedColorProperty);
        }
    }
}
