using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MahApps.Metro.Controls
{
    public class PasswordBoxHelper
    {
        public static readonly DependencyProperty CapsLockIconProperty
            = DependencyProperty.RegisterAttached("CapsLockIcon",
                                                  typeof(object),
                                                  typeof(PasswordBoxHelper),
                                                  new PropertyMetadata("!", ShowCapslockWarningChanged));
        public static readonly DependencyProperty CapsLockWarningToolTipProperty
            = DependencyProperty.RegisterAttached("CapsLockWarningToolTip",
                                                  typeof(object),
                                                  typeof(PasswordBoxHelper),
                                                  new PropertyMetadata("Caps lock is on"));

        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(PasswordBox))]
        public static object GetCapsLockIcon(PasswordBox element)
        {
            return element.GetValue(CapsLockIconProperty);
        }

        public static void SetCapsLockIcon(PasswordBox element, object value)
        {
            element.SetValue(CapsLockIconProperty, value);
        }

        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(PasswordBox))]
        public static object GetCapsLockWarningToolTip(PasswordBox element)
        {
            return element.GetValue(CapsLockWarningToolTipProperty);
        }

        public static void SetCapsLockWarningToolTip(PasswordBox element, object value)
        {
            element.SetValue(CapsLockWarningToolTipProperty, value);
        }

        private static void ShowCapslockWarningChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != e.OldValue)
            {
                PasswordBox pb = (PasswordBox)d;

                WeakEventManager<PasswordBox, KeyEventArgs>.RemoveHandler(pb, "PreviewKeyDown", RefreshCapslockStatusHandler);
                WeakEventManager<PasswordBox, RoutedEventArgs>.RemoveHandler(pb, "GotFocus", RefreshCapslockStatusHandler);
                WeakEventManager<PasswordBox, KeyboardFocusChangedEventArgs>.RemoveHandler(pb, "PreviewGotKeyboardFocus", RefreshCapslockStatusHandler);
                WeakEventManager<PasswordBox, RoutedEventArgs>.RemoveHandler(pb, "LostFocus", RefreshCapslockStatusHandler);
                if (e.NewValue != null)
                {
                    WeakEventManager<PasswordBox, KeyEventArgs>.AddHandler(pb, "PreviewKeyDown", RefreshCapslockStatusHandler);
                    WeakEventManager<PasswordBox, RoutedEventArgs>.AddHandler(pb, "GotFocus", RefreshCapslockStatusHandler);
                    WeakEventManager<PasswordBox, KeyboardFocusChangedEventArgs>.AddHandler(pb, "PreviewGotKeyboardFocus", RefreshCapslockStatusHandler);
                    WeakEventManager<PasswordBox, RoutedEventArgs>.AddHandler(pb, "LostFocus", RefreshCapslockStatusHandler);
                }
            }
        }

        private static void RefreshCapslockStatusHandler(object sender, RoutedEventArgs e)
        {
            Console.WriteLine($">>> Capslock event = {e.RoutedEvent.Name}");
            RefreshCapslockStatus(sender as PasswordBox, Equals(e.RoutedEvent.Name, "LostFocus"));
        }

        private static void RefreshCapslockStatus(PasswordBox passwordBox, bool forceCollapsedVisibility = false)
        {
            if (null != passwordBox)
            {
                FrameworkElement fe = FindCapsLockIndicator(passwordBox);
                if (fe != null)
                {
                    fe.Visibility = !passwordBox.IsEnabled || forceCollapsedVisibility || !Keyboard.IsKeyToggled(Key.CapsLock) ? Visibility.Collapsed : Visibility.Visible;
                }
            }
        }

        private static FrameworkElement FindCapsLockIndicator(Control pb)
        {
            return pb?.Template?.FindName("PART_CapsLockIndicator", pb) as FrameworkElement;
        }
    }
}