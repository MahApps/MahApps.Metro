using System.Threading.Tasks;
using System.Windows.Controls;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using MahApps.Metro.Tests.TestHelpers;
using Xunit;

namespace MahApps.Metro.Tests
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
            var dialog = (CustomDialog) window.Resources["CustomDialog"];

            await window.ShowMetroDialogAsync(dialog);

            await TestHost.SwitchToAppThread(); // No idea why we have to do this again

            Assert.Equal(await window.GetCurrentDialogAsync<CustomDialog>(), dialog);
            Assert.NotNull(await window.GetCurrentDialogAsync<BaseMetroDialog>());
            Assert.Null(await window.GetCurrentDialogAsync<MessageDialog>());

            dialog.DataContext = vm;
            var textBlock = dialog.FindChild<TextBlock>("TheDialogBody");

            Assert.Equal(vm.Text, textBlock.Text);

            await window.HideMetroDialogAsync(dialog);

            await TestHost.SwitchToAppThread(); // No idea why we have to do this again

            Assert.Null(await window.GetCurrentDialogAsync<MessageDialog>());
        }

        private class TheViewModel
        {
            public string Text
            {
                get { return "TheText"; }
            }
        }
    }
}
