﻿using System.Threading.Tasks;
using System.Windows.Media;
using MahApps.Metro.Tests.TestHelpers;
using Xunit;

namespace MahApps.Metro.Tests
{
    public class CleanWindowTest : AutomationTestBase
    {
        [Fact]
        [DisplayTestMethodName]
        public async Task DefaultWindowCommandColorIsBlack()
        {
            await TestHost.SwitchToAppThread();

            var window = await WindowHelpers.CreateInvisibleWindowAsync<CleanWindow>();

            var blackBrushColor = ((SolidColorBrush)ThemeManager.GetTheme("Light.Blue").Resources["BlackBrush"]).Color;

            window.AssertWindowCommandsColor(blackBrushColor);
        }
    }
}
