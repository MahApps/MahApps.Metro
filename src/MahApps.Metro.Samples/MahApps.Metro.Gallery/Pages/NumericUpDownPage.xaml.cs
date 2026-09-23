// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows.Controls;
using MahApps.Metro.Controls;

namespace MahApps.Metro.Gallery.Pages
{
    /// <summary>
    /// Interaction logic for NumericUpDownPage.xaml
    /// </summary>
    public partial class NumericUpDownPage : UserControl
    {
        public NumericUpDownPage()
        {
            this.InitializeComponent();

            this.NumberExample.Watch(this.Number,
                                     NumericUpDown.ValueProperty,
                                     NumericUpDown.MinimumProperty,
                                     NumericUpDown.MaximumProperty,
                                     NumericUpDown.IntervalProperty,
                                     NumericUpDown.NumericInputModeProperty,
                                     NumericUpDown.HideUpDownButtonsProperty);
            this.NumberExample.Watch("Layout", this.Number, WidthProperty);

            this.FormattedExample.Watch(this.Formatted,
                                        NumericUpDown.ValueProperty,
                                        NumericUpDown.StringFormatProperty,
                                        NumericUpDown.IntervalProperty,
                                        NumericUpDown.SnapToMultipleOfIntervalProperty);
            this.FormattedExample.Watch("Attached", this.Formatted, TextBoxHelper.ClearTextButtonProperty);

            this.Win10Example.Watch(this.Win10,
                                    NumericUpDown.ValueProperty,
                                    NumericUpDown.HideUpDownButtonsProperty,
                                    NumericUpDown.ButtonsAlignmentProperty,
                                    NumericUpDown.SwitchUpDownButtonsProperty,
                                    IsEnabledProperty);
            this.Win10Example.Watch("Attached", this.Win10, TextBoxHelper.WatermarkProperty, TextBoxHelper.ClearTextButtonProperty);

            this.WinUIExample.Watch(this.WinUI,
                                    NumericUpDown.ValueProperty,
                                    NumericUpDown.HideUpDownButtonsProperty,
                                    NumericUpDown.ButtonsAlignmentProperty,
                                    NumericUpDown.SwitchUpDownButtonsProperty,
                                    IsEnabledProperty);
            this.WinUIExample.Watch("Attached", this.WinUI, TextBoxHelper.WatermarkProperty, TextBoxHelper.ClearTextButtonProperty);
        }
    }
}
