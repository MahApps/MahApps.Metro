// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Linq;
using System.Threading.Tasks;
using System.Windows.Controls;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using MahApps.Metro.Tests.TestHelpers;
using MahApps.Metro.Tests.Views;
using NUnit.Framework;

namespace MahApps.Metro.Tests.Tests
{
    [TestFixture]
    public class CustomDialogTest
    {
        [Test]
        public async Task ReceivesDataContext()
        {
            var window = await WindowHelpers.CreateInvisibleWindowAsync<DialogWindow>();
            var vm = new TheViewModel();
            var dialog = (CustomDialog)window.Resources["CustomDialog"];

            await window.ShowMetroDialogAsync(dialog);

            Assert.That(dialog, Is.EqualTo(await window.GetCurrentDialogAsync<CustomDialog>()));
            Assert.That(await window.GetCurrentDialogAsync<BaseMetroDialog>(), Is.Not.Null);
            Assert.That(await window.GetCurrentDialogAsync<MessageDialog>(), Is.Null);

            dialog.DataContext = vm;
            var bodyTextBlock = dialog.FindChild<TextBlock>("TheDialogBody");
            Assert.That(bodyTextBlock, Is.Not.Null);
            Assert.That(bodyTextBlock.Text, Is.EqualTo(vm.Text));

            var title = dialog.FindChild<ContentPresenter>("PART_Title");
            Assert.That(title, Is.Not.Null);
            Assert.That(title.Content, Is.EqualTo(vm.Title));

            await window.HideMetroDialogAsync(dialog);

            Assert.That(await window.GetCurrentDialogAsync<MessageDialog>(), Is.Null);

            window.Close();
        }

        [Test]
        public async Task DisplaysTitleWithCustomContent()
        {
            var window = await WindowHelpers.CreateInvisibleWindowAsync<DialogCustomTitleWindow>();
            var vm = new TheViewModel();
            var dialog = (CustomDialog)window.Resources["CustomDialog"];

            await window.ShowMetroDialogAsync(dialog);

            Assert.That(dialog, Is.EqualTo(await window.GetCurrentDialogAsync<CustomDialog>()));
            Assert.That(await window.GetCurrentDialogAsync<BaseMetroDialog>(), Is.Not.Null);
            Assert.That(await window.GetCurrentDialogAsync<MessageDialog>(), Is.Null);

            dialog.DataContext = vm;

            var titleContainer = dialog.FindChild<DockPanel>("TheDialogTitle");

            var rects = titleContainer.FindChildren<System.Windows.Shapes.Rectangle>().ToList();
            Assert.That(rects.Count, Is.EqualTo(2));
            TextBlock text = titleContainer.FindChild<TextBlock>();
            Assert.That(text, Is.Not.Null);
            Assert.That(text.Text, Is.EqualTo(vm.Title));

            await window.HideMetroDialogAsync(dialog);

            Assert.That(await window.GetCurrentDialogAsync<MessageDialog>(), Is.Null);

            window.Close();
        }

        private class TheViewModel
        {
            public string Text => "TheText";

            public string Title => "TheTitle";
        }
    }
}