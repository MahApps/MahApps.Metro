using System;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;

namespace MahApps.Metro.Native
{
    using System.Windows.Interop;

    /// <summary>
    /// This class is for methods that are potentially dangerous. Any caller of these methods
    /// must perform a full security review to make sure that the usage is secure because no stack walk will be performed.
    /// </summary>
    /// <devdoc>http://msdn.microsoft.com/en-us/library/ms182161.aspx</devdoc>
    [SuppressUnmanagedCodeSecurity]
    internal static class UnsafeNativeMethods
    {
        /// <summary>
        /// Obtains a value that indicates whether Desktop Window Manager (DWM) composition is enabled. Applications can listen for composition state changes by handling the WM_DWMCOMPOSITIONCHANGED notification.
        /// </summary>
        /// <returns>
        /// A value indicating whether Desktop Window Manager is composition enabled. 
        /// </returns>
        /// <devdoc>http://msdn.microsoft.com/en-us/library/windows/desktop/aa969518%28v=vs.85%29.aspx</devdoc>
        [DllImport("dwmapi", PreserveSig = false, CallingConvention = CallingConvention.Winapi)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool DwmIsCompositionEnabled();

        /// <summary>
        /// Extends the window frame into the client area.
        /// </summary>
        /// <param name="hWnd">
        /// The handle to the window in which the frame will be extended into the client area.
        /// </param>
        /// <param name="pMarInset">
        /// A pointer to a <see cref="MARGINS"/> structure that describes the margins to use when extending the frame into the client area.
        /// </param>
        /// <returns>
        /// If this function succeeds, it returns S_OK. Otherwise, it returns an HRESULT error code.
        /// </returns>
        /// <remarks>Should <paramref name="hWnd"/> be replaced with a <see cref="SafeHandle"/>?</remarks>
        /// <devdoc>http://msdn.microsoft.com/en-us/library/windows/desktop/aa969512%28v=vs.85%29.aspx</devdoc>
        [DllImport("dwmapi", PreserveSig = true, CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
        [return: MarshalAs(UnmanagedType.Error)]
        internal static extern int DwmExtendFrameIntoClientArea(IntPtr hWnd, [In] ref MARGINS pMarInset);

        /// <summary>
        /// Sets the value of non-client rendering attributes for a window.
        /// </summary>
        /// <param name="hwnd">
        /// The handle to the window that will receive the attributes.
        /// </param>
        /// <param name="attr">
        /// A single <a href="http://msdn.microsoft.com/en-us/library/windows/desktop/aa969530%28v=vs.85%29.aspx">DWMWINDOWATTRIBUTE</a> flag to apply to the window. This parameter specifies the attribute and the pvAttribute parameter points to the value of that attribute.
        /// </param>
        /// <param name="attrValue">
        /// A pointer to the value of the attribute specified in the dwAttribute parameter. Different <a href="http://msdn.microsoft.com/en-us/library/windows/desktop/aa969530%28v=vs.85%29.aspx">DWMWINDOWATTRIBUTE</a> flags require different value types. 
        /// </param>
        /// <param name="attrSize">
        /// The size, in bytes, of the value type pointed to by the <paramref name="attrValue"/> parameter.
        /// </param>
        /// <returns>
        /// If this function succeeds, it returns S_OK. Otherwise, it returns an HRESULT error code.
        /// </returns>
        /// <devdoc>http://msdn.microsoft.com/en-us/library/windows/desktop/aa969524%28v=vs.85%29.aspx</devdoc>
        [DllImport("dwmapi", PreserveSig = true, CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
        internal static extern int DwmSetWindowAttribute(
            [In] IntPtr hwnd, [In] int attr, [In] ref int attrValue, [In] int attrSize);

        /// <summary>
        /// Calls the default window procedure to provide default processing for any window messages that an application does not process. This function ensures that every message is processed. DefWindowProc is called with the same parameters received by the window procedure.
        /// </summary>
        /// <param name="hwnd">
        /// A handle to the window procedure that received the message.
        /// </param>
        /// <param name="msg">
        /// The message.
        /// </param>
        /// <param name="wParam">
        /// Additional message information. The content of this parameter depends on the value of the <paramref name="msg"/> parameter. 
        /// </param>
        /// <param name="lParam">
        /// Additional message information. The content of this parameter depends on the value of the <paramref name="msg"/> parameter.
        /// </param>
        /// <returns>
        /// The return value is the result of the message processing and depends on the message. 
        /// </returns>
        /// <devdoc>Documentation source:<a href="http://msdn.microsoft.com/en-us/library/windows/desktop/ms633572%28v=vs.85%29.aspx">http://msdn.microsoft.com/en-us/library/windows/desktop/ms633572%28v=vs.85%29.aspx</a>
        /// <para>
        /// Please note that <see cref="UIntPtr"/> acts VERY differently than <see cref="IntPtr"/> this is VERY important as IntPtr is
        /// sign extended and UIntPtr is not. This causes very different behavior on WOW64 systems when the pointer/Handle is extended at the
        /// marshaling layer. <paramref name="wParam"/> is <a href="http://msdn.microsoft.com/en-us/library/aa383751%28v=VS.85%29.aspx">defined</a> to be <see cref="UIntPtr"/> however <see cref="HwndSource.AddHook"/> passes
        /// an <see cref="IntPtr"/>.
        /// </para>
        /// <para>Also note, this is a Winapi calling convention function, calling it with another calling convention may cause stack
        /// corruption and thus program failure</para></devdoc>
        [DllImport("user32", CallingConvention = CallingConvention.Winapi)]
        internal static extern IntPtr DefWindowProc(
            [In] IntPtr hwnd, [In] int msg, [In] IntPtr wParam, [In] IntPtr lParam);

        /// <summary>
        /// The GetMonitorInfo function retrieves information about a display monitor.
        /// </summary>
        /// <param name="hMonitor">
        /// A handle to the display monitor of interest.
        /// </param>
        /// <param name="lpmi">
        /// <para>A pointer to a <see cref="MONITORINFO"/> or <a href="http://msdn.microsoft.com/en-us/library/dd145066%28v=vs.85%29.aspx">MONITORINFOEX</a> structure that receives information about the specified display monitor.</para>
        /// <para>You must set the cbSize member of the structure to sizeof(MONITORINFO) or sizeof(MONITORINFOEX) before calling the GetMonitorInfo function. Doing so lets the function determine the type of structure you are passing to it.</para>
        /// <para>The MONITORINFOEX structure is a superset of the MONITORINFO structure. It has one additional member: a string that contains a name for the display monitor. Most applications have no use for a display monitor name, and so can save some bytes by using a MONITORINFO structure.</para>
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the function succeeded; <see langword="false"/> otherwise
        /// <para>
        /// If the function succeeds, the return value is nonzero.</para>
        /// <para>If the function fails, the return value is zero.</para>
        /// </returns>
        /// <devdoc>http://msdn.microsoft.com/en-us/library/dd144901%28v=VS.85%29.aspx</devdoc>
        [DllImport("user32", EntryPoint = "GetMonitorInfoW", ExactSpelling = true, CharSet = CharSet.Unicode)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool GetMonitorInfo([In] IntPtr hMonitor, [Out] MONITORINFO lpmi);

        /// <summary>
        /// The MonitorFromWindow function retrieves a handle to the display monitor that has the largest area of intersection with the bounding rectangle of a specified window.
        /// </summary>
        /// <param name="handle">
        /// A handle to the window of interest.
        /// </param>
        /// <param name="flags">
        /// <para>Determines the function's return value if the window does not intersect any display monitor.</para>
        /// See <a href="http://msdn.microsoft.com/en-us/library/dd145064%28v=VS.85%29.aspx">MonitorFromWindow function</a> documentation for
        /// more information.
        /// </param>
        /// <returns>
        /// <para>If the window intersects one or more display monitor rectangles, the return value is an HMONITOR
        /// handle to the display monitor that has the largest area of intersection with the window.</para>
        /// <para>If the window does not intersect a display monitor, the return value depends on the value of dwFlags.</para>
        /// </returns>
        /// <remarks>If the window is currently minimized, MonitorFromWindow uses the rectangle of the window before it was minimized.</remarks>
        /// <devdoc>http://msdn.microsoft.com/en-us/library/dd145064%28v=VS.85%29.aspx</devdoc>
        [DllImport("user32")]
        internal static extern IntPtr MonitorFromWindow([In] IntPtr handle, [In] int flags);

        /// <summary>
        /// Loads a string resource from the executable file associated with a specified module,
        /// copies the string into a buffer, and appends a terminating null character.
        /// </summary>
        /// <param name="hInstance">
        /// A handle to an instance of the module whose executable file contains the string resource.
        /// To get the handle to the application itself, call the GetModuleHandle function with NULL.
        /// </param>
        /// <param name="uID">
        /// The identifier of the string to be loaded.
        /// </param>
        /// <param name="lpBuffer">
        /// The buffer is to receive the string. This buffer MUST be preallocated
        /// </param>
        /// <param name="nBufferMax">
        /// The size of the buffer, in characters. The string is truncated and null-terminated if it
        /// is longer than the number of characters specified. If this parameter is 0, then lpBuffer
        /// receives a read-only pointer to the resource itself.
        /// </param>
        /// <returns>
        /// If the function succeeds, the return value is the number of characters copied into the
        /// buffer, not including the terminating null character, or zero if the string resource
        /// does not exist. To get extended error information, call <see cref="Marshal.GetLastWin32Error"/>
        /// </returns>
        /// <devdoc>http://msdn.microsoft.com/en-us/library/windows/desktop/ms647486%28v=vs.85%29.aspx
        /// <para>In this application we will use <see cref="LoadLibrary"/> to get the module handle, this prevents a race
        /// condition as described by <a href="http://msdn.microsoft.com/en-us/library/windows/desktop/ms683199%28v=vs.85%29.aspx">GetModuleHandle</a> documentation</para>
        /// </devdoc>
        [DllImport("user32", CharSet = CharSet.Unicode, ExactSpelling = true, EntryPoint = "LoadStringW", SetLastError = true,
            CallingConvention = CallingConvention.Winapi)]
        internal static extern int LoadString(
            [In] [Optional] IntPtr hInstance, [In] uint uID, [Out] StringBuilder lpBuffer, [In] int nBufferMax);

        /// <summary>
        /// Loads the specified module into the address space of the calling process. The specified module may cause other modules to be loaded.
        /// </summary>
        /// <param name="lpFileName">
        /// <para>The name of the module. This can be either a library module (a .dll file) or an executable module (an .exe file). The name
        /// specified is the file name of the module and is not related to the name stored in the library module itself, as specified by the
        /// LIBRARY keyword in the module-definition (.def) file.</para>
        /// <para>If the string specifies a full path, the function searches only that path for the module.</para>
        /// <para>If the string specifies a relative path or a module name without a path, the function uses a standard search strategy to
        /// find the module; for more information, see the
        /// <a href="http://msdn.microsoft.com/en-us/library/windows/desktop/ms684175%28v=vs.85%29.aspx">Remarks</a>.</para>
        /// <para>If the function cannot find the module, the function fails. When specifying a path, be sure to use backslashes
        /// (\), not forward slashes (/). For more information about paths, see Naming a
        /// <a href="http://msdn.microsoft.com/en-us/library/windows/desktop/aa365247(v=vs.85).aspx">File or Directory.</a></para>
        /// <para>If the string specifies a module name without a path and the file name extension is omitted, the function appends
        /// the default library extension .dll to the module name. To prevent the function from appending .dll to the module name,
        /// include a trailing point character (.) in the module name string.</para>
        /// </param>
        /// <returns>
        /// <para>If the function succeeds, the return value is a handle to the module.</para>
        /// <para>If the function fails, the return value is <see cref="IntPtr.Zero"/>. To get extended error information, call <see cref="Marshal.GetLastWin32Error"/>.</para>
        /// </returns>
        /// <devdoc>http://msdn.microsoft.com/en-us/library/windows/desktop/ms684175%28v=vs.85%29.aspx</devdoc>
        [DllImport("kernel32", CharSet = CharSet.Unicode, ExactSpelling = true, EntryPoint = "LoadLibraryW", SetLastError = true,
            CallingConvention = CallingConvention.Winapi)]
        internal static extern IntPtr LoadLibrary([In] [MarshalAs(UnmanagedType.LPWStr)] string lpFileName);

        /// <summary>
        /// Frees the loaded dynamic-link library (DLL) module and, if necessary, decrements its reference count. When the reference count reaches zero,
        /// the module is unloaded from the address space of the calling process and the handle is no longer valid.
        /// </summary>
        /// <param name="hModule">
        /// A handle to the loaded library module. The <see cref="LoadLibrary"/>,
        /// LoadLibraryEx, GetModuleHandle, or GetModuleHandleEx function returns this handle.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the function succeeds; otherwise <see langword="false"/>
        /// </returns>
        /// <devdoc>http://msdn.microsoft.com/en-us/library/windows/desktop/ms683152%28v=vs.85%29.aspx</devdoc>
        [DllImport("kernel32", CallingConvention = CallingConvention.Winapi)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool FreeLibrary([In] IntPtr hModule);
    }
}