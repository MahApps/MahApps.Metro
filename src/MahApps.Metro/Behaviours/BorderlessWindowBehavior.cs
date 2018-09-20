namespace MahApps.Metro.Behaviours
{
    using System.Windows;
    using System.Windows.Data;
    using ControlzEx.Behaviors;
    using MahApps.Metro.Controls;
    using ControlzEx.Windows.Shell;

    public class BorderlessWindowBehavior : WindowChromeBehavior
    {
        protected override void OnAttached()
        {
            BindingOperations.SetBinding(this, IgnoreTaskbarOnMaximizeProperty, new Binding { Path = new PropertyPath(MetroWindow.IgnoreTaskbarOnMaximizeProperty), Source = this.AssociatedObject });
            BindingOperations.SetBinding(this, ResizeBorderThicknessProperty, new Binding { Path = new PropertyPath(MetroWindow.ResizeBorderThicknessProperty), Source = this.AssociatedObject });
            BindingOperations.SetBinding(this, TryToBeFlickerFreeProperty, new Binding { Path = new PropertyPath(MetroWindow.TryToBeFlickerFreeProperty), Source = this.AssociatedObject });
            BindingOperations.SetBinding(this, KeepBorderOnMaximizeProperty, new Binding { Path = new PropertyPath(MetroWindow.KeepBorderOnMaximizeProperty), Source = this.AssociatedObject });

            base.OnAttached();
        }

        protected override void OnDetaching()
        {
            BindingOperations.ClearBinding(this, IgnoreTaskbarOnMaximizeProperty);
            BindingOperations.ClearBinding(this, ResizeBorderThicknessProperty);
            BindingOperations.ClearBinding(this, TryToBeFlickerFreeProperty);
            BindingOperations.ClearBinding(this, KeepBorderOnMaximizeProperty);

            base.OnDetaching();
        }

        protected override void AssociatedObject_Loaded(object sender, RoutedEventArgs e)
        {
            var window = sender as MetroWindow;
            if (window == null)
            {
                return;
            }

            if (window.ResizeMode != ResizeMode.NoResize)
            {
                //window.SetIsHitTestVisibleInChromeProperty<Border>("PART_Border");
                window.SetIsHitTestVisibleInChromeProperty<UIElement>("PART_Icon");
                window.SetWindowChromeResizeGripDirection("WindowResizeGrip", ResizeGripDirection.BottomRight);
            }
        }
    }
}