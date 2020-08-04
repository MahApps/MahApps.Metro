// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows;

namespace MahApps.Metro.Controls
{
    /// <summary>
    /// Represents an icon that uses a glyph from the specified font.
    /// </summary>
    public class FontIcon : IconElement
    {
        /// <summary>Identifies the <see cref="Glyph"/> dependency property.</summary>
        public static readonly DependencyProperty GlyphProperty
            = DependencyProperty.Register(
                nameof(Glyph),
                typeof(string),
                typeof(FontIcon),
                new FrameworkPropertyMetadata(string.Empty));

        /// <summary>
        /// Gets or sets the character code that identifies the icon glyph.
        /// </summary>
        /// <returns>The hexadecimal character code for the icon glyph.</returns>
        public string Glyph
        {
            get { return (string)this.GetValue(GlyphProperty); }
            set { this.SetValue(GlyphProperty, value); }
        }

        static FontIcon()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(FontIcon), new FrameworkPropertyMetadata(typeof(FontIcon)));
        }
    }
}