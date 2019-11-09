using System;
using System.Globalization;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using MahApps.Metro.Controls;
using MahApps.Metro.Tests.TestHelpers;
using Xunit;

namespace MahApps.Metro.Tests
{
    public class NumericUpDownTests : AutomationTestBase, IAsyncLifetime
    {
        private NumericUpDownWindow window;
        private TextBox textBox;
        private RepeatButton numUp;
        private RepeatButton numDown;

        /// <summary>
        /// Called immediately after the class has been created, before it is used.
        /// </summary>
        public async Task InitializeAsync()
        {
            await TestHost.SwitchToAppThread();
            this.window = await WindowHelpers.CreateInvisibleWindowAsync<NumericUpDownWindow>().ConfigureAwait(false);

            this.textBox = this.window.TheNUD.FindChild<TextBox>();
            this.numUp = this.window.TheNUD.FindChild<RepeatButton>("PART_NumericUp");
            this.numDown = this.window.TheNUD.FindChild<RepeatButton>("PART_NumericDown");
        }

        /// <summary>
        /// Called when an object is no longer needed. Called just before <see cref="M:System.IDisposable.Dispose" />
        /// if the class also implements that.
        /// </summary>
        public Task DisposeAsync()
        {
            return Task.CompletedTask;
        }

        public static bool NearlyEqual(double a, double b, double epsilon)
        {
            double absA = Math.Abs(a);
            double absB = Math.Abs(b);
            double diff = Math.Abs(a - b);

            if (a == b)
            {
                // shortcut, handles infinities
                return true;
            }
            else if (a == 0 || b == 0 || diff < Double.Epsilon)
            {
                // a or b is zero or both are extremely close to it
                // relative error is less meaningful here
                return diff < epsilon;
            }
            else
            {
                // use relative error
                return diff / (absA + absB) < epsilon;
            }
        }

        [Fact]
        [DisplayTestMethodName]
        public async Task ShouldSnapToMultipleOfInterval()
        {
            await TestHost.SwitchToAppThread();

            this.window.TheNUD.Interval = 0.1;
            this.window.TheNUD.SnapToMultipleOfInterval = true;

            this.window.TheNUD.Value = 0;
            for (int i = 1; i < 15; i++)
            {
                this.numUp.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
                Assert.Equal(0d + 0.1 * i, this.window.TheNUD.Value);
            }

            this.window.TheNUD.Value = 0;
            for (int i = 1; i < 15; i++)
            {
                this.numDown.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
                Assert.Equal(0d - 0.1 * i, this.window.TheNUD.Value);
            }
        }

        [Theory]
        [InlineData(42d, "", "42")]
        [InlineData(null, "", "")]
        [InlineData(0.25d, "{}{0:0.00%}", "25.00%")] // 3376 Case 1
        [InlineData(0.25d, "{0:0.00%}", "25.00%")] // 3376 Case 1
        [InlineData(0.25d, "0.00%", "25.00%")] // 3376 Case 1
        [InlineData(0.25d, "{}{0:0.00‰}", "250.00‰")] // 3376 Case 2
        [InlineData(0.25d, "{0:0.00‰}", "250.00‰")] // 3376 Case 2
        [InlineData(0.25d, "0.00‰", "250.00‰")] // 3376 Case 2
        [InlineData(0.25d, "{}{0:0.0000}%", "0.2500%")] // 3376 Case 3
        [InlineData(0.25d, "{0:0.0000}%", "0.2500%")] // 3376 Case 3
        [InlineData(0.25d, "{}{0:0.00000}‰", "0.25000‰")] // 3376 Case 4
        [InlineData(0.25d, "{0:0.00000}‰", "0.25000‰")] // 3376 Case 4
        [InlineData(0.25d, "{}{0:P}", "25.00%")] // 3376 Case 5
        [InlineData(0.25d, "{0:P}", "25.00%")] // 3376 Case 5
        [InlineData(0.25d, "P", "25.00%")] // 3376 Case 5
        [DisplayTestMethodName]
        public async Task ShouldFormatValueInput(object value, string format, string expectedText)
        {
            await TestHost.SwitchToAppThread();

            this.window.TheNUD.NumericInputMode = NumericInput.All;
            this.window.TheNUD.StringFormat = format;

            this.window.TheNUD.SetCurrentValue(NumericUpDown.ValueProperty, value);

            Assert.Equal(expectedText, this.textBox.Text);
            Assert.Equal(value, this.window.TheNUD.Value);
        }

