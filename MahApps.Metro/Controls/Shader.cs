using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace MahApps.Metro.Controls
{
    /// <summary>
    /// Adorner to create background shading over an element.
    /// </summary>
    public class Shader : Adorner
    {
        /// <summary>
        /// Creates a Shader with 50% black shading.
        /// </summary>
        /// <param name="adornedElement">The element over which to apply the shading.</param>
        public Shader(UIElement adornedElement) : base(adornedElement)
        {
            Background = new SolidColorBrush(Colors.Black) {Opacity = 0.5d};
            StrokeBorder = null;
        }

        /// <summary>
        /// Creates a Shader with the specified background and border.
        /// </summary>
        /// <param name="adornedElement">The element over which to apply the shading.</param>
        /// <param name="background">The brush to use to fill the shading.  Note less than 100% opacity should have been set before the brush is passed through.</param>
        /// <param name="strokeBorder">The border to apply.</param>
        public Shader(UIElement adornedElement, SolidColorBrush background, Pen strokeBorder) : this(adornedElement)
        {
            //caller needs to have set opacity on background brush
            Background = background;
            StrokeBorder = strokeBorder;
        }

        SolidColorBrush Background
        {
            get;
            set;
        }

        Pen StrokeBorder
        {
            get;
            set;
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            FrameworkElement elem = (FrameworkElement)AdornedElement;
            Rect adornedElementRect = new Rect(0, 0, elem.ActualWidth, elem.ActualHeight);
            drawingContext.DrawRectangle(Background, StrokeBorder, adornedElementRect);
        }
    }
}