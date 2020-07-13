// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using MahApps.Metro.Controls;
using MahApps.Metro.Tests.TestHelpers;
using Xunit;

namespace MahApps.Metro.Tests
{
    public class ButtonTest : AutomationTestBase
    {
        [Fact]
        [DisplayTestMethodName]
        public async Task DefaultButtonTextIsUpperCase()
        {
            await TestHost.SwitchToAppThread();

            var window = await WindowHelpers.CreateInvisibleWindowAsync<ButtonWindow>();
            var presenter = window.DefaultButton.FindChild<ContentPresenter>("PART_ContentPresenter");

            Assert.Equal("SOMETEXT", presenter.Content);
        }

        [Fact]
        [DisplayTestMethodName]
        public async Task DefaultButtonRespectsControlsHelperContentCharacterCasing()
        {
            await TestHost.SwitchToAppThread();

            var window = await WindowHelpers.CreateInvisibleWindowAsync<ButtonWindow>();

            Button defaultButton = window.DefaultButton;
            var presenter = defaultButton.FindChild<ContentPresenter>("PART_ContentPresenter");

            ControlsHelper.SetContentCharacterCasing(defaultButton, CharacterCasing.Normal);
            Assert.Equal("SomeText", presenter.Content);

            ControlsHelper.SetContentCharacterCasing(defaultButton, CharacterCasing.Lower);
            Assert.Equal("sometext", presenter.Content);

            ControlsHelper.SetContentCharacterCasing(defaultButton, CharacterCasing.Upper);
            Assert.Equal("SOMETEXT", presenter.Content);
        }

        [Fact]
        [DisplayTestMethodName]
        public async Task SquareButtonButtonTextIsLowerCase()
        {
            await TestHost.SwitchToAppThread();

            var window = await WindowHelpers.CreateInvisibleWindowAsync<ButtonWindow>();
            var presenter = window.SquareButton.FindChild<ContentPresenter>("PART_ContentPresenter");

            Assert.Equal("sometext", presenter.Content);
        }

        [Fact]
        [DisplayTestMethodName]
        public async Task SquareButtonRespectsButtonHelperContentCharacterCasing()
        {
            await TestHost.SwitchToAppThread();

            var window = await WindowHelpers.CreateInvisibleWindowAsync<ButtonWindow>();

            Button defaultButton = window.SquareButton;
            ControlsHelper.SetContentCharacterCasing(defaultButton, CharacterCasing.Normal);
            var presenter = defaultButton.FindChild<ContentPresenter>("PART_ContentPresenter");

            Assert.Equal("SomeText", presenter.Content);
        }

        [Fact]
        [DisplayTestMethodName]
        public async Task DropDownButtonShouldRespectParentIsEnabledProperty()
        {
            await TestHost.SwitchToAppThread();

            var window = await WindowHelpers.CreateInvisibleWindowAsync<ButtonWindow>();

            window.TheStackPanel.SetCurrentValue(UIElement.IsEnabledProperty, false);
            Assert.False(window.TheDropDownButton.IsEnabled);

            window.TheStackPanel.SetCurrentValue(UIElement.IsEnabledProperty, true);
            Assert.True(window.TheDropDownButton.IsEnabled);
        }

        [Fact]
        [DisplayTestMethodName]
        public async Task SplitButtonShouldRespectParentIsEnabledProperty()
        {
            await TestHost.SwitchToAppThread();

            var window = await WindowHelpers.CreateInvisibleWindowAsync<ButtonWindow>();

            window.TheStackPanel.SetCurrentValue(UIElement.IsEnabledProperty, false);
            Assert.False(window.TheSplitButton.IsEnabled);

            window.TheStackPanel.SetCurrentValue(UIElement.IsEnabledProperty, true);
            Assert.True(window.TheSplitButton.IsEnabled);
        }
    }
}