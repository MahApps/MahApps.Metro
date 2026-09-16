// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using MahApps.Metro.Tests.TestHelpers;
using MahApps.Metro.Tests.Views;
using NUnit.Framework;

namespace MahApps.Metro.Tests.Tests
{
    /// <summary>
    /// GH-4544: the message of a dialog can be written in a colour of its own, which until now took
    /// the foreground of the whole dialog and coloured its title along with it.
    /// </summary>
    [TestFixture]
    public class DialogMessageForegroundTests
    {
        private static readonly Color Warning = Colors.Red;

        private static MetroDialogSettings Settings(Brush? messageForeground)
        {
            return new MetroDialogSettings
                   {
                       AnimateShow = false,
                       AnimateHide = false,
                       MessageForeground = messageForeground
                   };
        }

        private static async Task<TDialog> WaitFor<TDialog>(MetroWindow window)
            where TDialog : BaseMetroDialog
        {
            TDialog? dialog = null;

            for (var turn = 0; turn < 100 && dialog is null; turn++)
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

        private static Color ColourOfTheMessage(BaseMetroDialog dialog)
        {
            var message = dialog.FindChild<TextBlock>("PART_MessageTextBlock");
            Assert.That(message, Is.Not.Null, "the template should carry the message");
            Assert.That(message!.Foreground, Is.InstanceOf<SolidColorBrush>(), "written in a plain colour");

            return ((SolidColorBrush)message.Foreground).Color;
        }

        private static Color ColourOfTheDialog(BaseMetroDialog dialog)
        {
            Assert.That(dialog.Foreground, Is.InstanceOf<SolidColorBrush>());
            return ((SolidColorBrush)dialog.Foreground).Color;
        }

        [Test]
        [Description("A message dialog writes its message in the colour it was given, and nothing else in it changes.")]
        public async Task TheMessageDialogWritesItsMessageInTheColourItWasGiven()
        {
            var window = await WindowHelpers.CreateInvisibleWindowAsync<DialogWindow>();

            try
            {
                _ = window.ShowMessageAsync("Title", "Message", MessageDialogStyle.Affirmative, Settings(new SolidColorBrush(Warning)));
                var dialog = await WaitFor<MessageDialog>(window);

                Assert.That(ColourOfTheMessage(dialog), Is.EqualTo(Warning));
                Assert.That(ColourOfTheDialog(dialog), Is.Not.EqualTo(Warning), "the rest of the dialog is left alone");

                var title = dialog.FindChild<TextBlock>("PART_Title");
                Assert.That(((SolidColorBrush)title!.Foreground).Color, Is.Not.EqualTo(Warning), "and so is the title");
            }
            finally
            {
                window.Close();
            }
        }

        [Test]
        [Description("So does the one that asks for a line of text.")]
        public async Task TheInputDialogWritesItsMessageInTheColourItWasGiven()
        {
            var window = await WindowHelpers.CreateInvisibleWindowAsync<DialogWindow>();

            try
            {
                _ = window.ShowInputAsync("Title", "Message", Settings(new SolidColorBrush(Warning)));
                var dialog = await WaitFor<InputDialog>(window);

                Assert.That(ColourOfTheMessage(dialog), Is.EqualTo(Warning));
                Assert.That(ColourOfTheDialog(dialog), Is.Not.EqualTo(Warning), "the rest of the dialog is left alone");
            }
            finally
            {
                window.Close();
            }
        }

        [Test]
        [Description("And the one that asks who is there.")]
        public async Task TheLoginDialogWritesItsMessageInTheColourItWasGiven()
        {
            var window = await WindowHelpers.CreateInvisibleWindowAsync<DialogWindow>();

            try
            {
                _ = window.ShowLoginAsync("Title", "Message", new LoginDialogSettings(Settings(new SolidColorBrush(Warning))));
                var dialog = await WaitFor<LoginDialog>(window);

                Assert.That(ColourOfTheMessage(dialog), Is.EqualTo(Warning));
            }
            finally
            {
                window.Close();
            }
        }

        [Test]
        [Description("And the one that says how far along something is.")]
        public async Task TheProgressDialogWritesItsMessageInTheColourItWasGiven()
        {
            var window = await WindowHelpers.CreateInvisibleWindowAsync<DialogWindow>();

            try
            {
                var controller = await window.ShowProgressAsync("Title", "Message", settings: Settings(new SolidColorBrush(Warning)));
                var dialog = await WaitFor<ProgressDialog>(window);

                Assert.That(ColourOfTheMessage(dialog), Is.EqualTo(Warning));

                await controller.CloseAsync();
            }
            finally
            {
                window.Close();
            }
        }

        [Test]
        [Description("Told no colour, the message is written along with the rest of the dialog, the way it always was.")]
        public async Task WithoutAColourTheMessageGoesWithTheDialog()
        {
            var window = await WindowHelpers.CreateInvisibleWindowAsync<DialogWindow>();

            try
            {
                _ = window.ShowMessageAsync("Title", "Message", MessageDialogStyle.Affirmative, Settings(null));
                var dialog = await WaitFor<MessageDialog>(window);

                Assert.That(ColourOfTheMessage(dialog), Is.EqualTo(ColourOfTheDialog(dialog)));
            }
            finally
            {
                window.Close();
            }
        }

        [Test]
        [Description("Plain settings carry no colour either, which is what every caller from before passes.")]
        public async Task PlainSettingsLeaveTheMessageWhereItWas()
        {
            var window = await WindowHelpers.CreateInvisibleWindowAsync<DialogWindow>();

            try
            {
                _ = window.ShowMessageAsync("Title", "Message", MessageDialogStyle.Affirmative, new MetroDialogSettings { AnimateShow = false, AnimateHide = false });
                var dialog = await WaitFor<MessageDialog>(window);

                Assert.That(ColourOfTheMessage(dialog), Is.EqualTo(ColourOfTheDialog(dialog)));
            }
            finally
            {
                window.Close();
            }
        }
    }
}
