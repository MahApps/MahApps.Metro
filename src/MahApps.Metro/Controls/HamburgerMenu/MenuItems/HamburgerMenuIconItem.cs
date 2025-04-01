﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows;

namespace MahApps.Metro.Controls
{
    /// <summary>
    /// The HamburgerMenuIconItem provides an icon based implementation for HamburgerMenu entries.
    /// </summary>
    public class HamburgerMenuIconItem : HamburgerMenuItem
    {
        /// <summary>
        /// Identifies the <see cref="Icon"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IconProperty
            = DependencyProperty.Register(nameof(Icon),
                                          typeof(object),
                                          typeof(HamburgerMenuIconItem),
                                          new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets a value that specifies an user specific object which can be used as icon.
        /// </summary>
        public object? Icon
        {
            get => this.GetValue(IconProperty);
            set => this.SetValue(IconProperty, value);
        }

        protected override Freezable CreateInstanceCore()
        {
            return new HamburgerMenuIconItem();
        }
    }
}