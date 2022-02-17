// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using ControlzEx;

namespace MahApps.Metro.Controls
{
    public class GridViewHeaderRowPresenterEx : GridViewHeaderRowPresenter
    {
        private PropertyChangeNotifier? visibilityPropertyChangeNotifier;

        protected override void OnVisualChildrenChanged(DependencyObject visualAdded, DependencyObject visualRemoved)
        {
            base.OnVisualChildrenChanged(visualAdded, visualRemoved);

            if (visualAdded is Separator addedIndicator)
            {
                this.visibilityPropertyChangeNotifier = new PropertyChangeNotifier(addedIndicator, Separator.IsVisibleProperty);
                this.visibilityPropertyChangeNotifier.ValueChanged += (o, e) =>
                    {
                        if (o is Separator { IsVisible: true } indicator)
                        {
                            var border = indicator.FindChild<Border>();
                            if (border is not null)
                            {
                                var itemsControl = FindItemsControlThroughTemplatedParent(this);
                                if (itemsControl is not null)
                                {
                                    var brush = ItemHelper.GetGridViewHeaderIndicatorBrush(itemsControl) ?? Brushes.Navy;
                                    border.SetValue(Border.BackgroundProperty, brush);
                                }
                            }
                        }
                    };
            }
        }

        private static ItemsControl? FindItemsControlThroughTemplatedParent(GridViewHeaderRowPresenter presenter)
        {
            FrameworkElement? fe = presenter.TemplatedParent as FrameworkElement;
            ItemsControl? itemsControl = null;

            while (fe != null)
            {
                itemsControl = fe as ItemsControl;
                if (itemsControl != null)
                {
                    break;
                }

                fe = fe.TemplatedParent as FrameworkElement;
            }

            return itemsControl;
        }
    }
}