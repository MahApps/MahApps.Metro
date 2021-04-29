// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace MahApps.Metro.Controls
{
    public class MultiFrameImage : Image
    {
        /// <summary>Identifies the <see cref="MultiFrameImageMode"/> dependency property.</summary>
        public static readonly DependencyProperty MultiFrameImageModeProperty
            = DependencyProperty.Register(nameof(MultiFrameImageMode),
                                          typeof(MultiFrameImageMode),
                                          typeof(MultiFrameImage),
                                          new FrameworkPropertyMetadata(MultiFrameImageMode.ScaleDownLargerFrame, FrameworkPropertyMetadataOptions.AffectsRender));

        /// <summary>
        /// Gets or sets the mode for the rendered image.
        /// </summary>
        public MultiFrameImageMode MultiFrameImageMode
        {
            get => (MultiFrameImageMode)this.GetValue(MultiFrameImageModeProperty);
            set => this.SetValue(MultiFrameImageModeProperty, value);
        }

        static MultiFrameImage()
        {
            SourceProperty.OverrideMetadata(typeof(MultiFrameImage), new FrameworkPropertyMetadata(OnSourceChanged));
        }

        private static void OnSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var multiFrameImage = (MultiFrameImage)d;
            multiFrameImage.UpdateFrameList();
        }

        private readonly List<BitmapSource> frames = new List<BitmapSource>();

        private void UpdateFrameList()
        {
            this.frames.Clear();

            var decoder = (this.Source as BitmapFrame)?.Decoder;
            if (decoder is null || decoder.Frames.Count == 0)
            {
                return;
            }

            // order all frames by size, take the frame with the highest color depth per size
            this.frames.AddRange(decoder
                                 .Frames
                                 .GroupBy(f => f.PixelWidth * f.PixelHeight)
                                 .OrderBy(g => g.Key)
                                 .Select(g => g.OrderByDescending(f => f.Format.BitsPerPixel).First())
            );
        }

        protected override void OnRender(DrawingContext dc)
        {
            if (this.frames.Count == 0)
            {
                base.OnRender(dc);
                return;
            }

            switch (this.MultiFrameImageMode)
            {
                case MultiFrameImageMode.ScaleDownLargerFrame:
                    var minSize = Math.Max(this.RenderSize.Width, this.RenderSize.Height);
                    var minFrame = this.frames.FirstOrDefault(f => f.Width >= minSize && f.Height >= minSize) ?? this.frames.Last();
                    dc.DrawImage(minFrame, new Rect(0, 0, this.RenderSize.Width, this.RenderSize.Height));
                    break;
                case MultiFrameImageMode.NoScaleSmallerFrame:
                    var maxSize = Math.Min(this.RenderSize.Width, this.RenderSize.Height);
                    var maxFrame = this.frames.LastOrDefault(f => f.Width <= maxSize && f.Height <= maxSize) ?? this.frames.First();
                    dc.DrawImage(maxFrame, new Rect((this.RenderSize.Width - maxFrame.Width) / 2, (this.RenderSize.Height - maxFrame.Height) / 2, maxFrame.Width, maxFrame.Height));
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    public enum MultiFrameImageMode
    {
        ScaleDownLargerFrame,
        NoScaleSmallerFrame,
    }
}