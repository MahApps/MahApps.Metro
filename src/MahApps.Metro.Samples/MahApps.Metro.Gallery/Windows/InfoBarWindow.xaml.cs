// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows;
using MahApps.Metro.Controls;
using MahApps.Metro.Gallery.Core;

namespace MahApps.Metro.Gallery.Windows
{
    /// <summary>
    /// Interaction logic for InfoBarWindow.xaml
    /// </summary>
    public partial class InfoBarWindow : MetroWindow
    {
        public InfoBarWindow()
        {
            this.InitializeComponent();

            // a sample window shows what the default set gives it, whatever the gallery around it wears
            DefaultStyleSet.Apply(this);
        }

        private void OnReopenClick(object sender, RoutedEventArgs e)
        {
            this.UpdateBar.IsOpen = true;
            this.OfflineBar.IsOpen = true;
        }

        private void OnFlyoutClick(object sender, RoutedEventArgs e)
        {
            this.SettingsFlyout.IsOpen = true;
        }
    }
}
