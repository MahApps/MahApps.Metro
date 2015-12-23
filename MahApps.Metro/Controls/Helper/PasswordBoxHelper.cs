namespace MahApps.Metro.Controls
{
    using System.ComponentModel;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;

    public class PasswordBoxHelper
    {
        #region Static Fields
        
        public static readonly DependencyProperty CapsLockIconProperty = DependencyProperty.RegisterAttached(
            "CapsLockIcon", typeof(object), typeof(PasswordBoxHelper), new PropertyMetadata("!", ShowCapslockWarningChanged));
        public static readonly DependencyProperty CapsLockWarningToolTipProperty = DependencyProperty.RegisterAttached(
            "CapsLockWarningToolTip", typeof(object), typeof(PasswordBoxHelper), new PropertyMetadata("Caps lock is on"));

        #endregion

        #region Public Methods and Operators

        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(PasswordBox))]
        public static object GetCapsLockIcon(PasswordBox element)
        {
            return element.GetValue(CapsLockIconProperty);
        }

        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(PasswordBox))]
        public static object GetCapsLockWarningToolTip(PasswordBox element)
        {
            return element.GetValue(CapsLockWarningToolTipProperty);
        }

        public static void SetCapsLockIcon(PasswordBox element, object value)
        {
            element.SetValue(CapsLockIconProperty, value);
        }

        public static void SetCapsLockWarningToolTip(PasswordBox element, object value)
        {
            element.SetValue(CapsLockWarningToolTipProperty, value);
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