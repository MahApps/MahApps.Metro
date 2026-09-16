// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using MahApps.Metro.Tests.TestHelpers;
using MahApps.Metro.Tests.Views;
using NUnit.Framework;

namespace MahApps.Metro.Tests.Tests
{
    /// <summary>
    /// GH-4526: the dialog that asks for a line of text can be given a check, and a line it turns
    /// down keeps the dialog where it is instead of handing the caller something it cannot use.
    /// </summary>
    [TestFixture]
    public class InputDialogValidationTests
    {
        private const string TooShort = "at least three characters";

        private static InputDialogSettings Settings(System.Func<string?, string?>? check)
        {
            return new InputDialogSettings
                   {
                       // these tests close the dialogs they open, and a closing storyboard is a
                       // clock that does not always tick where these tests run
                       AnimateShow = false,
                       AnimateHide = false,
                       ValidateInput = check
                   };
        }

        private static string? ShorterThanThree(string? input)
        {
            return input is null || input.Length < 3 ? TooShort : null;
        }

        private static async Task<InputDialog> WaitForTheDialog(MetroWindow window)
        {
            InputDialog? dialog = null;

            for (var turn = 0; turn < 100 && dialog is null; turn++)
            {
                dialog = await window.GetCurrentDialogAsync<InputDialog>();

                if (dialog is null)
                {
                    await Task.Delay(20);
                }
            }

            Assert.That(dialog, Is.Not.Null, "the input dialog should be up");
            ClipAssert.Pump();

            return dialog!;
        }

        private static TextBox FieldOf(InputDialog dialog)
        {
            var field = dialog.FindChild<TextBox>("PART_TextBox");
            Assert.That(field, Is.Not.Null, "the template should carry the field");
            return field!;
        }

        private static void Press(InputDialog dialog, string part)
        {
            var button = dialog.FindChild<Button>(part);
            Assert.That(button, Is.Not.Null, $"the template should carry {part}");

            button!.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
            ClipAssert.Pump();
        }

        private static IEnumerable<string?> ComplaintsOn(TextBox field)
        {
            return Validation.GetErrors(field).Select(error => error.ErrorContent as string);
        }


        [Test]
        [Description("What the check said goes as soon as the line it was said about is typed over.")]
        public async Task TypingOnTakesTheComplaintOffAgain()
        {
            var window = await WindowHelpers.CreateInvisibleWindowAsync<DialogWindow>();

            try
            {
                var asked = window.ShowInputAsync("Title", "Message", Settings(ShorterThanThree));
                var dialog = await WaitForTheDialog(window);
                var field = FieldOf(dialog);

                field.SetCurrentValue(TextBox.TextProperty, "no");
                Press(dialog, "PART_AffirmativeButton");

                Assert.That(Validation.GetHasError(field), Is.True, "the field should be marked to begin with");

                field.SetCurrentValue(TextBox.TextProperty, "now it is long enough");
                ClipAssert.Pump();

                Assert.That(Validation.GetHasError(field), Is.False, "and clear again once it has been typed over");

                Press(dialog, "PART_NegativeButton");
                await asked;
            }
            finally
            {
                window.Close();
            }
        }

        [Test]
        [Description("A line the check turns down keeps the dialog where it is and says what is wrong with it.")]
        public async Task ATurnedDownInputKeepsTheDialogOpen()
        {
            var window = await WindowHelpers.CreateInvisibleWindowAsync<DialogWindow>();

            try
            {
                var asked = window.ShowInputAsync("Title", "Message", Settings(ShorterThanThree));
                var dialog = await WaitForTheDialog(window);
                var field = FieldOf(dialog);

                field.SetCurrentValue(TextBox.TextProperty, "no");
                Press(dialog, "PART_AffirmativeButton");

                Assert.That(asked.IsCompleted, Is.False, "the dialog should still be asking");
                Assert.That(Validation.GetHasError(field), Is.True, "and the field should be marked");
                Assert.That(ComplaintsOn(field), Has.Member(TooShort), "with what the check said about it");

                // the theme shows what is wrong with a field while the caret is in it, and the caret
                // is on the button that was just pressed, so the dialog has to hand it back
                Assert.That(field.IsFocused, Is.True, "and the caret should be back in the field");

                // and it lets go once the line is one the check accepts
                field.SetCurrentValue(TextBox.TextProperty, "yes it is");
                Press(dialog, "PART_AffirmativeButton");

                Assert.That(await asked, Is.EqualTo("yes it is"));
            }
            finally
            {
                window.Close();
            }
        }

        [Test]
        [Description("Giving up on the dialog is nobody's business but the caller's, so the check is not asked.")]
        public async Task TheCheckDoesNotHoldOnToAnybodyLeaving()
        {
            var window = await WindowHelpers.CreateInvisibleWindowAsync<DialogWindow>();

            try
            {
                var asked = window.ShowInputAsync("Title", "Message", Settings(ShorterThanThree));
                var dialog = await WaitForTheDialog(window);

                FieldOf(dialog).SetCurrentValue(TextBox.TextProperty, "no");
                Press(dialog, "PART_NegativeButton");

                Assert.That(await asked, Is.Null, "leaving should come back with nothing");
            }
            finally
            {
                window.Close();
            }
        }

        [Test]
        [Description("A dialog nobody gave a check to takes whatever is typed, the way it always has.")]
        public async Task WithoutACheckAnythingGoes()
        {
            var window = await WindowHelpers.CreateInvisibleWindowAsync<DialogWindow>();

            try
            {
                var asked = window.ShowInputAsync("Title", "Message", Settings(null));
                var dialog = await WaitForTheDialog(window);

                FieldOf(dialog).SetCurrentValue(TextBox.TextProperty, "no");
                Press(dialog, "PART_AffirmativeButton");

                Assert.That(await asked, Is.EqualTo("no"));
            }
            finally
            {
                window.Close();
            }
        }

        [Test]
        [Description("Plain settings carry no check either, which is what every caller from before passes.")]
        public async Task PlainSettingsTakeWhateverIsTyped()
        {
            var window = await WindowHelpers.CreateInvisibleWindowAsync<DialogWindow>();

            try
            {
                var asked = window.ShowInputAsync("Title", "Message", new MetroDialogSettings { AnimateShow = false, AnimateHide = false });
                var dialog = await WaitForTheDialog(window);

                FieldOf(dialog).SetCurrentValue(TextBox.TextProperty, "no");
                Press(dialog, "PART_AffirmativeButton");

                Assert.That(await asked, Is.EqualTo("no"));
            }
            finally
            {
                window.Close();
            }
        }
    }
}
