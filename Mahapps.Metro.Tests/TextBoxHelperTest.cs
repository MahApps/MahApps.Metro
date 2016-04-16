using System.Threading.Tasks;
using System.Windows.Controls;
using MahApps.Metro.Controls;
using MahApps.Metro.Tests.TestHelpers;
using Xunit;


namespace MahApps.Metro.Tests
{
    using System.Windows.Media;

    public class TextBoxHelperTest : AutomationTestBase
    {
        [Fact]
        public async Task TestAttachedPropertyButtonFontFamily()
        {
            await TestHost.SwitchToAppThread();

            var window = await WindowHelpers.CreateInvisibleWindowAsync<TextBoxHelperTestWindow>().ConfigureAwait(false);

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
            Assert.Equal(fontFamily, window.TestComboBox.FindChild<Button>("PART_ClearText").FontFamily);

            window.TestNumericUpDown.SetValue(TextBoxHelper.ButtonFontFamilyProperty, fontFamily);
            Assert.Equal(fontFamily, window.TestNumericUpDown.FindChild<Button>("PART_ClearText").FontFamily);
        }

        [Fact]
        public async Task TestAttachedPropertyButtonFontSize()
        {
            await TestHost.SwitchToAppThread();

            var window = await WindowHelpers.CreateInvisibleWindowAsync<TextBoxHelperTestWindow>().ConfigureAwait(false);

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
            Assert.Equal(fontSize, window.TestComboBox.FindChild<Button>("PART_ClearText").FontSize);

            window.TestNumericUpDown.SetValue(TextBoxHelper.ButtonFontSizeProperty, fontSize);
            Assert.Equal(fontSize, window.TestNumericUpDown.FindChild<Button>("PART_ClearText").FontSize);
        }
    }
}
