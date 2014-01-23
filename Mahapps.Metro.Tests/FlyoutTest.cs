using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
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
        [InlineData(FlyoutTheme.Dark, FlyoutTheme.Dark, Theme.Dark, "BlackBrush")]
        [InlineData(FlyoutTheme.Dark, FlyoutTheme.Light, Theme.Dark, "BlackBrush")]
        [InlineData(FlyoutTheme.Light, FlyoutTheme.Dark, Theme.Light, "BlackBrush")]
        [InlineData(FlyoutTheme.Light, FlyoutTheme.Light, Theme.Light, "BlackBrush")]
        public async Task ClosingFlyoutWithOtherFlyoutBelowHasCorrectWindowCommandsColor(
            FlyoutTheme belowTheme, FlyoutTheme upperTheme, Theme themeLookup, string brush)
        {
            await TestHost.SwitchToAppThread();

            var window = await TestHelpers.CreateInvisibleWindowAsync<FlyoutWindow>();
            window.RightFlyout.Theme = FlyoutTheme.Light;
            window.RightFlyout2.Theme = FlyoutTheme.Light;

            window.RightFlyout.IsOpen = true;
            window.RightFlyout2.IsOpen = true;

            window.RightFlyout2.IsOpen = false;

            var brushColor = default(Color);

            switch (themeLookup)
            {
                case Theme.Dark:
                    brushColor = ((SolidColorBrush)ThemeManager.DarkResource[brush]).Color;
                    break;

                case Theme.Light:
                    brushColor = ((SolidColorBrush)ThemeManager.LightResource[brush]).Color;
                    break;
            }

            Assert.Equal(brushColor, ((SolidColorBrush)window.WindowCommands.Foreground).Color);
            Assert.Equal(brushColor, ((SolidColorBrush)window.WindowButtonCommands.Foreground).Color);
        }
    }
}
