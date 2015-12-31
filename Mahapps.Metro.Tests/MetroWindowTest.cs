using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using MahApps.Metro.Controls;
using MahApps.Metro.Tests.TestHelpers;
using Xunit;

namespace MahApps.Metro.Tests
{
    public class MetroWindowTest : AutomationTestBase
    {
        [Fact]
        public async Task MetroWindowSmokeTest()
        {
            await TestHost.SwitchToAppThread();

            await WindowHelpers.CreateInvisibleWindowAsync<MetroWindow>();
        }

        [Fact]
        public async Task WindowCommandsShouldHaveTheParentWindow()
        {
            await TestHost.SwitchToAppThread();

            var window = await WindowHelpers.CreateInvisibleWindowAsync<MetroWindow>();

            Assert.Equal(window, window.LeftWindowCommands.ParentWindow);
            Assert.Equal(window, window.RightWindowCommands.ParentWindow);
            Assert.Equal(window, window.FindChild<WindowButtonCommands>("PART_WindowButtonCommands").ParentWindow);
        }

        [Fact]
        public async Task ShowsRightWindowCommandsOnTopByDefault()
        {
            await TestHost.SwitchToAppThread();

            var window = new MetroWindow();

            Assert.Equal(WindowCommandsOverlayBehavior.Always, window.RightWindowCommandsOverlayBehavior);
        }

        [Fact]
        public async Task IconShouldBeVisibleByDefault()
        {
            await TestHost.SwitchToAppThread();

            var window = await WindowHelpers.CreateInvisibleWindowAsync<MetroWindow>();
            var icon = window.FindChild<ContentControl>("PART_Icon");

            Assert.Equal(Visibility.Visible, icon.Visibility);
        }

        [Fact]
        public async Task IconShouldBeCollapsedWithShowIconOnTitleBarFalse()
        {
            await TestHost.SwitchToAppThread();

            var window = await WindowHelpers.CreateInvisibleWindowAsync<MetroWindow>(w => w.ShowIconOnTitleBar = false);
            var icon = window.FindChild<ContentControl>("PART_Icon");

            Assert.Equal(Visibility.Collapsed, icon.Visibility);
        }

        [Fact]
        public async Task IconShouldBeCollapsedWithShowTitleBarFalse()
        {
            await TestHost.SwitchToAppThread();

            var window = await WindowHelpers.CreateInvisibleWindowAsync<MetroWindow>(w => w.ShowTitleBar = false);
            var icon = window.FindChild<ContentControl>("PART_Icon");

            Assert.Equal(Visibility.Collapsed, icon.Visibility);
        }

        [Fact]
        public async Task IconShouldBeVisibleWithShowTitleBarFalseAndOverlayBehaviorHiddenTitleBar()
        {
            await TestHost.SwitchToAppThread();

            var window = await WindowHelpers.CreateInvisibleWindowAsync<MetroWindow>(w => {
                                                                                         w.IconOverlayBehavior = WindowCommandsOverlayBehavior.HiddenTitleBar;
                                                                                         w.ShowTitleBar = false;
                                                                                     });
            var icon = window.FindChild<ContentControl>("PART_Icon");

            Assert.Equal(Visibility.Visible, icon.Visibility);
        }

        [Fact]
        public async Task IconShouldBeHiddenWithChangedShowIconOnTitleBar()
        {
            await TestHost.SwitchToAppThread();

            var window = await WindowHelpers.CreateInvisibleWindowAsync<MetroWindow>();
            var icon = window.FindChild<ContentControl>("PART_Icon");

            Assert.Equal(Visibility.Visible, icon.Visibility);

            window.ShowIconOnTitleBar = false;

            Assert.Equal(Visibility.Collapsed, icon.Visibility);
        }

        [Fact]
        public async Task IconCanOverlayHiddenTitlebar()
        {
            await TestHost.SwitchToAppThread();

            var window = await WindowHelpers.CreateInvisibleWindowAsync<MetroWindow>();
            window.IconOverlayBehavior = WindowCommandsOverlayBehavior.HiddenTitleBar;
            window.ShowTitleBar = false;
            var icon = window.FindChild<ContentControl>("PART_Icon");

            Assert.Equal(Visibility.Visible, icon.Visibility);
        }


        private Button GetButton(MetroWindow window, string buttonName)
        {
            var windowButtonCommands = window.FindChild<WindowButtonCommands>("PART_WindowButtonCommands");
            Assert.NotNull(windowButtonCommands);

            var button = windowButtonCommands.Template.FindName(buttonName, windowButtonCommands) as Button;
            Assert.NotNull(button);

            return button;
        }

        [Fact]
        public async Task MinMaxCloseButtonsShouldBeVisibleByDefault()
        {
            await TestHost.SwitchToAppThread();

            var window = await WindowHelpers.CreateInvisibleWindowAsync<MetroWindow>();

            var minButton = GetButton(window, "PART_Min");
            var maxButton = GetButton(window, "PART_Max");
            var closeButton = GetButton(window, "PART_Close");

            // min/max/close should be visible
            Assert.True(minButton.IsVisible);
            Assert.True(maxButton.IsVisible);
            Assert.True(closeButton.IsVisible);
            Assert.Equal(ResizeMode.CanResize, window.ResizeMode);
        }

