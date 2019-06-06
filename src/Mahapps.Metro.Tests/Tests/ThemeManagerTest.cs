namespace MahApps.Metro.Tests
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Windows;    
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Media;
    using MahApps.Metro.Controls;
    using MahApps.Metro.Tests.TestHelpers;    
    using Xunit;
    using Theme = MahApps.Metro.Theme;

    public class ThemeManagerTest : AutomationTestBase
    {
        public override void Dispose()
        {
            Application.Current.Dispatcher.Invoke(ThemeManager.ClearThemes);

            base.Dispose();
        }

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
        public async Task NewThemeAddsNewBaseColorAndColorScheme()
        {
            await TestHost.SwitchToAppThread();

            var resource = new ResourceDictionary
                           {
                               {
                                   Theme.ThemeNameKey, "Runtime"
                               },
                               {
                                   Theme.ThemeDisplayNameKey, "Runtime"
                               },
                               {
                                    Theme.ThemeBaseColorSchemeKey, "Foo"
                               },
                               {
                                   Theme.ThemeColorSchemeKey, "Bar"
                               },
                           };

            Assert.True(ThemeManager.AddTheme(resource));
            Assert.Equal(new[] { ThemeManager.BaseColorLight, ThemeManager.BaseColorDark, "Foo" }, ThemeManager.BaseColors);
            Assert.Contains("Bar", ThemeManager.ColorSchemes.Select(x => x.Name));
        }

        [Fact]
        [DisplayTestMethodName]
        public async Task ChangingAppThemeChangesWindowTheme()
        {
            await TestHost.SwitchToAppThread();

            var window = await WindowHelpers.CreateInvisibleWindowAsync<MetroWindow>();
            {
                var expectedTheme = ThemeManager.GetTheme("Dark.Teal");
                ThemeManager.ChangeTheme(Application.Current, expectedTheme);

                Assert.Equal(expectedTheme, ThemeManager.DetectTheme(Application.Current));
                Assert.Equal(expectedTheme, ThemeManager.DetectTheme(window));
            }
        }

        [Fact]
        [DisplayTestMethodName]
        public async Task ChangeBaseColor()
        {
            await TestHost.SwitchToAppThread();

            {
                var currentTheme = ThemeManager.DetectTheme(Application.Current);

                Assert.NotNull(currentTheme);
                ThemeManager.ChangeThemeBaseColor(Application.Current, ThemeManager.GetInverseTheme(currentTheme).BaseColorScheme);

                Assert.NotEqual(currentTheme.BaseColorScheme, ThemeManager.DetectTheme(Application.Current).BaseColorScheme);
                Assert.Equal(currentTheme.ColorScheme, ThemeManager.DetectTheme(Application.Current).ColorScheme);
            }

            {
                var window = await WindowHelpers.CreateInvisibleWindowAsync<MetroWindow>();                
                {
                    var currentTheme = ThemeManager.DetectTheme(window);

                    Assert.NotNull(currentTheme);
                    ThemeManager.ChangeThemeBaseColor(window, ThemeManager.GetInverseTheme(currentTheme).BaseColorScheme);

                    Assert.NotEqual(currentTheme.BaseColorScheme, ThemeManager.DetectTheme(window).BaseColorScheme);
                    Assert.Equal(currentTheme.ColorScheme, ThemeManager.DetectTheme(window).ColorScheme);
                }
            }

            {
                var currentTheme = ThemeManager.DetectTheme(Application.Current);

                Assert.NotNull(currentTheme);

                var control = new Control();
                ThemeManager.ChangeThemeBaseColor(control.Resources, currentTheme, ThemeManager.GetInverseTheme(currentTheme).BaseColorScheme);

                Assert.NotEqual(currentTheme.BaseColorScheme, ThemeManager.DetectTheme(control.Resources).BaseColorScheme);
                Assert.Equal(currentTheme.ColorScheme, ThemeManager.DetectTheme(control.Resources).ColorScheme);
            }
        }

        [Fact]
        [DisplayTestMethodName]
        public async Task ChangeColorScheme()
        {
            await TestHost.SwitchToAppThread();

            {
                var currentTheme = ThemeManager.DetectTheme(Application.Current);

                Assert.NotNull(currentTheme);
                ThemeManager.ChangeThemeColorScheme(Application.Current, "Yellow");

                Assert.Equal(currentTheme.BaseColorScheme, ThemeManager.DetectTheme(Application.Current).BaseColorScheme);
                Assert.Equal("Yellow", ThemeManager.DetectTheme(Application.Current).ColorScheme);
            }

            {
                var window = await WindowHelpers.CreateInvisibleWindowAsync<MetroWindow>();
                {
                    var currentTheme = ThemeManager.DetectTheme(window);

                    Assert.NotNull(currentTheme);
                    ThemeManager.ChangeThemeColorScheme(window, "Green");

                    Assert.Equal(currentTheme.BaseColorScheme, ThemeManager.DetectTheme(window).BaseColorScheme);
                    Assert.Equal("Green", ThemeManager.DetectTheme(window).ColorScheme);
                }
            }

            {
                var currentTheme = ThemeManager.DetectTheme(Application.Current);

                Assert.NotNull(currentTheme);

                var control = new Control();
                ThemeManager.ChangeThemeColorScheme(control.Resources, currentTheme, "Red");

                Assert.Equal(currentTheme.BaseColorScheme, ThemeManager.DetectTheme(control.Resources).BaseColorScheme);
                Assert.Equal("Red", ThemeManager.DetectTheme(control.Resources).ColorScheme);
            }

            Assert.Equal("Yellow", ThemeManager.DetectTheme(Application.Current).ColorScheme);
        }

        [Fact]
        [DisplayTestMethodName]
        public async Task ChangeBaseColorAndColorScheme()
        {
            await TestHost.SwitchToAppThread();

            {
                var currentTheme = ThemeManager.DetectTheme(Application.Current);

                Assert.NotNull(currentTheme);
                ThemeManager.ChangeTheme(Application.Current, ThemeManager.BaseColorDark, "Yellow");

                Assert.Equal(ThemeManager.BaseColorDark, ThemeManager.DetectTheme(Application.Current).BaseColorScheme);
                Assert.Equal("Yellow", ThemeManager.DetectTheme(Application.Current).ColorScheme);
            }

            {
                var window = await WindowHelpers.CreateInvisibleWindowAsync<MetroWindow>();
                {
                    var currentTheme = ThemeManager.DetectTheme(window);

                    Assert.NotNull(currentTheme);
                    ThemeManager.ChangeTheme(window, ThemeManager.BaseColorLight, "Green");

                    Assert.Equal(ThemeManager.BaseColorLight, ThemeManager.DetectTheme(window).BaseColorScheme);
                    Assert.Equal("Green", ThemeManager.DetectTheme(window).ColorScheme);
                }
            }

            {
                var currentTheme = ThemeManager.DetectTheme(Application.Current);

                Assert.NotNull(currentTheme);

                var control = new Control();
                ThemeManager.ChangeTheme(control.Resources, currentTheme, ThemeManager.BaseColorDark, "Red");

                Assert.Equal(ThemeManager.BaseColorDark, ThemeManager.DetectTheme(control.Resources).BaseColorScheme);
                Assert.Equal("Red", ThemeManager.DetectTheme(control.Resources).ColorScheme);
            }

            Assert.Equal("Yellow", ThemeManager.DetectTheme(Application.Current).ColorScheme);
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
        public async Task GetThemes()
        {
            await TestHost.SwitchToAppThread();

            var expectedThemes = new[]
                                 {
                                     "Amber (Dark)",
                                     "Amber (Light)",
                                     "Blue (Dark)",
                                     "Blue (Light)",
                                     "Brown (Dark)",
                                     "Brown (Light)",
                                     "Cobalt (Dark)",
                                     "Cobalt (Light)",
                                     "Crimson (Dark)",
                                     "Crimson (Light)",
                                     "Cyan (Dark)",
                                     "Cyan (Light)",
                                     "Emerald (Dark)",
                                     "Emerald (Light)",
                                     "Green (Dark)",
                                     "Green (Light)",
                                     "Indigo (Dark)",
                                     "Indigo (Light)",
                                     "Lime (Dark)",
                                     "Lime (Light)",
                                     "Magenta (Dark)",
                                     "Magenta (Light)",
                                     "Mauve (Dark)",
                                     "Mauve (Light)",
                                     "Olive (Dark)",
                                     "Olive (Light)",
                                     "Orange (Dark)",
                                     "Orange (Light)",
                                     "Pink (Dark)",
                                     "Pink (Light)",
                                     "Purple (Dark)",
                                     "Purple (Light)",
                                     "Red (Dark)",
                                     "Red (Light)",
                                     "Sienna (Dark)",
                                     "Sienna (Light)",
                                     "Steel (Dark)",
                                     "Steel (Light)",
                                     "Taupe (Dark)",
                                     "Taupe (Light)",
                                     "Teal (Dark)",
                                     "Teal (Light)",
                                     "Violet (Dark)",
                                     "Violet (Light)",
                                     "Yellow (Dark)",
                                     "Yellow (Light)"
                                 };
            Assert.Equal(expectedThemes, CollectionViewSource.GetDefaultView(ThemeManager.Themes).Cast<Theme>().Select(x => x.DisplayName).ToList());
        }

        [Fact]
        [DisplayTestMethodName]
        public async Task GetBaseColors()
        {
            await TestHost.SwitchToAppThread();

            ThemeManager.ClearThemes();

            Assert.NotEmpty(ThemeManager.BaseColors);
        }

        [Fact]
        [DisplayTestMethodName]
        public async Task GetColorSchemes()
        {
            await TestHost.SwitchToAppThread();

            ThemeManager.ClearThemes();

            Assert.NotEmpty(ThemeManager.ColorSchemes);
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

        [Fact]
        [DisplayTestMethodName]
        public async Task CreateDynamicAccentWithColorAndChangeBaseColorScheme()
        {
            await TestHost.SwitchToAppThread();

            var applicationTheme = ThemeManager.DetectTheme(Application.Current);

            var ex = Record.Exception(() => ThemeHelper.CreateTheme("Dark", Colors.Red));
            Assert.Null(ex);
            ex = Record.Exception(() => ThemeHelper.CreateTheme("Light", Colors.Red, changeImmediately: true));
            Assert.Null(ex);

            var detected = ThemeManager.DetectTheme(Application.Current);
            Assert.NotNull(detected);
            Assert.Equal(Colors.Red.ToString().Replace("#", string.Empty), detected.ColorScheme);

            var newTheme = ThemeManager.ChangeThemeBaseColor(Application.Current, "Dark");
            Assert.NotNull(newTheme);

            newTheme = ThemeManager.ChangeThemeBaseColor(Application.Current, "Light");
            Assert.NotNull(newTheme);

            ThemeManager.ChangeTheme(Application.Current, applicationTheme);
        }
    }
}