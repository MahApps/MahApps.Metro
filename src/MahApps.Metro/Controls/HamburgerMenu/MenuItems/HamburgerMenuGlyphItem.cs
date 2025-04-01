﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows;

namespace MahApps.Metro.Controls
{
    /// <summary>
    /// The HamburgerMenuGlyphItem provides a glyph based implementation for HamburgerMenu entries.
    /// </summary>
    public class HamburgerMenuGlyphItem : HamburgerMenuItem
    {
        /// <summary>
        /// Identifies the <see cref="Glyph"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty GlyphProperty
            = DependencyProperty.Register(nameof(Glyph),
                                          typeof(string),
                                          typeof(HamburgerMenuGlyphItem),
                                          new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets a value that specifies the glyph to use from a font like Segoe MDL2 Assets font.
        /// </summary>
        public string? Glyph
        {
            get => (string?)this.GetValue(GlyphProperty);

            set => this.SetValue(GlyphProperty, value);
        }

        protected override Freezable CreateInstanceCore()
        {
            return new HamburgerMenuGlyphItem();
        }
    }
}