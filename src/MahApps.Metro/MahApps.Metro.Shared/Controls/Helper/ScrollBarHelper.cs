using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace MahApps.Metro.Controls
{
    [Obsolete("This helper class will be deleted in the next release. Instead use the ScrollViewerHelper.")]
    public static class ScrollBarHelper
    {
        /// <summary>
        /// This property can be used to set vertical scrollbar left side from the tabpanel (look at MetroAnimatedSingleRowTabControl)
        /// </summary>
        [Obsolete("This attached property will be deleted in the next release. Instead use ScrollViewerHelper.VerticalScrollBarOnLeftSide attached property.")]
        public static readonly DependencyProperty VerticalScrollBarOnLeftSideProperty =
            DependencyProperty.RegisterAttached("VerticalScrollBarOnLeftSide",
                                                typeof(bool),
                                                typeof(ScrollBarHelper),
                                                new FrameworkPropertyMetadata(false,
                                                                              FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.Inherits,
                                                                              (o, e) =>
                                                                                  {
                                                                                      var element = o as ScrollViewer;
                                                                                      if (element != null && e.OldValue != e.NewValue && e.NewValue is bool)
                                                                                      {
                                                                                          ScrollViewerHelper.SetVerticalScrollBarOnLeftSide(element, (bool)e.NewValue);
                                                                                      }
                                                                                  }));

        [Obsolete("This attached property will be deleted in the next release. Instead use ScrollViewerHelper.VerticalScrollBarOnLeftSide attached property.")]
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(ScrollViewer))]
        public static bool GetVerticalScrollBarOnLeftSide(ScrollViewer obj)
        {
            return (bool)obj.GetValue(VerticalScrollBarOnLeftSideProperty);
        }

        [Obsolete("This attached property will be deleted in the next release. Instead use ScrollViewerHelper.VerticalScrollBarOnLeftSide attached property.")]
        public static void SetVerticalScrollBarOnLeftSide(ScrollViewer obj, bool value)
        {
            obj.SetValue(VerticalScrollBarOnLeftSideProperty, value);
        }
    }
}
