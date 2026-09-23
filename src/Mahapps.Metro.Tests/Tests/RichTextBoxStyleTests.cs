// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using MahApps.Metro.Controls;
using MahApps.Metro.Tests.TestHelpers;
using NUnit.Framework;

namespace MahApps.Metro.Tests.Tests
{
    /// <summary>
    /// The rich text box hands the font of the box down to its document and paints a hyperlink the
    /// way the theme paints text, and it does that from the resources of the default style. The two
    /// Windows styles stand on that one, so what they inherit is worth an assertion of its own.
    /// </summary>
    [TestFixture]
    public class RichTextBoxStyleTests
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

        [TestCase("MahApps.Styles.RichTextBox")]
        [TestCase("MahApps.Styles.RichTextBox.Win10")]
        [TestCase("MahApps.Styles.RichTextBox.WinUI")]
        [Description("The document is no child of the box in the tree, so it takes the font of the box through a style rather than by inheritance.")]
        public void TheDocumentKeepsTheFontOfTheBox(string key)
        {
            var box = this.Show(key);

            box.SetCurrentValue(Control.FontSizeProperty, 23d);
            this.Settle();

            Assert.Multiple(() =>
                {
                    Assert.That(box.Document.FontSize, Is.EqualTo(23d), "the document should be set in the size the box is set in");
                    Assert.That(box.Document.FontFamily, Is.EqualTo(box.FontFamily), "and in the same family");
                });
        }

        [TestCase("MahApps.Styles.RichTextBox")]
        [TestCase("MahApps.Styles.RichTextBox.Win10")]
        [TestCase("MahApps.Styles.RichTextBox.WinUI")]
        [Description("A link in the document is underlined and painted the way the theme paints text.")]
        public void AHyperlinkKeepsTheLookOfTheTheme(string key)
        {
            var link = new Hyperlink(new Run("Beam me up..."));

            var box = this.Show(key);
            // a link in a box that is being written in is no link yet, and the style paints that one grey
            box.IsReadOnly = true;
            box.Document.Blocks.Clear();
            box.Document.Blocks.Add(new Paragraph(link));
            this.Settle();

            Assume.That(link.IsEnabled, Is.True);

            Assert.Multiple(() =>
                {
                    Assert.That(link.Foreground, Is.SameAs(box.FindResource("MahApps.Brushes.Text")), "the link should take the colour of the text");
                    Assert.That(link.ForceCursor, Is.True, "and say so with the cursor");
                });
        }

        [TestCase("MahApps.Styles.RichTextBox.Win10")]
        [TestCase("MahApps.Styles.RichTextBox.WinUI")]
        [Description("The delete button of a UWP box is there for as long as the caret is in the box.")]
        public void TheDeleteButtonWaitsForTheCaret(string key)
        {
            var box = this.Show(key);
            TextBoxHelper.SetClearTextButton(box, true);
            this.Settle();

            var button = box.FindChild<Button>("PART_ClearText");
            Assert.That(button, Is.Not.Null, "the template should carry the clear button");

            Assert.That(button!.Visibility, Is.Not.EqualTo(Visibility.Visible), "a box nobody is writing in should not offer to clear itself");

            box.Focus();
            Keyboard.Focus(box);
            this.Settle();

            // a build agent hands the keyboard to one window at a time
            Assume.That(box.IsKeyboardFocusWithin, Is.True);

            Assert.That(button.Visibility, Is.EqualTo(Visibility.Visible), "and the button should be there once the caret is");
        }

        private RichTextBox Show(string key)
        {
            Assert.That(this.window, Is.Not.Null);

            var box = new RichTextBox
                      {
                          Style = (Style)Application.Current.FindResource(key),
                          Width = 280
                      };

            box.Document.Blocks.Clear();
            box.Document.Blocks.Add(new Paragraph(new Run("Beam me up...")));

            this.window!.Content = box;
            this.Settle();

            return box;
        }

        private void Settle()
        {
            this.window!.UpdateLayout();
            ClipAssert.Pump();
        }
    }
}
