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
using NUnit.Framework;

namespace MahApps.Metro.Tests.Tests
{
    [TestFixture]
    public class TextBoxHelperTests
    {
        [Test]
        public async Task TestAttachedPropertyButtonWidth()
        {
            var window = await WindowHelpers.CreateInvisibleWindowAsync<TextBoxHelperTestWindow>().ConfigureAwait(false);

            const double width = 42d;

            window.TestTextBox.SetValue(TextBoxHelper.ButtonWidthProperty, width);
            Assert.That(window.TestTextBox.FindChild<Button>("PART_ClearText")?.Width, Is.EqualTo(width));
            window.TestButtonTextBox.SetValue(TextBoxHelper.ButtonWidthProperty, width);
            Assert.That(window.TestButtonTextBox.FindChild<Button>("PART_ClearText")?.Width, Is.EqualTo(width));

            window.TestPasswordBox.SetValue(TextBoxHelper.ButtonWidthProperty, width);
            Assert.That(window.TestPasswordBox.FindChild<Button>("PART_ClearText")?.Width, Is.EqualTo(width));
            window.TestButtonRevealedPasswordBox.SetValue(TextBoxHelper.ButtonWidthProperty, width);
            Assert.That(window.TestButtonRevealedPasswordBox.FindChild<Button>("PART_ClearText")?.Width, Is.EqualTo(width));
            Assert.That(window.TestButtonRevealedPasswordBox.FindChild<Button>("PART_RevealButton")?.Width, Is.EqualTo(width));

            window.TestComboBox.SetValue(TextBoxHelper.ButtonWidthProperty, width);
            var toggleButton = window.TestComboBox.FindChild<ToggleButton>("PART_DropDownToggle");
            Assert.That(toggleButton?.FindChild<Button>("PART_ClearText")?.Width, Is.EqualTo(width));
            Assert.That(toggleButton?.FindChild<Grid>("BtnArrowBackground")?.Width, Is.EqualTo(width));

            window.TestEditableComboBox.SetValue(TextBoxHelper.ButtonWidthProperty, width);
            var toggleButtonEditable = window.TestEditableComboBox.FindChild<ToggleButton>("PART_DropDownToggle");
            Assert.That(toggleButtonEditable?.FindChild<Button>("PART_ClearText")?.Width, Is.EqualTo(width));
            Assert.That(toggleButtonEditable?.FindChild<Grid>("BtnArrowBackground")?.Width, Is.EqualTo(width));

            window.TestNumericUpDown.SetValue(TextBoxHelper.ButtonWidthProperty, width);
            Assert.That(window.TestNumericUpDown.FindChild<Button>("PART_ClearText")?.Width, Is.EqualTo(width));

            window.TestHotKeyBox.SetValue(TextBoxHelper.ButtonWidthProperty, width);
            Assert.That(window.TestHotKeyBox.FindChild<Button>("PART_ClearText")?.Width, Is.EqualTo(width));

            window.Close();
        }

