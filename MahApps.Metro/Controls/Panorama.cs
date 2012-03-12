namespace MahApps.Metro.Controls
{
    using System;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Animation;

    public class Panorama : ItemsControl
    {
        #region Constants and Fields

        public static readonly DependencyProperty HeaderOffsetProperty = DependencyProperty.Register(
            "HeaderOffset", typeof(double), typeof(Panorama), new PropertyMetadata(1.0));

        public static readonly DependencyProperty ItemHeightProperty = DependencyProperty.Register(
            "ItemHeight", typeof(double), typeof(Panorama), new PropertyMetadata(Double.NaN));

        public static readonly DependencyProperty ItemWidthProperty = DependencyProperty.Register(
            "ItemWidth", typeof(double), typeof(Panorama), new PropertyMetadata(Double.NaN));

        private double _currentWidth = -1;

        private double _increment;

        private bool _mouseCaptured;

        #endregion

        #region Constructors and Destructors

        public Panorama()
        {
            this.DefaultStyleKey = typeof(Panorama);
            this.Loaded += (s, e) =>
                {
                    foreach (var panoramaItem in this.Items.OfType<PanoramaItem>())
                    {
                        panoramaItem.RenderTransform = new TranslateTransform();
                    }
                    CompositionTarget.Rendering += this.CompositionTargetOnRendering;
                };
            this.Unloaded += (s, e) => CompositionTarget.Rendering -= this.CompositionTargetOnRendering;
        }

        #endregion

        #region Public Properties

        public double HeaderOffset
        {
            get
            {
                return (double)this.GetValue(HeaderOffsetProperty);
            }
            set
            {
                this.SetValue(HeaderOffsetProperty, value);
            }
        }

        public double ItemHeight
        {
            get
            {
                return (double)this.GetValue(ItemHeightProperty);
            }
            set
            {
                this.SetValue(ItemHeightProperty, value);
            }
        }

        public double ItemWidth
        {
            get
            {
                return (double)this.GetValue(ItemWidthProperty);
            }
            set
            {
                this.SetValue(ItemWidthProperty, value);
            }
        }

        public double Traslation
        {
            get
            {
                if (this.Items != null || this.Items.Count > 0)
                {
                    if ((this.Items[0] as PanoramaItem).RenderTransform == null)
                    {
                        foreach (var ob in this.Items)
                        {
                            (ob as PanoramaItem).RenderTransform = new TranslateTransform();
                        }
                    }
                    return ((TranslateTransform)((this.Items[0] as PanoramaItem).RenderTransform)).X;
                }
                return 0;
            }
            set
            {
                if (this.Items != null || this.Items.Count > 0)
                {
                    foreach (var ob in this.Items)
                    {
                        var currentpanorama = (ob as PanoramaItem);
                        ((TranslateTransform)(currentpanorama.RenderTransform)).X = value;
                    }
                }
            }
        }

        #endregion

        #region Methods

        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            var panoramaItem = element as PanoramaItem;
            if (panoramaItem == null)
            {
                return;
            }

            panoramaItem.Width = this.ItemWidth;
            panoramaItem.Height = this.ItemHeight;
        }

        private void Animate(double begin, double end)
        {
            var da = new DoubleAnimationUsingKeyFrames();
            da.Completed += (s, e) =>
                {
                    foreach (var ob in this.Items)
                    {
                        (ob as PanoramaItem).RenderTransform =
                            new TranslateTransform(
                                this.RenderTranslateTransform(ob).X, this.RenderTranslateTransform(ob).Y);
                    }
                };

            var e0 = new EasingDoubleKeyFrame(begin, KeyTime.FromTimeSpan(TimeSpan.Zero))
                { EasingFunction = new BackEase { EasingMode = EasingMode.EaseOut, Amplitude = 0.5 } };
            da.KeyFrames.Add(e0);

            var e1 = new EasingDoubleKeyFrame(end, KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0.6)))
                { EasingFunction = new BackEase { EasingMode = EasingMode.EaseOut, Amplitude = 0.5 } };
            da.KeyFrames.Add(e1);

            foreach (var ob in this.Items)
            {
                this.RenderTranslateTransform(ob).BeginAnimation(
                    TranslateTransform.XProperty, da, HandoffBehavior.SnapshotAndReplace);
            }
        }

        private void CompositionTargetOnRendering(object sender, EventArgs eventArgs)
        {
            var c = 0;
            if (this._mouseCaptured && Mouse.LeftButton == MouseButtonState.Released)
            {
                this._mouseCaptured = false;
                this._currentWidth = -1;
                var w = -1 * this.GetWidths();

                if (this.Traslation < (this.Items.Count - 1) * w)
                {
                    c = this.Items.Count - 1;
                }
                else if (this.Traslation < 0)
                {
                    if (Math.Abs(this.Traslation) + c * -this.ActualWidth > 20)
                    {
                        c += 1;
                    }
                    else if (Math.Abs(this.Traslation) + c * -this.ActualWidth < -20)
                    {
                        c -= 1;
                    }
                }
                else
                {
                    c = 0;
                }

                this.Animate(this.Traslation, Math.Min(0, c * -this.ActualWidth));
                return;
            }

            if (!this._mouseCaptured && Mouse.LeftButton == MouseButtonState.Pressed && this.MouseOver())
            {
                this._mouseCaptured = true;
                this._currentWidth = Mouse.GetPosition(this.Items[0] as PanoramaItem).X;
                return;
            }

            if (this._mouseCaptured && Mouse.LeftButton == MouseButtonState.Pressed && this.MouseOver())
            {
                this._increment = Mouse.GetPosition(this.Items[0] as PanoramaItem).X;
                this.Traslation += this._increment - this._currentWidth;
            }
        }

        private double GetWidths()
        {
            return this.Items.Cast<PanoramaItem>().Sum(i => i.ActualWidth);
        }

        private bool MouseOver()
        {
            var p = Mouse.GetPosition(this);
            return !(p.X < 0 || p.X > this.Width - 120 || p.Y < 0 || p.Y > this.Height);
        }

        private TranslateTransform RenderTranslateTransform(object ob)
        {
            return (ob as PanoramaItem).RenderTransform as TranslateTransform;
        }

        #endregion
    }
}