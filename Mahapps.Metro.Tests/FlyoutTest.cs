using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
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

            var window = new FlyoutWindow();

            Assert.Equal(FlyoutTheme.Dark, window.RightFlyout.Theme);
        }

        [Fact]
        public async Task DefaultActualThemeIsDark()
        {
            await TestHost.SwitchToAppThread();

            var window = new FlyoutWindow
            {
                Visibility = Visibility.Hidden, 
                ShowInTaskbar = false
            };
            window.Show();

            await window.AwaitLoaded();

            Assert.Equal(Theme.Dark, window.RightFlyout.ActualTheme);
        }
    }
}
