using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using MahApps.Metro;
using MahApps.Metro.Controls;
using Xunit;

namespace Mahapps.Metro.Tests
{
    public class FlyoutTest : AutomationTestBase
    {
        [Fact]
        public async Task DefaultFlyoutThemeIsDark()
        {
            await TestHost.SwitchToAppThread();

            var window = await TestHelpers.CreateInvisibleWindowAsync<FlyoutWindow>();

            Assert.Equal(FlyoutTheme.Dark, window.RightFlyout.Theme);
        }

        [Fact]
        public async Task DefaultActualThemeIsDark()
        {
            await TestHost.SwitchToAppThread();

            var window = await TestHelpers.CreateInvisibleWindowAsync<FlyoutWindow>();

            await window.AwaitLoaded();

            Assert.Equal(Theme.Dark, window.RightFlyout.ActualTheme);
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
    }
}
