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
                                                                        (d, e) => ((FlipViewItem)d).ExecuteWhenLoaded(() => ((FlipViewItem)d).Owner?.SetCurrentValue(FlipView.BannerTextProperty, e.NewValue))));

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

        public FlipView Owner
        {
            get => (FlipView)this.GetValue(OwnerProperty);
            protected set => this.SetValue(OwnerPropertyKey, value);
        }

        static FlipViewItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(FlipViewItem), new FrameworkPropertyMetadata(typeof(FlipViewItem)));
        }

        /// <inheritdoc />
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            var flipView = ItemsControl.ItemsControlFromItemContainer(this) as FlipView;
            this.SetValue(OwnerPropertyKey, flipView);
        }
    }
}