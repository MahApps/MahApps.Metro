// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows.Controls;
using MahApps.Metro.Controls;

namespace MetroDemo.ExampleViews
{
    /// <summary>
    /// Interaction logic for DateExamples.xaml
    /// </summary>
    public partial class DateExamples : UserControl
    {
        public DateExamples()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Both pickers hand this the text they could not read as a date and a time. Without it a
        /// typo and a field somebody emptied on purpose look exactly alike, both being a null value.
        /// </summary>
        private void OnDateTimeValidationError(object sender, DateTimeValidationErrorEventArgs e)
        {
            this.DateTimePickerValidationError.Text = $"{e.Text} is not a date and a time I can read";
        }
    }
}