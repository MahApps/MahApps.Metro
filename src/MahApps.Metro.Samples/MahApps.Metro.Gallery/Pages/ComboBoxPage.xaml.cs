// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Linq;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using MahApps.Metro.Controls;

namespace MahApps.Metro.Gallery.Pages
{
    /// <summary>
    /// Interaction logic for ComboBoxPage.xaml
    /// </summary>
    public partial class ComboBoxPage : UserControl
    {
        public ComboBoxPage()
        {
            this.InitializeComponent();

            this.LongList.ItemsSource = Enumerable.Range(1, 4200).Select(entry => $"Entry {entry}").ToList();

            this.PickExample.Watch(this.Pick,
                                   Selector.SelectedIndexProperty,
                                   ComboBox.MaxDropDownHeightProperty,
                                   IsEnabledProperty);
            this.PickExample.Watch("Attached",
                                   this.Pick,
                                   TextBoxHelper.WatermarkProperty,
                                   TextBoxHelper.UseFloatingWatermarkProperty,
                                   ControlsHelper.CornerRadiusProperty);
            this.PickExample.Watch("Layout", this.Pick, WidthProperty);

            this.TypeExample.Watch(this.Typed,
                                   ComboBox.IsEditableProperty,
                                   ComboBox.IsReadOnlyProperty,
                                   ComboBox.IsTextSearchEnabledProperty);
            this.TypeExample.Watch("Attached",
                                   this.Typed,
                                   TextBoxHelper.WatermarkProperty,
                                   TextBoxHelper.ClearTextButtonProperty,
                                   TextBoxHelper.UseFloatingWatermarkProperty,
                                   ComboBoxHelper.MaxLengthProperty,
                                   ComboBoxHelper.CharacterCasingProperty);

            this.LongExample.Watch(this.LongList,
                                   Selector.SelectedIndexProperty,
                                   ComboBox.MaxDropDownHeightProperty);
            this.LongExample.Watch("Layout", this.LongList, WidthProperty);
        }
    }
}
