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
    }
}
