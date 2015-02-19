using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;
using System.Windows.Media;
using MahApps.Metro.Controls;

namespace MahApps.Metro.Behaviours
{
    /// <summary>
    /// This behavior can be used within TextBox, PasswordBox and ComboBox styles for the ClearText button
    /// </summary>
    public class ClearTextButtonBehavior : Behavior<Button>
    {
        protected override void OnAttached()
        {
            base.OnAttached();

            if (AssociatedObject != null)
            {
                AssociatedObject.Click -= ButtonClicked;
                AssociatedObject.Click += ButtonClicked;
            }
        }

        protected override void OnDetaching()
        {
            if (AssociatedObject != null)
            {
                AssociatedObject.Click -= ButtonClicked;
            }
            base.OnDetaching();
        }

        public static void ButtonClicked(object sender, RoutedEventArgs e)
        {
            var button = ((Button)sender);
            var parent = VisualTreeHelper.GetParent(button);
            while (!(parent is TextBox || parent is PasswordBox || parent is ComboBox))
            {
                parent = VisualTreeHelper.GetParent(parent);
            }

            var command = TextBoxHelper.GetButtonCommand(parent);
            if (command != null && command.CanExecute(parent))
            {
                var commandParameter = TextBoxHelper.GetButtonCommandParameter(parent);

                command.Execute(commandParameter ?? parent);
            }

            if (TextBoxHelper.GetClearTextButton(parent))
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
    }
}