// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using MahApps.Metro.Controls;
using MahApps.Metro.Gallery.Core;

namespace MahApps.Metro.Gallery.Pages
{
    /// <summary>
    /// Interaction logic for DataGridPage.xaml
    /// </summary>
    public partial class DataGridPage : UserControl
    {
        public DataGridPage()
        {
            this.InitializeComponent();

            this.BasicExample.Watch(this.Basic,
                                    DataGrid.SelectionModeProperty,
                                    DataGrid.SelectionUnitProperty,
                                    DataGrid.CanUserResizeColumnsProperty,
                                    DataGrid.CanUserReorderColumnsProperty,
                                    DataGrid.FrozenColumnCountProperty,
                                    DataGrid.GridLinesVisibilityProperty,
                                    DataGrid.HeadersVisibilityProperty);

            this.GroupingExample.Watch(this.Grouping,
                                       DataGrid.GridLinesVisibilityProperty,
                                       DataGrid.HeadersVisibilityProperty);

            this.TemplatesExample.Watch(this.Templates,
                                        DataGrid.CanUserSortColumnsProperty,
                                        DataGrid.GridLinesVisibilityProperty,
                                        DataGrid.HeadersVisibilityProperty);
        }

        private void BasicSorting(object sender, DataGridSortingEventArgs e)
        {
            // ascending, descending, and the third click puts the rows back the way the source has them
            ListSortDirection? next = e.Column.SortDirection switch
            {
                null => ListSortDirection.Ascending,
                ListSortDirection.Ascending => ListSortDirection.Descending,
                _ => null
            };

            if (next is null)
            {
                e.Handled = true;
                e.Column.SortDirection = null;
                this.Basic.Items.SortDescriptions.Clear();
                this.SortStatus.Text = "Rows are in source order.";
            }
            else
            {
                this.SortStatus.Text = $"Sorted by {e.Column.Header}, {next.Value.ToString().ToLowerInvariant()}.";
            }
        }

        private void BasicSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // a selection made while the page is being built comes before the line that reports it
            if (this.SelectionStatus is null)
            {
                return;
            }

            this.SelectionStatus.Text = this.Basic.SelectedItem is Language language
                ? $"Selected: {language.Name} (ID {language.Id})."
                : "No row selected.";
        }

        private void FilterChanged(object sender, TextChangedEventArgs e)
        {
            var text = this.Filter.Text.Trim();

            this.Grouping.Items.Filter = text.Length == 0
                ? null
                : item => item is Language language
                          && (language.Name.IndexOf(text, StringComparison.CurrentCultureIgnoreCase) >= 0
                              || language.Paradigm.IndexOf(text, StringComparison.CurrentCultureIgnoreCase) >= 0);
        }

        private void GroupByChanged(object sender, SelectionChangedEventArgs e)
        {
            // the combo box picks its first item while the page is still being built, before the grid is there
            if (this.Grouping is null)
            {
                return;
            }

            var groups = this.Grouping.Items.GroupDescriptions;
            groups.Clear();

            if (this.GroupBy.SelectedItem is ComboBoxItem { Content: string property } && property != "None")
            {
                groups.Add(new PropertyGroupDescription(property));
            }
        }

        private void ExpandAllGroups(object sender, RoutedEventArgs e)
        {
            this.SetGroupsExpanded(true);
        }

        private void CollapseAllGroups(object sender, RoutedEventArgs e)
        {
            this.SetGroupsExpanded(false);
        }

        /// <summary>
        /// Opens or closes every group the grid shows. The header is an Expander in the Metro set and
        /// a toggle in the other two, and both of them are called PART_Header.
        /// </summary>
        private void SetGroupsExpanded(bool isExpanded)
        {
            foreach (var group in this.Grouping.FindChildren<GroupItem>(true))
            {
                switch (group.Template?.FindName("PART_Header", group))
                {
                    case Expander expander:
                        expander.SetCurrentValue(Expander.IsExpandedProperty, isExpanded);
                        break;
                    case ToggleButton toggle:
                        toggle.SetCurrentValue(ToggleButton.IsCheckedProperty, isExpanded);
                        break;
                }
            }
        }
    }
}
