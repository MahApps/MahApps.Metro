// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UnsafeNativeMethods.cs" company="LeetSoftwerx">
//   2012 © LeetSoftwerx
// </copyright>
// <summary>
//   Unsafe native methods
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace MahApps.Metro.Native
{
    using System;
    using System.Runtime.InteropServices;
    using System.Security;
    using System.Text;

    /// <summary>
    /// Unsafe native methods
    /// </summary>
    [SuppressUnmanagedCodeSecurity]
    internal static class UnsafeNativeMethods
    {
        /// <summary>
        /// The dwm is composition enabled.
        /// </summary>
        /// <returns>
        /// The dwm is composition enabled. 
        /// </returns>
        [DllImport("dwmapi", PreserveSig = false)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool DwmIsCompositionEnabled();

        /// <summary>
        /// The dwm extend frame into client area.
        /// </summary>
        /// <param name="hWnd">
        /// The h wnd. 
        /// </param>
        /// <param name="pMarInset">
        /// The p mar inset. 
        /// </param>
        /// <returns>
        /// The dwm extend frame into client area. 
        /// </returns>
        [DllImport("dwmapi", PreserveSig = true)]
        [return: MarshalAs(UnmanagedType.Error)]
        internal static extern int DwmExtendFrameIntoClientArea(IntPtr hWnd, [In] ref MARGINS pMarInset);

        /// <summary>
        /// The dwm set window attribute.
        /// </summary>
        /// <param name="hwnd">
        /// The hwnd. 
        /// </param>
        /// <param name="attr">
        /// The attr. 
        /// </param>
        /// <param name="attrValue">
        /// The attr value. 
        /// </param>
        /// <param name="attrSize">
        /// The attr size. 
        /// </param>
        /// <returns>
        /// The dwm set window attribute. 
        /// </returns>
        [DllImport("dwmapi", PreserveSig = true)]
        internal static extern int DwmSetWindowAttribute(
            [In] IntPtr hwnd, [In] int attr, [In] ref int attrValue, [In] int attrSize);

        /// <summary>
        /// The def window proc.
        /// </summary>
        /// <param name="hwnd">
        /// The hwnd. 
        /// </param>
        /// <param name="msg">
        /// The msg. 
        /// </param>
        /// <param name="wParam">
        /// The w param. 
        /// </param>
        /// <param name="lParam">
        /// The l param. 
        /// </param>
        /// <returns>
        /// </returns>
        [DllImport("user32", CallingConvention = CallingConvention.Winapi)]
        internal static extern IntPtr DefWindowProc(
            [In] IntPtr hwnd, [In] int msg, [In] IntPtr wParam, [In] IntPtr lParam);

        /// <summary>
        /// The get monitor info.
        /// </summary>
        /// <param name="hMonitor">
        /// The h monitor. 
        /// </param>
        /// <param name="lpmi">
        /// The lpmi. 
        /// </param>
        /// <returns>
        /// The get monitor info. 
        /// </returns>
        [DllImport("user32")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool GetMonitorInfo([In] IntPtr hMonitor, [Out] MONITORINFO lpmi);

        /// <summary>
        /// The monitor from window.
        /// </summary>
        /// <param name="handle">
        /// The handle. 
        /// </param>
        /// <param name="flags">
        /// The flags. 
        /// </param>
        /// <returns>
        /// </returns>
        [DllImport("user32")]
        internal static extern IntPtr MonitorFromWindow([In] IntPtr handle, [In] int flags);

        /// <summary>
        /// The load string.
        /// </summary>
        /// <param name="hInstance">
        /// The h instance.
        /// </param>
        /// <param name="uID">
        /// The u id.
        /// </param>
        /// <param name="lpBuffer">
        /// The lp buffer.
        /// </param>
        /// <param name="nBufferMax">
        /// The n buffer max.
        /// </param>
        /// <returns>
        /// The load string.
        /// </returns>
        [DllImport("user32", CharSet = CharSet.Unicode, ExactSpelling = true, EntryPoint = "LoadStringW", 
            CallingConvention = CallingConvention.Winapi)]
        internal static extern int LoadString(
            [In] [Optional] IntPtr hInstance, [In] uint uID, [Out] StringBuilder lpBuffer, [In] int nBufferMax);

        /// <summary>
        /// The load library.
        /// </summary>
        /// <param name="lpFileName">
        /// The lp file name.
        /// </param>
        /// <returns>
        /// </returns>
        [DllImport("kernel32", CharSet = CharSet.Unicode, ExactSpelling = true, EntryPoint = "LoadLibraryW", 
            CallingConvention = CallingConvention.Winapi)]
        internal static extern IntPtr LoadLibrary([In] string lpFileName);

        /// <summary>
        /// The free library.
        /// </summary>
        /// <param name="hModule">
        /// The h module.
        /// </param>
        /// <returns>
        /// The free library.
        /// </returns>
        [DllImport("kernel32", CallingConvention = CallingConvention.Winapi)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool FreeLibrary([In] IntPtr hModule);
    }
}