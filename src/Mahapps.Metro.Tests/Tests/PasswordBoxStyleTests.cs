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
    /// The Windows 10 and the WinUI password box carry the eye a UWP box carries, and UWP shows it
    /// while the caret is in the box and something is written in it. The delete button, which is the
    /// library's own, answers to the same rule, so a box nobody is writing in shows neither of them.
    /// </summary>
    [TestFixture]
    public class PasswordBoxStyleTests
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
            this.window?.Resources.Remove("MahApps.Styles.Button.TextControl.Reveal");
        }

        [TestCase("MahApps.Styles.PasswordBox.Win10")]
        [TestCase("MahApps.Styles.PasswordBox.WinUI")]
        [Description("The eye is there for as long as the caret is in the box.")]
        public void TheEyeWaitsForTheCaret(string key)
        {
            var box = this.Show(key, "42 lets you in");
            var eye = Eye(box);

            Assert.That(eye.Visibility, Is.Not.EqualTo(Visibility.Visible), "a box nobody is writing in should not offer to show the password");

            box.Focus();
            Keyboard.Focus(box);
            this.Settle();

            // a build agent hands the keyboard to one window at a time
            Assume.That(box.IsKeyboardFocused, Is.True);

            Assert.That(eye.Visibility, Is.EqualTo(Visibility.Visible), "and the eye should be there once the caret is");
        }

        [TestCase("MahApps.Styles.PasswordBox.Win10")]
        [TestCase("MahApps.Styles.PasswordBox.WinUI")]
        [Description("An empty box has nothing to show, caret or no caret.")]
        public void AnEmptyBoxKeepsTheEyeAway(string key)
        {
            var box = this.Show(key, string.Empty);

            box.Focus();
            Keyboard.Focus(box);
            this.Settle();

            Assume.That(box.IsKeyboardFocused, Is.True);

            Assert.That(Eye(box).Visibility, Is.Not.EqualTo(Visibility.Visible));
        }

        [TestCase("MahApps.Styles.PasswordBox.Win10")]
        [TestCase("MahApps.Styles.PasswordBox.WinUI")]
        [Description("The eye wears a key of its own, so an application can say something about it without saying it about every delete button as well.")]
        public void AStyleFromOutsideReachesTheEye(string key)
        {
            var mine = new Style(typeof(Button));
            mine.Setters.Add(new Setter(Control.PaddingProperty, new Thickness(7)));
            mine.Seal();

            this.window!.Resources["MahApps.Styles.Button.TextControl.Reveal"] = mine;

            var box = this.Show(key, "42 lets you in");
            var eye = Eye(box);

            Assert.Multiple(() =>
                {
                    Assert.That(eye.Style, Is.SameAs(mine), "the eye should wear the style the application put under that key");
                    Assert.That(eye.Padding, Is.EqualTo(new Thickness(7)), "and with it what the style has to say");
                });
        }

        [TestCase("MahApps.Styles.PasswordBox.Win10")]
        [TestCase("MahApps.Styles.PasswordBox.WinUI")]
        [Description("UWP draws one button for both jobs, so the eye and the delete button wear the chrome the box hands out.")]
        public void BothButtonsWearTheChromeOfTheBox(string key)
        {
            var box = this.Show(key, "42 lets you in");

            var chrome = TextBoxHelper.GetButtonTemplate(box);

            Assert.That(chrome, Is.Not.Null, "the style should hand the buttons a template");
            Assert.Multiple(() =>
                {
                    Assert.That(Eye(box).Template, Is.SameAs(chrome), "the eye");
                    Assert.That(box.FindChild<Button>("PART_ClearText")?.Template, Is.SameAs(chrome), "the delete button");
                });
        }

        private static Button Eye(PasswordBox box)
        {
            var eye = box.FindChild<Button>("PART_RevealButton");
            Assert.That(eye, Is.Not.Null, "the template should carry the eye");

            return eye!;
        }

        private PasswordBox Show(string key, string password)
        {
            Assert.That(this.window, Is.Not.Null);

            var box = new PasswordBox
                      {
                          Style = (Style)Application.Current.FindResource(key),
                          Width = 280,
                          Password = password
                      };

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
