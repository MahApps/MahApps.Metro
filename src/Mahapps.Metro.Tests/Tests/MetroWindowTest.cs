// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using MahApps.Metro.Controls;
using MahApps.Metro.Tests.TestHelpers;
using MahApps.Metro.Tests.Views;
using NUnit.Framework;

namespace MahApps.Metro.Tests.Tests
{
    [TestFixture]
    public class MetroWindowTest
    {
        [Test]
        public async Task MetroWindowSmokeTest()
        {
            var window = await WindowHelpers.CreateInvisibleWindowAsync<TestWindow>();
            window.Close();
        }

        [Test]
        public async Task WindowCommandsShouldHaveTheParentWindow()
        {
            var window = await WindowHelpers.CreateInvisibleWindowAsync<TestWindow>();

            Assert.That(window.LeftWindowCommands, Is.Not.Null);
            Assert.That(window.RightWindowCommands, Is.Not.Null);
            Assert.That(window.WindowButtonCommands, Is.Not.Null);

            Assert.That(window.LeftWindowCommands.ParentWindow, Is.EqualTo(window));
            Assert.That(window.RightWindowCommands.ParentWindow, Is.EqualTo(window));
            Assert.That(window.WindowButtonCommands.ParentWindow, Is.EqualTo(window));

            window.Close();
        }

        [Test]
        public async Task CheckDefaultOverlayBehaviors()
        {
            var window = await WindowHelpers.CreateInvisibleWindowAsync<TestWindow>();

            Assert.That(window.IconOverlayBehavior, Is.EqualTo(OverlayBehavior.Never));
            Assert.That(window.LeftWindowCommandsOverlayBehavior, Is.EqualTo(WindowCommandsOverlayBehavior.Never));
            Assert.That(window.RightWindowCommandsOverlayBehavior, Is.EqualTo(WindowCommandsOverlayBehavior.Never));
            Assert.That(window.WindowButtonCommandsOverlayBehavior, Is.EqualTo(OverlayBehavior.Always));

            window.Close();
        }

        [Test]
        public async Task IconShouldBeCollapsedByDefault()
        {
            var window = await WindowHelpers.CreateInvisibleWindowAsync<TestWindow>();

            var icon = window.FindChild<ContentControl>("PART_Icon");
            Assert.That(icon, Is.Not.Null);

            Assert.That(icon.Visibility, Is.EqualTo(Visibility.Collapsed));

            window.Close();
        }

        [Test]
        public async Task IconShouldBeCollapsedWithShowIconOnTitleBarFalse()
        {
            var window = await WindowHelpers.CreateInvisibleWindowAsync<TestWindow>(w => w.ShowIconOnTitleBar = false);

            var icon = window.FindChild<ContentControl>("PART_Icon");
            Assert.That(icon, Is.Not.Null);

            Assert.That(icon.Visibility, Is.EqualTo(Visibility.Collapsed));

            window.Close();
        }

        [Test]
        public async Task IconShouldBeCollapsedWithShowTitleBarFalse()
        {
            var window = await WindowHelpers.CreateInvisibleWindowAsync<TestWindow>(w => w.ShowTitleBar = false);

            var icon = window.FindChild<ContentControl>("PART_Icon");
            Assert.That(icon, Is.Not.Null);

            Assert.That(icon.Visibility, Is.EqualTo(Visibility.Collapsed));

            window.Close();
        }

        [Test]
        public async Task IconShouldBeVisibleWithShowTitleBarFalseAndOverlayBehaviorHiddenTitleBar()
        {
            var window = await WindowHelpers.CreateInvisibleWindowAsync<TestWindow>(w =>
                {
                    w.Icon = new BitmapImage(new Uri("pack://application:,,,/MahApps.Metro.Tests;component/mahapps.metro.logo2.ico", UriKind.RelativeOrAbsolute));
                    w.IconOverlayBehavior = OverlayBehavior.HiddenTitleBar;
                    w.ShowTitleBar = false;
                });

            var icon = window.FindChild<ContentControl>("PART_Icon");
            Assert.That(icon, Is.Not.Null);

            Assert.That(icon.Visibility, Is.EqualTo(Visibility.Visible));

            window.Close();
        }

