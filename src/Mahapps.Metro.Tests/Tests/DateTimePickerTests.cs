// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Interop;
using MahApps.Metro.Controls;
using MahApps.Metro.Tests.TestHelpers;
using MahApps.Metro.Tests.Views;
using NUnit.Framework;

namespace MahApps.Metro.Tests.Tests
{
    [TestFixture]
    public class DateTimePickerTests
    {
        private DateAndTimePickerWindow? window;

        [OneTimeSetUp]
        public async Task OneTimeSetUp()
        {
            this.window = await WindowHelpers.CreateInvisibleWindowAsync<DateAndTimePickerWindow>().ConfigureAwait(false);
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            this.window?.Close();
            this.window = null;
        }

        [SetUp]
        public void SetUp()
        {
            this.PreparePropertiesForTest();
        }

        private void PreparePropertiesForTest(IList<string>? properties = null)
        {
            // nothing to do here
        }

        [Test]
        public void DateTimePickerSetCulture()
        {
            Assert.That(this.window, Is.Not.Null);

            Assert.That(window.TheDateTimePicker.SelectedDateTime, Is.Not.Null);
            Assert.That(window.TheDateTimePicker.Culture, Is.Not.Null);
            Assert.That(window.TheDateTimePicker.IsMilitaryTime, Is.False);
            Assert.That(window.TheDateTimePicker.Culture.IetfLanguageTag, Is.EqualTo("pt-BR"));
            var datePickerTextBox = window.TheDateTimePicker.FindChild<DatePickerTextBox>(string.Empty);
            Assert.That(datePickerTextBox, Is.Not.Null);
            Assert.That(datePickerTextBox?.Text, Is.EqualTo("31/08/2016 14:00:01"));
        }

        [Test]
        public void TimePickerCultureDeTest()
        {
            Assert.That(this.window, Is.Not.Null);

            Assert.That(window.TheTimePickerDe.SelectedDateTime, Is.Not.Null);
            Assert.That(window.TheTimePickerDe.Culture, Is.Not.Null);
            Assert.That(window.TheTimePickerDe.IsMilitaryTime, Is.False);
            Assert.That(window.TheTimePickerDe.Culture.IetfLanguageTag, Is.EqualTo("de-DE"));
            var datePickerTextBox = window.TheTimePickerDe.FindChild<DatePickerTextBox>(string.Empty);
            Assert.That(datePickerTextBox, Is.Not.Null);
            Assert.That(datePickerTextBox?.Text, Is.EqualTo("14:00:01"));
        }

        [Test]
        public void TimePickerCultureUsTest()
        {
            Assert.That(this.window, Is.Not.Null);

            Assert.That(window.TheTimePickerUs.SelectedDateTime, Is.Not.Null);
            Assert.That(window.TheTimePickerUs.Culture, Is.Not.Null);
            Assert.That(window.TheTimePickerUs.IsMilitaryTime, Is.True);
            Assert.That(window.TheTimePickerUs.Culture.IetfLanguageTag, Is.EqualTo("en-US"));
            var datePickerTextBox = window.TheTimePickerUs.FindChild<DatePickerTextBox>(string.Empty);
            Assert.That(datePickerTextBox, Is.Not.Null);
            Assert.That(datePickerTextBox?.Text, Is.EqualTo("2:00:01 PM"));
        }

        [Test]
        public void TheTimePickerCsCzTest()
        {
            Assert.That(this.window, Is.Not.Null);

            Assert.That(window.TheTimePickerCsCz.SelectedDateTime, Is.Not.Null);
            Assert.That(window.TheTimePickerCsCz.Culture, Is.Not.Null);
            Assert.That(window.TheTimePickerCsCz.IsMilitaryTime, Is.False);
            Assert.That(window.TheTimePickerCsCz.Culture.IetfLanguageTag, Is.EqualTo("cs-CZ"));
            var datePickerTextBox = window.TheTimePickerCsCz.FindChild<DatePickerTextBox>(string.Empty);
            Assert.That(datePickerTextBox, Is.Not.Null);
            Assert.That(datePickerTextBox?.Text, Is.EqualTo("22:23:24"));
        }

        [Test]
        public void TimePickerTimeFormat()
        {
            Assert.That(this.window, Is.Not.Null);

            Assert.That(window.TheDateTimeFormatPicker.Culture, Is.Not.Null);
            Assert.That(window.TheDateTimeFormatPicker.Culture.IetfLanguageTag, Is.EqualTo("it-IT"));
            Assert.That(window.TheDateTimeFormatPicker.IsMilitaryTime, Is.False);

            var datePickerTextBox = window.TheDateTimeFormatPicker.FindChild<DatePickerTextBox>(string.Empty);
            Assert.That(datePickerTextBox, Is.Not.Null);

            window.TheDateTimeFormatPicker.SetCurrentValue(TimePickerBase.SelectedTimeFormatProperty, TimePickerFormat.Short);
            Assert.That(datePickerTextBox.Text, Is.EqualTo("31/08/2016 14:00"));

            window.TheDateTimeFormatPicker.SetCurrentValue(TimePickerBase.SelectedTimeFormatProperty, TimePickerFormat.Long);
            Assert.That(datePickerTextBox.Text, Is.EqualTo("31/08/2016 14:00:01"));

            window.TheDateTimeFormatPicker.SetCurrentValue(DateTimePicker.SelectedDateFormatProperty, DatePickerFormat.Long);
            Assert.That(datePickerTextBox.Text, Is.EqualTo("mercoledì 31 agosto 2016 14:00:01"));

            window.TheDateTimeFormatPicker.SetCurrentValue(TimePickerBase.SelectedTimeFormatProperty, TimePickerFormat.Short);
            Assert.That(datePickerTextBox.Text, Is.EqualTo("mercoledì 31 agosto 2016 14:00"));
        }

