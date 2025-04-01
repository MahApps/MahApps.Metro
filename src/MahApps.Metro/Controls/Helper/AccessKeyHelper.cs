// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows;
using System.Windows.Input;
using MahApps.Metro.ValueBoxes;

namespace MahApps.Metro.Controls.Helper
{
    /// <summary>
    /// Class used to manage generic scoping of access keys
    /// </summary>
    internal static class AccessKeyHelper
    {
        /// <summary>
        /// Identifies the IsAccessKeyScope attached dependency property
        /// </summary>
        public static readonly DependencyProperty IsAccessKeyScopeProperty
            = DependencyProperty.RegisterAttached("IsAccessKeyScope",
                                                  typeof(bool),
                                                  typeof(AccessKeyHelper),
                                                  new PropertyMetadata(BooleanBoxes.FalseBox, HandleIsAccessKeyScopePropertyChanged));

        /// <summary>
        /// Sets the IsAccessKeyScope attached property value for the specified object
        /// </summary>
        /// <param name="obj">The object to retrieve the value for</param>
        /// <param name="value">Whether the object is an access key scope</param>
        public static void SetIsAccessKeyScope(DependencyObject obj, bool value)
        {
            obj.SetValue(IsAccessKeyScopeProperty, BooleanBoxes.Box(value));
        }

        /// <summary>
        /// Gets the value of the IsAccessKeyScope attached property for the specified object
        /// </summary>
        /// <param name="obj">The object to retrieve the value for</param>
        /// <returns>The value of IsAccessKeyScope attached property for the specified object</returns>
        public static bool GetIsAccessKeyScope(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsAccessKeyScopeProperty);
        }

        private static void HandleIsAccessKeyScopePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue.Equals(true))
            {
                AccessKeyManager.AddAccessKeyPressedHandler(d, HandleScopedElementAccessKeyPressed);
            }
            else
            {
                AccessKeyManager.RemoveAccessKeyPressedHandler(d, HandleScopedElementAccessKeyPressed);
            }
        }

        private static void HandleScopedElementAccessKeyPressed(object sender, AccessKeyPressedEventArgs e)
        {
            if (!e.Handled && sender is DependencyObject dependencyObject && GetIsAccessKeyScope(dependencyObject))
            {
                e.Scope = sender;
                e.Handled = true;
            }
        }
    }
}