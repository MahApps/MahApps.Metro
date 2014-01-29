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

            var blackBrushColor = ((SolidColorBrush)ThemeManager.LightResource["BlackBrush"]).Color;

            Assert.Equal(blackBrushColor, ((SolidColorBrush) window.WindowCommands.Foreground).Color);
            Assert.Equal(blackBrushColor, ((SolidColorBrush) window.WindowButtonCommands.Foreground).Color);
        }
    }
}
