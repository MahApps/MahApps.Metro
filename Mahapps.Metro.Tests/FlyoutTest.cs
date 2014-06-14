using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Mahapps.Metro.Tests.TestHelpers;
using MahApps.Metro;
using MahApps.Metro.Controls;
using Xunit;

namespace Mahapps.Metro.Tests
{
    public class FlyoutTest : AutomationTestBase
    {
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

        public class TheOverlayBehavior
        {
            [Fact]
            public async Task HiddenLeftWindowCommandsAreBelowFlyout()
            {
                await TestHost.SwitchToAppThread();

                var window = await WindowHelpers.CreateInvisibleWindowAsync<FlyoutWindow>();
                window.LeftWindowCommandsBehavior = WindowCommandsBehavior.Never;
                window.LeftFlyout.IsOpen = true;

                int windowCommandsZIndex = Panel.GetZIndex(window.LeftWindowCommands);
                int flyoutindex = Panel.GetZIndex(window.LeftFlyout);

                Assert.True(flyoutindex < windowCommandsZIndex);
            }

            [Fact]
            public async Task HiddenRightWindowCommandsAreBelowFlyout()
            {
                await TestHost.SwitchToAppThread();

                var window = await WindowHelpers.CreateInvisibleWindowAsync<FlyoutWindow>();
                window.RightWindowCommandsBehavior = WindowCommandsBehavior.Never;
                window.RightFlyout.IsOpen = true;

                int windowCommandsZIndex = Panel.GetZIndex(window.RightWindowCommands);
                int flyoutindex = Panel.GetZIndex(window.RightFlyout);

                Assert.True(flyoutindex < windowCommandsZIndex);
            }

            [Fact]
            public async Task HiddenWindowCommandsAreBelowFlyoutObsoleteTest()
            {
                await TestHost.SwitchToAppThread();

                var window = await WindowHelpers.CreateInvisibleWindowAsync<FlyoutWindow>();
                window.RightFlyout.IsOpen = true;
                window.ShowWindowCommandsOnTop = false;

                int windowCommandsZIndex = Panel.GetZIndex(window.WindowButtonCommands);
                int flyoutindex = Panel.GetZIndex(window.RightFlyout);

                Assert.True(flyoutindex < windowCommandsZIndex);
            }

            [Fact]
            public async Task LeftWindowCommandsAreOverFlyout()
            {
                await TestHost.SwitchToAppThread();

                var window = await WindowHelpers.CreateInvisibleWindowAsync<FlyoutWindow>();
                window.DefaultFlyout.IsOpen = true;

                int windowCommandsZIndex = Panel.GetZIndex(window.LeftWindowCommands);
                int flyoutindex = Panel.GetZIndex(window.DefaultFlyout);

                Assert.True(windowCommandsZIndex > flyoutindex);
            }

            [Fact]
            public async Task RightWindowCommandsAreOverFlyout()
            {
                await TestHost.SwitchToAppThread();

                var window = await WindowHelpers.CreateInvisibleWindowAsync<FlyoutWindow>();
                window.RightFlyout.IsOpen = true;

                int windowCommandsZIndex = Panel.GetZIndex(window.RightWindowCommands);
                int flyoutindex = Panel.GetZIndex(window.RightFlyout);

                Assert.True(windowCommandsZIndex > flyoutindex);
            }

            [Fact]
            public async Task WindowButtonCommandsAreOverFlyout()
            {
                await TestHost.SwitchToAppThread();

                var window = await WindowHelpers.CreateInvisibleWindowAsync<FlyoutWindow>();
                window.RightFlyout.IsOpen = true;

                int windowCommandsZIndex = Panel.GetZIndex(window.WindowButtonCommands);
                int flyoutindex = Panel.GetZIndex(window.RightFlyout);

                Assert.True(windowCommandsZIndex > flyoutindex);
            }
        }

        public class ThePositionProperty
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
        }
    }
}