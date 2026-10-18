// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
// Ported from RatingItemFontInfo.h of microsoft-ui-xaml, Copyright (c) Microsoft Corporation, MIT License.

using System.Windows;

namespace MahApps.Metro.Controls
{
    /// <summary>
    /// Describes the glyphs a <see cref="RatingControl"/> draws for its items, one per state.
    /// </summary>
    public class RatingItemFontInfo : RatingItemInfo
    {
        /// <summary>Identifies the <see cref="Glyph"/> dependency property.</summary>
        public static readonly DependencyProperty GlyphProperty
            = DependencyProperty.Register(nameof(Glyph),
                                          typeof(string),
                                          typeof(RatingItemFontInfo),
                                          new PropertyMetadata(null));

        /// <summary>Gets or sets the glyph of the set items.</summary>
        public string? Glyph
        {
            get => (string?)this.GetValue(GlyphProperty);
            set => this.SetValue(GlyphProperty, value);
        }

        /// <summary>Identifies the <see cref="UnsetGlyph"/> dependency property.</summary>
        public static readonly DependencyProperty UnsetGlyphProperty
            = DependencyProperty.Register(nameof(UnsetGlyph),
                                          typeof(string),
                                          typeof(RatingItemFontInfo),
                                          new PropertyMetadata(null));

        /// <summary>Gets or sets the glyph of the items that are not set, falls back to <see cref="Glyph"/>.</summary>
        public string? UnsetGlyph
        {
            get => (string?)this.GetValue(UnsetGlyphProperty);
            set => this.SetValue(UnsetGlyphProperty, value);
        }

        /// <summary>Identifies the <see cref="PlaceholderGlyph"/> dependency property.</summary>
        public static readonly DependencyProperty PlaceholderGlyphProperty
            = DependencyProperty.Register(nameof(PlaceholderGlyph),
                                          typeof(string),
                                          typeof(RatingItemFontInfo),
                                          new PropertyMetadata(null));

        /// <summary>Gets or sets the glyph of the items of the placeholder value, falls back to <see cref="Glyph"/>.</summary>
        public string? PlaceholderGlyph
        {
            get => (string?)this.GetValue(PlaceholderGlyphProperty);
            set => this.SetValue(PlaceholderGlyphProperty, value);
        }

        /// <summary>Identifies the <see cref="PointerOverGlyph"/> dependency property.</summary>
        public static readonly DependencyProperty PointerOverGlyphProperty
            = DependencyProperty.Register(nameof(PointerOverGlyph),
                                          typeof(string),
                                          typeof(RatingItemFontInfo),
                                          new PropertyMetadata(null));

        /// <summary>Gets or sets the glyph of the items under the pointer while a rating is set, falls back to <see cref="Glyph"/>.</summary>
        public string? PointerOverGlyph
        {
            get => (string?)this.GetValue(PointerOverGlyphProperty);
            set => this.SetValue(PointerOverGlyphProperty, value);
        }

        /// <summary>Identifies the <see cref="PointerOverPlaceholderGlyph"/> dependency property.</summary>
        public static readonly DependencyProperty PointerOverPlaceholderGlyphProperty
            = DependencyProperty.Register(nameof(PointerOverPlaceholderGlyph),
                                          typeof(string),
                                          typeof(RatingItemFontInfo),
                                          new PropertyMetadata(null));

        /// <summary>Gets or sets the glyph of the items under the pointer while no rating is set, falls back to <see cref="PlaceholderGlyph"/>, then <see cref="Glyph"/>.</summary>
        public string? PointerOverPlaceholderGlyph
        {
            get => (string?)this.GetValue(PointerOverPlaceholderGlyphProperty);
            set => this.SetValue(PointerOverPlaceholderGlyphProperty, value);
        }

        /// <summary>Identifies the <see cref="DisabledGlyph"/> dependency property.</summary>
        public static readonly DependencyProperty DisabledGlyphProperty
            = DependencyProperty.Register(nameof(DisabledGlyph),
                                          typeof(string),
                                          typeof(RatingItemFontInfo),
                                          new PropertyMetadata(null));

        /// <summary>Gets or sets the glyph of the set items of a disabled control, falls back to <see cref="Glyph"/>.</summary>
        public string? DisabledGlyph
        {
            get => (string?)this.GetValue(DisabledGlyphProperty);
            set => this.SetValue(DisabledGlyphProperty, value);
        }

        /// <inheritdoc />
        protected override Freezable CreateInstanceCore()
        {
            return new RatingItemFontInfo();
        }
    }
}
