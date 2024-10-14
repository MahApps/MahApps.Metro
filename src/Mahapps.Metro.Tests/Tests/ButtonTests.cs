// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using MahApps.Metro.Controls;
using MahApps.Metro.Tests.TestHelpers;
using MahApps.Metro.Tests.Views;
using NUnit.Framework;

namespace MahApps.Metro.Tests.Tests
{
    [TestFixture]
    public class ButtonTests
    {
        [Test]
        public async Task DefaultButtonTextIsUpperCase()
        {
            var window = await WindowHelpers.CreateInvisibleWindowAsync<ButtonWindow>().ConfigureAwait(false);
            var presenter = window.DefaultButton.FindChild<ContentPresenter>("PART_ContentPresenter");

            Assert.That(presenter, Is.Not.Null);
            Assert.That(presenter.Content, Is.EqualTo("SOMETEXT"));

            window.Close();
        }

        [Test]
        public async Task DefaultButtonRespectsControlsHelperContentCharacterCasing()
        {
            var window = await WindowHelpers.CreateInvisibleWindowAsync<ButtonWindow>().ConfigureAwait(false);

            Button defaultButton = window.DefaultButton;
            Assert.That(defaultButton, Is.Not.Null);

            var presenter = defaultButton.FindChild<ContentPresenter>("PART_ContentPresenter");
            Assert.That(presenter, Is.Not.Null);

            defaultButton.SetValue(ControlsHelper.ContentCharacterCasingProperty, CharacterCasing.Normal);
            Assert.That(presenter.Content, Is.EqualTo("SomeText"));

            defaultButton.SetValue(ControlsHelper.ContentCharacterCasingProperty, CharacterCasing.Lower);
            Assert.That(presenter.Content, Is.EqualTo("sometext"));

            defaultButton.SetValue(ControlsHelper.ContentCharacterCasingProperty, CharacterCasing.Upper);
            Assert.That(presenter.Content, Is.EqualTo("SOMETEXT"));

            window.Close();
        }

        [Test]
        public async Task SquareButtonButtonTextIsLowerCase()
        {
            var window = await WindowHelpers.CreateInvisibleWindowAsync<ButtonWindow>().ConfigureAwait(false);
            var presenter = window.SquareButton.FindChild<ContentPresenter>("PART_ContentPresenter");

            Assert.That(presenter, Is.Not.Null);
            Assert.That(presenter.Content, Is.EqualTo("sometext"));

            window.Close();
        }

        [Test]
        public async Task SquareButtonRespectsButtonHelperContentCharacterCasing()
        {
            var window = await WindowHelpers.CreateInvisibleWindowAsync<ButtonWindow>().ConfigureAwait(false);

            Button squareButton = window.SquareButton;
            Assert.That(squareButton, Is.Not.Null);

            var presenter = squareButton.FindChild<ContentPresenter>("PART_ContentPresenter");
            Assert.That(presenter, Is.Not.Null);

            squareButton.SetValue(ControlsHelper.ContentCharacterCasingProperty, CharacterCasing.Normal);
            Assert.That(presenter.Content, Is.EqualTo("SomeText"));

            squareButton.SetValue(ControlsHelper.ContentCharacterCasingProperty, CharacterCasing.Lower);
            Assert.That(presenter.Content, Is.EqualTo("sometext"));

            squareButton.SetValue(ControlsHelper.ContentCharacterCasingProperty, CharacterCasing.Upper);
            Assert.That(presenter.Content, Is.EqualTo("SOMETEXT"));

            window.Close();
        }

        [Test]
        public async Task DropDownButtonShouldRespectParentIsEnabledProperty()
        {
            var window = await WindowHelpers.CreateInvisibleWindowAsync<ButtonWindow>().ConfigureAwait(false);

            window.TheStackPanel.SetCurrentValue(UIElement.IsEnabledProperty, false);
            Assert.That(window.TheDropDownButton.IsEnabled, Is.False);

            window.TheStackPanel.SetCurrentValue(UIElement.IsEnabledProperty, true);
            Assert.That(window.TheDropDownButton.IsEnabled, Is.True);

            window.Close();
        }

        [Test]
        public async Task SplitButtonShouldRespectParentIsEnabledProperty()
        {
            var window = await WindowHelpers.CreateInvisibleWindowAsync<ButtonWindow>().ConfigureAwait(false);

            window.TheStackPanel.SetCurrentValue(UIElement.IsEnabledProperty, false);
            Assert.That(window.TheSplitButton.IsEnabled, Is.False);

            window.TheStackPanel.SetCurrentValue(UIElement.IsEnabledProperty, true);
            Assert.That(window.TheSplitButton.IsEnabled, Is.True);

            window.Close();
        }
    }
}