        [Theory]
        [InlineData("42", NumericInput.All, 42d)]
        [InlineData("42.", NumericInput.All, 42d)]
        [InlineData("42.2", NumericInput.All, 42.2d)]
        [InlineData(".", NumericInput.All, 0d)]
        [InlineData(".9", NumericInput.All, 0.9d)]
        [InlineData(".0115", NumericInput.All, 0.0115d)]
        [InlineData("", NumericInput.All, null)]
        [InlineData("42", NumericInput.Decimal, 42d)]
        [InlineData("42.", NumericInput.Decimal, 42d)]
        [InlineData("42.2", NumericInput.Decimal, 42.2d)]
        [InlineData(".", NumericInput.Decimal, 0d)]
        [InlineData(".9", NumericInput.Decimal, 0.9d)]
        [InlineData(".0115", NumericInput.Decimal, 0.0115d)]
        [InlineData("", NumericInput.Decimal, null)]
        [InlineData("42", NumericInput.Numbers, 42d)]
        [InlineData("42.", NumericInput.Numbers, null)]
        [InlineData("42.2", NumericInput.Numbers, null)]
        [InlineData(".", NumericInput.Numbers, null)]
        [InlineData(".9", NumericInput.Numbers, null)]
        [InlineData("", NumericInput.Numbers, null)]
        [DisplayTestMethodName]
        public async Task ShouldConvertTextInput(string text, NumericInput numericInput, object expectedValue)
        {
            await TestHost.SwitchToAppThread();

            this.window.TheNUD.NumericInputMode = numericInput;

            SetText(this.textBox, text);

            Assert.Equal(expectedValue, this.window.TheNUD.Value);
        }

        [Theory]
        [InlineData("42", "{}{0:N2} cm", 42d, "42.00 cm")]
        [InlineData("42.", "{}{0:N2} cm", 42d, "42.00 cm")]
        [InlineData("42.2", "{}{0:N2} cm", 42.2d, "42.20 cm")]
        [InlineData(".", "{}{0:N2} cm", 0d, "0.00 cm")]
        [InlineData(".9", "{}{0:N2} cm", 0.9d, "0.90 cm")]
        [InlineData(".0115", "{}{0:N2} cm", 0.0115d, "0.01 cm")]
        [InlineData(".0155", "{}{0:N2} cm", 0.0155d, "0.02 cm")]
        [InlineData("100.00 cm", "{}{0:N2} cm", 100d, "100.00 cm")]
        [InlineData("200.00cm", "{}{0:N2} cm", 200d, "200.00 cm")]
        [InlineData("200.20", "{}{0:N2} cm", 200.2d, "200.20 cm")]
        [InlineData("15", "{}{0}mmHg", 15d, "15mmHg")] // GH-3551
        [InlineData("0.986", "{}{0:G3} mPa·s", 0.986d, "0.986 mPa·s")] // GH-3376#issuecomment-472324787
        [InlineData("", "{}{0:N2} cm", null, "")]
        [DisplayTestMethodName]
        public async Task ShouldConvertTextInputWithStringFormat(string text, string format, object expectedValue, string expectedText)
        {
            await TestHost.SwitchToAppThread();

            this.window.TheNUD.NumericInputMode = NumericInput.All;
            this.window.TheNUD.StringFormat = format;

            SetText(this.textBox, text);

            Assert.Equal(expectedValue, this.window.TheNUD.Value);
        }

