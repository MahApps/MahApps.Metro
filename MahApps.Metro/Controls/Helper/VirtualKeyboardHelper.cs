using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System;
using System.Linq;
using System.Management;

namespace MahApps.Metro.Controls.Helper
{
    public class VirtualKeyboardHelper
    {
        private const uint SC_CLOSE = 61536;
        private const uint WM_SYSCOMMAND = 274;
        private static DateTime lastCloseRequest;

        public static readonly DependencyProperty EnableVirtualKeyboardProperty = DependencyProperty.RegisterAttached(
            "EnableVirtualKeyboard",
            typeof(bool),
            typeof(VirtualKeyboardHelper),
            new PropertyMetadata(default(bool), OnEnableVirtualKeyboardChanged));

        private static void OnEnableVirtualKeyboardChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            UIElement tb = (UIElement)d;
            if (tb != null)
            {
                tb.GotFocus += OnGotFocus;
                tb.LostFocus += OnLostFocus;
            }
        }

        public static bool GetEnableVirtualKeyboard(DependencyObject element)
        {
            return (bool)element.GetValue(EnableVirtualKeyboardProperty);
        }

        public static void SetEnableVirtualKeyboard(DependencyObject element, bool value)
        {
            element.SetValue(EnableVirtualKeyboardProperty, value);
        }

        private static void CloseVirtualKeyboard()
        {
            IntPtr keyboardWnd = FindWindow("IPTip_Main_Window", null);
            PostMessage(keyboardWnd.ToInt32(), WM_SYSCOMMAND, (int)SC_CLOSE, 0);
        }

        [DllImport("user32.dll")]
        private static extern IntPtr FindWindow(string sClassName, string sAppName);

        private static bool IsSurfaceKeyboardAttached()
        {
            using (var objOsDetails = new ManagementObjectSearcher("Win32_Keyboard"))
            {
                using (var osDetailsCollection = objOsDetails.Get())
                {
                    var amountExternalKeyboards = osDetailsCollection.Cast<ManagementObject>()
                                                                     .Select(mo => (string)mo.GetPropertyValue("PNPDeviceID"))
                                                                     .Count(i => i.StartsWith("USB"));

                    return amountExternalKeyboards > 0;
                }
            }
        }

        private static void OnGotFocus(object sender, RoutedEventArgs e)
        {
            lastCloseRequest = DateTime.MaxValue;
            if (GetEnableVirtualKeyboard((DependencyObject)sender) && !IsSurfaceKeyboardAttached())
            {
                OpenVirtualKeyboard();
            }
        }

        private static void OnLostFocus(object sender, RoutedEventArgs e)
        {
            lastCloseRequest = DateTime.Now;
            Task.Factory
                .StartNew(() => { Thread.Sleep(10); })
                .ContinueWith(t =>
                                  {
                                      if (DateTime.Now - lastCloseRequest > TimeSpan.Zero)
                                      {
                                          CloseVirtualKeyboard();
                                      }
                                  });
        }

        private static void OpenVirtualKeyboard()
        {
            ProcessStartInfo startInfo;
            if (Environment.Is64BitOperatingSystem)
            {
                startInfo = new ProcessStartInfo(@"C:\Program Files\Common Files\Microsoft Shared\ink\TabTip.exe");
            }
            else
            {
                startInfo = new ProcessStartInfo(@"C:\Program Files (x86)\Common Files\Microsoft Shared\ink\TabTip32.exe");
            }
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            Process.Start(startInfo);
        }

        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool PostMessage(int hWnd, uint msg, int wParam, int lParam);
    }
}