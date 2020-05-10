using ControlzEx.Native;
using ControlzEx.Standard;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Interop;

namespace MahApps.Metro.Controls
{
    /// <summary>
    /// This custom popup is used by the validation error template.
    /// It provides some additional nice features:
    ///     - repositioning if host-window size or location changed
    ///     - repositioning if host-window gets maximized and vice versa
    ///     - it's only topmost if the host-window is activated
    /// </summary>
    public class CustomValidationPopup : Popup
    {
        private Window hostWindow;
        private ScrollViewer scrollViewer;

        /// <summary>Identifies the <see cref="CloseOnMouseLeftButtonDown"/> dependency property.</summary>
        public static readonly DependencyProperty CloseOnMouseLeftButtonDownProperty
            = DependencyProperty.Register(nameof(CloseOnMouseLeftButtonDown),
                                          typeof(bool),
                                          typeof(CustomValidationPopup),
                                          new PropertyMetadata(true));

        /// <summary>
        /// Gets or sets whether if the popup can be closed by left mouse button down.
        /// </summary>
        public bool CloseOnMouseLeftButtonDown
        {
            get => (bool)this.GetValue(CloseOnMouseLeftButtonDownProperty);
            set => this.SetValue(CloseOnMouseLeftButtonDownProperty, value);
        }

        /// <summary>Identifies the <see cref="ShowValidationErrorOnMouseOver"/> dependency property.</summary>
        public static readonly DependencyProperty ShowValidationErrorOnMouseOverProperty
            = DependencyProperty.RegisterAttached(nameof(ShowValidationErrorOnMouseOver),
                                                  typeof(bool),
                                                  typeof(CustomValidationPopup),
                                                  new PropertyMetadata(false));

        /// <summary>
        /// Gets or sets whether the validation error text will be shown when hovering the validation triangle.
        /// </summary>
        public bool ShowValidationErrorOnMouseOver
        {
            get => (bool)this.GetValue(ShowValidationErrorOnMouseOverProperty);
            set => this.SetValue(ShowValidationErrorOnMouseOverProperty, value);
        }

        /// <summary>Identifies the <see cref="AdornedElement"/> dependency property.</summary>
        public static readonly DependencyProperty AdornedElementProperty
            = DependencyProperty.Register(nameof(AdornedElement),
                                          typeof(UIElement),
                                          typeof(CustomValidationPopup),
                                          new PropertyMetadata(default(UIElement)));

        /// <summary>
        /// Gets or sets the <see cref="T:System.Windows.UIElement" /> that this <see cref="T:System.Windows.Controls.Primitives.Popup" /> object is reserving space for.
        /// </summary>
        public UIElement AdornedElement
        {
            get => (UIElement)this.GetValue(AdornedElementProperty);
            set => this.SetValue(AdornedElementProperty, value);
        }

        public CustomValidationPopup()
        {
            this.Loaded += this.CustomValidationPopup_Loaded;
            this.Opened += this.CustomValidationPopup_Opened;
        }

        protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            if (this.CloseOnMouseLeftButtonDown)
            {
                this.SetCurrentValue(IsOpenProperty, false);
            }
            else
            {
                var adornedElement = this.AdornedElement;
                if (adornedElement != null && ValidationHelper.GetCloseOnMouseLeftButtonDown(adornedElement))
                {
                    this.SetCurrentValue(IsOpenProperty, false);
                }
                else
                {
                    e.Handled = true;
                }
            }
        }

        private void CustomValidationPopup_Loaded(object sender, RoutedEventArgs e)
        {
            var adornedElement = this.AdornedElement;
            if (adornedElement is null)
            {
                return;
            }

            this.hostWindow = Window.GetWindow(adornedElement);
            if (this.hostWindow == null)
            {
                return;
            }

            if (this.scrollViewer != null)
            {
                this.scrollViewer.ScrollChanged -= this.ScrollViewer_ScrollChanged;
            }

            this.scrollViewer = adornedElement.TryFindParent<ScrollViewer>();
            if (this.scrollViewer != null)
            {
                this.scrollViewer.ScrollChanged += this.ScrollViewer_ScrollChanged;
            }

            this.hostWindow.LocationChanged -= this.hostWindow_SizeOrLocationChanged;
            this.hostWindow.LocationChanged += this.hostWindow_SizeOrLocationChanged;
            this.hostWindow.SizeChanged -= this.hostWindow_SizeOrLocationChanged;
            this.hostWindow.SizeChanged += this.hostWindow_SizeOrLocationChanged;
            this.hostWindow.StateChanged -= this.hostWindow_StateChanged;
            this.hostWindow.StateChanged += this.hostWindow_StateChanged;
            this.hostWindow.Activated -= this.hostWindow_Activated;
            this.hostWindow.Activated += this.hostWindow_Activated;
            this.hostWindow.Deactivated -= this.hostWindow_Deactivated;
            this.hostWindow.Deactivated += this.hostWindow_Deactivated;

            if (this.PlacementTarget is FrameworkElement frameworkElement)
            {
                frameworkElement.SizeChanged -= this.hostWindow_SizeOrLocationChanged;
                frameworkElement.SizeChanged += this.hostWindow_SizeOrLocationChanged;
            }

            this.Unloaded -= this.CustomValidationPopup_Unloaded;
            this.Unloaded += this.CustomValidationPopup_Unloaded;
        }

