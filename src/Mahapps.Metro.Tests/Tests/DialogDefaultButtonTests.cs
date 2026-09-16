// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using MahApps.Metro.Tests.TestHelpers;
using MahApps.Metro.Tests.Views;
using NUnit.Framework;

namespace MahApps.Metro.Tests.Tests
{
    /// <summary>
    /// GH-4358: the settings say which button a press of return stands for, and that one is marked.
    /// The dialog that asks for a line of text marked the one that carries on whatever the settings
    /// said. Nothing said at all leaves each dialog with the button it has always marked.
    /// </summary>
    [TestFixture]
    public class DialogDefaultButtonTests
    {
        private static async Task<TDialog> WaitFor<TDialog>(MetroWindow window)
            where TDialog : BaseMetroDialog
        {
            TDialog? dialog = null;

            for (var i = 0; i < 100 && dialog is null; i++)
            {
                dialog = await window.GetCurrentDialogAsync<TDialog>();

                if (dialog is null)
                {
                    await Task.Delay(20);
                }
            }

            Assert.That(dialog, Is.Not.Null, $"the {typeof(TDialog).Name} should be up");
            ClipAssert.Pump();

            return dialog!;
        }

        private static Style? Accent(FrameworkElement dialog)
        {
            return dialog.TryFindResource("MahApps.Styles.Button.Dialogs.Accent") as Style;
        }

        private static void Marked(FrameworkElement dialog, string part)
        {
            var other = part == "PART_AffirmativeButton" ? "PART_NegativeButton" : "PART_AffirmativeButton";

            var marked = dialog.FindChild<Button>(part);
            var plain = dialog.FindChild<Button>(other);

            Assert.That(marked, Is.Not.Null, $"the template should carry {part}");
            Assert.That(marked!.Style, Is.SameAs(Accent(dialog)), $"{part} should be the marked one");
            Assert.That(plain?.Style, Is.Not.SameAs(Accent(dialog)), $"and {other} should not");
        }

        [Test]
        [Description("Asked to mark the one that says no, the input dialog marks that one.")]
        public async Task TheInputDialogMarksTheButtonItWasTold()
        {
            var window = await WindowHelpers.CreateInvisibleWindowAsync<DialogWindow>();

            try
            {
                _ = window.ShowInputAsync("Title", "Message", new MetroDialogSettings { DefaultButtonFocus = MessageDialogResult.Negative });

                Marked(await WaitFor<InputDialog>(window), "PART_NegativeButton");
            }
            finally
            {
                window.Close();
            }
        }

        [Test]
        [Description("Told nothing, it marks the one that carries on, the way it always has.")]
        public async Task TheInputDialogMarksTheAffirmativeButtonByItself()
        {
            var window = await WindowHelpers.CreateInvisibleWindowAsync<DialogWindow>();

            try
            {
                _ = window.ShowInputAsync("Title", "Message");

                Marked(await WaitFor<InputDialog>(window), "PART_AffirmativeButton");
            }
            finally
            {
                window.Close();
            }
        }

        [Test]
        [Description("The dialog that asks who you are follows the same settings.")]
        public async Task TheLoginDialogMarksTheButtonItWasTold()
        {
            var window = await WindowHelpers.CreateInvisibleWindowAsync<DialogWindow>();

            try
            {
                _ = window.ShowLoginAsync("Title", "Message", new LoginDialogSettings { DefaultButtonFocus = MessageDialogResult.Negative, NegativeButtonVisibility = Visibility.Visible });

                Marked(await WaitFor<LoginDialog>(window), "PART_NegativeButton");
            }
            finally
            {
                window.Close();
            }
        }

        [Test]
        [Description("And told nothing, it marks the one that logs you in.")]
        public async Task TheLoginDialogMarksTheAffirmativeButtonByItself()
        {
            var window = await WindowHelpers.CreateInvisibleWindowAsync<DialogWindow>();

            try
            {
                _ = window.ShowLoginAsync("Title", "Message", new LoginDialogSettings { NegativeButtonVisibility = Visibility.Visible });

                Marked(await WaitFor<LoginDialog>(window), "PART_AffirmativeButton");
            }
            finally
            {
                window.Close();
            }
        }

        [Test]
        [Description("The message dialog is left as it was: told nothing, the one that says no is marked.")]
        public async Task TheMessageDialogIsLeftAsItWas()
        {
            var window = await WindowHelpers.CreateInvisibleWindowAsync<DialogWindow>();

            try
            {
                _ = window.ShowMessageAsync("Title", "Message", MessageDialogStyle.AffirmativeAndNegative);

                Marked(await WaitFor<MessageDialog>(window), "PART_NegativeButton");
            }
            finally
            {
                window.Close();
            }
        }

        [Test]
        [Description("Whatever is marked, the caret waits in the box, because that is what somebody has come to fill in.")]
        public async Task TheCaretStaysInTheBox()
        {
            var window = await WindowHelpers.CreateInvisibleWindowAsync<DialogWindow>();

            try
            {
                _ = window.ShowInputAsync("Title", "Message", new MetroDialogSettings { DefaultButtonFocus = MessageDialogResult.Negative });

                var dialog = await WaitFor<InputDialog>(window);

                Assert.That(dialog.FindChild<TextBox>("PART_TextBox")?.IsFocused, Is.True);
            }
            finally
            {
                window.Close();
            }
        }
    }
}
