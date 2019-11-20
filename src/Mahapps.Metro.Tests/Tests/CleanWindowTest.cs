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

            var window = await WindowHelpers.CreateInvisibleWindowAsync<CleanWindow>().ConfigureAwait(false);

            var theme = ThemeManager.DetectTheme();
            Assert.NotNull(theme);

            var brush = theme.Resources["MahApps.Brushes.Black"] as SolidColorBrush;
            Assert.NotNull(brush);

            var blackBrushColor = brush.Color;

            window.AssertWindowCommandsColor(blackBrushColor);
        }
    }
}