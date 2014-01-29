using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
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

            var window = await TestHelpers.CreateInvisibleWindowAsync<MetroWindow>();

            Accent expectedAccent = ThemeManager.DefaultAccents.First(x => x.Name == "Teal");
            ThemeManager.ChangeTheme(Application.Current, expectedAccent, Theme.Dark);

            var theme = ThemeManager.DetectTheme(window);

            Assert.Equal(Theme.Dark, theme.Item1);
            Assert.Equal(expectedAccent, theme.Item2);
        }
    }
}
