using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace MahApps.Metro.Controls
{
    /// <summary>
    /// A helper class that provides various attached properties for the ToolBar control.
    /// </summary>
    public static class ToolBarHelper
    {
        public static readonly DependencyPropertyKey IsInOverflowPanelKey =
            DependencyProperty.RegisterAttachedReadOnly("IsInOverflowPanel", typeof(bool), typeof(ToolBarHelper), new PropertyMetadata(false));
        public static readonly DependencyProperty IsInOverflowPanelProperty = IsInOverflowPanelKey.DependencyProperty;

        public static readonly DependencyProperty TrackParentPanelProperty =
            DependencyProperty.RegisterAttached("TrackParentPanel", typeof(bool), typeof(ToolBarHelper),
                                                new PropertyMetadata(false, OnTrackParentPanelPropertyChanged));

        private static void OnTrackParentPanelPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var element = d as UIElement;
            if (element == null)
                return;

            if ((bool)e.NewValue)
                element.LayoutUpdated += (s, arg) => OnControlLayoutUpdated(element);
        }

        /// <summary>
        /// Indicates whether or not the current element should track its position within the <see cref="ToolBarPanel"/>.
        /// </summary>
        [Category(AppName.MahApps)]
        public static bool GetTrackParentPanel(DependencyObject d)
        {
            return (bool)d.GetValue(TrackParentPanelProperty);
        }

        public static void SetTrackParentPanel(DependencyObject d, bool value)
        {
            d.SetValue(TrackParentPanelProperty, value);
        }

        /// <summary>
        /// Indicates whether or not the current <see cref="UIElement"/> is actually in the overflow panel part of the ToolBar.
        /// </summary>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(UIElement))]
        public static bool GetIsInOverflowPanel(UIElement target)
        {
            return (bool)target.GetValue(IsInOverflowPanelProperty);
        }

        private static void OnControlLayoutUpdated(UIElement element)
        {
            var isInOverflow = element.FindParent<ToolBarOverflowPanel>() != null; // FIX ME: MahApps VisualTree helper gives a different result from this helper
            element.SetValue(IsInOverflowPanelKey, isInOverflow);
        }
    }

    internal static class TreeHelper2
    {
        public static T FindParent<T>(this DependencyObject obj) where T : DependencyObject
        {
            return obj.GetAncestors().OfType<T>().FirstOrDefault();
        }

        public static IEnumerable<DependencyObject> GetAncestors(this DependencyObject element)
        {
            do
            {
                yield return element;
                element = VisualTreeHelper.GetParent(element);
            } while (element != null);
        }
    }
}
