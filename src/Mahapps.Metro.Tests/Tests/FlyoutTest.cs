// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
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
            window.Flyouts?.Items.Add(flyout);

            var completionSource = new TaskCompletionSource<bool>();

            void FlyoutOnOpeningFinished(object o, RoutedEventArgs routedEventArgs)
            {
                flyout.OpeningFinished -= FlyoutOnOpeningFinished;
                completionSource.SetResult(flyout.IsOpen);
            }

            flyout.OpeningFinished += FlyoutOnOpeningFinished;

            flyout.IsOpen = true;

            var opened = await completionSource.Task;
            Assert.True(opened);

            window.AssertWindowButtonCommandsColor(((SolidColorBrush)flyout.Foreground).Color);
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

            Assert.NotNull(window.Icon);
            var icon = window.FindChild<FrameworkElement>("PART_Icon");
            Assert.NotNull(icon);

            var flyout = window.LeftFlyout;

            var completionSource = new TaskCompletionSource<bool>();

            void FlyoutOnOpeningFinished(object o, RoutedEventArgs routedEventArgs)
            {
                flyout.OpeningFinished -= FlyoutOnOpeningFinished;
                completionSource.SetResult(flyout.IsOpen);
            }

            flyout.OpeningFinished += FlyoutOnOpeningFinished;

            flyout.IsOpen = true;

            var opened = await completionSource.Task;
            Assert.True(opened);

            var iconZIndex = Panel.GetZIndex(icon);
            var flyoutZIndex = Panel.GetZIndex(flyout);

            Assert.Equal(0, flyoutZIndex);
            Assert.Equal(1, iconZIndex);
            Assert.True(flyoutZIndex < iconZIndex);
        }

        [Fact]
        [DisplayTestMethodName]
        public async Task HiddenLeftWindowCommandsAreBelowFlyout()
        {
            await TestHost.SwitchToAppThread();
            var window = await WindowHelpers.CreateInvisibleWindowAsync<FlyoutWindow>();

            window.LeftWindowCommandsOverlayBehavior = WindowCommandsOverlayBehavior.Never;

            var windowCommands = window.FindChild<ContentPresenter>("PART_LeftWindowCommands");
            Assert.NotNull(windowCommands);

            var flyout = window.LeftFlyout;

            var completionSource = new TaskCompletionSource<bool>();

            void FlyoutOnOpeningFinished(object o, RoutedEventArgs routedEventArgs)
            {
                flyout.OpeningFinished -= FlyoutOnOpeningFinished;
                completionSource.SetResult(flyout.IsOpen);
            }

            flyout.OpeningFinished += FlyoutOnOpeningFinished;

            flyout.IsOpen = true;

            var opened = await completionSource.Task;
            Assert.True(opened);

            var windowCommandsZIndex = Panel.GetZIndex(windowCommands);
            var flyoutZIndex = Panel.GetZIndex(flyout);

            Assert.Equal(0, flyoutZIndex);
            Assert.Equal(1, windowCommandsZIndex);
            Assert.True(flyoutZIndex < windowCommandsZIndex);
        }

        [Fact]
        [DisplayTestMethodName]
        public async Task HiddenRightWindowCommandsAreBelowFlyout()
        {
            await TestHost.SwitchToAppThread();
            var window = await WindowHelpers.CreateInvisibleWindowAsync<FlyoutWindow>();

            window.RightWindowCommandsOverlayBehavior = WindowCommandsOverlayBehavior.Never;

            var windowCommands = window.FindChild<ContentPresenter>("PART_RightWindowCommands");
            Assert.NotNull(windowCommands);

            var flyout = window.RightFlyout;

            var completionSource = new TaskCompletionSource<bool>();

            void FlyoutOnOpeningFinished(object o, RoutedEventArgs routedEventArgs)
            {
                flyout.OpeningFinished -= FlyoutOnOpeningFinished;
                completionSource.SetResult(flyout.IsOpen);
            }

            flyout.OpeningFinished += FlyoutOnOpeningFinished;

            flyout.IsOpen = true;

            var opened = await completionSource.Task;
            Assert.True(opened);

            var windowCommandsZIndex = Panel.GetZIndex(windowCommands);
            var flyoutZIndex = Panel.GetZIndex(flyout);

            Assert.Equal(0, flyoutZIndex);
            Assert.Equal(1, windowCommandsZIndex);
            Assert.True(flyoutZIndex < windowCommandsZIndex);
        }

        [Fact]
        [DisplayTestMethodName]
        public async Task WindowButtonCommandsAreOverFlyout()
        {
            await TestHost.SwitchToAppThread();
            var window = await WindowHelpers.CreateInvisibleWindowAsync<FlyoutWindow>();

            var windowCommands = window.FindChild<ContentPresenter>("PART_WindowButtonCommands");
            Assert.NotNull(windowCommands);

            var flyout = window.RightFlyout;

            var completionSource = new TaskCompletionSource<bool>();

            void FlyoutOnOpeningFinished(object o, RoutedEventArgs routedEventArgs)
            {
                flyout.OpeningFinished -= FlyoutOnOpeningFinished;
                completionSource.SetResult(flyout.IsOpen);
            }

            flyout.OpeningFinished += FlyoutOnOpeningFinished;

            flyout.IsOpen = true;

            var opened = await completionSource.Task;
            Assert.True(opened);

            var windowCommandsZIndex = Panel.GetZIndex(windowCommands);
            var flyoutZIndex = Panel.GetZIndex(flyout);

            Assert.True(windowCommandsZIndex > flyoutZIndex);
        }

        [Fact]
        [DisplayTestMethodName]
        public async Task RaisesIsOpenChangedEvent()
        {
            await TestHost.SwitchToAppThread();
            var window = await WindowHelpers.CreateInvisibleWindowAsync<FlyoutWindow>();

            var flyout = window.RightFlyout;

            var completionSource = new TaskCompletionSource<bool>();

            void FlyoutOnIsOpenChanged(object o, RoutedEventArgs routedEventArgs)
            {
                flyout.IsOpenChanged -= FlyoutOnIsOpenChanged;
                completionSource.SetResult(flyout.IsOpen);
            }

            flyout.IsOpenChanged += FlyoutOnIsOpenChanged;

            flyout.IsOpen = true;

            var opened = await completionSource.Task;
            Assert.True(opened);
        }

        [Fact]
        [DisplayTestMethodName]
        public async Task RaisesClosingFinishedEvent()
        {
            await TestHost.SwitchToAppThread();
            var window = await WindowHelpers.CreateInvisibleWindowAsync<FlyoutWindow>();

            var flyout = window.RightFlyout;

            var openingCompletionSource = new TaskCompletionSource<bool>();
            var closingCompletionSource = new TaskCompletionSource<bool>();

            void FlyoutOnOpeningFinished(object o, RoutedEventArgs routedEventArgs)
            {
                flyout.OpeningFinished -= FlyoutOnOpeningFinished;
                openingCompletionSource.SetResult(flyout.IsOpen);
            }

            void FlyoutOnClosingFinished(object o, RoutedEventArgs routedEventArgs)
            {
                flyout.ClosingFinished -= FlyoutOnClosingFinished;
                closingCompletionSource.SetResult(!flyout.IsOpen);
            }

            flyout.OpeningFinished += FlyoutOnOpeningFinished;
            flyout.ClosingFinished += FlyoutOnClosingFinished;

            flyout.IsOpen = true;

            var opened = await openingCompletionSource.Task;
            Assert.True(opened);

            flyout.IsOpen = false;

            var closed = await closingCompletionSource.Task;
            Assert.True(closed);
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
                    var flyoutOnGrid = flyouts.FirstOrDefault(f => f.Name == nameof(window.FlyoutOnGrid));
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

            Assert.NotNull(window.WindowButtonCommands);

            var brush = Brushes.Red;

            window.OverrideDefaultWindowCommandsBrush = brush;

            var flyout = window.RightFlyout;

            var openingCompletionSource = new TaskCompletionSource<bool>();
            var closingCompletionSource = new TaskCompletionSource<bool>();

            void FlyoutOnOpeningFinished(object o, RoutedEventArgs routedEventArgs)
            {
                flyout.OpeningFinished -= FlyoutOnOpeningFinished;
                openingCompletionSource.SetResult(flyout.IsOpen);
            }

            void FlyoutOnClosingFinished(object o, RoutedEventArgs routedEventArgs)
            {
                flyout.ClosingFinished -= FlyoutOnClosingFinished;
                closingCompletionSource.SetResult(!flyout.IsOpen);
            }

            flyout.OpeningFinished += FlyoutOnOpeningFinished;
            flyout.ClosingFinished += FlyoutOnClosingFinished;

            flyout.IsOpen = true;

            var opened = await openingCompletionSource.Task;
            Assert.True(opened);

            Assert.Equal(brush, window.WindowButtonCommands.Foreground);

            flyout.IsOpen = false;

            var closed = await closingCompletionSource.Task;
            Assert.True(closed);

            Assert.Equal(brush, window.WindowButtonCommands.Foreground);
        }

        [Fact]
        [DisplayTestMethodName]
        public async Task LeftWindowCommandsForegroundBrushShouldAlwaysOverrideDefaultWindowCommandsBrush()
        {
            await TestHost.SwitchToAppThread();
            var window = await WindowHelpers.CreateInvisibleWindowAsync<FlyoutWindow>();

            Assert.NotNull(window.LeftWindowCommands);

            var brush = Brushes.Red;

            window.OverrideDefaultWindowCommandsBrush = brush;

            var flyout = window.LeftFlyout;

            var openingCompletionSource = new TaskCompletionSource<bool>();
            var closingCompletionSource = new TaskCompletionSource<bool>();

            void FlyoutOnOpeningFinished(object o, RoutedEventArgs routedEventArgs)
            {
                flyout.OpeningFinished -= FlyoutOnOpeningFinished;
                openingCompletionSource.SetResult(flyout.IsOpen);
            }

            void FlyoutOnClosingFinished(object o, RoutedEventArgs routedEventArgs)
            {
                flyout.ClosingFinished -= FlyoutOnClosingFinished;
                closingCompletionSource.SetResult(!flyout.IsOpen);
            }

            flyout.OpeningFinished += FlyoutOnOpeningFinished;
            flyout.ClosingFinished += FlyoutOnClosingFinished;

            flyout.IsOpen = true;

            var opened = await openingCompletionSource.Task;
            Assert.True(opened);

            Assert.Equal(brush, window.LeftWindowCommands.Foreground);

            flyout.IsOpen = false;

            var closed = await closingCompletionSource.Task;
            Assert.True(closed);

            Assert.Equal(brush, window.LeftWindowCommands.Foreground);
        }

        [Fact]
        [DisplayTestMethodName]
        public async Task RightWindowCommandsForegroundBrushShouldAlwaysOverrideDefaultWindowCommandsBrush()
        {
            await TestHost.SwitchToAppThread();
            var window = await WindowHelpers.CreateInvisibleWindowAsync<FlyoutWindow>();

            Assert.NotNull(window.RightWindowCommands);

            var brush = Brushes.Red;

            window.OverrideDefaultWindowCommandsBrush = brush;

            var flyout = window.RightFlyout;

            var openingCompletionSource = new TaskCompletionSource<bool>();
            var closingCompletionSource = new TaskCompletionSource<bool>();

            void FlyoutOnOpeningFinished(object o, RoutedEventArgs routedEventArgs)
            {
                flyout.OpeningFinished -= FlyoutOnOpeningFinished;
                openingCompletionSource.SetResult(flyout.IsOpen);
            }

            void FlyoutOnClosingFinished(object o, RoutedEventArgs routedEventArgs)
            {
                flyout.ClosingFinished -= FlyoutOnClosingFinished;
                closingCompletionSource.SetResult(!flyout.IsOpen);
            }

            flyout.OpeningFinished += FlyoutOnOpeningFinished;
            flyout.ClosingFinished += FlyoutOnClosingFinished;

            flyout.IsOpen = true;

            var opened = await openingCompletionSource.Task;
            Assert.True(opened);

            Assert.Equal(brush, window.RightWindowCommands.Foreground);

            flyout.IsOpen = false;

            var closed = await closingCompletionSource.Task;
            Assert.True(closed);

            Assert.Equal(brush, window.RightWindowCommands.Foreground);
        }
    }
}