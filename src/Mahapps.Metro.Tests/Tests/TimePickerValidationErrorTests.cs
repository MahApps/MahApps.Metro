// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using System.Windows.Controls;
using MahApps.Metro.Controls;
using MahApps.Metro.Tests.TestHelpers;
using NUnit.Framework;

namespace MahApps.Metro.Tests.Tests
{
    /// <summary>
    /// GH-4645: text that would not parse was dropped without a word. The value went to null and the
    /// field was rewritten from it, which is what an emptied field does too, so a form could not tell
    /// somebody clearing the date from somebody mistyping it.
    /// </summary>
    [TestFixture]
    public class TimePickerValidationErrorTests
    {
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
        [Description("A day February does not have is not a date, and the picker now says so instead of only emptying itself.")]
        public void TextThatWillNotParseIsReported()
        {
            var picker = this.window.Show(new DateTimePicker { Culture = new CultureInfo("de-DE"), SelectedDateTime = Afternoon });
            var reported = Watch(picker);

            picker.Type("31.02.2026");

            Assert.Multiple(() =>
                {
                    Assert.That(reported, Has.Count.EqualTo(1), "one report for one piece of unreadable text");
                    Assert.That(reported[0], Is.EqualTo("31.02.2026"), "and it should carry what was typed, not what the field ended up showing");
                    Assert.That(picker.SelectedDateTime, Is.Null, "the value still goes, as it always did");
                });
        }

        [Test]
        [Description("The plain time picker reports as well, the event sitting on the base both share.")]
        public void ATimePickerReportsToo()
        {
            var picker = this.window.Show(new TimePicker { Culture = new CultureInfo("de-DE"), SelectedDateTime = Afternoon });
            var reported = Watch(picker);

            picker.Type("25:61");

            Assert.That(reported, Is.EqualTo(new[] { "25:61" }));
        }

        [Test]
        [Description("An emptied field is somebody clearing the value, so there is nothing to report about it.")]
        public void AnEmptiedFieldIsNotAnError()
        {
            var picker = this.window.Show(new DateTimePicker { Culture = new CultureInfo("de-DE"), SelectedDateTime = Afternoon });
            var reported = Watch(picker);

            picker.Type(string.Empty);

            Assert.Multiple(() =>
                {
                    Assert.That(reported, Is.Empty, "clearing a field is not a mistake");
                    Assert.That(picker.SelectedDateTime, Is.Null, "and it still clears the value");
                });
        }

        [Test]
        [Description("Nor is whitespace, which is the same thing typed with the space bar.")]
        public void NorIsWhitespace()
        {
            var picker = this.window.Show(new DateTimePicker { Culture = new CultureInfo("de-DE"), SelectedDateTime = Afternoon });
            var reported = Watch(picker);

            picker.Type("   ");

            Assert.That(reported, Is.Empty);
        }

        [Test]
        [Description("A date the picker can read is not an error either, however it was typed.")]
        public void TextThatParsesIsNotReported()
        {
            var picker = this.window.Show(new DateTimePicker { Culture = new CultureInfo("de-DE"), SelectedDateTime = Afternoon });
            var reported = Watch(picker);

            picker.Type("24.12.2026 18:00");

            Assert.Multiple(() =>
                {
                    Assert.That(reported, Is.Empty);
                    Assert.That(picker.SelectedDateTime, Is.EqualTo(new DateTime(2026, 12, 24, 18, 0, 0)));
                });
        }

        [Test]
        [Description("It bubbles, so a form can listen once further up instead of on every picker it holds.")]
        public void ItBubblesUpTheTree()
        {
            var picker = new DateTimePicker { Culture = new CultureInfo("de-DE"), SelectedDateTime = Afternoon };
            var panel = new StackPanel();
            panel.Children.Add(picker);

            this.window.Show(panel);

            var reported = new List<string?>();
            panel.AddHandler(TimePickerBase.DateTimeValidationErrorEvent, new EventHandler<DateTimeValidationErrorEventArgs>((_, e) => reported.Add(e.Text)));

            picker.Type("31.02.2026");

            Assert.That(reported, Is.EqualTo(new[] { "31.02.2026" }));
        }

        private static List<string?> Watch(TimePickerBase picker)
        {
            var reported = new List<string?>();

            picker.DateTimeValidationError += (_, e) => reported.Add(e.Text);

            return reported;
        }
    }
}
