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
using Xunit;

namespace MahApps.Metro.Tests.Tests
{
    public class CustomDialogTest : AutomationTestBase
    {
        [Fact]
        [DisplayTestMethodName]
        public async Task ReceivesDataContext()
        {
            await TestHost.SwitchToAppThread();

            var window = await WindowHelpers.CreateInvisibleWindowAsync<DialogWindow>();
            var vm = new TheViewModel();
            var dialog = (CustomDialog)window.Resources["CustomDialog"];

            await window.ShowMetroDialogAsync(dialog);

            Assert.Equal(await window.GetCurrentDialogAsync<CustomDialog>(), dialog);
            Assert.NotNull(await window.GetCurrentDialogAsync<BaseMetroDialog>());
            Assert.Null(await window.GetCurrentDialogAsync<MessageDialog>());

            dialog.DataContext = vm;
            var bodyTextBlock = dialog.FindChild<TextBlock>("TheDialogBody");

            Assert.Equal(vm.Text, bodyTextBlock.Text);

            var title = dialog.FindChild<ContentPresenter>("PART_Title");

            Assert.Equal(vm.Title, title.Content);

            await window.HideMetroDialogAsync(dialog);

            Assert.Null(await window.GetCurrentDialogAsync<MessageDialog>());
        }

        [Fact]
        [DisplayTestMethodName]
        public async Task DisplaysTitleWithCustomContent()
        {
            await TestHost.SwitchToAppThread();

            var window = await WindowHelpers.CreateInvisibleWindowAsync<DialogCustomTitleWindow>();
            var vm = new TheViewModel();
            var dialog = (CustomDialog)window.Resources["CustomDialog"];

            await window.ShowMetroDialogAsync(dialog);

            Assert.Equal(await window.GetCurrentDialogAsync<CustomDialog>(), dialog);
            Assert.NotNull(await window.GetCurrentDialogAsync<BaseMetroDialog>());
            Assert.Null(await window.GetCurrentDialogAsync<MessageDialog>());

            dialog.DataContext = vm;

            var titleContainer = dialog.FindChild<DockPanel>("TheDialogTitle");

            var rects = titleContainer.FindChildren<System.Windows.Shapes.Rectangle>().ToList();
            Assert.Equal(2, rects.Count);
            TextBlock text = titleContainer.FindChild<TextBlock>();
            Assert.Equal(vm.Title, text.Text);
            
            await window.HideMetroDialogAsync(dialog);

            Assert.Null(await window.GetCurrentDialogAsync<MessageDialog>());
        }

        private class TheViewModel
        {
            public string Text => "TheText";
            public string Title => "TheTitle";
        }
    }
}