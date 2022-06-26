// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using MahApps.Metro.Controls;
using MahApps.Metro.Tests.TestHelpers;
using Xunit;

namespace MahApps.Metro.Tests.Tests
{
    public class TextBoxHelperTests : AutomationTestFixtureBase<TextBoxHelperTestsFixture>
    {
        public TextBoxHelperTests(TextBoxHelperTestsFixture fixture)
            : base(fixture)
        {
        }

        [Fact]
        [DisplayTestMethodName]
        public async Task TestAttachedPropertyButtonWidth()
        {
            await this.fixture.PrepareForTestAsync(new[] { TextBoxHelper.ButtonWidthProperty.Name }).ConfigureAwait(false);
            await TestHost.SwitchToAppThread();

            const double width = 42d;

            this.fixture.Window?.TestTextBox.SetValue(TextBoxHelper.ButtonWidthProperty, width);
            Assert.Equal(width, this.fixture.Window?.TestTextBox.FindChild<Button>("PART_ClearText")?.Width);
            this.fixture.Window?.TestButtonTextBox.SetValue(TextBoxHelper.ButtonWidthProperty, width);
            Assert.Equal(width, this.fixture.Window?.TestButtonTextBox.FindChild<Button>("PART_ClearText")?.Width);

            this.fixture.Window?.TestPasswordBox.SetValue(TextBoxHelper.ButtonWidthProperty, width);
            Assert.Equal(width, this.fixture.Window?.TestPasswordBox.FindChild<Button>("PART_ClearText")?.Width);
            this.fixture.Window?.TestButtonRevealedPasswordBox.SetValue(TextBoxHelper.ButtonWidthProperty, width);
            Assert.Equal(width, this.fixture.Window?.TestButtonRevealedPasswordBox.FindChild<Button>("PART_ClearText")?.Width);
            Assert.Equal(width, this.fixture.Window?.TestButtonRevealedPasswordBox.FindChild<Button>("PART_RevealButton")?.Width);

            this.fixture.Window?.TestComboBox.SetValue(TextBoxHelper.ButtonWidthProperty, width);
            var toggleButton = this.fixture.Window?.TestComboBox.FindChild<ToggleButton>("PART_DropDownToggle");
            Assert.Equal(width, toggleButton?.FindChild<Button>("PART_ClearText")?.Width);
            Assert.Equal(width, toggleButton?.FindChild<Grid>("BtnArrowBackground")?.Width);

            this.fixture.Window?.TestEditableComboBox.SetValue(TextBoxHelper.ButtonWidthProperty, width);
            var toggleButtonEditable = this.fixture.Window?.TestEditableComboBox.FindChild<ToggleButton>("PART_DropDownToggle");
            Assert.Equal(width, toggleButtonEditable?.FindChild<Button>("PART_ClearText")?.Width);
            Assert.Equal(width, toggleButtonEditable?.FindChild<Grid>("BtnArrowBackground")?.Width);

            this.fixture.Window?.TestNumericUpDown.SetValue(TextBoxHelper.ButtonWidthProperty, width);
            Assert.Equal(width, this.fixture.Window?.TestNumericUpDown.FindChild<Button>("PART_ClearText")?.Width);

            this.fixture.Window?.TestHotKeyBox.SetValue(TextBoxHelper.ButtonWidthProperty, width);
            Assert.Equal(width, this.fixture.Window?.TestHotKeyBox.FindChild<Button>("PART_ClearText")?.Width);
        }