        [Theory]
        [InlineData("100", "{}{0:P0}", "en-EN", 1d, "100%")]
        [InlineData("100 %", "{}{0:P0}", "en-EN", 1d, "100%")]
        [InlineData("100%", "{}{0:P0}", "en-EN", 1d, "100%")]
        [InlineData("-0.39678", "{}{0:P1}", "en-EN", -0.0039678d, "-0.4%", true)]
        [InlineData("50", "P0", "en-EN", 0.5d, "50%")]
        [InlineData("50", "P1", "en-EN", 0.5d, "50.0%")]
        [InlineData("-0.39678", "P1", "en-EN", -0.0039678d, "-0.4%", true)]
        [InlineData("10", "{}{0:P0}", null, 0.1d, "10 %")]
        [InlineData("-0.39678", "{}{0:P1}", null, -0.0039678d, "-0.4 %", true)]
        [InlineData("1", "P0", null, 0.01d, "1 %")]
        [InlineData("-0.39678", "P1", null, -0.0039678d, "-0.4 %", true)]
        [InlineData("1", "{}{0:0.0%}", null, 0.01d, "1.0%")]
        [InlineData("1", "0.0%", null, 0.01d, "1.0%")]
        [InlineData("0.25", "{0:0.0000}%", null, 0.25d, "0.2500%")] // GH-3376 Case 3
        [InlineData("100", "{}{0}%", null, 100d, "100%")]
        [InlineData("100%", "{}{0}%", null, 100d, "100%")]
        [InlineData("100 %", "{}{0}%", null, 100d, "100%")]
        [DisplayTestMethodName]
        public async Task ShouldConvertTextInputWithPercentageStringFormat(string text, string format, string culture, object expectedValue, string expectedText, bool useEpsilon = false)
        {
            await TestHost.SwitchToAppThread();

            this.window.TheNUD.NumericInputMode = NumericInput.All;
            this.window.TheNUD.Culture = string.IsNullOrEmpty(culture) ? CultureInfo.InvariantCulture : CultureInfo.GetCultureInfo(culture);
            this.window.TheNUD.StringFormat = format;

            SetText(this.textBox, text);

            if (useEpsilon)
            {
                Assert.True(NearlyEqual((double)expectedValue, this.window.TheNUD.Value.Value, 0.000005), $"The input '{text}' should be '{expectedValue} ({expectedText})', but value is '{this.window.TheNUD.Value.Value}'");
            }
            else
            {
                Assert.Equal(expectedValue, this.window.TheNUD.Value);
            }
        }

