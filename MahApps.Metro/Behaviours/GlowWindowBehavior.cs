using System;
using System.Windows;
using System.Windows.Interactivity;
using MahApps.Metro.Controls;

namespace MahApps.Metro.Behaviours
{
    public class GlowWindowBehavior : Behavior<Window>
    {
        private GlowWindow left, right, top, bottom;

        protected override void OnAttached()
        {
            base.OnAttached();

            this.AssociatedObject.Loaded += AssociatedObjectOnLoaded;
        }

        private void AssociatedObjectOnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            // now glow effect if UseNoneWindowStyle is true or GlowBrush not set
            var metroWindow = this.AssociatedObject as MetroWindow;
            if (metroWindow != null && (metroWindow.UseNoneWindowStyle || metroWindow.GlowBrush == null))
            {
                return;
            }

            this.left = new GlowWindow(this.AssociatedObject, GlowDirection.Left);
            this.right = new GlowWindow(this.AssociatedObject, GlowDirection.Right);
            this.top = new GlowWindow(this.AssociatedObject, GlowDirection.Top);
            this.bottom = new GlowWindow(this.AssociatedObject, GlowDirection.Bottom);

            this.Show();
            this.Update();

            var windowTransitionsEnabled = metroWindow != null && metroWindow.WindowTransitionsEnabled;
            if (!windowTransitionsEnabled)
            {
                // no storyboard so set opacity to 1
                this.SetOpacityTo(1);
            }
            else
            {
                // start the opacity storyboard 0->1
                this.StartOpacityStoryboard();
                // hide the glows if window get invisible state
                this.AssociatedObject.IsVisibleChanged += this.AssociatedObjectIsVisibleChanged;
                // closing always handled
                this.AssociatedObject.Closing += (o, args) => {
                                                     if (!args.Cancel)
                                                     {
                                                         this.AssociatedObject.IsVisibleChanged -= this.AssociatedObjectIsVisibleChanged;
                                                     }
                                                 };
            }
        }

        private void AssociatedObjectIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (!this.AssociatedObject.IsVisible)
            {
                // the associated owner got invisible so set opacity to 0 to start the storyboard by 0 for the next visible state
                this.SetOpacityTo(0);
            }
            else
            {
                this.StartOpacityStoryboard();
            }
        }

        /// <summary>
        /// Updates all glow windows (visible, hidden, collapsed)
        /// </summary>
        private void Update()
        {
            if (this.left != null
                && this.right != null
                && this.top != null
                && this.bottom != null)
            {
                this.left.Update();
                this.right.Update();
                this.top.Update();
                this.bottom.Update();
            }
        }

        /// <summary>
        /// Sets the opacity to all glow windows
        /// </summary>
        private void SetOpacityTo(double newOpacity)
        {
            if (this.left != null
                && this.right != null
                && this.top != null
                && this.bottom != null)
            {
                this.left.Opacity = newOpacity;
                this.right.Opacity = newOpacity;
                this.top.Opacity = newOpacity;
                this.bottom.Opacity = newOpacity;
            }
        }

        /// <summary>
        /// Starts the opacity storyboard 0 -> 1
        /// </summary>
        private void StartOpacityStoryboard()
        {
            if (this.left != null && this.left.OpacityStoryboard != null
                && this.right != null && this.right.OpacityStoryboard != null
                && this.top != null && this.top.OpacityStoryboard != null
                && this.bottom != null && this.bottom.OpacityStoryboard != null)
            {
                this.left.BeginStoryboard(this.left.OpacityStoryboard);
                this.right.BeginStoryboard(this.right.OpacityStoryboard);
                this.top.BeginStoryboard(this.top.OpacityStoryboard);
                this.bottom.BeginStoryboard(this.bottom.OpacityStoryboard);
            }
        }

        /// <summary>
        /// Shows all glow windows
        /// </summary>
        private void Show()
        {
            left.Show();
            right.Show();
            top.Show();
            bottom.Show();
        }
    }
}
