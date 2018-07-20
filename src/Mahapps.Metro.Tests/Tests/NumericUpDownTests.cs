using System.Globalization;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using MahApps.Metro.Controls;
using MahApps.Metro.Tests.TestHelpers;
using Xunit;

namespace MahApps.Metro.Tests
{
    public class NumericUpDownTests : AutomationTestBase
    {
        [Fact]
        [DisplayTestMethodName]
        public async Task ShouldConvertTextInput()
        {
            await TestHost.SwitchToAppThread();
            var window = await WindowHelpers.CreateInvisibleWindowAsync<NumericUpDownWindow>().ConfigureAwait(false);
            await TestHost.SwitchToAppThread();

            var textBox = window.TheNUD.FindChild<TextBox>(string.Empty);
            Assert.NotNull(textBox);

            window.TheNUD.NumericInputMode = NumericInput.All;

            SetText(textBox, "42");
            Assert.Equal(42d, window.TheNUD.Value);

            SetText(textBox, "42.2");
            Assert.Equal(42.2d, window.TheNUD.Value);

            SetText(textBox, ".");
            Assert.Equal(0d, window.TheNUD.Value);

            SetText(textBox, ".9");
            Assert.Equal(0.9d, window.TheNUD.Value);

            SetText(textBox, ".0115");
            Assert.Equal(0.0115d, window.TheNUD.Value);
        }

        [Fact]
        [DisplayTestMethodName]
        public async Task ShouldConvertTextInputWithStringFormat()
        {
            await TestHost.SwitchToAppThread();
            var window = await WindowHelpers.CreateInvisibleWindowAsync<NumericUpDownWindow>().ConfigureAwait(false);
            await TestHost.SwitchToAppThread();

            var textBox = window.TheNUD.FindChild<TextBox>(string.Empty);
            Assert.NotNull(textBox);

            window.TheNUD.NumericInputMode = NumericInput.All;
            window.TheNUD.StringFormat = "{}{0:N2} cm";

            SetText(textBox, "42");
            Assert.Equal(42d, window.TheNUD.Value);
            Assert.Equal("42.00 cm", textBox.Text);

            SetText(textBox, "42.2");
            Assert.Equal(42.2d, window.TheNUD.Value);
            Assert.Equal("42.20 cm", textBox.Text);

            SetText(textBox, ".");
            Assert.Equal(0d, window.TheNUD.Value);
            Assert.Equal("0.00 cm", textBox.Text);

            SetText(textBox, ".9");
            Assert.Equal(0.9d, window.TheNUD.Value);
            Assert.Equal("0.90 cm", textBox.Text);

            SetText(textBox, ".0115");
            Assert.Equal(0.0115d, window.TheNUD.Value);
            Assert.Equal("0.01 cm", textBox.Text);

            SetText(textBox, ".0155");
            Assert.Equal(0.0155d, window.TheNUD.Value);
            Assert.Equal("0.02 cm", textBox.Text);

            SetText(textBox, "100.00 cm");
            Assert.Equal(100d, window.TheNUD.Value);
            Assert.Equal("100.00 cm", textBox.Text);

            SetText(textBox, "200.00cm");
            Assert.Equal(200d, window.TheNUD.Value);
            Assert.Equal("200.00 cm", textBox.Text);

            SetText(textBox, "200.00");
            Assert.Equal(200d, window.TheNUD.Value);
            Assert.Equal("200.00 cm", textBox.Text);
        }

        [Fact]
        [DisplayTestMethodName]
        public async Task ShouldConvertTextInputWithPercentStringFormat()
        {
            await TestHost.SwitchToAppThread();
            var window = await WindowHelpers.CreateInvisibleWindowAsync<NumericUpDownWindow>().ConfigureAwait(false);
            await TestHost.SwitchToAppThread();

            var textBox = window.TheNUD.FindChild<TextBox>(string.Empty);
            Assert.NotNull(textBox);

            window.TheNUD.NumericInputMode = NumericInput.All;

            window.TheNUD.Culture = CultureInfo.CreateSpecificCulture("en-EN");

            window.TheNUD.StringFormat = "{}{0:P0}";
            SetText(textBox, "100");
            Assert.Equal(100d, window.TheNUD.Value);
            Assert.Equal("100 %", textBox.Text);

            SetText(textBox, "100 %");
            Assert.Equal(100d, window.TheNUD.Value);
            Assert.Equal("100 %", textBox.Text);

            SetText(textBox, "100%");
            Assert.Equal(100d, window.TheNUD.Value);
            Assert.Equal("100 %", textBox.Text);

            window.TheNUD.StringFormat = "{}{0:P1}";
            SetText(textBox, "-0.39678");
            Assert.Equal(-0.39678d, window.TheNUD.Value);
            Assert.Equal("-0.4 %", textBox.Text);

            window.TheNUD.StringFormat = "{}{0:0%}";
            SetText(textBox, "100%");
            Assert.Equal(100d, window.TheNUD.Value);
            Assert.Equal("100%", textBox.Text);

            window.TheNUD.StringFormat = "P0";
            SetText(textBox, "50");
            Assert.Equal(50d, window.TheNUD.Value);
            Assert.Equal("50 %", textBox.Text);

            window.TheNUD.StringFormat = "P1";
            SetText(textBox, "-0.39678");
            Assert.Equal(-0.39678d, window.TheNUD.Value);
            Assert.Equal("-0.4 %", textBox.Text);

            window.TheNUD.Culture = CultureInfo.InvariantCulture;

            window.TheNUD.StringFormat = "{}{0:P0}";
            SetText(textBox, "10");
            Assert.Equal(10d, window.TheNUD.Value);
            Assert.Equal("10 %", textBox.Text);

            window.TheNUD.StringFormat = "{}{0:P1}";
            SetText(textBox, "-0.39678");
            Assert.Equal(-0.39678d, window.TheNUD.Value);
            Assert.Equal("-0.4 %", textBox.Text);

            window.TheNUD.StringFormat = "P0";
            SetText(textBox, "1");
            Assert.Equal(1d, window.TheNUD.Value);
            Assert.Equal("1 %", textBox.Text);

            window.TheNUD.StringFormat = "P1";
            SetText(textBox, "-0.39678");
            Assert.Equal(-0.39678d, window.TheNUD.Value);
            Assert.Equal("-0.4 %", textBox.Text);
        }

