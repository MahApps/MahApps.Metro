// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Globalization;
using System.Threading.Tasks;
using System.Windows.Controls;
using MahApps.Metro.Controls;
using MahApps.Metro.Tests.TestHelpers;
using NUnit.Framework;

namespace MahApps.Metro.Tests.Tests
{
    /// <summary>
    /// GH-4644: short or long out of the culture was the whole choice, so a field that should read
    /// dd.MM.yyyy HH:mm meant bending a CultureInfo into shape or overriding GetValueForTextBox in a
    /// subclass. SelectedDateTimeFormat says it outright, and the parse side has to take it back.
    /// </summary>
    [TestFixture]
    public class TimePickerFormatTests
    {
        private const string DayFirst = "dd.MM.yyyy HH:mm";

        private static readonly DateTime Afternoon = new DateTime(2026, 3, 17, 14, 35, 0);

        private TestWindow? window;

        [OneTimeSetUp]
        public async Task OneTimeSetUp()
        {
            this.window = await WindowHelpers.CreateInvisibleWindowAsync<TestWindow>().ConfigureAwait(false);
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            this.window?.Close();
            this.window = null;
        }

        [Test]
        [Description("A format is what the field reads, whatever the culture would have made of the value.")]
        public void AFormatIsWhatTheFieldReads()
        {
            var picker = this.window.Show(new DateTimePicker
                                   {
                                       Culture = new CultureInfo("en-US"),
                                       SelectedDateTime = Afternoon,
                                       SelectedDateTimeFormat = DayFirst
                                   });

            Assert.That(picker.Reads(), Is.EqualTo("17.03.2026 14:35"));
        }

        [Test]
        [Description("It has the first word over the two enums, which is the point of having it: long and long is what en-US would say here.")]
        public void AFormatWinsOverTheTwoEnums()
        {
            var picker = this.window.Show(new DateTimePicker
                                   {
                                       Culture = new CultureInfo("en-US"),
                                       SelectedDateTime = Afternoon,
                                       SelectedDateFormat = DatePickerFormat.Long,
                                       SelectedTimeFormat = TimePickerFormat.Long,
                                       SelectedDateTimeFormat = DayFirst
                                   });

            Assert.That(picker.Reads(), Is.EqualTo("17.03.2026 14:35"));
        }

        [Test]
        [Description("The plain time picker takes one too, since the property sits on the base both share.")]
        public void ATimePickerTakesAFormatAsWell()
        {
            var picker = this.window.Show(new TimePicker
                                   {
                                       Culture = new CultureInfo("en-US"),
                                       SelectedDateTime = Afternoon,
                                       SelectedDateTimeFormat = "HH:mm"
                                   });

            Assert.That(picker.Reads(), Is.EqualTo("14:35"));
        }

        [Test]
        [Description("Nothing about the old behaviour moves: taken away again, the culture says what the field reads.")]
        public void WithTheFormatGoneTheCultureSaysItAgain()
        {
            var picker = this.window.Show(new DateTimePicker
                                   {
                                       Culture = new CultureInfo("en-US"),
                                       SelectedDateTime = Afternoon,
                                       SelectedDateTimeFormat = DayFirst
                                   });

            picker.SetCurrentValue(TimePickerBase.SelectedDateTimeFormatProperty, null);

            Assert.That(picker.Reads(), Is.EqualTo("3/17/2026 2:35:00 PM"));
        }

        [Test]
        [Description("What the field wrote has to read back as what it was. 03.04.2026 is the third of April to the format and the fourth of March to en-US, and the format is what wrote it.")]
        public void WhatTheFieldWroteReadsBackTheWayTheFormatSaysIt()
        {
            var couldSwap = new DateTime(2026, 4, 3, 14, 35, 0);

            var picker = this.window.Show(new DateTimePicker
                                   {
                                       Culture = new CultureInfo("en-US"),
                                       SelectedDateTime = couldSwap,
                                       SelectedDateTimeFormat = DayFirst
                                   });

            var wasWritten = picker.Reads();

            Assert.That(wasWritten, Is.EqualTo("03.04.2026 14:35"));

            picker.Type(wasWritten);

            Assert.Multiple(() =>
                {
                    Assert.That(picker.SelectedDateTime, Is.EqualTo(couldSwap), "the value should have survived being typed back in");
                    Assert.That(picker.Reads(), Is.EqualTo(wasWritten));
                });
        }

        [Test]
        [Description("A format costs nothing in what may be typed: anything the culture can still make sense of is taken.")]
        public void TypingStaysAsForgivingAsItWas()
        {
            var picker = this.window.Show(new DateTimePicker
                                   {
                                       Culture = new CultureInfo("de-DE"),
                                       SelectedDateTime = Afternoon,
                                       SelectedDateTimeFormat = DayFirst
                                   });

            // a date on its own, which the format does not describe and the culture reads without trouble
            picker.Type("24.12.2026");

            Assert.That(picker.SelectedDateTime, Is.EqualTo(new DateTime(2026, 12, 24)));
        }
    }
}
