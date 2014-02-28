using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Interop;
using System.Windows;
using Microsoft.DwayneNeed.Extensions;
using System.Diagnostics;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows.Threading;
using System.ComponentModel;
using Microsoft.DwayneNeed;
using Microsoft.DwayneNeed.Win32.User32;
using Microsoft.DwayneNeed.Win32.Gdi32;
using Microsoft.DwayneNeed.Win32.Kernel32;
using Microsoft.DwayneNeed.Win32;
using System.Runtime.InteropServices;

namespace Microsoft.DwayneNeed.Interop
{
    class RedirectedWindow : WindowBase
    {
        /// <summary>
        ///     The alpha value for the window. 
        /// </summary>
        public byte Alpha
        {
            get
            {
                return _alpha;
            }

            set
            {
                if(value != _alpha)
                {
                    _alpha = value;

                    SetLayeredWindowAttributes(_alpha);
                }
            }

        }

        /// <summary>
        ///     Whether or not the window is hittestable.
        /// </summary>
        public bool IsHitTestable
        {
            get
            {
                return _isHitTestable;
            }

            set
            {
                if(value != _isHitTestable)
                {
                    _isHitTestable = value;

                    WS_EX exStyle = (WS_EX) NativeMethods.GetWindowLong(Handle, GWL.EXSTYLE);
                    if(_isHitTestable)
                    {
                        exStyle &= ~WS_EX.TRANSPARENT;
                    }
                    else
                    {
                        exStyle |= WS_EX.TRANSPARENT;
                    }
                    NativeMethods.SetWindowLong(Handle, GWL.EXSTYLE, (int) exStyle);
                }
            }
        }

        /// <summary>
        ///     Aligns the RedirectedWindow such that the specified client
        ///     coordinate is aligned with the specified screen coordinate.
        /// </summary>
        public void AlignClientAndScreen(int xClient, int yClient, int xScreen, int yScreen)
        {
            POINT pt = new POINT(xClient, yClient);
            NativeMethods.ClientToScreen(Handle, ref pt);

            int dx = xScreen - pt.x;
            int dy = yScreen - pt.y;

            RECT rcWindow = new RECT();
            NativeMethods.GetWindowRect(Handle, ref rcWindow);

            NativeMethods.SetWindowPos(
                Handle,
                HWND.NULL,
                rcWindow.left + dx,
                rcWindow.top + dy,
                0,
                0,
                SWP.NOSIZE | SWP.NOZORDER | SWP.NOACTIVATE);
        }

        /// <summary>
        ///     Sizes the window such that the client area has the specified
        ///     size.
        /// </summary>
        public bool SetClientAreaSize(int width, int height)
        {
            POINT ptClient = new POINT();
            NativeMethods.ClientToScreen(Handle, ref ptClient);

            RECT rect = new RECT();
            rect.left = ptClient.x;
            rect.top = ptClient.y;
            rect.right = rect.left + width;
            rect.bottom = rect.top + height;

            WS style = (WS) NativeMethods.GetWindowLong(Handle, GWL.STYLE);
            WS_EX exStyle = (WS_EX) NativeMethods.GetWindowLong(Handle, GWL.EXSTYLE);
            NativeMethods.AdjustWindowRectEx(ref rect, style, false, exStyle);

            NativeMethods.SetWindowPos(
                Handle,
                HWND.NULL,
                rect.left,
                rect.top,
                rect.width,
                rect.height,
                SWP.NOZORDER | SWP.NOCOPYBITS);

            NativeMethods.GetClientRect(Handle, ref rect);
            return rect.width == width && rect.height == height;
        }

