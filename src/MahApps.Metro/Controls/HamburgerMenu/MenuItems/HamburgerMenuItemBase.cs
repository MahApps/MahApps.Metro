// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows;
using MahApps.Metro.ValueBoxes;

namespace MahApps.Metro.Controls
{
    /// <summary>
    /// The base of everything a <see cref="HamburgerMenu"/> shows in its pane.
    /// </summary>
    /// <remarks>
    /// An item is a content element rather than a plain data object, and the menu takes it into its
    /// logical tree. That is what lets a <c>DynamicResource</c> on one of its properties find the
    /// dictionaries above the menu and hear about it when one of them is exchanged, which is what a
    /// menu whose labels come from a language dictionary needs.
    /// </remarks>
    public class HamburgerMenuItemBase : FrameworkContentElement, IHamburgerMenuItemBase
    {
        /// <summary>
        /// Identifies the <see cref="IsVisible" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsVisibleProperty
            = DependencyProperty.Register(nameof(IsVisible),
                                          typeof(bool),
                                          typeof(HamburgerMenuItemBase),
                                          new PropertyMetadata(BooleanBoxes.TrueBox));

        /// <summary>
        /// Gets or sets the value indicating whether this element is visible in the user interface (UI). This is a dependency property.
        /// </summary>
        /// <returns>
        /// true if the item is visible, otherwise false. The default value is true.
        /// </returns>
        public bool IsVisible
        {
            get => (bool)this.GetValue(IsVisibleProperty);
            set => this.SetValue(IsVisibleProperty, BooleanBoxes.Box(value));
        }
    }
}
