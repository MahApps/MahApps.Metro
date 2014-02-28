using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.Runtime.InteropServices;
using System.Windows.Navigation;
using Microsoft.DwayneNeed.Interop;
using System.Reflection;
using Microsoft.DwayneNeed.Win32.User32;
using Microsoft.DwayneNeed.Win32.ComCtl32;
using Microsoft.DwayneNeed.Win32;

namespace Microsoft.DwayneNeed.Extensions
{
    public static class WebBrowserExtensions
    {
        /// <summary>
        ///     Attached property to suppress script errors.
        /// </summary>
        public static readonly DependencyProperty SuppressScriptErrorsProperty = DependencyProperty.RegisterAttached(
            "SuppressScriptErrors",
            typeof(bool),
            typeof(WebBrowserExtensions),
            new FrameworkPropertyMetadata(
                false,
                FrameworkPropertyMetadataOptions.Inherits,
                new PropertyChangedCallback(OnSuppressScriptErrorsChanged)));

        /// <summary>
        ///     Attached property that can be specified to subclass the
        ///     WebBrowser hosted IE window and suppress the WM_ERASEBKGND
        ///     message.
        /// </summary>
        public static readonly DependencyProperty SuppressEraseBackgroundProperty = DependencyProperty.RegisterAttached(
            "SuppressEraseBackground",
            typeof(bool),
            typeof(WebBrowserExtensions),
            new FrameworkPropertyMetadata(
                false,
                FrameworkPropertyMetadataOptions.Inherits,
                new PropertyChangedCallback(OnSuppressEraseBackgroundChanged)));

        /// <summary>
        ///     Attached property that is used to store the HwndHook used to
        ///     intercept the messages to the IE window.
        /// </summary>
        private static readonly DependencyProperty SuppressEraseBackgroundWindowHookProperty = DependencyProperty.RegisterAttached(
            "SuppressEraseBackgroundWindowHook",
            typeof(IEWindowHook),
            typeof(WebBrowserExtensions),
            new FrameworkPropertyMetadata(null));

        /// <summary>
        ///     Attached property getter for the SuppressScriptErrors property.
        /// </summary>
        public static bool GetSuppressScriptErrors(WebBrowser webBrowser)
        {
            return (bool)webBrowser.GetValue(SuppressScriptErrorsProperty);
        }

        /// <summary>
        ///     Attached property setter for the SuppressScriptErrors property.
        /// </summary>
        public static void SetSuppressScriptErrors(WebBrowser webBrowser, bool value)
        {
            webBrowser.SetValue(SuppressScriptErrorsProperty, value);
        }

        private static void OnSuppressScriptErrorsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            WebBrowser webBrowser = d as WebBrowser;
            if (webBrowser != null)
            {
                bool value = (bool) e.NewValue;

                if (!TrySetSuppressScriptErrors(webBrowser, value))
                {
                    webBrowser.Navigated += (s, e2) => { TrySetSuppressScriptErrors(webBrowser, value); };
                }
            }
        }

