// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using ControlzEx.Windows.Shell;

namespace MahApps.Metro.Controls
{
    public class ContentPresenterEx : ContentPresenter
    {
        static ContentPresenterEx()
        {
            ContentProperty.OverrideMetadata(typeof(ContentPresenterEx), new FrameworkPropertyMetadata(OnContentPropertyChanged));
        }

        private static void OnContentPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue is IInputElement && e.OldValue is DependencyObject oldInputElement)
            {
                BindingOperations.ClearBinding(oldInputElement, WindowChrome.IsHitTestVisibleInChromeProperty);
            }

            if (e.NewValue is IInputElement && e.NewValue is DependencyObject newInputElement)
            {
                BindingOperations.SetBinding(newInputElement, WindowChrome.IsHitTestVisibleInChromeProperty, new Binding { Path = new PropertyPath(WindowChrome.IsHitTestVisibleInChromeProperty), Source = d });
            }
        }
    }
}