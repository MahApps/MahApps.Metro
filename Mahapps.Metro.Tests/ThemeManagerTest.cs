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

            Assert.Equal(expectedTheme, theme.Item1);
            Assert.Equal(expectedAccent, theme.Item2);
        }

        [Fact]
        public async Task CanAddAccentBeforeGetterIsCalled()
        {
            await TestHost.SwitchToAppThread();

            ThemeManager.AddAccent("TestAccent", new Uri("pack://application:,,,/MahApps.Metro;component/Styles/Accents/Blue.xaml"));
        }

        [Fact]
        public async Task CanAddAppThemeBeforeGetterIsCalled()
        {
            await TestHost.SwitchToAppThread();

            ThemeManager.AddAppTheme("TestTheme", new Uri("pack://application:,,,/MahApps.Metro;component/Styles/Accents/BaseDark.xaml"));
        }
    }
}
