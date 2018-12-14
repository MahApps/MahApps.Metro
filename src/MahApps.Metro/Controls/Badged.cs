using System;
using System.Windows;
using System.Windows.Media.Animation;
using ControlzEx;

namespace MahApps.Metro.Controls
{
    [TemplatePart(Name = BadgeContainerPartName, Type = typeof(UIElement))]
    public class Badged : BadgedEx
    {
        public static readonly DependencyProperty BadgeChangedStoryboardProperty = DependencyProperty.Register(
            "BadgeChangedStoryboard", typeof(Storyboard), typeof(Badged), new PropertyMetadata(default(Storyboard)));

        public Storyboard BadgeChangedStoryboard
        {
            get { return (Storyboard)this.GetValue(BadgeChangedStoryboardProperty); }
            set { this.SetValue(BadgeChangedStoryboardProperty, value); }
        }

        static Badged()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Badged), new FrameworkPropertyMetadata(typeof(Badged)));
        }

        public override void OnApplyTemplate()
        {
            this.BadgeChanged -= this.OnBadgeChanged;

            base.OnApplyTemplate();

            this.BadgeChanged += this.OnBadgeChanged;
        }

        private void OnBadgeChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            var sb = this.BadgeChangedStoryboard;
            if (this._badgeContainer != null && sb != null)
            {
                try
                {
                    this._badgeContainer.BeginStoryboard(sb);
                }
                catch (Exception exception)
                {
                    throw new MahAppsException("Uups, it seems like there is something wrong with the given Storyboard.", exception);
                }
            }
        }
    }
}
