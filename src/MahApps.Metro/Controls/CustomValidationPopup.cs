// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using ControlzEx.Native;
using ControlzEx.Standard;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Interop;
using MahApps.Metro.ValueBoxes;

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
        private MetroContentControl metroContentControl;
        private TransitioningContentControl transitioningContentControl;
        private Flyout flyout;

        /// <summary>Identifies the <see cref="CloseOnMouseLeftButtonDown"/> dependency property.</summary>
        public static readonly DependencyProperty CloseOnMouseLeftButtonDownProperty
            = DependencyProperty.Register(nameof(CloseOnMouseLeftButtonDown),
                                          typeof(bool),
                                          typeof(CustomValidationPopup),
                                          new PropertyMetadata(BooleanBoxes.TrueBox));

        /// <summary>
        /// Gets or sets whether if the popup can be closed by left mouse button down.
        /// </summary>
        public bool CloseOnMouseLeftButtonDown
        {
            get => (bool)this.GetValue(CloseOnMouseLeftButtonDownProperty);
            set => this.SetValue(CloseOnMouseLeftButtonDownProperty, BooleanBoxes.Box(value));
        }

        /// <summary>Identifies the <see cref="ShowValidationErrorOnMouseOver"/> dependency property.</summary>
        public static readonly DependencyProperty ShowValidationErrorOnMouseOverProperty
            = DependencyProperty.RegisterAttached(nameof(ShowValidationErrorOnMouseOver),
                                                  typeof(bool),
                                                  typeof(CustomValidationPopup),
                                                  new PropertyMetadata(BooleanBoxes.FalseBox));

        /// <summary>
        /// Gets or sets whether the validation error text will be shown when hovering the validation triangle.
        /// </summary>
        public bool ShowValidationErrorOnMouseOver
        {
            get => (bool)this.GetValue(ShowValidationErrorOnMouseOverProperty);
            set => this.SetValue(ShowValidationErrorOnMouseOverProperty, BooleanBoxes.Box(value));
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

        /// <summary>Identifies the <see cref="CanShow"/> dependency property.</summary>
        private static readonly DependencyPropertyKey CanShowPropertyKey
            = DependencyProperty.RegisterReadOnly(nameof(CanShow),
                                                  typeof(bool),
                                                  typeof(CustomValidationPopup),
                                                  new PropertyMetadata(BooleanBoxes.FalseBox));

        /// <summary>Identifies the <see cref="CanShow"/> dependency property.</summary>
        public static readonly DependencyProperty CanShowProperty = CanShowPropertyKey.DependencyProperty;

        /// <summary>
        /// Gets whether the popup can be shown (useful for transitions).
        /// </summary>
        public bool CanShow
        {
            get => (bool)this.GetValue(CanShowProperty);
            protected set => this.SetValue(CanShowPropertyKey, BooleanBoxes.Box(value));
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
                this.SetCurrentValue(IsOpenProperty, BooleanBoxes.FalseBox);
            }
            else
            {
                var adornedElement = this.AdornedElement;
                if (adornedElement != null && ValidationHelper.GetCloseOnMouseLeftButtonDown(adornedElement))
                {
                    this.SetCurrentValue(IsOpenProperty, BooleanBoxes.FalseBox);
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
            if (this.hostWindow is null)
            {
                return;
            }

            this.SetValue(CanShowPropertyKey, BooleanBoxes.FalseBox);
            var canShow = true;

            if (this.scrollViewer != null)
            {
                this.scrollViewer.ScrollChanged -= this.ScrollViewer_ScrollChanged;
            }

            this.scrollViewer = adornedElement.GetVisualAncestor<ScrollViewer>();
            if (this.scrollViewer != null)
            {
                this.scrollViewer.ScrollChanged += this.ScrollViewer_ScrollChanged;
            }

            if (this.metroContentControl != null)
            {
                this.metroContentControl.TransitionStarted -= this.OnTransitionStarted;
                this.metroContentControl.TransitionCompleted -= this.OnTransitionCompleted;
            }

            this.metroContentControl = adornedElement.TryFindParent<MetroContentControl>();
            if (this.metroContentControl != null)
            {
                canShow = !this.metroContentControl.TransitionsEnabled || !this.metroContentControl.IsTransitioning;
                this.metroContentControl.TransitionStarted += this.OnTransitionStarted;
                this.metroContentControl.TransitionCompleted += this.OnTransitionCompleted;
            }

            if (this.transitioningContentControl != null)
            {
                this.transitioningContentControl.TransitionCompleted -= this.OnTransitionCompleted;
            }

            this.transitioningContentControl = adornedElement.TryFindParent<TransitioningContentControl>();
            if (this.transitioningContentControl != null)
            {
                canShow = canShow && (this.transitioningContentControl.CurrentTransition == null || !this.transitioningContentControl.IsTransitioning);
                this.transitioningContentControl.TransitionCompleted += this.OnTransitionCompleted;
            }

            if (this.flyout != null)
            {
                this.flyout.OpeningFinished -= this.Flyout_OpeningFinished;
                this.flyout.IsOpenChanged -= this.Flyout_IsOpenChanged;
                this.flyout.ClosingFinished -= this.Flyout_ClosingFinished;
            }

            this.flyout = adornedElement.TryFindParent<Flyout>();
            if (this.flyout != null)
            {
                canShow = canShow && (!this.flyout.AreAnimationsEnabled || this.flyout.IsShown);
                this.flyout.OpeningFinished += this.Flyout_OpeningFinished;
                this.flyout.IsOpenChanged += this.Flyout_IsOpenChanged;
                this.flyout.ClosingFinished += this.Flyout_ClosingFinished;
            }

            this.hostWindow.LocationChanged -= this.OnSizeOrLocationChanged;
            this.hostWindow.LocationChanged += this.OnSizeOrLocationChanged;
            this.hostWindow.SizeChanged -= this.OnSizeOrLocationChanged;
            this.hostWindow.SizeChanged += this.OnSizeOrLocationChanged;
            this.hostWindow.StateChanged -= this.OnHostWindowStateChanged;
            this.hostWindow.StateChanged += this.OnHostWindowStateChanged;
            this.hostWindow.Activated -= this.OnHostWindowActivated;
            this.hostWindow.Activated += this.OnHostWindowActivated;
            this.hostWindow.Deactivated -= this.OnHostWindowDeactivated;
            this.hostWindow.Deactivated += this.OnHostWindowDeactivated;

            if (this.PlacementTarget is FrameworkElement frameworkElement)
            {
                frameworkElement.SizeChanged -= this.OnSizeOrLocationChanged;
                frameworkElement.SizeChanged += this.OnSizeOrLocationChanged;
            }

            this.RefreshPosition();
            this.SetValue(CanShowPropertyKey, BooleanBoxes.Box(canShow));

            this.OnLoaded();

            this.Unloaded -= this.CustomValidationPopup_Unloaded;
            this.Unloaded += this.CustomValidationPopup_Unloaded;
        }

        private void Flyout_OpeningFinished(object sender, RoutedEventArgs e)
        {
            this.RefreshPosition();

            var adornedElement = this.AdornedElement;
            var isOpen = Validation.GetHasError(adornedElement) && adornedElement.IsKeyboardFocusWithin;
            this.SetCurrentValue(IsOpenProperty, BooleanBoxes.Box(isOpen));

            this.SetValue(CanShowPropertyKey, BooleanBoxes.TrueBox);
        }

        private void Flyout_IsOpenChanged(object sender, RoutedEventArgs e)
        {
            this.RefreshPosition();
            this.SetValue(CanShowPropertyKey, BooleanBoxes.FalseBox);
        }

        private void Flyout_ClosingFinished(object sender, RoutedEventArgs e)
        {
            this.RefreshPosition();
            this.SetValue(CanShowPropertyKey, BooleanBoxes.FalseBox);
        }

        private void OnTransitionStarted(object sender, RoutedEventArgs e)
        {
            this.RefreshPosition();
            this.SetValue(CanShowPropertyKey, BooleanBoxes.FalseBox);
        }

        private void OnTransitionCompleted(object sender, RoutedEventArgs e)
        {
            this.RefreshPosition();

            var adornedElement = this.AdornedElement;
            var isOpen = Validation.GetHasError(adornedElement) && adornedElement.IsKeyboardFocusWithin;
            this.SetCurrentValue(IsOpenProperty, BooleanBoxes.Box(isOpen));

            this.SetValue(CanShowPropertyKey, BooleanBoxes.TrueBox);
        }

        private void ScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (e.VerticalChange > 0 || e.VerticalChange < 0 || e.HorizontalChange > 0 || e.HorizontalChange < 0)
            {
                this.RefreshPosition();

                if (IsElementVisible(this.AdornedElement as FrameworkElement, this.scrollViewer))
                {
                    var adornedElement = this.AdornedElement;
                    var isOpen = Validation.GetHasError(adornedElement) && adornedElement.IsKeyboardFocusWithin;
                    this.SetCurrentValue(IsOpenProperty, BooleanBoxes.Box(isOpen));
                }
                else
                {
                    this.SetCurrentValue(IsOpenProperty, BooleanBoxes.FalseBox);
                }
            }
        }

        private static bool IsElementVisible(FrameworkElement element, FrameworkElement container)
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

        private void OnHostWindowActivated(object sender, EventArgs e)
        {
            this.SetTopmostState(true);
        }

        private void OnHostWindowDeactivated(object sender, EventArgs e)
        {
            this.SetTopmostState(false);
        }

        private void CustomValidationPopup_Unloaded(object sender, RoutedEventArgs e)
        {
            this.OnUnLoaded();

            if (this.PlacementTarget is FrameworkElement frameworkElement)
            {
                frameworkElement.SizeChanged -= this.OnSizeOrLocationChanged;
            }

            if (this.hostWindow != null)
            {
                this.hostWindow.LocationChanged -= this.OnSizeOrLocationChanged;
                this.hostWindow.SizeChanged -= this.OnSizeOrLocationChanged;
                this.hostWindow.StateChanged -= this.OnHostWindowStateChanged;
                this.hostWindow.Activated -= this.OnHostWindowActivated;
                this.hostWindow.Deactivated -= this.OnHostWindowDeactivated;
            }

            if (this.scrollViewer != null)
            {
                this.scrollViewer.ScrollChanged -= this.ScrollViewer_ScrollChanged;
            }

            if (this.metroContentControl != null)
            {
                this.metroContentControl.TransitionStarted -= this.OnTransitionStarted;
                this.metroContentControl.TransitionCompleted -= this.OnTransitionCompleted;
            }

            if (this.transitioningContentControl != null)
            {
                this.transitioningContentControl.TransitionCompleted -= this.OnTransitionCompleted;
            }

            if (this.flyout != null)
            {
                this.flyout.OpeningFinished -= this.Flyout_OpeningFinished;
                this.flyout.IsOpenChanged -= this.Flyout_IsOpenChanged;
                this.flyout.ClosingFinished -= this.Flyout_ClosingFinished;
            }

            this.Unloaded -= this.CustomValidationPopup_Unloaded;
            this.Opened -= this.CustomValidationPopup_Opened;
            this.hostWindow = null;
        }

        protected virtual void OnLoaded()
        {
        }

        protected virtual void OnUnLoaded()
        {
        }

        private void OnHostWindowStateChanged(object sender, EventArgs e)
        {
            if (this.hostWindow != null && this.hostWindow.WindowState != WindowState.Minimized)
            {
                var adornedElement = this.AdornedElement;
                if (adornedElement != null)
                {
                    this.PopupAnimation = PopupAnimation.None;
                    this.SetCurrentValue(IsOpenProperty, BooleanBoxes.FalseBox);
                    var errorTemplate = adornedElement.GetValue(Validation.ErrorTemplateProperty);
                    adornedElement.SetValue(Validation.ErrorTemplateProperty, null);
                    adornedElement.SetValue(Validation.ErrorTemplateProperty, errorTemplate);
                }
            }
        }

        private void OnSizeOrLocationChanged(object sender, EventArgs e)
        {
            this.RefreshPosition();
        }

        private void RefreshPosition()
        {
            var offset = this.HorizontalOffset;
            // "bump" the offset to cause the popup to reposition itself on its own
            this.SetCurrentValue(HorizontalOffsetProperty, offset + 1);
            this.SetCurrentValue(HorizontalOffsetProperty, offset);
        }

        private bool? appliedTopMost;

        private void SetTopmostState(bool isTop)
        {
            // Dont apply state if its the same as incoming state
            if (this.appliedTopMost.HasValue && this.appliedTopMost == isTop)
            {
                return;
            }

            if (this.Child is null)
            {
                return;
            }

            if (!(PresentationSource.FromVisual(this.Child) is HwndSource hwndSource))
            {
                return;
            }

            var handle = hwndSource.Handle;

#pragma warning disable 618
            if (!UnsafeNativeMethods.GetWindowRect(handle, out var rect))
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
                NativeMethods.SetWindowPos(handle, Constants.HWND_TOPMOST, left, top, width, height, SWP.TOPMOST);
            }
            else
            {
                // Z-Order would only get refreshed/reflected if clicking the
                // the titlebar (as opposed to other parts of the external
                // window) unless I first set the popup to HWND_BOTTOM
                // then HWND_TOP before HWND_NOTOPMOST
                NativeMethods.SetWindowPos(handle, Constants.HWND_BOTTOM, left, top, width, height, SWP.TOPMOST);
                NativeMethods.SetWindowPos(handle, Constants.HWND_TOP, left, top, width, height, SWP.TOPMOST);
                NativeMethods.SetWindowPos(handle, Constants.HWND_NOTOPMOST, left, top, width, height, SWP.TOPMOST);
            }

            this.appliedTopMost = isTop;
#pragma warning restore 618
        }
    }
}