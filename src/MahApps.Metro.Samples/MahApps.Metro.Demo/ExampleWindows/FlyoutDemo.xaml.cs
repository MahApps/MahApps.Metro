﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using MahApps.Metro.Controls;
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

            var mainWindow = (MetroWindow)this;
            var windowPlacementSettings = mainWindow.GetWindowPlacementSettings();
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

        protected void Dispose(bool disposing)
        {
            if (!this._disposed && disposing)
            {
                this._disposed = true;
                this._hideOnClose = false;
                this.Close();
            }
        }

        private void ShowSecond(object sender, RoutedEventArgs e)
        {
            this.ToggleFlyout(1);
        }

        private void ShowThird(object sender, RoutedEventArgs e)
        {
            this.ToggleFlyout(2);
        }

        private void ShowInverse(object sender, RoutedEventArgs e)
        {
            this.ToggleFlyout(3);
        }

        private void ShowAdapt(object sender, RoutedEventArgs e)
        {
            this.ToggleFlyout(4);
        }

        private void ShowSettings(object sender, RoutedEventArgs e)
        {
            this.ToggleFlyout(5);
        }

        private void ShowLeft(object sender, RoutedEventArgs e)
        {
            this.ToggleFlyout(6);
        }

        private void ShowCustomTop(object sender, RoutedEventArgs e)
        {
            this.ToggleFlyout(7);
        }

        private void ShowTop(object sender, RoutedEventArgs e)
        {
            this.ToggleFlyout(8);
        }

        private void ShowBottom(object sender, RoutedEventArgs e)
        {
            this.ToggleFlyout(9);
        }

        private void ShowModal(object sender, RoutedEventArgs e)
        {
            this.ToggleFlyout(10);
        }

        private void ShowAppBar(object sender, RoutedEventArgs e)
        {
            this.ToggleFlyout(11);
        }

        private void CloseMe(object sender, RoutedEventArgs e)
        {
            this._hideOnClose = false;
            this.Close();
        }

        private void ToggleFlyout(int index)
        {
            var flyout = this.Flyouts?.Items[index] as Flyout;
            if (flyout == null)
            {
                return;
            }

            flyout.IsOpen = !flyout.IsOpen;
        }

        private void ShowSettingsLeft(object sender, RoutedEventArgs e)
        {
            var flyout = (Flyout)this.Flyouts!.Items[6];
            flyout.Position = Position.Left;
        }

        private void ShowSettingsRight(object sender, RoutedEventArgs e)
        {
            var flyout = (Flyout)this.Flyouts!.Items[6];
            flyout.Position = Position.Right;
        }

        private void ShowDynamicFlyout(object sender, RoutedEventArgs e)
        {
            var flyout = new DynamicFlyout
                         {
                             Header = "Dynamic flyout"
                         };

            // when the flyout is closed, remove it from the hosting FlyoutsControl
            RoutedEventHandler? closingFinishedHandler = null;
            closingFinishedHandler = (o, args) =>
                {
                    flyout.ClosingFinished -= closingFinishedHandler;
                    this.flyoutsControl.Items.Remove(flyout);
                };
            flyout.ClosingFinished += closingFinishedHandler;

            this.flyoutsControl.Items.Add(flyout);

            flyout.IsOpen = true;
        }

        private void ShowAutoCloseFlyout(object sender, RoutedEventArgs e)
        {
            this.ToggleFlyout(13);
        }

        private async void ClickMeOnClick(object sender, RoutedEventArgs e)
        {
            await this.ShowMessageAsync("Title Template Test", "Thx for using MahApps.Metro!!!");
        }

        private void TopFlyoutCloseButtonOnClick(object sender, RoutedEventArgs e)
        {
            this.ToggleFlyout(8);
        }
    }
}