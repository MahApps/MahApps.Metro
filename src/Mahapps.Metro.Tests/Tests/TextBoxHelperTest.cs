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
using MahApps.Metro.Tests.Views;
using Xunit;

namespace MahApps.Metro.Tests.Tests
{
    public class TextBoxHelperTest : AutomationTestBase
    {
        [Fact]
        [DisplayTestMethodName]
        public async Task TestAttachedPropertyButtonWidth()
        {
            await TestHost.SwitchToAppThread();

            var window = await WindowHelpers.CreateInvisibleWindowAsync<TextBoxHelperTestWindow>().ConfigureAwait(false);

            window.Invoke(() =>
                {
                    const double width = 42d;

                    window.TestTextBox.SetValue(TextBoxHelper.ButtonWidthProperty, width);
                    Assert.Equal(width, window.TestTextBox.FindChild<Button>("PART_ClearText")?.Width);
                    window.TestButtonTextBox.SetValue(TextBoxHelper.ButtonWidthProperty, width);
                    Assert.Equal(width, window.TestButtonTextBox.FindChild<Button>("PART_ClearText")?.Width);

                    window.TestPasswordBox.SetValue(TextBoxHelper.ButtonWidthProperty, width);
                    Assert.Equal(width, window.TestPasswordBox.FindChild<Button>("PART_ClearText")?.Width);
                    window.TestButtonPasswordBox.SetValue(TextBoxHelper.ButtonWidthProperty, width);
                    Assert.Equal(width, window.TestButtonPasswordBox.FindChild<Button>("PART_ClearText")?.Width);
                    window.TestButtonRevealedPasswordBox.SetValue(TextBoxHelper.ButtonWidthProperty, width);
                    Assert.Equal(width, window.TestButtonRevealedPasswordBox.FindChild<Button>("PART_ClearText")?.Width);
                    Assert.Equal(width, window.TestButtonRevealedPasswordBox.FindChild<Button>("PART_RevealButton")?.Width);

                    window.TestComboBox.SetValue(TextBoxHelper.ButtonWidthProperty, width);
                    var toggleButton = window.TestComboBox.FindChild<ToggleButton>("PART_DropDownToggle");
                    Assert.Equal(width, toggleButton?.FindChild<Button>("PART_ClearText")?.Width);
                    Assert.Equal(width, toggleButton?.FindChild<Grid>("BtnArrowBackground")?.Width);

                    window.TestEditableComboBox.SetValue(TextBoxHelper.ButtonWidthProperty, width);
                    var edTextBox = window.TestEditableComboBox.FindChild<TextBox>("PART_EditableTextBox");
                    Assert.Equal(width, edTextBox?.FindChild<Button>("PART_ClearText")?.Width);

                    window.TestNumericUpDown.SetValue(TextBoxHelper.ButtonWidthProperty, width);
                    Assert.Equal(width, window.TestNumericUpDown.FindChild<Button>("PART_ClearText")?.Width);

                    window.TestHotKeyBox.SetValue(TextBoxHelper.ButtonWidthProperty, width);
                    Assert.Equal(width, window.TestHotKeyBox.FindChild<Button>("PART_ClearText")?.Width);
                });
        }

