// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using MahApps.Metro.Controls;
using MahApps.Metro.Tests.TestHelpers;
using MahApps.Metro.Tests.Views;
using NUnit.Framework;

namespace MahApps.Metro.Tests.Tests
{
    [TestFixture]
    public class NumericUpDownTests
    {
        private NumericUpDownWindow? window;

        [OneTimeSetUp]
        public async Task OneTimeSetUp()
        {
            this.window = await WindowHelpers.CreateInvisibleWindowAsync<NumericUpDownWindow>().ConfigureAwait(false);
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            this.window?.Close();
            this.window = null;
        }

        [SetUp]
        public void SetUp()
        {
            this.PreparePropertiesForTest();
        }

        private void PreparePropertiesForTest(IList<string>? properties = null)
        {
            this.window?.TheNUD.ClearDependencyProperties(properties);
        }

        private static bool NearlyEqual(double a, double b, double epsilon)
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

        [Test]
        public void ShouldSnapToMultipleOfInterval()
        {
            Assert.That(this.window, Is.Not.Null);

            var numUp = this.window.TheNUD.FindChild<RepeatButton>("PART_NumericUp");
            var numDown = this.window.TheNUD.FindChild<RepeatButton>("PART_NumericDown");

            Assert.That(numUp, Is.Not.Null);
            Assert.That(numDown, Is.Not.Null);

            this.window.TheNUD.Interval = 0.1;
            this.window.TheNUD.SnapToMultipleOfInterval = true;

            this.window.TheNUD.Value = 0;
            for (int i = 1; i < 15; i++)
            {
                numUp.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
                Assert.That(this.window.TheNUD.Value, Is.EqualTo(0d + 0.1 * i));
            }

            this.window.TheNUD.Value = 0;
            for (int i = 1; i < 15; i++)
            {
                numDown.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
                Assert.That(this.window.TheNUD.Value, Is.EqualTo(0d - 0.1 * i));
            }
        }

        [Theory]
        [TestCase(42d, "", "42")]
        [TestCase(null, "", "")]
        [TestCase(0.25d, "{}{0:0.00%}", "25.00%")] // 3376 Case 1
        [TestCase(0.25d, "{0:0.00%}", "25.00%")] // 3376 Case 1
        [TestCase(0.25d, "0.00%", "25.00%")] // 3376 Case 1
        [TestCase(0.25d, "{}{0:0.00‰}", "250.00‰")] // 3376 Case 2
        [TestCase(0.25d, "{0:0.00‰}", "250.00‰")] // 3376 Case 2
        [TestCase(0.25d, "0.00‰", "250.00‰")] // 3376 Case 2
        [TestCase(0.25d, "{}{0:0.0000}%", "0.2500%")] // 3376 Case 3
        [TestCase(0.25d, "{0:0.0000}%", "0.2500%")] // 3376 Case 3
        [TestCase(0.25d, "{}{0:0.00000}‰", "0.25000‰")] // 3376 Case 4
        [TestCase(0.25d, "{0:0.00000}‰", "0.25000‰")] // 3376 Case 4
        [TestCase(0.25d, "{}{0:P}", "25.00 %")] // 3376 Case 5
        [TestCase(0.25d, "{0:P}", "25.00 %")] // 3376 Case 5
        [TestCase(0.25d, "P", "25.00 %")] // 3376 Case 5
        [TestCase(123456789d, "X", "75BCD15")]
        [TestCase(123456789d, "X2", "75BCD15")]
        [TestCase(255d, "X", "FF")]
        [TestCase(-1d, "x", "ffffffff")]
        [TestCase(255d, "x4", "00ff")]
        [TestCase(-1d, "X4", "FFFFFFFF")]
        public void ShouldFormatValueInput(object? value, string format, string expectedText)
        {
            Assert.That(this.window, Is.Not.Null);

            var textBox = this.window.TheNUD.FindChild<TextBox>();
            var numUp = this.window.TheNUD.FindChild<RepeatButton>("PART_NumericUp");
            var numDown = this.window.TheNUD.FindChild<RepeatButton>("PART_NumericDown");

            Assert.That(numUp, Is.Not.Null);
            Assert.That(numDown, Is.Not.Null);
            Assert.That(textBox, Is.Not.Null);

            this.window.TheNUD.Culture = CultureInfo.InvariantCulture;
            this.window.TheNUD.NumericInputMode = NumericInput.All;
            this.window.TheNUD.StringFormat = format;

            this.window.TheNUD.SetCurrentValue(NumericUpDown.ValueProperty, value);

            Assert.That(textBox.Text, Is.EqualTo(expectedText));
            Assert.That(this.window.TheNUD.Value, Is.EqualTo(value));
        }

