using System.Threading.Tasks;
using MahApps.Metro.Controls;
using MahApps.Metro.Tests;
using Xunit;
using MahApps.Metro.Tests.TestHelpers;

namespace Mahapps.Metro.Tests
{
    public class AutoWatermarkTest : AutomationTestBase
    {
        [Fact]
        [DisplayTestMethodName]
        public async Task TestAutoWatermark()
        {
            await TestHost.SwitchToAppThread();

            var window = await WindowHelpers.CreateInvisibleWindowAsync<AutoWatermarkTestWindow>().ConfigureAwait(false);

            window.Invoke(() =>
                              {
                                  var autoWatermark = "AutoWatermark";

                                  Assert.Equal(autoWatermark, window.TestTextBox.GetValue(TextBoxHelper.WatermarkProperty));
                                  Assert.Equal(autoWatermark, window.TestTextBoxSubModel.GetValue(TextBoxHelper.WatermarkProperty));
                                  Assert.Equal(autoWatermark, window.TestComboBox.GetValue(TextBoxHelper.WatermarkProperty));
                                  Assert.Equal(autoWatermark, window.TestNumericUpDown.GetValue(TextBoxHelper.WatermarkProperty));
                                  Assert.Equal(autoWatermark, window.TestDatePicker.GetValue(TextBoxHelper.WatermarkProperty));
                              });
        }
    }
}