using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.DwayneNeed.Interop;
using System.Windows;
using System.Windows.Interop;
using Microsoft.DwayneNeed.Win32.User32;
using Microsoft.DwayneNeed.Win32.ComCtl32;
using Microsoft.DwayneNeed.Win32;

namespace Microsoft.DwayneNeed.Extensions
{
    public static class HwndHostExtensions
    {
        /// <summary>
        ///     Attached property for HwndHost instances that specifies the
        ///     behavior for handling the SWP_NOCOPYBITS flag during moving
        ///     or sizing operations.
        /// </summary>
        public static readonly DependencyProperty CopyBitsBehaviorProperty = DependencyProperty.RegisterAttached(
            "CopyBitsBehavior",
            typeof(CopyBitsBehavior),
            typeof(HwndHostExtensions),
            new FrameworkPropertyMetadata(
                CopyBitsBehavior.Default,
                new PropertyChangedCallback(OnCopyBitsBehaviorChanged)));

        /// <summary>
        ///     Attached property for HwndHost instances that specifies
        ///     whether or not a HwndHostCommands.MouseActivate command is
        ///     raised in response to WM_MOUSEACTIVATE.
        /// </summary>
        public static readonly DependencyProperty RaiseMouseActivateCommandProperty = DependencyProperty.RegisterAttached(
            "RaiseMouseActivateCommand",
            typeof(bool),
            typeof(HwndHostExtensions),
            new FrameworkPropertyMetadata(
                false,
                new PropertyChangedCallback(OnRaiseMouseActivateCommandChanged)));

        /// <summary>
        ///     Attached property that is used to store the HwndHook used to
        ///     intercept the messages to the HwndHost window.  We have to
        ///     store this to prevent premature garbage collection.
        /// </summary>
        private static readonly DependencyProperty WindowHookProperty = DependencyProperty.RegisterAttached(
            "WindowHook",
            typeof(HwndHostExtensionsWindowHook),
            typeof(HwndHostExtensions),
            new FrameworkPropertyMetadata(null));

        /// <summary>
        ///     Attached property that is used to record the users of the
        ///     attached window hook.  It is basically a simple reference
        ///     counting scheme.
        /// </summary>
        private static readonly DependencyProperty WindowHookRefCountProperty = DependencyProperty.RegisterAttached(
            "WindowHookRefCount",
            typeof(int),
            typeof(HwndHostExtensions),
            new FrameworkPropertyMetadata(0));

        /// <summary>
        ///     Attached property getter for the CopyBitsBehavior property.
        /// </summary>
        public static CopyBitsBehavior GetCopyBitsBehavior(this HwndHost @this)
        {
            return (CopyBitsBehavior)@this.GetValue(CopyBitsBehaviorProperty);
        }

        /// <summary>
        ///     Attached property setter for the CopyBitsBehavior property.
        /// </summary>
        public static void SetCopyBitsBehavior(this HwndHost @this, CopyBitsBehavior value)
        {
            @this.SetValue(CopyBitsBehaviorProperty, value);
        }

        private static void OnCopyBitsBehaviorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            HwndHost hwndHost = d as HwndHost;

            if (hwndHost != null)
            {
                CopyBitsBehavior newValue = (CopyBitsBehavior) e.NewValue;
                if(newValue != CopyBitsBehavior.Default)
                {
                    AddWndProcUsage(hwndHost);
                }
                else
                {
                    RemoveWndProcUsage(hwndHost);
                }
            }
        }

        /// <summary>
        ///     Attached property getter for the RaiseMouseActivateCommand property.
        /// </summary>
        public static bool GetRaiseMouseActivateCommand(this HwndHost @this)
        {
            return (bool)@this.GetValue(RaiseMouseActivateCommandProperty);
        }

        /// <summary>
        ///     Attached property setter for the RaiseMouseActivateCommand property.
        /// </summary>
        public static void SetRaiseMouseActivateCommand(this HwndHost @this, bool value)
        {
            @this.SetValue(RaiseMouseActivateCommandProperty, value);
        }

        private static void OnRaiseMouseActivateCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            HwndHost hwndHost = d as HwndHost;

