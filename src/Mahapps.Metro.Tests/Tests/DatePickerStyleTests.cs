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
    /// The three sets share one template for the date picker, so what tells them apart is the
    /// brushes and the calendar each of them hands over. The WinUI one says the focus with the line
    /// along its bottom edge, the other two say it with the frame.
    /// </summary>
    [TestFixture]
    public class DatePickerStyleTests
    {
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

        [TestCase("MahApps.Styles.DatePicker")]
        [TestCase("MahApps.Styles.DatePicker.Win10")]
        [Description("A set that draws no edge of its own says the focus with the frame, the way the picker always has.")]
        public void WithoutAnEdgeTheFrameSaysTheFocus(string key)
        {
            var picker = this.Focused(key);

            Assert.Multiple(() =>
                {
                    Assert.That(Frame(picker).BorderBrush, Is.SameAs(ControlsHelper.GetFocusBorderBrush(picker)), "the frame should take the focus brush");
                    Assert.That(Edge(picker).BorderBrush, Is.Null, "and nothing should be drawn along the bottom edge");
                });
        }

        [Test]
        [Description("The WinUI picker says it with that edge instead, and the frame behind it stays the colour it was.")]
        public void TheWinUIEdgeSaysTheFocus()
        {
            var picker = this.Focused("MahApps.Styles.DatePicker.WinUI");

            Assert.Multiple(() =>
                {
                    Assert.That(Edge(picker).BorderBrush, Is.SameAs(ControlsHelper.GetFocusBorderBrush(picker)), "the bottom edge should take the focus brush");
                    Assert.That(Frame(picker).BorderBrush, Is.SameAs(picker.BorderBrush), "while the frame stays what it was");
                });
        }

        [TestCase("MahApps.Styles.DatePicker.WinUI", "MahApps.Colors.WinUI.ControlStrokeSecondary")]
        [Description("Idle, that edge is the stroke WinUI draws a little stronger than the rest of the frame.")]
        public void AnIdlePickerCarriesTheStrongerStroke(string key, string colourKey)
        {
            var picker = this.Show(key);

            Assert.That(ColourOf(Edge(picker).BorderBrush), Is.EqualTo((Color)picker.FindResource(colourKey)));
        }

        [TestCase("MahApps.Styles.DatePicker", "MahApps.Styles.Calendar.Base")]
        [TestCase("MahApps.Styles.DatePicker.Win10", "MahApps.Styles.Calendar.Win10")]
        [TestCase("MahApps.Styles.DatePicker.WinUI", "MahApps.Styles.Calendar.WinUI")]
        [Description("The drop-down of a picker is a calendar, and each set hands over the one it draws itself.")]
        public void EachSetHandsOverItsOwnCalendar(string key, string calendarKey)
        {
            var picker = this.Show(key);

            Assert.That(picker.CalendarStyle, Is.SameAs(Application.Current.FindResource(calendarKey)));
        }

        [TestCase("MahApps.Styles.DatePicker.Win10")]
        [TestCase("MahApps.Styles.DatePicker.WinUI")]
        [Description("The caret changes what a picker looks like, not how tall it is.")]
        public void APickerTakingTheCaretStaysAsTallAsItWas(string key)
        {
            var picker = this.Show(key);
            var height = picker.ActualHeight;

            Assume.That(height, Is.GreaterThan(0), "the picker should be laid out, otherwise this test proves nothing");

            picker.Focus();
            Keyboard.Focus(picker);
            this.Settle();

            Assume.That(picker.IsKeyboardFocusWithin, Is.True);

            Assert.That(picker.ActualHeight, Is.EqualTo(height).Within(0.001), $"the picker was {height:0.00} high and is {picker.ActualHeight:0.00} with the caret in it");
        }

        private DatePicker Focused(string key)
        {
            var picker = this.Show(key);

            picker.Focus();
            Keyboard.Focus(picker);
            this.Settle();

            // a build agent hands the keyboard to one window at a time
            Assume.That(picker.IsKeyboardFocusWithin, Is.True);

            return picker;
        }

        private DatePicker Show(string key)
        {
            Assert.That(this.window, Is.Not.Null);

            var picker = new DatePicker
                         {
                             Style = (Style)Application.Current.FindResource(key),
                             SelectedDate = new DateTime(2026, 9, 23),
                             Width = 280
                         };

            this.window!.Content = picker;
            this.Settle();

            return picker;
        }

        private static Border Frame(Control picker)
        {
            var frame = picker.FindChild<Border>("Base");
            Assert.That(frame, Is.Not.Null, "the template should carry the frame");

            return frame!;
        }

        private static Border Edge(Control picker)
        {
            var edge = picker.FindChild<Border>("BottomEdge");
            Assert.That(edge, Is.Not.Null, "the template should carry the bottom edge");

            return edge!;
        }

        private static Color ColourOf(Brush? brush)
        {
            Assert.That(brush, Is.InstanceOf<SolidColorBrush>(), "this one should be a brush of a single colour");

            return ((SolidColorBrush)brush!).Color;
        }

        private void Settle()
        {
            this.window!.UpdateLayout();
            ClipAssert.Pump();
        }
    }
}
