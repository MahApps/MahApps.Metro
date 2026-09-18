// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using MahApps.Metro.Controls;
using NUnit.Framework;

namespace MahApps.Metro.Tests.TestHelpers
{
    /// <summary>
    /// The few things a test needs to know about the inside of a <see cref="TimePickerBase"/>, in one
    /// place, because several fixtures ask it the same questions.
    /// </summary>
    public static class PickerHelpers
    {
        /// <summary>
        /// The text field of a picker.
        /// </summary>
        public static DatePickerTextBox Field(this TimePickerBase picker)
        {
            var field = picker.FindChild<DatePickerTextBox>("PART_TextBox");

            Assert.That(field, Is.Not.Null, "the template should carry its text box");

            return field!;
        }

        /// <summary>
        /// What the field of a picker reads.
        /// </summary>
        public static string Reads(this TimePickerBase picker)
        {
            return picker.Field().Text;
        }

        /// <summary>
        /// Types into the field and leaves it again, which is when a picker reads the field back and
        /// takes its value from it.
        /// </summary>
        /// <remarks>
        /// The field is emptied first, because assigning the text it already holds raises no
        /// <see cref="TextBoxBase.TextChanged"/> and the picker only reads the field back when
        /// something was typed into it.
        /// </remarks>
        public static void Type(this TimePickerBase picker, string text)
        {
            var field = picker.Field();

            field.Clear();
            field.Text = text;
            field.RaiseEvent(new RoutedEventArgs(UIElement.LostFocusEvent));
        }

        /// <summary>
        /// The button in the drop-down that puts the picker on the here and now.
        /// </summary>
        public static Button NowButton(this TimePickerBase picker)
        {
            // it lives in the drop-down, so the visual tree has nothing until that is opened
            var button = picker.Template?.FindName("PART_NowButton", picker) as Button;

            Assert.That(button, Is.Not.Null, "the template should carry the button");

            return button!;
        }
    }
}