        [Fact]
        [DisplayTestMethodName]
        public async Task TestAttachedPropertyButtonContent()
        {
            await this.fixture.PrepareForTestAsync(new[] { TextBoxHelper.ButtonContentProperty.Name }).ConfigureAwait(false);
            await TestHost.SwitchToAppThread();

            var content = "42";

            this.fixture.Window?.TestTextBox.SetValue(TextBoxHelper.ButtonContentProperty, content);
            Assert.Equal(content, this.fixture.Window?.TestTextBox.FindChild<Button>("PART_ClearText")?.Content);
            this.fixture.Window?.TestButtonTextBox.SetValue(TextBoxHelper.ButtonContentProperty, content);
            Assert.Equal(content, this.fixture.Window?.TestButtonTextBox.FindChild<Button>("PART_ClearText")?.Content);

            this.fixture.Window?.TestPasswordBox.SetValue(TextBoxHelper.ButtonContentProperty, content);
            Assert.Equal(content, this.fixture.Window?.TestPasswordBox.FindChild<Button>("PART_ClearText")?.Content);
            this.fixture.Window?.TestButtonRevealedPasswordBox.SetValue(TextBoxHelper.ButtonContentProperty, content);
            Assert.Equal(content, this.fixture.Window?.TestButtonRevealedPasswordBox.FindChild<Button>("PART_ClearText")?.Content);

            this.fixture.Window?.TestComboBox.SetValue(TextBoxHelper.ButtonContentProperty, content);
            var toggleButton = this.fixture.Window?.TestComboBox.FindChild<ToggleButton>("PART_DropDownToggle");
            Assert.Equal(content, toggleButton.FindChild<Button>("PART_ClearText")?.Content);

            this.fixture.Window?.TestEditableComboBox.SetValue(TextBoxHelper.ButtonContentProperty, content);
            var toggleButtonEditable = this.fixture.Window?.TestEditableComboBox.FindChild<ToggleButton>("PART_DropDownToggle");
            Assert.Equal(content, toggleButtonEditable.FindChild<Button>("PART_ClearText")?.Content);

            this.fixture.Window?.TestNumericUpDown.SetValue(TextBoxHelper.ButtonContentProperty, content);
            Assert.Equal(content, this.fixture.Window?.TestNumericUpDown.FindChild<Button>("PART_ClearText")?.Content);

            this.fixture.Window?.TestHotKeyBox.SetValue(TextBoxHelper.ButtonContentProperty, content);
            Assert.Equal(content, this.fixture.Window?.TestHotKeyBox.FindChild<Button>("PART_ClearText")?.Content);
        }

        [Fact]
        [DisplayTestMethodName]
        public async Task TestAttachedPropertyButtonContentTemplate()
        {
            await this.fixture.PrepareForTestAsync(new[] { TextBoxHelper.ButtonContentTemplateProperty.Name }).ConfigureAwait(false);
            await TestHost.SwitchToAppThread();

            const string resourceKey = "TestDataTemplate";
            var dataTemplate = this.fixture.Window?.TryFindResource(resourceKey) as DataTemplate;
            Assert.NotNull(dataTemplate);

            this.fixture.Window?.TestTextBox.SetResourceReference(TextBoxHelper.ButtonContentTemplateProperty, resourceKey);
            Assert.Equal(dataTemplate, this.fixture.Window?.TestTextBox.FindChild<Button>("PART_ClearText")?.ContentTemplate);
            this.fixture.Window?.TestButtonTextBox.SetResourceReference(TextBoxHelper.ButtonContentTemplateProperty, resourceKey);
            Assert.Equal(dataTemplate, this.fixture.Window?.TestButtonTextBox.FindChild<Button>("PART_ClearText")?.ContentTemplate);

            this.fixture.Window?.TestPasswordBox.SetResourceReference(TextBoxHelper.ButtonContentTemplateProperty, resourceKey);
            Assert.Equal(dataTemplate, this.fixture.Window?.TestPasswordBox.FindChild<Button>("PART_ClearText")?.ContentTemplate);
            this.fixture.Window?.TestButtonRevealedPasswordBox.SetResourceReference(TextBoxHelper.ButtonContentTemplateProperty, resourceKey);
            Assert.Equal(dataTemplate, this.fixture.Window?.TestButtonRevealedPasswordBox.FindChild<Button>("PART_ClearText")?.ContentTemplate);

            this.fixture.Window?.TestComboBox.SetResourceReference(TextBoxHelper.ButtonContentTemplateProperty, resourceKey);
            var toggleButton = this.fixture.Window?.TestComboBox.FindChild<ToggleButton>("PART_DropDownToggle");
            Assert.Equal(dataTemplate, toggleButton.FindChild<Button>("PART_ClearText")?.ContentTemplate);

            this.fixture.Window?.TestEditableComboBox.SetResourceReference(TextBoxHelper.ButtonContentTemplateProperty, resourceKey);
            var toggleButtonEditable = this.fixture.Window?.TestEditableComboBox.FindChild<ToggleButton>("PART_DropDownToggle");
            Assert.Equal(dataTemplate, toggleButtonEditable.FindChild<Button>("PART_ClearText")?.ContentTemplate);

            this.fixture.Window?.TestNumericUpDown.SetResourceReference(TextBoxHelper.ButtonContentTemplateProperty, resourceKey);
            Assert.Equal(dataTemplate, this.fixture.Window?.TestNumericUpDown.FindChild<Button>("PART_ClearText")?.ContentTemplate);

            this.fixture.Window?.TestHotKeyBox.SetResourceReference(TextBoxHelper.ButtonContentTemplateProperty, resourceKey);
            Assert.Equal(dataTemplate, this.fixture.Window?.TestHotKeyBox.FindChild<Button>("PART_ClearText")?.ContentTemplate);
        }

