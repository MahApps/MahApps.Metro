using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using ExposedObject;
using MahApps.Metro.Tests.TestHelpers;
using MahApps.Metro;
using MahApps.Metro.Controls;
using Xunit;

namespace MahApps.Metro.Tests
{
    public class FlyoutTest : AutomationTestBase
    {
        [Fact]
        public async Task AdaptsWindowCommandsToDarkFlyout()
        {
            await TestHost.SwitchToAppThread();

            var window = await WindowHelpers.CreateInvisibleWindowAsync<FlyoutWindow>();

            var flyout = new Flyout { Theme = FlyoutTheme.Dark };
            window.Flyouts.Items.Add(flyout);

            flyout.IsOpen = true;

            Color expectedColor = ((SolidColorBrush)ThemeManager.GetAppTheme("BaseDark").Resources["BlackBrush"]).Color;

            window.AssertWindowCommandsColor(expectedColor);
        }

        [Fact]
        public async Task DefaultFlyoutPositionIsLeft()
        {
            await TestHost.SwitchToAppThread();

            var window = await WindowHelpers.CreateInvisibleWindowAsync<FlyoutWindow>();

            Assert.Equal(Position.Left, window.DefaultFlyout.Position);
        }

        [Fact]
        public async Task FlyoutIsClosedByDefault()
        {
            await TestHost.SwitchToAppThread();

            var window = await WindowHelpers.CreateInvisibleWindowAsync<FlyoutWindow>();

            Assert.False(window.DefaultFlyout.IsOpen);
        }

        [Fact]
        public async Task FlyoutIsHiddenByDefault()
        {
            await TestHost.SwitchToAppThread();

            var window = await WindowHelpers.CreateInvisibleWindowAsync<FlyoutWindow>();

            // root grid should be hidden
            Assert.Equal(Visibility.Hidden, window.DefaultFlyout.Visibility);
        }

        [Fact]
        public async Task HiddenIconIsBelowFlyout()
        {
            await TestHost.SwitchToAppThread();

            var window = await WindowHelpers.CreateInvisibleWindowAsync<FlyoutWindow>();
            window.LeftWindowCommandsOverlayBehavior = WindowCommandsOverlayBehavior.Never;
            window.LeftFlyout.IsOpen = true;

            var exposedWindow = Exposed.From(window);
            int windowCommandsZIndex = Panel.GetZIndex(exposedWindow.icon);
            int flyoutindex = Panel.GetZIndex(window.LeftFlyout);

            Assert.True(flyoutindex < windowCommandsZIndex);
        }

        [Fact]
        public async Task HiddenLeftWindowCommandsAreBelowFlyout()
        {
            await TestHost.SwitchToAppThread();

            var window = await WindowHelpers.CreateInvisibleWindowAsync<FlyoutWindow>();
            window.LeftWindowCommandsOverlayBehavior = WindowCommandsOverlayBehavior.Never;
            window.LeftFlyout.IsOpen = true;

            var exposedWindow = Exposed.From(window);
            int windowCommandsZIndex = Panel.GetZIndex(exposedWindow.LeftWindowCommandsPresenter);
            int flyoutindex = Panel.GetZIndex(window.LeftFlyout);

            Assert.True(flyoutindex < windowCommandsZIndex);
        }

        [Fact]
        public async Task HiddenRightWindowCommandsAreBelowFlyout()
        {
            await TestHost.SwitchToAppThread();

            var window = await WindowHelpers.CreateInvisibleWindowAsync<FlyoutWindow>();
            window.RightWindowCommandsOverlayBehavior = WindowCommandsOverlayBehavior.Never;
            window.RightFlyout.IsOpen = true;

            var exposedWindow = Exposed.From(window);
            int windowCommandsZIndex = Panel.GetZIndex(exposedWindow.RightWindowCommandsPresenter);
            int flyoutindex = Panel.GetZIndex(window.RightFlyout);

            Assert.True(flyoutindex < windowCommandsZIndex);
        }

        [Fact]
        public async Task LeftWindowCommandsAreOverFlyout()
        {
            await TestHost.SwitchToAppThread();

            var window = await WindowHelpers.CreateInvisibleWindowAsync<FlyoutWindow>();
            window.LeftFlyout.IsOpen = true;

            var exposedWindow = Exposed.From(window);
            int windowCommandsZIndex = Panel.GetZIndex(exposedWindow.LeftWindowCommandsPresenter);
            int flyoutindex = Panel.GetZIndex(window.LeftFlyout);

            Assert.True(windowCommandsZIndex > flyoutindex);
        }

        [Fact]
        public async Task RightWindowCommandsAreOverFlyout()
        {
            await TestHost.SwitchToAppThread();

            var window = await WindowHelpers.CreateInvisibleWindowAsync<FlyoutWindow>();
            window.RightFlyout.IsOpen = true;

            var exposedWindow = Exposed.From(window);
            int windowCommandsZIndex = Panel.GetZIndex(exposedWindow.RightWindowCommandsPresenter);
            int flyoutindex = Panel.GetZIndex(window.RightFlyout);

            Assert.True(windowCommandsZIndex > flyoutindex);
        }

        [Fact]
        public async Task WindowButtonCommandsAreOverFlyout()
        {
            await TestHost.SwitchToAppThread();

            var window = await WindowHelpers.CreateInvisibleWindowAsync<FlyoutWindow>();
            window.RightFlyout.IsOpen = true;

            int windowCommandsZIndex = Panel.GetZIndex(window.FindChild<WindowButtonCommands>("PART_WindowButtonCommands"));
            int flyoutindex = Panel.GetZIndex(window.RightFlyout);

            Assert.True(windowCommandsZIndex > flyoutindex);
        }

        [Fact]
        public async Task RaisesIsOpenChangedEvent()
        {
            await TestHost.SwitchToAppThread();

            var window = await WindowHelpers.CreateInvisibleWindowAsync<FlyoutWindow>();

            bool eventRaised = false;

            window.RightFlyout.IsOpenChanged += (sender, args) => {
                eventRaised = true;
            };

            window.RightFlyout.IsOpen = true;

            // IsOpen fires IsOpenChangedEvent with DispatcherPriority.Background
            window.RightFlyout.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => Assert.True(eventRaised)));
        }

        public class ColorTest
        {
            [Fact]
            public async Task DefaultFlyoutThemeIsDark()
            {
                await TestHost.SwitchToAppThread();

                var window = await WindowHelpers.CreateInvisibleWindowAsync<FlyoutWindow>();

                Assert.Equal(FlyoutTheme.Dark, window.DefaultFlyout.Theme);
            }
        }
    }
}