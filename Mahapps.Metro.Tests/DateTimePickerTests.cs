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

        [Fact]
        [DisplayTestMethodName]
        public async Task TimePickerCultureDeTest()
        {
            await TestHost.SwitchToAppThread();
            var window = await WindowHelpers.CreateInvisibleWindowAsync<DateAndTimePickerWindow>().ConfigureAwait(false);
            window.Invoke(() =>
                              {
                                  Assert.NotNull(window.TheTimePickerDe.SelectedTime);
                                  Assert.NotNull(window.TheTimePickerDe.Culture);
                                  Assert.Equal("de-DE", window.TheTimePickerDe.Culture.IetfLanguageTag);
                                  Assert.Equal("14:00:01", window.TheTimePickerDe.FindChild<DatePickerTextBox>(string.Empty).Text);
                              });
        }

        [Fact]
        [DisplayTestMethodName]
        public async Task TimePickerCultureUsTest()
        {
            await TestHost.SwitchToAppThread();
            var window = await WindowHelpers.CreateInvisibleWindowAsync<DateAndTimePickerWindow>().ConfigureAwait(false);
            window.Invoke(() =>
                              {
                                  Assert.NotNull(window.TheTimePickerUs.SelectedTime);
                                  Assert.NotNull(window.TheTimePickerUs.Culture);
                                  Assert.Equal("en-US", window.TheTimePickerUs.Culture.IetfLanguageTag);
                                  Assert.Equal("2:00:01 PM", window.TheTimePickerUs.FindChild<DatePickerTextBox>(string.Empty).Text);
                              });
        }
    }
}