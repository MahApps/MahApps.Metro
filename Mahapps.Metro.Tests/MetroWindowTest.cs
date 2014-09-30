﻿using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using MahApps.Metro.Controls;
using Mahapps.Metro.Tests.TestHelpers;
using Xunit;

namespace Mahapps.Metro.Tests
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
            var icon = window.GetPart<ContentControl>("PART_Icon");

            Assert.Equal(Visibility.Visible, icon.Visibility);
        }

        [Fact]
        public async Task IconCanOverlayHiddenTitlebar()
        {
            await TestHost.SwitchToAppThread();

            var window = await WindowHelpers.CreateInvisibleWindowAsync<MetroWindow>();
            window.IconOverlayBehavior = WindowCommandsOverlayBehavior.HiddenTitleBar;
            window.ShowTitleBar = false;
            var icon = window.GetPart<ContentControl>("PART_Icon");

            Assert.Equal(Visibility.Visible, icon.Visibility);
        }


        private Button GetButton(MetroWindow window, string buttonName)
        {
            var windowButtonCommands = window.GetPart<WindowButtonCommands>("PART_WindowButtonCommands");
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
    }
}
