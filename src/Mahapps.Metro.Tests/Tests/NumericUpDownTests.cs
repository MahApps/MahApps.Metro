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
        public async Task ShouldConvertTextInputWithSnapToMultipleOfInterval()
        {
            await TestHost.SwitchToAppThread();

            this.window.TheNUD.Interval = 0.1;
            this.window.TheNUD.SnapToMultipleOfInterval = true;

            this.window.TheNUD.Value = 0;
            for (int i = 1; i < 15; i++)
            {
                numUp.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
                Assert.Equal(0d + 0.1 * i, this.window.TheNUD.Value);
            }

            this.window.TheNUD.Value = 0;
            for (int i = 1; i < 15; i++)
            {
                numDown.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
                Assert.Equal(0d - 0.1 * i, this.window.TheNUD.Value);
            }
        }

        [Fact]
        [DisplayTestMethodName]
        public async Task ShouldConvertTextInput()
        {
            await TestHost.SwitchToAppThread();

            this.window.TheNUD.NumericInputMode = NumericInput.All;

            SetText(textBox, "42");
            Assert.Equal(42d, this.window.TheNUD.Value);

            SetText(textBox, "42.2");
            Assert.Equal(42.2d, this.window.TheNUD.Value);

            SetText(textBox, ".");
            Assert.Equal(0d, this.window.TheNUD.Value);

            SetText(textBox, ".9");
            Assert.Equal(0.9d, this.window.TheNUD.Value);

            SetText(textBox, ".0115");
            Assert.Equal(0.0115d, this.window.TheNUD.Value);
        }

        [Fact]
        [DisplayTestMethodName]
        public async Task ShouldConvertTextInputWithStringFormat()
        {
            await TestHost.SwitchToAppThread();

            this.window.TheNUD.NumericInputMode = NumericInput.All;
            this.window.TheNUD.StringFormat = "{}{0:N2} cm";

            SetText(textBox, "42");
            Assert.Equal(42d, this.window.TheNUD.Value);
            Assert.Equal("42.00 cm", textBox.Text);

            SetText(textBox, "42.2");
            Assert.Equal(42.2d, this.window.TheNUD.Value);
            Assert.Equal("42.20 cm", textBox.Text);

            SetText(textBox, ".");
            Assert.Equal(0d, this.window.TheNUD.Value);
            Assert.Equal("0.00 cm", textBox.Text);

            SetText(textBox, ".9");
            Assert.Equal(0.9d, this.window.TheNUD.Value);
            Assert.Equal("0.90 cm", textBox.Text);

            SetText(textBox, ".0115");
            Assert.Equal(0.0115d, this.window.TheNUD.Value);
            Assert.Equal("0.01 cm", textBox.Text);

            SetText(textBox, ".0155");
            Assert.Equal(0.0155d, this.window.TheNUD.Value);
            Assert.Equal("0.02 cm", textBox.Text);

            SetText(textBox, "100.00 cm");
            Assert.Equal(100d, this.window.TheNUD.Value);
            Assert.Equal("100.00 cm", textBox.Text);

            SetText(textBox, "200.00cm");
            Assert.Equal(200d, this.window.TheNUD.Value);
            Assert.Equal("200.00 cm", textBox.Text);

            SetText(textBox, "200.00");
            Assert.Equal(200d, this.window.TheNUD.Value);
            Assert.Equal("200.00 cm", textBox.Text);

            // GH-3551
            this.window.TheNUD.StringFormat = "{}{0}mmHg";
            SetText(textBox, "15");
            Assert.Equal(15, this.window.TheNUD.Value);
            Assert.Equal("15mmHg", textBox.Text);
        }

        [Fact]
        [DisplayTestMethodName]
        public async Task ShouldConvertTextInputWithPercentStringFormat()
        {
            await TestHost.SwitchToAppThread();

            this.window.TheNUD.NumericInputMode = NumericInput.All;

            this.window.TheNUD.Culture = CultureInfo.GetCultureInfo("en-EN");

            this.window.TheNUD.StringFormat = "{}{0:P0}";
            SetText(textBox, "100");
            Assert.Equal(1d, this.window.TheNUD.Value);
            Assert.Equal("100%", textBox.Text);

            SetText(textBox, "100 %");
            Assert.Equal(1d, this.window.TheNUD.Value);
            Assert.Equal("100%", textBox.Text);

            SetText(textBox, "100%");
            Assert.Equal(1d, this.window.TheNUD.Value);
            Assert.Equal("100%", textBox.Text);

            this.window.TheNUD.StringFormat = "{}{0:P1}";
            SetText(textBox, "-0.39678");
            Assert.True(NearlyEqual(-0.0039678d, this.window.TheNUD.Value.Value, 0.000005));
            Assert.Equal("-0.4%", textBox.Text);

            this.window.TheNUD.StringFormat = "{}{0:0%}";
            SetText(textBox, "100%");
            Assert.Equal(1d, this.window.TheNUD.Value);
            Assert.Equal("100%", textBox.Text);

            this.window.TheNUD.StringFormat = "P0";
            SetText(textBox, "50");
            Assert.Equal(0.5d, this.window.TheNUD.Value);
            Assert.Equal("50%", textBox.Text);

            this.window.TheNUD.StringFormat = "P1";
            SetText(textBox, "-0.39678");
            Assert.True(NearlyEqual(-0.0039678d, this.window.TheNUD.Value.Value, 0.000005));
            Assert.Equal("-0.4%", textBox.Text);

            this.window.TheNUD.Culture = CultureInfo.InvariantCulture;

            this.window.TheNUD.StringFormat = "{}{0:P0}";
            SetText(textBox, "10");
            Assert.Equal(0.1d, this.window.TheNUD.Value);
            Assert.Equal("10 %", textBox.Text);

            this.window.TheNUD.StringFormat = "{}{0:P1}";
            SetText(textBox, "-0.39678");
            Assert.True(NearlyEqual(-0.0039678d, this.window.TheNUD.Value.Value, 0.000005));
            Assert.Equal("-0.4 %", textBox.Text);

            this.window.TheNUD.StringFormat = "P0";
            SetText(textBox, "1");
            Assert.Equal(0.01d, this.window.TheNUD.Value);
            Assert.Equal("1 %", textBox.Text);

            this.window.TheNUD.StringFormat = "P1";
            SetText(textBox, "-0.39678");
            Assert.True(NearlyEqual(-0.0039678d, this.window.TheNUD.Value.Value, 0.000005));
            Assert.Equal("-0.4 %", textBox.Text);

            this.window.TheNUD.StringFormat = "{}{0:0.0%}";
            SetText(textBox, "1");
            Assert.Equal(0.01d, this.window.TheNUD.Value);
            Assert.Equal("1.0%", textBox.Text);

            this.window.TheNUD.StringFormat = "{0:0.0%}";
            SetText(textBox, "1");
            Assert.Equal(0.01d, this.window.TheNUD.Value);
            Assert.Equal("1.0%", textBox.Text);

            this.window.TheNUD.StringFormat = "0.0%";
            SetText(textBox, "1");
            Assert.Equal(0.01d, this.window.TheNUD.Value);
            Assert.Equal("1.0%", textBox.Text);

            this.window.TheNUD.StringFormat = "{}{0:0.0‰}";
            SetText(textBox, "1");
            Assert.Equal(0.001d, this.window.TheNUD.Value);
            Assert.Equal("1.0‰", textBox.Text);

            this.window.TheNUD.StringFormat = "{0:0.0‰}";
            SetText(textBox, "1");
            Assert.Equal(0.001d, this.window.TheNUD.Value);
            Assert.Equal("1.0‰", textBox.Text);

            this.window.TheNUD.StringFormat = "0.0‰";
            SetText(textBox, "1");
            Assert.Equal(0.001d, this.window.TheNUD.Value);
            Assert.Equal("1.0‰", textBox.Text);

            // GH-3376 Case 3
            this.window.TheNUD.StringFormat = "{0:0.0000}%";
            SetText(textBox, "0.25");
            Assert.Equal(0.25d, this.window.TheNUD.Value);
            Assert.Equal("0.2500%", textBox.Text);

            // GH-3376 Case 4
            this.window.TheNUD.StringFormat = "{0:0.0000}‰";
            SetText(textBox, "0.25");
            Assert.Equal(0.25d, this.window.TheNUD.Value);
            Assert.Equal("0.2500‰", textBox.Text);

            // GH-3376#issuecomment-472324787
            this.window.TheNUD.StringFormat = "{}{0:G3} mPa·s";
            SetText(textBox, "0.986");
            Assert.Equal(0.986d, this.window.TheNUD.Value);
            Assert.Equal("0.986 mPa·s", textBox.Text);
        }

        [Fact]
        [DisplayTestMethodName]
        public async Task ShouldConvertDecimalTextInput()
        {
            await TestHost.SwitchToAppThread();

            this.window.TheNUD.NumericInputMode = NumericInput.Decimal;

            SetText(textBox, "42");
            Assert.Equal(42d, this.window.TheNUD.Value);

            SetText(textBox, "42.2");
            Assert.Equal(42.2d, this.window.TheNUD.Value);

            SetText(textBox, ".");
            Assert.Equal(0d, this.window.TheNUD.Value);

            SetText(textBox, ".9");
            Assert.Equal(0.9d, this.window.TheNUD.Value);

            SetText(textBox, ".0115");
            Assert.Equal(0.0115d, this.window.TheNUD.Value);
        }

        [Fact]
        [DisplayTestMethodName]
        public async Task ShouldConvertDecimalTextInputWithSpecialCulture()
        {
            await TestHost.SwitchToAppThread();

            this.window.TheNUD.NumericInputMode = NumericInput.Decimal;

            this.window.TheNUD.Culture = CultureInfo.GetCultureInfo("fa-IR");

            SetText(textBox, "42");
            Assert.Equal(42d, this.window.TheNUD.Value);

            SetText(textBox, "42/751");
            Assert.Equal(42.751d, this.window.TheNUD.Value);

            SetText(textBox, "/");
            Assert.Equal(0d, this.window.TheNUD.Value);

            SetText(textBox, "/9");
            Assert.Equal(0.9d, this.window.TheNUD.Value);

            SetText(textBox, "/0115");
            Assert.Equal(0.0115d, this.window.TheNUD.Value);
        }

        [Fact]
        [DisplayTestMethodName]
        public async Task ShouldConvertNumericTextInput()
        {
            await TestHost.SwitchToAppThread();

            this.window.TheNUD.NumericInputMode = NumericInput.Numbers;

            SetText(textBox, "42");
            Assert.Equal(42d, this.window.TheNUD.Value);

            SetText(textBox, "42.2");
            Assert.Null(this.window.TheNUD.Value);

            SetText(textBox, ".");
            Assert.Null(this.window.TheNUD.Value);

            SetText(textBox, ".9");
            Assert.Null(this.window.TheNUD.Value);
        }

        [Fact]
        [DisplayTestMethodName]
        public async Task ShouldConvertHexadecimalTextInput()
        {
            await TestHost.SwitchToAppThread();

            this.window.TheNUD.NumericInputMode = NumericInput.Numbers;
            this.window.TheNUD.ParsingNumberStyle = NumberStyles.HexNumber;

            SetText(textBox, "F");
            Assert.Equal(15d, this.window.TheNUD.Value);

            SetText(textBox, "1F");
            Assert.Equal(31d, this.window.TheNUD.Value);

            SetText(textBox, "37C5");
            Assert.Equal(14277d, this.window.TheNUD.Value);

            SetText(textBox, "ACDC");
            Assert.Equal(44252d, this.window.TheNUD.Value);

            SetText(textBox, "10000");
            Assert.Equal(65536d, this.window.TheNUD.Value);

            SetText(textBox, "AFFE");
            Assert.Equal(45054d, this.window.TheNUD.Value);

            SetText(textBox, "AFFE0815");
            Assert.Equal(2952661013d, this.window.TheNUD.Value);
        }

        private static void SetText(TextBox textBox, string text)
        {
            textBox.Clear();
            var textCompositionEventArgs = new TextCompositionEventArgs(Keyboard.PrimaryDevice, new TextComposition(InputManager.Current, textBox, text));
            textCompositionEventArgs.RoutedEvent = UIElement.PreviewTextInputEvent;
            textBox.RaiseEvent(textCompositionEventArgs);
            textCompositionEventArgs.RoutedEvent = UIElement.TextInputEvent;
            textBox.RaiseEvent(textCompositionEventArgs);
            textBox.RaiseEvent(new RoutedEventArgs(UIElement.LostFocusEvent));
        }
    }
}