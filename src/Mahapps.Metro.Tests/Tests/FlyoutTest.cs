// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using ControlzEx.Theming;
using MahApps.Metro.Controls;
using MahApps.Metro.Tests.TestHelpers;
using MahApps.Metro.Tests.Views;
using Xunit;

namespace MahApps.Metro.Tests.Tests
{
    public class FlyoutTest : AutomationTestBase
    {
        [Fact]
        [DisplayTestMethodName]
        public async Task AdaptsWindowCommandsToDarkFlyout()
        {
            await TestHost.SwitchToAppThread();

            var window = await WindowHelpers.CreateInvisibleWindowAsync<FlyoutWindow>();

            var flyout = new Flyout { Theme = FlyoutTheme.Dark };
            window.Flyouts.Items.Add(flyout);

            flyout.IsOpen = true;

            var expectedColor = ((SolidColorBrush)ThemeManager.Current.GetTheme("Dark.Blue").Resources["MahApps.Brushes.ThemeForeground"]).Color;

            window.AssertWindowCommandsColor(expectedColor);
        }

        [Fact]
        [DisplayTestMethodName]
        public async Task DefaultFlyoutPositionIsLeft()
        {
            await TestHost.SwitchToAppThread();

            var window = await WindowHelpers.CreateInvisibleWindowAsync<FlyoutWindow>();

            Assert.Equal(Position.Left, window.DefaultFlyout.Position);
        }

        [Fact]
        [DisplayTestMethodName]
        public async Task FlyoutIsClosedByDefault()
        {
            await TestHost.SwitchToAppThread();

            var window = await WindowHelpers.CreateInvisibleWindowAsync<FlyoutWindow>();

            Assert.False(window.DefaultFlyout.IsOpen);
        }

        [Fact]
        [DisplayTestMethodName]
        public async Task FlyoutIsHiddenByDefault()
        {
            await TestHost.SwitchToAppThread();

            var window = await WindowHelpers.CreateInvisibleWindowAsync<FlyoutWindow>();

            // root grid should be hidden
            Assert.Equal(Visibility.Hidden, window.DefaultFlyout.Visibility);
        }

        [Fact]
        [DisplayTestMethodName]
        public async Task HiddenIconIsBelowFlyout()
        {
            await TestHost.SwitchToAppThread();

            var window = await WindowHelpers.CreateInvisibleWindowAsync<FlyoutWindow>();
            window.IconOverlayBehavior = OverlayBehavior.Never;
            window.LeftFlyout.IsOpen = true;

            var icon = window.FindChild<FrameworkElement>("PART_Icon");
            Assert.NotNull(icon);

            var iconZIndex = Panel.GetZIndex(icon);
            var flyoutZIndex = Panel.GetZIndex(window.LeftFlyout);

            Assert.True(flyoutZIndex < iconZIndex);
        }

        [Fact]
        [DisplayTestMethodName]
        public async Task HiddenLeftWindowCommandsAreBelowFlyout()
        {
            await TestHost.SwitchToAppThread();

            var window = await WindowHelpers.CreateInvisibleWindowAsync<FlyoutWindow>();
            window.LeftWindowCommandsOverlayBehavior = WindowCommandsOverlayBehavior.Never;
            window.LeftFlyout.IsOpen = true;

            var windowCommands = window.FindChild<ContentPresenter>("PART_LeftWindowCommands");
            Assert.NotNull(windowCommands);

            var windowCommandsZIndex = Panel.GetZIndex(windowCommands);
            var flyoutZIndex = Panel.GetZIndex(window.LeftFlyout);

            Assert.True(flyoutZIndex < windowCommandsZIndex);
        }

        [Fact]
        [DisplayTestMethodName]
        public async Task HiddenRightWindowCommandsAreBelowFlyout()
        {
            await TestHost.SwitchToAppThread();

            var window = await WindowHelpers.CreateInvisibleWindowAsync<FlyoutWindow>();
            window.RightWindowCommandsOverlayBehavior = WindowCommandsOverlayBehavior.Never;
            window.RightFlyout.IsOpen = true;

            var windowCommands = window.FindChild<ContentPresenter>("PART_RightWindowCommands");
            Assert.NotNull(windowCommands);

            var windowCommandsZIndex = Panel.GetZIndex(windowCommands);
            var flyoutZIndex = Panel.GetZIndex(window.RightFlyout);

            Assert.True(flyoutZIndex < windowCommandsZIndex);
        }

        [Fact]
        [DisplayTestMethodName]
        public async Task LeftWindowCommandsAreOverFlyout()
        {
            await TestHost.SwitchToAppThread();

            var window = await WindowHelpers.CreateInvisibleWindowAsync<FlyoutWindow>();
            window.LeftFlyout.IsOpen = true;

            var windowCommands = window.FindChild<ContentPresenter>("PART_LeftWindowCommands");
            Assert.NotNull(windowCommands);

            var windowCommandsZIndex = Panel.GetZIndex(windowCommands);
            var flyoutZIndex = Panel.GetZIndex(window.LeftFlyout);

            Assert.True(windowCommandsZIndex > flyoutZIndex);
        }