        [Theory]
        [TestCase("42", NumericInput.All, 42d)]
        [TestCase("42.", NumericInput.All, 42d)]
        [TestCase("42.2", NumericInput.All, 42.2d)]
        [TestCase(".", NumericInput.All, 0d)]
        [TestCase(".9", NumericInput.All, 0.9d)]
        [TestCase(".0115", NumericInput.All, 0.0115d)]
        [TestCase("-.5", NumericInput.All, -0.5d)]
        [TestCase("", NumericInput.All, null)]
        [TestCase("42", NumericInput.Decimal, 42d)]
        [TestCase("42.", NumericInput.Decimal, 42d)]
        [TestCase("42.2", NumericInput.Decimal, 42.2d)]
        [TestCase(".", NumericInput.Decimal, 0d)]
        [TestCase(".9", NumericInput.Decimal, 0.9d)]
        [TestCase(".0115", NumericInput.Decimal, 0.0115d)]
        [TestCase("-.5", NumericInput.Decimal, -0.5d)]
        [TestCase("", NumericInput.Decimal, null)]
        [TestCase("42", NumericInput.Numbers, 42d)]
        [TestCase("42.", NumericInput.Numbers, 42d)]
        [TestCase("42.2", NumericInput.Numbers, 422d)]
        [TestCase(".", NumericInput.Numbers, null)]
        [TestCase(".9", NumericInput.Numbers, 9d)]
        [TestCase("", NumericInput.Numbers, null)]
        public void ShouldConvertManualTextInput(string text, NumericInput numericInput, object? expectedValue)
        {
            Assert.That(this.window, Is.Not.Null);

            var textBox = this.window.TheNUD.FindChild<TextBox>();
            var numUp = this.window.TheNUD.FindChild<RepeatButton>("PART_NumericUp");
            var numDown = this.window.TheNUD.FindChild<RepeatButton>("PART_NumericDown");

            Assert.That(numUp, Is.Not.Null);
            Assert.That(numDown, Is.Not.Null);
            Assert.That(textBox, Is.Not.Null);

            this.window.TheNUD.NumericInputMode = numericInput;

            SetText(textBox, text);

            Assert.That(this.window.TheNUD.Value, Is.EqualTo(expectedValue));
        }

        [Theory]
        [TestCase("42", "{}{0:N2} cm", 42d, "42.00 cm")]
        [TestCase("42.", "{}{0:N2} cm", 42d, "42.00 cm")]
        [TestCase("42.2", "{}{0:N2} cm", 42.2d, "42.20 cm")]
        [TestCase(".", "{}{0:N2} cm", 0d, "0.00 cm")]
        [TestCase(".9", "{}{0:N2} cm", 0.9d, "0.90 cm")]
        [TestCase(".0115", "{}{0:N2} cm", 0.0115d, "0.01 cm")]
        [TestCase(".0155", "{}{0:N2} cm", 0.0155d, "0.02 cm")]
        [TestCase("-.5", "{}{0:N2} cm", -0.5d, "-0.50 cm")]
        [TestCase("100.00 cm", "{}{0:N2} cm", 100d, "100.00 cm")]
        [TestCase("200.00cm", "{}{0:N2} cm", 200d, "200.00 cm")]
        [TestCase("200.20", "{}{0:N2} cm", 200.2d, "200.20 cm")]
        [TestCase("15", "{}{0}mmHg", 15d, "15mmHg")] // GH-3551
        [TestCase("0.986", "{}{0:G3} mPa·s", 0.986d, "0.986 mPa·s")] // GH-3376#issuecomment-472324787
        [TestCase("", "{}{0:N2} cm", null, "")]
        public void ShouldConvertTextInputWithStringFormat(string text, string format, object? expectedValue, string expectedText)
        {
            Assert.That(this.window, Is.Not.Null);

            var textBox = this.window.TheNUD.FindChild<TextBox>();
            var numUp = this.window.TheNUD.FindChild<RepeatButton>("PART_NumericUp");
            var numDown = this.window.TheNUD.FindChild<RepeatButton>("PART_NumericDown");

            Assert.That(numUp, Is.Not.Null);
            Assert.That(numDown, Is.Not.Null);
            Assert.That(textBox, Is.Not.Null);

            this.window.TheNUD.NumericInputMode = NumericInput.All;
            this.window.TheNUD.StringFormat = format;

            SetText(textBox, text);

            Assert.That(this.window.TheNUD.Value, Is.EqualTo(expectedValue));
            Assert.That(textBox.Text, Is.EqualTo(expectedText));
        }

