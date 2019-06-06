using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using MahApps.Metro.Controls;
using MahApps.Metro.Tests.TestHelpers;
using Xunit;

namespace MahApps.Metro.Tests
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
                    var width = 42d;

                    window.TestTextBox.SetValue(TextBoxHelper.ButtonWidthProperty, width);
                    Assert.Equal(width, window.TestTextBox.FindChild<Button>("PART_ClearText").Width);
                    window.TestButtonTextBox.SetValue(TextBoxHelper.ButtonWidthProperty, width);
                    Assert.Equal(width, window.TestButtonTextBox.FindChild<Button>("PART_ClearText").Width);

                    window.TestPasswordBox.SetValue(TextBoxHelper.ButtonWidthProperty, width);
                    Assert.Equal(width, window.TestPasswordBox.FindChild<Button>("PART_ClearText").Width);
                    window.TestButtonPasswordBox.SetValue(TextBoxHelper.ButtonWidthProperty, width);
                    Assert.Equal(width, window.TestButtonPasswordBox.FindChild<Button>("PART_ClearText").Width);
                    window.TestButtonRevealedPasswordBox.SetValue(TextBoxHelper.ButtonWidthProperty, width);
                    Assert.Equal(width, window.TestButtonRevealedPasswordBox.FindChild<Button>("PART_ClearText").Width);
                    Assert.Equal(width, window.TestButtonRevealedPasswordBox.FindChild<Button>("PART_RevealButton").Width);

                    window.TestComboBox.SetValue(TextBoxHelper.ButtonWidthProperty, width);
                    var toggleButton = window.TestComboBox.FindChild<ToggleButton>("PART_DropDownToggle");
                    Assert.Equal(width, toggleButton.FindChild<Button>("PART_ClearText").Width);
                    Assert.Equal(width, toggleButton.FindChild<Grid>("BtnArrowBackground").Width);

                    window.TestEditableComboBox.SetValue(TextBoxHelper.ButtonWidthProperty, width);
                    var edTextBox = window.TestEditableComboBox.FindChild<TextBox>("PART_EditableTextBox");
                    Assert.Equal(width, edTextBox.FindChild<Button>("PART_ClearText").Width);

                    window.TestNumericUpDown.SetValue(TextBoxHelper.ButtonWidthProperty, width);
                    Assert.Equal(width, window.TestNumericUpDown.FindChild<Button>("PART_ClearText").Width);

                    window.TestHotKeyBox.SetValue(TextBoxHelper.ButtonWidthProperty, width);
                    Assert.Equal(width, window.TestHotKeyBox.FindChild<Button>("PART_ClearText").Width);
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
                    Assert.Equal(content, window.TestTextBox.FindChild<Button>("PART_ClearText").Content);
                    window.TestButtonTextBox.SetValue(TextBoxHelper.ButtonContentProperty, content);
                    Assert.Equal(content, window.TestButtonTextBox.FindChild<Button>("PART_ClearText").Content);

                    window.TestPasswordBox.SetValue(TextBoxHelper.ButtonContentProperty, content);
                    Assert.Equal(content, window.TestPasswordBox.FindChild<Button>("PART_ClearText").Content);
                    window.TestButtonPasswordBox.SetValue(TextBoxHelper.ButtonContentProperty, content);
                    Assert.Equal(content, window.TestButtonPasswordBox.FindChild<Button>("PART_ClearText").Content);
                    window.TestButtonRevealedPasswordBox.SetValue(TextBoxHelper.ButtonContentProperty, content);
                    Assert.Equal(content, window.TestButtonRevealedPasswordBox.FindChild<Button>("PART_ClearText").Content);

                    window.TestComboBox.SetValue(TextBoxHelper.ButtonContentProperty, content);
                    var toggleButton = window.TestComboBox.FindChild<ToggleButton>("PART_DropDownToggle");
                    Assert.Equal(content, toggleButton.FindChild<Button>("PART_ClearText").Content);

                    window.TestEditableComboBox.SetValue(TextBoxHelper.ButtonContentProperty, content);
                    var edTextBox = window.TestEditableComboBox.FindChild<TextBox>("PART_EditableTextBox");
                    Assert.Equal(content, edTextBox.FindChild<Button>("PART_ClearText").Content);

                    window.TestNumericUpDown.SetValue(TextBoxHelper.ButtonContentProperty, content);
                    Assert.Equal(content, window.TestNumericUpDown.FindChild<Button>("PART_ClearText").Content);

                    window.TestHotKeyBox.SetValue(TextBoxHelper.ButtonContentProperty, content);
                    Assert.Equal(content, window.TestHotKeyBox.FindChild<Button>("PART_ClearText").Content);
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
                    var contentTemplate = new DataTemplate();

                    window.TestTextBox.SetValue(TextBoxHelper.ButtonContentTemplateProperty, contentTemplate);
                    Assert.Equal(contentTemplate, window.TestTextBox.FindChild<Button>("PART_ClearText").ContentTemplate);
                    window.TestButtonTextBox.SetValue(TextBoxHelper.ButtonContentTemplateProperty, contentTemplate);
                    Assert.Equal(contentTemplate, window.TestButtonTextBox.FindChild<Button>("PART_ClearText").ContentTemplate);

                    window.TestPasswordBox.SetValue(TextBoxHelper.ButtonContentTemplateProperty, contentTemplate);
                    Assert.Equal(contentTemplate, window.TestPasswordBox.FindChild<Button>("PART_ClearText").ContentTemplate);
                    window.TestButtonPasswordBox.SetValue(TextBoxHelper.ButtonContentTemplateProperty, contentTemplate);
                    Assert.Equal(contentTemplate, window.TestButtonPasswordBox.FindChild<Button>("PART_ClearText").ContentTemplate);
                    window.TestButtonRevealedPasswordBox.SetValue(TextBoxHelper.ButtonContentTemplateProperty, contentTemplate);
                    Assert.Equal(contentTemplate, window.TestButtonRevealedPasswordBox.FindChild<Button>("PART_ClearText").ContentTemplate);

                    window.TestComboBox.SetValue(TextBoxHelper.ButtonContentTemplateProperty, contentTemplate);
                    var toggleButton = window.TestComboBox.FindChild<ToggleButton>("PART_DropDownToggle");
                    Assert.Equal(contentTemplate, toggleButton.FindChild<Button>("PART_ClearText").ContentTemplate);

                    window.TestEditableComboBox.SetValue(TextBoxHelper.ButtonContentTemplateProperty, contentTemplate);
                    var edTextBox = window.TestEditableComboBox.FindChild<TextBox>("PART_EditableTextBox");
                    Assert.Equal(contentTemplate, edTextBox.FindChild<Button>("PART_ClearText").ContentTemplate);

                    window.TestNumericUpDown.SetValue(TextBoxHelper.ButtonContentTemplateProperty, contentTemplate);
                    Assert.Equal(contentTemplate, window.TestNumericUpDown.FindChild<Button>("PART_ClearText").ContentTemplate);

                    window.TestHotKeyBox.SetValue(TextBoxHelper.ButtonContentTemplateProperty, contentTemplate);
                    Assert.Equal(contentTemplate, window.TestHotKeyBox.FindChild<Button>("PART_ClearText").ContentTemplate);
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
                    Assert.Equal(fontFamily, window.TestTextBox.FindChild<Button>("PART_ClearText").FontFamily);
                    window.TestButtonTextBox.SetValue(TextBoxHelper.ButtonFontFamilyProperty, fontFamily);
                    Assert.Equal(fontFamily, window.TestButtonTextBox.FindChild<Button>("PART_ClearText").FontFamily);

                    window.TestPasswordBox.SetValue(TextBoxHelper.ButtonFontFamilyProperty, fontFamily);
                    Assert.Equal(fontFamily, window.TestPasswordBox.FindChild<Button>("PART_ClearText").FontFamily);
                    window.TestButtonPasswordBox.SetValue(TextBoxHelper.ButtonFontFamilyProperty, fontFamily);
                    Assert.Equal(fontFamily, window.TestButtonPasswordBox.FindChild<Button>("PART_ClearText").FontFamily);
                    window.TestButtonRevealedPasswordBox.SetValue(TextBoxHelper.ButtonFontFamilyProperty, fontFamily);
                    Assert.Equal(fontFamily, window.TestButtonRevealedPasswordBox.FindChild<Button>("PART_ClearText").FontFamily);

                    window.TestComboBox.SetValue(TextBoxHelper.ButtonFontFamilyProperty, fontFamily);
                    var toggleButton = window.TestComboBox.FindChild<ToggleButton>("PART_DropDownToggle");
                    Assert.Equal(fontFamily, toggleButton.FindChild<Button>("PART_ClearText").FontFamily);

                    window.TestEditableComboBox.SetValue(TextBoxHelper.ButtonFontFamilyProperty, fontFamily);
                    var edTextBox = window.TestEditableComboBox.FindChild<TextBox>("PART_EditableTextBox");
                    Assert.Equal(fontFamily, edTextBox.FindChild<Button>("PART_ClearText").FontFamily);

                    window.TestNumericUpDown.SetValue(TextBoxHelper.ButtonFontFamilyProperty, fontFamily);
                    Assert.Equal(fontFamily, window.TestNumericUpDown.FindChild<Button>("PART_ClearText").FontFamily);

                    window.TestHotKeyBox.SetValue(TextBoxHelper.ButtonFontFamilyProperty, fontFamily);
                    Assert.Equal(fontFamily, window.TestHotKeyBox.FindChild<Button>("PART_ClearText").FontFamily);
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
                    Assert.Equal(fontSize, window.TestTextBox.FindChild<Button>("PART_ClearText").FontSize);
                    window.TestButtonTextBox.SetValue(TextBoxHelper.ButtonFontSizeProperty, fontSize);
                    Assert.Equal(fontSize, window.TestButtonTextBox.FindChild<Button>("PART_ClearText").FontSize);

                    window.TestPasswordBox.SetValue(TextBoxHelper.ButtonFontSizeProperty, fontSize);
                    Assert.Equal(fontSize, window.TestPasswordBox.FindChild<Button>("PART_ClearText").FontSize);
                    window.TestButtonPasswordBox.SetValue(TextBoxHelper.ButtonFontSizeProperty, fontSize);
                    Assert.Equal(fontSize, window.TestButtonPasswordBox.FindChild<Button>("PART_ClearText").FontSize);
                    window.TestButtonRevealedPasswordBox.SetValue(TextBoxHelper.ButtonFontSizeProperty, fontSize);
                    Assert.Equal(fontSize, window.TestButtonRevealedPasswordBox.FindChild<Button>("PART_ClearText").FontSize);

                    window.TestComboBox.SetValue(TextBoxHelper.ButtonFontSizeProperty, fontSize);
                    var toggleButton = window.TestComboBox.FindChild<ToggleButton>("PART_DropDownToggle");
                    Assert.Equal(fontSize, toggleButton.FindChild<Button>("PART_ClearText").FontSize);

                    window.TestEditableComboBox.SetValue(TextBoxHelper.ButtonFontSizeProperty, fontSize);
                    var edTextBox = window.TestEditableComboBox.FindChild<TextBox>("PART_EditableTextBox");
                    Assert.Equal(fontSize, edTextBox.FindChild<Button>("PART_ClearText").FontSize);

                    window.TestNumericUpDown.SetValue(TextBoxHelper.ButtonFontSizeProperty, fontSize);
                    Assert.Equal(fontSize, window.TestNumericUpDown.FindChild<Button>("PART_ClearText").FontSize);

                    window.TestHotKeyBox.SetValue(TextBoxHelper.ButtonFontSizeProperty, fontSize);
                    Assert.Equal(fontSize, window.TestHotKeyBox.FindChild<Button>("PART_ClearText").FontSize);
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
                    var controlTemplate = new ControlTemplate(typeof(Button));

                    window.TestButtonTextBox.SetValue(TextBoxHelper.ButtonTemplateProperty, controlTemplate);
                    Assert.Equal(controlTemplate, window.TestButtonTextBox.FindChild<Button>("PART_ClearText").Template);

                    window.TestButtonPasswordBox.SetValue(TextBoxHelper.ButtonTemplateProperty, controlTemplate);
                    Assert.Equal(controlTemplate, window.TestButtonPasswordBox.FindChild<Button>("PART_ClearText").Template);
                    window.TestButtonRevealedPasswordBox.SetValue(TextBoxHelper.ButtonTemplateProperty, controlTemplate);
                    Assert.Equal(controlTemplate, window.TestButtonRevealedPasswordBox.FindChild<Button>("PART_ClearText").Template);
                });
        }
    }
}
