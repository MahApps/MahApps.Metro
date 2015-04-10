//	--------------------------------------------------------------------
//		Obtained from: WPFSmartLibrary
//		For more information see : http://wpfsmartlibrary.codeplex.com/
//		(by DotNetMastermind)
//	--------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;

namespace MahApps.Metro.Behaviours
{
    public class PasswordBoxBindingBehavior : Behavior<PasswordBox>
    {
        #region Fields

        private static bool IsUpdating;

        #endregion

        #region DependencyProperty - Password ("string")

        public static string GetPassword(DependencyObject dpo)
        {
            return (string)dpo.GetValue(PasswordProperty);
        }
        public static void SetPassword(DependencyObject dpo, string value)
        {
            dpo.SetValue(PasswordProperty, value);
        }
        /// <summary>
        /// Gets or sets the bindable Password property on the PasswordBox control. This is a dependency property.
        /// </summary>
        public static readonly DependencyProperty PasswordProperty =
                    DependencyProperty.RegisterAttached("Password", typeof(string),
                                                                     typeof(PasswordBoxBindingBehavior),
                                                                     new FrameworkPropertyMetadata(String.Empty,
                                                                     new PropertyChangedCallback(OnPasswordPropertyChanged)));
        /// <summary>
        /// Handles changes to the 'Password' attached property.
        /// </summary>
        private static void OnPasswordPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            PasswordBox targetPasswordBox = sender as PasswordBox;
            if (targetPasswordBox != null)
            {
                targetPasswordBox.PasswordChanged -= PasswordBox_PasswordChanged;
                if (IsUpdating == false)
                {
                    targetPasswordBox.Password = (string)e.NewValue;
                }
                targetPasswordBox.PasswordChanged += PasswordBox_PasswordChanged;
            }
        }

        /// <summary>
        /// Handle the 'PasswordChanged'-event on the PasswordBox
        /// </summary>
        private static void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            PasswordBox passwordBox = sender as PasswordBox;
            IsUpdating = true;
            SetPassword(passwordBox, passwordBox.Password);
            IsUpdating = false;
        }

        #endregion
    }
}