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
using Microsoft.DwayneNeed.Win32.User32;

namespace Microsoft.DwayneNeed.Interop
{
    /// <summary>
    ///     A special type of RedirectedHwndHost that hosts more WPF content
    ///     via an HwndSource.  Since WPF is on both sides of the HWND
    ///     boundary, we can use the logical tree to bridge across, rather
    ///     than the less functional IKeyboardInputSink.
    /// </summary>    
    [ContentProperty("Child")]
    public class RedirectedHwndSourceHost : RedirectedHwndHost
    {
        /// <summary>
        ///     The child of this HwndSourceHost.
        /// </summary>
        public static DependencyProperty ChildProperty = DependencyProperty.Register(
            /* Name:                 */ "Child",
            /* Value Type:           */ typeof(FrameworkElement),
            /* Owner Type:           */ typeof(RedirectedHwndSourceHost),
            /* Metadata:             */ new PropertyMetadata(
            /*     Default Value:    */ null,
            /*     Property Changed: */ (d, e) => ((RedirectedHwndSourceHost)d).OnChildChanged(e)));

        public FrameworkElement Child
        {
            get { return (FrameworkElement)GetValue(ChildProperty); }
            set { SetValue(ChildProperty, value); }
        }

        protected sealed override HWND BuildWindowCore(HWND hwndParent)
        {
            HwndSourceParameters hwndSourceParameters = new HwndSourceParameters();
            hwndSourceParameters.WindowStyle = (int)(WS.VISIBLE | WS.CHILD | WS.CLIPSIBLINGS | WS.CLIPCHILDREN);
            //hwndSourceParameters.ExtendedWindowStyle = (int)(WS_EX.NOACTIVATE);
            hwndSourceParameters.ParentWindow = hwndParent.DangerousGetHandle();

            _hwndSource = new HwndSource(hwndSourceParameters);
            _hwndSource.SizeToContent = SizeToContent.Manual;

            // TODO: make this an option
            // On Vista, or when Win7 uses vista-blit, DX content is not
            // available via BitBlit or PrintWindow?  If WPF is using hardware
            // acceleration, anything it renders won't be available either.
            // One workaround is to force WPF to use software rendering.  Of
            // course, this is only a partial workaround since other content
            // like XNA or D2D won't work either.
            //_hwndSource.CompositionTarget.RenderMode = RenderMode.SoftwareOnly;

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
