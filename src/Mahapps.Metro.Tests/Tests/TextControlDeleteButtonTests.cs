// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using MahApps.Metro.Controls;
using MahApps.Metro.Tests.TestHelpers;
using NUnit.Framework;

namespace MahApps.Metro.Tests.Tests
{
    /// <summary>
    /// The delete button of the Win10 and the WinUI box wears MahApps.Styles.Button.TextControl.Delete,
    /// text box and password box alike, and a key that reads like one of the library's own has to be
    /// one: the style used to sit in the resources of the template itself, where the lookup found it
    /// first and an application had no way of saying anything about that button.
    /// </summary>
    [TestFixture]
    public class TextControlDeleteButtonTests
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
            this.window?.Resources.Remove("MahApps.Styles.Button.TextControl.Delete");
        }

        [TestCase("MahApps.Styles.TextBox.Win10")]
        [TestCase("MahApps.Styles.TextBox.WinUI")]
        [TestCase("MahApps.Styles.PasswordBox.Win10")]
        [TestCase("MahApps.Styles.PasswordBox.WinUI")]
        [Description("The glyph takes the colour of the text in the box, which is what makes it visible on a box that has not turned white yet.")]
        public void TheGlyphFollowsTheTextOfTheBox(string key)
        {
            var box = this.Show(key);

            box.SetCurrentValue(Control.ForegroundProperty, Brushes.Red);
            this.Settle();

            var button = Button(box);

            Assert.That(((SolidColorBrush)button.Foreground).Color, Is.EqualTo(Colors.Red), "the glyph should be painted in the colour the box paints its text");
        }

        [TestCase("MahApps.Styles.TextBox.Win10")]
        [TestCase("MahApps.Styles.TextBox.WinUI")]
        [TestCase("MahApps.Styles.PasswordBox.Win10")]
        [TestCase("MahApps.Styles.PasswordBox.WinUI")]
        [Description("And an application can say something about that button, which a style inside the template would not let it.")]
        public void AStyleFromOutsideReachesTheButton(string key)
        {
            var mine = new Style(typeof(Button));
            mine.Setters.Add(new Setter(Control.PaddingProperty, new Thickness(7)));
            mine.Seal();

            this.window!.Resources["MahApps.Styles.Button.TextControl.Delete"] = mine;

            var box = this.Show(key);
            var button = Button(box);

            Assert.That(button.Style, Is.SameAs(mine), "the button should wear the style the application put under that key");
            Assert.That(button.Padding, Is.EqualTo(new Thickness(7)), "and with it what the style has to say");
        }

        [TestCase("MahApps.Styles.TextBox.Win10")]
        [TestCase("MahApps.Styles.TextBox.WinUI")]
        [TestCase("MahApps.Styles.PasswordBox.Win10")]
        [TestCase("MahApps.Styles.PasswordBox.WinUI")]
        [Description("The style is built on the chromeless button, which carries a template of its own, and the one the box hands over is the one that has to win.")]
        public void TheButtonKeepsTheTemplateTheBoxHandsIt(string key)
        {
            var box = this.Show(key);
            var button = Button(box);

            Assert.That(TextBoxHelper.GetButtonTemplate(box), Is.Not.Null, "the style should hand the button a template");
            Assert.That(button.Template, Is.SameAs(TextBoxHelper.GetButtonTemplate(box)), "and that is the template the button should wear");
        }

        private static Button Button(Control box)
        {
            var button = box.FindChild<Button>("PART_ClearText");
            Assert.That(button, Is.Not.Null, "the template should carry the clear button");

            return button!;
        }

        /// <summary>
        /// The password box of those two sets carries the same button, so the key says which of the
        /// two controls the style belongs to.
        /// </summary>
        private Control Show(string key)
        {
            Assert.That(this.window, Is.Not.Null);

            Control box = key.Contains("PasswordBox")
                ? new PasswordBox { Password = "Konrad" }
                : new TextBox { Text = "Konrad" };

            box.Style = (Style)Application.Current.FindResource(key);
            box.Width = 200;

            TextBoxHelper.SetClearTextButton(box, true);

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
