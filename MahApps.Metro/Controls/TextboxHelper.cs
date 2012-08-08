using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
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
        private static readonly DependencyProperty HasTextProperty = DependencyProperty.RegisterAttached("HasText", typeof(bool), typeof(TextboxHelper), new FrameworkPropertyMetadata(false));
        public static readonly DependencyProperty TextLengthProperty = DependencyProperty.RegisterAttached("TextLength", typeof(int), typeof(TextboxHelper), new UIPropertyMetadata(0));
        public static readonly DependencyProperty ClearTextButtonProperty = DependencyProperty.RegisterAttached("ClearTextButton", typeof(bool), typeof(TextboxHelper), new FrameworkPropertyMetadata(false, ClearTextChanged));

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
            obj.SetValue(HasTextProperty, value >= 1);
        }

        public bool HasText
        {
            get { return (bool)GetValue(HasTextProperty); }
        }

        static void OnIsMonitoringChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TextBox)
            {
                var txtBox = d as TextBox;

                if ((bool)e.NewValue)
                    txtBox.TextChanged += TextChanged;
                else
                    txtBox.TextChanged -= TextChanged;
            }
            else if (d is PasswordBox)
            {
                var passBox = d as PasswordBox;

                if ((bool)e.NewValue)
                    passBox.PasswordChanged += PasswordChanged;
                else
                    passBox.PasswordChanged -= PasswordChanged;
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

        public static bool GetClearTextButton(DependencyObject d)
        {
            return (bool)d.GetValue(ClearTextButtonProperty);
        }

        public static void SetClearTextButton(DependencyObject obj, bool value)
        {
            obj.SetValue(ClearTextButtonProperty, value);
        }

        private static void ClearTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var textbox = d as TextBox;
            if (textbox != null)
                textbox.Loaded += TextBoxLoaded;
        }

        static void TextBoxLoaded(object sender, RoutedEventArgs e)
        {
            if (!(sender is TextBox))
                return;

            var textbox = sender as TextBox;
            if (textbox.Style == null)
                return;

            var template = textbox.Template;
            if (template == null)
                return;

            var button = template.FindName("PART_ClearText", textbox) as Button;
            if (button == null)
                return;

            if (GetClearTextButton(textbox))
                button.Click += ClearTextClicked;
            else
                button.Click -= ClearTextClicked;
        }

        static void ClearTextClicked(object sender, RoutedEventArgs e)
        {
            var button = ((Button)sender);
            var parent = VisualTreeHelper.GetParent(button);
            while (!(parent is TextBox))
            {
                parent = VisualTreeHelper.GetParent(parent);
            }

            ((TextBox)parent).Text = string.Empty;
        }
    }

}
