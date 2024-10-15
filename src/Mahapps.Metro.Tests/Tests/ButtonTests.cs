// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
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
        private ButtonWindow? window;

        [OneTimeSetUp]
        public async Task OneTimeSetUp()
        {
            this.window = await WindowHelpers.CreateInvisibleWindowAsync<ButtonWindow>().ConfigureAwait(false);
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            this.window?.Close();
            this.window = null;
        }

        [SetUp]
        public void SetUp()
        {
            this.PreparePropertiesForTest([
                ControlsHelper.ContentCharacterCasingProperty.Name,
                UIElement.IsEnabledProperty.Name
            ]);
        }

        private void PreparePropertiesForTest(IList<string>? properties = null)
        {
            this.window?.DefaultButton.ClearDependencyProperties(properties);
            this.window?.SquareButton.ClearDependencyProperties(properties);
            this.window?.TheDropDownButton.ClearDependencyProperties(properties);
            this.window?.TheSplitButton.ClearDependencyProperties(properties);
        }

        [Test]
        public void DefaultButtonTextIsUpperCase()
        {
            Assert.That(this.window, Is.Not.Null);

            var presenter = this.window.DefaultButton.FindChild<ContentPresenter>("PART_ContentPresenter");

            Assert.That(presenter, Is.Not.Null);
            Assert.That(presenter.Content, Is.EqualTo("SOMETEXT"));
        }

        [Test]
        public void DefaultButtonRespectsControlsHelperContentCharacterCasing()
        {
            Assert.That(this.window, Is.Not.Null);

            Button defaultButton = this.window.DefaultButton;
            Assert.That(defaultButton, Is.Not.Null);

            var presenter = defaultButton.FindChild<ContentPresenter>("PART_ContentPresenter");
            Assert.That(presenter, Is.Not.Null);

            defaultButton.SetValue(ControlsHelper.ContentCharacterCasingProperty, CharacterCasing.Normal);
            Assert.That(presenter.Content, Is.EqualTo("SomeText"));

            defaultButton.SetValue(ControlsHelper.ContentCharacterCasingProperty, CharacterCasing.Lower);
            Assert.That(presenter.Content, Is.EqualTo("sometext"));

            defaultButton.SetValue(ControlsHelper.ContentCharacterCasingProperty, CharacterCasing.Upper);
            Assert.That(presenter.Content, Is.EqualTo("SOMETEXT"));
        }

        [Test]
        public void SquareButtonButtonTextIsLowerCase()
        {
            Assert.That(this.window, Is.Not.Null);

            var presenter = this.window.SquareButton.FindChild<ContentPresenter>("PART_ContentPresenter");

            Assert.That(presenter, Is.Not.Null);
            Assert.That(presenter.Content, Is.EqualTo("sometext"));
        }

        [Test]
        public void SquareButtonRespectsButtonHelperContentCharacterCasing()
        {
            Assert.That(this.window, Is.Not.Null);

            Button squareButton = this.window.SquareButton;
            Assert.That(squareButton, Is.Not.Null);

            var presenter = squareButton.FindChild<ContentPresenter>("PART_ContentPresenter");
            Assert.That(presenter, Is.Not.Null);

            squareButton.SetValue(ControlsHelper.ContentCharacterCasingProperty, CharacterCasing.Normal);
            Assert.That(presenter.Content, Is.EqualTo("SomeText"));

            squareButton.SetValue(ControlsHelper.ContentCharacterCasingProperty, CharacterCasing.Lower);
            Assert.That(presenter.Content, Is.EqualTo("sometext"));

            squareButton.SetValue(ControlsHelper.ContentCharacterCasingProperty, CharacterCasing.Upper);
            Assert.That(presenter.Content, Is.EqualTo("SOMETEXT"));
        }

        [Test]
        public void DropDownButtonShouldRespectParentIsEnabledProperty()
        {
            Assert.That(this.window, Is.Not.Null);

            this.window.TheStackPanel.SetCurrentValue(UIElement.IsEnabledProperty, false);
            Assert.That(this.window.TheDropDownButton.IsEnabled, Is.False);

            this.window.TheStackPanel.SetCurrentValue(UIElement.IsEnabledProperty, true);
            Assert.That(this.window.TheDropDownButton.IsEnabled, Is.True);
        }

        [Test]
        public void SplitButtonShouldRespectParentIsEnabledProperty()
        {
            Assert.That(this.window, Is.Not.Null);

            this.window.TheStackPanel.SetCurrentValue(UIElement.IsEnabledProperty, false);
            Assert.That(this.window.TheSplitButton.IsEnabled, Is.False);

            this.window.TheStackPanel.SetCurrentValue(UIElement.IsEnabledProperty, true);
            Assert.That(this.window.TheSplitButton.IsEnabled, Is.True);
        }
    }
}