namespace MahApps.Metro.Tests
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Windows;    
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Media;
    using ControlzEx.Theming;
    using MahApps.Metro.Controls;
    using MahApps.Metro.Tests.TestHelpers;    
    using Xunit;

    public class ThemeManagerTest : AutomationTestBase
    {
        public override void Dispose()
        {
            Application.Current.Dispatcher.Invoke(ThemeManager.Current.ClearThemes);

            base.Dispose();
        }

        [Fact]
        [DisplayTestMethodName]
        public async Task ChangeThemeForAppShouldThrowArgumentNullException()
        {
            await TestHost.SwitchToAppThread();

            Assert.Throws<ArgumentNullException>(() => ThemeManager.Current.ChangeTheme((Application)null, ThemeManager.Current.GetTheme("Light.Red")));
            Assert.Throws<ArgumentNullException>(() => ThemeManager.Current.ChangeTheme(Application.Current, ThemeManager.Current.GetTheme("UnknownTheme")));
        }

        [Fact]
        [DisplayTestMethodName]
        public async Task ChangeThemeForWindowShouldThrowArgumentNullException()
        {
            await TestHost.SwitchToAppThread();

            await WindowHelpers.CreateInvisibleWindowAsync<MetroWindow>();

            Assert.Throws<ArgumentNullException>(() => ThemeManager.Current.ChangeTheme((Window)null, ThemeManager.Current.GetTheme("Light.Red")));
            Assert.Throws<ArgumentNullException>(() => ThemeManager.Current.ChangeTheme(Application.Current.MainWindow, ThemeManager.Current.GetTheme("UnknownTheme")));
        }

        [Fact]
        [DisplayTestMethodName]
        public async Task CanAddThemeBeforeGetterIsCalled()
        {
            await TestHost.SwitchToAppThread();

            ThemeManager.Current.AddTheme(new Theme(new LibraryTheme(new Uri("pack://application:,,,/MahApps.Metro;component/Styles/Themes/Dark.Cobalt.xaml"), null, true)));

            var resource = new ResourceDictionary
                           {
                               {
                                   Theme.ThemeNameKey, "Runtime"
                               },
                               {
                                   Theme.ThemeDisplayNameKey, "Runtime"
                               },
                               {
                                   Theme.ThemePrimaryAccentColorKey, Colors.Red
                               }
                           };
            ThemeManager.Current.AddTheme(new Theme(new LibraryTheme(resource, null, true)));
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
                               {
                                   Theme.ThemePrimaryAccentColorKey, Colors.Red
                               },
                           };

            ThemeManager.Current.AddTheme(new Theme(new LibraryTheme(resource, null, true)));
            Assert.Equal(new[] { ThemeManager.BaseColorLight, ThemeManager.BaseColorDark, "Foo" }, ThemeManager.Current.BaseColors);
            Assert.Contains("Bar", ThemeManager.Current.ColorSchemes.Select(x => x));
        }

        [Fact]
        [DisplayTestMethodName]
        public async Task ChangingAppThemeChangesWindowTheme()
        {
            await TestHost.SwitchToAppThread();

            var window = await WindowHelpers.CreateInvisibleWindowAsync<MetroWindow>();
            {
                var expectedTheme = ThemeManager.Current.GetTheme("Dark.Teal");
                ThemeManager.Current.ChangeTheme(Application.Current, expectedTheme);

                Assert.Equal(expectedTheme, ThemeManager.Current.DetectTheme(Application.Current));
                Assert.Equal(expectedTheme, ThemeManager.Current.DetectTheme(window));
            }
        }

        [Fact]
        [DisplayTestMethodName]
        public async Task ChangeBaseColor()
        {
            await TestHost.SwitchToAppThread();

            {
                var currentTheme = ThemeManager.Current.DetectTheme(Application.Current);

                Assert.NotNull(currentTheme);
                ThemeManager.Current.ChangeThemeBaseColor(Application.Current, ThemeManager.Current.GetInverseTheme(currentTheme).BaseColorScheme);

                Assert.NotEqual(currentTheme.BaseColorScheme, ThemeManager.Current.DetectTheme(Application.Current).BaseColorScheme);
                Assert.Equal(currentTheme.ColorScheme, ThemeManager.Current.DetectTheme(Application.Current).ColorScheme);
            }

            {
                var window = await WindowHelpers.CreateInvisibleWindowAsync<MetroWindow>();                
                {
                    var currentTheme = ThemeManager.Current.DetectTheme(window);

                    Assert.NotNull(currentTheme);
                    ThemeManager.Current.ChangeThemeBaseColor(window, ThemeManager.Current.GetInverseTheme(currentTheme).BaseColorScheme);

                    Assert.NotEqual(currentTheme.BaseColorScheme, ThemeManager.Current.DetectTheme(window).BaseColorScheme);
                    Assert.Equal(currentTheme.ColorScheme, ThemeManager.Current.DetectTheme(window).ColorScheme);
                }
            }

            {
                var currentTheme = ThemeManager.Current.DetectTheme(Application.Current);

                Assert.NotNull(currentTheme);

                var control = new Control();
                ThemeManager.Current.ChangeThemeBaseColor(control, control.Resources, currentTheme, ThemeManager.Current.GetInverseTheme(currentTheme).BaseColorScheme);

                Assert.NotEqual(currentTheme.BaseColorScheme, ThemeManager.Current.DetectTheme(control.Resources).BaseColorScheme);
                Assert.Equal(currentTheme.ColorScheme, ThemeManager.Current.DetectTheme(control.Resources).ColorScheme);
            }
        }

        [Fact]
        [DisplayTestMethodName]
        public async Task ChangeColorScheme()
        {
            await TestHost.SwitchToAppThread();

            {
                var currentTheme = ThemeManager.Current.DetectTheme(Application.Current);

                Assert.NotNull(currentTheme);
                ThemeManager.Current.ChangeThemeColorScheme(Application.Current, "Yellow");

                Assert.Equal(currentTheme.BaseColorScheme, ThemeManager.Current.DetectTheme(Application.Current).BaseColorScheme);
                Assert.Equal("Yellow", ThemeManager.Current.DetectTheme(Application.Current).ColorScheme);
            }

            {
                var window = await WindowHelpers.CreateInvisibleWindowAsync<MetroWindow>();
                {
                    var currentTheme = ThemeManager.Current.DetectTheme(window);

                    Assert.NotNull(currentTheme);
                    ThemeManager.Current.ChangeThemeColorScheme(window, "Green");

                    Assert.Equal(currentTheme.BaseColorScheme, ThemeManager.Current.DetectTheme(window).BaseColorScheme);
                    Assert.Equal("Green", ThemeManager.Current.DetectTheme(window).ColorScheme);
                }
            }

            {
                var currentTheme = ThemeManager.Current.DetectTheme(Application.Current);

                Assert.NotNull(currentTheme);

                var control = new Control();
                ThemeManager.Current.ChangeThemeColorScheme(control, control.Resources, currentTheme, "Red");

                Assert.Equal(currentTheme.BaseColorScheme, ThemeManager.Current.DetectTheme(control.Resources).BaseColorScheme);
                Assert.Equal("Red", ThemeManager.Current.DetectTheme(control.Resources).ColorScheme);
            }

            Assert.Equal("Yellow", ThemeManager.Current.DetectTheme(Application.Current).ColorScheme);
        }

        [Fact]
        [DisplayTestMethodName]
        public async Task ChangeBaseColorAndColorScheme()
        {
            await TestHost.SwitchToAppThread();

            {
                var currentTheme = ThemeManager.Current.DetectTheme(Application.Current);

                Assert.NotNull(currentTheme);
                ThemeManager.Current.ChangeTheme(Application.Current, ThemeManager.BaseColorDark, "Yellow");

                Assert.Equal(ThemeManager.BaseColorDark, ThemeManager.Current.DetectTheme(Application.Current).BaseColorScheme);
                Assert.Equal("Yellow", ThemeManager.Current.DetectTheme(Application.Current).ColorScheme);
            }

            {
                var window = await WindowHelpers.CreateInvisibleWindowAsync<MetroWindow>();
                {
                    var currentTheme = ThemeManager.Current.DetectTheme(window);

                    Assert.NotNull(currentTheme);
                    ThemeManager.Current.ChangeTheme(window, ThemeManager.BaseColorLight, "Green");

                    Assert.Equal(ThemeManager.BaseColorLight, ThemeManager.Current.DetectTheme(window).BaseColorScheme);
                    Assert.Equal("Green", ThemeManager.Current.DetectTheme(window).ColorScheme);
                }
            }

            {
                var currentTheme = ThemeManager.Current.DetectTheme(Application.Current);

                Assert.NotNull(currentTheme);

                var control = new Control();
                ThemeManager.Current.ChangeTheme(control, control.Resources, currentTheme, ThemeManager.BaseColorDark, "Red");

                Assert.Equal(ThemeManager.BaseColorDark, ThemeManager.Current.DetectTheme(control.Resources).BaseColorScheme);
                Assert.Equal("Red", ThemeManager.Current.DetectTheme(control.Resources).ColorScheme);
            }

            Assert.Equal("Yellow", ThemeManager.Current.DetectTheme(Application.Current).ColorScheme);
        }

        [Fact]
        [DisplayTestMethodName]
        public async Task GetInverseAppThemeReturnsDarkTheme()
        {
            await TestHost.SwitchToAppThread();

            var theme = ThemeManager.Current.GetInverseTheme(ThemeManager.Current.GetTheme("Light.Blue"));

            Assert.Equal("Dark.Blue", theme.Name);
        }

        [Fact]
        [DisplayTestMethodName]
        public async Task GetInverseThemeReturnsLightTheme()
        {
            await TestHost.SwitchToAppThread();

            var theme = ThemeManager.Current.GetInverseTheme(ThemeManager.Current.GetTheme("Dark.Blue"));

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
                                   Theme.ThemeNameKey, "Runtime"
                               },
                               {
                                   Theme.ThemeDisplayNameKey, "Runtime"
                               },
                               {
                                   Theme.ThemePrimaryAccentColorKey, Colors.Red
                               },
                           };
            var theme = ThemeManager.Current.AddTheme(new Theme(new LibraryTheme(resource, null, true)));

            var inverseTheme = ThemeManager.Current.GetInverseTheme(theme);

            Assert.Null(inverseTheme);
        }

        [Fact]
        [DisplayTestMethodName]
        public async Task GetThemeIsCaseInsensitive()
        {
            await TestHost.SwitchToAppThread();

            var theme = ThemeManager.Current.GetTheme("dark.blue");

            Assert.NotNull(theme);
            Assert.Equal("pack://application:,,,/MahApps.Metro;component/Styles/Themes/Dark.Blue.xaml".ToLower(), theme.LibraryThemes.First().Resources.MergedDictionaries.First().Source.ToString().ToLower());
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

            var theme = ThemeManager.Current.GetTheme(dic);

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
            Assert.Equal(expectedThemes, CollectionViewSource.GetDefaultView(ThemeManager.Current.Themes).Cast<Theme>().Select(x => x.DisplayName).ToList());
        }

        [Fact]
        [DisplayTestMethodName]
        public async Task GetBaseColors()
        {
            await TestHost.SwitchToAppThread();

            ThemeManager.Current.ClearThemes();

            Assert.NotEmpty(ThemeManager.Current.BaseColors);
        }

        [Fact]
        [DisplayTestMethodName]
        public async Task GetColorSchemes()
        {
            await TestHost.SwitchToAppThread();

            ThemeManager.Current.ClearThemes();

            Assert.NotEmpty(ThemeManager.Current.ColorSchemes);
        }

        [Fact]
        [DisplayTestMethodName]
        public async Task CreateDynamicAccentWithColor()
        {
            await TestHost.SwitchToAppThread();

            var applicationTheme = ThemeManager.Current.DetectTheme(Application.Current);

            var ex = Record.Exception(() => ThemeManager.Current.ChangeTheme(Application.Current, RuntimeThemeGenerator.Current.GenerateRuntimeTheme("Dark", Colors.Red)));
            Assert.Null(ex);

            var detected = ThemeManager.Current.DetectTheme(Application.Current);
            Assert.NotNull(detected);
            Assert.Equal("Dark.Runtime_#FFFF0000", detected.Name);

            ex = Record.Exception(() => ThemeManager.Current.ChangeTheme(Application.Current, RuntimeThemeGenerator.Current.GenerateRuntimeTheme("Light", Colors.Green)));
            Assert.Null(ex);

            detected = ThemeManager.Current.DetectTheme(Application.Current);
            Assert.NotNull(detected);
            Assert.Equal("Light.Runtime_#FF008000", detected.Name);

            ThemeManager.Current.ChangeTheme(Application.Current, applicationTheme);
        }

        [Fact]
        [DisplayTestMethodName]
        public async Task CreateDynamicAccentWithColorAndChangeBaseColorScheme()
        {
            await TestHost.SwitchToAppThread();

            var applicationTheme = ThemeManager.Current.DetectTheme(Application.Current);

            var ex = Record.Exception(() => ThemeManager.Current.AddTheme(RuntimeThemeGenerator.Current.GenerateRuntimeTheme("Dark", Colors.Red)));
            Assert.Null(ex);
            ex = Record.Exception(() => ThemeManager.Current.AddTheme(ThemeManager.Current.ChangeTheme(Application.Current, RuntimeThemeGenerator.Current.GenerateRuntimeTheme("Light", Colors.Red))));
            Assert.Null(ex);

            var detected = ThemeManager.Current.DetectTheme(Application.Current);
            Assert.NotNull(detected);
            Assert.Equal(Colors.Red.ToString(), detected.ColorScheme);

            var newTheme = ThemeManager.Current.ChangeThemeBaseColor(Application.Current, "Dark");
            Assert.NotNull(newTheme);

            newTheme = ThemeManager.Current.ChangeThemeBaseColor(Application.Current, "Light");
            Assert.NotNull(newTheme);

            ThemeManager.Current.ChangeTheme(Application.Current, applicationTheme);
        }
    }
}