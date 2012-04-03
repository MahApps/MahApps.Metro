using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace MahApps.Metro.Controls
{
    public static class TextboxHelper
    {
        public static readonly DependencyProperty ClearTextButtonProperty = DependencyProperty.RegisterAttached("ClearTextButton", typeof(bool), typeof(TextboxHelper), new FrameworkPropertyMetadata(false, ClearTextChanged));
        public static readonly DependencyProperty WatermarkProperty = DependencyProperty.RegisterAttached("Watermark", typeof(string), typeof(TextboxHelper), new FrameworkPropertyMetadata(string.Empty));

        public static string GetWatermark(DependencyObject d)
        {
            return (string)d.GetValue(WatermarkProperty);
        }

        public static void SetWatermark(DependencyObject obj, string value)
        {
            obj.SetValue(WatermarkProperty, value);
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

            var setter =textbox.Style.Setters.FirstOrDefault(s => ((Setter) s).Property.ToString() == "Template") as Setter;

            if (setter == null) 
                return;

            var template = setter.Value as ControlTemplate;
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
            var button = ((Button) sender);
            var parent = VisualTreeHelper.GetParent(button);
            while (!(parent is TextBox))
            {
                parent = VisualTreeHelper.GetParent(parent);
            }

            ((TextBox)parent).Text = string.Empty;
        }
    }
}
