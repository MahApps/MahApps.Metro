using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using MahApps.Metro.Tests.TestHelpers;
using MahApps.Metro.Controls;
using Xunit;

namespace MahApps.Metro.Tests
{
    public class ThemeManagerTest : AutomationTestBase
    {
        [Fact]
        [DisplayTestMethodName]
        public async Task ChangeAppStyleForAppShouldThrowArgumentNullException()
        {
            await TestHost.SwitchToAppThread();

            Assert.Throws<ArgumentNullException>(() => ThemeManager.ChangeAppStyle((Application)null, ThemeManager.GetAccent("Red"), ThemeManager.GetAppTheme("BaseLight")));
            Assert.Throws<ArgumentNullException>(() => ThemeManager.ChangeAppStyle(Application.Current, ThemeManager.GetAccent("Red"), ThemeManager.GetAppTheme("UnknownTheme")));
            Assert.Throws<ArgumentNullException>(() => ThemeManager.ChangeAppStyle(Application.Current, ThemeManager.GetAccent("UnknownAccentColor"), ThemeManager.GetAppTheme("BaseLight")));
        }

        [Fact]
        [DisplayTestMethodName]
        public async Task ChangeAppStyleForWindowShouldThrowArgumentNullException()
        {
            await TestHost.SwitchToAppThread();

            await WindowHelpers.CreateInvisibleWindowAsync<MetroWindow>();

            Assert.Throws<ArgumentNullException>(() => ThemeManager.ChangeAppStyle((Window)null, ThemeManager.GetAccent("Red"), ThemeManager.GetAppTheme("BaseLight")));
            Assert.Throws<ArgumentNullException>(() => ThemeManager.ChangeAppStyle(Application.Current.MainWindow, ThemeManager.GetAccent("Red"), ThemeManager.GetAppTheme("UnknownTheme")));
            Assert.Throws<ArgumentNullException>(() => ThemeManager.ChangeAppStyle(Application.Current.MainWindow, ThemeManager.GetAccent("UnknownAccentColor"), ThemeManager.GetAppTheme("BaseLight")));
        }

        [Fact]
        [DisplayTestMethodName]
        public async Task CanAddAccentBeforeGetterIsCalled()
        {
            await TestHost.SwitchToAppThread();

            Assert.True(ThemeManager.AddAccent("TestAccent", new Uri("pack://application:,,,/MahApps.Metro;component/Styles/Accents/Blue.xaml")));
        }

        [Fact]
        [DisplayTestMethodName]
        public async Task CanAddAppThemeBeforeGetterIsCalled()
        {
            await TestHost.SwitchToAppThread();

            Assert.True(ThemeManager.AddAppTheme("TestTheme", new Uri("pack://application:,,,/MahApps.Metro;component/Styles/Accents/BaseDark.xaml")));
        }

        [Fact]
        [DisplayTestMethodName]
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
        [DisplayTestMethodName]
        public async Task GetInverseAppThemeReturnsDarkTheme()
        {
            await TestHost.SwitchToAppThread();

            AppTheme theme = ThemeManager.GetInverseAppTheme(ThemeManager.GetAppTheme("BaseLight"));

            Assert.Equal("BaseDark", theme.Name);
        }

        [Fact]
        [DisplayTestMethodName]
        public async Task GetInverseAppThemeReturnsLightTheme()
        {
            await TestHost.SwitchToAppThread();

            AppTheme theme = ThemeManager.GetInverseAppTheme(ThemeManager.GetAppTheme("BaseDark"));

            Assert.Equal("BaseLight", theme.Name);
        }

        [Fact]
        [DisplayTestMethodName]
        public async Task GetInverseAppThemeReturnsNullForMissingTheme()
        {
            await TestHost.SwitchToAppThread();

            var appTheme = new AppTheme("TestTheme", new Uri("pack://application:,,,/MahApps.Metro;component/Styles/Accents/BaseDark.xaml"));

            AppTheme theme = ThemeManager.GetInverseAppTheme(appTheme);

            Assert.Null(theme);
        }

        [Fact]
        [DisplayTestMethodName]
        public async Task GetAppThemeIsCaseInsensitive()
        {
            await TestHost.SwitchToAppThread();

            AppTheme theme = ThemeManager.GetAppTheme("basedark");

            Assert.NotNull(theme);
            Assert.Equal(new Uri("pack://application:,,,/MahApps.Metro;component/Styles/Accents/BaseDark.xaml"), theme.Resources.Source);
        }

        [Fact]
        [DisplayTestMethodName]
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
        [DisplayTestMethodName]
        public async Task GetAccentIsCaseInsensitive()
        {
            await TestHost.SwitchToAppThread();

            Accent accent = ThemeManager.GetAccent("blue");

            Assert.NotNull(accent);
            Assert.Equal(new Uri("pack://application:,,,/MahApps.Metro;component/Styles/Accents/Blue.xaml"), accent.Resources.Source);
        }

        [Fact]
        [DisplayTestMethodName]
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

        [Fact]
        [DisplayTestMethodName]
        public async Task CreateDynamicAccentWithColor()
        {
            await TestHost.SwitchToAppThread();

            var applicationTheme = ThemeManager.DetectAppStyle(Application.Current);

            var ex = Record.Exception(() => AccentHelper.ApplyColor(Colors.Red, "CustomAccentRed"));
            Assert.Null(ex);
            
            var detected = ThemeManager.DetectAppStyle(Application.Current);
            Assert.NotNull(detected);
            Assert.Equal("CustomAccentRed", detected.Item2.Name);

            ex = Record.Exception(() => AccentHelper.ApplyColor(Colors.Green, "CustomAccentGreen"));
            Assert.Null(ex);
            
            detected = ThemeManager.DetectAppStyle(Application.Current);
            Assert.NotNull(detected);
            Assert.Equal("CustomAccentGreen", detected.Item2.Name);

            ThemeManager.ChangeAppStyle(Application.Current, applicationTheme.Item2, applicationTheme.Item1);
        }
    }
}