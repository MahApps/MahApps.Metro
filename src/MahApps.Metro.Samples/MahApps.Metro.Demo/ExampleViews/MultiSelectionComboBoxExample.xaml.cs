// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using System.Diagnostics;
using System.Windows.Controls;

namespace MetroDemo.ExampleViews
{
    /// <summary>
    /// Interaction logic for MultiSelectionComboBoxExample.xaml
    /// </summary>
    public partial class MultiSelectionComboBoxExample : UserControl
    {
        public MultiSelectionComboBoxExample()
        {
            this.InitializeComponent();
        }

        private void Mscb_Example_AddingItem(object? sender, AddingItemEventArgs args)
        {
            // We don't want to get double entries so let`s check if we already have one.
            args.Accepted = args.TargetList is not null && !args.TargetList.Contains(args.ParsedObject);
        }

        private async void Mscb_Example_AddedItem(object? sender, AddedItemEventArgs args)
        {
            var window = this.TryFindParent<MetroWindow>();
            if (window is null)
            {
                return;
            }

            await window.ShowMessageAsync("Added Item", $"Successfully added \"{args.AddedItem}\" to your Items-Collection");
        }

        private void mscb_Example_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            foreach (var item in e.AddedItems)
            {
                Debug.WriteLine($"MultiSelectionComboBox-Example: Selected item \"{item}\"");
            }

            foreach (var item in e.RemovedItems)
            {
                Debug.WriteLine($"MultiSelectionComboBox-Example: Unselected item \"{item}\"");
            }
        }
    }
}