        [Theory]
        [TestCase("100", "{}{0:P0}", "en-EN", 1d, "100%", false)]
        [TestCase("100 %", "{}{0:P0}", "en-EN", 1d, "100%", false)]
        [TestCase("100%", "{}{0:P0}", "en-EN", 1d, "100%", false)]
        [TestCase("-0.39678", "{}{0:P1}", "en-EN", -0.0039678d, "-0.4%", true)]
        [TestCase("50", "P0", "en-EN", 0.5d, "50%", false)]
        [TestCase("50", "P1", "en-EN", 0.5d, "50.0%", false)]
        [TestCase("-0.39678", "P1", "en-EN", -0.0039678d, "-0.4%", true)]
        [TestCase("10", "{}{0:P0}", null, 0.1d, "10 %", false)]
        [TestCase("-0.39678", "{}{0:P1}", null, -0.0039678d, "-0.4 %", true)]
        [TestCase("1", "P0", null, 0.01d, "1 %", false)]
        [TestCase("-0.39678", "P1", null, -0.0039678d, "-0.4 %", true)]
        [TestCase("1", "{}{0:0.0%}", null, 0.01d, "1.0%", false)]
        [TestCase("1", "0.0%", null, 0.01d, "1.0%", false)]
        [TestCase("0.25", "{0:0.0000}%", null, 0.25d, "0.2500%", false)] // GH-3376 Case 3
        [TestCase("100", "{}{0}%", null, 100d, "100%", false)]
        [TestCase("100%", "{}{0}%", null, 100d, "100%", false)]
        [TestCase("100 %", "{}{0}%", null, 100d, "100%", false)]
        public void ShouldConvertTextInputWithPercentageStringFormat(string text, string format, string? culture, object expectedValue, string expectedText, bool useEpsilon)
        {
            Assert.That(this.window, Is.Not.Null);

            var textBox = this.window.TheNUD.FindChild<TextBox>();
            var numUp = this.window.TheNUD.FindChild<RepeatButton>("PART_NumericUp");
            var numDown = this.window.TheNUD.FindChild<RepeatButton>("PART_NumericDown");

            Assert.That(numUp, Is.Not.Null);
            Assert.That(numDown, Is.Not.Null);
            Assert.That(textBox, Is.Not.Null);

            this.window.TheNUD.NumericInputMode = NumericInput.All;
            this.window.TheNUD.Culture = string.IsNullOrEmpty(culture) ? CultureInfo.InvariantCulture : CultureInfo.GetCultureInfo(culture);
            this.window.TheNUD.StringFormat = format;

            SetText(textBox, text);

            if (useEpsilon)
            {
                Assert.That(this.window.TheNUD.Value.HasValue, Is.True);
                Assert.That(NearlyEqual((double)expectedValue, this.window.TheNUD.Value.Value, 0.000005),
                            Is.True,
                            $"The input '{text}' should be '{expectedValue} ({expectedText})', but value is '{this.window.TheNUD.Value.Value}'");
            }
            else
            {
                Assert.That(this.window.TheNUD.Value, Is.EqualTo(expectedValue));
            }

            Assert.That(textBox.Text, Is.EqualTo(expectedText));
        }

