// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
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
            var picker = this.Show(new DateTimePicker { Culture = new CultureInfo("de-DE"), SelectedDateTime = Afternoon });
            var reported = Watch(picker);

            Type(picker, "31.02.2026");

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
            var picker = this.Show(new TimePicker { Culture = new CultureInfo("de-DE"), SelectedDateTime = Afternoon });
            var reported = Watch(picker);

            Type(picker, "25:61");

            Assert.That(reported, Is.EqualTo(new[] { "25:61" }));
        }

        [Test]
        [Description("An emptied field is somebody clearing the value, so there is nothing to report about it.")]
        public void AnEmptiedFieldIsNotAnError()
        {
            var picker = this.Show(new DateTimePicker { Culture = new CultureInfo("de-DE"), SelectedDateTime = Afternoon });
            var reported = Watch(picker);

            Type(picker, string.Empty);

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
            var picker = this.Show(new DateTimePicker { Culture = new CultureInfo("de-DE"), SelectedDateTime = Afternoon });
            var reported = Watch(picker);

            Type(picker, "   ");

            Assert.That(reported, Is.Empty);
        }

        [Test]
        [Description("A date the picker can read is not an error either, however it was typed.")]
        public void TextThatParsesIsNotReported()
        {
            var picker = this.Show(new DateTimePicker { Culture = new CultureInfo("de-DE"), SelectedDateTime = Afternoon });
            var reported = Watch(picker);

            Type(picker, "24.12.2026 18:00");

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

            this.Show(panel);

            var reported = new List<string?>();
            panel.AddHandler(TimePickerBase.DateTimeValidationErrorEvent, new EventHandler<DateTimeValidationErrorEventArgs>((_, e) => reported.Add(e.Text)));

            Type(picker, "31.02.2026");

            Assert.That(reported, Is.EqualTo(new[] { "31.02.2026" }));
        }

        private static List<string?> Watch(TimePickerBase picker)
        {
            var reported = new List<string?>();

            picker.DateTimeValidationError += (_, e) => reported.Add(e.Text);

            return reported;
        }

        private static void Type(TimePickerBase picker, string text)
        {
            var box = picker.FindChild<DatePickerTextBox>("PART_TextBox");

            Assert.That(box, Is.Not.Null, "the template should carry its text box");

            // emptied first, because assigning the text the field already holds raises no TextChanged
            // and the picker only reads the field back when something was typed into it
            box!.Clear();
            box.Text = text;
            box.RaiseEvent(new RoutedEventArgs(UIElement.LostFocusEvent));
        }

        private T Show<T>(T content)
            where T : FrameworkElement
        {
            Assert.That(this.window, Is.Not.Null);

            this.window!.Content = content;
            this.window.UpdateLayout();
            ClipAssert.Pump();

            Assert.That(content.IsLoaded, Is.True, "the picker should be up before a test looks at it");

            return content;
        }
    }
}
