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
        static MultiFrameImage()
        {
            SourceProperty.OverrideMetadata(typeof(MultiFrameImage), new FrameworkPropertyMetadata(OnSourceChanged));
        }

        private static void OnSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var multiFrameImage = (MultiFrameImage) d;
            multiFrameImage.UpdateFrameList();
        }

        private readonly List<BitmapSource> _frames = new List<BitmapSource>();

        private void UpdateFrameList()
        {
            _frames.Clear();

            var bitmapFrame = Source as BitmapFrame;
            if (bitmapFrame == null)
            {
                return;
            }

            var decoder = bitmapFrame.Decoder;
            if (decoder == null || decoder.Frames.Count == 0)
            {
                return;
            }

            // order all frames by size, take the frame with the highest color depth per size
            _frames.AddRange(
                decoder
                    .Frames
                    .GroupBy(f => f.PixelWidth * f.PixelHeight)
                    .OrderBy(g => g.Key)
                    .Select(g => g.OrderByDescending(f => f.Format.BitsPerPixel).First())
                    );
        }

        protected override void OnRender(DrawingContext dc)
        {
            if (_frames.Count == 0)
            {
                base.OnRender(dc);
                return;
            }

            var minSize = Math.Max(RenderSize.Width, RenderSize.Height);
            var frame = _frames.FirstOrDefault(f => f.Width >= minSize && f.Height >= minSize) ?? _frames.Last();
            dc.DrawImage(frame, new Rect(0, 0, RenderSize.Width, RenderSize.Height));
        }
    }
}