        [Theory]
        [TestCase("1", "{}{0:0.0‰}", null, 0.001d, "1.0‰")]
        [TestCase("1‰", "{}{0:0.0‰}", null, 0.001d, "1.0‰")]
        [TestCase("1 ‰", "{}{0:0.0‰}", null, 0.001d, "1.0‰")]
        [TestCase("1", "{0:0.0‰}", null, 0.001d, "1.0‰")]
        [TestCase("1‰", "{0:0.0‰}", null, 0.001d, "1.0‰")]
        [TestCase("1 ‰", "{0:0.0‰}", null, 0.001d, "1.0‰")]
        [TestCase("1", "0.0‰", null, 0.001d, "1.0‰")]
        [TestCase("1‰", "0.0‰", null, 0.001d, "1.0‰")]
        [TestCase("1 ‰", "0.0‰", null, 0.001d, "1.0‰")]
        [TestCase("1", "{}{0:0.0‰}", "en-EN", 0.001d, "1.0‰")]
        [TestCase("1‰", "{}{0:0.0‰}", "en-EN", 0.001d, "1.0‰")]
        [TestCase("1 ‰", "{}{0:0.0‰}", "en-EN", 0.001d, "1.0‰")]
        [TestCase("1", "{0:0.0‰}", "en-EN", 0.001d, "1.0‰")]
        [TestCase("1‰", "{0:0.0‰}", "en-EN", 0.001d, "1.0‰")]
        [TestCase("1 ‰", "{0:0.0‰}", "en-EN", 0.001d, "1.0‰")]
        [TestCase("1", "0.0‰", "en-EN", 0.001d, "1.0‰")]
        [TestCase("1‰", "0.0‰", "en-EN", 0.001d, "1.0‰")]
        [TestCase("1 ‰", "0.0‰", "en-EN", 0.001d, "1.0‰")]
        [TestCase("0.25", "{0:0.0000}‰", null, 0.25d, "0.2500‰")]
        public void ShouldConvertTextInputWithPermilleStringFormat(string text, string format, string? culture, object expectedValue, string expectedText)
        {
            Assert.That(this.window, Is.Not.Null);

            var textBox = this.window.TheNUD.FindChild<TextBox>();
            var numUp = this.window.TheNUD.FindChild<RepeatButton>("PART_NumericUp");
            var numDown = this.window.TheNUD.FindChild<RepeatButton>("PART_NumericDown");

            Assert.That(numUp, Is.Not.Null);
            Assert.That(numDown, Is.Not.Null);
            Assert.That(textBox, Is.Not.Null);

            this.window.TheNUD.NumericInputMode = NumericInput.All;
            this.window.TheNUD.Culture = string.IsNullOrEmpty(culture) ? CultureInfo.InvariantCulture : CultureInfo.GetCultureInfo(culture);
            this.window.TheNUD.StringFormat = format;

            SetText(textBox, text);

            Assert.That(this.window.TheNUD.Value, Is.EqualTo(expectedValue));
            Assert.That(textBox.Text, Is.EqualTo(expectedText));
        }

        [Theory]
        [TestCase("42", 42d)]
        [TestCase("42/751", 42.751d)]
        [TestCase("/", 0d)]
        [TestCase("/9", 0.9d)]
        [TestCase("/0115", 0.0115d)]
        public void ShouldConvertDecimalTextInputWithSpecialCulture(string text, object expectedValue)
        {
            Assert.That(this.window, Is.Not.Null);

            var textBox = this.window.TheNUD.FindChild<TextBox>();
            var numUp = this.window.TheNUD.FindChild<RepeatButton>("PART_NumericUp");
            var numDown = this.window.TheNUD.FindChild<RepeatButton>("PART_NumericDown");

            Assert.That(numUp, Is.Not.Null);
            Assert.That(numDown, Is.Not.Null);
            Assert.That(textBox, Is.Not.Null);

            this.window.TheNUD.NumericInputMode = NumericInput.Decimal;
            this.window.TheNUD.Culture = CultureInfo.GetCultureInfo("fa-IR");

            SetText(textBox, text);

            Assert.That(this.window.TheNUD.Value, Is.EqualTo(expectedValue));
        }

        [Theory]
        [TestCase("42", 66d)]
        [TestCase("F", 15d)]
        [TestCase("1F", 31d)]
        [TestCase("37C5", 14277d)]
        [TestCase("ACDC", 44252d)]
        [TestCase("10000", 65536d)]
        [TestCase("AFFE", 45054d)]
        [TestCase("AFFE0815", 2952661013d)]
        public void ShouldConvertHexadecimalTextInput(string text, object expectedValue)
        {
            Assert.That(this.window, Is.Not.Null);

            var textBox = this.window.TheNUD.FindChild<TextBox>();
            var numUp = this.window.TheNUD.FindChild<RepeatButton>("PART_NumericUp");
            var numDown = this.window.TheNUD.FindChild<RepeatButton>("PART_NumericDown");

            Assert.That(numUp, Is.Not.Null);
            Assert.That(numDown, Is.Not.Null);
            Assert.That(textBox, Is.Not.Null);

            this.window.TheNUD.NumericInputMode = NumericInput.Numbers;
            this.window.TheNUD.ParsingNumberStyle = NumberStyles.HexNumber;

            SetText(textBox, text);

            Assert.That(this.window.TheNUD.Value, Is.EqualTo(expectedValue));
        }

