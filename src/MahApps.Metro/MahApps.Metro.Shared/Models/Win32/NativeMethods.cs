using System;
using System.Runtime.InteropServices;

namespace MahApps.Metro.Models.Win32
{
	static class NativeMethods
	{
		public static WS GetWindowLong(this IntPtr hWnd)
		{
			return (WS)NativeMethods.GetWindowLong(hWnd, (int)GWL.STYLE);
		}
		public static WSEX GetWindowLongEx(this IntPtr hWnd)
		{
			return (WSEX)NativeMethods.GetWindowLong(hWnd, (int)GWL.EXSTYLE);
		}

		[DllImport("user32.dll", EntryPoint = "GetWindowLongA", SetLastError = true)]
		public static extern int GetWindowLong(IntPtr hWnd, int nIndex);

		public static WS SetWindowLong(this IntPtr hWnd, WS dwNewLong)
		{
			return (WS)NativeMethods.SetWindowLong(hWnd, (int)GWL.STYLE, (int)dwNewLong);
		}
		public static WSEX SetWindowLongEx(this IntPtr hWnd, WSEX dwNewLong)
		{
			return (WSEX)NativeMethods.SetWindowLong(hWnd, (int)GWL.EXSTYLE, (int)dwNewLong);
		}
		
		[DllImport("user32.dll")]
		public static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

		[DllImport("user32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int x, int y, int cx, int cy, SWP flags);

		[DllImport("user32.dll", SetLastError = true)]
		public static extern bool PostMessage(IntPtr hwnd, uint Msg, IntPtr wParam, IntPtr lParam);
	}
}
