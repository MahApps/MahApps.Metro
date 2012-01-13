using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace MahApps.Metro.Controls
{
    public class Panorama : ItemsControl
    {
        public static readonly DependencyProperty HeaderOffsetProperty = DependencyProperty.Register("HeaderOffset", typeof(double), typeof(Panorama), new PropertyMetadata(1.0));

        public static readonly DependencyProperty ItemWidthProperty = DependencyProperty.Register("ItemWidth", typeof(double), typeof(Panorama), new PropertyMetadata(Double.NaN));

        public static readonly DependencyProperty ItemHeightProperty = DependencyProperty.Register("ItemHeight", typeof(double), typeof(Panorama), new PropertyMetadata(Double.NaN));

        private double _currentWidth = -1;
        private double _increment;
        private bool _mouseCaptured;

        private double GetWidths()
        {
            return Items.Cast<PanoramaItem>().Sum(i => i.ActualWidth);
        }

        public Panorama()
        {
            DefaultStyleKey = typeof(Panorama);
            Loaded += (s, e) =>
            {
                foreach (var panoramaItem in Items.OfType<PanoramaItem>())
                {
                    panoramaItem.RenderTransform = new TranslateTransform();
                }
                CompositionTarget.Rendering += CompositionTargetOnRendering;
            };
            Unloaded += (s, e) => CompositionTarget.Rendering -= CompositionTargetOnRendering;
        }

        private void CompositionTargetOnRendering(object sender, EventArgs eventArgs)
        {
            var c = 0;
            if (_mouseCaptured && Mouse.LeftButton == MouseButtonState.Released)
            {
                _mouseCaptured = false;
                _currentWidth = -1;
                var w = -1 * GetWidths();

                if (Traslation < (Items.Count - 1) * w)
                {
                    c = Items.Count - 1;
                }
                else if (Traslation < 0)
                {
                    if (Math.Abs(Traslation) + c * -ActualWidth > 20)
                        c += 1;
                    else if (Math.Abs(Traslation) + c * -ActualWidth < -20)
                        c -= 1;
                }
                else
                {
                    c = 0;
                }

                Animate(Traslation, Math.Min(0, c * -ActualWidth));
                return;
            }

            if (!_mouseCaptured && Mouse.LeftButton == MouseButtonState.Pressed && MouseOver())
            {
                _mouseCaptured = true;
                _currentWidth = Mouse.GetPosition(Items[0] as PanoramaItem).X;
                return;
            }

            if (_mouseCaptured && Mouse.LeftButton == MouseButtonState.Pressed && MouseOver())
            {
                _increment = Mouse.GetPosition(Items[0] as PanoramaItem).X;
                Traslation += _increment - _currentWidth;
            }
        }

        public double HeaderOffset
        {
            get { return (double)GetValue(HeaderOffsetProperty); }
            set { SetValue(HeaderOffsetProperty, value); }
        }

        public double ItemWidth
        {
            get { return (double)GetValue(ItemWidthProperty); }
            set { SetValue(ItemWidthProperty, value); }
        }

        public double ItemHeight
        {
            get { return (double)GetValue(ItemHeightProperty); }
            set { SetValue(ItemHeightProperty, value); }
        }

        public double Traslation
        {
            get
            {
                if (Items != null || Items.Count > 0)
                {
                    if ((Items[0] as PanoramaItem).RenderTransform == null)
                    {
                        foreach (object ob in Items)
                        {
                            (ob as PanoramaItem).RenderTransform = new TranslateTransform();
                        }
                    }
                    return ((TranslateTransform)((Items[0] as PanoramaItem).RenderTransform)).X;
                }
                return 0;
            }
            set
            {
                if (Items != null || Items.Count > 0)
                {
                    foreach (object ob in Items)
                    {
                        var currentpanorama = (ob as PanoramaItem);
                        ((TranslateTransform)(currentpanorama.RenderTransform)).X = value;
                    }
                }
            }
        }

        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            var panoramaItem = element as PanoramaItem;
            if (panoramaItem == null)
                return;

            panoramaItem.Width = ItemWidth;
            panoramaItem.Height = ItemHeight;
        }

        private bool MouseOver()
        {
            Point p = Mouse.GetPosition(this);
            return !(p.X < 0 || p.X > Width - 120 || p.Y < 0 || p.Y > Height);
        }

        private TranslateTransform RenderTranslateTransform(object ob)
        {
            return (ob as PanoramaItem).RenderTransform as TranslateTransform;
        }

        private void Animate(double begin, double end)
        {
            var da = new DoubleAnimationUsingKeyFrames();
            da.Completed += (s, e) =>
            {
                foreach (object ob in Items)
                {
                    (ob as PanoramaItem).RenderTransform = new TranslateTransform(RenderTranslateTransform(ob).X, RenderTranslateTransform(ob).Y);
                }
            };

            var e0 = new EasingDoubleKeyFrame(begin, KeyTime.FromTimeSpan(TimeSpan.Zero))
            {
                EasingFunction = new BackEase { EasingMode = EasingMode.EaseOut, Amplitude = 0.5 }
            };
            da.KeyFrames.Add(e0);

            var e1 = new EasingDoubleKeyFrame(end, KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0.6)))
            {
                EasingFunction = new BackEase { EasingMode = EasingMode.EaseOut, Amplitude = 0.5 }
            };
            da.KeyFrames.Add(e1);

            foreach (object ob in Items)
            {
                RenderTranslateTransform(ob).BeginAnimation(TranslateTransform.XProperty, da, HandoffBehavior.SnapshotAndReplace);
            }
        }
    }
}
