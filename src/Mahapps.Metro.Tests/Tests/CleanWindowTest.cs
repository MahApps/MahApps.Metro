// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Threading.Tasks;
using System.Windows.Media;
using ControlzEx.Theming;
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

            var theme = ThemeManager.Current.DetectTheme();
            Assert.NotNull(theme);

            var brush = theme.Resources["MahApps.Brushes.ThemeForeground"] as SolidColorBrush;
            Assert.NotNull(brush);

            var blackBrushColor = brush.Color;

            window.AssertWindowCommandsColor(blackBrushColor);
        }
    }
}