        [Fact]
        public async Task MinMaxButtonsShouldBeHiddenWithNoResizeMode()
        {
            await TestHost.SwitchToAppThread();

            var window = await WindowHelpers.CreateInvisibleWindowAsync<MetroWindow>();

            var minButton = GetButton(window, "PART_Min");
            var maxButton = GetButton(window, "PART_Max");

            // min/max should be visible
            Assert.True(minButton.IsVisible);
            Assert.True(maxButton.IsVisible);
            Assert.Equal(ResizeMode.CanResize, window.ResizeMode);

            window.ResizeMode = ResizeMode.NoResize;

            // min/max should be hidden
            Assert.False(minButton.IsVisible);
            Assert.False(maxButton.IsVisible);
            Assert.Equal(ResizeMode.NoResize, window.ResizeMode);
        }

        [Fact]
        public async Task MaxButtonShouldBeHiddenWithCanMinimizeResizeMode()
        {
            await TestHost.SwitchToAppThread();

            var window = await WindowHelpers.CreateInvisibleWindowAsync<MetroWindow>();

            var minButton = GetButton(window, "PART_Min");
            var maxButton = GetButton(window, "PART_Max");

            // min/max should be visible
            Assert.True(minButton.IsVisible);
            Assert.True(maxButton.IsVisible);
            Assert.Equal(ResizeMode.CanResize, window.ResizeMode);

            window.ResizeMode = ResizeMode.CanMinimize;

            // min should be visible, max hidden
            Assert.True(minButton.IsVisible);
            Assert.False(maxButton.IsVisible);
            Assert.Equal(ResizeMode.CanMinimize, window.ResizeMode);
        }

        [Fact]
        public async Task MinMaxButtonsShouldBeToggled()
        {
            await TestHost.SwitchToAppThread();

            var window = await WindowHelpers.CreateInvisibleWindowAsync<MetroWindow>();

            var minButton = GetButton(window, "PART_Min");
            var maxButton = GetButton(window, "PART_Max");

            // min/max should be visible
            Assert.True(minButton.IsVisible);
            Assert.True(maxButton.IsVisible);
            Assert.Equal(ResizeMode.CanResize, window.ResizeMode);

            window.ResizeMode = ResizeMode.CanMinimize;

            // min should be visible, max hidden
            Assert.True(minButton.IsVisible);
            Assert.False(maxButton.IsVisible);
            Assert.Equal(ResizeMode.CanMinimize, window.ResizeMode);

            window.ShowMinButton = false;
            // min should be hidden
            Assert.False(minButton.IsVisible);

            window.ResizeMode = ResizeMode.NoResize;

            // min/max should be hidden
            Assert.False(minButton.IsVisible);
            Assert.False(maxButton.IsVisible);
            Assert.Equal(ResizeMode.NoResize, window.ResizeMode);

            window.ShowMaxRestoreButton = false;
            // max should be hidden
            Assert.False(maxButton.IsVisible);

            window.ResizeMode = ResizeMode.CanResizeWithGrip;

            // min/max should be hidden
            Assert.False(minButton.IsVisible);
            Assert.False(maxButton.IsVisible);
            Assert.Equal(ResizeMode.CanResizeWithGrip, window.ResizeMode);

            window.ShowMinButton = true;
            window.ShowMaxRestoreButton = true;
            // min/max should be visible
            Assert.True(minButton.IsVisible);
            Assert.True(maxButton.IsVisible);

            window.ResizeMode = ResizeMode.NoResize;

            // min/max should be hidden
            Assert.False(minButton.IsVisible);
            Assert.False(maxButton.IsVisible);
            Assert.Equal(ResizeMode.NoResize, window.ResizeMode);
        }

        /// <summary>
        /// #1362: ShowMinButton="False" and ShowMaxRestoreButton="False" not working
        /// </summary>
        [Fact]
        public async Task MinMaxCloseButtonsShouldBeHidden()
        {
            await TestHost.SwitchToAppThread();

            var window = await WindowHelpers.CreateInvisibleWindowAsync<HiddenMinMaxCloseButtonsWindow>();

            var minButton = GetButton(window, "PART_Min");
            var maxButton = GetButton(window, "PART_Max");
            var closeButton = GetButton(window, "PART_Close");

            // min/max/close should be hidden
            Assert.False(minButton.IsVisible);
            Assert.False(maxButton.IsVisible);
            Assert.False(closeButton.IsVisible);
            Assert.Equal(ResizeMode.CanResize, window.ResizeMode);
        }

        [Fact]
        public async Task WindowSettingsUpgradeSettingsShouldBeTrueByDefault()
        {
            await TestHost.SwitchToAppThread();

            var window = await WindowHelpers.CreateInvisibleWindowAsync<MetroWindow>();
            window.SaveWindowPosition = true;

            var settings = window.GetWindowPlacementSettings();
            Assert.NotNull(settings);
            Assert.Equal(true, settings.UpgradeSettings);
        }
    }
}
