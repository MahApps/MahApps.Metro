// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows;
using MahApps.Metro.Controls;

namespace MahApps.Metro.Gallery.Core
{
    /// <summary>
    /// A row of the pane that stands for a category rather than for a page. The menu shows it as an
    /// expander with a menu of its own underneath, which is the submenu GH-3018 and GH-4459 asked
    /// for, and it keeps the pages in a property of its own so that <c>Tag</c> stays free.
    /// </summary>
    public class GalleryCategoryItem : HamburgerMenuIconItem
    {
        /// <summary>Identifies the <see cref="IsExpanded"/> dependency property.</summary>
        public static readonly DependencyProperty IsExpandedProperty
            = DependencyProperty.Register(nameof(IsExpanded),
                                          typeof(bool),
                                          typeof(GalleryCategoryItem),
                                          new PropertyMetadata(true));

        /// <summary>
        /// Gets or sets whether the pages of this category are shown. The expander writes back to
        /// it, so that a category which is opened or closed stays that way while the reader walks
        /// the gallery.
        /// </summary>
        public bool IsExpanded
        {
            get => (bool)this.GetValue(IsExpandedProperty);
            set => this.SetValue(IsExpandedProperty, value);
        }

        /// <summary>
        /// The pages under this category, which are menu items like any other.
        /// </summary>
        public HamburgerMenuItemCollection Pages { get; } = new HamburgerMenuItemCollection();
    }
}
