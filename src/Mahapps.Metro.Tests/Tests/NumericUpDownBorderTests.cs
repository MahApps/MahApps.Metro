// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using MahApps.Metro.Controls;
using MahApps.Metro.Tests.TestHelpers;
using NUnit.Framework;

namespace MahApps.Metro.Tests.Tests
{
    /// <summary>
    /// The frame around a NumericUpDown belongs to the control, and the text box inside it draws
    /// none. The style of a text box gives its border the focused thickness once the caret is in it,
    /// which would put a second frame inside the first one.
    /// </summary>
    [TestFixture]
    public class NumericUpDownBorderTests
    {
        private TestWindow? window;

        [OneTimeSetUp]
        public async Task OneTimeSetUp()
        {
            this.window = await WindowHelpers.CreateInvisibleWindowAsync<TestWindow>().ConfigureAwait(true);
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            this.window?.Close();
            this.window = null;
        }

        [TearDown]
        public void TearDown()
        {
            if (this.window is not null)
            {
                this.window.Content = null;
            }
        }

        [TestCase("MahApps.Styles.NumericUpDown")]
        [TestCase("MahApps.Styles.NumericUpDown.Win10")]
        [Description("The caret changes the colour of the frame the control draws, and adds none of its own.")]
        public void TheTextBoxInsideDrawsNoFrameWithTheCaretInIt(string key)
        {
            var numericUpDown = new NumericUpDown
                                {
                                    Style = (Style)Application.Current.FindResource(key),
                                    Width = 200,
                                    Value = 42
                                };

            var page = new StackPanel();
            page.Children.Add(new Button { Content = "somewhere else to put the focus" });
            page.Children.Add(numericUpDown);

            this.window!.Content = page;
            this.window.UpdateLayout();
            ClipAssert.Pump();

            var inside = numericUpDown.FindChild<TextBox>("PART_TextBox");
            Assert.That(inside, Is.Not.Null, "the control should have its text box by now");

            var border = inside!.FindChild<Border>("BorderElement");
            Assert.That(border, Is.Not.Null, "and the text box its border element");

            Assume.That(border!.BorderThickness, Is.EqualTo(default(Thickness)), "nothing should be drawn before the focus");

            inside.Focus();
            Keyboard.Focus(inside);
            this.window.UpdateLayout();
            ClipAssert.Pump();

            // a build agent hands the keyboard to one window at a time
            Assume.That(inside.IsFocused, Is.True);

            Assert.That(border.BorderThickness, Is.EqualTo(default(Thickness)), $"{key} draws a second frame of {border.BorderThickness} inside the control");
        }
    }
}
