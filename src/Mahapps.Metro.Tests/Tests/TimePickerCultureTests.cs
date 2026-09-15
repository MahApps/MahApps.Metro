// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Markup;
using MahApps.Metro.Controls;
using MahApps.Metro.Tests.TestHelpers;
using NUnit.Framework;

namespace MahApps.Metro.Tests.Tests
{
    /// <summary>
    /// GH-4064: a picker nobody told anything about the culture showed an American date on a German
    /// machine, while the DatePicker next to it showed a German one. The pickers went by Language,
    /// whose default in WPF is en-US whatever the thread says, and the date controls WPF brings along
    /// go by the culture of the thread instead.
    /// </summary>
    [TestFixture]
    public class TimePickerCultureTests
    {
        private static readonly DateTime Afternoon = new DateTime(2026, 3, 17, 14, 35, 0);

        private TestWindow? window;
        private CultureInfo? wasCulture;

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

        [SetUp]
        public void SetUp()
        {
            this.wasCulture = Thread.CurrentThread.CurrentCulture;
            Thread.CurrentThread.CurrentCulture = new CultureInfo("de-DE");
        }

        [TearDown]
        public void TearDown()
        {
            Thread.CurrentThread.CurrentCulture = this.wasCulture!;
        }

        [Test]
        [Description("Told nothing at all, a picker goes by the culture of the thread, the way the DatePicker beside it does.")]
        public void APickerNobodyToldGoesByTheCultureOfTheThread()
        {
            var picker = this.Show(new DateTimePicker { SelectedDateTime = Afternoon });

            Assert.That(Reads(picker), Is.EqualTo("17.03.2026 14:35:00"));
        }

        [Test]
        [Description("A culture named by hand still wins, culture being the property that is there to say so.")]
        public void ACultureNamedByHandWins()
        {
            var picker = this.Show(new DateTimePicker { SelectedDateTime = Afternoon, Culture = new CultureInfo("en-US") });

            Assert.That(Reads(picker), Is.EqualTo("3/17/2026 2:35:00 PM"));
        }

        [Test]
        [Description("And so does a language, which is the other way of saying it and the one that is inherited down the tree.")]
        public void ALanguageSetOnThePickerWins()
        {
            var picker = this.Show(new DateTimePicker { SelectedDateTime = Afternoon, Language = XmlLanguage.GetLanguage("en-US") });

            Assert.That(Reads(picker), Is.EqualTo("3/17/2026 2:35:00 PM"));
        }

        [Test]
        [Description("A language inherited from further up counts as somebody having said so, since that is how a whole window is set to one language. What it then reads is what the same picture would read with that culture named outright.")]
        public void ALanguageInheritedFromTheWindowWins()
        {
            // a third language, so that neither the thread nor the default of en-US could be the one
            // answering by accident
            this.window!.SetCurrentValue(FrameworkElement.LanguageProperty, XmlLanguage.GetLanguage("fr-FR"));

            try
            {
                var inherited = Reads(this.Show(new DateTimePicker { SelectedDateTime = Afternoon }));
                var named = Reads(this.Show(new DateTimePicker { SelectedDateTime = Afternoon, Culture = new CultureInfo("fr-FR") }));

                Assert.That(inherited, Is.EqualTo(named), "a language passed down should read the same as the culture named outright");
                Assert.That(inherited, Is.Not.EqualTo("17.03.2026 14:35:00"), "and not fall back to the thread");
            }
            finally
            {
                this.window.ClearValue(FrameworkElement.LanguageProperty);
            }
        }

        [Test]
        [Description("The plain time picker goes the same way, since the two share what decides it.")]
        public void ATimePickerGoesByTheCultureOfTheThreadToo()
        {
            var picker = this.Show(new TimePicker { SelectedDateTime = Afternoon });

            Assert.That(Reads(picker), Is.EqualTo("14:35:00"));
        }

        private static string Reads(TimePickerBase picker)
        {
            var box = picker.FindChild<DatePickerTextBox>("PART_TextBox");

            Assert.That(box, Is.Not.Null, "the template should carry its text box");

            return box!.Text;
        }

        private T Show<T>(T picker)
            where T : TimePickerBase
        {
            Assert.That(this.window, Is.Not.Null);

            this.window!.Content = picker;
            this.window.UpdateLayout();
            ClipAssert.Pump();

            Assert.That(picker.IsLoaded, Is.True, "the picker should be up before a test looks at it");

            return picker;
        }
    }
}