        [Theory]
        [TestCase("42", "{}{0:X}", 66d, "42")]
        [TestCase("42", "{0:X}", 66d, "42")]
        [TestCase("42", "X", 66d, "42")]
        [TestCase("42", "{}{0:x}", 66d, "42")]
        [TestCase("42", "{0:x}", 66d, "42")]
        [TestCase("42", "x", 66d, "42")]
        [TestCase("255", "{}{0:X4}", 597d, "0255")]
        [TestCase("255", "{0:X4}", 597d, "0255")]
        [TestCase("255", "X4", 597d, "0255")]
        [TestCase("AFFE", "{}{0:X8}", 45054d, "0000AFFE")]
        [TestCase("AFFE", "{0:X8}", 45054d, "0000AFFE")]
        [TestCase("AFFE", "X8", 45054d, "0000AFFE")]
        public void ShouldConvertHexadecimalTextInputWithStringFormat(string text, string format, object expectedValue, string expectedText)
        {
            Assert.That(this.window, Is.Not.Null);

            var textBox = this.window.TheNUD.FindChild<TextBox>();
            var numUp = this.window.TheNUD.FindChild<RepeatButton>("PART_NumericUp");
            var numDown = this.window.TheNUD.FindChild<RepeatButton>("PART_NumericDown");

            Assert.That(numUp, Is.Not.Null);
            Assert.That(numDown, Is.Not.Null);
            Assert.That(textBox, Is.Not.Null);

            this.window.TheNUD.StringFormat = format;

            SetText(textBox, text);

            Assert.That(this.window.TheNUD.Value, Is.EqualTo(expectedValue));
            Assert.That(textBox.Text, Is.EqualTo(expectedText));
        }

        private static void SetText(TextBox theTextBox, string theText)
        {
            theTextBox.Clear();
            foreach (var c in theText)
            {
                var textCompositionEventArgs = new TextCompositionEventArgs(Keyboard.PrimaryDevice, new TextComposition(InputManager.Current, theTextBox, c.ToString()));
                textCompositionEventArgs.RoutedEvent = UIElement.PreviewTextInputEvent;
                theTextBox.RaiseEvent(textCompositionEventArgs);
                textCompositionEventArgs.RoutedEvent = UIElement.TextInputEvent;
                theTextBox.RaiseEvent(textCompositionEventArgs);
            }

            theTextBox.RaiseEvent(new RoutedEventArgs(UIElement.LostFocusEvent));
        }

        [Test]
        public void ShouldSetDefaultValue()
        {
            Assert.That(this.window, Is.Not.Null);

            var textBox = this.window.TheNUD.FindChild<TextBox>();
            var numUp = this.window.TheNUD.FindChild<RepeatButton>("PART_NumericUp");
            var numDown = this.window.TheNUD.FindChild<RepeatButton>("PART_NumericDown");

            Assert.That(numUp, Is.Not.Null);
            Assert.That(numDown, Is.Not.Null);
            Assert.That(textBox, Is.Not.Null);

            var nud = this.window.TheNUD;
            nud.Minimum = 0;
            nud.Maximum = 10;

            // 1. Test: The default value must be set here. Let's check this.

            nud.DefaultValue = 1;
            nud.Value = null;

            Assert.That(nud.DefaultValue, Is.EqualTo(nud.Value));

            // 2. Test: There is no default value, so we should be able to set it to null

            nud.DefaultValue = null;
            nud.Value = null;

            Assert.That(nud.DefaultValue, Is.EqualTo(nud.Value));

            // 3. Test: We set the Default Value greater than the Maximum. It should be corrected by the control
            nud.DefaultValue = 100;
            nud.Value = null;

            Assert.That(nud.DefaultValue, Is.EqualTo(nud.Maximum));
            Assert.That(nud.Value, Is.EqualTo(nud.Maximum));

            // 4. Test: We set the Default Value lower than the Minimum. It should be corrected by the control
            nud.DefaultValue = -100;
            nud.Value = null;

            Assert.That(nud.DefaultValue, Is.EqualTo(nud.Minimum));
            Assert.That(nud.Value, Is.EqualTo(nud.Minimum));
        }
    }
}