using System.Threading.Tasks;
using System.Windows.Media;
using MahApps.Metro;
using MahApps.Metro.Tests.TestHelpers;
using Xunit;

namespace MahApps.Metro.Tests
{
    public class CleanWindowTest : AutomationTestBase
    {
        [Fact]
        public async Task DefaultWindowCommandColorIsBlack()
        {
            await TestHost.SwitchToAppThread();

            var window = await WindowHelpers.CreateInvisibleWindowAsync<CleanWindow>();

            var blackBrushColor = ((SolidColorBrush)ThemeManager.GetAppTheme("BaseLight").Resources["BlackBrush"]).Color;

            window.AssertWindowCommandsColor(blackBrushColor);
        }
    }
}