        [Fact]
        [DisplayTestMethodName]
        public async Task ShouldConvertDecimalTextInput()
        {
            await TestHost.SwitchToAppThread();
            var window = await WindowHelpers.CreateInvisibleWindowAsync<NumericUpDownWindow>().ConfigureAwait(false);
            await TestHost.SwitchToAppThread();

            var textBox = window.TheNUD.FindChild<TextBox>(string.Empty);
            Assert.NotNull(textBox);

            window.TheNUD.NumericInputMode = NumericInput.Decimal;

            SetText(textBox, "42");
            Assert.Equal(42d, window.TheNUD.Value);

            SetText(textBox, "42.2");
            Assert.Equal(42.2d, window.TheNUD.Value);

            SetText(textBox, ".");
            Assert.Equal(0d, window.TheNUD.Value);

            SetText(textBox, ".9");
            Assert.Equal(0.9d, window.TheNUD.Value);

            SetText(textBox, ".0115");
            Assert.Equal(0.0115d, window.TheNUD.Value);
        }

        [Fact]
        [DisplayTestMethodName]
        public async Task ShouldConvertNumericTextInput()
        {
            await TestHost.SwitchToAppThread();
            var window = await WindowHelpers.CreateInvisibleWindowAsync<NumericUpDownWindow>().ConfigureAwait(false);
            await TestHost.SwitchToAppThread();

            var textBox = window.TheNUD.FindChild<TextBox>(string.Empty);
            Assert.NotNull(textBox);

            window.TheNUD.NumericInputMode = NumericInput.Numbers;

            SetText(textBox, "42");
            Assert.Equal(42d, window.TheNUD.Value);

            SetText(textBox, "42.2");
            Assert.Equal(null, window.TheNUD.Value);

            SetText(textBox, ".");
            Assert.Equal(null, window.TheNUD.Value);

            SetText(textBox, ".9");
            Assert.Equal(null, window.TheNUD.Value);
        }

        [Fact]
        [DisplayTestMethodName]
        public async Task ShouldConvertHexadecimalTextInput()
        {
            await TestHost.SwitchToAppThread();
            var window = await WindowHelpers.CreateInvisibleWindowAsync<NumericUpDownWindow>().ConfigureAwait(false);
            await TestHost.SwitchToAppThread();

            var textBox = window.TheNUD.FindChild<TextBox>(string.Empty);
            Assert.NotNull(textBox);

            window.TheNUD.NumericInputMode = NumericInput.Numbers;
            window.TheNUD.ParsingNumberStyle = NumberStyles.HexNumber;

            SetText(textBox, "F");
            Assert.Equal(15d, window.TheNUD.Value);

            SetText(textBox, "1F");
            Assert.Equal(31d, window.TheNUD.Value);

            SetText(textBox, "37C5");
            Assert.Equal(14277d, window.TheNUD.Value);

            SetText(textBox, "ACDC");
            Assert.Equal(44252d, window.TheNUD.Value);

            SetText(textBox, "10000");
            Assert.Equal(65536d, window.TheNUD.Value);

            SetText(textBox, "AFFE");
            Assert.Equal(45054d, window.TheNUD.Value);

            SetText(textBox, "AFFE0815");
            Assert.Equal(2952661013d, window.TheNUD.Value);
        }

        private static void SetText(TextBox textBox, string text)
        {
            textBox.Clear();
            var textCompositionEventArgs = new TextCompositionEventArgs(Keyboard.PrimaryDevice, new TextComposition(InputManager.Current, textBox, text));
            textCompositionEventArgs.RoutedEvent = TextBox.PreviewTextInputEvent;
            textBox.RaiseEvent(textCompositionEventArgs);
            textCompositionEventArgs.RoutedEvent = TextBox.TextInputEvent;
            textBox.RaiseEvent(textCompositionEventArgs);
            textBox.RaiseEvent(new RoutedEventArgs(TextBox.LostFocusEvent));
        }
    }
}