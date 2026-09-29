// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Threading;
using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.UI.WindowsAndMessaging;
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
        private Window? hostWindow;
        private ScrollViewer? scrollViewer;
        private MetroContentControl? metroContentControl;
        private TransitioningContentControl? transitioningContentControl;
        private Flyout? flyout;

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

        /// <summary>Identifies the <see cref="ShowValidationErrorOnKeyboardFocus"/> dependency property.</summary>
        public static readonly DependencyProperty ShowValidationErrorOnKeyboardFocusProperty
            = DependencyProperty.Register(nameof(ShowValidationErrorOnKeyboardFocus),
                                          typeof(bool),
                                          typeof(CustomValidationPopup),
                                          new PropertyMetadata(BooleanBoxes.TrueBox));

        /// <summary>
        /// Gets or sets whether the validation error text will be shown when the element has the keyboard focus.
        /// </summary>
        public bool ShowValidationErrorOnKeyboardFocus
        {
            get => (bool)this.GetValue(ShowValidationErrorOnKeyboardFocusProperty);
            set => this.SetValue(ShowValidationErrorOnKeyboardFocusProperty, BooleanBoxes.Box(value));
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
        public UIElement? AdornedElement
        {
            get => (UIElement?)this.GetValue(AdornedElementProperty);
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

        private void CustomValidationPopup_Loaded(object? sender, RoutedEventArgs e)
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
            this.SetValue(CanShowPropertyKey, BooleanBoxes.Box(canShow && this.IsAdornedElementInView()));

            if (canShow)
            {
                // the element and the view it sits in are measured after this, so whether one can
                // be seen in the other is only settled a moment later
                this.Dispatcher.BeginInvoke(DispatcherPriority.Loaded,
                                            new Action(() => this.SetValue(CanShowPropertyKey, BooleanBoxes.Box(this.IsAdornedElementInView()))));
            }

            this.OnLoaded();

            this.Unloaded -= this.CustomValidationPopup_Unloaded;
            this.Unloaded += this.CustomValidationPopup_Unloaded;
        }

        private bool ShouldPopupOpen()
        {
            var adornedElement = this.AdornedElement;
            var isOpen = adornedElement is not null
                         && Validation.GetHasError(adornedElement)
                         && (ValidationHelper.GetAlwaysShowValidationError(adornedElement)
                             || (adornedElement.IsKeyboardFocusWithin
                                 && this.ShowValidationErrorOnKeyboardFocus
                                 && ValidationHelper.GetShowValidationErrorOnKeyboardFocus(adornedElement)));
            return isOpen;
        }

        private void Flyout_OpeningFinished(object? sender, RoutedEventArgs e)
        {
            this.RefreshPosition();

            var inView = this.IsAdornedElementInView();
            this.SetCurrentValue(IsOpenProperty, BooleanBoxes.Box(inView && this.ShouldPopupOpen()));
            this.SetValue(CanShowPropertyKey, BooleanBoxes.Box(inView));
        }

        private void Flyout_IsOpenChanged(object? sender, RoutedEventArgs e)
        {
            this.RefreshPosition();
            this.SetValue(CanShowPropertyKey, BooleanBoxes.FalseBox);
        }

        private void Flyout_ClosingFinished(object? sender, RoutedEventArgs e)
        {
            this.RefreshPosition();
            this.SetValue(CanShowPropertyKey, BooleanBoxes.FalseBox);
        }

        private void OnTransitionStarted(object? sender, RoutedEventArgs e)
        {
            this.RefreshPosition();
            this.SetValue(CanShowPropertyKey, BooleanBoxes.FalseBox);
        }

        private void OnTransitionCompleted(object? sender, RoutedEventArgs e)
        {
            this.RefreshPosition();

            var inView = this.IsAdornedElementInView();
            this.SetCurrentValue(IsOpenProperty, BooleanBoxes.Box(inView && this.ShouldPopupOpen()));
            this.SetValue(CanShowPropertyKey, BooleanBoxes.Box(inView));
        }

        private void ScrollViewer_ScrollChanged(object? sender, ScrollChangedEventArgs e)
        {
            if (e.VerticalChange is > 0 or < 0 || e.HorizontalChange is > 0 or < 0)
            {
                this.RefreshPosition();

                var inView = this.IsAdornedElementInView();
                this.SetValue(CanShowPropertyKey, BooleanBoxes.Box(inView));
                this.SetCurrentValue(IsOpenProperty, BooleanBoxes.Box(inView && this.ShouldPopupOpen()));
            }
        }

        /// <summary>
        /// Whether the element the message belongs to is there to be seen. The message is shown
        /// beside that element, so a message for an element the view it sits in has scrolled away
        /// would float on its own, next to nothing, which is what GH-4404 shows.
        /// </summary>
        private bool IsAdornedElementInView()
        {
            if (this.AdornedElement is not FrameworkElement adornedElement || !adornedElement.IsVisible)
            {
                return false;
            }

            // the popup is asked this before it is loaded as well, so what it found back then is
            // only a shortcut here and not what the answer rests on
            var view = this.scrollViewer ?? adornedElement.GetVisualAncestor<ScrollViewer>();

            // neither of the two answers anything before it has been measured, and a question
            // without an answer is no reason to keep the message away
            if (view is null
                || view.ActualWidth <= 0 || view.ActualHeight <= 0
                || adornedElement.ActualWidth <= 0 || adornedElement.ActualHeight <= 0)
            {
                return true;
            }

            return IsElementVisible(adornedElement, view);
        }

        private static bool IsElementVisible(FrameworkElement? element, FrameworkElement? container)
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

        private void CustomValidationPopup_Opened(object? sender, EventArgs e)
        {
            if (!this.IsAdornedElementInView())
            {
                this.SetValue(CanShowPropertyKey, BooleanBoxes.FalseBox);
                this.SetCurrentValue(IsOpenProperty, BooleanBoxes.FalseBox);
                return;
            }

            // the window behind the message is made before it is placed, so it stands at the corner
            // of the screen for as long as that takes, which is the flash GH-4404 reports
            this.RefreshPosition();

            this.SetTopmostState(true);
        }

        private void OnHostWindowActivated(object? sender, EventArgs e)
        {
            this.SetTopmostState(true);
        }

        private void OnHostWindowDeactivated(object? sender, EventArgs e)
        {
            this.SetTopmostState(false);
        }

        private void CustomValidationPopup_Unloaded(object? sender, RoutedEventArgs e)
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

        private void OnHostWindowStateChanged(object? sender, EventArgs e)
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

        private void OnSizeOrLocationChanged(object? sender, EventArgs e)
        {
            this.RefreshPosition();

            // a window that is made smaller can put the box out of the view it sits in just as
            // scrolling does, and then the message has nothing left to stand beside
            var inView = this.IsAdornedElementInView();
            this.SetValue(CanShowPropertyKey, BooleanBoxes.Box(inView));
            this.SetCurrentValue(IsOpenProperty, BooleanBoxes.Box(inView && this.ShouldPopupOpen()));
        }

        private void RefreshPosition()
        {
            var offset = this.HorizontalOffset;
            // "bump" the offset to cause the popup to reposition itself on its own
            this.SetCurrentValue(HorizontalOffsetProperty, offset + 1);
            this.SetCurrentValue(HorizontalOffsetProperty, offset);
        }

        private bool? appliedTopMost;

        private unsafe void SetTopmostState(bool isTop)
        {
            // Don't apply state if it's the same as incoming state
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

            var handle = new HWND(hwndSource.Handle);

            var rect = new RECT();
            PInvoke.GetWindowRect(handle, &rect);
            if (rect.left == 0
                && rect.top == 0
                && rect.right == 0
                && rect.bottom == 0)
            {
                return;
            }
            //Debug.WriteLine("setting z-order " + isTop);

            const SET_WINDOW_POS_FLAGS SWP_TOPMOST = SET_WINDOW_POS_FLAGS.SWP_NOACTIVATE | SET_WINDOW_POS_FLAGS.SWP_NOOWNERZORDER | SET_WINDOW_POS_FLAGS.SWP_NOSIZE | SET_WINDOW_POS_FLAGS.SWP_NOMOVE | SET_WINDOW_POS_FLAGS.SWP_NOREDRAW | SET_WINDOW_POS_FLAGS.SWP_NOSENDCHANGING;

            var left = rect.left;
            var top = rect.top;
            var width = rect.GetWidth();
            var height = rect.GetHeight();
            if (isTop)
            {
                PInvoke.SetWindowPos(handle, HWND.HWND_TOPMOST, left, top, width, height, SWP_TOPMOST);
            }
            else
            {
                // Z-Order would only get refreshed/reflected if clicking the
                // the titlebar (as opposed to other parts of the external
                // window) unless I first set the popup to HWND_BOTTOM
                // then HWND_TOP before HWND_NOTOPMOST
                PInvoke.SetWindowPos(handle, HWND.HWND_BOTTOM, left, top, width, height, SWP_TOPMOST);
                PInvoke.SetWindowPos(handle, HWND.HWND_TOP, left, top, width, height, SWP_TOPMOST);
                PInvoke.SetWindowPos(handle, HWND.HWND_NOTOPMOST, left, top, width, height, SWP_TOPMOST);
            }

            this.appliedTopMost = isTop;
        }
    }
}