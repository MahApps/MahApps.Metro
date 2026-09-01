// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
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

        [TearDown]
        public void TearDown()
        {
            // The control's editing flag is private state that ClearDependencyProperties cannot
            // reach. A test that types without committing would leave it set, and OnValueChanged
            // then stops updating the text box for every test that follows. Ending the edit here
            // keeps that failure mode out of the fixture.
            this.window?.TheNUD.FindChild<TextBox>()?.RaiseEvent(new RoutedEventArgs(UIElement.LostFocusEvent));
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
        [TestCase(3000000000d, "X", "B2D05E00")] // GH-4565: was saturated to int.MaxValue and rendered 7FFFFFFF
        [TestCase(2147483648d, "X", "80000000")] // GH-4565: int.MaxValue + 1
        [TestCase(-3000000000d, "X", "FFFFFFFF4D2FA200")] // GH-4565: below int.MinValue
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
            TypeText(theTextBox, theText);

            theTextBox.RaiseEvent(new RoutedEventArgs(UIElement.LostFocusEvent));
        }

        /// <summary>
        /// Simulates the manual text input, but keeps the control in editing state, so no LostFocus event is raised here.
        /// </summary>
        private static void TypeText(TextBox theTextBox, string theText)
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

        [Test]
        public void ShouldNotSyncTextWithValueWhileEditingByDefault()
        {
            Assert.That(this.window, Is.Not.Null);

            Assert.That(this.window.TheNUD.SyncTextWithValueWhileEditing, Is.False);
        }

        [Theory]
        // The value coerced by the view model is shown while the user is still editing
        [TestCase(true, "", "10", "10")]
        [TestCase(true, "{}{0:N2} cm", "10.00 cm", "10.00 cm")]
        // The old behavior: the typed text stays until the control loses the focus
        [TestCase(false, "", "50", "10")]
        [TestCase(false, "{}{0:N2} cm", "50", "10.00 cm")]
        public void ShouldSyncTextWithCoercedValueFromBindingWhileEditing(bool syncTextWithValueWhileEditing, string format, string expectedText, string expectedTextAfterLostFocus)
        {
            Assert.That(this.window, Is.Not.Null);

            var textBox = this.window.TheNUD.FindChild<TextBox>();

            Assert.That(textBox, Is.Not.Null);

            var nud = this.window.TheNUD;
            nud.Culture = CultureInfo.InvariantCulture;
            nud.NumericInputMode = NumericInput.All;
            nud.StringFormat = format;
            nud.SyncTextWithValueWhileEditing = syncTextWithValueWhileEditing;

            var viewModel = new ClampingTestViewModel { MaxAllowedValue = 10d };

            BindingOperations.SetBinding(nud,
                                        NumericUpDown.ValueProperty,
                                        new Binding(nameof(ClampingTestViewModel.Value))
                                        {
                                            Source = viewModel,
                                            Mode = BindingMode.TwoWay,
                                            UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
                                        });

            // The user types a value which is too large for the view model, so it gets clamped there
            TypeText(textBox, "50");

            Assert.That(viewModel.Value, Is.EqualTo(10d));
            Assert.That(nud.Value, Is.EqualTo(10d));
            Assert.That(textBox.Text, Is.EqualTo(expectedText));

            // On lost focus the text is refreshed from the value in both cases
            textBox.RaiseEvent(new RoutedEventArgs(UIElement.LostFocusEvent));

            Assert.That(textBox.Text, Is.EqualTo(expectedTextAfterLostFocus));

            BindingOperations.ClearBinding(nud, NumericUpDown.ValueProperty);
        }

        [Test]
        public void ShouldSelectTextAfterExternalValueSyncWhileEditing()
        {
            Assert.That(this.window, Is.Not.Null);

            var textBox = this.window.TheNUD.FindChild<TextBox>();

            Assert.That(textBox, Is.Not.Null);

            var nud = this.window.TheNUD;
            nud.Culture = CultureInfo.InvariantCulture;
            nud.NumericInputMode = NumericInput.All;
            nud.SyncTextWithValueWhileEditing = true;

            var viewModel = new ClampingTestViewModel { MaxAllowedValue = 10d };

            BindingOperations.SetBinding(nud,
                                        NumericUpDown.ValueProperty,
                                        new Binding(nameof(ClampingTestViewModel.Value))
                                        {
                                            Source = viewModel,
                                            Mode = BindingMode.TwoWay,
                                            UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
                                        });

            textBox.Focus();

            // The text is only selected if the TextBox really has the keyboard focus,
            // which is not always possible on a build agent.
            Assume.That(textBox.IsKeyboardFocused, Is.True);

            try
            {
                TypeText(textBox, "50");

                Assert.That(textBox.Text, Is.EqualTo("10"));
                Assert.That(textBox.SelectedText, Is.EqualTo("10"));
            }
            finally
            {
                Keyboard.ClearFocus();

                BindingOperations.ClearBinding(nud, NumericUpDown.ValueProperty);
            }
        }

        [Theory]
        [TestCase("42.", "")]
        [TestCase("42.", "{}{0:N2} cm")]
        [TestCase("-.5", "")]
        [TestCase("-0.39678", "{}{0:P1}")] // The rounded value must not overwrite the typed text
        public void ShouldKeepInProgressTextWhileTypingWithSyncTextWithValueWhileEditing(string text, string format)
        {
            Assert.That(this.window, Is.Not.Null);

            var textBox = this.window.TheNUD.FindChild<TextBox>();

            Assert.That(textBox, Is.Not.Null);

            var nud = this.window.TheNUD;
            nud.Culture = CultureInfo.InvariantCulture;
            nud.NumericInputMode = NumericInput.All;
            nud.StringFormat = format;
            nud.SyncTextWithValueWhileEditing = true;

            // The typed text must not be reformatted while the user is still typing
            TypeText(textBox, text);

            Assert.That(textBox.Text, Is.EqualTo(text));
        }


        /// <summary>
        /// GH-4565: ChangeValueFromTextInput called OnValueChanged in addition to the change
        /// already raised by the ValueProperty callback, so every single value change was
        /// reported to the consumer twice.
        /// </summary>
        [Test]
        public void ShouldRaiseValueChangedOnlyOncePerChangeOnManualTextInput()
        {
            Assert.That(this.window, Is.Not.Null);

            var textBox = this.window.TheNUD.FindChild<TextBox>();
            Assert.That(textBox, Is.Not.Null);

            this.window.TheNUD.SetCurrentValue(NumericUpDown.ValueProperty, 0d);

            // Typing "42" legitimately walks the value through several states, so the count
            // itself is not the contract. No single transition may be reported twice.
            var transitions = new List<string>();

            void OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double?> e)
            {
                transitions.Add(FormattableString.Invariant($"{e.OldValue} -> {e.NewValue}"));
            }

            this.window.TheNUD.ValueChanged += OnValueChanged;
            try
            {
                SetText(textBox, "42");
            }
            finally
            {
                this.window.TheNUD.ValueChanged -= OnValueChanged;
            }

            Assert.That(this.window.TheNUD.Value, Is.EqualTo(42d));
            Assert.That(transitions, Is.Not.Empty);
            Assert.That(transitions, Is.Unique, $"a value change was reported more than once: {string.Join(" | ", transitions)}");
        }

        /// <summary>
        /// GH-4565: OnPreviewTextInput rejected any keystroke whose resulting text would be
        /// coerced, so a value below Minimum could never be typed digit by digit.
        /// </summary>
        [Test]
        public void ShouldNotBlockTypingADigitBelowMinimum()
        {
            Assert.That(this.window, Is.Not.Null);

            var textBox = this.window.TheNUD.FindChild<TextBox>();
            Assert.That(textBox, Is.Not.Null);

            this.window.TheNUD.SetCurrentValue(NumericUpDown.MinimumProperty, 50d);
            this.window.TheNUD.SetCurrentValue(NumericUpDown.MaximumProperty, 100d);
            this.window.TheNUD.SetCurrentValue(NumericUpDown.ValueProperty, 60d);

            textBox.Clear();

            // "5" is the first keystroke of "55", but on its own it is below Minimum.
            var args = new TextCompositionEventArgs(Keyboard.PrimaryDevice, new TextComposition(InputManager.Current, textBox, "5"))
                       {
                           RoutedEvent = UIElement.PreviewTextInputEvent
                       };

            textBox.RaiseEvent(args);

            // The edit is left open on purpose; [TearDown] closes it.
            Assert.That(args.Handled, Is.False, "the keystroke was swallowed, so the value can never be typed");
        }

        /// <summary>
        /// GH-4565: guards the hexadecimal formatting fallback. A value outside the int range
        /// must not end up at double.ToString("X"), which throws a FormatException.
        /// </summary>
        [Test]
        public void ShouldNotThrowWhenFormattingHexadecimalValueAboveIntMax()
        {
            Assert.That(this.window, Is.Not.Null);

            var textBox = this.window.TheNUD.FindChild<TextBox>();
            Assert.That(textBox, Is.Not.Null);

            this.window.TheNUD.Culture = CultureInfo.InvariantCulture;
            this.window.TheNUD.NumericInputMode = NumericInput.All;
            this.window.TheNUD.StringFormat = "X";

            Assert.That(() => this.window.TheNUD.SetCurrentValue(NumericUpDown.ValueProperty, (double)int.MaxValue + 1d),
                        Throws.Nothing);
            Assert.That(textBox.Text, Is.Not.Empty);
        }

        private sealed class ClampingTestViewModel : INotifyPropertyChanged
        {
            private double? value;

            public double? MaxAllowedValue { get; set; }

            public double? Value
            {
                get => this.value;
                set
                {
                    var coercedValue = value > this.MaxAllowedValue ? this.MaxAllowedValue : value;

                    if (Nullable.Equals(this.value, coercedValue))
                    {
                        return;
                    }

                    this.value = coercedValue;
                    this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.Value)));
                }
            }

            public event PropertyChangedEventHandler? PropertyChanged;
        }
    }
}