        private static bool TrySetSuppressScriptErrors(WebBrowser webBrowser, bool value)
        {
            FieldInfo field = typeof(WebBrowser).GetField("_axIWebBrowser2", BindingFlags.Instance | BindingFlags.NonPublic);
            if (field != null)
            {
                object axIWebBrowser2 = field.GetValue(webBrowser);
                if (axIWebBrowser2 != null)
                {
                    axIWebBrowser2.GetType().InvokeMember("Silent", BindingFlags.SetProperty, null, axIWebBrowser2, new object[] { value });
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        ///     Attached property getter for the SuppressEraseBackground property.
        /// </summary>
        /// <param name="element">
        ///     The element the property should be read from.
        /// </param>
        /// <returns>
        ///     The value of the SuppressEraseBackground property on the
        ///     specified element.
        /// </returns>
        public static bool GetSuppressEraseBackground(DependencyObject element)
        {
            return (bool)element.GetValue(SuppressEraseBackgroundProperty);
        }

        /// <summary>
        ///     Attached property setter for the SuppressEraseBackground property.
        /// </summary>
        /// <param name="webBrowser">
        ///     The element to property should be read from.
        /// </param>
        /// <param name="value">
        ///     The value of the SuppressEraseBackground property on the
        ///     specified element.
        /// </param>
        public static void SetSuppressEraseBackground(DependencyObject element, bool value)
        {
            element.SetValue(SuppressEraseBackgroundProperty, value);
        }

        private static void OnSuppressEraseBackgroundChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is WebBrowser)
            {
                WebBrowser webBrowser = (WebBrowser)d;

                bool newValue = (bool)e.NewValue;
                if (newValue)
                {
                    if (!TryHookWebBrowser(webBrowser))
                    {
                        // The IE window has not been created yet, so we'll
                        // look for it once a web page has been loaded.
                        webBrowser.LoadCompleted += new LoadCompletedEventHandler(WebBrowserLoadCompleted);
                    }
                }
                else
                {
                    // When no longer suppressing the WM_ERASEBKGND message,
                    // dispose our window hook.
                    IEWindowHook hook = (IEWindowHook)webBrowser.GetValue(SuppressEraseBackgroundWindowHookProperty);
                    if (hook != null)
                    {
                        hook.Dispose();
                    }
                    webBrowser.ClearValue(SuppressEraseBackgroundWindowHookProperty);
                }
            }
        }

        private static bool TryHookWebBrowser(WebBrowser webBrowser)
        {
            if (GetSuppressEraseBackground(webBrowser))
            {
                // Try to find the IE window several layers within the WebBrowser.
                IntPtr hwndIEWindow = GetIEWindow(webBrowser);
                if (hwndIEWindow != IntPtr.Zero)
                {
                    // Hook the window messages so we can intercept the
                    // WM_ERASEBKGND message.
                    IEWindowHook hook = new IEWindowHook(new HWND(hwndIEWindow));

                    // Keep our hook alive.
                    webBrowser.SetValue(SuppressEraseBackgroundWindowHookProperty, hook);

                    return true;
                }
            }

            return false;
        }

        private static void WebBrowserLoadCompleted(object sender, NavigationEventArgs e)
        {
            // We only need to do this the first time. 
            WebBrowser webBrowser = (WebBrowser)sender;
            webBrowser.LoadCompleted -= new LoadCompletedEventHandler(WebBrowserLoadCompleted);

            if (GetSuppressEraseBackground(webBrowser) && !TryHookWebBrowser(webBrowser))
            {
                throw new InvalidOperationException("Unable to hook the WebBrowser.");
            }
        }

        private static IntPtr GetIEWindow(WebBrowser webBrowser)
        {
            HWND hwndIeWindow = HWND.NULL;

            HWND hwndShellEmbeddingWindow = new HWND(webBrowser.Handle);
            if (hwndShellEmbeddingWindow != HWND.NULL)
            {
                HWND hwndShellDocObjectView = NativeMethods.GetWindow(hwndShellEmbeddingWindow, GW.CHILD);
                if (hwndShellDocObjectView != HWND.NULL)
                {
                    hwndIeWindow = NativeMethods.GetWindow(hwndShellDocObjectView, GW.CHILD);
                }
            }

            return hwndIeWindow.DangerousGetHandle();
        }

        private class IEWindowHook : WindowSubclass
        {
            public IEWindowHook(HWND hwnd)
                : base(hwnd)
            {
            }

            protected override IntPtr WndProcOverride(HWND hwnd, WM msg, IntPtr wParam, IntPtr lParam, IntPtr id, IntPtr data)
            {
                // IE doesn't seem to really need to erase its background, since
                // it will paint the entire window with the web page in WM_PAINT.
                // However, it causes flickering on some systems, so we just
                // ignore the message.
                if (msg == WM.ERASEBKGND)
                {
                    return new IntPtr(1);
                }

                return base.WndProcOverride(hwnd, msg, wParam, lParam, id, data);
            }
        }
    }
}