        [Test]
        public void MilitaryTimeShouldBeConvertedToDateTime()
        {
            Assert.That(this.window, Is.Not.Null);

            var datePickerTextBox = window.EmptyTimePicker?.FindChild<DatePickerTextBox>(string.Empty);
            Assert.That(datePickerTextBox, Is.Not.Null);

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

            Assert.That((window.EmptyTimePicker).SelectedDateTime, Is.EqualTo(default(DateTime) + new TimeSpan(14, 42, 12)));
        }

        /// <summary>
        /// GH-4586: TimePicker.SetSelectedDateTime combined its DateTimeStyles flags with "&"
        /// instead of "|", which folds the constant to DateTimeStyles.None.
        /// Nails down the parse behaviour the control actually offers, so that correcting the
        /// operator, or touching the flags later, cannot change it unnoticed.
        /// </summary>
        [Theory]
        [TestCase("2:30:45 PM", 14, 30, 45)]
        [TestCase("2 : 30 : 45 PM", 14, 30, 45)] // inner whitespace
        [TestCase("  2:30:45 PM  ", 14, 30, 45)] // leading and trailing whitespace
        [TestCase("2  :  30  :  45  PM", 14, 30, 45)]
        public void ShouldParseTimeWithWhitespace(string text, int hours, int minutes, int seconds)
        {
            Assert.That(this.window, Is.Not.Null);

            var picker = this.window.EmptyTimePicker;
            Assert.That(picker, Is.Not.Null);

            picker.SetCurrentValue(TimePickerBase.SelectedDateTimeProperty, null);

            CommitText(picker, text);

            Assert.That(picker.SelectedDateTime, Is.EqualTo(default(DateTime) + new TimeSpan(hours, minutes, seconds)));
        }

        /// <summary>
        /// GH-4586 and GH-4551: the DateTimeStyles of the TimePicker contain AssumeLocal, which
        /// never reaches the stored value because the result is built from the previous value's
        /// date. Documents that the control reports Unspecified, matching the calendar path.
        /// </summary>
        [Test]
        public void ShouldReportUnspecifiedKindForTextInput()
        {
            Assert.That(this.window, Is.Not.Null);

            var picker = this.window.EmptyTimePicker;
            Assert.That(picker, Is.Not.Null);

            picker.SetCurrentValue(TimePickerBase.SelectedDateTimeProperty, null);

            CommitText(picker, "2:42:12 PM");

            Assert.That(picker.SelectedDateTime, Is.Not.Null);
            Assert.That(picker.SelectedDateTime!.Value.Kind, Is.EqualTo(DateTimeKind.Unspecified));
        }

        /// <summary>
        /// GH-4551: picking the time before the date ran through ClockSelectedTimeChanged, whose
        /// fallback was DateTime.Today and therefore carried DateTimeKind.Local, while the
        /// calendar and the text box both produce Unspecified. One control handed its consumer
        /// two different kinds depending on the order the user picked things in.
        /// </summary>
        [Test]
        public void ShouldReportUnspecifiedKindWhenTimeIsPickedFromDropDown()
        {
            Assert.That(this.window, Is.Not.Null);

            var picker = this.window.EmptyTimePicker;
            picker.SetCurrentValue(TimePickerBase.SelectedDateTimeProperty, null);
            picker.ApplyTemplate();
            picker.SetCurrentValue(TimePickerBase.IsDropDownOpenProperty, true);

            try
            {
                var popup = picker.FindChild<Popup>(string.Empty);
                Assert.That(popup, Is.Not.Null, "no popup found");

                var content = popup.Child as FrameworkElement;
                Assert.That(content, Is.Not.Null, "popup has no child");

                content.ApplyTemplate();

                var hourPicker = content.FindChild<Selector>("PART_HourPicker");
                Assert.That(hourPicker, Is.Not.Null, "no PART_HourPicker inside the popup");

                hourPicker.SetCurrentValue(Selector.SelectedIndexProperty, 7);

                Assert.That(picker.SelectedDateTime, Is.Not.Null, "selecting an hour did not set a value");
                Assert.That(picker.SelectedDateTime!.Value.Kind, Is.EqualTo(DateTimeKind.Unspecified));

                // On an empty picker the drop down fills the date from today, where the text
                // path uses default(DateTime). Documented on the TimePicker page.
                Assert.That(picker.SelectedDateTime!.Value.Date, Is.EqualTo(DateTime.Today));
            }
            finally
            {
                picker.SetCurrentValue(TimePickerBase.IsDropDownOpenProperty, false);
            }
        }

        /// <summary>
        /// Commits text into a picker's text box the way a user does: type it, then press Return.
        /// </summary>
        private static void CommitText(TimePickerBase picker, string text)
        {
            var datePickerTextBox = picker.FindChild<DatePickerTextBox>(string.Empty);
            Assert.That(datePickerTextBox, Is.Not.Null);

            datePickerTextBox.SetCurrentValue(TextBox.TextProperty, text);

            datePickerTextBox.RaiseEvent(new KeyEventArgs(
                                             Keyboard.PrimaryDevice,
                                             new HwndSource(0, 0, 0, 0, 0, "", IntPtr.Zero), // dummy presentation source
                                             0,
                                             Key.Return)
                                         {
                                             RoutedEvent = Keyboard.KeyDownEvent
                                         });
        }

    }
}