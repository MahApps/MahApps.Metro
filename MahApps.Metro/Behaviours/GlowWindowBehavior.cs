using System.Windows;
using System.Windows.Interactivity;
using System.Windows.Media.Animation;
using MahApps.Metro.Controls;

namespace MahApps.Metro.Behaviours
{
    public class GlowWindowBehavior : Behavior<Window>
    {
        private GlowWindow left, right, top, bottom;

        protected override void OnAttached()
        {
            base.OnAttached();

            // now glow effect if UseNoneWindowStyle is true or GlowBrush not set
            var metroWindow = this.AssociatedObject as MetroWindow;
            if (metroWindow != null && (metroWindow.UseNoneWindowStyle || metroWindow.GlowBrush == null))
            {
                return;
            }

            this.AssociatedObject.Loaded += (sender, e) =>
            {
                this.left = new GlowWindow(this.AssociatedObject, GlowDirection.Left);
                this.right = new GlowWindow(this.AssociatedObject, GlowDirection.Right);
                this.top = new GlowWindow(this.AssociatedObject, GlowDirection.Top);
                this.bottom = new GlowWindow(this.AssociatedObject, GlowDirection.Bottom);

                this.Show();

                this.left.Update();
                this.right.Update();
                this.top.Update();
                this.bottom.Update();

                var windowTransitionsEnabled = metroWindow != null && metroWindow.WindowTransitionsEnabled;
                if (!windowTransitionsEnabled) {
                    this.left.Opacity = 1;
                    this.right.Opacity = 1;
                    this.top.Opacity = 1;
                    this.bottom.Opacity = 1;
                }
                else
                {
                    if (this.left.OpacityStoryboard != null
                        && this.right.OpacityStoryboard != null
                        && this.top.OpacityStoryboard != null
                        && this.bottom.OpacityStoryboard != null)
                    {
                        this.left.BeginStoryboard(this.left.OpacityStoryboard);
                        this.right.BeginStoryboard(this.right.OpacityStoryboard);
                        this.top.BeginStoryboard(this.top.OpacityStoryboard);
                        this.bottom.BeginStoryboard(this.bottom.OpacityStoryboard);
                    }
                }
            };

            this.AssociatedObject.Closed += (sender, args) =>
            {
                if (left != null) left.Close();
                if (right != null) right.Close();
                if (top != null) top.Close();
                if (bottom != null) bottom.Close();
            };
        }

        public void Show()
        {
            left.Show();
            right.Show();
            top.Show();
            bottom.Show();
        }
    }
}
