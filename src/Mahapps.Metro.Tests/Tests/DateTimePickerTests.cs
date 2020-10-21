// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using MahApps.Metro.Controls;
using MahApps.Metro.Tests.TestHelpers;
using Xunit;

namespace MahApps.Metro.Tests
{
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
                    Assert.NotNull(window.TheDateTimePicker.SelectedDateTime);
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
                    Assert.NotNull(window.TheTimePickerDe.SelectedDateTime);
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
                    Assert.NotNull(window.TheTimePickerUs.SelectedDateTime);
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
                    Assert.NotNull(window.TheTimePickerCsCz.SelectedDateTime);
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

                    var datePickerTextBox = window.TheDateTimeFormatPicker.FindChild<DatePickerTextBox>(string.Empty);

                    window.TheDateTimeFormatPicker.SelectedTimeFormat = TimePickerFormat.Short;
                    Assert.Equal("31/08/2016 14:00", datePickerTextBox.Text);

                    window.TheDateTimeFormatPicker.SelectedTimeFormat = TimePickerFormat.Long;
                    Assert.Equal("31/08/2016 14:00:01", datePickerTextBox.Text);

                    window.TheDateTimeFormatPicker.SelectedDateFormat = DatePickerFormat.Long;
                    Assert.Equal("mercoledì 31 agosto 2016 14:00:01", datePickerTextBox.Text);

                    window.TheDateTimeFormatPicker.SelectedTimeFormat = TimePickerFormat.Short;
                    Assert.Equal("mercoledì 31 agosto 2016 14:00", datePickerTextBox.Text);
                });
        }

        [Fact]
        [DisplayTestMethodName]
        public async Task MilitaryTimeShouldBeConvertedToDateTime()
        {
            await TestHost.SwitchToAppThread();
            var window = await WindowHelpers.CreateInvisibleWindowAsync<DateAndTimePickerWindow>(
                w => { w.EmptyTimePicker.Focus(); }
            );

            var timePicker = window.EmptyTimePicker;
            var datePickerTextBox = timePicker.FindChild<DatePickerTextBox>(string.Empty);

            datePickerTextBox.Text = "2:42:12 PM";
            datePickerTextBox.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));

            Assert.Equal(default(DateTime) + new TimeSpan(14, 42, 12), timePicker.SelectedDateTime);
        }
    }
}