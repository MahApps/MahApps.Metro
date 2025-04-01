// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using MahApps.Metro.Behaviors;

namespace MahApps.Metro.Controls
{
    public static class MahAppsCommands
    {
        public static ICommand ClearControlCommand { get; } = new RoutedUICommand("Clear", nameof(ClearControlCommand), typeof(MahAppsCommands));

        public static ICommand SearchCommand { get; } = new RoutedUICommand("Search", nameof(SearchCommand), typeof(MahAppsCommands));

        static MahAppsCommands()
        {
            // Register CommandBinding for all windows.
            CommandManager.RegisterClassCommandBinding(typeof(DatePicker), new CommandBinding(ClearControlCommand, (_, args) => ClearControl(args), (_, args) => CanClearControl(args)));
            CommandManager.RegisterClassCommandBinding(typeof(TimePickerBase), new CommandBinding(ClearControlCommand, (_, args) => ClearControl(args), (_, args) => CanClearControl(args)));
            CommandManager.RegisterClassCommandBinding(typeof(TextBoxBase), new CommandBinding(ClearControlCommand, (_, args) => ClearControl(args), (_, args) => CanClearControl(args)));
            CommandManager.RegisterClassCommandBinding(typeof(HotKeyBox), new CommandBinding(ClearControlCommand, (_, args) => ClearControl(args), (_, args) => CanClearControl(args)));
            CommandManager.RegisterClassCommandBinding(typeof(NumericUpDown), new CommandBinding(ClearControlCommand, (_, args) => ClearControl(args), (_, args) => CanClearControl(args)));
            CommandManager.RegisterClassCommandBinding(typeof(PasswordBox), new CommandBinding(ClearControlCommand, (_, args) => ClearControl(args), (_, args) => CanClearControl(args)));
            CommandManager.RegisterClassCommandBinding(typeof(ColorPickerBase), new CommandBinding(ClearControlCommand, (_, args) => ClearControl(args), (_, args) => CanClearControl(args)));
            CommandManager.RegisterClassCommandBinding(typeof(ComboBox), new CommandBinding(ClearControlCommand, (_, args) => ClearControl(args), (_, args) => CanClearControl(args)));
        }

        private static void CanClearControl(CanExecuteRoutedEventArgs args)
        {
            if (args.Handled)
            {
                return;
            }

            if (args.OriginalSource is not DependencyObject control || false == TextBoxHelper.GetClearTextButton(control))
            {
                return;
            }

            args.CanExecute = control switch
            {
                DatePicker datePicker => !ControlsHelper.GetIsReadOnly(datePicker),
                TimePickerBase timePicker => !timePicker.IsReadOnly,
                TextBoxBase textBox => !textBox.IsReadOnly,
                ComboBox comboBox => !comboBox.IsReadOnly,
                _ => true
            };
        }

        private static void ClearControl(RoutedEventArgs args)
        {
            if (args.Handled)
            {
                return;
            }

            if (args.OriginalSource is not DependencyObject control || false == TextBoxHelper.GetClearTextButton(control))
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
                    datePicker.SetCurrentValue(DatePicker.TextProperty, string.Empty);
                    datePicker.GetBindingExpression(TextBox.TextProperty)?.UpdateSource();
                    break;
                case TimePickerBase timePicker:
                    timePicker.Clear();
                    timePicker.GetBindingExpression(TimePickerBase.SelectedDateTimeProperty)?.UpdateSource();
                    break;
                case HotKeyBox hotKeyBox:
                    hotKeyBox.SetCurrentValue(HotKeyBox.HotKeyProperty, null);
                    hotKeyBox.GetBindingExpression(HotKeyBox.HotKeyProperty)?.UpdateSource();
                    break;
                case NumericUpDown numericUpDown:
                    numericUpDown.SetCurrentValue(NumericUpDown.ValueProperty, numericUpDown.DefaultValue);
                    numericUpDown.GetBindingExpression(NumericUpDown.ValueProperty)?.UpdateSource();
                    break;
                case TextBox textBox:
                    textBox.Clear();
                    textBox.GetBindingExpression(TextBox.TextProperty)?.UpdateSource();
                    break;
                case PasswordBox passwordBox:
                    passwordBox.Clear();
                    passwordBox.GetBindingExpression(PasswordBoxBindingBehavior.PasswordProperty)?.UpdateSource();
                    break;
                case ColorPickerBase colorPicker:
                    colorPicker.SetCurrentValue(ColorPickerBase.SelectedColorProperty, null);
                    colorPicker.GetBindingExpression(ColorPickerBase.SelectedColorProperty)?.UpdateSource();
                    break;
                case ComboBox comboBox:
                {
                    if (comboBox is MultiSelectionComboBox multiSelectionComboBox)
                    {
                        if (multiSelectionComboBox.HasCustomText)
                        {
                            multiSelectionComboBox.ResetEditableText(true);
                        }
                        else
                        {
                            switch (multiSelectionComboBox.SelectionMode)
                            {
                                case SelectionMode.Single:
                                    multiSelectionComboBox.SetCurrentValue(MultiSelectionComboBox.SelectedItemProperty, null);
                                    break;
                                case SelectionMode.Multiple:
                                case SelectionMode.Extended:
                                    multiSelectionComboBox.SelectedItems?.Clear();
                                    break;
                                default:
                                    throw new NotSupportedException("Unknown SelectionMode");
                            }

                            multiSelectionComboBox.ResetEditableText(true);
                        }

                        comboBox.GetBindingExpression(ComboBox.TextProperty)?.UpdateSource();
                        comboBox.GetBindingExpression(MultiSelectionComboBox.SelectedItemProperty)?.UpdateSource();
                        comboBox.GetBindingExpression(MultiSelectionComboBox.SelectedItemsProperty)?.UpdateSource();
                    }
                    else
                    {
                        if (comboBox.IsEditable)
                        {
                            comboBox.SetCurrentValue(ComboBox.TextProperty, string.Empty);
                            comboBox.GetBindingExpression(ComboBox.TextProperty)?.UpdateSource();
                        }

                        comboBox.SetCurrentValue(Selector.SelectedItemProperty, null);
                        comboBox.GetBindingExpression(Selector.SelectedItemProperty)?.UpdateSource();
                    }

                    break;
                }
            }
        }
    }
}