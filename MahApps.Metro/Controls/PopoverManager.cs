using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace MahApps.Metro.Controls
{
    public static class PopoverManager
    {
        #region Popover adorner 

        // see tech.pro/tutorial/856/wpf-tutorial-using-a-visual-collection by Michael Kuehl

        class PopoverAdorner : Adorner
        {
            readonly MetroWindow _window;
            readonly VisualCollection _visuals;
            readonly MetroPopover _popover;

            public PopoverAdorner(MetroWindow window, UIElement adornedElement, MetroPopover popover)
                : base(adornedElement)
            {
                _window = window;
                _popover = popover;
                _visuals = new VisualCollection(this);
                _visuals.Add(popover);

                _popover.Closed += OnPopoverClosed;
                _window.PreviewMouseDown += OnPreviewMouseDownForWindow;
            }

            void OnPreviewMouseDownForWindow(object sender, MouseButtonEventArgs e)
            {
                var target = e.OriginalSource as DependencyObject;
                if (!IsDescendant(_popover, target)) {
                    _popover.RequestCloseAsync();
                }
            }

            private void OnPopoverClosed(object sender, EventArgs e)
            {
                _popover.Closed -= OnPopoverClosed;
                _window.PreviewMouseDown -= OnPreviewMouseDownForWindow;

                var adornerLayer = AdornerLayer.GetAdornerLayer(this.AdornedElement);
                if (adornerLayer != null) {
                    adornerLayer.Remove(this);
                }
            }

            public MetroWindow Window
            {
                get { return _window; }
            }

            public MetroPopover Popover
            {
                get { return _popover; }
            }

            protected override Size MeasureOverride(Size constraint)
            {
                _popover.Measure(constraint);
                return _popover.DesiredSize;
            }

            protected override Size ArrangeOverride(Size finalSize)
            {
                var offsetY = AdornedElement.RenderSize.Height;
                if (AdornedElement is Control) {
                    offsetY -= ((Control)AdornedElement).Margin.Bottom;
                }
                _popover.Arrange(new Rect(0, offsetY, finalSize.Width, finalSize.Height));
                return _popover.RenderSize;
            }

            protected override Visual GetVisualChild(int index)
            {
                return _visuals[index];
            }

            protected override int VisualChildrenCount
            {
                get { return _visuals.Count; }
            }


            static bool IsDescendant(DependencyObject reference, DependencyObject node)
            {
                bool result = false;
                DependencyObject dependencyObject = node;
                while (dependencyObject != null) {
                    if (dependencyObject == reference) {
                        result = true;
                        break;
                    }

                    dependencyObject = dependencyObject.GetParentObject();
                }
                return result;
            }
        }
        
        #endregion
        
        /// <summary>
        /// Displays a MetroPopover inside of the specified window, attached to the given control.
        /// <para>Note that this method returns as soon as the dialog is loaded and won't wait on a call of <see cref="HideMetroPopoverAsync"/>.</para>
        /// </summary>
        /// <param name="window">The owning window of the dialog.</param>
        /// <param name="target">The target element to attach the popover to.</param>
        /// <param name="popoverContent">The popover content to be shown.</param>
        /// <returns>A task representing the operation.</returns>
        public static Task<MetroPopover> ShowMetroPopoverAsync(this MetroWindow window, UIElement target, object popoverContent)
        {
            window.Dispatcher.VerifyAccess();
            var popover = new MetroPopover(target) {
                Content = popoverContent
            };
            SetupAndOpenPopup(window, target, popover);

            return popover.OpenAsync()
                .ContinueWith(x => {
                    if (PopoverOpened != null) {
                        PopoverOpened(window, new PopoverStateChangedEventArgs() { });
                    }
                    return popover;
                }, TaskScheduler.FromCurrentSynchronizationContext());
        }
     

        private static void SetupAndOpenPopup(MetroWindow window, UIElement target, MetroPopover popover)
        {
            var adornedLayer = AdornerLayer.GetAdornerLayer(target);
            if (adornedLayer == null) {
                throw new InvalidOperationException("Couldn't find adorner layer.");
            }

            var popooverAdorner = new PopoverAdorner(window, target, popover);
            adornedLayer.Add(popooverAdorner);
            popover.Owner = window;
        }


        public delegate void PopoverStateChangedHandler(object sender, PopoverStateChangedEventArgs args);

        public static event PopoverStateChangedHandler PopoverOpened;
        public static event PopoverStateChangedHandler PopoverClosed;

    }
}
