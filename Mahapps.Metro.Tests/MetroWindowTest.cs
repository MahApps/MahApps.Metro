using System.Threading.Tasks;
using System.Windows;
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

            Window window = new MetroWindow();
            window.Visibility = Visibility.Hidden;
            window.ShowInTaskbar = false;
            window.Show();
        }
    }
}
