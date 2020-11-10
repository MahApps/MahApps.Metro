// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows;
using System.Windows.Media;

namespace MahApps.Metro.Controls
{
    public class ColorEyePreviewData : DependencyObject
    {
        /// <summary>Identifies the <see cref="PreviewImageProperty"/> dependency property.</summary>
        internal static readonly DependencyPropertyKey PreviewImagePropertyKey
            = DependencyProperty.RegisterReadOnly(nameof(PreviewImage),
                                                  typeof(ImageSource),
                                                  typeof(ColorEyePreviewData),
                                                  new PropertyMetadata(default(ImageSource)));

        /// <summary>Identifies the <see cref="PreviewImage"/> dependency property.</summary>
        public static readonly DependencyProperty PreviewImageProperty = PreviewImagePropertyKey.DependencyProperty;

        /// <summary>
        /// Gets the preview image while the cursor is moving
        /// </summary>
        public ImageSource PreviewImage => (ImageSource)this.GetValue(PreviewImageProperty);

        /// <summary>Identifies the <see cref="PreviewBrushProperty"/> dependency property.</summary>
        internal static readonly DependencyPropertyKey PreviewBrushPropertyKey
            = DependencyProperty.RegisterReadOnly(nameof(PreviewBrush),
                                                  typeof(Brush),
                                                  typeof(ColorEyePreviewData),
                                                  new PropertyMetadata(Brushes.Transparent));

        /// <summary>Identifies the <see cref="PreviewBrush"/> dependency property.</summary>
        public static readonly DependencyProperty PreviewBrushProperty = PreviewBrushPropertyKey.DependencyProperty;

        /// <summary>
        /// Gets the preview brush while the cursor is moving
        /// </summary>
        public Brush PreviewBrush => (Brush)this.GetValue(PreviewBrushProperty);
    }
}