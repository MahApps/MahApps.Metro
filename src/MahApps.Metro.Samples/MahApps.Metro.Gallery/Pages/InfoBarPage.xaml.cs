// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using MahApps.Metro.Controls;

namespace MahApps.Metro.Gallery.Pages
{
    /// <summary>
    /// Interaction logic for InfoBarPage.xaml
    /// </summary>
    public partial class InfoBarPage : UserControl
    {
        public InfoBarPage()
        {
            this.InitializeComponent();

            this.SeverityExample.Watch(this.SeverityInfoBar,
                                       InfoBar.IsOpenProperty,
                                       InfoBar.SeverityProperty,
                                       InfoBar.TitleProperty,
                                       InfoBar.MessageProperty,
                                       InfoBar.IsClosableProperty,
                                       InfoBar.IsIconVisibleProperty);
        }

        /// <summary>
        /// The events of the last closes of the refusing bar, the newest on top.
        /// </summary>
        public ObservableCollection<string> EventLog { get; } = new();

        private void Log(string entry)
        {
            this.EventLog.Insert(0, entry);
            while (this.EventLog.Count > 6)
            {
                this.EventLog.RemoveAt(this.EventLog.Count - 1);
            }
        }

        private void OnCancelInfoBarCloseButtonClick(object sender, RoutedEventArgs e)
        {
            this.Log("CloseButtonClick");
        }

        private void OnCancelInfoBarClosing(object? sender, InfoBarClosingEventArgs e)
        {
            e.Cancel = this.AllowClose.IsChecked != true;
            this.Log($"Closing, Reason {e.Reason}, Cancel {e.Cancel}");
        }

        private void OnCancelInfoBarClosed(object? sender, InfoBarClosedEventArgs e)
        {
            this.Log($"Closed, Reason {e.Reason}");
        }

        private void OnReopenClick(object sender, RoutedEventArgs e)
        {
            this.CancelInfoBar.IsOpen = true;
        }
    }
}
