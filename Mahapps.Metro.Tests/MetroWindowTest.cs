using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using MahApps.Metro;
using MahApps.Metro.Controls;
using Xunit;

namespace Mahapps.Metro.Tests
{
    public class MetroWindowTest : AutomationTestBase
    {
        [Fact]
        public async Task MetroWindowSmokeTest()
        {
            await TestHost.SwitchToAppThread();

            await TestHelpers.CreateInvisibleWindowAsync<MetroWindow>();
        }

        [Fact]
        public async Task ShowsWindowCommandsOnTopByDefault()
        {
            await TestHost.SwitchToAppThread();

            var window = new MetroWindow();

            Assert.True(window.ShowWindowCommandsOnTop);
        }

        [Fact]
        public async Task AdaptsWindowCommandsToDarkFlyout()
        {
            await TestHost.SwitchToAppThread();

            var window = await TestHelpers.CreateInvisibleWindowAsync<MetroWindow>();

            var flyout = new Flyout { Theme = FlyoutTheme.Dark };
            window.Flyouts.Items.Add(flyout);

            flyout.IsOpen = true;

            Assert.Equal(((SolidColorBrush)ThemeManager.DarkResource["BlackBrush"]).Color, ((SolidColorBrush)window.WindowButtonCommands.Foreground).Color);
        }
    }
}
