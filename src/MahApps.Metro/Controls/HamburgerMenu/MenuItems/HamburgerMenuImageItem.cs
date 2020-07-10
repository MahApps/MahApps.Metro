// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows;
using System.Windows.Media;

namespace MahApps.Metro.Controls
{
    /// <summary>
    /// The HamburgerMenuImageItem provides an image based implementation for HamburgerMenu entries.
    /// </summary>
    public class HamburgerMenuImageItem : HamburgerMenuItem
    {
        /// <summary>
        /// Identifies the <see cref="Thumbnail"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ThumbnailProperty = DependencyProperty.Register(nameof(Thumbnail), typeof(ImageSource), typeof(HamburgerMenuImageItem), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets a value that specifies a bitmap to display with an Image control.
        /// </summary>
        public ImageSource Thumbnail
        {
            get { return (ImageSource)GetValue(ThumbnailProperty); }

            set { SetValue(ThumbnailProperty, value); }
        }

        protected override Freezable CreateInstanceCore()
        {
            return new HamburgerMenuImageItem();
        }
    }
}