        [Fact]
        [DisplayTestMethodName]
        public async Task TestAttachedPropertyButtonFontFamily()
        {
            await this.fixture.PrepareForTestAsync(new[] { TextBoxHelper.ButtonFontFamilyProperty.Name }).ConfigureAwait(false);
            await TestHost.SwitchToAppThread();

            var fontFamily = new FontFamilyConverter().ConvertFromString("Arial");

            this.fixture.Window?.TestTextBox.SetValue(TextBoxHelper.ButtonFontFamilyProperty, fontFamily);
            Assert.Equal(fontFamily, this.fixture.Window?.TestTextBox.FindChild<Button>("PART_ClearText")?.FontFamily);
            this.fixture.Window?.TestButtonTextBox.SetValue(TextBoxHelper.ButtonFontFamilyProperty, fontFamily);
            Assert.Equal(fontFamily, this.fixture.Window?.TestButtonTextBox.FindChild<Button>("PART_ClearText")?.FontFamily);

            this.fixture.Window?.TestPasswordBox.SetValue(TextBoxHelper.ButtonFontFamilyProperty, fontFamily);
            Assert.Equal(fontFamily, this.fixture.Window?.TestPasswordBox.FindChild<Button>("PART_ClearText")?.FontFamily);
            this.fixture.Window?.TestButtonRevealedPasswordBox.SetValue(TextBoxHelper.ButtonFontFamilyProperty, fontFamily);
            Assert.Equal(fontFamily, this.fixture.Window?.TestButtonRevealedPasswordBox.FindChild<Button>("PART_ClearText")?.FontFamily);

            this.fixture.Window?.TestComboBox.SetValue(TextBoxHelper.ButtonFontFamilyProperty, fontFamily);
            var toggleButton = this.fixture.Window?.TestComboBox.FindChild<ToggleButton>("PART_DropDownToggle");
            Assert.Equal(fontFamily, toggleButton.FindChild<Button>("PART_ClearText")?.FontFamily);

            this.fixture.Window?.TestEditableComboBox.SetValue(TextBoxHelper.ButtonFontFamilyProperty, fontFamily);
            var toggleButtonEditable = this.fixture.Window?.TestEditableComboBox.FindChild<ToggleButton>("PART_DropDownToggle");
            Assert.Equal(fontFamily, toggleButtonEditable.FindChild<Button>("PART_ClearText")?.FontFamily);

            this.fixture.Window?.TestNumericUpDown.SetValue(TextBoxHelper.ButtonFontFamilyProperty, fontFamily);
            Assert.Equal(fontFamily, this.fixture.Window?.TestNumericUpDown.FindChild<Button>("PART_ClearText")?.FontFamily);

            this.fixture.Window?.TestHotKeyBox.SetValue(TextBoxHelper.ButtonFontFamilyProperty, fontFamily);
            Assert.Equal(fontFamily, this.fixture.Window?.TestHotKeyBox.FindChild<Button>("PART_ClearText")?.FontFamily);
        }

