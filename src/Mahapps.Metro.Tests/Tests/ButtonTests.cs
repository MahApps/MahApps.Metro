// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using MahApps.Metro.Controls;
using MahApps.Metro.Tests.TestHelpers;
using Xunit;

namespace MahApps.Metro.Tests.Tests
{
    public class ButtonTests : AutomationTestFixtureBase<ButtonTestsFixture>
    {
        public ButtonTests(ButtonTestsFixture fixture)
            : base(fixture)
        {
        }

        [Fact]
        [DisplayTestMethodName]
        public async Task DefaultButtonTextIsUpperCase()
        {
            await TestHost.SwitchToAppThread();

            var presenter = this.fixture.Window?.DefaultButton.FindChild<ContentPresenter>("PART_ContentPresenter");

            Assert.NotNull(presenter);

            Assert.Equal("SOMETEXT", presenter.Content);
        }

        [Fact]
        [DisplayTestMethodName]
        public async Task DefaultButtonRespectsControlsHelperContentCharacterCasing()
        {
            await this.fixture.PrepareForTestAsync(new[] { ControlsHelper.ContentCharacterCasingProperty.Name }).ConfigureAwait(false);
            await TestHost.SwitchToAppThread();

            Button defaultButton = this.fixture.Window?.DefaultButton;
            Assert.NotNull(defaultButton);

            var presenter = defaultButton.FindChild<ContentPresenter>("PART_ContentPresenter");
            Assert.NotNull(presenter);

            defaultButton.SetValue(ControlsHelper.ContentCharacterCasingProperty, CharacterCasing.Normal);
            Assert.Equal("SomeText", presenter.Content);

            defaultButton.SetValue(ControlsHelper.ContentCharacterCasingProperty, CharacterCasing.Lower);
            Assert.Equal("sometext", presenter.Content);

            defaultButton.SetValue(ControlsHelper.ContentCharacterCasingProperty, CharacterCasing.Upper);
            Assert.Equal("SOMETEXT", presenter.Content);
        }

        [Fact]
        [DisplayTestMethodName]
        public async Task SquareButtonButtonTextIsLowerCase()
        {
            await TestHost.SwitchToAppThread();

            var presenter = this.fixture.Window?.SquareButton.FindChild<ContentPresenter>("PART_ContentPresenter");

            Assert.NotNull(presenter);

            Assert.Equal("sometext", presenter.Content);
        }

        [Fact]
        [DisplayTestMethodName]
        public async Task SquareButtonRespectsButtonHelperContentCharacterCasing()
        {
            await this.fixture.PrepareForTestAsync(new[] { ControlsHelper.ContentCharacterCasingProperty.Name }).ConfigureAwait(false);
            await TestHost.SwitchToAppThread();

            Button squareButton = this.fixture.Window?.SquareButton;
            Assert.NotNull(squareButton);

            var presenter = squareButton.FindChild<ContentPresenter>("PART_ContentPresenter");
            Assert.NotNull(presenter);

            squareButton.SetValue(ControlsHelper.ContentCharacterCasingProperty, CharacterCasing.Normal);
            Assert.Equal("SomeText", presenter.Content);

            squareButton.SetValue(ControlsHelper.ContentCharacterCasingProperty, CharacterCasing.Lower);
            Assert.Equal("sometext", presenter.Content);

            squareButton.SetValue(ControlsHelper.ContentCharacterCasingProperty, CharacterCasing.Upper);
            Assert.Equal("SOMETEXT", presenter.Content);
            await TestHost.SwitchToAppThread();
        }

        [Fact]
        [DisplayTestMethodName]
        public async Task DropDownButtonShouldRespectParentIsEnabledProperty()
        {
            await this.fixture.PrepareForTestAsync(new[] { UIElement.IsEnabledProperty.Name }).ConfigureAwait(false);
            await TestHost.SwitchToAppThread();

            this.fixture.Window?.TheStackPanel.SetCurrentValue(UIElement.IsEnabledProperty, false);
            Assert.False(this.fixture.Window?.TheDropDownButton.IsEnabled);

            this.fixture.Window?.TheStackPanel.SetCurrentValue(UIElement.IsEnabledProperty, true);
            Assert.True(this.fixture.Window?.TheDropDownButton.IsEnabled);
        }

        [Fact]
        [DisplayTestMethodName]
        public async Task SplitButtonShouldRespectParentIsEnabledProperty()
        {
            await this.fixture.PrepareForTestAsync(new[] { UIElement.IsEnabledProperty.Name }).ConfigureAwait(false);
            await TestHost.SwitchToAppThread();

            this.fixture.Window?.TheStackPanel.SetCurrentValue(UIElement.IsEnabledProperty, false);
            Assert.False(this.fixture.Window?.TheSplitButton.IsEnabled);

            this.fixture.Window?.TheStackPanel.SetCurrentValue(UIElement.IsEnabledProperty, true);
            Assert.True(this.fixture.Window?.TheSplitButton.IsEnabled);
        }
    }
}