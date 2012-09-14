using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace MahApps.Metro.Controls
{
    public static class DependencyObjectExtensions
    {
        public static DependencyObject GetTopLevelControl(this DependencyObject control)
        {
            DependencyObject tmp = control;
            DependencyObject parent = null;
            while ((tmp = VisualTreeHelper.GetParent(tmp)) != null)
            {
                parent = tmp;
            }
            return parent;
        }
        public static DependencyObject GetParentControlOffsetFromTop(this DependencyObject control, int offset)
        {
            Queue<DependencyObject> controlQueue = new Queue<DependencyObject>(offset);
            DependencyObject tmp = control;
            controlQueue.Enqueue(tmp);
            while ((tmp = VisualTreeHelper.GetParent(tmp)) != null)
            {
                if (controlQueue.Count == (offset + 1)) controlQueue.Dequeue();
                controlQueue.Enqueue(tmp);
            }
            return controlQueue.First();
        }

        public static DependencyObject GetTopLevelControlOfType<T>(this DependencyObject control) where T : DependencyObject
        {
            T controlOfType = null;
            DependencyObject tmp = control;
            DependencyObject parent = null;
            if (tmp.GetType() == typeof(T)) controlOfType = tmp as T;
            while ((tmp = VisualTreeHelper.GetParent(tmp)) != null)
            {
                if (tmp.GetType() == typeof(T)) controlOfType = tmp as T;
            }
            return controlOfType;
        }
    }
}