namespace MahApps.Metro.Controls
{
    using System.Windows;

    /// <summary>
    /// The HamburgerMenuItem provides a glyph based implementation for HamburgerMenu entries.
    /// </summary>
    public class HamburgerMenuGlyphItem : HamburgerMenuItem
    {
        /// <summary>
        /// Identifies the <see cref="Glyph"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty GlyphProperty = DependencyProperty.Register(nameof(Glyph), typeof(string), typeof(HamburgerMenuItem), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets gets of sets a value that specifies the glyph to use from Segoe MDL2 Assets font.
        /// </summary>
        public string Glyph
        {
            get
            {
                return (string)GetValue(GlyphProperty);
            }

            set
            {
                SetValue(GlyphProperty, value);
            }
        }
    }
}
