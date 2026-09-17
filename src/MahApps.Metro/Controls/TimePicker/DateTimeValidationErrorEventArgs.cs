// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows;

namespace MahApps.Metro.Controls
{
    /// <summary>
    /// Carries the text a <see cref="TimePickerBase"/> could not read as a date and a time.
    /// </summary>
    public class DateTimeValidationErrorEventArgs : RoutedEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DateTimeValidationErrorEventArgs"/> class.
        /// </summary>
        /// <param name="routedEvent">The event being raised.</param>
        /// <param name="source">The picker the text was typed into.</param>
        /// <param name="text">The text that would not parse.</param>
        public DateTimeValidationErrorEventArgs(RoutedEvent routedEvent, object source, string? text)
            : base(routedEvent, source)
        {
            this.Text = text;
        }

        /// <summary>
        /// Gets the text that was in the field and could not be read as a date and a time.
        /// </summary>
        public string? Text { get; }
    }
}
