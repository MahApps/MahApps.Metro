// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows;
using MahApps.Metro.ValueBoxes;

namespace MahApps.Metro.Controls
{
    public class HamburgerMenuItemBase : Freezable, IHamburgerMenuItemBase
    {
        /// <summary>
        /// Identifies the <see cref="Tag"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty TagProperty = DependencyProperty.Register(nameof(Tag), typeof(object), typeof(HamburgerMenuItemBase), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="IsVisible" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsVisibleProperty = DependencyProperty.Register(nameof(IsVisible), typeof(bool), typeof(HamburgerMenuItemBase), new PropertyMetadata(BooleanBoxes.TrueBox));

        /// <summary>
        /// Gets or sets a value that specifies an user specific value.
        /// </summary>
        public object Tag
        {
            get { return this.GetValue(TagProperty); }

            set { this.SetValue(TagProperty, value); }
        }

        /// <summary>
        /// Gets or sets the value indicating whether this element is visible in the user interface (UI). This is a dependency property.
        /// </summary>
        /// <returns>
        /// true if the item is visible, otherwise false. The default value is true.
        /// </returns>
        public bool IsVisible
        {
            get { return (bool)this.GetValue(IsVisibleProperty); }

            set { this.SetValue(IsVisibleProperty, BooleanBoxes.Box(value)); }
        }

        protected override Freezable CreateInstanceCore()
        {
            return new HamburgerMenuItemBase();
        }
    }
}