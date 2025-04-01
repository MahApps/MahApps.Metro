// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using MahApps.Metro.Controls.Dialogs;

namespace MetroDemo.ExampleWindows
{
    public partial class FlyoutDemo : IDisposable
    {
        private bool _disposed;
        private bool _hideOnClose = true;

        public FlyoutDemo()
        {
            this.DataContext = new MainWindowViewModel(DialogCoordinator.Instance);

            this.InitializeComponent();

            this.Closing += (s, e) =>
                {
                    if (this._hideOnClose)
                    {
                        this.Hide();
                        e.Cancel = true;
                    }
                };

            var windowPlacementSettings = this.GetWindowPlacementSettings();
            if (windowPlacementSettings is not null && windowPlacementSettings.UpgradeSettings)
            {
                windowPlacementSettings.Upgrade();
                windowPlacementSettings.UpgradeSettings = false;
                windowPlacementSettings.Save();
            }
        }

        public void Launch()
        {
            this.Owner = Application.Current.MainWindow;
            // only for this window, because we allow minimizing
            if (this.WindowState == WindowState.Minimized)
            {
                this.WindowState = WindowState.Normal;
            }

            this.Show();
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (!this._disposed && disposing)
            {
                this._disposed = true;
                this._hideOnClose = false;
                this.Close();
            }
        }

        private void CloseMe(object sender, RoutedEventArgs e)
        {
            this._hideOnClose = false;
            this.Close();
        }

        private void ShowDynamicFlyout(object sender, RoutedEventArgs e)
        {
            var flyout = new DynamicFlyout
                         {
                             Header = "Dynamic flyout"
                         };

            // when the flyout is closed, remove it from the hosting FlyoutsControl
            void ClosingFinishedHandler(object o, RoutedEventArgs args)
            {
                flyout.ClosingFinished -= ClosingFinishedHandler;
                this.flyoutsControl.Items.Remove(flyout);
            }

            flyout.ClosingFinished += ClosingFinishedHandler;

            this.flyoutsControl.Items.Add(flyout);

            flyout.IsOpen = true;
        }

        private async void ClickMeOnClick(object sender, RoutedEventArgs e)
        {
            await this.ShowMessageAsync("Title Template Test", "Thx for using MahApps.Metro!!!");
        }
    }
}