// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Interop;
using MahApps.Metro.Controls;
using MahApps.Metro.Tests.TestHelpers;
using NUnit.Framework;

namespace MahApps.Metro.Tests.Tests
{
    /// <summary>
    /// GH-4153: a button in the drop-down that puts the picker on the here and now, date and time in
    /// one go, so somebody does not have to walk the calendar and three lists to say what the clock
    /// on the wall already says.
    /// </summary>
    [TestFixture]
    public class PickerNowButtonTests
    {
        private static readonly DateTime LongAgo = new DateTime(2003, 6, 2, 8, 15, 30);

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
        [Description("The button is there to be seen without anybody asking for it.")]
        public void ThePickerShowsTheButtonWithoutBeingAsked()
        {
            var picker = this.Show(new DateTimePicker());

            Assert.That(picker.IsNowButtonVisible, Is.True, "the picker should say it shows one");
            Assert.That(Button(picker).Visibility, Is.EqualTo(Visibility.Visible), "and the button should be visible");
        }

        [Test]
        [Description("Pressing it puts the picker on the here and now, the time of day along with the date.")]
        public void PressingTheButtonSetsBothTheDateAndTheTime()
        {
            var picker = this.Show(new DateTimePicker { SelectedDateTime = LongAgo });

            var before = DateTime.Now;
            Button(picker).RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
            var after = DateTime.Now;

            Assert.That(picker.SelectedDateTime, Is.Not.Null);
            Assert.That(picker.SelectedDateTime, Is.InRange(before, after), "the picker should stand at the moment the button was pressed");
        }

        [Test]
        [Description("A picker told to do without it does without it, so a form that has no use for one is not given one.")]
        public void APickerToldToDoWithoutTheButtonHidesIt()
        {
            var picker = this.Show(new DateTimePicker { IsNowButtonVisible = false });

            Assert.That(Button(picker).Visibility, Is.Not.EqualTo(Visibility.Visible));
        }

        [Test]
        [Description("The caption is the picker's to hand out, since Now is a word that wants translating.")]
        public void TheCaptionIsThePickersToHandOut()
        {
            var picker = this.Show(new DateTimePicker { NowButtonContent = "Jetzt" });

            Assert.That(Button(picker).Content, Is.EqualTo("Jetzt"));
        }

        [Test]
        [Description("The plain time picker carries one as well, and pressing it lands on the time of day.")]
        public void TheTimePickerCarriesOneToo()
        {
            var picker = this.Show(new TimePicker { SelectedDateTime = LongAgo });

            var before = DateTime.Now;
            Button(picker).RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
            var after = DateTime.Now;

            Assert.That(picker.SelectedDateTime, Is.InRange(before, after));
        }

        [Test]
        [Description("Escape closes the drop-down and puts back what was there before, whether the keyboard is on one of the lists or on this button.")]
        public void EscapeOnTheButtonClosesTheDropDownAndPutsBackWhatWasThere()
        {
            var picker = this.Show(new DateTimePicker { SelectedDateTime = LongAgo });

            picker.SetCurrentValue(TimePickerBase.IsDropDownOpenProperty, true);
            this.Settle();

            var button = Button(picker);
            button.Focus();
            this.Settle();

            button.RaiseEvent(new KeyEventArgs(Keyboard.PrimaryDevice, new HwndSource(0, 0, 0, 0, 0, string.Empty, IntPtr.Zero), 0, Key.Escape)
                              {
                                  RoutedEvent = Keyboard.PreviewKeyDownEvent
                              });
            this.Settle();

            Assert.That(picker.IsDropDownOpen, Is.False, "the drop-down should be shut");
            Assert.That(picker.SelectedDateTime, Is.EqualTo(LongAgo), "and the value should be the one it had before");
        }

        private static Button Button(TimePickerBase picker)
        {
            // it lives in the drop-down, so the visual tree has nothing until that is opened
            var button = picker.Template?.FindName("PART_NowButton", picker) as Button;

            Assert.That(button, Is.Not.Null, "the template should carry the button");

            return button!;
        }

        private T Show<T>(T picker)
            where T : TimePickerBase
        {
            Assert.That(this.window, Is.Not.Null);

            this.window!.Content = picker;
            this.Settle();

            Assert.That(picker.IsLoaded, Is.True, "the picker should be up before a test looks at it");

            return picker;
        }

        private void Settle()
        {
            this.window!.UpdateLayout();
            ClipAssert.Pump();
        }
    }
}
