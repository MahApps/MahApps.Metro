using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Runtime.ConstrainedExecution;
using System.Diagnostics;
using Microsoft.DwayneNeed.Win32.User32;
using Microsoft.DwayneNeed.Win32.ComCtl32;
using Microsoft.DwayneNeed.Win32;

namespace Microsoft.DwayneNeed.Win32.ComCtl32
{
    /// <summary>
    ///     WindowSubclass hooks into the stream of messages that are dispatched to
    ///     a window.
    /// </summary>
    public abstract class WindowSubclass : CriticalFinalizerObject, IDisposable
    {
        static WindowSubclass()
        {
            _disposeMessage = NativeMethods.RegisterWindowMessage("WindowSubclass.DisposeMessage");
        }

        /// <summary>
        ///     Hooks into the stream of messages that are dispatched to the
        ///     specified window.
        /// </summary>
        /// <remarks>
        ///     The window must be owned by the calling thread.
        /// </remarks>
        public WindowSubclass(HWND hwnd)
        {
            if (!IsCorrectThread(hwnd))
            {
                throw new InvalidOperationException("May not hook a window created by a different thread.");
            }

            _hwnd = hwnd;
            _wndproc = WndProcStub;
            _wndprocPtr = Marshal.GetFunctionPointerForDelegate(_wndproc);

            // Because our window proc is an instance method, it is connected
            // to this instance of WindowSubclass, where we can store state.
            // We do not need to store any extra state in native memory.
            NativeMethods.SetWindowSubclass(hwnd, _wndproc, IntPtr.Zero, IntPtr.Zero);
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            DisposeHelper(true);
        }

        ~WindowSubclass()
        {
            // The finalizer is our last chance!  The finalizer is always on
            // the wrong thread, but we need to ensure that native code will
            // not try to call into us since we are being removed from memory.
            // Even though it is dangerous, and we risk a deadlock, we
            // send the dispose message to the WndProc to remove itself on
            // the correct thread.
            DisposeHelper(false);
        }

        protected virtual void Dispose(bool disposing)
        {
            if(_hwnd == null || !IsCorrectThread(_hwnd))
            {
                throw new InvalidOperationException("Dispose virtual should only be called by WindowSubclass once on the correct thread.");
            }

            NativeMethods.RemoveWindowSubclass(_hwnd, _wndproc, IntPtr.Zero);
            _hwnd = null;
        }

        protected virtual IntPtr WndProcOverride(HWND hwnd, WM msg, IntPtr wParam, IntPtr lParam, IntPtr id, IntPtr data)
        {
            // Call the next window proc in the subclass chain.
            return NativeMethods.DefSubclassProc(hwnd, msg, wParam, lParam);
        }

        protected HWND Hwnd
        {
            get
            {
                return _hwnd;
            }
        }

        private bool IsCorrectThread(HWND hwnd)
        {
            int processId;
            int threadId = NativeMethods.GetWindowThreadProcessId(hwnd, out processId);

            return (processId == NativeMethods.GetCurrentProcessId() && threadId == NativeMethods.GetCurrentThreadId());
        }

        private void DisposeHelper(bool disposing)
        {
            HWND hwnd = _hwnd;

            if (hwnd != null)
            {
                if (IsCorrectThread(hwnd))
                {
                    // Call the virtual Dispose(bool)
                    Dispose(disposing);
                }
                else
                {
                    // Send a message to the right thread to dispose for us.
                    NativeMethods.SendMessage(hwnd, _disposeMessage, _wndprocPtr, disposing ? new IntPtr(1) : IntPtr.Zero);
                }
            }
        }

        private IntPtr WndProcStub(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, IntPtr id, IntPtr data)
        {
            HWND hwnd2 = new HWND(hwnd);
            return WndProc(hwnd2, (WM)msg, wParam, lParam, id, data);
        }

        private IntPtr WndProc(HWND hwnd, WM msg, IntPtr wParam, IntPtr lParam, IntPtr id, IntPtr data)
        {
            IntPtr retval = IntPtr.Zero;

            try
            {
                retval = WndProcOverride(hwnd, msg, wParam, lParam, id, data);
            }
            finally
            {
                if (_hwnd != HWND.NULL)
                {
                    Debug.Assert(_hwnd == hwnd);

                    if (msg == WM.NCDESTROY)
                    {
                        Dispose();
                    }
                    else if (msg == _disposeMessage && wParam == _wndprocPtr)
                    {
                        DisposeHelper(lParam == IntPtr.Zero ? false : true);
                    }
                }
            }

            return retval;
        }

        private HWND _hwnd;
        private SUBCLASSPROC _wndproc;
        private IntPtr _wndprocPtr;
        private static readonly WM _disposeMessage;
    }
}

