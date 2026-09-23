// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using MahApps.Metro.Tests.TestHelpers;
using NUnit.Framework;

namespace MahApps.Metro.Tests.Tests
{
    /// <summary>
    /// A box that takes the caret may change how it looks and not how much room it needs. The WinUI
    /// style marks the focus with a thicker line along its bottom edge, and a border that grows takes
    /// the height of the control with it: measured without layout rounding, which is what hides it at
    /// some scalings, the box grew by a pixel and everything under it moved down with it.
    /// </summary>
    [TestFixture]
    public class TextControlFocusLayoutTests
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

        [TestCase("MahApps.Styles.TextBox")]
        [TestCase("MahApps.Styles.TextBox.Search")]
        [TestCase("MahApps.Styles.TextBox.Win10")]
        [TestCase("MahApps.Styles.TextBox.WinUI")]
        [TestCase("MahApps.Styles.PasswordBox")]
        [TestCase("MahApps.Styles.PasswordBox.Win10")]
        [TestCase("MahApps.Styles.PasswordBox.WinUI")]
        [TestCase("MahApps.Styles.RichTextBox")]
        [TestCase("MahApps.Styles.RichTextBox.Win10")]
        [TestCase("MahApps.Styles.RichTextBox.WinUI")]
        [TestCase("MahApps.Styles.DatePicker")]
        [TestCase("MahApps.Styles.DatePicker.Win10")]
        [TestCase("MahApps.Styles.DatePicker.WinUI")]
        [Description("The caret changes what a box looks like, not how tall it is.")]
        public void ABoxTakingTheCaretMovesNothingUnderIt(string key)
        {
            Control box = key switch
            {
                _ when key.Contains("PasswordBox") => new PasswordBox { Password = "Konrad" },
                _ when key.Contains("RichTextBox") => Document("Konrad"),
                _ when key.Contains("DatePicker") => new DatePicker { SelectedDate = new DateTime(2026, 9, 23) },
                _ => new TextBox { Text = "Konrad" }
            };

            box.Style = (Style)Application.Current.FindResource(key);
            box.Width = 200;
            box.VerticalAlignment = VerticalAlignment.Top;

            var below = new Border { Height = 20 };

            // the rounding of the layout to whole device pixels hides a pixel at some scalings and
            // shows it at others, so it is out of the way here
            var page = new StackPanel { UseLayoutRounding = false, SnapsToDevicePixels = false };
            page.Children.Add(box);
            page.Children.Add(below);

            this.window!.Content = page;
            this.Settle();

            var height = box.ActualHeight;
            var whatIsUnderIt = below.TranslatePoint(default, this.window).Y;

            Assume.That(height, Is.GreaterThan(0), "the box should be laid out, otherwise this test proves nothing");

            box.Focus();
            Keyboard.Focus(box);
            this.Settle();

            // a build agent hands the keyboard to one window at a time, and the caret of a picker
            // lands in a box inside its template rather than on the picker itself
            Assume.That(box.IsKeyboardFocusWithin, Is.True);

            Assert.Multiple(() =>
                {
                    Assert.That(box.ActualHeight, Is.EqualTo(height).Within(0.001), $"the box was {height:0.00} high and is {box.ActualHeight:0.00} with the caret in it");
                    Assert.That(below.TranslatePoint(default, this.window).Y, Is.EqualTo(whatIsUnderIt).Within(0.001), "and what stands under it has not moved");
                });
        }

        /// <summary>
        /// A rich text box takes its text as a document rather than as a string.
        /// </summary>
        private static RichTextBox Document(string text)
        {
            var box = new RichTextBox();
            box.Document.Blocks.Clear();
            box.Document.Blocks.Add(new Paragraph(new Run(text)));

            return box;
        }

        private void Settle()
        {
            this.window!.UpdateLayout();
            ClipAssert.Pump();
        }
    }
}