        [Test]
        public async Task IconShouldBeHiddenWithChangedShowIconOnTitleBar()
        {
            var window = await WindowHelpers.CreateInvisibleWindowAsync<TestWindow>(w => { w.Icon = new BitmapImage(new Uri("pack://application:,,,/MahApps.Metro.Tests;component/mahapps.metro.logo2.ico", UriKind.RelativeOrAbsolute)); });

            var icon = window.FindChild<ContentControl>("PART_Icon");
            Assert.That(icon, Is.Not.Null);

            Assert.That(icon.Visibility, Is.EqualTo(Visibility.Visible));

            window.ShowIconOnTitleBar = false;

            Assert.That(icon.Visibility, Is.EqualTo(Visibility.Collapsed));

            window.Close();
        }

        private Button GetButton(MetroWindow window, string buttonName)
        {
            var windowButtonCommands = window.WindowButtonCommands;
            Assert.That(windowButtonCommands, Is.Not.Null);

            var button = windowButtonCommands.Template.FindName(buttonName, windowButtonCommands) as Button;
            Assert.That(button, Is.Not.Null);

            return button;
        }

        [Test]
        public async Task MinMaxCloseButtonsShouldBeVisibleByDefault()
        {
            var window = await WindowHelpers.CreateInvisibleWindowAsync<TestWindow>();

            var minButton = this.GetButton(window, "PART_Min");
            var maxButton = this.GetButton(window, "PART_Max");
            var closeButton = this.GetButton(window, "PART_Close");

            // min/max/close should be visible
            Assert.That(minButton.IsVisible, Is.True);
            Assert.That(maxButton.IsVisible, Is.True);
            Assert.That(closeButton.IsVisible, Is.True);
            Assert.That(window.ResizeMode, Is.EqualTo(ResizeMode.CanResize));

            window.Close();
        }

        [Test]
        public async Task MinMaxButtonsShouldBeHiddenWithNoResizeMode()
        {
            var window = await WindowHelpers.CreateInvisibleWindowAsync<TestWindow>();

            var minButton = this.GetButton(window, "PART_Min");
            var maxButton = this.GetButton(window, "PART_Max");

            // min/max should be visible
            Assert.That(minButton.IsVisible, Is.True);
            Assert.That(maxButton.IsVisible, Is.True);
            Assert.That(window.ResizeMode, Is.EqualTo(ResizeMode.CanResize));

            window.ResizeMode = ResizeMode.NoResize;

            // min/max should be hidden
            Assert.That(minButton.IsVisible, Is.False);
            Assert.That(maxButton.IsVisible, Is.False);
            Assert.That(window.ResizeMode, Is.EqualTo(ResizeMode.NoResize));

            window.Close();
        }

        [Test]
        public async Task MaxButtonShouldBeHiddenWithCanMinimizeResizeMode()
        {
            var window = await WindowHelpers.CreateInvisibleWindowAsync<TestWindow>();

            var minButton = this.GetButton(window, "PART_Min");
            var maxButton = this.GetButton(window, "PART_Max");

            // min/max should be visible
            Assert.That(minButton.IsVisible, Is.True);
            Assert.That(maxButton.IsVisible, Is.True);
            Assert.That(window.ResizeMode, Is.EqualTo(ResizeMode.CanResize));

            window.ResizeMode = ResizeMode.CanMinimize;

            // min should be visible, max hidden
            Assert.That(minButton.IsVisible, Is.True);
            Assert.That(maxButton.IsVisible, Is.False);
            Assert.That(window.ResizeMode, Is.EqualTo(ResizeMode.CanMinimize));

            window.Close();
        }

