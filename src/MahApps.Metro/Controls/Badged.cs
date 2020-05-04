using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using ControlzEx;

namespace MahApps.Metro.Controls
{
    [TemplatePart(Name = BadgeContainerPartName, Type = typeof(UIElement))]
    public class Badged : BadgedEx
    {
        /// <summary>Identifies the <see cref="BadgeChangedStoryboard"/> dependency property.</summary>
        public static readonly DependencyProperty BadgeChangedStoryboardProperty
            = DependencyProperty.Register(nameof(BadgeChangedStoryboard),
                                          typeof(Storyboard),
                                          typeof(Badged),
                                          new PropertyMetadata(default(Storyboard)));

        /// <summary>Identifies the <see cref="BadgeTemplate"/> dependency property.</summary>
        public static readonly DependencyProperty BadgeTemplateProperty 
            = DependencyProperty.Register(nameof(BadgeTemplate), 
                                          typeof(DataTemplate), 
                                          typeof(Badged), 
                                          new PropertyMetadata(null));

        /// <summary>Identifies the <see cref="BadgeTemplateSelector"/> dependency property.</summary>
        public static readonly DependencyProperty BadgeTemplateSelectorProperty 
            = DependencyProperty.Register(nameof(BadgeTemplateSelector), 
                                          typeof(DataTemplateSelector), 
                                          typeof(Badged),
                                          new PropertyMetadata(null));


        public Storyboard BadgeChangedStoryboard
        {
            get => (Storyboard)this.GetValue(BadgeChangedStoryboardProperty);
            set => this.SetValue(BadgeChangedStoryboardProperty, value);
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



        /// <summary>
        /// Gets or Sets the <see cref="DataTemplate"/> for the Badge
        /// </summary>
        public DataTemplate BadgeTemplate
        {
            get { return (DataTemplate)GetValue(BadgeTemplateProperty); }
            set { SetValue(BadgeTemplateProperty, value); }
        }

        /// <summary>
        /// Gets or Sets the <see cref="DataTemplateSelector"/> for the Badge
        /// </summary>
        public DataTemplateSelector BadgeTemplateSelector
        {
            get { return (DataTemplateSelector)GetValue(BadgeTemplateSelectorProperty); }
            set { SetValue(BadgeTemplateSelectorProperty, value); }
        }

        



    }
}