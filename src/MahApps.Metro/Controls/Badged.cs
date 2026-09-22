// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Media;
using System.Windows.Media.Animation;
using ControlzEx;
using MahApps.Metro.Automation.Peers;

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

        /// <inheritdoc />
        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new BadgedAutomationPeer(this);
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

        /// <summary>
        /// Hangs the badge over the corner it is placed at, by half of itself.
        /// </summary>
        /// <remarks>
        /// <see cref="BadgedEx" /> works that out from the container's DesiredSize, and a DesiredSize
        /// has the margin already taken off it. So half of it is half of what is left of the badge
        /// rather than half of the badge, the next pass takes half of that, and the two answers keep
        /// swapping places: a badge that jumps between two spots and a layout pass that never ends.
        /// It only ever looked right because the margin that leaves nothing of the DesiredSize is
        /// the correct one, and a badge that is already sitting there stays there.
        ///
        /// The arranged size is the badge itself, whatever margin it carries, so half of that is the
        /// answer at once and it stays the answer.
        /// </remarks>
        protected override Size ArrangeOverride(Size arrangeBounds)
        {
            var result = base.ArrangeOverride(arrangeBounds);

            if (this._badgeContainer is not null)
            {
                var horizontal = 0 - (this._badgeContainer.ActualWidth / 2);
                var vertical = 0 - (this._badgeContainer.ActualHeight / 2);

                this._badgeContainer.Margin = new Thickness(horizontal, vertical, horizontal, vertical);
            }

            return result;
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