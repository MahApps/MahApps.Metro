using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Markup;
using System.Windows;
using System.Runtime.InteropServices;
using System.Windows.Interop;
using System.Windows.Input;
using System.Diagnostics;
using System.Collections;
using System.Windows.Media;
using Microsoft.DwayneNeed.Extensions;
using Microsoft.DwayneNeed;
using Microsoft.DwayneNeed.Win32.User32;

namespace Microsoft.DwayneNeed.Interop
{
    /// <summary>
    ///     A special type of HwndHost that hosts more WPF content via an
    ///     HwndSource.  Since WPF is on both sides of the HWND boundary,
    ///     we can use the logical tree to bridge across, rather than the
    ///     less functional IKeyboardInputSink.
    /// </summary>    
    [ContentProperty("Child")]
    public class HwndSourceHost : HwndHostEx
    {
        #region Background
        public static readonly DependencyProperty BackgroundProperty = DependencyProperty.Register(
            /* Name:                 */ "Background",
            /* Value Type:           */ typeof(Brush),
            /* Owner Type:           */ typeof(HwndSourceHost),
            /* Metadata:             */ new FrameworkPropertyMetadata(
            /*     Default Value:    */ null,
            /*     Flags:            */ FrameworkPropertyMetadataOptions.AffectsRender));

        /// <summary>
        ///     The brush to paint the background.
        /// </summary>
        public Brush Background
        {
            get { return (Brush)GetValue(BackgroundProperty); }
            set { SetValue(BackgroundProperty, value); }
        }
        #endregion

        #region Child
        /// <summary>
        ///     The child of this HwndSourceHost.
        /// </summary>
        public static DependencyProperty ChildProperty = DependencyProperty.Register(
            /* Name:                 */ "Child",
            /* Value Type:           */ typeof(FrameworkElement),
            /* Owner Type:           */ typeof(HwndSourceHost),
            /* Metadata:             */ new PropertyMetadata(
            /*     Default Value:    */ null,
            /*     Property Changed: */ (d, e) => ((HwndSourceHost)d).OnChildChanged(e)));

        public FrameworkElement Child
        {
            get { return (FrameworkElement)GetValue(ChildProperty); }
            set { SetValue(ChildProperty, value); }
        }
        #endregion

        protected sealed override HWND BuildWindowOverride(HWND hwndParent)
        {
            HwndSourceParameters hwndSourceParameters = new HwndSourceParameters();
            hwndSourceParameters.WindowStyle = (int)(WS.VISIBLE | WS.CHILD | WS.CLIPSIBLINGS | WS.CLIPCHILDREN);
            hwndSourceParameters.ParentWindow = hwndParent.DangerousGetHandle();

            _hwndSource = new HwndSource(hwndSourceParameters);
            _hwndSource.SizeToContent = SizeToContent.Manual;

            // Set the root visual of the HwndSource to an instance of
            // HwndSourceHostRoot.  Hook it up as a logical child if
            // we are on the same thread.
            HwndSourceHostRoot root = new HwndSourceHostRoot();
            _hwndSource.RootVisual = root;

            root.OnMeasure += OnRootMeasured;
            AddLogicalChild(_hwndSource.RootVisual);

            SetRootVisual(Child);

            return new HWND(_hwndSource.Handle);
        }

        protected sealed override void DestroyWindowOverride(HWND hwnd)
        {
            Debug.Assert(hwnd.DangerousGetHandle() == _hwndSource.Handle);

            _hwndSource.Dispose();
            _hwndSource = null;
        }

        /// <summary>
        ///     Determine the desired size of this element within the
        ///     specified constraints.
        /// </summary>
        protected override Size MeasureOverride(Size constraint)
        {
            if (_hwndSource != null && _hwndSource.RootVisual != null)
            {
                HwndSourceHostRoot root = (HwndSourceHostRoot)_hwndSource.RootVisual;

                // We are a simple pass-through element.
                root.Measure(constraint);

                return root.DesiredSize;
            }
            else
            {
                // We don't have a child yet.
                return new Size();
            }
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            if (_hwndSource != null && _hwndSource.RootVisual != null)
            {
                UIElement root = (UIElement)_hwndSource.RootVisual;

                // We are a simple pass-through element.
                root.Arrange(new Rect(finalSize));
                return root.RenderSize;
            }
            else
            {
                // We don't have a child yet.
                return finalSize;
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                // Walk the visual tree looking for disposable elements.
                if (_hwndSource != null)
                {
                    UIElement root = _hwndSource.RootVisual as UIElement;
                    if (root != null)
                    {
                        root.DisposeSubTree();
                    }

                    _hwndSource.Dispose();
                    _hwndSource = null;
                }
            }

            base.Dispose(disposing);
        }

