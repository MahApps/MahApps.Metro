// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Windows.Media;
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

        public Storyboard? BadgeChangedStoryboard
        {
            get => (Storyboard?)this.GetValue(BadgeChangedStoryboardProperty);
            set => this.SetValue(BadgeChangedStoryboardProperty, value);
        }

        static Badged()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Badged), new FrameworkPropertyMetadata(typeof(Badged)));
        }

        /// <summary>
        /// A badge hangs over the edge of what it is put on, by half its own size, so it is outside
        /// the bounds of this control by design. The layout clip WPF hands out when an element is
        /// arranged a hair smaller than it asked for would cut all of that away, and a hair is all it
        /// takes: rounding a tab header to whole device pixels is enough. Nothing here needs clipping.
        /// </summary>
        protected override Geometry? GetLayoutClip(Size layoutSlotSize)
        {
            return null;
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
                    throw new MahAppsException("Uups, it seems like there is something wrong with the given BadgeChangedStoryboard.", exception);
                }
            }
        }
    }
}