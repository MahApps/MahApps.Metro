using System;
using System.Windows.Interop;
using System.Runtime.InteropServices;
using System.Windows;
using Microsoft.DwayneNeed.Extensions;
using Microsoft.DwayneNeed.Win32.User32;
using System.Diagnostics;
using Microsoft.DwayneNeed.Win32;

namespace Microsoft.DwayneNeed.Interop
{
    /// <summary>
    ///     This class works around issues with the HwndHost in current WPF versions.
    /// </summary>
    public class HwndHostEx : HwndHost, IKeyboardInputSink
    {
        /// <summary>
        ///     The behavior for handling of the SWP_NOCOPYBITS flag during
        ///     moving or sizing operations.
        /// </summary>
        /// <remarks>
        ///     Real implementation is HwndHostExtensions.CopyBitsBehaviorProperty
        /// </remarks>
        public static readonly DependencyProperty CopyBitsBehaviorProperty = HwndHostExtensions.CopyBitsBehaviorProperty.AddOwner(typeof(HwndHostEx));
        
        /// <summary>
        ///     The behavior for handling of the SWP_NOCOPYBITS flag during
        ///     moving or sizing operations.
        /// </summary>
        public CopyBitsBehavior CopyBitsBehavior
        {
            get { return (CopyBitsBehavior) GetValue(CopyBitsBehaviorProperty); }
            set { SetValue(CopyBitsBehaviorProperty, value); }
        }

        /// <summary>
        ///     Specifies whether or not a HwndHostCommands.MouseActivate
        ///     command is raised in response to WM_MOUSEACTIVATE.
        /// </summary>
        /// <remarks>
        ///     Real implementation is HwndHostExtensions.RaiseMouseActivateCommandProperty
        /// </remarks>
        public static readonly DependencyProperty RaiseMouseActivateCommandProperty = HwndHostExtensions.RaiseMouseActivateCommandProperty;

        /// <summary>
        ///     Specifies whether or not a HwndHostCommands.MouseActivate
        ///     command is raised in response to WM_MOUSEACTIVATE.
        /// </summary>
        public bool RaiseMouseActivateCommand
        {
            get { return (bool)GetValue(RaiseMouseActivateCommandProperty); }
            set { SetValue(RaiseMouseActivateCommandProperty, value); }
        }

        // Set the window position asynchronously since for nested WPF
        // layouts, the layout manager will refuse to raise the
        // LayoutUpdated event which we depend on.
        //
        // This is very important since HwndSourceHost will cause this
        // nesting scenario very frequently.
        /// <summary>
        ///     Specifies whether or not to set the window position of the
        ///     hosted window asynchronously.
        /// </summary>
        /// <remarks>
        ///     HwndHost depends on the LayoutUpdated event to synchronize the
        ///     position of the HWND with the layout slot.  However, when WPF
        ///     gets into a recursive/nested layout loop, the LayoutUpdated
        ///     event is not raised.  Since we resize the hosted HWND in
        ///     response to this event, if the hosted HWND contains an
        ///     HwndSource, that HwndSource will run an immediate layout pass,
        ///     but the LayoutUpdated event will not be raised.
        ///     TODO: fix in 4.5?
        /// </remarks>
        public static readonly DependencyProperty AsyncUpdateWindowPosProperty = DependencyProperty.Register(
            "AsyncUpdateWindowPos",
            typeof(bool),
            typeof(HwndHostEx),
            new FrameworkPropertyMetadata(false));

        public bool AsyncUpdateWindowPos
        {
            get { return (bool)GetValue(AsyncUpdateWindowPosProperty); }
            set { SetValue(AsyncUpdateWindowPosProperty, value); }
        }

        protected sealed override HandleRef BuildWindowCore(HandleRef hwndParent)
        {
            HWND hwndParent2 = new HWND(hwndParent.Handle);
            _hwndChild = BuildWindowOverride(hwndParent2);

            // Ideally, the window would have been created with the expected
            // parent.  But just in case, we set it explicitly.
            NativeMethods.SetParent(_hwndChild, hwndParent2);

            return new HandleRef(this, _hwndChild.DangerousGetHandle());
        }

        protected sealed override void DestroyWindowCore(HandleRef hwnd)
        {
            HWND hwndChild = new HWND(hwnd.Handle);
            Debug.Assert(hwndChild == _hwndChild); // Equivalency, not reference equality.
            
            DestroyWindowOverride(_hwndChild);

            // Release our reference to the child HWND.  Note that we do not
            // explicitly dispose this handle, because we let
            // DestroyWindowOverride decide what to do with it.
            _hwndChild = null;
        }
               
        /// <summary>
        ///     Default implementation of BuildWindowOverride, which just
        ///     creates a simple "STATIC" HWND.  This is almost certainly not
        ///     the desired window, but at least something shows up on the
        ///     screen. Override this method in your derived class and build
        ///     the window you want.
        /// </summary>
        protected virtual HWND BuildWindowOverride(HWND hwndParent)
        {
            return NativeMethods.CreateWindowEx(
                0,
                "STATIC",
                "Default HwndHostEx Window",
                WS.CHILD | WS.CLIPSIBLINGS | WS.CLIPCHILDREN,
                0,
                0,
                0,
                0,
                hwndParent,
                IntPtr.Zero,
                IntPtr.Zero,
                IntPtr.Zero);
        }

        /// <summary>
        ///     Default implementation of DestroyWindowCore, which just
        ///     destroys the hosted window.  If this is undesirable,
        ///     override this method and provide alternative logic.
        /// </summary>
        protected virtual void DestroyWindowOverride(HWND hwnd)
        {
            NativeMethods.DestroyWindow(hwnd);
        }

        protected sealed override IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            Debug.Assert(hwnd == _hwndChild.DangerousGetHandle());

            bool messageHandledOldValue = _messageHandled;
            try
            {
                _messageHandled = true;
                IntPtr result = WndProcOverride(_hwndChild, (WM)msg, wParam, lParam);

                handled = _messageHandled;
                return result;
            }
            finally
            {
                _messageHandled = messageHandledOldValue;
            }
        }

        protected virtual IntPtr WndProcOverride(HWND hwnd, WM msg, IntPtr wParam, IntPtr lParam)
        {
            // The default implementation is to mark the message as not
            // handled.
            _messageHandled = false;
            return IntPtr.Zero;
        }

        /// <summary>
        /// </summary>
        protected override void OnWindowPositionChanged(Rect rcBoundingBox)
        {
            bool callBase = false;
            bool callAsync = false;

            if (_lastWindowPosChanged != null)
            {
                // Only call the base if something changed since last time.
                if (rcBoundingBox != _lastWindowPosChanged.Value)
                {
                    callBase = true;

                    if (rcBoundingBox.Width != _lastWindowPosChanged.Value.Width ||
                        rcBoundingBox.Height != _lastWindowPosChanged.Value.Height)
                    {
                        callAsync = true;
                    }
                }
            }
            else
            {
                // First time.
                callBase = true;
                callAsync = true;
            }

            if (callBase)
            {
                if (callAsync)
                {
                    Dispatcher.BeginInvoke((Action)delegate
                    {
                        base.OnWindowPositionChanged(rcBoundingBox);
                    });
                }
                else
                {
                    base.OnWindowPositionChanged(rcBoundingBox);
                }
            }
            _lastWindowPosChanged = rcBoundingBox;
        }

        private Rect? _lastWindowPosChanged = null;
        private HWND _hwndChild;
        private bool _messageHandled;
    }
}