        [Fact]
        [DisplayTestMethodName]
        public async Task TestAttachedPropertyButtonContent()
        {
            await TestHost.SwitchToAppThread();

            var window = await WindowHelpers.CreateInvisibleWindowAsync<TextBoxHelperTestWindow>().ConfigureAwait(false);

            window.Invoke(() =>
                {
                    var content = "42";

                    window.TestTextBox.SetValue(TextBoxHelper.ButtonContentProperty, content);
                    Assert.Equal(content, window.TestTextBox.FindChild<Button>("PART_ClearText")?.Content);
                    window.TestButtonTextBox.SetValue(TextBoxHelper.ButtonContentProperty, content);
                    Assert.Equal(content, window.TestButtonTextBox.FindChild<Button>("PART_ClearText")?.Content);

                    window.TestPasswordBox.SetValue(TextBoxHelper.ButtonContentProperty, content);
                    Assert.Equal(content, window.TestPasswordBox.FindChild<Button>("PART_ClearText")?.Content);
                    window.TestButtonPasswordBox.SetValue(TextBoxHelper.ButtonContentProperty, content);
                    Assert.Equal(content, window.TestButtonPasswordBox.FindChild<Button>("PART_ClearText")?.Content);
                    window.TestButtonRevealedPasswordBox.SetValue(TextBoxHelper.ButtonContentProperty, content);
                    Assert.Equal(content, window.TestButtonRevealedPasswordBox.FindChild<Button>("PART_ClearText")?.Content);

                    window.TestComboBox.SetValue(TextBoxHelper.ButtonContentProperty, content);
                    var toggleButton = window.TestComboBox.FindChild<ToggleButton>("PART_DropDownToggle");
                    Assert.Equal(content, toggleButton.FindChild<Button>("PART_ClearText")?.Content);

                    window.TestEditableComboBox.SetValue(TextBoxHelper.ButtonContentProperty, content);
                    var edTextBox = window.TestEditableComboBox.FindChild<TextBox>("PART_EditableTextBox");
                    Assert.Equal(content, edTextBox.FindChild<Button>("PART_ClearText")?.Content);

                    window.TestNumericUpDown.SetValue(TextBoxHelper.ButtonContentProperty, content);
                    Assert.Equal(content, window.TestNumericUpDown.FindChild<Button>("PART_ClearText")?.Content);

                    window.TestHotKeyBox.SetValue(TextBoxHelper.ButtonContentProperty, content);
                    Assert.Equal(content, window.TestHotKeyBox.FindChild<Button>("PART_ClearText")?.Content);
                });
        }

        [Fact]
        [DisplayTestMethodName]
        public async Task TestAttachedPropertyButtonContentTemplate()
        {
            await TestHost.SwitchToAppThread();

            var window = await WindowHelpers.CreateInvisibleWindowAsync<TextBoxHelperTestWindow>().ConfigureAwait(false);

            window.Invoke(() =>
                {
                    const string resourceKey = "TestDataTemplate";
                    var dataTemplate = window.TryFindResource(resourceKey) as DataTemplate;
                    Assert.NotNull(dataTemplate);

                    window.TestTextBox.SetResourceReference(TextBoxHelper.ButtonContentTemplateProperty, resourceKey);
                    Assert.Equal(dataTemplate, window.TestTextBox.FindChild<Button>("PART_ClearText")?.ContentTemplate);
                    window.TestButtonTextBox.SetResourceReference(TextBoxHelper.ButtonContentTemplateProperty, resourceKey);
                    Assert.Equal(dataTemplate, window.TestButtonTextBox.FindChild<Button>("PART_ClearText")?.ContentTemplate);

                    window.TestPasswordBox.SetResourceReference(TextBoxHelper.ButtonContentTemplateProperty, resourceKey);
                    Assert.Equal(dataTemplate, window.TestPasswordBox.FindChild<Button>("PART_ClearText")?.ContentTemplate);
                    window.TestButtonPasswordBox.SetResourceReference(TextBoxHelper.ButtonContentTemplateProperty, resourceKey);
                    Assert.Equal(dataTemplate, window.TestButtonPasswordBox.FindChild<Button>("PART_ClearText")?.ContentTemplate);
                    window.TestButtonRevealedPasswordBox.SetResourceReference(TextBoxHelper.ButtonContentTemplateProperty, resourceKey);
                    Assert.Equal(dataTemplate, window.TestButtonRevealedPasswordBox.FindChild<Button>("PART_ClearText")?.ContentTemplate);

                    window.TestComboBox.SetResourceReference(TextBoxHelper.ButtonContentTemplateProperty, resourceKey);
                    var toggleButton = window.TestComboBox.FindChild<ToggleButton>("PART_DropDownToggle");
                    Assert.Equal(dataTemplate, toggleButton.FindChild<Button>("PART_ClearText")?.ContentTemplate);

                    window.TestEditableComboBox.SetResourceReference(TextBoxHelper.ButtonContentTemplateProperty, resourceKey);
                    var edTextBox = window.TestEditableComboBox.FindChild<TextBox>("PART_EditableTextBox");
                    Assert.Equal(dataTemplate, edTextBox.FindChild<Button>("PART_ClearText")?.ContentTemplate);

                    window.TestNumericUpDown.SetResourceReference(TextBoxHelper.ButtonContentTemplateProperty, resourceKey);
                    Assert.Equal(dataTemplate, window.TestNumericUpDown.FindChild<Button>("PART_ClearText")?.ContentTemplate);

                    window.TestHotKeyBox.SetResourceReference(TextBoxHelper.ButtonContentTemplateProperty, resourceKey);
                    Assert.Equal(dataTemplate, window.TestHotKeyBox.FindChild<Button>("PART_ClearText")?.ContentTemplate);
                });
        }