        /// <summary>
        ///     Returns a bitmap of the contents of the window.
        /// </summary>
        /// <remarks>
        ///     RedirectedWindow maintains a bitmap internally and only
        ///     allocates a new bitmap if the dimensions of the window
        ///     have changed.  Even if UpdateRedirectedBitmap returns the same
        ///     bitmap, the contents are guaranteed to have been updated with
        ///     the current contents of the window.
        /// </remarks>
        public BitmapSource UpdateRedirectedBitmap()
        {
            RECT rcClient = new RECT();
            NativeMethods.GetClientRect(Handle, ref rcClient);
            if (_interopBitmap == null || rcClient.width != _bitmapWidth || rcClient.height != _bitmapHeight)
            {
                if (_interopBitmap != null)
                {
                    DestroyBitmap();
                }

                CreateBitmap(rcClient.width, rcClient.height);
            }

            // PrintWindow doesn't seem to work any better than BitBlt.
            // TODO: make it an option
            // User32.NativeMethods.PrintWindow(Handle, _hDC, PW.DEFAULT);

            IntPtr hdcSrc = NativeMethods.GetDC(Handle);
            NativeMethods.BitBlt(_hDC, 0, 0, _bitmapWidth, _bitmapHeight, hdcSrc, 0, 0, ROP.SRCCOPY);
            NativeMethods.ReleaseDC(Handle, hdcSrc);

            _interopBitmap.Invalidate();

            return _interopBitmap;
        }

        private void CreateBitmap(int width, int height)
        {
            Debug.Assert(_hSection == IntPtr.Zero);
            Debug.Assert(_hBitmap == IntPtr.Zero);
            Debug.Assert(_interopBitmap == null);

            if (width == 0 || height == 0) return;

            _stride = (width * _format.BitsPerPixel + 7) / 8;
            int size = height * _stride;

            _hSection = NativeMethods.CreateFileMapping(
                PAGE.READWRITE,
                SEC.NONE,
                0,
                (uint)size,
                null);


            BITMAPINFO bmi = new BITMAPINFO();
            bmi.biSize = Marshal.SizeOf(bmi);
            bmi.biWidth = width;
            bmi.biHeight = -height; // top-down
            bmi.biPlanes = 1;
            bmi.biBitCount = 32;
            bmi.biCompression = BI.RGB;

            IntPtr hdcScreen = NativeMethods.GetDC(HWND.NULL);
            
            _hBitmap = NativeMethods.CreateDIBSection(
                hdcScreen,
                ref bmi,
                DIB.RGB_COLORS,
                out _pBits,
                _hSection,
                0);

            // TODO: probably don't need a new DC every time...
            _hDC = NativeMethods.CreateCompatibleDC(hdcScreen);
            NativeMethods.SelectObject(_hDC, _hBitmap);

            _interopBitmap = Imaging.CreateBitmapSourceFromMemorySection(
                _hSection,
                width,
                height,
                _format,
                _stride,
                0) as InteropBitmap;

            _bitmapWidth = width;
            _bitmapHeight = height;
        }

        private void DestroyBitmap()
        {
            _interopBitmap = null;

            if (_hDC != IntPtr.Zero)
            {
                NativeMethods.DeleteObject(_hDC);
                _hDC = IntPtr.Zero;
            }

            if (_hBitmap != IntPtr.Zero)
            {
                NativeMethods.DeleteObject(_hBitmap);
                _hBitmap = IntPtr.Zero;
            }

            _pBits = IntPtr.Zero;

            if (_hSection != IntPtr.Zero)
            {
                NativeMethods.CloseHandle(_hSection);
                _hSection = IntPtr.Zero;
            }

            _stride = 0;
            _bitmapWidth = 0;
            _bitmapHeight = 0;
        }

        private static readonly PixelFormat _format = PixelFormats.Bgr32;
        private int _stride;
        private IntPtr _hSection;
        private IntPtr _hBitmap;
        private IntPtr _hDC;
        private IntPtr _pBits;
        private int _bitmapWidth;
        private int _bitmapHeight;
        private InteropBitmap _interopBitmap;

        private byte _alpha;
        private bool _isHitTestable;
    }
}