        [Fact]
        [DisplayTestMethodName]
        public async Task RightWindowCommandsAreOverFlyout()
        {
            await TestHost.SwitchToAppThread();

            var window = await WindowHelpers.CreateInvisibleWindowAsync<FlyoutWindow>();
            window.RightFlyout.IsOpen = true;

            var windowCommands = window.FindChild<ContentPresenter>("PART_RightWindowCommands");
            Assert.NotNull(windowCommands);

            var windowCommandsZIndex = Panel.GetZIndex(windowCommands);
            var flyoutZIndex = Panel.GetZIndex(window.RightFlyout);

            Assert.True(windowCommandsZIndex > flyoutZIndex);
        }

        [Fact]
        [DisplayTestMethodName]
        public async Task WindowButtonCommandsAreOverFlyout()
        {
            await TestHost.SwitchToAppThread();

            var window = await WindowHelpers.CreateInvisibleWindowAsync<FlyoutWindow>();
            window.RightFlyout.IsOpen = true;

            var windowCommands = window.FindChild<ContentPresenter>("PART_WindowButtonCommands");
            Assert.NotNull(windowCommands);

            var windowCommandsZIndex = Panel.GetZIndex(windowCommands);
            var flyoutZIndex = Panel.GetZIndex(window.RightFlyout);

            Assert.True(windowCommandsZIndex > flyoutZIndex);
        }

        [Fact]
        [DisplayTestMethodName]
        public async Task RaisesIsOpenChangedEvent()
        {
            await TestHost.SwitchToAppThread();

            var window = await WindowHelpers.CreateInvisibleWindowAsync<FlyoutWindow>();

            var eventRaised = false;

            window.RightFlyout.IsOpenChanged += (sender, args) => { eventRaised = true; };

            window.RightFlyout.IsOpen = true;

            // IsOpen fires IsOpenChangedEvent with DispatcherPriority.Background
            await window.RightFlyout.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => Assert.True(eventRaised)));
        }

        [Fact]
        [DisplayTestMethodName]
        public async Task FindFlyoutWithFindChildren()
        {
            await TestHost.SwitchToAppThread();

            var window = await WindowHelpers.CreateInvisibleWindowAsync<FlyoutWindow>();

            var ex = Record.Exception(() =>
                {
                    var flyouts = (window.Content as DependencyObject).FindChildren<Flyout>(true);
                    var flyoutOnGrid = flyouts.FirstOrDefault(f => f.Name == "FlyoutOnGrid");
                    Assert.NotNull(flyoutOnGrid);
                });
            Assert.Null(ex);
        }

        [Fact]
        [DisplayTestMethodName]
        public async Task DefaultFlyoutThemeIsDark()
        {
            await TestHost.SwitchToAppThread();

            var window = await WindowHelpers.CreateInvisibleWindowAsync<FlyoutWindow>();

            Assert.Equal(FlyoutTheme.Dark, window.DefaultFlyout.Theme);
        }

        [Fact]
        [DisplayTestMethodName]
        public async Task WindowButtonCommandsForegroundBrushShouldBeAlwaysOverrideDefaultWindowCommandsBrush()
        {
            await TestHost.SwitchToAppThread();

            var window = await WindowHelpers.CreateInvisibleWindowAsync<FlyoutWindow>();
            window.OverrideDefaultWindowCommandsBrush = Brushes.Red;
            window.RightFlyout.IsOpen = true;

            Assert.Equal(Brushes.Red, window.WindowButtonCommands.Foreground);
            window.RightFlyout.IsOpen = false;

            Assert.Equal(Brushes.Red, window.WindowButtonCommands.Foreground);
        }

        [Fact]
        [DisplayTestMethodName]
        public async Task WindowCommandsForegroundBrushShouldBeAlwaysOverrideDefaultWindowCommandsBrush()
        {
            await TestHost.SwitchToAppThread();

            var window = await WindowHelpers.CreateInvisibleWindowAsync<FlyoutWindow>();
            window.OverrideDefaultWindowCommandsBrush = Brushes.Red;

            window.RightFlyout.IsOpen = true;
            Assert.Equal(Brushes.Red, window.RightWindowCommands.Foreground);

            window.RightFlyout.IsOpen = false;
            Assert.Equal(Brushes.Red, window.RightWindowCommands.Foreground);

            window.LeftFlyout.IsOpen = true;
            Assert.Equal(Brushes.Red, window.LeftWindowCommands.Foreground);

            window.LeftFlyout.IsOpen = false;
            Assert.Equal(Brushes.Red, window.LeftWindowCommands.Foreground);
        }
    }
}