        protected sealed override IEnumerator LogicalChildren
        {
            get
            {
                if (_hwndSource != null)
                {
                    yield return _hwndSource.RootVisual;
                }
            }
        }

        protected override void OnRender(System.Windows.Media.DrawingContext drawingContext)
        {
            // Draw something so that rendering does something.  This
            // is part of the fix for rendering artifacts, see WndProc
            // hooking WM_WINDOWPOSCHANGED too.
            //
            // TODO: this actually makes things worse!  At least on my home machine...
            if (Background != null)
            {
                drawingContext.DrawRectangle(Background, null, new Rect(RenderSize));
            }

            base.OnRender(drawingContext);
        }

        #region IKeyboardInputSink

        // Delegate IKeyboardInputSink calls to the hosted HwndSource.
        protected sealed override bool HasFocusWithinCore()
        {
            if (_hwndSource != null)
            {
                return ((IKeyboardInputSink)_hwndSource).HasFocusWithin();
            }
            else
            {
                return base.HasFocusWithinCore();
            }
        }

        // Delegate IKeyboardInputSink calls to the hosted HwndSource.
        protected sealed override bool OnMnemonicCore(ref MSG msg, ModifierKeys modifiers)
        {
            if (_hwndSource != null)
            {
                return ((IKeyboardInputSink)_hwndSource).OnMnemonic(ref msg, modifiers);
            }
            else
            {
                return base.OnMnemonicCore(ref msg, modifiers);
            }
        }

        // Delegate IKeyboardInputSink calls to the hosted HwndSource.
        protected sealed override bool TabIntoCore(TraversalRequest request)
        {
            if (_hwndSource != null)
            {
                return ((IKeyboardInputSink)_hwndSource).TabInto(request);
            }
            else
            {
                return base.TabIntoCore(request);
            }
        }

        // Delegate IKeyboardInputSink calls to the hosted HwndSource.
        protected sealed override bool TranslateAcceleratorCore(ref MSG msg, ModifierKeys modifiers)
        {
            if (_hwndSource != null)
            {
                HwndSourceHostRoot root = (HwndSourceHostRoot)_hwndSource.RootVisual;

                Debug.Assert(root.IsLogicalParentEnabled);
                root.IsLogicalParentEnabled = false;
                try
                {
                    return ((IKeyboardInputSink)_hwndSource).TranslateAccelerator(ref msg, modifiers);
                }
                finally
                {
                    root.IsLogicalParentEnabled = true;
                }
            }
            else
            {
                return base.TranslateAcceleratorCore(ref msg, modifiers);
            }
        }

        // Delegate IKeyboardInputSink calls to the hosted HwndSource.
        protected sealed override bool TranslateCharCore(ref MSG msg, ModifierKeys modifiers)
        {
            if (_hwndSource != null)
            {
                return ((IKeyboardInputSink)_hwndSource).TranslateChar(ref msg, modifiers);
            }
            else
            {
                return base.TranslateCharCore(ref msg, modifiers);
            }
        }
        #endregion

        private void OnChildChanged(DependencyPropertyChangedEventArgs e)
        {
            FrameworkElement child = (FrameworkElement)e.NewValue;
            if (_hwndSource != null)
            {
                SetRootVisual(child);
            }
        }

        private object SetRootVisual(object arg)
        {
            Debug.Assert(_hwndSource != null);

            FrameworkElement child = arg as FrameworkElement;
            if (child == null && arg is Uri)
            {
                child = (FrameworkElement)Application.LoadComponent((Uri)arg);
            }

            HwndSourceHostRoot root = (HwndSourceHostRoot)_hwndSource.RootVisual;
            root.Child = child;

            // Invalidate measure on this HwndHost so that we can remeasure
            // ourselves to our content.
            InvalidateMeasure();

            return null;
        }

        private void OnRootMeasured(object sender, EventArgs e)
        {
            // If the root visual gets measured, there is a good chance we may
            // need to be remeasured too.  But since we are not connected
            // visually, we need to propagate this manually.
            //
            // Note: sometimes we cause the measure ourselves, so there is no
            // need to propagate this back.
            InvalidateMeasure();
        }

        protected HwndSource _hwndSource;
    }
}