        [Theory]
        [InlineData("1", "{}{0:0.0‰}", null, 0.001d, "1.0%")]
        [InlineData("1‰", "{}{0:0.0‰}", null, 0.001d, "1.0%")]
        [InlineData("1 ‰", "{}{0:0.0‰}", null, 0.001d, "1.0%")]
        [InlineData("1", "{0:0.0‰}", null, 0.001d, "1.0%")]
        [InlineData("1‰", "{0:0.0‰}", null, 0.001d, "1.0%")]
        [InlineData("1 ‰", "{0:0.0‰}", null, 0.001d, "1.0%")]
        [InlineData("1", "0.0‰", null, 0.001d, "1.0%")]
        [InlineData("1‰", "0.0‰", null, 0.001d, "1.0%")]
        [InlineData("1 ‰", "0.0‰", null, 0.001d, "1.0%")]
        [InlineData("1", "{}{0:0.0‰}", "en-EN", 0.001d, "1.0%")]
        [InlineData("1‰", "{}{0:0.0‰}", "en-EN", 0.001d, "1.0%")]
        [InlineData("1 ‰", "{}{0:0.0‰}", "en-EN", 0.001d, "1.0%")]
        [InlineData("1", "{0:0.0‰}", "en-EN", 0.001d, "1.0%")]
        [InlineData("1‰", "{0:0.0‰}", "en-EN", 0.001d, "1.0%")]
        [InlineData("1 ‰", "{0:0.0‰}", "en-EN", 0.001d, "1.0%")]
        [InlineData("1", "0.0‰", "en-EN", 0.001d, "1.0%")]
        [InlineData("1‰", "0.0‰", "en-EN", 0.001d, "1.0%")]
        [InlineData("1 ‰", "0.0‰", "en-EN", 0.001d, "1.0%")]
        [InlineData("0.25", "{0:0.0000}‰", null, 0.25d, "0.2500‰")]
        [DisplayTestMethodName]
        public async Task ShouldConvertTextInputWithPermilleStringFormat(string text, string format, string culture, object expectedValue, string expectedText, bool useEpsilon = false)
        {
            await TestHost.SwitchToAppThread();

            this.window.TheNUD.NumericInputMode = NumericInput.All;
            this.window.TheNUD.Culture = string.IsNullOrEmpty(culture) ? CultureInfo.InvariantCulture : CultureInfo.GetCultureInfo(culture);
            this.window.TheNUD.StringFormat = format;

            SetText(this.textBox, text);

            if (useEpsilon)
            {
                Assert.True(NearlyEqual((double)expectedValue, this.window.TheNUD.Value.Value, 0.000005), $"The input '{text}' should be '{expectedValue} ({expectedText})', but value is '{this.window.TheNUD.Value.Value}'");
            }
            else
            {
                Assert.Equal(expectedValue, this.window.TheNUD.Value);
            }
        }

        [Theory]
        [InlineData("42", 42d)]
        [InlineData("42/751", 42.751d)]
        [InlineData("/", 0d)]
        [InlineData("/9", 0.9d)]
        [InlineData("/0115", 0.0115d)]
        [DisplayTestMethodName]
        public async Task ShouldConvertDecimalTextInputWithSpecialCulture(string text, object expectedValue)
        {
            await TestHost.SwitchToAppThread();

            this.window.TheNUD.NumericInputMode = NumericInput.Decimal;
            this.window.TheNUD.Culture = CultureInfo.GetCultureInfo("fa-IR");

            SetText(this.textBox, text);

            Assert.Equal(expectedValue, this.window.TheNUD.Value);
        }

        [Theory]
        [InlineData("42", 66d)]
        [InlineData("F", 15d)]
        [InlineData("1F", 31d)]
        [InlineData("37C5", 14277d)]
        [InlineData("ACDC", 44252d)]
        [InlineData("10000", 65536d)]
        [InlineData("AFFE", 45054d)]
        [InlineData("AFFE0815", 2952661013d)]
        [DisplayTestMethodName]
        public async Task ShouldConvertHexadecimalTextInput(string text, object expectedValue)
        {
            await TestHost.SwitchToAppThread();

            this.window.TheNUD.NumericInputMode = NumericInput.Numbers;
            this.window.TheNUD.ParsingNumberStyle = NumberStyles.HexNumber;

            SetText(this.textBox, text);

            Assert.Equal(expectedValue, this.window.TheNUD.Value);
        }

        private static void SetText(TextBox theTextBox, string theText)
        {
            theTextBox.Clear();
            var textCompositionEventArgs = new TextCompositionEventArgs(Keyboard.PrimaryDevice, new TextComposition(InputManager.Current, theTextBox, theText));
            textCompositionEventArgs.RoutedEvent = UIElement.PreviewTextInputEvent;
            theTextBox.RaiseEvent(textCompositionEventArgs);
            textCompositionEventArgs.RoutedEvent = UIElement.TextInputEvent;
            theTextBox.RaiseEvent(textCompositionEventArgs);
            theTextBox.RaiseEvent(new RoutedEventArgs(UIElement.LostFocusEvent));
        }
    }
}