// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using MahApps.Metro.Controls;
using MahApps.Metro.Gallery.Core;
using MahApps.Metro.Controls.Dialogs;

namespace MahApps.Metro.Gallery.Pages
{
    /// <summary>
    /// Interaction logic for MetroTabItemPage.xaml
    /// </summary>
    /// <remarks>
    /// A card holds the same tabs three times over, once per set the library draws. Only the first
    /// of the three is watched; the other two read those properties off it, so one option moves all
    /// three. The four brushes of TabControlHelper are the exception: each set picks its own and a
    /// card is there to show which, so those stay with the control they are turned on.
    /// </remarks>
    public partial class MetroTabItemPage : UserControl
    {
        public MetroTabItemPage()
        {
            this.InitializeComponent();

            this.CloseExample.Watch(this.Tabs, TabControl.TabStripPlacementProperty, Selector.SelectedIndexProperty);
            // a tab that can be closed carries the answer itself, which beats what came down from
            // the control, so the switch for it is the one on the tab below
            this.CloseExample.WatchTheTabControlHelper(this.Tabs, theCloseButtonToo: false);
            this.CloseExample.Watch("First tab",
                                    this.First,
                                    MetroTabItem.CloseButtonEnabledProperty,
                                    MetroTabItem.HeaderProperty,
                                    MetroTabItem.CloseButtonMarginProperty);

            this.AskExample.Watch(this.AskingTabs, TabControl.TabStripPlacementProperty, Selector.SelectedIndexProperty);
            this.AskExample.WatchTheTabControlHelper(this.AskingTabs, theCloseButtonToo: false);
        }

        /// <summary>
        /// What a real application does when a tab holds something the reader has not saved: it asks
        /// first. The deferral is what buys the time for that question, since the answer to it only
        /// arrives once the dialog is gone.
        /// </summary>
        private async void AskBeforeTheTabGoes(object? sender, BaseMetroTabControl.TabItemClosingEventArgs e)
        {
            using var deferral = e.GetDeferral();

            var window = this.TryFindParent<MetroWindow>();
            if (window is null)
            {
                return;
            }

            var answer = await window.ShowMessageAsync($"Close {e.ClosingTabItem.Header}?",
                                                       "What is in this tab has not been saved anywhere. Close it all the same?",
                                                       MessageDialogStyle.AffirmativeAndNegative,
                                                       new MetroDialogSettings
                                                       {
                                                           AffirmativeButtonText = "Close it",
                                                           NegativeButtonText = "Keep it"
                                                       });

            e.Cancel = answer != MessageDialogResult.Affirmative;
        }
    }
}
