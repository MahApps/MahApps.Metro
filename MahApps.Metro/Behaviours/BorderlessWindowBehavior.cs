using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;
using System.Windows.Interop;
using System.Windows.Media;
using MahApps.Metro.Controls;
using MahApps.Metro.Native;

namespace MahApps.Metro.Behaviours
{
    public class BorderlessWindowBehavior : Behavior<Window>
    {
        private HwndSource m_hwndSource;
        private IntPtr m_hwnd;

        public static DependencyProperty ResizeWithGripProperty = DependencyProperty.Register("ResizeWithGrip", typeof(bool), typeof(BorderlessWindowBehavior), new PropertyMetadata(true));
        public static DependencyProperty AutoSizeToContentProperty = DependencyProperty.Register("AutoSizeToContent", typeof(bool), typeof(BorderlessWindowBehavior), new PropertyMetadata(false));

        private static void WmGetMinMaxInfo(IntPtr hwnd, IntPtr lParam)
        {

            var mmi = (MINMAXINFO)Marshal.PtrToStructure(lParam, typeof(MINMAXINFO));

            // Adjust the maximized size and position to fit the work area of the correct monitor

            System.IntPtr monitor = UnsafeNativeMethods.MonitorFromWindow(hwnd, Constants.MONITOR_DEFAULTTONEAREST);

            if (monitor != System.IntPtr.Zero)
            {

                MONITORINFO monitorInfo = new MONITORINFO();
                UnsafeNativeMethods.GetMonitorInfo(monitor, monitorInfo);
                RECT rcWorkArea = monitorInfo.rcWork;
                RECT rcMonitorArea = monitorInfo.rcMonitor;
                mmi.ptMaxPosition.x = Math.Abs(rcWorkArea.left - rcMonitorArea.left);
                mmi.ptMaxPosition.y = Math.Abs(rcWorkArea.top - rcMonitorArea.top);
                mmi.ptMaxSize.x = Math.Abs(rcWorkArea.right - rcWorkArea.left);
                mmi.ptMaxSize.y = Math.Abs(rcWorkArea.bottom - rcWorkArea.top);
            }

            Marshal.StructureToPtr(mmi, lParam, true);
        }

        public bool ResizeWithGrip
        {
            get { return (bool)GetValue(ResizeWithGripProperty); }
            set { SetValue(ResizeWithGripProperty, value); }
        }

        public bool AutoSizeToContent
        {
            get { return (bool)GetValue(AutoSizeToContentProperty); }
            set { SetValue(AutoSizeToContentProperty, value); }
        }

        public Border Border { get; set; }

        protected override void OnAttached()
        {
            if (PresentationSource.FromVisual(AssociatedObject) != null)
                AddHwndHook();
            else
                AssociatedObject.SourceInitialized += AssociatedObject_SourceInitialized;

            AssociatedObject.WindowStyle = WindowStyle.None;

            if (AssociatedObject is MetroWindow)
            {
                var window = ((MetroWindow) AssociatedObject);
                //MetroWindow already has a border we can use
                AssociatedObject.Loaded += (s, e) =>
                                               {
                                                   var ancestors = window.GetPart<Border>("PART_Border");
                                                   Border = ancestors;
                                                   if (Environment.OSVersion.Version.Major < 6 || !UnsafeNativeMethods.DwmIsCompositionEnabled()) 
                                                       Border.BorderThickness = new Thickness(1);
                                               };

                if (AssociatedObject.ResizeMode == ResizeMode.NoResize)
                {
                    window.ShowMaxRestoreButton = false;
                    window.ShowMinButton = false;
                    ResizeWithGrip = false;
                }
                else if (AssociatedObject.ResizeMode == ResizeMode.CanMinimize)
                {
                    window.ShowMaxRestoreButton = false;
                    ResizeWithGrip = false;
                }
            }
            else { 
                //Other windows may not, easiest to just inject one!
                var content = (UIElement) AssociatedObject.Content;
                AssociatedObject.Content = null;

                Border = new Border
                            {
                                Child =  content,
                                BorderBrush = new SolidColorBrush(Colors.Black)
                            };
                
                AssociatedObject.Content = Border;
            }

            if (ResizeWithGrip)
                AssociatedObject.ResizeMode = ResizeMode.CanResizeWithGrip;
            
            if (AutoSizeToContent)
                AssociatedObject.Loaded += (s, e) =>
                                               {
                                                   //Temp fix, thanks @lynnx
                                                   AssociatedObject.SizeToContent = SizeToContent.Height;
                                                   AssociatedObject.SizeToContent = AutoSizeToContent
                                                                                        ? SizeToContent.WidthAndHeight
                                                                                        : SizeToContent.Manual;
                                               };



            base.OnAttached();
        }

