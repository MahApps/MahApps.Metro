using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace MahApps.Metro.Controls
{
    using System.ComponentModel;

    /// <summary>
    /// A helper class that provides various attached properties for the TextBox control.
    /// </summary>
    /// <remarks>
    /// Password watermarking code from: http://prabu-guru.blogspot.com/2010/06/how-to-add-watermark-text-to-textbox.html
    /// </remarks>
    public class TextBoxHelper
    {
        public static readonly DependencyProperty IsMonitoringProperty = DependencyProperty.RegisterAttached("IsMonitoring", typeof(bool), typeof(TextBoxHelper), new UIPropertyMetadata(false, OnIsMonitoringChanged));
        public static readonly DependencyProperty WatermarkProperty = DependencyProperty.RegisterAttached("Watermark", typeof(string), typeof(TextBoxHelper), new UIPropertyMetadata(string.Empty));
        public static readonly DependencyProperty UseFloatingWatermarkProperty = DependencyProperty.RegisterAttached("UseFloatingWatermark", typeof(bool), typeof(TextBoxHelper), new FrameworkPropertyMetadata(false, ButtonCommandOrClearTextChanged));
        public static readonly DependencyProperty TextLengthProperty = DependencyProperty.RegisterAttached("TextLength", typeof(int), typeof(TextBoxHelper), new UIPropertyMetadata(0));
        public static readonly DependencyProperty ClearTextButtonProperty = DependencyProperty.RegisterAttached("ClearTextButton", typeof(bool), typeof(TextBoxHelper), new FrameworkPropertyMetadata(false, ButtonCommandOrClearTextChanged));
        public static readonly DependencyProperty ButtonsAlignmentProperty = DependencyProperty.RegisterAttached("ButtonsAlignment", typeof(ButtonsAlignment), typeof(TextBoxHelper), new FrameworkPropertyMetadata(ButtonsAlignment.Right, FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsMeasure));
        /// <summary>
        /// The clear text button behavior property. It sets a click event to the button if the value is true.
        /// </summary>
        public static readonly DependencyProperty IsClearTextButtonBehaviorEnabledProperty = DependencyProperty.RegisterAttached("IsClearTextButtonBehaviorEnabled", typeof(bool), typeof(TextBoxHelper), new FrameworkPropertyMetadata(false, IsClearTextButtonBehaviorEnabledChanged));
        
        public static readonly DependencyProperty ButtonCommandProperty = DependencyProperty.RegisterAttached("ButtonCommand", typeof(ICommand), typeof(TextBoxHelper), new FrameworkPropertyMetadata(null, ButtonCommandOrClearTextChanged));
        public static readonly DependencyProperty ButtonCommandParameterProperty = DependencyProperty.RegisterAttached("ButtonCommandParameter", typeof(object), typeof(TextBoxHelper), new FrameworkPropertyMetadata(null));
        public static readonly DependencyProperty ButtonContentProperty = DependencyProperty.RegisterAttached("ButtonContent", typeof(object), typeof(TextBoxHelper), new FrameworkPropertyMetadata("r"));
        public static readonly DependencyProperty ButtonTemplateProperty = DependencyProperty.RegisterAttached("ButtonTemplate", typeof(ControlTemplate), typeof(TextBoxHelper), new FrameworkPropertyMetadata(null));
        public static readonly DependencyProperty ButtonFontFamilyProperty = DependencyProperty.RegisterAttached("ButtonFontFamily", typeof(FontFamily), typeof(TextBoxHelper), new FrameworkPropertyMetadata((new FontFamilyConverter()).ConvertFromString("Marlett")));
        
        public static readonly DependencyProperty SelectAllOnFocusProperty = DependencyProperty.RegisterAttached("SelectAllOnFocus", typeof(bool), typeof(TextBoxHelper), new FrameworkPropertyMetadata(false));
        public static readonly DependencyProperty IsWaitingForDataProperty = DependencyProperty.RegisterAttached("IsWaitingForData", typeof(bool), typeof(TextBoxHelper), new UIPropertyMetadata(false));

        public static readonly DependencyProperty HasTextProperty = DependencyProperty.RegisterAttached("HasText", typeof (bool), typeof (TextBoxHelper), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty IsSpellCheckContextMenuEnabledProperty = DependencyProperty.RegisterAttached("IsSpellCheckContextMenuEnabled", typeof(bool), typeof(TextBoxHelper), new FrameworkPropertyMetadata(false, UseSpellCheckContextMenuChanged));

        /// <summary>
        /// Indicates if a TextBox or RichTextBox should use SpellCheck context menu
        /// </summary>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(TextBoxBase))]
        public static bool GetIsSpellCheckContextMenuEnabled(UIElement element)
        {
            return (bool)element.GetValue(IsSpellCheckContextMenuEnabledProperty);
        }

        [AttachedPropertyBrowsableForType(typeof(TextBoxBase))]
        public static void SetIsSpellCheckContextMenuEnabled(UIElement element, bool value)
        {
            element.SetValue(IsSpellCheckContextMenuEnabledProperty, value);
        }

        private static void UseSpellCheckContextMenuChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var tb = d as TextBoxBase;
            if (null == tb)
            {
                throw new InvalidOperationException("The property 'IsSpellCheckContextMenuEnabled' may only be set on TextBoxBase elements.");
            }

            if ((bool)e.NewValue) {
                // set the spell check to true
                tb.SetValue(SpellCheck.IsEnabledProperty, true);
                // override pre defined context menu
                tb.ContextMenu = GetDefaultTextBoxBaseContextMenu();
                tb.ContextMenuOpening += TextBoxBaseContextMenuOpening;
            }
            else
            {
                tb.SetValue(SpellCheck.IsEnabledProperty, false);
                tb.ContextMenu = GetDefaultTextBoxBaseContextMenu();
                tb.ContextMenuOpening -= TextBoxBaseContextMenuOpening;
            }
        }

        private static void TextBoxBaseContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            var tbBase = (TextBoxBase)sender;
            var textBox = tbBase as TextBox;
            var richTextBox = tbBase as RichTextBox;

            tbBase.ContextMenu = GetDefaultTextBoxBaseContextMenu();

            var cmdIndex = 0;
            var spellingError = textBox != null
                ? textBox.GetSpellingError(textBox.CaretIndex)
                : (richTextBox != null
                    ? richTextBox.GetSpellingError(richTextBox.CaretPosition)
                    : null);
            if (spellingError != null) {
                var suggestions = spellingError.Suggestions;
                if (suggestions.Any()) {
                    foreach (var suggestion in suggestions) {
                        var mi = new MenuItem();
                        mi.Header = suggestion;
                        mi.FontWeight = FontWeights.Bold;
                        mi.Command = EditingCommands.CorrectSpellingError;
                        mi.CommandParameter = suggestion;
                        mi.CommandTarget = tbBase;
                        mi.SetResourceReference(FrameworkElement.StyleProperty, "MetroMenuItem");
                        tbBase.ContextMenu.Items.Insert(cmdIndex, mi);
                        cmdIndex++;
                    }
                    // add a separator
                    tbBase.ContextMenu.Items.Insert(cmdIndex, new Separator());
                    cmdIndex++;
                }
                var ignoreAllMI = new MenuItem();
                ignoreAllMI.Header = "Ignore All";
                ignoreAllMI.Command = EditingCommands.IgnoreSpellingError;
                ignoreAllMI.CommandTarget = tbBase;
                ignoreAllMI.SetResourceReference(FrameworkElement.StyleProperty, "MetroMenuItem");
                tbBase.ContextMenu.Items.Insert(cmdIndex, ignoreAllMI);
                cmdIndex++;
                // add another separator
                var separatorMenuItem2 = new Separator();
                tbBase.ContextMenu.Items.Insert(cmdIndex, separatorMenuItem2);
            }
        }

        // Gets a fresh context menu. 
        private static ContextMenu GetDefaultTextBoxBaseContextMenu()
        {
            var defaultMenu = new ContextMenu();

            var m1 = new MenuItem { Command = ApplicationCommands.Cut };
            m1.SetResourceReference(FrameworkElement.StyleProperty, "MetroMenuItem");
            var m2 = new MenuItem { Command = ApplicationCommands.Copy };
            m2.SetResourceReference(FrameworkElement.StyleProperty, "MetroMenuItem");
            var m3 = new MenuItem { Command = ApplicationCommands.Paste };
            m3.SetResourceReference(FrameworkElement.StyleProperty, "MetroMenuItem");

            defaultMenu.Items.Add(m1);
            defaultMenu.Items.Add(m2);
            defaultMenu.Items.Add(m3);

            return defaultMenu;
        }

        public static void SetIsWaitingForData(DependencyObject obj, bool value)
        {
            obj.SetValue(IsWaitingForDataProperty, value);
        }
        
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(TextBoxBase))]
        [AttachedPropertyBrowsableForType(typeof(PasswordBox))]
        public static bool GetIsWaitingForData(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsWaitingForDataProperty);
        }

        public static void SetSelectAllOnFocus(DependencyObject obj, bool value)
        {
            obj.SetValue(SelectAllOnFocusProperty, value);
        }

        public static bool GetSelectAllOnFocus(DependencyObject obj)
        {
            return (bool)obj.GetValue(SelectAllOnFocusProperty);
        }

        public static void SetIsMonitoring(DependencyObject obj, bool value)
        {
            obj.SetValue(IsMonitoringProperty, value);
        }

        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(TextBoxBase))]
        [AttachedPropertyBrowsableForType(typeof(PasswordBox))]
        [AttachedPropertyBrowsableForType(typeof(ComboBox))]
        [AttachedPropertyBrowsableForType(typeof(DatePicker))]
        public static string GetWatermark(DependencyObject obj)
        {
            return (string)obj.GetValue(WatermarkProperty);
        }

        public static void SetWatermark(DependencyObject obj, string value)
        {
            obj.SetValue(WatermarkProperty, value);
        }

        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(TextBoxBase))]
        [AttachedPropertyBrowsableForType(typeof(PasswordBox))]
        [AttachedPropertyBrowsableForType(typeof(ComboBox))]
        public static bool GetUseFloatingWatermark(DependencyObject obj)
        {
            return (bool)obj.GetValue(UseFloatingWatermarkProperty);
        }

        public static void SetUseFloatingWatermark(DependencyObject obj, bool value)
        {
            obj.SetValue(UseFloatingWatermarkProperty, value);
        }

        /// <summary>
        /// Gets if the attached TextBox has text.
        /// </summary>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(TextBoxBase))]
        [AttachedPropertyBrowsableForType(typeof(ComboBox))]
        [AttachedPropertyBrowsableForType(typeof(DatePicker))]
        [AttachedPropertyBrowsableForType(typeof(PasswordBox))]
        public static bool GetHasText(DependencyObject obj)
        {
            return (bool)obj.GetValue(HasTextProperty);
        }

        public static void SetHasText(DependencyObject obj, bool value)
        {
            obj.SetValue(HasTextProperty, value);
        }

        private static void OnIsMonitoringChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TextBox)
            {
                var txtBox = d as TextBox;

                if ((bool)e.NewValue)
                {
                    txtBox.TextChanged += TextChanged;
                    txtBox.GotFocus += TextBoxGotFocus;

                    txtBox.Dispatcher.BeginInvoke((Action)(() => 
                        TextChanged(txtBox, new TextChangedEventArgs(TextBox.TextChangedEvent, UndoAction.None))));
                }
                else
                {
                    txtBox.TextChanged -= TextChanged;
                    txtBox.GotFocus -= TextBoxGotFocus;
                }
            }
            else if (d is PasswordBox)
            {
                var passBox = d as PasswordBox;

                if ((bool)e.NewValue)
                {
                    passBox.PasswordChanged += PasswordChanged;
                    passBox.GotFocus += PasswordGotFocus;

                    // Also fixes 1343, also triggers the show of the floating watermark if necessary
                    passBox.Dispatcher.BeginInvoke((Action)(() =>
                        PasswordChanged(passBox, new RoutedEventArgs(PasswordBox.PasswordChangedEvent, passBox))));
                }
                else
                {
                    passBox.PasswordChanged -= PasswordChanged;
                    passBox.GotFocus -= PasswordGotFocus;
                }
            }
            else if (d is NumericUpDown)
            {
                var numericUpDown = d as NumericUpDown;
                numericUpDown.SelectAllOnFocus = (bool)e.NewValue;
                if ((bool)e.NewValue)
                {
                    numericUpDown.ValueChanged += OnNumericUpDownValueChaged;
                    numericUpDown.GotFocus += NumericUpDownGotFocus;
            }
                else
                {
                    numericUpDown.ValueChanged -= OnNumericUpDownValueChaged;
                    numericUpDown.GotFocus -= NumericUpDownGotFocus;
        }
            }
        }

        private static void SetTextLength<TDependencyObject>(TDependencyObject sender, Func<TDependencyObject, int> funcTextLength) where TDependencyObject : DependencyObject
        {
            if (sender != null)
            {
                var value = funcTextLength(sender);
                sender.SetValue(TextLengthProperty, value);
                sender.SetValue(HasTextProperty, value >= 1);
        }
        }

        private static void TextChanged(object sender, RoutedEventArgs e)
        {
            SetTextLength(sender as TextBox, textBox => textBox.Text.Length);
        }

        private static void OnNumericUpDownValueChaged(object sender, RoutedEventArgs e)
        {
            SetTextLength(sender as NumericUpDown, numericUpDown => numericUpDown.Value.HasValue ? 1 : 0);
        }

        private static void PasswordChanged(object sender, RoutedEventArgs e)
        {
            SetTextLength(sender as PasswordBox, passwordBox => passwordBox.Password.Length);
        }

        private static void TextBoxGotFocus(object sender, RoutedEventArgs e)
        {
            ControlGotFocus(sender as TextBox, textBox => textBox.SelectAll());
        }

        private static void NumericUpDownGotFocus(object sender, RoutedEventArgs e)
        {
            ControlGotFocus(sender as NumericUpDown, numericUpDown => numericUpDown.SelectAll());
        }

        private static void PasswordGotFocus(object sender, RoutedEventArgs e)
        {
            ControlGotFocus(sender as PasswordBox, passwordBox => passwordBox.SelectAll());
        }

        private static void ControlGotFocus<TDependencyObject>(TDependencyObject sender, Action<TDependencyObject> action) where TDependencyObject : DependencyObject
        {
            if (sender != null)
            {
                if (GetSelectAllOnFocus(sender))
                {
                    sender.Dispatcher.BeginInvoke(action, sender);
                }
            }
        }

        [Category(AppName.MahApps)]
        public static bool GetClearTextButton(DependencyObject d)
        {
            return (bool)d.GetValue(ClearTextButtonProperty);
        }

        public static void SetClearTextButton(DependencyObject obj, bool value)
        {
            obj.SetValue(ClearTextButtonProperty, value);
        }

        /// <summary>
        /// Gets the buttons placement variant.
        /// </summary>
        [Category(AppName.MahApps)]
        public static ButtonsAlignment GetButtonsAlignment(DependencyObject d)
        {
            return (ButtonsAlignment)d.GetValue(ButtonsAlignmentProperty);
        }

        /// <summary>
        /// Sets the buttons placement variant.
        /// </summary>
        public static void SetButtonsAlignment(DependencyObject obj, ButtonsAlignment value)
        {
            obj.SetValue(ButtonsAlignmentProperty, value);
        }

        /// <summary>
        /// Gets the clear text button behavior.
        /// </summary>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(Button))]
        public static bool GetIsClearTextButtonBehaviorEnabled(Button d)
        {
            return (bool)d.GetValue(IsClearTextButtonBehaviorEnabledProperty);
        }

        /// <summary>
        /// Sets the clear text button behavior.
        /// </summary>
        [AttachedPropertyBrowsableForType(typeof(Button))]
        public static void SetIsClearTextButtonBehaviorEnabled(Button obj, bool value)
        {
            obj.SetValue(IsClearTextButtonBehaviorEnabledProperty, value);
        }

        [Category(AppName.MahApps)]
        public static ICommand GetButtonCommand(DependencyObject d)
        {
            return (ICommand)d.GetValue(ButtonCommandProperty);
        }

        public static void SetButtonCommand(DependencyObject obj, ICommand value)
        {
            obj.SetValue(ButtonCommandProperty, value);
        }

        [Category(AppName.MahApps)]
        public static object GetButtonCommandParameter(DependencyObject d)
        {
            return (object)d.GetValue(ButtonCommandParameterProperty);
        }

        public static void SetButtonCommandParameter(DependencyObject obj, object value)
        {
            obj.SetValue(ButtonCommandParameterProperty, value);
        }

        [Category(AppName.MahApps)]
        public static object GetButtonContent(DependencyObject d)
        {
            return (object)d.GetValue(ButtonContentProperty);
        }

        public static void SetButtonContent(DependencyObject obj, object value)
        {
            obj.SetValue(ButtonContentProperty, value);
        }

        [Category(AppName.MahApps)]
        public static ControlTemplate GetButtonTemplate(DependencyObject d)
        {
            return (ControlTemplate)d.GetValue(ButtonTemplateProperty);
        }

        public static void SetButtonTemplate(DependencyObject obj, ControlTemplate value)
        {
            obj.SetValue(ButtonTemplateProperty, value);
        }

        [Category(AppName.MahApps)]
        public static FontFamily GetButtonFontFamily(DependencyObject d)
        {
            return (FontFamily)d.GetValue(ButtonFontFamilyProperty);
        }

        public static void SetButtonFontFamily(DependencyObject obj, FontFamily value)
        {
            obj.SetValue(ButtonFontFamilyProperty, value);
        }

        private static void IsClearTextButtonBehaviorEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var button = d as Button;
            if (e.OldValue != e.NewValue && button != null)
            {
                button.Click -= ButtonClicked;
                if ((bool)e.NewValue)
                {
                    button.Click += ButtonClicked;
                }
            }
        }

        public static void ButtonClicked(object sender, RoutedEventArgs e)
        {
            var button = ((Button)sender);
            var parent = VisualTreeHelper.GetParent(button);
            while (!(parent is TextBox || parent is PasswordBox || parent is ComboBox))
            {
                parent = VisualTreeHelper.GetParent(parent);
            }

            var command = GetButtonCommand(parent);
            if (command != null && command.CanExecute(parent))
            {
                var commandParameter = GetButtonCommandParameter(parent);

                command.Execute(commandParameter ?? parent);
            }

            if (GetClearTextButton(parent))
            {
                if (parent is TextBox)
                {
                    ((TextBox)parent).Clear();
                }
                else if (parent is PasswordBox)
                {
                    ((PasswordBox)parent).Clear();
                }
                else if (parent is ComboBox)
                {
                    if (((ComboBox)parent).IsEditable)
                    {
                        ((ComboBox)parent).Text = string.Empty;
                    }
                    ((ComboBox)parent).SelectedItem = null;
                }
            }
        }

        private static void ButtonCommandOrClearTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var textbox = d as TextBox;
            if (textbox != null)
            {
                // only one loaded event
                textbox.Loaded -= TextChanged;
                textbox.Loaded += TextChanged;
                if (textbox.IsLoaded)
                {
                    TextChanged(textbox, new RoutedEventArgs());
                }
            }
            var passbox = d as PasswordBox;
            if (passbox != null)
            {
                // only one loaded event
                passbox.Loaded -= PasswordChanged;
                passbox.Loaded += PasswordChanged;
                if (passbox.IsLoaded)
                {
                    PasswordChanged(passbox, new RoutedEventArgs());
                }
            }
            var combobox = d as ComboBox;
            if (combobox != null)
            {
                // only one loaded event
                combobox.Loaded -= ComboBoxLoaded;
                combobox.Loaded += ComboBoxLoaded;
                if (combobox.IsLoaded)
                {
                    ComboBoxLoaded(combobox, new RoutedEventArgs());
                }
            }
        }

        static void ComboBoxLoaded(object sender, RoutedEventArgs e)
        {
            var comboBox = sender as ComboBox;
            if (comboBox != null)
            {
                comboBox.SetValue(HasTextProperty, !string.IsNullOrWhiteSpace(comboBox.Text) || comboBox.SelectedItem != null);
            }
        }
    }
}
