using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MahApps.Metro;
using MahApps.Metro.Controls;
using Xunit;

namespace Mahapps.Metro.Tests
{
    public class ThemeManagerTest : AutomationTestBase
    {
        [Fact]
        public async Task ChangesWindowTheme()
        {
            await TestHost.SwitchToAppThread();

            var window = new MetroWindow();
            window.Show();

            Accent expectedAccent = ThemeManager.DefaultAccents.First(x => x.Name == "Teal");
            ThemeManager.ChangeTheme(window, expectedAccent, Theme.Dark);

            var theme = ThemeManager.DetectTheme(window);

            Assert.Equal(Theme.Dark, theme.Item1);
            Assert.Equal(expectedAccent, theme.Item2);
        }
    }
}
