// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace MahApps.Metro.Controls
{
    internal static class EyeDropperHelper
    {
        [DllImport("gdi32.dll")]
        private static extern bool BitBlt(IntPtr hdcDest, int nxDest, int nyDest, int nWidth, int nHeight, IntPtr hdcSrc, int nXSrc, int nYSrc, int dwRop);

        [DllImport("gdi32.dll")]
        private static extern IntPtr CreateCompatibleBitmap(IntPtr hdc, int width, int nHeight);

        [DllImport("gdi32.dll")]
        private static extern IntPtr CreateCompatibleDC(IntPtr hdc);

        [DllImport("gdi32.dll")]
        private static extern IntPtr DeleteDC(IntPtr hdc);

        [DllImport("gdi32.dll")]
        private static extern IntPtr DeleteObject(IntPtr hObject);

        [DllImport("user32.dll")]
        private static extern IntPtr GetDesktopWindow();

        [DllImport("user32.dll")]
        private static extern IntPtr GetWindowDC(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern bool ReleaseDC(IntPtr hWnd, IntPtr hDc);

        [DllImport("user32.dll")]
        private static extern IntPtr GetDC(IntPtr hwnd);

        [DllImport("gdi32.dll")]
        private static extern uint GetPixel(IntPtr hdc, int nXPos, int nYPos);

        [DllImport("gdi32.dll")]
        private static extern IntPtr SelectObject(IntPtr hdc, IntPtr hObject);

        private const int SRCCOPY = 0x00CC0020;
        private const int CAPTUREBLT = 0x40000000;

        public static BitmapSource CaptureRegion(Int32Rect region)
        {
            BitmapSource result;

            IntPtr desktophWnd = GetDesktopWindow();
            IntPtr desktopDc = GetWindowDC(desktophWnd);
            IntPtr memoryDc = CreateCompatibleDC(desktopDc);
            IntPtr bitmap = CreateCompatibleBitmap(desktopDc, region.Width, region.Height);
            IntPtr oldBitmap = SelectObject(memoryDc, bitmap);

            bool success = BitBlt(memoryDc, 0, 0, region.Width, region.Height, desktopDc, region.X, region.Y, SRCCOPY | CAPTUREBLT);

            try
            {
                if (!success)
                {
                    throw new Win32Exception();
                }

                result = Imaging.CreateBitmapSourceFromHBitmap(bitmap, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            }
            finally
            {
                SelectObject(memoryDc, oldBitmap);
                DeleteObject(bitmap);
                DeleteDC(memoryDc);
                ReleaseDC(desktophWnd, desktopDc);
            }

            result.Freeze();
            return result;
        }

        public static Color GetPixelColor(Point point)
        {
            IntPtr hdc = GetDC(IntPtr.Zero);
            uint pixel = GetPixel(hdc, (int)Math.Round(point.X), (int)Math.Round(point.Y));
            ReleaseDC(IntPtr.Zero, hdc);
            Color color = Color.FromRgb((byte)(pixel & 0x000000FF),
                                        (byte)((pixel & 0x0000FF00) >> 8),
                                        (byte)((pixel & 0x00FF0000) >> 16));
            return color;
        }
    }
}