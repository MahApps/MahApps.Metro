// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

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
    public class NumericUpDownTests : AutomationTestBase, IClassFixture<NumericUpDownFixture>
    {
        private readonly NumericUpDownFixture fixture;

        public NumericUpDownTests(NumericUpDownFixture fixture)
        {
            this.fixture = fixture;
        }

        public static bool NearlyEqual(double a, double b, double epsilon)
        {
            double absA = Math.Abs(a);
            double absB = Math.Abs(b);
            double diff = Math.Abs(a - b);

            if (a.Equals(b))
            {
                // shortcut, handles infinities
                return true;
            }
            else if (a.Equals(0) || b.Equals(0) || diff < double.Epsilon)
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
            await this.fixture.PrepareForTestAsync().ConfigureAwait(false);
            await TestHost.SwitchToAppThread();

            this.fixture.Window.TheNUD.Interval = 0.1;
            this.fixture.Window.TheNUD.SnapToMultipleOfInterval = true;

            this.fixture.Window.TheNUD.Value = 0;
            for (int i = 1; i < 15; i++)
            {
                this.fixture.NumUp.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
                Assert.Equal(0d + 0.1 * i, this.fixture.Window.TheNUD.Value);
            }

            this.fixture.Window.TheNUD.Value = 0;
            for (int i = 1; i < 15; i++)
            {
                this.fixture.NumDown.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
                Assert.Equal(0d - 0.1 * i, this.fixture.Window.TheNUD.Value);
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
        [InlineData(123456789d, "X", "75BCD15")]
        [InlineData(123456789d, "X2", "75BCD15")]
        [InlineData(255d, "X", "FF")]
        [InlineData(-1d, "x", "ffffffff")]
        [InlineData(255d, "x4", "00ff")]
        [InlineData(-1d, "X4", "FFFFFFFF")]
        [DisplayTestMethodName]
        public async Task ShouldFormatValueInput(object value, string format, string expectedText)
        {
            await this.fixture.PrepareForTestAsync().ConfigureAwait(false);
            await TestHost.SwitchToAppThread();

            this.fixture.Window.TheNUD.NumericInputMode = NumericInput.All;
            this.fixture.Window.TheNUD.StringFormat = format;

            this.fixture.Window.TheNUD.SetCurrentValue(NumericUpDown.ValueProperty, value);

            Assert.Equal(expectedText, this.fixture.TextBox.Text);
            Assert.Equal(value, this.fixture.Window.TheNUD.Value);
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
            await this.fixture.PrepareForTestAsync().ConfigureAwait(false);
            await TestHost.SwitchToAppThread();

            this.fixture.Window.TheNUD.NumericInputMode = numericInput;

            SetText(this.fixture.TextBox, text);

            Assert.Equal(expectedValue, this.fixture.Window.TheNUD.Value);
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
            await this.fixture.PrepareForTestAsync().ConfigureAwait(false);
            await TestHost.SwitchToAppThread();

            this.fixture.Window.TheNUD.NumericInputMode = NumericInput.All;
            this.fixture.Window.TheNUD.StringFormat = format;

            SetText(this.fixture.TextBox, text);

            Assert.Equal(expectedValue, this.fixture.Window.TheNUD.Value);
            Assert.Equal(expectedText, this.fixture.TextBox.Text);
        }

        [Theory]
        [InlineData("100", "{}{0:P0}", "en-EN", 1d, "100%", false)]
        [InlineData("100 %", "{}{0:P0}", "en-EN", 1d, "100%", false)]
        [InlineData("100%", "{}{0:P0}", "en-EN", 1d, "100%", false)]
        [InlineData("-0.39678", "{}{0:P1}", "en-EN", -0.0039678d, "-0.4%", true)]
        [InlineData("50", "P0", "en-EN", 0.5d, "50%", false)]
        [InlineData("50", "P1", "en-EN", 0.5d, "50.0%", false)]
        [InlineData("-0.39678", "P1", "en-EN", -0.0039678d, "-0.4%", true)]
        [InlineData("10", "{}{0:P0}", null, 0.1d, "10 %", false)]
        [InlineData("-0.39678", "{}{0:P1}", null, -0.0039678d, "-0.4 %", true)]
        [InlineData("1", "P0", null, 0.01d, "1 %", false)]
        [InlineData("-0.39678", "P1", null, -0.0039678d, "-0.4 %", true)]
        [InlineData("1", "{}{0:0.0%}", null, 0.01d, "1.0%", false)]
        [InlineData("1", "0.0%", null, 0.01d, "1.0%", false)]
        [InlineData("0.25", "{0:0.0000}%", null, 0.25d, "0.2500%", false)] // GH-3376 Case 3
        [InlineData("100", "{}{0}%", null, 100d, "100%", false)]
        [InlineData("100%", "{}{0}%", null, 100d, "100%", false)]
        [InlineData("100 %", "{}{0}%", null, 100d, "100%", false)]
        [DisplayTestMethodName]
        public async Task ShouldConvertTextInputWithPercentageStringFormat(string text, string format, string culture, object expectedValue, string expectedText, bool useEpsilon)
        {
            await this.fixture.PrepareForTestAsync().ConfigureAwait(false);
            await TestHost.SwitchToAppThread();

            this.fixture.Window.TheNUD.NumericInputMode = NumericInput.All;
            this.fixture.Window.TheNUD.Culture = string.IsNullOrEmpty(culture) ? CultureInfo.InvariantCulture : CultureInfo.GetCultureInfo(culture);
            this.fixture.Window.TheNUD.StringFormat = format;

            SetText(this.fixture.TextBox, text);

            if (useEpsilon)
            {
                Assert.True(NearlyEqual((double)expectedValue, this.fixture.Window.TheNUD.Value.Value, 0.000005), $"The input '{text}' should be '{expectedValue} ({expectedText})', but value is '{this.fixture.Window.TheNUD.Value.Value}'");
            }
            else
            {
                Assert.Equal(expectedValue, this.fixture.Window.TheNUD.Value);
            }

            Assert.Equal(expectedText, this.fixture.TextBox.Text);
        }

        [Theory]
        [InlineData("1", "{}{0:0.0‰}", null, 0.001d, "1.0‰")]
        [InlineData("1‰", "{}{0:0.0‰}", null, 0.001d, "1.0‰")]
        [InlineData("1 ‰", "{}{0:0.0‰}", null, 0.001d, "1.0‰")]
        [InlineData("1", "{0:0.0‰}", null, 0.001d, "1.0‰")]
        [InlineData("1‰", "{0:0.0‰}", null, 0.001d, "1.0‰")]
        [InlineData("1 ‰", "{0:0.0‰}", null, 0.001d, "1.0‰")]
        [InlineData("1", "0.0‰", null, 0.001d, "1.0‰")]
        [InlineData("1‰", "0.0‰", null, 0.001d, "1.0‰")]
        [InlineData("1 ‰", "0.0‰", null, 0.001d, "1.0‰")]
        [InlineData("1", "{}{0:0.0‰}", "en-EN", 0.001d, "1.0‰")]
        [InlineData("1‰", "{}{0:0.0‰}", "en-EN", 0.001d, "1.0‰")]
        [InlineData("1 ‰", "{}{0:0.0‰}", "en-EN", 0.001d, "1.0‰")]
        [InlineData("1", "{0:0.0‰}", "en-EN", 0.001d, "1.0‰")]
        [InlineData("1‰", "{0:0.0‰}", "en-EN", 0.001d, "1.0‰")]
        [InlineData("1 ‰", "{0:0.0‰}", "en-EN", 0.001d, "1.0‰")]
        [InlineData("1", "0.0‰", "en-EN", 0.001d, "1.0‰")]
        [InlineData("1‰", "0.0‰", "en-EN", 0.001d, "1.0‰")]
        [InlineData("1 ‰", "0.0‰", "en-EN", 0.001d, "1.0‰")]
        [InlineData("0.25", "{0:0.0000}‰", null, 0.25d, "0.2500‰")]
        [DisplayTestMethodName]
        public async Task ShouldConvertTextInputWithPermilleStringFormat(string text, string format, string culture, object expectedValue, string expectedText)
        {
            await this.fixture.PrepareForTestAsync().ConfigureAwait(false);
            await TestHost.SwitchToAppThread();

            this.fixture.Window.TheNUD.NumericInputMode = NumericInput.All;
            this.fixture.Window.TheNUD.Culture = string.IsNullOrEmpty(culture) ? CultureInfo.InvariantCulture : CultureInfo.GetCultureInfo(culture);
            this.fixture.Window.TheNUD.StringFormat = format;

            SetText(this.fixture.TextBox, text);

            Assert.Equal(expectedValue, this.fixture.Window.TheNUD.Value);
            Assert.Equal(expectedText, this.fixture.TextBox.Text);
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
            await this.fixture.PrepareForTestAsync().ConfigureAwait(false);
            await TestHost.SwitchToAppThread();

            this.fixture.Window.TheNUD.NumericInputMode = NumericInput.Decimal;
            this.fixture.Window.TheNUD.Culture = CultureInfo.GetCultureInfo("fa-IR");

            SetText(this.fixture.TextBox, text);

            Assert.Equal(expectedValue, this.fixture.Window.TheNUD.Value);
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
            await this.fixture.PrepareForTestAsync().ConfigureAwait(false);
            await TestHost.SwitchToAppThread();

            this.fixture.Window.TheNUD.NumericInputMode = NumericInput.Numbers;
            this.fixture.Window.TheNUD.ParsingNumberStyle = NumberStyles.HexNumber;

            SetText(this.fixture.TextBox, text);

            Assert.Equal(expectedValue, this.fixture.Window.TheNUD.Value);
        }

        [Theory]
        [InlineData("42", "{}{0:X}", 66d, "42")]
        [InlineData("42", "{0:X}", 66d, "42")]
        [InlineData("42", "X", 66d, "42")]
        [InlineData("42", "{}{0:x}", 66d, "42")]
        [InlineData("42", "{0:x}", 66d, "42")]
        [InlineData("42", "x", 66d, "42")]
        [InlineData("255", "{}{0:X4}", 597d, "0255")]
        [InlineData("255", "{0:X4}", 597d, "0255")]
        [InlineData("255", "X4", 597d, "0255")]
        [InlineData("AFFE", "{}{0:X8}", 45054d, "0000AFFE")]
        [InlineData("AFFE", "{0:X8}", 45054d, "0000AFFE")]
        [InlineData("AFFE", "X8", 45054d, "0000AFFE")]
        [DisplayTestMethodName]
        public async Task ShouldConvertHexadecimalTextInputWithStringFormat(string text, string format, object expectedValue, string expectedText)
        {
            await this.fixture.PrepareForTestAsync().ConfigureAwait(false);
            await TestHost.SwitchToAppThread();

            this.fixture.Window.TheNUD.StringFormat = format;

            SetText(this.fixture.TextBox, text);

            Assert.Equal(expectedValue, this.fixture.Window.TheNUD.Value);
            Assert.Equal(expectedText, this.fixture.TextBox.Text);
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