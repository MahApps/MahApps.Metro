using System.Threading.Tasks;
using System.Windows.Controls;
using MahApps.Metro.Controls;
using MahApps.Metro.Tests.TestHelpers;
using Xunit;

namespace MahApps.Metro.Tests
{
    public class ButtonTest : AutomationTestBase
    {
        [Fact]
        public async Task DefaultButtonTextIsUpperCase()
        {
            await TestHost.SwitchToAppThread();

            var window = await WindowHelpers.CreateInvisibleWindowAsync<ButtonWindow>();
            var presenter = window.DefaultButton.FindChild<ContentPresenter>("contentPresenter");

            Assert.Equal("SOMETEXT", presenter.Content);
        }

        [Fact]
        public async Task DefaultButtonRespectsButtonHelperPreserveTextCase()
        {
            await TestHost.SwitchToAppThread();

            var window = await WindowHelpers.CreateInvisibleWindowAsync<ButtonWindow>();

            Button defaultButton = window.DefaultButton;
            ButtonHelper.SetPreserveTextCase(defaultButton, true);
            var presenter = defaultButton.FindChild<ContentPresenter>("contentPresenter");

            Assert.Equal("SomeText", presenter.Content); 
        }

        [Fact]
        public async Task SquareButtonButtonTextIsLowerCase()
        {
            await TestHost.SwitchToAppThread();

            var window = await WindowHelpers.CreateInvisibleWindowAsync<ButtonWindow>();
            var presenter = window.SquareButton.FindChild<ContentPresenter>("contentPresenter");

            Assert.Equal("sometext", presenter.Content);
        }

        [Fact]
        public async Task SquareButtonBespectsButtonHelperPreserveTextCase()
        {
            await TestHost.SwitchToAppThread();

            var window = await WindowHelpers.CreateInvisibleWindowAsync<ButtonWindow>();

            Button defaultButton = window.SquareButton;
            ButtonHelper.SetPreserveTextCase(defaultButton, true);
            var presenter = defaultButton.FindChild<ContentPresenter>("contentPresenter");

            Assert.Equal("SomeText", presenter.Content);
        }
    }
}