            if (hwndHost != null)
            {
                bool newValue = (bool)e.NewValue;
                if (newValue)
                {
                    AddWndProcUsage(hwndHost);
                }
                else
                {
                    RemoveWndProcUsage(hwndHost);
                }
            }
        }

        private static void AddWndProcUsage(HwndHost hwndHost)
        {
            int refCount = (int) hwndHost.GetValue(WindowHookRefCountProperty);
            refCount++;
            hwndHost.SetValue(WindowHookRefCountProperty, refCount);

            if(refCount == 1)
            {
                if (!TryHookWndProc(hwndHost))
                {
                    // Try again later, when the HwndHost is loaded.
                    hwndHost.Loaded += (s, e) => TryHookWndProc((HwndHost)s);
                }
            }
        }

        private static bool TryHookWndProc(HwndHost hwndHost)
        {
            if (hwndHost.Handle != IntPtr.Zero)
            {
                // Hook the window messages so we can intercept the
                // various messages.
                HwndHostExtensionsWindowHook hook = new HwndHostExtensionsWindowHook(hwndHost);

                // Keep our hook alive.
                hwndHost.SetValue(WindowHookProperty, hook);

                return true;
            }
            else
            {
                return false;
            }
        }

        private static void RemoveWndProcUsage(HwndHost hwndHost)
        {
            int refCount = (int) hwndHost.GetValue(WindowHookRefCountProperty);
            refCount--;
            hwndHost.SetValue(WindowHookRefCountProperty, refCount);

            if (refCount == 0)
            {
                HwndHostExtensionsWindowHook hook = (HwndHostExtensionsWindowHook) hwndHost.GetValue(WindowHookProperty);
                hook.Dispose();
                hwndHost.ClearValue(WindowHookProperty);
            }
        }

        private class HwndHostExtensionsWindowHook : WindowSubclass
        {
            static HwndHostExtensionsWindowHook()
            {
                _redrawMessage = NativeMethods.RegisterWindowMessage("HwndHostExtensionsWindowHook.RedrawMessage");
            }

            public HwndHostExtensionsWindowHook(HwndHost hwndHost)
                : base(new HWND(hwndHost.Handle))
            {
                _hwndHost = hwndHost;
            }

            protected override IntPtr WndProcOverride(HWND hwnd, WM msg, IntPtr wParam, IntPtr lParam, IntPtr id, IntPtr data)
            {
                IntPtr? result = null;

                if (msg == WM.WINDOWPOSCHANGING)
                {
                    unsafe
                    {
                        WINDOWPOS* pWindowPos = (WINDOWPOS*)lParam;

                        CopyBitsBehavior copyBitsBehavior = _hwndHost.GetCopyBitsBehavior();

                        switch(copyBitsBehavior)
                        {
                            case CopyBitsBehavior.AlwaysCopyBits:
                                pWindowPos->flags &= ~SWP.NOCOPYBITS;
                                break;

                            case CopyBitsBehavior.CopyBitsAndRepaint:
                                pWindowPos->flags &= ~SWP.NOCOPYBITS;
                                if (!_redrawMessagePosted)
                                {
                                    NativeMethods.PostMessage(hwnd, _redrawMessage, IntPtr.Zero, IntPtr.Zero);
                                    _redrawMessagePosted = true;
                                }
                                break;

                            case CopyBitsBehavior.NeverCopyBits:
                                pWindowPos->flags |= SWP.NOCOPYBITS;
                                break;

                            case CopyBitsBehavior.Default:
                            default:
                                // do nothing.
                                break;
                        }
                    }
                }
                else if (msg == _redrawMessage)
                {
                    _redrawMessagePosted = false;
                    
                    // Invalidate the window that moved, because it might have copied garbage
                    // due to WPF rendering through DX on a different thread.
                    NativeMethods.RedrawWindow(hwnd, IntPtr.Zero, IntPtr.Zero, RDW.INVALIDATE | RDW.ALLCHILDREN);

                    // Then immediately redraw all invalid regions within the top-level window.
                    HWND hwndRoot = NativeMethods.GetAncestor(hwnd, GA.ROOT);
                    NativeMethods.RedrawWindow(hwndRoot, IntPtr.Zero, IntPtr.Zero, RDW.UPDATENOW | RDW.ALLCHILDREN);
                }
                else if (msg == WM.MOUSEACTIVATE)
                {
                    bool raiseMouseActivateCommand = _hwndHost.GetRaiseMouseActivateCommand();
                    if (raiseMouseActivateCommand)
                    {
                        // Raise the HwndHostCommands.MouseActivate command.
                        MouseActivateParameter parameter = new MouseActivateParameter();
                        HwndHostCommands.MouseActivate.Execute(parameter, _hwndHost);

                        if (parameter.HandleMessage)
                        {
                            if (parameter.Activate == false && parameter.EatMessage == false)
                            {
                                result = new IntPtr((int)MA.NOACTIVATE);
                            }
                            else if (parameter.Activate == false && parameter.EatMessage == true)
                            {
                                result = new IntPtr((int)MA.NOACTIVATEANDEAT);
                            }
                            else if (parameter.Activate == true && parameter.EatMessage == false)
                            {
                                result = new IntPtr((int)MA.ACTIVATE);
                            }
                            else // if(parameter.Activate == true && parameter.EatMessage == true)
                            {
                                result = new IntPtr((int)MA.ACTIVATEANDEAT);
                            }
                        }
                    }
                }

                return result ?? base.WndProcOverride(hwnd, msg, wParam, lParam, id, data);
            }

            private HwndHost _hwndHost;
            private static readonly WM _redrawMessage;
            private bool _redrawMessagePosted;
        }
    }
}
