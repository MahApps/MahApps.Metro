// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

// ReSharper disable once CheckNamespace
namespace MahApps.Metro.Controls
{
    /// <summary>
    /// Represents an icon that uses a glyph from the specified font.
    /// </summary>
    [TemplatePart(Name = nameof(PART_Glyph), Type = typeof(TextBlock))]
    public class FontIcon : IconElement
    {
        /// <summary>Identifies the <see cref="Glyph"/> dependency property.</summary>
        public static readonly DependencyProperty GlyphProperty
            = DependencyProperty.Register(nameof(Glyph),
                                          typeof(string),
                                          typeof(FontIcon),
                                          new FrameworkPropertyMetadata(string.Empty));

        /// <summary>
        /// Gets or sets the character code that identifies the icon glyph.
        /// </summary>
        /// <returns>The hexadecimal character code for the icon glyph.</returns>
        public string Glyph
        {
            get => (string)this.GetValue(GlyphProperty);
            set => this.SetValue(GlyphProperty, value);
        }

        static FontIcon()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(FontIcon), new FrameworkPropertyMetadata(typeof(FontIcon)));
        }

        private TextBlock? PART_Glyph { get; set; }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            this.PART_Glyph = this.GetTemplateChild(nameof(this.PART_Glyph)) as TextBlock;

            if (this.PART_Glyph is not null && this.InheritsForegroundFromVisualParent)
            {
                this.PART_Glyph.Foreground = this.VisualParentForeground;
            }
        }

        protected override void OnInheritsForegroundFromVisualParentPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnInheritsForegroundFromVisualParentPropertyChanged(e);

            if (this.PART_Glyph is not null)
            {
                if (this.InheritsForegroundFromVisualParent)
                {
                    this.PART_Glyph.Foreground = this.VisualParentForeground;
                }
                else
                {
                    this.PART_Glyph.ClearValue(TextBlock.ForegroundProperty);
                }
            }
        }

        protected override void OnVisualParentForegroundPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnVisualParentForegroundPropertyChanged(e);

            if (this.PART_Glyph is not null && this.InheritsForegroundFromVisualParent)
            {
                this.PART_Glyph.Foreground = e.NewValue as Brush;
            }
        }
    }
}