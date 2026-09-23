// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using MahApps.Metro.Controls;
using MahApps.Metro.Tests.TestHelpers;
using NUnit.Framework;

namespace MahApps.Metro.Tests.Tests
{
    /// <summary>
    /// The three pickers share their templates across the sets, so two habits the Windows 10 and the
    /// WinUI text boxes have are knobs on those templates rather than markup of their own: the clear
    /// button that waits for the caret, and a switched-off control that says so with its colours
    /// instead of with a veil drawn over it. The Metro set asks for neither and is left as it was.
    /// </summary>
    [TestFixture]
    public class PickerSetStateTests
    {
        private const string MetroDate = "MahApps.Styles.DatePicker";
        private const string MetroTime = "MahApps.Styles.TimePicker";
        private const string MetroDateTime = "MahApps.Styles.DateTimePicker";

        private TestWindow? window;

        [OneTimeSetUp]
        public async Task OneTimeSetUp()
        {
            this.window = await WindowHelpers.CreateInvisibleWindowAsync<TestWindow>().ConfigureAwait(true);
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            this.window?.Close();
            this.window = null;
        }

        [TestCase("MahApps.Styles.DatePicker.Win10")]
        [TestCase("MahApps.Styles.DatePicker.WinUI")]
        [TestCase("MahApps.Styles.TimePicker.Win10")]
        [TestCase("MahApps.Styles.TimePicker.WinUI")]
        [TestCase("MahApps.Styles.DateTimePicker.Win10")]
        [TestCase("MahApps.Styles.DateTimePicker.WinUI")]
        [Description("The delete button of a UWP box is there while the caret is in it, and the pickers of the two Windows sets follow that.")]
        public void TheClearButtonWaitsForTheCaret(string key)
        {
            var picker = this.Show(key, clearButton: true);

            var button = ClearButton(picker);
            Assert.That(button.Visibility, Is.Not.EqualTo(Visibility.Visible), "a picker nobody is writing in should not offer to clear itself");

            picker.Focus();
            Keyboard.Focus(picker);
            this.Settle();

            // a build agent hands the keyboard to one window at a time
            Assume.That(picker.IsKeyboardFocusWithin, Is.True);

            Assert.That(button.Visibility, Is.EqualTo(Visibility.Visible), "and the button should be there once the caret is");
        }

        [TestCase(MetroDate)]
        [TestCase(MetroTime)]
        [TestCase(MetroDateTime)]
        [Description("The Metro picker asks for none of that and keeps the button it has always shown.")]
        public void TheMetroClearButtonStandsWhereItAlwaysDid(string key)
        {
            var picker = this.Show(key, clearButton: true);

            Assert.That(ClearButton(picker).Visibility, Is.EqualTo(Visibility.Visible));
        }

        [TestCase("MahApps.Styles.DatePicker.Win10")]
        [TestCase("MahApps.Styles.DatePicker.WinUI")]
        [TestCase("MahApps.Styles.TimePicker.Win10")]
        [TestCase("MahApps.Styles.TimePicker.WinUI")]
        [TestCase("MahApps.Styles.DateTimePicker.Win10")]
        [TestCase("MahApps.Styles.DateTimePicker.WinUI")]
        [Description("A picker of these sets that is switched off says so with its own colours, the way the text box of the set does, and draws no veil over itself.")]
        public void ASwitchedOffPickerSaysSoWithItsColours(string key)
        {
            var picker = this.Show(key);

            picker.IsEnabled = false;
            this.Settle();

            Assert.Multiple(() =>
                {
                    Assert.That(ControlsHelper.GetDisabledVisualElementVisibility(picker), Is.EqualTo(Visibility.Collapsed), "no veil over this one");
                    Assert.That(Frame(picker).BorderBrush, Is.SameAs(ControlsHelper.GetDisabledBorderBrush(picker)), "the frame should take the disabled stroke");
                    Assert.That(picker.Background, Is.SameAs(picker.FindResource(key.Contains(".WinUI") ? "MahApps.Brushes.TextControl.WinUI.BackgroundDisabled" : "MahApps.Brushes.TextControl.BackgroundDisabled")), "and the fill should be the disabled one");
                    Assert.That(picker.Foreground, Is.SameAs(picker.FindResource(key.Contains(".WinUI") ? "MahApps.Brushes.TextControl.WinUI.ForegroundDisabled" : "MahApps.Brushes.TextControl.ForegroundDisabled")), "as should the text");
                });
        }

        [TestCase(MetroDate)]
        [TestCase(MetroTime)]
        [TestCase(MetroDateTime)]
        [Description("The Metro picker keeps the veil it has always drawn over itself.")]
        public void TheMetroPickerKeepsItsVeil(string key)
        {
            var picker = this.Show(key);

            picker.IsEnabled = false;
            this.Settle();

            var veil = picker.FindChild<Border>("DisabledVisualElement");
            Assert.That(veil, Is.Not.Null, "the template should carry the veil");

            Assert.Multiple(() =>
                {
                    Assert.That(ControlsHelper.GetDisabledVisualElementVisibility(picker), Is.EqualTo(Visibility.Visible));
                    Assert.That(veil!.Opacity, Is.EqualTo(0.6).Within(0.001), "and draw it once the picker is off");
                });
        }

        private static Button ClearButton(Control picker)
        {
            var button = picker.FindChild<Button>("PART_ClearText");
            Assert.That(button, Is.Not.Null, "the template should carry the clear button");

            return button!;
        }

        private static Border Frame(Control picker)
        {
            var frame = picker.FindChild<Border>("Base");
            Assert.That(frame, Is.Not.Null, "the template should carry the frame");

            return frame!;
        }

        private Control Show(string key, bool clearButton = false)
        {
            Assert.That(this.window, Is.Not.Null);

            Control picker = key.Contains("DateTimePicker")
                ? new DateTimePicker { SelectedDateTime = new DateTime(2026, 9, 23, 14, 42, 0) }
                : key.Contains("TimePicker")
                    ? new TimePicker { SelectedDateTime = new DateTime(2026, 9, 23, 14, 42, 0) }
                    : new DatePicker { SelectedDate = new DateTime(2026, 9, 23) };

            picker.Style = (Style)Application.Current.FindResource(key);
            picker.Width = 280;

            if (clearButton)
            {
                TextBoxHelper.SetClearTextButton(picker, true);
            }

            this.window!.Content = picker;
            this.Settle();

            return picker;
        }

        private void Settle()
        {
            this.window!.UpdateLayout();
            ClipAssert.Pump();
        }
    }
}