        [Test]
        public async Task MinMaxButtonsShouldBeToggled()
        {
            var window = await WindowHelpers.CreateInvisibleWindowAsync<TestWindow>();

            var minButton = this.GetButton(window, "PART_Min");
            var maxButton = this.GetButton(window, "PART_Max");

            // min/max should be visible
            Assert.That(minButton.IsVisible, Is.True);
            Assert.That(maxButton.IsVisible, Is.True);
            Assert.That(window.ResizeMode, Is.EqualTo(ResizeMode.CanResize));

            window.ResizeMode = ResizeMode.CanMinimize;

            // min should be visible, max hidden
            Assert.That(minButton.IsVisible, Is.True);
            Assert.That(maxButton.IsVisible, Is.False);
            Assert.That(window.ResizeMode, Is.EqualTo(ResizeMode.CanMinimize));

            window.ShowMinButton = false;
            // min should be hidden
            Assert.That(minButton.IsVisible, Is.False);

            window.ResizeMode = ResizeMode.NoResize;

            // min/max should be hidden
            Assert.That(minButton.IsVisible, Is.False);
            Assert.That(maxButton.IsVisible, Is.False);
            Assert.That(window.ResizeMode, Is.EqualTo(ResizeMode.NoResize));

            window.ShowMaxRestoreButton = false;
            // max should be hidden
            Assert.That(maxButton.IsVisible, Is.False);

            window.ResizeMode = ResizeMode.CanResizeWithGrip;

            // min/max should be hidden
            Assert.That(minButton.IsVisible, Is.False);
            Assert.That(maxButton.IsVisible, Is.False);
            Assert.That(window.ResizeMode, Is.EqualTo(ResizeMode.CanResizeWithGrip));

            window.ShowMinButton = true;
            window.ShowMaxRestoreButton = true;
            // min/max should be visible
            Assert.That(minButton.IsVisible, Is.True);
            Assert.That(maxButton.IsVisible, Is.True);

            window.ResizeMode = ResizeMode.NoResize;

            // min/max should be hidden
            Assert.That(minButton.IsVisible, Is.False);
            Assert.That(maxButton.IsVisible, Is.False);
            Assert.That(window.ResizeMode, Is.EqualTo(ResizeMode.NoResize));

            window.Close();
        }

        /// <summary>
        /// #1362: ShowMinButton="False" and ShowMaxRestoreButton="False" not working
        /// </summary>
        [Test]
        public async Task MinMaxCloseButtonsShouldBeHidden()
        {
            var window = await WindowHelpers.CreateInvisibleWindowAsync<HiddenMinMaxCloseButtonsWindow>();

            var minButton = this.GetButton(window, "PART_Min");
            var maxButton = this.GetButton(window, "PART_Max");
            var closeButton = this.GetButton(window, "PART_Close");

            // min/max/close should be hidden
            Assert.That(minButton.IsVisible, Is.False);
            Assert.That(maxButton.IsVisible, Is.False);
            Assert.That(closeButton.IsVisible, Is.False);
            Assert.That(window.ResizeMode, Is.EqualTo(ResizeMode.CanResize));

            window.Close();
        }

        [Test]
        public async Task WindowSettingsUpgradeSettingsShouldBeTrueByDefault()
        {
            var window = await WindowHelpers.CreateInvisibleWindowAsync<TestWindow>();
            window.SaveWindowPosition = true;

            var settings = window.GetWindowPlacementSettings();
            Assert.That(settings, Is.Not.Null);
            Assert.That(settings.UpgradeSettings, Is.True);

            window.Close();
        }

        [Test]
        public async Task TestTitleCharacterCasingProperty()
        {
            var window = await WindowHelpers.CreateInvisibleWindowAsync<TestWindow>(w => w.Title = "Test");

            var titleBar = window.FindChild<ContentControl>("PART_TitleBar");
            Assert.That(titleBar, Is.Not.Null);

            var titleBarContent = titleBar.FindChild<ContentPresenter>("PART_ContentPresenter");
            Assert.That(titleBarContent, Is.Not.Null);

            var be = BindingOperations.GetBindingExpression(titleBarContent, ContentControl.ContentProperty);
            Assert.That(be, Is.Not.Null);
            be.UpdateTarget();

            // default should be UPPER
            Assert.That(window.TitleCharacterCasing, Is.EqualTo(CharacterCasing.Upper));
            Assert.That(titleBarContent.Content, Is.EqualTo("TEST"));

            window.TitleCharacterCasing = CharacterCasing.Lower;
            be.UpdateTarget();
            Assert.That(titleBarContent.Content, Is.EqualTo("test"));

            window.TitleCharacterCasing = CharacterCasing.Normal;
            be.UpdateTarget();
            Assert.That(titleBarContent.Content, Is.EqualTo("Test"));

            window.Close();
        }
    }
}