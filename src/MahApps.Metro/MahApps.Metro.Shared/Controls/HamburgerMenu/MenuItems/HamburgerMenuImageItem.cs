namespace MahApps.Metro.Controls
{
    using System.Windows;
    using System.Windows.Media.Imaging;

    /// <summary>
    /// The HamburgerMenuItem provides an image based implementation for HamburgerMenu entries.
    /// </summary>
    public class HamburgerMenuImageItem : HamburgerMenuItem
    {
        /// <summary>
        /// Identifies the <see cref="Thumbnail"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ThumbnailProperty = DependencyProperty.Register(nameof(Thumbnail), typeof(BitmapImage), typeof(HamburgerMenuItem), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets gets of sets a value that specifies the glyph to use from Segoe MDL2 Assets font.
        /// </summary>
        public BitmapImage Thumbnail
        {
            get
            {
                return (BitmapImage)GetValue(ThumbnailProperty);
            }

            set
            {
                SetValue(ThumbnailProperty, value);
            }
        }
    }
}