        [Test]
        public async Task TestAttachedPropertyButtonContent()
        {
            var window = await WindowHelpers.CreateInvisibleWindowAsync<TextBoxHelperTestWindow>().ConfigureAwait(false);

            var content = "M237.5 75A12.5 12.5 0 0 0 237.5 100A12.5 12.5 0 0 1 250 112.5V212.5A12.5 12.5 0 0 1 237.5 225H142.6875L136.8 241.675A12.5125 12.5125 0 0 1 125 250H62.5A12.5 12.5 0 0 1 50 237.5V112.5A12.5 12.5 0 0 1 62.5 100A12.5 12.5 0 0 0 62.5 75A37.5 37.5 0 0 0 25 112.5V237.5A37.5 37.5 0 0 0 62.5 275H125C141.325 275 155.2125 264.5625 160.375 250H237.5A37.5 37.5 0 0 0 275 212.5V112.5A37.5 37.5 0 0 0 237.5 75zM174.875 76.2A62.525 62.525 0 0 0 96.2125 172.5375A62.5 62.5 0 0 0 192.55 93.875L228.8 57.625A12.5 12.5 0 0 0 211.1125 39.9625L174.8625 76.2000000000001zM166.925 101.825A37.5 37.5 0 1 1 113.875 154.875A37.5 37.5 0 0 1 166.9125 101.8375z";

            window.TestButtonTextBox.SetValue(TextBoxHelper.ButtonContentProperty, content);
            Assert.That(window.TestButtonTextBox.FindChild<Button>("PART_ClearText")?.Content, Is.EqualTo(content));

            content = "42";

            window.TestTextBox.SetValue(TextBoxHelper.ButtonContentProperty, content);
            Assert.That(window.TestTextBox.FindChild<Button>("PART_ClearText")?.Content, Is.EqualTo(content));

            window.TestPasswordBox.SetValue(TextBoxHelper.ButtonContentProperty, content);
            Assert.That(window.TestPasswordBox.FindChild<Button>("PART_ClearText")?.Content, Is.EqualTo(content));
            window.TestButtonRevealedPasswordBox.SetValue(TextBoxHelper.ButtonContentProperty, content);
            Assert.That(window.TestButtonRevealedPasswordBox.FindChild<Button>("PART_ClearText")?.Content, Is.EqualTo(content));

            window.TestComboBox.SetValue(TextBoxHelper.ButtonContentProperty, content);
            var toggleButton = window.TestComboBox.FindChild<ToggleButton>("PART_DropDownToggle");
            Assert.That(toggleButton.FindChild<Button>("PART_ClearText")?.Content, Is.EqualTo(content));

            window.TestEditableComboBox.SetValue(TextBoxHelper.ButtonContentProperty, content);
            var toggleButtonEditable = window.TestEditableComboBox.FindChild<ToggleButton>("PART_DropDownToggle");
            Assert.That(toggleButtonEditable.FindChild<Button>("PART_ClearText")?.Content, Is.EqualTo(content));

            window.TestNumericUpDown.SetValue(TextBoxHelper.ButtonContentProperty, content);
            Assert.That(window.TestNumericUpDown.FindChild<Button>("PART_ClearText")?.Content, Is.EqualTo(content));

            window.TestHotKeyBox.SetValue(TextBoxHelper.ButtonContentProperty, content);
            Assert.That(window.TestHotKeyBox.FindChild<Button>("PART_ClearText")?.Content, Is.EqualTo(content));

            window.Close();
        }

