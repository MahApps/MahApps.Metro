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

            Accent expectedAccent = ThemeManager.Accents.First(x => x.Name == "Teal");
            AppTheme expectedTheme = ThemeManager.GetAppTheme("BaseDark");
            ThemeManager.ChangeAppStyle(Application.Current, expectedAccent, expectedTheme);

            var theme = ThemeManager.DetectAppStyle(window);

            Assert.Equal(Theme.Dark, theme.Item1.Theme);
            Assert.Equal(expectedTheme, theme.Item1);
            Assert.Equal(expectedAccent, theme.Item2);
        }
    }
}
