using System;
using System.Windows;
using System.Windows.Interactivity;
using MahApps.Metro.Controls;
using System.Windows.Threading;

namespace MahApps.Metro.Behaviours
{
    public class GlowWindowBehavior : Behavior<Window>
    {
        private static readonly TimeSpan GlowTimerDelay = TimeSpan.FromMilliseconds(200); //200 ms delay, the same as VS2013
        private GlowWindow left, right, top, bottom;
        private DispatcherTimer makeGlowVisibleTimer;
        
        protected override void OnAttached()
        {
            base.OnAttached();

            this.AssociatedObject.Loaded += AssociatedObjectOnLoaded;
            this.AssociatedObject.Unloaded += AssociatedObjectUnloaded;
        }

        void AssociatedObjectStateChanged(object sender, EventArgs e)
        {
            if (makeGlowVisibleTimer != null)
            {
                makeGlowVisibleTimer.Stop();
            }
            if(AssociatedObject.WindowState != WindowState.Minimized)
            {
                var metroWindow = this.AssociatedObject as MetroWindow;
                var ignoreTaskBar = metroWindow != null && metroWindow.IgnoreTaskbarOnMaximize;
                if (makeGlowVisibleTimer != null && SystemParameters.MinimizeAnimation && !ignoreTaskBar)
                {
                    makeGlowVisibleTimer.Start();
                }
                else
                {
                    RestoreGlow();
                }
            }
            else
            {
                HideGlow();
            }
        }

        void AssociatedObjectUnloaded(object sender, RoutedEventArgs e)
        {
            if(makeGlowVisibleTimer != null)
            {
                makeGlowVisibleTimer.Stop();
                makeGlowVisibleTimer.Tick -= makeGlowVisibleTimer_Tick;
                makeGlowVisibleTimer = null;
            }
        }

        private void makeGlowVisibleTimer_Tick(object sender, EventArgs e)
        {
            if(makeGlowVisibleTimer != null)
            {
                makeGlowVisibleTimer.Stop();
            }
            RestoreGlow();
        }

        private void RestoreGlow()
        {
            if (left != null) left.IsGlowing = true;
            if (top != null) top.IsGlowing = true;
            if (right != null) right.IsGlowing = true;
            if (bottom != null) bottom.IsGlowing = true;
            Update();
        }

        private void HideGlow()
        {
            if (left != null) left.IsGlowing = false;
            if (top != null) top.IsGlowing = false;
            if (right != null) right.IsGlowing = false;
            if (bottom != null) bottom.IsGlowing = false;
            Update();
        }

        private void AssociatedObjectOnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            // No glow effect if UseNoneWindowStyle is true or GlowBrush not set.
            var metroWindow = this.AssociatedObject as MetroWindow;
            if (metroWindow != null && (metroWindow.UseNoneWindowStyle || metroWindow.GlowBrush == null))
            {
                return;
            }

            this.AssociatedObject.StateChanged -= AssociatedObjectStateChanged;
            this.AssociatedObject.StateChanged += AssociatedObjectStateChanged;

            if (makeGlowVisibleTimer == null)
            {
                makeGlowVisibleTimer = new DispatcherTimer { Interval = GlowTimerDelay };
                makeGlowVisibleTimer.Tick += makeGlowVisibleTimer_Tick;
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
                this.AssociatedObject.Closing += (o, args) =>
                {
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
            if (left != null) left.Update();
            if (right != null) right.Update();
            if (top != null) top.Update();
            if (bottom != null) bottom.Update();
        }

        /// <summary>
        /// Sets the opacity to all glow windows
        /// </summary>
        private void SetOpacityTo(double newOpacity)
        {
            if (left != null) left.Opacity = newOpacity;
            if (right != null) right.Opacity = newOpacity;
            if (top != null) top.Opacity = newOpacity;
            if (bottom != null) bottom.Opacity = newOpacity;
        }

        /// <summary>
        /// Starts the opacity storyboard 0 -> 1
        /// </summary>
        private void StartOpacityStoryboard()
        {
            if (left != null && this.left.OpacityStoryboard != null) left.BeginStoryboard(this.left.OpacityStoryboard);
            if (right != null && this.right.OpacityStoryboard != null) right.BeginStoryboard(this.right.OpacityStoryboard);
            if (top != null && this.top.OpacityStoryboard != null) top.BeginStoryboard(this.top.OpacityStoryboard);
            if (bottom != null && this.bottom.OpacityStoryboard != null) bottom.BeginStoryboard(this.bottom.OpacityStoryboard);
        }

        /// <summary>
        /// Shows all glow windows
        /// </summary>
        private void Show()
        {
            if (left != null) left.Show();
            if (right != null) right.Show();
            if (top != null) top.Show();
            if (bottom != null) bottom.Show();
        }
    }
}
