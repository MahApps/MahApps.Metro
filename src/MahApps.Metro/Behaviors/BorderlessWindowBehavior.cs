// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows;
using System.Windows.Data;
using ControlzEx.Behaviors;
using MahApps.Metro.Controls;
using ControlzEx.Windows.Shell;

namespace MahApps.Metro.Behaviors
{
    public class BorderlessWindowBehavior : WindowChromeBehavior
    {
        protected override void OnAttached()
        {
            BindingOperations.SetBinding(this, IgnoreTaskbarOnMaximizeProperty, new Binding { Path = new PropertyPath(MetroWindow.IgnoreTaskbarOnMaximizeProperty), Source = this.AssociatedObject });
            BindingOperations.SetBinding(this, ResizeBorderThicknessProperty, new Binding { Path = new PropertyPath(MetroWindow.ResizeBorderThicknessProperty), Source = this.AssociatedObject });
            BindingOperations.SetBinding(this, TryToBeFlickerFreeProperty, new Binding { Path = new PropertyPath(MetroWindow.TryToBeFlickerFreeProperty), Source = this.AssociatedObject });
            BindingOperations.SetBinding(this, KeepBorderOnMaximizeProperty, new Binding { Path = new PropertyPath(MetroWindow.KeepBorderOnMaximizeProperty), Source = this.AssociatedObject });
            BindingOperations.SetBinding(this, EnableMinimizeProperty, new Binding { Path = new PropertyPath(MetroWindow.ShowMinButtonProperty), Source = this.AssociatedObject });
            BindingOperations.SetBinding(this, EnableMaxRestoreProperty, new Binding { Path = new PropertyPath(MetroWindow.ShowMaxRestoreButtonProperty), Source = this.AssociatedObject });

            base.OnAttached();
        }

        protected override void OnDetaching()
        {
            BindingOperations.ClearBinding(this, IgnoreTaskbarOnMaximizeProperty);
            BindingOperations.ClearBinding(this, ResizeBorderThicknessProperty);
            BindingOperations.ClearBinding(this, TryToBeFlickerFreeProperty);
            BindingOperations.ClearBinding(this, KeepBorderOnMaximizeProperty);
            BindingOperations.ClearBinding(this, EnableMinimizeProperty);
            BindingOperations.ClearBinding(this, EnableMaxRestoreProperty);

            base.OnDetaching();
        }

        protected override void AssociatedObject_Loaded(object sender, RoutedEventArgs e)
        {
            if (sender is MetroWindow window)
            {
                window.SetIsHitTestVisibleInChromeProperty<UIElement>("PART_Icon");

                if (window.ResizeMode != ResizeMode.NoResize)
                {
                    window.SetWindowChromeResizeGripDirection("WindowResizeGrip", ResizeGripDirection.BottomRight);
                }
            }
        }
    }
}