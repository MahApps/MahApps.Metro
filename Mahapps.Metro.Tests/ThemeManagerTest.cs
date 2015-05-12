using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using MahApps.Metro.Tests.TestHelpers;
using MahApps.Metro.Controls;
using Xunit;

namespace MahApps.Metro.Tests
{
    public class ThemeManagerTest : AutomationTestBase
    {
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

        [Fact]
        public async Task ChangesWindowTheme()
        {
            await TestHost.SwitchToAppThread();

            var window = await WindowHelpers.CreateInvisibleWindowAsync<MetroWindow>();

            Accent expectedAccent = ThemeManager.Accents.First(x => x.Name == "Teal");
            AppTheme expectedTheme = ThemeManager.GetAppTheme("BaseDark");
            ThemeManager.ChangeAppStyle(Application.Current, expectedAccent, expectedTheme);

            var theme = ThemeManager.DetectAppStyle(window);

            Assert.Equal(expectedTheme, theme.Item1);
            Assert.Equal(expectedAccent, theme.Item2);
        }

        [Fact]
        public async Task GetInverseAppThemeReturnsDarkTheme()
        {
            await TestHost.SwitchToAppThread();

            AppTheme theme = ThemeManager.GetInverseAppTheme(ThemeManager.GetAppTheme("BaseLight"));

            Assert.Equal("BaseDark", theme.Name);
        }

        [Fact]
        public async Task GetInverseAppThemeReturnsLightTheme()
        {
            await TestHost.SwitchToAppThread();

            AppTheme theme = ThemeManager.GetInverseAppTheme(ThemeManager.GetAppTheme("BaseDark"));

            Assert.Equal("BaseLight", theme.Name);
        }

        [Fact]
        public async Task GetInverseAppThemeReturnsNullForMissingTheme()
        {
            await TestHost.SwitchToAppThread();

            var appTheme = new AppTheme("TestTheme", new Uri("pack://application:,,,/MahApps.Metro;component/Styles/Accents/BaseDark.xaml"));

            AppTheme theme = ThemeManager.GetInverseAppTheme(appTheme);

            Assert.Null(theme);
        }

        [Fact]
        public async Task GetAppThemeIsCaseInsensitive()
        {
            await TestHost.SwitchToAppThread();

            AppTheme theme = ThemeManager.GetAppTheme("basedark");

            Assert.NotNull(theme);
            Assert.Equal(new Uri("pack://application:,,,/MahApps.Metro;component/Styles/Accents/BaseDark.xaml"), theme.Resources.Source);
        }

        [Fact]
        public async Task GetAppThemeWithUriIsCaseInsensitive()
        {
            await TestHost.SwitchToAppThread();

            var dic = new ResourceDictionary
            {
                Source = new Uri("pack://application:,,,/MahApps.Metro;component/Styles/Accents/basedark.xaml")
            };

            AppTheme detected = ThemeManager.GetAppTheme(dic);

            Assert.NotNull(detected);
            Assert.Equal("BaseDark", detected.Name);
        }

        [Fact]
        public async Task GetAccentIsCaseInsensitive()
        {
            await TestHost.SwitchToAppThread();

            Accent accent = ThemeManager.GetAccent("blue");

            Assert.NotNull(accent);
            Assert.Equal(new Uri("pack://application:,,,/MahApps.Metro;component/Styles/Accents/Blue.xaml"), accent.Resources.Source);
        }

        [Fact]
        public async Task GetAccentWithUriIsCaseInsensitive()
        {
            await TestHost.SwitchToAppThread();

            var dic = new ResourceDictionary
            {
                Source = new Uri("pack://application:,,,/MahApps.Metro;component/Styles/Accents/blue.xaml")
            };

            Accent detected = ThemeManager.GetAccent(dic);

            Assert.NotNull(detected);
            Assert.Equal("Blue", detected.Name);
        }
    }
}