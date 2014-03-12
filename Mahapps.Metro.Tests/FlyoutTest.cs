using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using MahApps.Metro;
using MahApps.Metro.Controls;
using Xunit;
using Xunit.Extensions;

namespace Mahapps.Metro.Tests
{
    public class FlyoutTest : AutomationTestBase
    {
        [Fact]
        public async Task DefaultFlyoutPositionIsLeft()
        {
            await TestHost.SwitchToAppThread();

            var window = await TestHelpers.CreateInvisibleWindowAsync<FlyoutWindow>();

            Assert.Equal(Position.Left, window.DefaultFlyout.Position);
        }
        
        [Fact]
        public async Task DefaultFlyoutThemeIsDark()
        {
            await TestHost.SwitchToAppThread();

            var window = await TestHelpers.CreateInvisibleWindowAsync<FlyoutWindow>();

            Assert.Equal(FlyoutTheme.Dark, window.DefaultFlyout.Theme);
        }
        
        [Fact]
        public async Task DefaultActualThemeIsDark()
        {
            await TestHost.SwitchToAppThread();

            var window = await TestHelpers.CreateInvisibleWindowAsync<FlyoutWindow>();

            Assert.Equal(Theme.Dark, window.DefaultFlyout.ActualTheme);
        }

        [Fact]
        public async Task WindowButtonCommandsAreOverFlyout()
        {
            await TestHost.SwitchToAppThread();

            var window = await TestHelpers.CreateInvisibleWindowAsync<FlyoutWindow>();

            int windowCommandsZIndex = Panel.GetZIndex(window.WindowButtonCommands);
            int flyoutindex = Panel.GetZIndex(window.RightFlyout);

            Assert.True(windowCommandsZIndex > flyoutindex);
        }

        [Fact]
        public async Task HiddenWindowCommandsAreBelowFlyout()
        {
            await TestHost.SwitchToAppThread();

            var window = await TestHelpers.CreateInvisibleWindowAsync<FlyoutWindow>();
            window.ShowWindowCommandsOnTop = false;

            int windowCommandsZIndex = Panel.GetZIndex(window.WindowButtonCommands);
            int flyoutindex = Panel.GetZIndex(window.RightFlyout);

            Assert.True(flyoutindex < windowCommandsZIndex);
        }

        [Fact]
        public async Task InverseFlyoutHasInverseWindowTheme()
        {
            await TestHost.SwitchToAppThread();

            var window = await TestHelpers.CreateInvisibleWindowAsync<FlyoutWindow>();
            window.DefaultFlyout.Theme = FlyoutTheme.Inverse;

            Assert.Equal(Theme.Dark, window.DefaultFlyout.ActualTheme);
        }

        [Fact]
        public async Task FlyoutRespondsToFlyoutThemeChange()
        {
            await TestHost.SwitchToAppThread();

            var window = await TestHelpers.CreateInvisibleWindowAsync<FlyoutWindow>();
            window.DefaultFlyout.Theme = FlyoutTheme.Light;

            Assert.Equal(Theme.Light, window.DefaultFlyout.ActualTheme);
        }

        [Fact]
        public async Task FlyoutIsClosedByDefault()
        {
            await TestHost.SwitchToAppThread();

            var window = await TestHelpers.CreateInvisibleWindowAsync<FlyoutWindow>();

            Assert.False(window.DefaultFlyout.IsOpen);
        }

        [Theory]
        [InlineData(FlyoutTheme.Dark, FlyoutTheme.Dark)]
        [InlineData(FlyoutTheme.Dark, FlyoutTheme.Light)]
        [InlineData(FlyoutTheme.Light, FlyoutTheme.Dark)]
        [InlineData(FlyoutTheme.Light, FlyoutTheme.Light)]
        public async Task ClosingFlyoutWithOtherFlyoutBelowHasCorrectWindowCommandsColor(
            FlyoutTheme underLyingFlyoutTheme, FlyoutTheme upperFlyoutTheme)
        {
            await TestHost.SwitchToAppThread();

            var window = await TestHelpers.CreateInvisibleWindowAsync<FlyoutWindow>();
            window.RightFlyout.Theme = underLyingFlyoutTheme;
            window.RightFlyout2.Theme = upperFlyoutTheme;

            window.RightFlyout.IsOpen = true;
            window.RightFlyout2.IsOpen = true;

            window.RightFlyout2.IsOpen = false;

            var expectedBrushColor = default(Color);

            switch (window.RightFlyout.ActualTheme)
            {
                case Theme.Dark:
                    expectedBrushColor = ((SolidColorBrush)ThemeManager.DarkResource["BlackBrush"]).Color;
                    break;

                case Theme.Light:
                    expectedBrushColor = ((SolidColorBrush)ThemeManager.LightResource["BlackBrush"]).Color;
                    break;
            }
            
            window.AssertWindowCommandsColor(expectedBrushColor);
        }

        [Fact]
        public async Task AdaptsWindowCommandsToDarkFlyout()
        {
            await TestHost.SwitchToAppThread();

            var window = await TestHelpers.CreateInvisibleWindowAsync<FlyoutWindow>();

            var flyout = new Flyout { Theme = FlyoutTheme.Dark };
            window.Flyouts.Items.Add(flyout);

            flyout.IsOpen = true;

            Color expectedColor = ((SolidColorBrush)ThemeManager.DarkResource["BlackBrush"]).Color;

            window.AssertWindowCommandsColor(expectedColor);
        }

        [Fact]
        public async Task FlyoutIsHiddenByDefault()
        {
            await TestHost.SwitchToAppThread();

            var window = await TestHelpers.CreateInvisibleWindowAsync<FlyoutWindow>();

            // find the root grid in the visual tree
            var rootGrid = window.DefaultFlyout.FindChildren<Grid>(true).FirstOrDefault();
            Assert.NotNull(rootGrid);
            // root grid should be hidden
            Assert.Equal(Visibility.Hidden, rootGrid.Visibility);
        }
    }
}
