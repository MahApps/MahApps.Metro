// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace MetroDemo.ExampleViews
{
    /// <summary>
    /// Interaction logic for Badges.xaml
    /// </summary>
    public partial class Badges : UserControl
    {
        public Badges()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Paints the tab the mouse is holding down. A TabItem has no IsPressed to hang a trigger on,
        /// so the press is set here and cleared again on the way up, which leaves the hover trigger
        /// to take over. The walk up is the visual tree alone: the content of a selected tab hangs
        /// off the tab logically, so a click down in the content would otherwise light the header.
        /// </summary>
        private void OnTabPressed(object sender, MouseButtonEventArgs e)
        {
            if (Highlight(e.OriginalSource as DependencyObject) is { } header)
            {
                header.SetCurrentValue(BackgroundProperty, this.TryFindResource("MahApps.Brushes.Accent3") as Brush);
            }
        }

        private void OnTabLetGo(object sender, MouseEventArgs e)
        {
            if (sender is not TabControl tabs)
            {
                return;
            }

            foreach (TabItem tab in tabs.Items)
            {
                if (Highlight(tab) is { } header)
                {
                    header.ClearValue(BackgroundProperty);
                }
            }
        }

        /// <summary>
        /// The border a mailbox tab paints its header on, found from anything inside it or from the
        /// tab itself. Only the visual tree is walked: the content of a selected tab hangs off the
        /// tab logically, so a click down in the content would otherwise light up the header.
        /// </summary>
        private static Border? Highlight(DependencyObject? from)
        {
            for (var node = from; node is not null; node = VisualTreeHelper.GetParent(node))
            {
                if (node is Border named && named.Name == "HeaderHighlight")
                {
                    return named;
                }

                if (node is TabItem tab)
                {
                    return Inside(tab);
                }
            }

            return null;
        }

        private static Border? Inside(DependencyObject node)
        {
            if (node is Border named && named.Name == "HeaderHighlight")
            {
                return named;
            }

            for (var index = 0; index < VisualTreeHelper.GetChildrenCount(node); index++)
            {
                if (Inside(VisualTreeHelper.GetChild(node, index)) is { } found)
                {
                    return found;
                }
            }

            return null;
        }
    }
}
