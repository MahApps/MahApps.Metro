using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using MahApps.Metro.Controls;
using MahApps.Metro.Tests.TestHelpers;
using Xunit;

namespace MahApps.Metro.Tests
{
    public class MetroWindowTest : AutomationTestBase
    {
        [Fact]
        [DisplayTestMethodName]
        public async Task MetroWindowSmokeTest()
        {
            await TestHost.SwitchToAppThread();

            await WindowHelpers.CreateInvisibleWindowAsync<MetroWindow>();
        }

        [Fact]
        [DisplayTestMethodName]
        public async Task WindowCommandsShouldHaveTheParentWindow()
        {
            await TestHost.SwitchToAppThread();

            var window = await WindowHelpers.CreateInvisibleWindowAsync<MetroWindow>();

            Assert.Equal(window, window.LeftWindowCommands.ParentWindow);
            Assert.Equal(window, window.RightWindowCommands.ParentWindow);
            Assert.Equal(window, window.WindowButtonCommands.ParentWindow);
        }

        [Fact]
        [DisplayTestMethodName]
        public async Task ShowsRightWindowCommandsOnTopByDefault()
        {
            await TestHost.SwitchToAppThread();

            var window = await WindowHelpers.CreateInvisibleWindowAsync<MetroWindow>();

            Assert.Equal(WindowCommandsOverlayBehavior.Always, window.RightWindowCommandsOverlayBehavior);
        }

        [Fact]
        [DisplayTestMethodName]
        public async Task IconShouldBeVisibleByDefault()
        {
            await TestHost.SwitchToAppThread();

            var window = await WindowHelpers.CreateInvisibleWindowAsync<MetroWindow>();
            var icon = window.FindChild<ContentControl>("PART_Icon");

            Assert.Equal(Visibility.Visible, icon.Visibility);
        }

        [Fact]
        [DisplayTestMethodName]
        public async Task IconShouldBeCollapsedWithShowIconOnTitleBarFalse()
        {
            await TestHost.SwitchToAppThread();

            var window = await WindowHelpers.CreateInvisibleWindowAsync<MetroWindow>(w => w.ShowIconOnTitleBar = false);
            var icon = window.FindChild<ContentControl>("PART_Icon");

            Assert.Equal(Visibility.Collapsed, icon.Visibility);
        }

        [Fact]
        [DisplayTestMethodName]
        public async Task IconShouldBeCollapsedWithShowTitleBarFalse()
        {
            await TestHost.SwitchToAppThread();

            var window = await WindowHelpers.CreateInvisibleWindowAsync<MetroWindow>(w => w.ShowTitleBar = false);
            var icon = window.FindChild<ContentControl>("PART_Icon");

            Assert.Equal(Visibility.Collapsed, icon.Visibility);
        }

        [Fact]
        [DisplayTestMethodName]
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
        [DisplayTestMethodName]
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
        [DisplayTestMethodName]
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
            var windowButtonCommands = window.WindowButtonCommands;
            Assert.NotNull(windowButtonCommands);

            var button = windowButtonCommands.Template.FindName(buttonName, windowButtonCommands) as Button;
            Assert.NotNull(button);

            return button;
        }

        [Fact]
        [DisplayTestMethodName]
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
        [DisplayTestMethodName]
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
        [DisplayTestMethodName]
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
        [DisplayTestMethodName]
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
        [DisplayTestMethodName]
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
        [DisplayTestMethodName]
        public async Task WindowSettingsUpgradeSettingsShouldBeTrueByDefault()
        {
            await TestHost.SwitchToAppThread();

            var window = await WindowHelpers.CreateInvisibleWindowAsync<MetroWindow>();
            window.SaveWindowPosition = true;

            var settings = window.GetWindowPlacementSettings();
            Assert.NotNull(settings);
            Assert.Equal(true, settings.UpgradeSettings);
        }

        [Fact]
        [DisplayTestMethodName]
        public async Task TestTitleCapsProperty()
        {
            await TestHost.SwitchToAppThread();

            var window = await WindowHelpers.CreateInvisibleWindowAsync<MetroWindow>(w => w.Title = "Test");
            var titleBar = window.FindChild<ContentControl>("PART_TitleBar");
            var titleBarContent = titleBar.FindChild<ContentPresenter>("PART_ContentPresenter");

            var be = BindingOperations.GetBindingExpression(titleBarContent, ContentControl.ContentProperty);
            Assert.NotNull(be);
            be.UpdateTarget();

            // default should be UPPER
            Assert.Equal(true, window.TitleCaps);
            Assert.Equal("TEST", titleBarContent.Content);

            window.TitleCaps = false;
            be.UpdateTarget();
            Assert.Equal("Test", titleBarContent.Content);
        }

        [Fact]
        [DisplayTestMethodName]
        public async Task TestTitleCharacterCasingProperty()
        {
            await TestHost.SwitchToAppThread();

            var window = await WindowHelpers.CreateInvisibleWindowAsync<MetroWindow>(w => w.Title = "Test");
            var titleBar = window.FindChild<ContentControl>("PART_TitleBar");
            var titleBarContent = titleBar.FindChild<ContentPresenter>("PART_ContentPresenter");

            var be = BindingOperations.GetBindingExpression(titleBarContent, ContentControl.ContentProperty);
            Assert.NotNull(be);
            be.UpdateTarget();

            // default should be UPPER
            Assert.Equal(CharacterCasing.Upper, window.TitleCharacterCasing);
            Assert.Equal("TEST", titleBarContent.Content);

            window.TitleCharacterCasing = CharacterCasing.Lower;
            be.UpdateTarget();
            Assert.Equal("test", titleBarContent.Content);

            window.TitleCharacterCasing = CharacterCasing.Normal;
            be.UpdateTarget();
            Assert.Equal("Test", titleBarContent.Content);
        }
    }
}
