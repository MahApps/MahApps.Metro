namespace MahApps.Metro.Tests
{
    using System.Threading.Tasks;
    using System.Windows.Controls.Primitives;
    using MahApps.Metro.Controls;
    using MahApps.Metro.Tests.TestHelpers;
    using Xunit;

    public class DateTimePickerTests : AutomationTestBase
    {
        [Fact]
        [DisplayTestMethodName]
        public async Task DateTimePickerSetCulture()
        {
            await TestHost.SwitchToAppThread();

            var window = await WindowHelpers.CreateInvisibleWindowAsync<DateAndTimePickerWindow>().ConfigureAwait(false);
            window.Invoke(() =>
                {
                    Assert.NotNull(window.TheDateTimePicker.SelectedDate);
                    Assert.NotNull(window.TheDateTimePicker.Culture);
                    Assert.Equal("pt-BR", window.TheDateTimePicker.Culture.IetfLanguageTag);
                    Assert.Equal("31/08/2016 14:00:01", window.TheDateTimePicker.FindChild<DatePickerTextBox>(string.Empty).Text);
                });
        }
    }
}