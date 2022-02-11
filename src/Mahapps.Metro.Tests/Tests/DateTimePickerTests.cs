// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Interop;
using MahApps.Metro.Controls;
using MahApps.Metro.Tests.TestHelpers;
using Xunit;

namespace MahApps.Metro.Tests.Tests
{
    public class DateTimePickerTests : AutomationTestFixtureBase<DateTimePickerTestsFixture>
    {
        public DateTimePickerTests(DateTimePickerTestsFixture fixture)
            : base(fixture)
        {
        }

        [Fact]
        [DisplayTestMethodName]
        public async Task DateTimePickerSetCulture()
        {
            await TestHost.SwitchToAppThread();

            Assert.NotNull(this.fixture.Window?.TheDateTimePicker.SelectedDateTime);
            Assert.NotNull(this.fixture.Window?.TheDateTimePicker.Culture);
            Assert.False(this.fixture.Window?.TheDateTimePicker.IsMilitaryTime);
            Assert.Equal("pt-BR", this.fixture.Window?.TheDateTimePicker.Culture.IetfLanguageTag);
            var datePickerTextBox = this.fixture.Window?.TheDateTimePicker.FindChild<DatePickerTextBox>(string.Empty);
            Assert.NotNull(datePickerTextBox);
            Assert.Equal("31/08/2016 14:00:01", datePickerTextBox?.Text);
        }

        [Fact]
        [DisplayTestMethodName]
        public async Task TimePickerCultureDeTest()
        {
            await TestHost.SwitchToAppThread();

            Assert.NotNull(this.fixture.Window?.TheTimePickerDe.SelectedDateTime);
            Assert.NotNull(this.fixture.Window?.TheTimePickerDe.Culture);
            Assert.False(this.fixture.Window?.TheTimePickerDe.IsMilitaryTime);
            Assert.Equal("de-DE", this.fixture.Window?.TheTimePickerDe.Culture.IetfLanguageTag);
            var datePickerTextBox = this.fixture.Window?.TheTimePickerDe.FindChild<DatePickerTextBox>(string.Empty);
            Assert.NotNull(datePickerTextBox);
            Assert.Equal("14:00:01", datePickerTextBox?.Text);
        }

        [Fact]
        [DisplayTestMethodName]
        public async Task TimePickerCultureUsTest()
        {
            await TestHost.SwitchToAppThread();

            Assert.NotNull(this.fixture.Window?.TheTimePickerUs.SelectedDateTime);
            Assert.NotNull(this.fixture.Window?.TheTimePickerUs.Culture);
            Assert.True(this.fixture.Window?.TheTimePickerUs.IsMilitaryTime);
            Assert.Equal("en-US", this.fixture.Window?.TheTimePickerUs.Culture.IetfLanguageTag);
            var datePickerTextBox = this.fixture.Window?.TheTimePickerUs.FindChild<DatePickerTextBox>(string.Empty);
            Assert.NotNull(datePickerTextBox);
            Assert.Equal("2:00:01 PM", datePickerTextBox?.Text);
        }

        [Fact]
        [DisplayTestMethodName]
        public async Task TheTimePickerCsCzTest()
        {
            await TestHost.SwitchToAppThread();

            Assert.NotNull(this.fixture.Window?.TheTimePickerCsCz.SelectedDateTime);
            Assert.NotNull(this.fixture.Window?.TheTimePickerCsCz.Culture);
            Assert.False(this.fixture.Window?.TheTimePickerCsCz.IsMilitaryTime);
            Assert.Equal("cs-CZ", this.fixture.Window?.TheTimePickerCsCz.Culture.IetfLanguageTag);
            var datePickerTextBox = this.fixture.Window?.TheTimePickerCsCz.FindChild<DatePickerTextBox>(string.Empty);
            Assert.NotNull(datePickerTextBox);
            Assert.Equal("22:23:24", datePickerTextBox?.Text);
        }

        [Fact]
        [DisplayTestMethodName]
        public async Task TimePickerTimeFormat()
        {
            await TestHost.SwitchToAppThread();

            Assert.NotNull(this.fixture.Window?.TheDateTimeFormatPicker.Culture);
            Assert.Equal("it-IT", this.fixture.Window?.TheDateTimeFormatPicker.Culture.IetfLanguageTag);
            Assert.False(this.fixture.Window?.TheDateTimeFormatPicker.IsMilitaryTime);

            var datePickerTextBox = this.fixture.Window?.TheDateTimeFormatPicker.FindChild<DatePickerTextBox>(string.Empty);
            Assert.NotNull(datePickerTextBox);

            this.fixture.Window?.TheDateTimeFormatPicker.SetCurrentValue(TimePickerBase.SelectedTimeFormatProperty, TimePickerFormat.Short);
            Assert.Equal("31/08/2016 14:00", datePickerTextBox.Text);

            this.fixture.Window?.TheDateTimeFormatPicker.SetCurrentValue(TimePickerBase.SelectedTimeFormatProperty, TimePickerFormat.Long);
            Assert.Equal("31/08/2016 14:00:01", datePickerTextBox.Text);

            this.fixture.Window?.TheDateTimeFormatPicker.SetCurrentValue(DateTimePicker.SelectedDateFormatProperty, DatePickerFormat.Long);
            Assert.Equal("mercoledì 31 agosto 2016 14:00:01", datePickerTextBox.Text);

            this.fixture.Window?.TheDateTimeFormatPicker.SetCurrentValue(TimePickerBase.SelectedTimeFormatProperty, TimePickerFormat.Short);
            Assert.Equal("mercoledì 31 agosto 2016 14:00", datePickerTextBox.Text);
        }

        [Fact]
        [DisplayTestMethodName]
        public async Task MilitaryTimeShouldBeConvertedToDateTime()
        {
            await TestHost.SwitchToAppThread();

            var datePickerTextBox = this.fixture.Window?.EmptyTimePicker?.FindChild<DatePickerTextBox>(string.Empty);
            Assert.NotNull(datePickerTextBox);

            datePickerTextBox.SetCurrentValue(TextBox.TextProperty, "2:42:12 PM");

            datePickerTextBox.RaiseEvent(new KeyEventArgs(
                                             Keyboard.PrimaryDevice,
                                             new HwndSource(0, 0, 0, 0, 0, "", IntPtr.Zero), // dummy presentation source
                                             0,
                                             Key.Return)
                                         {
                                             RoutedEvent = Keyboard.KeyDownEvent
                                         }
            );

            Assert.Equal(default(DateTime) + new TimeSpan(14, 42, 12), (this.fixture.Window?.EmptyTimePicker).SelectedDateTime);
        }
    }
}