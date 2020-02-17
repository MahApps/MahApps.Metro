using System.Windows;
using System.Windows.Controls;

namespace MahApps.Metro.Controls
{
    public class LayoutInvalidationCatcher : Decorator
    {
        private Planerator PlaParent => this.Parent as Planerator;

        protected override Size MeasureOverride(Size constraint)
        {
            this.PlaParent?.InvalidateMeasure();
            return base.MeasureOverride(constraint);
        }

        protected override Size ArrangeOverride(Size arrangeSize)
        {
            this.PlaParent?.InvalidateArrange();
            return base.ArrangeOverride(arrangeSize);
        }
    }
}