        [Fact]
        [DisplayTestMethodName]
        public async Task TestAttachedPropertyButtonFontSize()
        {
            await this.fixture.PrepareForTestAsync(new[] { TextBoxHelper.ButtonFontSizeProperty.Name }).ConfigureAwait(false);
            await TestHost.SwitchToAppThread();

            var fontSize = 42d;

            this.fixture.Window?.TestTextBox.SetValue(TextBoxHelper.ButtonFontSizeProperty, fontSize);
            Assert.Equal(fontSize, this.fixture.Window?.TestTextBox.FindChild<Button>("PART_ClearText")?.FontSize);
            this.fixture.Window?.TestButtonTextBox.SetValue(TextBoxHelper.ButtonFontSizeProperty, fontSize);
            Assert.Equal(fontSize, this.fixture.Window?.TestButtonTextBox.FindChild<Button>("PART_ClearText")?.FontSize);

            this.fixture.Window?.TestPasswordBox.SetValue(TextBoxHelper.ButtonFontSizeProperty, fontSize);
            Assert.Equal(fontSize, this.fixture.Window?.TestPasswordBox.FindChild<Button>("PART_ClearText")?.FontSize);
            this.fixture.Window?.TestButtonRevealedPasswordBox.SetValue(TextBoxHelper.ButtonFontSizeProperty, fontSize);
            Assert.Equal(fontSize, this.fixture.Window?.TestButtonRevealedPasswordBox.FindChild<Button>("PART_ClearText")?.FontSize);

            this.fixture.Window?.TestComboBox.SetValue(TextBoxHelper.ButtonFontSizeProperty, fontSize);
            var toggleButton = this.fixture.Window?.TestComboBox.FindChild<ToggleButton>("PART_DropDownToggle");
            Assert.Equal(fontSize, toggleButton.FindChild<Button>("PART_ClearText")?.FontSize);

            this.fixture.Window?.TestEditableComboBox.SetValue(TextBoxHelper.ButtonFontSizeProperty, fontSize);
            var toggleButtonEditable = this.fixture.Window?.TestEditableComboBox.FindChild<ToggleButton>("PART_DropDownToggle");
            Assert.Equal(fontSize, toggleButtonEditable.FindChild<Button>("PART_ClearText")?.FontSize);

            this.fixture.Window?.TestNumericUpDown.SetValue(TextBoxHelper.ButtonFontSizeProperty, fontSize);
            Assert.Equal(fontSize, this.fixture.Window?.TestNumericUpDown.FindChild<Button>("PART_ClearText")?.FontSize);

            this.fixture.Window?.TestHotKeyBox.SetValue(TextBoxHelper.ButtonFontSizeProperty, fontSize);
            Assert.Equal(fontSize, this.fixture.Window?.TestHotKeyBox.FindChild<Button>("PART_ClearText")?.FontSize);
        }

        [Fact]
        [DisplayTestMethodName]
        public async Task TestAttachedPropertyButtonTemplate()
        {
            await this.fixture.PrepareForTestAsync(new[] { TextBoxHelper.ButtonTemplateProperty.Name }).ConfigureAwait(false);
            await TestHost.SwitchToAppThread();

            const string resourceKey = "TestControlTemplate";
            var controlTemplate = this.fixture.Window?.TryFindResource(resourceKey) as ControlTemplate;
            Assert.NotNull(controlTemplate);

            this.fixture.Window?.TestButtonTextBox.SetResourceReference(TextBoxHelper.ButtonTemplateProperty, resourceKey);
            Assert.Equal(controlTemplate, this.fixture.Window?.TestButtonTextBox.FindChild<Button>("PART_ClearText")?.Template);

            this.fixture.Window?.TestButtonRevealedPasswordBox.SetResourceReference(TextBoxHelper.ButtonTemplateProperty, resourceKey);
            Assert.Equal(controlTemplate, this.fixture.Window?.TestButtonRevealedPasswordBox.FindChild<Button>("PART_ClearText")?.Template);
        }
    }
}