namespace MahApps.Metro.Tests
{
    using System.Threading.Tasks;
    using System.Windows.Controls;
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
                                  Assert.False(window.TheDateTimePicker.IsMilitaryTime);
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
                                  Assert.False(window.TheTimePickerDe.IsMilitaryTime);
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
                                  Assert.True(window.TheTimePickerUs.IsMilitaryTime);
                                  Assert.Equal("en-US", window.TheTimePickerUs.Culture.IetfLanguageTag);
                                  Assert.Equal("2:00:01 PM", window.TheTimePickerUs.FindChild<DatePickerTextBox>(string.Empty).Text);
                              });
        }

        [Fact]
        [DisplayTestMethodName]
        public async Task TheTimePickerCsCzTest()
        {
            await TestHost.SwitchToAppThread();
            var window = await WindowHelpers.CreateInvisibleWindowAsync<DateAndTimePickerWindow>().ConfigureAwait(false);
            window.Invoke(() =>
                              {
                                  Assert.NotNull(window.TheTimePickerCsCz.SelectedTime);
                                  Assert.NotNull(window.TheTimePickerCsCz.Culture);
                                  Assert.False(window.TheTimePickerCsCz.IsMilitaryTime);
                                  Assert.Equal("cs-CZ", window.TheTimePickerCsCz.Culture.IetfLanguageTag);
                                  Assert.Equal("22:23:24", window.TheTimePickerCsCz.FindChild<DatePickerTextBox>(string.Empty).Text);
                              });
        }

        [Fact]
        [DisplayTestMethodName]
        public async Task TimePickerTimeFormat()
        {
            await TestHost.SwitchToAppThread();
            var window = await WindowHelpers.CreateInvisibleWindowAsync<DateAndTimePickerWindow>().ConfigureAwait(false);

            window.Invoke(() =>
                {
                    Assert.Equal("it-IT", window.TheDateTimeFormatPicker.Culture.IetfLanguageTag);
                    Assert.False(window.TheDateTimeFormatPicker.IsMilitaryTime);

                    window.TheDateTimeFormatPicker.SelectedTimeFormat = TimePickerFormat.Short;
                    Assert.Equal("31/08/2016 14:00", window.TheDateTimeFormatPicker.FindChild<DatePickerTextBox>(string.Empty).Text);

                    window.TheDateTimeFormatPicker.SelectedTimeFormat = TimePickerFormat.Long;
                    Assert.Equal("31/08/2016 14:00:01", window.TheDateTimeFormatPicker.FindChild<DatePickerTextBox>(string.Empty).Text);

                    window.TheDateTimeFormatPicker.SelectedDateFormat = DatePickerFormat.Long;
                    Assert.Equal("mercoledì 31 agosto 2016 14:00:01", window.TheDateTimeFormatPicker.FindChild<DatePickerTextBox>(string.Empty).Text);


                    window.TheDateTimeFormatPicker.SelectedTimeFormat = TimePickerFormat.Short;
                    Assert.Equal("mercoledì 31 agosto 2016 14:00", window.TheDateTimeFormatPicker.FindChild<DatePickerTextBox>(string.Empty).Text);
                });
        }
    }
}