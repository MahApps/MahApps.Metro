namespace MahApps.Metro.Controls
{
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;

    public class PasswordBoxHelper
    {
        #region Static Fields

        public static readonly DependencyProperty CapsLockWarningTextProperty = DependencyProperty.RegisterAttached(
            "CapsLockWarningText", typeof(string), typeof(PasswordBoxHelper), new PropertyMetadata("Caps lock is on"));
        public static readonly DependencyProperty ShowCapslockWarningProperty = DependencyProperty.RegisterAttached(
            "ShowCapslockWarning", typeof(bool), typeof(PasswordBoxHelper), new PropertyMetadata(default(bool), ShowCapslockWarningChanged));

        #endregion

        #region Public Methods and Operators

        public static string GetCapsLockWarningText(PasswordBox element)
        {
            return (string)element.GetValue(CapsLockWarningTextProperty);
        }

        public static bool GetShowCapslockWarning(PasswordBox element)
        {
            return (bool)element.GetValue(ShowCapslockWarningProperty);
        }

        public static void SetCapsLockWarningText(PasswordBox element, string value)
        {
            element.SetValue(CapsLockWarningTextProperty, value);
        }

        public static void SetShowCapslockWarning(PasswordBox element, bool value)
        {
            element.SetValue(ShowCapslockWarningProperty, value);
        }

        #endregion

        #region Methods

        private static void RefreshCapslockStatus(object sender, RoutedEventArgs e)
        {
            PasswordBox pb = (PasswordBox)sender;
            FrameworkElement fe = pb.Template.FindName("PART_CapsLockIndicator", pb) as FrameworkElement;

            if (fe != null)
            {
                fe.Visibility = Keyboard.IsKeyToggled(Key.CapsLock) ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        private static void ShowCapslockWarningChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            PasswordBox pb = (PasswordBox)d;
            if ((bool)e.NewValue)
            {
                pb.KeyDown += RefreshCapslockStatus;
                pb.GotFocus += RefreshCapslockStatus;
            }
            else
            {
                pb.KeyDown -= RefreshCapslockStatus;
                pb.GotFocus -= RefreshCapslockStatus;
            }
        }

        #endregion
    }
}