namespace MahApps.Metro.Controls
{
    using System;
    using System.Windows;
    using System.Windows.Controls.Primitives;

    /// <summary>
    ///     Represents a control that allows the user to select a time.
    /// </summary>
    public class TimePicker : TimePickerBase
    {
        static TimePicker()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TimePicker), new FrameworkPropertyMetadata(typeof(TimePicker)));
        }

        public TimePicker()
        {
            IsDatePickerVisible = false;
        }

        protected override void OnTextBoxLostFocus(object sender, RoutedEventArgs e)
        {
            TimeSpan ts;
            if (TimeSpan.TryParse(((DatePickerTextBox)sender).Text, SpecificCultureInfo, out ts))
            {
                this.SelectedDateTime = this.SelectedDateTime.GetValueOrDefault().Date + ts;
            }
            else
            {
                if (string.IsNullOrEmpty(((DatePickerTextBox)sender).Text))
                {
                    this.SelectedDateTime = this.SelectedDateTime.GetValueOrDefault().Date;
                    WriteValueToTextBox(string.Empty);
                }
                else
                {
                    WriteValueToTextBox();
                }
            }
        }
    }
}