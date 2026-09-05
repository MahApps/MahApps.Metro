// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Threading.Tasks;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using MahApps.Metro.Tests.TestHelpers;
using MahApps.Metro.Tests.Views;
using NUnit.Framework;

namespace MahApps.Metro.Tests.Tests
{
    [TestFixture]
    public class LoginDialogTests
    {
        private static MetroDialogSettings WindowOptions()
        {
            return new MetroDialogSettings
                   {
                       ColorScheme = MetroDialogColorScheme.Accented,
                       NegativeButtonText = "No",
                       DialogTitleFontSize = 42d
                   };
        }

        /// <summary>
        /// The dialog needs a moment to load before it is the current one, so give it that moment.
        /// </summary>
        private static async Task<LoginDialog> WaitForLoginDialogAsync(MetroWindow window)
        {
            LoginDialog? dialog = null;

            for (var i = 0; i < 100 && dialog is null; i++)
            {
                dialog = await window.GetCurrentDialogAsync<LoginDialog>();
                if (dialog is null)
                {
                    await Task.Delay(20);
                }
            }

            Assert.That(dialog, Is.Not.Null, "the login dialog should be up");

            return dialog!;
        }

        [Test]
        public async Task LoginDialogShouldTakeTheDialogOptionsOfTheWindow()
        {
            var window = await WindowHelpers.CreateInvisibleWindowAsync<DialogWindow>();
            window.SetCurrentValue(MetroWindow.MetroDialogOptionsProperty, WindowOptions());

            try
            {
                _ = window.ShowLoginAsync("Title", "Message");

                var dialog = await WaitForLoginDialogAsync(window);

                Assert.That(dialog.DialogSettings.ColorScheme, Is.EqualTo(MetroDialogColorScheme.Accented), "the login dialog should look like every other dialog of the window");
                Assert.That(dialog.DialogSettings.NegativeButtonText, Is.EqualTo("No"));
                Assert.That(dialog.DialogSettings.DialogTitleFontSize, Is.EqualTo(42d));

                await window.HideMetroDialogAsync(dialog);
            }
            finally
            {
                window.Close();
            }
        }

        [Test]
        public async Task LoginDialogShouldKeepItsOwnDefaults()
        {
            var window = await WindowHelpers.CreateInvisibleWindowAsync<DialogWindow>();
            window.SetCurrentValue(MetroWindow.MetroDialogOptionsProperty, WindowOptions());

            try
            {
                _ = window.ShowLoginAsync("Title", "Message");

                var dialog = await WaitForLoginDialogAsync(window);

                Assert.That(dialog.DialogSettings, Is.InstanceOf<LoginDialogSettings>(), "the dialog needs the login settings, not the plain ones");
                Assert.That(((LoginDialogSettings)dialog.DialogSettings).UsernameWatermark, Is.EqualTo("Username..."), "what only a login dialog has should still come from its own defaults");

                await window.HideMetroDialogAsync(dialog);
            }
            finally
            {
                window.Close();
            }
        }

        [Test]
        public async Task SettingsPassedInShouldBeatTheDialogOptionsOfTheWindow()
        {
            var window = await WindowHelpers.CreateInvisibleWindowAsync<DialogWindow>();
            window.SetCurrentValue(MetroWindow.MetroDialogOptionsProperty, WindowOptions());

            try
            {
                _ = window.ShowLoginAsync("Title", "Message", new LoginDialogSettings { NegativeButtonText = "Nope" });

                var dialog = await WaitForLoginDialogAsync(window);

                Assert.That(dialog.DialogSettings.NegativeButtonText, Is.EqualTo("Nope"), "settings handed to the call should still win");

                await window.HideMetroDialogAsync(dialog);
            }
            finally
            {
                window.Close();
            }
        }
    }
}
