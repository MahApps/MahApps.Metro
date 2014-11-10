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
        public async Task ReceivesDataContext()
        {
            await TestHost.SwitchToAppThread();

            var window = await WindowHelpers.CreateInvisibleWindowAsync<DialogWindow>();
            var vm = new TheViewModel();
            var dialog = (CustomDialog) window.Resources["CustomDialog"];

            await window.ShowMetroDialogAsync(dialog);

            await TestHost.SwitchToAppThread(); // No idea why we have to do this again

            dialog.DataContext = vm;
            var textBlock = dialog.FindChild<TextBlock>("TheDialogBody");

            Assert.Equal(vm.Text, textBlock.Text);
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
