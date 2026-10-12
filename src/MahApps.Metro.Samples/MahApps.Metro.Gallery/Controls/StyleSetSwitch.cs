// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Windows.Controls;
using MahApps.Metro.Controls;
using MahApps.Metro.Gallery.Core;

namespace MahApps.Metro.Gallery.Controls
{
    /// <summary>
    /// The button in the heading of a page that puts all of its samples into the Metro, the Windows 10
    /// or the WinUI set. It shows the set that is on, and its menu the three to choose from.
    /// </summary>
    public class StyleSetSwitch : DropDownButton
    {
        /// <summary>Identifies the <see cref="StyleSet"/> dependency property.</summary>
        public static readonly DependencyProperty StyleSetProperty
            = DependencyProperty.Register(nameof(StyleSet),
                                          typeof(GalleryStyleSet),
                                          typeof(StyleSetSwitch),
                                          new FrameworkPropertyMetadata(GalleryStyleSet.Metro, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnStyleSetChanged));

        public StyleSetSwitch()
        {
            foreach (GalleryStyleSet set in Enum.GetValues(typeof(GalleryStyleSet)))
            {
                var item = new MenuItem { Header = StyleSets.NameOf(set), Tag = set };
                item.Click += (_, _) => this.SetCurrentValue(StyleSetProperty, set);
                this.Items.Add(item);
            }

            this.Update();
        }

        /// <summary>
        /// Gets or sets the set that is on.
        /// </summary>
        public GalleryStyleSet StyleSet
        {
            get => (GalleryStyleSet)this.GetValue(StyleSetProperty);
            set => this.SetValue(StyleSetProperty, value);
        }

        private static void OnStyleSetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as StyleSetSwitch)?.Update();
        }

        private void Update()
        {
            this.Content = StyleSets.NameOf(this.StyleSet);
            foreach (MenuItem item in this.Items)
            {
                item.IsChecked = Equals(item.Tag, this.StyleSet);
            }
        }
    }
}
