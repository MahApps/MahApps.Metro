// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows;
using System.Windows.Controls;

namespace MahApps.Metro.Controls
{
    public class FlipViewItem : ContentControl
    {
        /// <summary>Identifies the <see cref="BannerText"/> dependency property.</summary>
        public static readonly DependencyProperty BannerTextProperty
            = DependencyProperty.Register(nameof(BannerText),
                                          typeof(object),
                                          typeof(FlipViewItem),
                                          new FrameworkPropertyMetadata("Banner",
                                                                        FrameworkPropertyMetadataOptions.AffectsRender,
                                                                        (d, e) => ((FlipViewItem)d).ExecuteWhenLoaded(() => ((FlipViewItem)d).TellTheOwnerAboutTheBanner(e.NewValue))));

        /// <summary>
        /// Gets or sets the banner text.
        /// </summary>
        public object BannerText
        {
            get => this.GetValue(BannerTextProperty);
            set => this.SetValue(BannerTextProperty, value);
        }

        /// <summary>Identifies the <see cref="Owner"/> dependency property.</summary>
        private static readonly DependencyPropertyKey OwnerPropertyKey =
            DependencyProperty.RegisterReadOnly(nameof(Owner),
                                                typeof(FlipView),
                                                typeof(FlipViewItem),
                                                new PropertyMetadata(null));

        /// <summary>Identifies the <see cref="Owner"/> dependency property.</summary>
        public static readonly DependencyProperty OwnerProperty = OwnerPropertyKey.DependencyProperty;

        public FlipView? Owner
        {
            get => (FlipView?)this.GetValue(OwnerProperty);
            protected set => this.SetValue(OwnerPropertyKey, value);
        }

        /// <summary>
        /// Hands the banner to the control, but only while this item is the one on show. The control
        /// shows its selected item in a presenter, and in the middle of a flip that presenter holds
        /// the item on its way out as well. That one loses its data context as it goes, its banner
        /// falls back to the text it started with, and letting it speak would wipe out the banner of
        /// the item that has just arrived.
        /// </summary>
        private void TellTheOwnerAboutTheBanner(object? banner)
        {
            var owner = this.Owner;
            if (owner is null)
            {
                return;
            }

            // written out one by one an item is the selected item itself, out of a source it carries it
            if (ReferenceEquals(this, owner.SelectedItem) || ReferenceEquals(this.DataContext, owner.SelectedItem))
            {
                owner.SetCurrentValue(FlipView.BannerTextProperty, banner);
            }
        }

        static FlipViewItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(FlipViewItem), new FrameworkPropertyMetadata(typeof(FlipViewItem)));
        }

        /// <inheritdoc />
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            // Written out one by one, an item is its own container and the control it belongs to is
            // right there. Put into an item template instead, it is content rather than a container,
            // and since the control shows its selected item in a presenter and wraps nothing in a
            // container of its own, that is the only place an item out of a source can name a
            // banner. Looking up the tree is what finds the control in that case.
            var flipView = ItemsControl.ItemsControlFromItemContainer(this) as FlipView ?? this.TryFindParent<FlipView>();
            this.SetValue(OwnerPropertyKey, flipView);
        }
    }
}