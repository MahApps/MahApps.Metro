// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

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
    public class TypedUpDownTests
    {
        private TypedUpDownWindow? window;

        [OneTimeSetUp]
        public async Task OneTimeSetUp()
        {
            this.window = await WindowHelpers.CreateInvisibleWindowAsync<TypedUpDownWindow>().ConfigureAwait(false);
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
            this.window?.TheDouble.ClearDependencyProperties();
            this.window?.TheDecimal.ClearDependencyProperties();
            this.window?.TheInteger.ClearDependencyProperties();
            this.window?.TheLong.ClearDependencyProperties();
        }

        [TearDown]
        public void TearDown()
        {
            // The editing flag is private state a cleared dependency property does not reach, so
            // every text box is taken out of editing here, as the double fixture does.
            this.window?.TheDouble.FindChild<TextBox>()?.RaiseEvent(new RoutedEventArgs(UIElement.LostFocusEvent));
            this.window?.TheDecimal.FindChild<TextBox>()?.RaiseEvent(new RoutedEventArgs(UIElement.LostFocusEvent));
            this.window?.TheInteger.FindChild<TextBox>()?.RaiseEvent(new RoutedEventArgs(UIElement.LostFocusEvent));
            this.window?.TheLong.FindChild<TextBox>()?.RaiseEvent(new RoutedEventArgs(UIElement.LostFocusEvent));
        }

        [Test]
        [Description("Every control shares the one template, so each of them has to find its parts.")]
        public void EveryControlGetsTheTemplate()
        {
            Assert.That(this.window, Is.Not.Null);

            NumericUpDownBase[] controls = { this.window.TheDouble, this.window.TheDecimal, this.window.TheInteger, this.window.TheLong };

            Assert.Multiple(() =>
                {
                    foreach (var control in controls)
                    {
                        var name = control.GetType().Name;
                        Assert.That(control.FindChild<TextBox>("PART_TextBox"), Is.Not.Null, name);
                        Assert.That(control.FindChild<RepeatButton>("PART_NumericUp"), Is.Not.Null, name);
                        Assert.That(control.FindChild<RepeatButton>("PART_NumericDown"), Is.Not.Null, name);
                    }
                });
        }

        [Test]
        [Description("Each control brings the bounds and the step of the type it holds.")]
        public void EveryControlBringsTheBoundsOfItsType()
        {
            Assert.That(this.window, Is.Not.Null);

            Assert.Multiple(() =>
                {
                    Assert.That(this.window.TheDouble.Minimum, Is.EqualTo(double.MinValue));
                    Assert.That(this.window.TheDouble.Maximum, Is.EqualTo(double.MaxValue));
                    Assert.That(this.window.TheDouble.Interval, Is.EqualTo(1d));

                    Assert.That(this.window.TheDecimal.Minimum, Is.EqualTo(decimal.MinValue));
                    Assert.That(this.window.TheDecimal.Maximum, Is.EqualTo(decimal.MaxValue));
                    Assert.That(this.window.TheDecimal.Interval, Is.EqualTo(1m));

                    Assert.That(this.window.TheInteger.Minimum, Is.EqualTo(int.MinValue));
                    Assert.That(this.window.TheInteger.Maximum, Is.EqualTo(int.MaxValue));
                    Assert.That(this.window.TheInteger.Interval, Is.EqualTo(1));

                    Assert.That(this.window.TheLong.Minimum, Is.EqualTo(long.MinValue));
                    Assert.That(this.window.TheLong.Maximum, Is.EqualTo(long.MaxValue));
                    Assert.That(this.window.TheLong.Interval, Is.EqualTo(1L));
                });
        }

        [Test]
        [Description("This is the whole reason the decimal control exists: three tenths added up come to 0.3.")]
        public void ADecimalKeepsTheDigitsItWasGiven()
        {
            Assert.That(this.window, Is.Not.Null);

            var textBox = this.window.TheDecimal.FindChild<TextBox>();
            Assert.That(textBox, Is.Not.Null);

            this.window.TheDecimal.SetCurrentValue(DecimalUpDown.IntervalProperty, 0.1m);
            SetText(textBox, "0.1");

            StepUp(this.window.TheDecimal);
            StepUp(this.window.TheDecimal);

            Assert.That(this.window.TheDecimal.Value, Is.EqualTo(0.3m));
            Assert.That(textBox.Text, Is.EqualTo("0.3"));
        }

        [Test]
        [Description("The same three steps in a double do not land on 0.3, which is what the decimal control is for.")]
        public void ADoubleDoesNotKeepThem()
        {
            Assert.That(this.window, Is.Not.Null);

            var textBox = this.window.TheDouble.FindChild<TextBox>();
            Assert.That(textBox, Is.Not.Null);

            this.window.TheDouble.SetCurrentValue(NumericUpDown.IntervalProperty, 0.1d);
            SetText(textBox, "0.1");

            StepUp(this.window.TheDouble);
            StepUp(this.window.TheDouble);

            Assert.That(this.window.TheDouble.Value, Is.Not.EqualTo(0.3d));
        }

        [Test]
        [Description("A count past what a double holds exactly stays what it was.")]
        public void ALongHoldsMoreThanADoubleCan()
        {
            Assert.That(this.window, Is.Not.Null);

            var textBox = this.window.TheLong.FindChild<TextBox>();
            Assert.That(textBox, Is.Not.Null);

            const long Counted = long.MaxValue - 1;

            SetText(textBox, Counted.ToString());

            Assert.That(this.window.TheLong.Value, Is.EqualTo(Counted));
            Assert.That((double)Counted, Is.EqualTo((double)long.MaxValue), "carried through a double it would not even be told apart from long.MaxValue");
        }

        [Test]
        [Description("An integer control takes no decimals, so the separator never reaches the text box.")]
        public void AnIntegerTakesNoDecimalSeparator()
        {
            Assert.That(this.window, Is.Not.Null);

            var textBox = this.window.TheInteger.FindChild<TextBox>();
            Assert.That(textBox, Is.Not.Null);

            Assert.That(this.window.TheInteger.NumericInputMode, Is.EqualTo(NumericInput.Numbers));

            SetText(textBox, "12.5");

            Assert.That(this.window.TheInteger.Value, Is.EqualTo(125));
        }

        [Test]
        [Description("The value is held to the bounds of the control, not to those of the type.")]
        public void TheValueStaysWithinTheBounds()
        {
            Assert.That(this.window, Is.Not.Null);

            this.window.TheInteger.SetCurrentValue(IntegerUpDown.MinimumProperty, 0);
            this.window.TheInteger.SetCurrentValue(IntegerUpDown.MaximumProperty, 10);

            this.window.TheInteger.SetCurrentValue(IntegerUpDown.ValueProperty, 42);
            Assert.That(this.window.TheInteger.Value, Is.EqualTo(10));

            this.window.TheInteger.SetCurrentValue(IntegerUpDown.ValueProperty, -42);
            Assert.That(this.window.TheInteger.Value, Is.EqualTo(0));
        }

        [Test]
        [Description("A StringFormat decides the text of a typed control just as it does for the double one.")]
        public void AStringFormatDecidesTheText()
        {
            Assert.That(this.window, Is.Not.Null);

            var textBox = this.window.TheDecimal.FindChild<TextBox>();
            Assert.That(textBox, Is.Not.Null);

            this.window.TheDecimal.SetCurrentValue(DecimalUpDown.StringFormatProperty, "{}{0:N2}");
            this.window.TheDecimal.SetCurrentValue(DecimalUpDown.ValueProperty, 1234.5m);

            Assert.That(textBox.Text, Is.EqualTo("1,234.50"));
        }

        [Test]
        [Description("A decimal binding stays a decimal all the way through, which is the half of #3673 a format cannot answer.")]
        public void ADecimalBindingIsNotCarriedThroughADouble()
        {
            Assert.That(this.window, Is.Not.Null);

            var viewModel = (TypedUpDownViewModel)this.window.DataContext;
            var textBox = this.window.TheBoundDecimal.FindChild<TextBox>();
            Assert.That(textBox, Is.Not.Null);

            SetText(textBox, "1.005");

            Assert.That(viewModel.Price, Is.EqualTo(1.005m));

            viewModel.Price = 2.675m;

            Assert.That(this.window.TheBoundDecimal.Value, Is.EqualTo(2.675m));
            Assert.That(textBox.Text, Is.EqualTo("2.675"));
        }

        [Test]
        [Description("A control nobody handed an interval to still steps by the one its type brought.")]
        public void TheFirstStepMovesTheValueWithoutAnIntervalBeingSet()
        {
            Assert.That(this.window, Is.Not.Null);

            this.window.TheLong.SetCurrentValue(LongUpDown.ValueProperty, 5L);

            StepUp(this.window.TheLong);

            Assert.That(this.window.TheLong.Value, Is.EqualTo(6L));
        }

        [Test]
        [Description("Holding a button down is a run of clicks, and every one of them has to move the value.")]
        public void EveryClickOfARepeatMovesTheValue()
        {
            Assert.That(this.window, Is.Not.Null);

            this.window.TheInteger.SetCurrentValue(IntegerUpDown.ValueProperty, 0);

            for (var i = 0; i < 10; i++)
            {
                StepUp(this.window.TheInteger);
            }

            // Ten steps of one, with the speed-up not yet at the hundred it takes to grow.
            Assert.That(this.window.TheInteger.Value, Is.EqualTo(10));
        }

        private static void StepUp(NumericUpDownBase control)
        {
            control.FindChild<RepeatButton>("PART_NumericUp")?.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
        }

        private static void SetText(TextBox theTextBox, string theText)
        {
            theTextBox.Clear();

            foreach (var c in theText)
            {
                var args = new TextCompositionEventArgs(Keyboard.PrimaryDevice, new TextComposition(InputManager.Current, theTextBox, c.ToString()))
                           {
                               RoutedEvent = UIElement.PreviewTextInputEvent
                           };
                theTextBox.RaiseEvent(args);
                args.RoutedEvent = UIElement.TextInputEvent;
                theTextBox.RaiseEvent(args);
            }

            theTextBox.RaiseEvent(new RoutedEventArgs(UIElement.LostFocusEvent));
        }
    }
}