        [Test]
        public async Task TestAttachedPropertyButtonContentTemplate()
        {
            var window = await WindowHelpers.CreateInvisibleWindowAsync<TextBoxHelperTestWindow>().ConfigureAwait(false);

            const string resourceKey = "TestDataTemplate";
            var dataTemplate = window.TryFindResource(resourceKey) as DataTemplate;
            Assert.That(dataTemplate, Is.Not.Null);

            window.TestTextBox.SetResourceReference(TextBoxHelper.ButtonContentTemplateProperty, resourceKey);
            Assert.That(window.TestTextBox.FindChild<Button>("PART_ClearText")?.ContentTemplate, Is.EqualTo(dataTemplate));
            window.TestButtonTextBox.SetResourceReference(TextBoxHelper.ButtonContentTemplateProperty, resourceKey);
            Assert.That(window.TestButtonTextBox.FindChild<Button>("PART_ClearText")?.ContentTemplate, Is.EqualTo(dataTemplate));

            window.TestPasswordBox.SetResourceReference(TextBoxHelper.ButtonContentTemplateProperty, resourceKey);
            Assert.That(window.TestPasswordBox.FindChild<Button>("PART_ClearText")?.ContentTemplate, Is.EqualTo(dataTemplate));
            window.TestButtonRevealedPasswordBox.SetResourceReference(TextBoxHelper.ButtonContentTemplateProperty, resourceKey);
            Assert.That(window.TestButtonRevealedPasswordBox.FindChild<Button>("PART_ClearText")?.ContentTemplate, Is.EqualTo(dataTemplate));

            window.TestComboBox.SetResourceReference(TextBoxHelper.ButtonContentTemplateProperty, resourceKey);
            var toggleButton = window.TestComboBox.FindChild<ToggleButton>("PART_DropDownToggle");
            Assert.That(toggleButton.FindChild<Button>("PART_ClearText")?.ContentTemplate, Is.EqualTo(dataTemplate));

            window.TestEditableComboBox.SetResourceReference(TextBoxHelper.ButtonContentTemplateProperty, resourceKey);
            var toggleButtonEditable = window.TestEditableComboBox.FindChild<ToggleButton>("PART_DropDownToggle");
            Assert.That(toggleButtonEditable.FindChild<Button>("PART_ClearText")?.ContentTemplate, Is.EqualTo(dataTemplate));

            window.TestNumericUpDown.SetResourceReference(TextBoxHelper.ButtonContentTemplateProperty, resourceKey);
            Assert.That(window.TestNumericUpDown.FindChild<Button>("PART_ClearText")?.ContentTemplate, Is.EqualTo(dataTemplate));

            window.TestHotKeyBox.SetResourceReference(TextBoxHelper.ButtonContentTemplateProperty, resourceKey);
            Assert.That(window.TestHotKeyBox.FindChild<Button>("PART_ClearText")?.ContentTemplate, Is.EqualTo(dataTemplate));

            window.Close();
        }

        [Test]
        public async Task TestAttachedPropertyButtonFontFamily()
        {
            var window = await WindowHelpers.CreateInvisibleWindowAsync<TextBoxHelperTestWindow>().ConfigureAwait(false);

            var fontFamily = new FontFamilyConverter().ConvertFromString("Arial");

            window.TestTextBox.SetValue(TextBoxHelper.ButtonFontFamilyProperty, fontFamily);
            Assert.That(window.TestTextBox.FindChild<Button>("PART_ClearText")?.FontFamily, Is.EqualTo(fontFamily));
            window.TestButtonTextBox.SetValue(TextBoxHelper.ButtonFontFamilyProperty, fontFamily);
            Assert.That(window.TestButtonTextBox.FindChild<Button>("PART_ClearText")?.FontFamily, Is.EqualTo(fontFamily));

            window.TestPasswordBox.SetValue(TextBoxHelper.ButtonFontFamilyProperty, fontFamily);
            Assert.That(window.TestPasswordBox.FindChild<Button>("PART_ClearText")?.FontFamily, Is.EqualTo(fontFamily));
            window.TestButtonRevealedPasswordBox.SetValue(TextBoxHelper.ButtonFontFamilyProperty, fontFamily);
            Assert.That(window.TestButtonRevealedPasswordBox.FindChild<Button>("PART_ClearText")?.FontFamily, Is.EqualTo(fontFamily));

            window.TestComboBox.SetValue(TextBoxHelper.ButtonFontFamilyProperty, fontFamily);
            var toggleButton = window.TestComboBox.FindChild<ToggleButton>("PART_DropDownToggle");
            Assert.That(toggleButton.FindChild<Button>("PART_ClearText")?.FontFamily, Is.EqualTo(fontFamily));

            window.TestEditableComboBox.SetValue(TextBoxHelper.ButtonFontFamilyProperty, fontFamily);
            var toggleButtonEditable = window.TestEditableComboBox.FindChild<ToggleButton>("PART_DropDownToggle");
            Assert.That(toggleButtonEditable.FindChild<Button>("PART_ClearText")?.FontFamily, Is.EqualTo(fontFamily));

            window.TestNumericUpDown.SetValue(TextBoxHelper.ButtonFontFamilyProperty, fontFamily);
            Assert.That(window.TestNumericUpDown.FindChild<Button>("PART_ClearText")?.FontFamily, Is.EqualTo(fontFamily));

            window.TestHotKeyBox.SetValue(TextBoxHelper.ButtonFontFamilyProperty, fontFamily);
            Assert.That(window.TestHotKeyBox.FindChild<Button>("PART_ClearText")?.FontFamily, Is.EqualTo(fontFamily));

            window.Close();
        }

