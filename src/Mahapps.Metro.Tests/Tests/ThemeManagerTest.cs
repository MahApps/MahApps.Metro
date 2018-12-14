using System;
using System.Threading.Tasks;
using System.Windows;
using MahApps.Metro.Tests.TestHelpers;
using MahApps.Metro.Controls;
using Xunit;

namespace MahApps.Metro.Tests
{
    using System.Windows.Media;

    public class ThemeManagerTest : AutomationTestBase
    {
        [Fact]
        [DisplayTestMethodName]
        public async Task ChangeThemeForAppShouldThrowArgumentNullException()
        {
            await TestHost.SwitchToAppThread();

            Assert.Throws<ArgumentNullException>(() => ThemeManager.ChangeTheme((Application)null, ThemeManager.GetTheme("Light.Red")));
            Assert.Throws<ArgumentNullException>(() => ThemeManager.ChangeTheme(Application.Current, ThemeManager.GetTheme("UnknownTheme")));
        }

        [Fact]
        [DisplayTestMethodName]
        public async Task ChangeThemeForWindowShouldThrowArgumentNullException()
        {
            await TestHost.SwitchToAppThread();

            await WindowHelpers.CreateInvisibleWindowAsync<MetroWindow>();

            Assert.Throws<ArgumentNullException>(() => ThemeManager.ChangeTheme((Window)null, ThemeManager.GetTheme("Light.Red")));
            Assert.Throws<ArgumentNullException>(() => ThemeManager.ChangeTheme(Application.Current.MainWindow, ThemeManager.GetTheme("UnknownTheme")));
        }

        [Fact]
        [DisplayTestMethodName]
        public async Task CanAddThemeBeforeGetterIsCalled()
        {
            await TestHost.SwitchToAppThread();

            Assert.False(ThemeManager.AddTheme(new Uri("pack://application:,,,/MahApps.Metro;component/Styles/Themes/Dark.Cobalt.xaml")));

            var resource = new ResourceDictionary
                           {
                               {
                                   "Theme.Name", "Runtime"
                               },
                               {
                                   "Theme.DisplayName", "Runtime"
                               }
                           };
            Assert.True(ThemeManager.AddTheme(resource));
        }

        [Fact]
        [DisplayTestMethodName]
        public async Task ChangesWindowTheme()
        {
            await TestHost.SwitchToAppThread();

            var window = await WindowHelpers.CreateInvisibleWindowAsync<MetroWindow>();

            var expectedTheme = ThemeManager.GetTheme("Dark.Teal");
            ThemeManager.ChangeTheme(Application.Current, expectedTheme);

            var theme = ThemeManager.DetectTheme(window);

            Assert.Equal(expectedTheme, theme);
        }

        [Fact]
        [DisplayTestMethodName]
        public async Task GetInverseAppThemeReturnsDarkTheme()
        {
            await TestHost.SwitchToAppThread();

            var theme = ThemeManager.GetInverseTheme(ThemeManager.GetTheme("Light.Blue"));

            Assert.Equal("Dark.Blue", theme.Name);
        }

        [Fact]
        [DisplayTestMethodName]
        public async Task GetInverseThemeReturnsLightTheme()
        {
            await TestHost.SwitchToAppThread();

            var theme = ThemeManager.GetInverseTheme(ThemeManager.GetTheme("Dark.Blue"));

            Assert.Equal("Light.Blue", theme.Name);            
        }

        [Fact]
        [DisplayTestMethodName]
        public async Task GetInverseAppThemeReturnsNullForMissingTheme()
        {
            await TestHost.SwitchToAppThread();

            var resource = new ResourceDictionary
                           {
                               {
                                   "Theme.Name", "Runtime"
                               },
                               {
                                   "Theme.DisplayName", "Runtime"
                               }
                           };
            var theme = new Theme(resource);

            var inverseTheme = ThemeManager.GetInverseTheme(theme);

            Assert.Null(inverseTheme);
        }

        [Fact]
        [DisplayTestMethodName]
        public async Task GetThemeIsCaseInsensitive()
        {
            await TestHost.SwitchToAppThread();

            var theme = ThemeManager.GetTheme("dark.blue");

            Assert.NotNull(theme);
            Assert.Equal("pack://application:,,,/MahApps.Metro;component/Styles/Themes/Dark.Blue.xaml".ToLower(), theme.Resources.Source.ToString().ToLower());
        }

        [Fact]
        [DisplayTestMethodName]
        public async Task GetThemeWithUriIsCaseInsensitive()
        {
            await TestHost.SwitchToAppThread();

            var dic = new ResourceDictionary
                      {
                          Source = new Uri("pack://application:,,,/MahApps.Metro;component/Styles/Themes/daRK.Blue.xaml")
                      };

            var theme = ThemeManager.GetTheme(dic);

            Assert.NotNull(theme);
            Assert.Equal("Dark.Blue", theme.Name);
        }

        [Fact]
        [DisplayTestMethodName]
        public async Task CreateDynamicAccentWithColor()
        {
            await TestHost.SwitchToAppThread();

            var applicationTheme = ThemeManager.DetectTheme(Application.Current);

            var ex = Record.Exception(() => ThemeHelper.CreateTheme("Dark", Colors.Red, "CustomAccentRed", changeImmediately: true));
            Assert.Null(ex);

            var detected = ThemeManager.DetectTheme(Application.Current);
            Assert.NotNull(detected);
            Assert.Equal("CustomAccentRed", detected.Name);

            ex = Record.Exception(() => ThemeHelper.CreateTheme("Light", Colors.Green, "CustomAccentGreen", changeImmediately: true));
            Assert.Null(ex);

            detected = ThemeManager.DetectTheme(Application.Current);
            Assert.NotNull(detected);
            Assert.Equal("CustomAccentGreen", detected.Name);

            ThemeManager.ChangeTheme(Application.Current, applicationTheme);
        }
    }
}