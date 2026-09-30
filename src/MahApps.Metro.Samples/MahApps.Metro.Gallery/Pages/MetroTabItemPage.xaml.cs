// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows.Controls;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;

namespace MahApps.Metro.Gallery.Pages
{
    /// <summary>
    /// Interaction logic for MetroTabItemPage.xaml
    /// </summary>
    public partial class MetroTabItemPage : UserControl
    {
        public MetroTabItemPage()
        {
            this.InitializeComponent();

            this.CloseExample.Watch(this.First,
                                    MetroTabItem.CloseButtonEnabledProperty,
                                    MetroTabItem.HeaderProperty,
                                    MetroTabItem.CloseButtonMarginProperty);
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
