using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using MahApps.Metro;
using Xunit;

namespace Mahapps.Metro.Tests
{
    public class CleanWindowTest : AutomationTestBase
    {
        [Fact]
        public async Task DefaultWindowCommandColorIsBlack()
        {
            await TestHost.SwitchToAppThread();

            var window = await TestHelpers.CreateInvisibleWindowAsync<CleanWindow>();

            Assert.Equal((SolidColorBrush)ThemeManager.LightResource["BlackBrush"], (SolidColorBrush) window.WindowCommands.Foreground);
        }
    }
}