        [Test]
        public async Task TestAttachedPropertyButtonFontSize()
        {
            var window = await WindowHelpers.CreateInvisibleWindowAsync<TextBoxHelperTestWindow>().ConfigureAwait(false);

            var fontSize = 42d;

            window.TestTextBox.SetValue(TextBoxHelper.ButtonFontSizeProperty, fontSize);
            Assert.That(window.TestTextBox.FindChild<Button>("PART_ClearText")?.FontSize, Is.EqualTo(fontSize));
            window.TestButtonTextBox.SetValue(TextBoxHelper.ButtonFontSizeProperty, fontSize);
            Assert.That(window.TestButtonTextBox.FindChild<Button>("PART_ClearText")?.FontSize, Is.EqualTo(fontSize));

            window.TestPasswordBox.SetValue(TextBoxHelper.ButtonFontSizeProperty, fontSize);
            Assert.That(window.TestPasswordBox.FindChild<Button>("PART_ClearText")?.FontSize, Is.EqualTo(fontSize));
            window.TestButtonRevealedPasswordBox.SetValue(TextBoxHelper.ButtonFontSizeProperty, fontSize);
            Assert.That(window.TestButtonRevealedPasswordBox.FindChild<Button>("PART_ClearText")?.FontSize, Is.EqualTo(fontSize));

            window.TestComboBox.SetValue(TextBoxHelper.ButtonFontSizeProperty, fontSize);
            var toggleButton = window.TestComboBox.FindChild<ToggleButton>("PART_DropDownToggle");
            Assert.That(toggleButton.FindChild<Button>("PART_ClearText")?.FontSize, Is.EqualTo(fontSize));

            window.TestEditableComboBox.SetValue(TextBoxHelper.ButtonFontSizeProperty, fontSize);
            var toggleButtonEditable = window.TestEditableComboBox.FindChild<ToggleButton>("PART_DropDownToggle");
            Assert.That(toggleButtonEditable.FindChild<Button>("PART_ClearText")?.FontSize, Is.EqualTo(fontSize));

            window.TestNumericUpDown.SetValue(TextBoxHelper.ButtonFontSizeProperty, fontSize);
            Assert.That(window.TestNumericUpDown.FindChild<Button>("PART_ClearText")?.FontSize, Is.EqualTo(fontSize));

            window.TestHotKeyBox.SetValue(TextBoxHelper.ButtonFontSizeProperty, fontSize);
            Assert.That(window.TestHotKeyBox.FindChild<Button>("PART_ClearText")?.FontSize, Is.EqualTo(fontSize));

            window.Close();
        }

        [Test]
        public async Task TestAttachedPropertyButtonTemplate()
        {
            var window = await WindowHelpers.CreateInvisibleWindowAsync<TextBoxHelperTestWindow>().ConfigureAwait(false);

            const string resourceKey = "TestControlTemplate";
            var controlTemplate = window.TryFindResource(resourceKey) as ControlTemplate;
            Assert.That(controlTemplate, Is.Not.Null);

            window.TestButtonTextBox.SetResourceReference(TextBoxHelper.ButtonTemplateProperty, resourceKey);
            Assert.That(window.TestButtonTextBox.FindChild<Button>("PART_ClearText")?.Template, Is.EqualTo(controlTemplate));

            window.TestButtonRevealedPasswordBox.SetResourceReference(TextBoxHelper.ButtonTemplateProperty, resourceKey);
            Assert.That(window.TestButtonRevealedPasswordBox.FindChild<Button>("PART_ClearText")?.Template, Is.EqualTo(controlTemplate));

            window.Close();
        }
    }
}