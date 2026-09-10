// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using MahApps.Metro.Controls;
using MahApps.Metro.Tests.TestHelpers;
using MahApps.Metro.Tests.Views;
using NUnit.Framework;

namespace MahApps.Metro.Tests.Tests
{
    [TestFixture]
    public class NumericUpDownFocusTests
    {
        private NumericUpDownWindow? window;
        private TextBox? neighbour;
        private NumericUpDown? numericUpDown;
        private TextBox? after;

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
            Assert.That(this.window, Is.Not.Null);

            var first = new Button { Content = "first", Height = 30 };
            this.neighbour = new TextBox { Text = "next to it", Height = 30 };
            this.numericUpDown = new NumericUpDown { Value = 200, Height = 30 };
            this.after = new TextBox { Text = "below it", Height = 30 };

            var panel = new StackPanel();
            panel.Children.Add(first);
            panel.Children.Add(this.neighbour);
            panel.Children.Add(this.numericUpDown);
            panel.Children.Add(this.after);

            this.window.Content = panel;
            this.window.UpdateLayout();
            ClipAssert.Pump();
        }

        [TearDown]
        public void TearDown()
        {
            if (this.window is not null)
            {
                this.window.Content = null;
            }
        }

        private TextBox TextBoxInside()
        {
            var inside = this.numericUpDown!.FindChild<TextBox>("PART_TextBox");
            Assert.That(inside, Is.Not.Null, "the control should have its text box by now");
            return inside!;
        }

        [Test]
        [Description("Focus given to the control belongs in its text box, or there is nowhere to type.")]
        public void FocusOnTheControlEndsUpInItsTextBox()
        {
            this.numericUpDown!.Focus();
            ClipAssert.Pump();

            // A build agent does not always hand out the keyboard focus.
            Assume.That(this.numericUpDown.IsKeyboardFocusWithin, Is.True);

            Assert.That(this.TextBoxInside().IsKeyboardFocused, Is.True, "the text box should have it, not the control");
            Assert.That(this.numericUpDown.IsKeyboardFocused, Is.False, "the control itself should not keep it");
        }

        [Test]
        [Description("The same holds when the focus comes from somewhere that already had it.")]
        public void FocusHandedOverFromANeighbourEndsUpInTheTextBox()
        {
            this.neighbour!.Focus();
            ClipAssert.Pump();
            Assume.That(this.neighbour.IsKeyboardFocused, Is.True);

            this.numericUpDown!.Focus();
            ClipAssert.Pump();

            Assume.That(this.numericUpDown.IsKeyboardFocusWithin, Is.True);
            Assert.That(this.TextBoxInside().IsKeyboardFocused, Is.True);
        }

        [Test]
        [Description("Tabbing onto the control walks straight into the text box as well.")]
        public void TabbingOntoTheControlEndsUpInTheTextBox()
        {
            this.neighbour!.Focus();
            ClipAssert.Pump();
            Assume.That(this.neighbour.IsKeyboardFocused, Is.True);

            this.neighbour.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
            ClipAssert.Pump();

            Assume.That(this.numericUpDown!.IsKeyboardFocusWithin, Is.True);
            Assert.That(this.TextBoxInside().IsKeyboardFocused, Is.True);
        }

        [Test]
        [Description("The focus manager names the control, and the text box is where typing goes.")]
        public void TheFocusManagerNamingTheControlEndsUpInTheTextBox()
        {
            var panel = (StackPanel)this.window!.Content;

            FocusManager.SetFocusedElement(panel, this.numericUpDown);
            Keyboard.Focus(this.numericUpDown);
            ClipAssert.Pump();

            Assume.That(this.numericUpDown!.IsKeyboardFocusWithin, Is.True);
            Assert.That(this.TextBoxInside().IsKeyboardFocused, Is.True);
        }

        [Test]
        [Description("Shift and tab out of the text box reaches what lies before the control, not the text box again.")]
        public void ShiftTabOutOfTheTextBoxLeavesTheControl()
        {
            this.numericUpDown!.Focus();
            ClipAssert.Pump();
            Assume.That(this.TextBoxInside().IsKeyboardFocused, Is.True);

            this.TextBoxInside().MoveFocus(new TraversalRequest(FocusNavigationDirection.Previous));
            ClipAssert.Pump();

            Assert.That(this.numericUpDown.IsKeyboardFocusWithin, Is.False, "the focus should have left the control");
            Assert.That(this.neighbour!.IsKeyboardFocused, Is.True, "and landed on what lies before it");
        }

        [Test]
        [Description("Tabbing on out of the text box reaches what lies after the control.")]
        public void TabOutOfTheTextBoxLeavesTheControl()
        {
            this.numericUpDown!.Focus();
            ClipAssert.Pump();
            Assume.That(this.TextBoxInside().IsKeyboardFocused, Is.True);

            this.TextBoxInside().MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
            ClipAssert.Pump();

            Assert.That(this.after!.IsKeyboardFocused, Is.True, "the one below it should have the focus");
        }

        [Test]
        [Description("The control steps out of the tab order while the text box holds the focus, and back in after.")]
        public void TheControlGivesItsTabStopBackWhenTheFocusHasLeft()
        {
            Assume.That(this.numericUpDown!.IsTabStop, Is.True, "a control is a tab stop to start with");

            this.numericUpDown.Focus();
            ClipAssert.Pump();
            Assume.That(this.TextBoxInside().IsKeyboardFocused, Is.True);

            Assert.That(this.numericUpDown.IsTabStop, Is.False, "out of the tab order while the text box has it");

            this.neighbour!.Focus();
            ClipAssert.Pump();

            Assert.That(this.numericUpDown.IsTabStop, Is.True, "and back in once the focus is gone");
        }

        [Test]
        [Description("A control taken out of the tab order by hand is left out of it.")]
        public void AControlKeptOutOfTheTabOrderIsLeftOutOfIt()
        {
            this.numericUpDown!.SetCurrentValue(NumericUpDown.IsTabStopProperty, false);

            this.numericUpDown.Focus();
            ClipAssert.Pump();
            Assume.That(this.TextBoxInside().IsKeyboardFocused, Is.True);

            this.neighbour!.Focus();
            ClipAssert.Pump();

            Assert.That(this.numericUpDown.IsTabStop, Is.False, "what the consumer asked for should come back, not true");
        }

        [Test]
        [Description("A control nobody may type into keeps the focus to itself.")]
        public void AControlThatTakesNoTypingKeepsTheFocus()
        {
            this.numericUpDown!.SetCurrentValue(NumericUpDown.InterceptManualEnterProperty, false);
            this.numericUpDown.Focus();
            ClipAssert.Pump();

            Assume.That(this.numericUpDown.IsKeyboardFocusWithin, Is.True);
            Assert.That(this.numericUpDown.IsKeyboardFocused, Is.True);
        }
    }
}
