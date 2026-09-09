// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using MahApps.Metro.Controls;
using MahApps.Metro.Tests.TestHelpers;
using MahApps.Metro.Tests.Views;
using NUnit.Framework;

namespace MahApps.Metro.Tests.Tests
{
    [TestFixture]
    public class NumericUpDownPasteTests
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
            this.window?.TheNUD.ClearDependencyProperties();
        }

        [TearDown]
        public void TearDown()
        {
            this.window?.TheNUD.FindChild<TextBox>()?.RaiseEvent(new RoutedEventArgs(UIElement.LostFocusEvent));
        }

        [TestCase("hello 42 world", "42", TestName = "words around a number")]
        [TestCase("12abc", "12", TestName = "letters behind a number")]
        [TestCase("abc12", "12", TestName = "letters in front of a number")]
        [TestCase("$1,234.50", "1,234.50", TestName = "a formatted amount keeps its separators")]
        [TestCase("42", "42", TestName = "a plain number is left alone")]
        [TestCase("-3.5", "-3.5", TestName = "a negative number is left alone")]
        [TestCase("1e5", "1e5", TestName = "an exponent is left alone")]
        [Description("Only the number is taken out of what was pasted, so nothing else ever shows up in the field.")]
        public void OnlyTheNumberIsPasted(string pasted, string expected)
        {
            var textBox = this.TextBox();

            var args = Paste(textBox, pasted);

            Assert.That(args.CommandCancelled, Is.False, "the paste should go through");
            Assert.That(args.DataObject.GetData(DataFormats.Text), Is.EqualTo(expected));
        }

        [Test]
        [Description("Text without a number in it is refused, as before.")]
        public void TextWithoutANumberIsRefused()
        {
            var textBox = this.TextBox();

            var args = Paste(textBox, "abc");

            Assert.That(args.CommandCancelled, Is.True);
        }

        [Test]
        [Description("What is pasted joins what is already there, so a number split over two pastes still adds up.")]
        public void APasteJoinsWhatIsAlreadyThere()
        {
            var textBox = this.TextBox();
            textBox.Text = "12";
            textBox.CaretIndex = 2;
            textBox.SelectionStart = 2;
            textBox.SelectionLength = 0;

            var args = Paste(textBox, "34");

            Assert.That(args.CommandCancelled, Is.False);
            Assert.That(args.DataObject.GetData(DataFormats.Text), Is.EqualTo("34"));
        }

        private TextBox TextBox()
        {
            Assert.That(this.window, Is.Not.Null);

            var textBox = this.window.TheNUD.FindChild<TextBox>();
            Assert.That(textBox, Is.Not.Null);

            textBox.Clear();
            return textBox;
        }

        private static DataObjectPastingEventArgs Paste(TextBox textBox, string text)
        {
            var args = new DataObjectPastingEventArgs(new DataObject(DataFormats.Text, text), false, DataFormats.Text);
            textBox.RaiseEvent(args);
            return args;
        }
    }
}