        protected override void OnDetaching()
        {
            RemoveHwndHook();
            base.OnDetaching();
        }

        private void AddHwndHook()
        {
            m_hwndSource = PresentationSource.FromVisual(AssociatedObject) as HwndSource;
            if (m_hwndSource != null) m_hwndSource.AddHook(HwndHook);
            m_hwnd = new WindowInteropHelper(AssociatedObject).Handle;
        }

        private void RemoveHwndHook()
        {
            AssociatedObject.SourceInitialized -= AssociatedObject_SourceInitialized;
            m_hwndSource.RemoveHook(HwndHook);
        }

        private void AssociatedObject_SourceInitialized(object sender, EventArgs e)
        {
            AddHwndHook();
        }

        private IntPtr HwndHook(IntPtr hWnd, int message, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            IntPtr returnval = IntPtr.Zero;
            switch (message)
            {
                case Constants.WM_CREATE:
                    handled = true;
                    var create = (CREATESTRUCT)Marshal.PtrToStructure(lParam, typeof(CREATESTRUCT));
                    if (create.style == Constants.WS_MAXIMIZE)
                    {
                        MessageBox.Show("Got here");
                    }
                    break;

                case Constants.WM_NCCALCSIZE:
                    /* Hides the border */
                    handled = true;
                    break;
                case Constants.WM_NCPAINT:
                    {
                        if (Environment.OSVersion.Version.Major >= 6 && UnsafeNativeMethods.DwmIsCompositionEnabled())
                        {
                            var val = 2;
                            UnsafeNativeMethods.DwmSetWindowAttribute(m_hwnd, 2, ref val, 4);
                            var m = new MARGINS { bottomHeight = 1, leftWidth = 1, rightWidth = 1, topHeight = 1 };
                            UnsafeNativeMethods.DwmExtendFrameIntoClientArea(m_hwnd, ref m);
                            if (Border != null)
                                Border.BorderThickness = new Thickness(0);
                        }
                        else
                        {
                            if (Border != null)
                                Border.BorderThickness = new Thickness(1);
                        }
                        handled = true;
                    }
                    break;
                case Constants.WM_NCACTIVATE:
                    {
                        /* As per http://msdn.microsoft.com/en-us/library/ms632633(VS.85).aspx , "-1" lParam
                         * "does not repaint the nonclient area to reflect the state change." */
                        returnval = UnsafeNativeMethods.DefWindowProc(hWnd, message, wParam, new IntPtr(-1));
                        handled = true;
                    }
                    break;
                case Constants.WM_GETMINMAXINFO:
                    /* http://blogs.msdn.com/b/llobo/archive/2006/08/01/maximizing-window-_2800_with-windowstyle_3d00_none_2900_-considering-taskbar.aspx */
                    WmGetMinMaxInfo(hWnd, lParam);

                    /* Setting handled to false enables the application to process it's own Min/Max requirements,
                     * as mentioned by jason.bullard (comment from September 22, 2011) on http://gallery.expression.microsoft.com/ZuneWindowBehavior/ */
                    handled = false;
                    break;
            }


            return returnval;
        }
    }
}