        private void ScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            this.Move();

            if (this.IsElementVisible(this.AdornedElement as FrameworkElement, this.scrollViewer))
            {
                var adornedElement = this.AdornedElement;
                var isOpen = Validation.GetHasError(adornedElement) && adornedElement.IsKeyboardFocusWithin;
                this.SetCurrentValue(IsOpenProperty, isOpen);
            }
            else
            {
                this.SetCurrentValue(IsOpenProperty, false);
            }
        }

        private bool IsElementVisible(FrameworkElement element, FrameworkElement container)
        {
            if (element is null || container is null || !element.IsVisible)
            {
                return false;
            }

            var bounds = element.TransformToAncestor(container)
                                .TransformBounds(new Rect(0.0, 0.0, element.ActualWidth, element.ActualHeight));
            var rect = new Rect(0.0, 0.0, container.ActualWidth, container.ActualHeight);
            return rect.IntersectsWith(bounds);
        }

        private void CustomValidationPopup_Opened(object sender, EventArgs e)
        {
            this.SetTopmostState(true);
        }

        private void hostWindow_Activated(object sender, EventArgs e)
        {
            this.SetTopmostState(true);
        }

        private void hostWindow_Deactivated(object sender, EventArgs e)
        {
            this.SetTopmostState(false);
        }

        private void CustomValidationPopup_Unloaded(object sender, RoutedEventArgs e)
        {
            if (this.PlacementTarget is FrameworkElement frameworkElement)
            {
                frameworkElement.SizeChanged -= this.hostWindow_SizeOrLocationChanged;
            }

            if (this.hostWindow != null)
            {
                this.hostWindow.LocationChanged -= this.hostWindow_SizeOrLocationChanged;
                this.hostWindow.SizeChanged -= this.hostWindow_SizeOrLocationChanged;
                this.hostWindow.StateChanged -= this.hostWindow_StateChanged;
                this.hostWindow.Activated -= this.hostWindow_Activated;
                this.hostWindow.Deactivated -= this.hostWindow_Deactivated;
            }

            if (this.scrollViewer != null)
            {
                this.scrollViewer.ScrollChanged -= this.ScrollViewer_ScrollChanged;
            }

            this.Unloaded -= this.CustomValidationPopup_Unloaded;
            this.Opened -= this.CustomValidationPopup_Opened;
            this.hostWindow = null;
        }

        private void hostWindow_StateChanged(object sender, EventArgs e)
        {
            if (this.hostWindow != null && this.hostWindow.WindowState != WindowState.Minimized)
            {
                var adornedElement = this.AdornedElement;
                if (adornedElement != null)
                {
                    this.PopupAnimation = PopupAnimation.None;
                    this.SetCurrentValue(IsOpenProperty, false);
                    var errorTemplate = adornedElement.GetValue(Validation.ErrorTemplateProperty);
                    adornedElement.SetValue(Validation.ErrorTemplateProperty, null);
                    adornedElement.SetValue(Validation.ErrorTemplateProperty, errorTemplate);
                }
            }
        }

        private void hostWindow_SizeOrLocationChanged(object sender, EventArgs e)
        {
            this.Move();
        }

        private void Move()
        {
            var offset = this.HorizontalOffset;
            // "bump" the offset to cause the popup to reposition itself on its own
            this.HorizontalOffset = offset + 1;
            this.HorizontalOffset = offset;
        }

        private bool? appliedTopMost;

        private void SetTopmostState(bool isTop)
        {
            // Don’t apply state if it’s the same as incoming state
            if (this.appliedTopMost.HasValue && this.appliedTopMost == isTop)
            {
                return;
            }

            if (this.Child == null)
            {
                return;
            }

            var hwndSource = (PresentationSource.FromVisual(this.Child)) as HwndSource;
            if (hwndSource == null)
            {
                return;
            }

            var hwnd = hwndSource.Handle;

#pragma warning disable 618
            RECT rect;
            if (!UnsafeNativeMethods.GetWindowRect(hwnd, out rect))
            {
                return;
            }
            //Debug.WriteLine("setting z-order " + isTop);

            var left = rect.Left;
            var top = rect.Top;
            var width = rect.Width;
            var height = rect.Height;
            if (isTop)
            {
                NativeMethods.SetWindowPos(hwnd, Constants.HWND_TOPMOST, left, top, width, height, SWP.TOPMOST);
            }
            else
            {
                // Z-Order would only get refreshed/reflected if clicking the
                // the titlebar (as opposed to other parts of the external
                // window) unless I first set the popup to HWND_BOTTOM
                // then HWND_TOP before HWND_NOTOPMOST
                NativeMethods.SetWindowPos(hwnd, Constants.HWND_BOTTOM, left, top, width, height, SWP.TOPMOST);
                NativeMethods.SetWindowPos(hwnd, Constants.HWND_TOP, left, top, width, height, SWP.TOPMOST);
                NativeMethods.SetWindowPos(hwnd, Constants.HWND_NOTOPMOST, left, top, width, height, SWP.TOPMOST);
            }

            this.appliedTopMost = isTop;
#pragma warning restore 618
        }
    }
}