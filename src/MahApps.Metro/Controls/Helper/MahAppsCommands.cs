using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using MahApps.Metro.Behaviors;

namespace MahApps.Metro.Controls
{
    public static class MahAppsCommands
    {
        public static ICommand ClearControlCommand { get; } = new RoutedUICommand("Clear", "ClearControlCommand", typeof(MahAppsCommands));

        public static void ClearControl(ExecutedRoutedEventArgs args)
        {
            if (args.Handled)
            {
                return;
            }

            if (!(args.OriginalSource is DependencyObject control) || false == TextBoxHelper.GetClearTextButton(control))
            {
                return;
            }

            switch (control)
            {
                case RichTextBox richTextBox:
                    richTextBox.Document?.Blocks?.Clear();
                    richTextBox.Selection?.Select(richTextBox.CaretPosition, richTextBox.CaretPosition);
                    break;
                case DatePicker datePicker:
                    datePicker.SetCurrentValue(DatePicker.SelectedDateProperty, null);
                    datePicker.SetCurrentValue(DatePicker.TextProperty, null);
                    datePicker.GetBindingExpression(TextBox.TextProperty)?.UpdateSource();
                    break;
                case TimePickerBase timePicker:
                    timePicker.SetCurrentValue(TimePickerBase.SelectedDateTimeProperty, null);
                    timePicker.GetBindingExpression(TimePickerBase.SelectedDateTimeProperty)?.UpdateSource();
                    break;
                case TextBox textBox:
                    textBox.Clear();
                    textBox.GetBindingExpression(TextBox.TextProperty)?.UpdateSource();
                    break;
                case PasswordBox passwordBox:
                    passwordBox.Clear();
                    passwordBox.GetBindingExpression(PasswordBoxBindingBehavior.PasswordProperty)?.UpdateSource();
                    break;
                case ComboBox comboBox:
                {
                    if (comboBox.IsEditable)
                    {
                        comboBox.SetCurrentValue(ComboBox.TextProperty, null);
                        comboBox.GetBindingExpression(ComboBox.TextProperty)?.UpdateSource();
                    }

                    comboBox.SetCurrentValue(ComboBox.SelectedItemProperty, null);
                    comboBox.GetBindingExpression(ComboBox.SelectedItemProperty)?.UpdateSource();
                    break;
                }
            }
        }
    }
}