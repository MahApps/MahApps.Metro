using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Interop;
using MahApps.Metro.Native;

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
        public static readonly DependencyProperty CloseOnMouseLeftButtonDownProperty = DependencyProperty.Register("CloseOnMouseLeftButtonDown", typeof(bool), typeof(CustomValidationPopup), new PropertyMetadata(true));

        private Window hostWindow;

        public CustomValidationPopup()
        {
            this.Loaded += this.CustomValidationPopup_Loaded;
            this.Opened += this.CustomValidationPopup_Opened;
        }

        /// <summary>
        /// Gets/sets if the popup can be closed by left mouse button down.
        /// </summary>
        public bool CloseOnMouseLeftButtonDown
        {
            get { return (bool)GetValue(CloseOnMouseLeftButtonDownProperty); }
            set { SetValue(CloseOnMouseLeftButtonDownProperty, value); }
        }

        protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            if (CloseOnMouseLeftButtonDown)
            {
                this.IsOpen = false;
            }
        }

        private void CustomValidationPopup_Loaded(object sender, RoutedEventArgs e)
        {
            var target = this.PlacementTarget as FrameworkElement;
            if (target == null)
            {
                return;
            }

            this.hostWindow = Window.GetWindow(target);
            if (this.hostWindow == null)
            {
                return;
            }

            this.hostWindow.LocationChanged -= this.hostWindow_SizeOrLocationChanged;
            this.hostWindow.LocationChanged += this.hostWindow_SizeOrLocationChanged;
            this.hostWindow.SizeChanged -= this.hostWindow_SizeOrLocationChanged;
            this.hostWindow.SizeChanged += this.hostWindow_SizeOrLocationChanged;
            target.SizeChanged -= this.hostWindow_SizeOrLocationChanged;
            target.SizeChanged += this.hostWindow_SizeOrLocationChanged;
            this.hostWindow.StateChanged -= this.hostWindow_StateChanged;
            this.hostWindow.StateChanged += this.hostWindow_StateChanged;
            this.hostWindow.Activated -= this.hostWindow_Activated;
            this.hostWindow.Activated += this.hostWindow_Activated;
            this.hostWindow.Deactivated -= this.hostWindow_Deactivated;
            this.hostWindow.Deactivated += this.hostWindow_Deactivated;

            this.Unloaded -= this.CustomValidationPopup_Unloaded;
            this.Unloaded += this.CustomValidationPopup_Unloaded;
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
            var target = this.PlacementTarget as FrameworkElement;
            if (target != null)
            {
                target.SizeChanged -= this.hostWindow_SizeOrLocationChanged;
            }
            if (this.hostWindow != null)
            {
                this.hostWindow.LocationChanged -= this.hostWindow_SizeOrLocationChanged;
                this.hostWindow.SizeChanged -= this.hostWindow_SizeOrLocationChanged;
                this.hostWindow.StateChanged -= this.hostWindow_StateChanged;
                this.hostWindow.Activated -= this.hostWindow_Activated;
                this.hostWindow.Deactivated -= this.hostWindow_Deactivated;
            }
            this.Unloaded -= this.CustomValidationPopup_Unloaded;
            this.Opened -= this.CustomValidationPopup_Opened;
            this.hostWindow = null;
        }

        private void hostWindow_StateChanged(object sender, EventArgs e)
        {
            if (this.hostWindow != null && this.hostWindow.WindowState != WindowState.Minimized)
            {
                var target = this.PlacementTarget as FrameworkElement;
                var holder = target != null ? target.DataContext as AdornedElementPlaceholder : null;
                if (holder != null && holder.AdornedElement != null)
                {
                    this.PopupAnimation = PopupAnimation.None;
                    this.IsOpen = false;
                    var errorTemplate = holder.AdornedElement.GetValue(Validation.ErrorTemplateProperty);
                    holder.AdornedElement.SetValue(Validation.ErrorTemplateProperty, null);
                    holder.AdornedElement.SetValue(Validation.ErrorTemplateProperty, errorTemplate);
                }
            }
        }

        private void hostWindow_SizeOrLocationChanged(object sender, EventArgs e)
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

            RECT rect;
            if (!UnsafeNativeMethods.GetWindowRect(hwnd, out rect))
            {
                return;
            }
            //Debug.WriteLine("setting z-order " + isTop);

            var left = rect.left;
            var top = rect.top;
            var width = rect.Width;
            var height = rect.Height;
            if (isTop)
            {
                UnsafeNativeMethods.SetWindowPos(hwnd, Constants.HWND_TOPMOST, left, top, width, height, Constants.TOPMOST_FLAGS);
            }
            else
            {
                // Z-Order would only get refreshed/reflected if clicking the
                // the titlebar (as opposed to other parts of the external
                // window) unless I first set the popup to HWND_BOTTOM
                // then HWND_TOP before HWND_NOTOPMOST
                UnsafeNativeMethods.SetWindowPos(hwnd, Constants.HWND_BOTTOM, left, top, width, height, Constants.TOPMOST_FLAGS);
                UnsafeNativeMethods.SetWindowPos(hwnd, Constants.HWND_TOP, left, top, width, height, Constants.TOPMOST_FLAGS);
                UnsafeNativeMethods.SetWindowPos(hwnd, Constants.HWND_NOTOPMOST, left, top, width, height, Constants.TOPMOST_FLAGS);
            }

            this.appliedTopMost = isTop;
        }
    }
}