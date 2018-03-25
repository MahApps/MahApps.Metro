using System.Windows;
using System.Windows.Controls;

namespace MahApps.Metro.Controls
{
    public class LayoutInvalidationCatcher : Decorator
    {
        public Planerator PlaParent
        {
            get { return Parent as Planerator; }
        }

        protected override Size MeasureOverride(Size constraint)
        {
            Planerator pl = PlaParent;
            if (pl != null)
            {
                pl.InvalidateMeasure();
            }
            return base.MeasureOverride(constraint);
        }

        protected override Size ArrangeOverride(Size arrangeSize)
        {
            Planerator pl = PlaParent;
            if (pl != null)
            {
                pl.InvalidateArrange();
            }
            return base.ArrangeOverride(arrangeSize);
        }
    }
}