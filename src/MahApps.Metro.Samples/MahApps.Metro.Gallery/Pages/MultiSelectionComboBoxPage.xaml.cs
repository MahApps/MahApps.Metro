// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Windows.Controls;
using MahApps.Metro.Controls;

namespace MahApps.Metro.Gallery.Pages
{
    /// <summary>
    /// Interaction logic for MultiSelectionComboBoxPage.xaml
    /// </summary>
    public partial class MultiSelectionComboBoxPage : UserControl
    {
        public MultiSelectionComboBoxPage()
        {
            this.InitializeComponent();

            this.DataContext = this;

            // three picks are enough to see what the row in front does with them
            foreach (var box in new[] { this.Picked, this.Ticks, this.Removable, this.Typed })
            {
                box.SelectedItems?.Add(this.Names[1]);
                box.SelectedItems?.Add(this.Names[3]);
                box.SelectedItems?.Add(this.Names[5]);
            }

            // and the two that are about the row itself want more than fits into one line
            foreach (var box in new[] { this.Wrapping, this.OneLine })
            {
                foreach (var name in this.Names)
                {
                    box.SelectedItems?.Add(name);
                }
            }

            this.PickExample.Watch(this.Picked,
                                   MultiSelectionComboBox.SelectionModeProperty,
                                   MultiSelectionComboBox.OrderSelectedItemsByProperty,
                                   ComboBox.IsDropDownOpenProperty,
                                   IsEnabledProperty);

            this.StyleExample.Watch("The upper one", this.Wrapping, WidthProperty);
            this.StyleExample.Watch("The lower one",
                                    this.OneLine,
                                    WidthProperty,
                                    MultiSelectionComboBox.HorizontalScrollBarVisibilityProperty);

            this.TickExample.Watch(this.Ticks,
                                   ComboBox.IsDropDownOpenProperty,
                                   MultiSelectionComboBox.SelectionModeProperty);

            this.RemoveExample.Watch(this.Removable,
                                     ComboBox.IsEditableProperty,
                                     MultiSelectionComboBox.OrderSelectedItemsByProperty);

            this.TypeExample.Watch(this.Typed,
                                   ComboBox.IsEditableProperty,
                                   MultiSelectionComboBox.SeparatorProperty,
                                   MultiSelectionComboBox.SelectItemsFromTextInputDelayProperty,
                                   MultiSelectionComboBox.EditableTextStringComparisionProperty,
                                   MultiSelectionComboBox.AcceptsReturnProperty,
                                   MultiSelectionComboBox.HasCustomTextProperty);
        }

        /// <summary>
        /// What the boxes on this page have to offer. A list of the kind anybody has lying about,
        /// long enough that the picks outgrow one line.
        /// </summary>
        public IReadOnlyList<string> Names { get; } = new[]
                                                      {
                                                          "Ada Lovelace",
                                                          "Grace Hopper",
                                                          "Alan Turing",
                                                          "Edsger Dijkstra",
                                                          "Barbara Liskov",
                                                          "Donald Knuth",
                                                          "Margaret Hamilton",
                                                          "Tony Hoare"
                                                      };
    }
}