        [Fact]
        [DisplayTestMethodName]
        public async Task TestAttachedPropertyButtonFontFamily()
        {
            await TestHost.SwitchToAppThread();

            var window = await WindowHelpers.CreateInvisibleWindowAsync<TextBoxHelperTestWindow>().ConfigureAwait(false);

            window.Invoke(() =>
                {
                    var fontFamily = new FontFamilyConverter().ConvertFromString("Arial");

                    window.TestTextBox.SetValue(TextBoxHelper.ButtonFontFamilyProperty, fontFamily);
                    Assert.Equal(fontFamily, window.TestTextBox.FindChild<Button>("PART_ClearText")?.FontFamily);
                    window.TestButtonTextBox.SetValue(TextBoxHelper.ButtonFontFamilyProperty, fontFamily);
                    Assert.Equal(fontFamily, window.TestButtonTextBox.FindChild<Button>("PART_ClearText")?.FontFamily);

                    window.TestPasswordBox.SetValue(TextBoxHelper.ButtonFontFamilyProperty, fontFamily);
                    Assert.Equal(fontFamily, window.TestPasswordBox.FindChild<Button>("PART_ClearText")?.FontFamily);
                    window.TestButtonPasswordBox.SetValue(TextBoxHelper.ButtonFontFamilyProperty, fontFamily);
                    Assert.Equal(fontFamily, window.TestButtonPasswordBox.FindChild<Button>("PART_ClearText")?.FontFamily);
                    window.TestButtonRevealedPasswordBox.SetValue(TextBoxHelper.ButtonFontFamilyProperty, fontFamily);
                    Assert.Equal(fontFamily, window.TestButtonRevealedPasswordBox.FindChild<Button>("PART_ClearText")?.FontFamily);

                    window.TestComboBox.SetValue(TextBoxHelper.ButtonFontFamilyProperty, fontFamily);
                    var toggleButton = window.TestComboBox.FindChild<ToggleButton>("PART_DropDownToggle");
                    Assert.Equal(fontFamily, toggleButton.FindChild<Button>("PART_ClearText")?.FontFamily);

                    window.TestEditableComboBox.SetValue(TextBoxHelper.ButtonFontFamilyProperty, fontFamily);
                    var edTextBox = window.TestEditableComboBox.FindChild<TextBox>("PART_EditableTextBox");
                    Assert.Equal(fontFamily, edTextBox.FindChild<Button>("PART_ClearText")?.FontFamily);

                    window.TestNumericUpDown.SetValue(TextBoxHelper.ButtonFontFamilyProperty, fontFamily);
                    Assert.Equal(fontFamily, window.TestNumericUpDown.FindChild<Button>("PART_ClearText")?.FontFamily);

                    window.TestHotKeyBox.SetValue(TextBoxHelper.ButtonFontFamilyProperty, fontFamily);
                    Assert.Equal(fontFamily, window.TestHotKeyBox.FindChild<Button>("PART_ClearText")?.FontFamily);
                });
        }

