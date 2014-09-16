namespace MahApps.Metro.Controls
{
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;

    public class PasswordBoxHelper
    {
        #region Static Fields
        
        public static readonly DependencyProperty CapslockIconProperty = DependencyProperty.RegisterAttached(
            "CapsLockIcon", typeof(object), typeof(PasswordBoxHelper), new PropertyMetadata("-", ShowCapslockWarningChanged));
        public static readonly DependencyProperty CapsLockWarningTextProperty = DependencyProperty.RegisterAttached(
            "CapsLockWarningText", typeof(object), typeof(PasswordBoxHelper), new PropertyMetadata("Caps lock is on"));

        #endregion

        #region Public Methods and Operators

        public static object GetCapsLockIcon(PasswordBox element)
        {
            return element.GetValue(CapslockIconProperty);
        }

        public static object GetCapsLockWarningText(PasswordBox element)
        {
            return element.GetValue(CapsLockWarningTextProperty);
        }

        public static void SetCapsLockIcon(PasswordBox element, object value)
        {
            element.SetValue(CapslockIconProperty, value);
        }

        public static void SetCapsLockWarningText(PasswordBox element, object value)
        {
            element.SetValue(CapsLockWarningTextProperty, value);
        }

        #endregion

        #region Methods

        private static void RefreshCapslockStatus(object sender, RoutedEventArgs e)
        {
            FrameworkElement fe = FindCapsLockIndicator((Control)sender);

            if (fe != null)
            {
                fe.Visibility = Keyboard.IsKeyToggled(Key.CapsLock) ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        private static FrameworkElement FindCapsLockIndicator(Control pb)
        {
            return pb.Template.FindName("PART_CapsLockIndicator", pb) as FrameworkElement;
        }

        private static void HandlePasswordBoxLostFocus(object sender, RoutedEventArgs e)
        {
            FrameworkElement fe = FindCapsLockIndicator((Control)sender);

            if (fe != null)
            {
                fe.Visibility = Visibility.Collapsed;
            }
        }

        private static void ShowCapslockWarningChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            PasswordBox pb = (PasswordBox)d;
            pb.KeyDown -= RefreshCapslockStatus;
            pb.GotFocus -= RefreshCapslockStatus;
            pb.LostFocus -= HandlePasswordBoxLostFocus;

            if (e.NewValue != null)
            {
                pb.KeyDown += RefreshCapslockStatus;
                pb.GotFocus += RefreshCapslockStatus;
                pb.LostFocus += HandlePasswordBoxLostFocus;
            }
        }

        #endregion
    }
}