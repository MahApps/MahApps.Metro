using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace MahApps.Metro.Controls
{
    /// <summary>
    /// Password watermarking code from: http://prabu-guru.blogspot.com/2010/06/how-to-add-watermark-text-to-textbox.html
    /// </summary>
    public class TextboxHelper : DependencyObject
    {
        public static readonly DependencyProperty IsMonitoringProperty = DependencyProperty.RegisterAttached("IsMonitoring", typeof(bool), typeof(TextboxHelper), new UIPropertyMetadata(false, OnIsMonitoringChanged));
        public static readonly DependencyProperty WatermarkProperty = DependencyProperty.RegisterAttached("Watermark", typeof(string), typeof(TextboxHelper), new UIPropertyMetadata(string.Empty));
        public static readonly DependencyProperty TextLengthProperty = DependencyProperty.RegisterAttached("TextLength", typeof(int), typeof(TextboxHelper), new UIPropertyMetadata(0));
        public static readonly DependencyProperty ClearTextButtonProperty = DependencyProperty.RegisterAttached("ClearTextButton", typeof(bool), typeof(TextboxHelper), new FrameworkPropertyMetadata(false, ButtonCommandOrClearTextChanged));
        public static readonly DependencyProperty ButtonCommandProperty = DependencyProperty.RegisterAttached("ButtonCommand", typeof(ICommand), typeof(TextboxHelper), new FrameworkPropertyMetadata(null, ButtonCommandOrClearTextChanged));
        public static readonly DependencyProperty ButtonContentProperty = DependencyProperty.RegisterAttached("ButtonContent", typeof(object), typeof(TextboxHelper), new FrameworkPropertyMetadata("r"));
        public static readonly DependencyProperty ButtonTemplateProperty = DependencyProperty.RegisterAttached("ButtonTemplate", typeof(ControlTemplate), typeof(TextboxHelper), new FrameworkPropertyMetadata(null));
        public static readonly DependencyProperty SelectAllOnFocusProperty = DependencyProperty.RegisterAttached("SelectAllOnFocus", typeof(bool), typeof(TextboxHelper), new FrameworkPropertyMetadata(false));

        private static readonly DependencyProperty hasTextProperty = DependencyProperty.RegisterAttached("HasText", typeof(bool), typeof(TextboxHelper), new FrameworkPropertyMetadata(false));

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

        public static string GetWatermark(DependencyObject obj)
        {
            return (string)obj.GetValue(WatermarkProperty);
        }

        public static void SetWatermark(DependencyObject obj, string value)
        {
            obj.SetValue(WatermarkProperty, value);
        }

        private static void SetTextLength(DependencyObject obj, int value)
        {
            obj.SetValue(TextLengthProperty, value);
            obj.SetValue(hasTextProperty, value >= 1);
        }

        public bool HasText
        {
            get { return (bool)GetValue(hasTextProperty); }
        }

        static void OnIsMonitoringChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TextBox)
            {
                var txtBox = d as TextBox;

                if ((bool)e.NewValue)
                {
                    txtBox.TextChanged += TextChanged;
                    txtBox.GotFocus += TextBoxGotFocus;
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
                }
                else
                {
                    passBox.PasswordChanged -= PasswordChanged;
                    passBox.GotFocus -= PasswordGotFocus;
                }
            }
        }

        static void TextChanged(object sender, TextChangedEventArgs e)
        {
            var txtBox = sender as TextBox;
            if (txtBox == null)
                return;
            SetTextLength(txtBox, txtBox.Text.Length);
        }

        static void PasswordChanged(object sender, RoutedEventArgs e)
        {
            var passBox = sender as PasswordBox;
            if (passBox == null)
                return;
            SetTextLength(passBox, passBox.Password.Length);
        }

        static void TextBoxGotFocus(object sender, RoutedEventArgs e)
        {
            var txtBox = sender as TextBox;
            if (txtBox == null)
                return;
            if (GetSelectAllOnFocus(txtBox))
            {
                txtBox.Dispatcher.BeginInvoke((Action)(txtBox.SelectAll));
            }
        }

        static void PasswordGotFocus(object sender, RoutedEventArgs e)
        {
            var passBox = sender as PasswordBox;
            if (passBox == null)
                return;
            if (GetSelectAllOnFocus(passBox))
            {
                passBox.Dispatcher.BeginInvoke((Action)(passBox.SelectAll));
            }
        }

        public static bool GetClearTextButton(DependencyObject d)
        {
            return (bool)d.GetValue(ClearTextButtonProperty);
        }

        public static void SetClearTextButton(DependencyObject obj, bool value)
        {
            obj.SetValue(ClearTextButtonProperty, value);
        }

        public static ICommand GetButtonCommand(DependencyObject d)
        {
            return (ICommand)d.GetValue(ButtonCommandProperty);
        }

        public static void SetButtonCommand(DependencyObject obj, ICommand value)
        {
            obj.SetValue(ButtonCommandProperty, value);
        }

        public static object GetButtonContent(DependencyObject d)
        {
            return (object)d.GetValue(ButtonContentProperty);
        }

        public static void SetButtonContent(DependencyObject obj, object value)
        {
            obj.SetValue(ButtonContentProperty, value);
        }

        public static ControlTemplate GetButtonTemplate(DependencyObject d)
        {
            return (ControlTemplate)d.GetValue(ButtonTemplateProperty);
        }

        public static void SetButtonTemplate(DependencyObject obj, ControlTemplate value)
        {
            obj.SetValue(ButtonTemplateProperty, value);
        }

        private static void ButtonCommandOrClearTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var textbox = d as TextBox;
            if (textbox != null)
            {
                // only one loaded event
                textbox.Loaded -= TextBoxLoaded;
                textbox.Loaded += TextBoxLoaded;
            }
            var passbox = d as PasswordBox;
            if (passbox != null)
            {
                // only one loaded event
                passbox.Loaded -= PassBoxLoaded;
                passbox.Loaded += PassBoxLoaded;
            }
        }

        static void PassBoxLoaded(object sender, RoutedEventArgs e)
        {
            var passbox = sender as PasswordBox;
            if (passbox == null || passbox.Style == null)
                return;

            var template = passbox.Template;
            if (template == null)
                return;

            var button = template.FindName("PART_ClearText", passbox) as Button;
            if (button == null)
                return;

            if (GetButtonCommand(passbox) != null || GetClearTextButton(passbox))
            {
                // only one event, because loaded event fires more than once, if the textbox is hosted in a tab item
                button.Click -= ButtonClicked;
                button.Click += ButtonClicked;
            }
            else
            {
                button.Click -= ButtonClicked;
            }
        }

        static void TextBoxLoaded(object sender, RoutedEventArgs e)
        {
            var textbox = sender as TextBox;
            if (textbox == null || textbox.Style == null)
                return;

            var template = textbox.Template;
            if (template == null)
                return;

            var button = template.FindName("PART_ClearText", textbox) as Button;
            if (button == null)
                return;

            if (GetButtonCommand(textbox) != null || GetClearTextButton(textbox))
            {
                // only one event, because loaded event fires more than once, if the textbox is hosted in a tab item
                button.Click -= ButtonClicked;
                button.Click += ButtonClicked;
            }
            else
            {
                button.Click -= ButtonClicked;
            }
        }

        static void ButtonClicked(object sender, RoutedEventArgs e)
        {
            var button = ((Button)sender);
            var parent = VisualTreeHelper.GetParent(button);
            while (!(parent is TextBox || parent is PasswordBox))
            {
                parent = VisualTreeHelper.GetParent(parent);
            }
            
            var command = GetButtonCommand(parent);
            if (command != null && command.CanExecute(parent))
            {
                command.Execute(parent);
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
            }
        }
    }

}
