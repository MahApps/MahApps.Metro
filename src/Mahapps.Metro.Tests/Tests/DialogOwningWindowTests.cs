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
    public class DialogOwningWindowTests
    {
        [Test]
        public async Task OwningWindowShouldBeTheWindowThatShowsTheDialog()
        {
            var window = await WindowHelpers.CreateInvisibleWindowAsync<DialogWindow>();
            var dialog = new OwningWindowProbeDialog();

            Assert.That(dialog.Owner, Is.Null, "a dialog nobody has shown yet has no window");

            try
            {
                await window.ShowMetroDialogAsync(dialog);

                Assert.That(dialog.Owner, Is.SameAs(window), "showing a dialog should tell it which window it is on");

                await window.HideMetroDialogAsync(dialog);
            }
            finally
            {
                window.Close();
            }
        }

        [Test]
        public async Task OwningWindowShouldFollowTheDialogToAnotherWindow()
        {
            var first = await WindowHelpers.CreateInvisibleWindowAsync<DialogWindow>();
            var second = await WindowHelpers.CreateInvisibleWindowAsync<DialogWindow>();
            var dialog = new OwningWindowProbeDialog();

            try
            {
                await first.ShowMetroDialogAsync(dialog);
                await first.HideMetroDialogAsync(dialog);

                await second.ShowMetroDialogAsync(dialog);

                Assert.That(dialog.Owner, Is.SameAs(second), "the window that shows the dialog now is the one it belongs to");

                await second.HideMetroDialogAsync(dialog);
            }
            finally
            {
                first.Close();
                second.Close();
            }
        }

        [Test]
        public async Task OwningWindowShouldStillComeFromTheConstructor()
        {
            var window = await WindowHelpers.CreateInvisibleWindowAsync<DialogWindow>();
            var dialog = new OwningWindowProbeDialog(window, null);

            try
            {
                Assert.That(dialog.Owner, Is.SameAs(window), "a dialog built with its window knows it before it is shown");

                await window.ShowMetroDialogAsync(dialog);

                Assert.That(dialog.Owner, Is.SameAs(window));

                await window.HideMetroDialogAsync(dialog);
            }
            finally
            {
                window.Close();
            }
        }

        [Test]
        public async Task ADialogDeclaredInXamlShouldGetItsWindowToo()
        {
            var window = await WindowHelpers.CreateInvisibleWindowAsync<DialogWindow>();
            var dialog = (OwningWindowProbeDialog)window.Resources["ProbeDialog"];

            Assert.That(dialog.Owner, Is.Null, "markup builds the dialog with its parameterless constructor, so it starts without a window");

            try
            {
                await window.ShowMetroDialogAsync(dialog);

                Assert.That(dialog.Owner, Is.SameAs(window), "a dialog written in XAML should reach its window like any other");

                await dialog.CloseItselfAsync();

                Assert.That(await window.GetCurrentDialogAsync<OwningWindowProbeDialog>(), Is.Null);
            }
            finally
            {
                window.Close();
            }
        }

        [Test]
        public async Task ADialogShouldBeAbleToCloseItself()
        {
            var window = await WindowHelpers.CreateInvisibleWindowAsync<DialogWindow>();
            var dialog = new OwningWindowProbeDialog();

            try
            {
                await window.ShowMetroDialogAsync(dialog);

                await dialog.CloseItselfAsync();

                Assert.That(await window.GetCurrentDialogAsync<OwningWindowProbeDialog>(), Is.Null, "a dialog should be able to close itself through its own window");
            }
            finally
            {
                window.Close();
            }
        }
    }
}