        [Fact]
        [DisplayTestMethodName]
        public async Task TestAttachedPropertyButtonFontSize()
        {
            await TestHost.SwitchToAppThread();

            var window = await WindowHelpers.CreateInvisibleWindowAsync<TextBoxHelperTestWindow>().ConfigureAwait(false);

            window.Invoke(() =>
                {
                    var fontSize = 42d;

                    window.TestTextBox.SetValue(TextBoxHelper.ButtonFontSizeProperty, fontSize);
                    Assert.Equal(fontSize, window.TestTextBox.FindChild<Button>("PART_ClearText")?.FontSize);
                    window.TestButtonTextBox.SetValue(TextBoxHelper.ButtonFontSizeProperty, fontSize);
                    Assert.Equal(fontSize, window.TestButtonTextBox.FindChild<Button>("PART_ClearText")?.FontSize);

                    window.TestPasswordBox.SetValue(TextBoxHelper.ButtonFontSizeProperty, fontSize);
                    Assert.Equal(fontSize, window.TestPasswordBox.FindChild<Button>("PART_ClearText")?.FontSize);
                    window.TestButtonPasswordBox.SetValue(TextBoxHelper.ButtonFontSizeProperty, fontSize);
                    Assert.Equal(fontSize, window.TestButtonPasswordBox.FindChild<Button>("PART_ClearText")?.FontSize);
                    window.TestButtonRevealedPasswordBox.SetValue(TextBoxHelper.ButtonFontSizeProperty, fontSize);
                    Assert.Equal(fontSize, window.TestButtonRevealedPasswordBox.FindChild<Button>("PART_ClearText")?.FontSize);

                    window.TestComboBox.SetValue(TextBoxHelper.ButtonFontSizeProperty, fontSize);
                    var toggleButton = window.TestComboBox.FindChild<ToggleButton>("PART_DropDownToggle");
                    Assert.Equal(fontSize, toggleButton.FindChild<Button>("PART_ClearText")?.FontSize);

                    window.TestEditableComboBox.SetValue(TextBoxHelper.ButtonFontSizeProperty, fontSize);
                    var edTextBox = window.TestEditableComboBox.FindChild<TextBox>("PART_EditableTextBox");
                    Assert.Equal(fontSize, edTextBox.FindChild<Button>("PART_ClearText")?.FontSize);

                    window.TestNumericUpDown.SetValue(TextBoxHelper.ButtonFontSizeProperty, fontSize);
                    Assert.Equal(fontSize, window.TestNumericUpDown.FindChild<Button>("PART_ClearText")?.FontSize);

                    window.TestHotKeyBox.SetValue(TextBoxHelper.ButtonFontSizeProperty, fontSize);
                    Assert.Equal(fontSize, window.TestHotKeyBox.FindChild<Button>("PART_ClearText")?.FontSize);
                });
        }

        [Fact]
        [DisplayTestMethodName]
        public async Task TestAttachedPropertyButtonTemplate()
        {
            await TestHost.SwitchToAppThread();

            var window = await WindowHelpers.CreateInvisibleWindowAsync<TextBoxHelperTestWindow>().ConfigureAwait(false);

            window.Invoke(() =>
                {
                    const string resourceKey = "TestControlTemplate";
                    var controlTemplate = window.TryFindResource(resourceKey) as ControlTemplate;
                    Assert.NotNull(controlTemplate);

                    window.TestButtonTextBox.SetResourceReference(TextBoxHelper.ButtonTemplateProperty, resourceKey);
                    Assert.Equal(controlTemplate, window.TestButtonTextBox.FindChild<Button>("PART_ClearText")?.Template);

                    window.TestButtonPasswordBox.SetResourceReference(TextBoxHelper.ButtonTemplateProperty, resourceKey);
                    Assert.Equal(controlTemplate, window.TestButtonPasswordBox.FindChild<Button>("PART_ClearText")?.Template);
                    window.TestButtonRevealedPasswordBox.SetResourceReference(TextBoxHelper.ButtonTemplateProperty, resourceKey);
                    Assert.Equal(controlTemplate, window.TestButtonRevealedPasswordBox.FindChild<Button>("PART_ClearText")?.Template);
                });
        }
    }
}