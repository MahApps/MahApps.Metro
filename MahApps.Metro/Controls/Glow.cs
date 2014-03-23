using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace MahApps.Metro.Controls
{
    public class Glow : Control
    {
        public static readonly DependencyProperty GlowBrushProperty = DependencyProperty.Register("GlowBrush", typeof(SolidColorBrush), typeof(Glow), new UIPropertyMetadata(Brushes.Transparent));
        public static readonly DependencyProperty NonActiveGlowBrushProperty = DependencyProperty.Register("NonActiveGlowBrush", typeof(SolidColorBrush), typeof(Glow), new UIPropertyMetadata(Brushes.Transparent));
        public static readonly DependencyProperty IsGlowProperty = DependencyProperty.Register("IsGlow", typeof(bool), typeof(Glow), new UIPropertyMetadata(true));
        public static readonly DependencyProperty OrientationProperty = DependencyProperty.Register("Orientation", typeof(Orientation), typeof(Glow), new UIPropertyMetadata(Orientation.Vertical));

        static Glow()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Glow), new FrameworkPropertyMetadata(typeof(Glow)));
        }

        public SolidColorBrush GlowBrush
        {
            get { return (SolidColorBrush)this.GetValue(GlowBrushProperty); }
            set { this.SetValue(GlowBrushProperty, value); }
        }

        public SolidColorBrush NonActiveGlowBrush
        {
            get { return (SolidColorBrush)this.GetValue(NonActiveGlowBrushProperty); }
            set { this.SetValue(NonActiveGlowBrushProperty, value); }
        }

        public bool IsGlow
        {
            get { return (bool)this.GetValue(IsGlowProperty); }
            set { this.SetValue(IsGlowProperty, value); }
        }

        public Orientation Orientation
        {
            get { return (Orientation)this.GetValue(OrientationProperty); }
            set { this.SetValue(OrientationProperty, value); }
        }
    }
}
