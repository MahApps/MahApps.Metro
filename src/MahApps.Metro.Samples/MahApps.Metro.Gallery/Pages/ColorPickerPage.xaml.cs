// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows.Controls;
using MahApps.Metro.Controls;
using MahApps.Metro.Gallery.Controls;

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

            Options(this.PickerExample, this.Picker);
            Options(this.Win10Example, this.Win10);
            Options(this.WinUIExample, this.WinUI);

            this.CanvasExample.Watch(this.Canvas, ColorCanvas.SelectedColorProperty);
            this.CanvasWin10Example.Watch(this.CanvasWin10, ColorCanvas.SelectedColorProperty);
            this.CanvasWinUIExample.Watch(this.CanvasWinUI, ColorCanvas.SelectedColorProperty);

            Options(this.DropperExample, this.Dropper);
            Options(this.DropperWin10Example, this.DropperWin10);
            Options(this.DropperWinUIExample, this.DropperWinUI);
        }

        private static void Options(ControlExample example, ColorEyeDropper dropper)
        {
            example.Watch(dropper,
                          ColorEyeDropper.SelectedColorProperty,
                          ColorEyeDropper.ContentProperty,
                          IsEnabledProperty);
        }

        private static void Options(ControlExample example, ColorPicker picker)
        {
            example.Watch(picker,
                          ColorPicker.SelectedColorProperty,
                          ColorPicker.DefaultColorProperty,
                          ColorPicker.IsAvailableColorPaletteVisibleProperty,
                          ColorPicker.IsStandardColorPaletteVisibleProperty,
                          ColorPicker.IsRecentColorPaletteVisibleProperty);
            example.Watch("Attached", picker, TextBoxHelper.WatermarkProperty);
            example.Watch("Layout", picker, WidthProperty);
        }
    }
}
