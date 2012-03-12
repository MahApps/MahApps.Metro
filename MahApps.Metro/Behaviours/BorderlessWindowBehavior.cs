namespace MahApps.Metro.Behaviours
{
    using System;
    using System.Runtime.InteropServices;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Interactivity;
    using System.Windows.Interop;
    using System.Windows.Media;

    using MahApps.Metro.Controls;
    using MahApps.Metro.Native;

    public class BorderlessWindowBehavior : Behavior<Window>
    {
        #region Constants and Fields

        public static DependencyProperty AutoSizeToContentProperty = DependencyProperty.Register(
            "AutoSizeToContent", typeof(bool), typeof(BorderlessWindowBehavior), new PropertyMetadata(false));

        public static DependencyProperty ResizeWithGripProperty = DependencyProperty.Register(
            "ResizeWithGrip", typeof(bool), typeof(BorderlessWindowBehavior), new PropertyMetadata(true));

        private IntPtr m_hwnd;

        private HwndSource m_hwndSource;

        #endregion

        #region Public Properties

        public bool AutoSizeToContent
        {
            get
            {
                return (bool)this.GetValue(AutoSizeToContentProperty);
            }
            set
            {
                this.SetValue(AutoSizeToContentProperty, value);
            }
        }

        public Border Border { get; set; }

        public bool ResizeWithGrip
        {
            get
            {
                return (bool)this.GetValue(ResizeWithGripProperty);
            }
            set
            {
                this.SetValue(ResizeWithGripProperty, value);
            }
        }

        #endregion

        #region Public Methods and Operators

        [DllImport("user32")]
        public static extern IntPtr DefWindowProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam);

        [DllImport("dwmapi", PreserveSig = false)]
        public static extern bool DwmIsCompositionEnabled();

        #endregion

        #region Methods

        [DllImport("user32")]
        internal static extern bool GetMonitorInfo(IntPtr hMonitor, MONITORINFO lpmi);

        [DllImport("user32")]
        internal static extern IntPtr MonitorFromWindow(IntPtr handle, int flags);

        protected override void OnAttached()
        {
            if (PresentationSource.FromVisual(this.AssociatedObject) != null)
            {
                this.AddHwndHook();
            }
            else
            {
                this.AssociatedObject.SourceInitialized += this.AssociatedObject_SourceInitialized;
            }

            this.AssociatedObject.WindowStyle = WindowStyle.None;

            if (this.AssociatedObject is MetroWindow)
            {
                var window = ((MetroWindow)this.AssociatedObject);
                //MetroWindow already has a border we can use
                this.AssociatedObject.Loaded += (s, e) =>
                    {
                        var ancestors = window.GetPart<Border>("PART_Border");
                        this.Border = ancestors;
                    };

                if (this.AssociatedObject.ResizeMode == ResizeMode.NoResize)
                {
                    window.ShowMaxRestoreButton = false;
                    window.ShowMinButton = false;
                    window.MaxWidth = window.Width;
                    window.MaxHeight = window.Height;
                    this.ResizeWithGrip = false;
                }
            }
            else
            {
                //Other windows may not, easiest to just inject one!
                var content = (UIElement)this.AssociatedObject.Content;
                this.AssociatedObject.Content = null;

                this.Border = new Border { Child = content, BorderBrush = new SolidColorBrush(Colors.Black) };

                this.AssociatedObject.Content = this.Border;
            }

            this.AssociatedObject.ResizeMode = this.ResizeWithGrip ? ResizeMode.CanResizeWithGrip : ResizeMode.CanResize;

            if (this.AutoSizeToContent)
            {
                this.AssociatedObject.Loaded += (s, e) =>
                    {
                        //Temp fix, thanks @lynnx
                        this.AssociatedObject.SizeToContent = SizeToContent.Height;
                        this.AssociatedObject.SizeToContent = this.AutoSizeToContent
                                                                  ? SizeToContent.WidthAndHeight
                                                                  : SizeToContent.Manual;
                    };
            }

            base.OnAttached();
        }

        protected override void OnDetaching()
        {
            this.RemoveHwndHook();
            base.OnDetaching();
        }

        [DllImport("dwmapi")]
        private static extern int DwmExtendFrameIntoClientArea(IntPtr hWnd, ref MARGINS pMarInset);

        private static void WmGetMinMaxInfo(IntPtr hwnd, IntPtr lParam)
        {
            var mmi = (MINMAXINFO)Marshal.PtrToStructure(lParam, typeof(MINMAXINFO));

            // Adjust the maximized size and position to fit the work area of the correct monitor

            var monitor = MonitorFromWindow(hwnd, Constants.MONITOR_DEFAULTTONEAREST);

            if (monitor != IntPtr.Zero)
            {
                var monitorInfo = new MONITORINFO();
                GetMonitorInfo(monitor, monitorInfo);
                var rcWorkArea = monitorInfo.rcWork;
                var rcMonitorArea = monitorInfo.rcMonitor;
                mmi.ptMaxPosition.x = Math.Abs(rcWorkArea.left - rcMonitorArea.left);
                mmi.ptMaxPosition.y = Math.Abs(rcWorkArea.top - rcMonitorArea.top);
                mmi.ptMaxSize.x = Math.Abs(rcWorkArea.right - rcWorkArea.left);
                mmi.ptMaxSize.y = Math.Abs(rcWorkArea.bottom - rcWorkArea.top);
            }

            Marshal.StructureToPtr(mmi, lParam, true);
        }

        private void AddHwndHook()
        {
            this.m_hwndSource = PresentationSource.FromVisual(this.AssociatedObject) as HwndSource;
            this.m_hwndSource.AddHook(this.HwndHook);
            this.m_hwnd = new WindowInteropHelper(this.AssociatedObject).Handle;
        }

        private void AssociatedObject_SourceInitialized(object sender, EventArgs e)
        {
            this.AddHwndHook();
        }

        private IntPtr HwndHook(IntPtr hWnd, int message, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            var returnval = IntPtr.Zero;
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
                        if (Environment.OSVersion.Version.Major >= 6 && DwmIsCompositionEnabled())
                        {
                            var m = new MARGINS { bottomHeight = 1, leftWidth = 1, rightWidth = 1, topHeight = 1 };
                            DwmExtendFrameIntoClientArea(this.m_hwnd, ref m);
                            if (this.Border != null)
                            {
                                this.Border.BorderThickness = new Thickness(0);
                            }
                        }
                        else
                        {
                            if (this.Border != null)
                            {
                                this.Border.BorderThickness = new Thickness(1);
                            }
                        }
                        handled = true;
                    }
                    break;
                case Constants.WM_NCACTIVATE:
                    {
                        /* As per http://msdn.microsoft.com/en-us/library/ms632633(VS.85).aspx , "-1" lParam
                         * "does not repaint the nonclient area to reflect the state change." */
                        returnval = DefWindowProc(hWnd, message, wParam, new IntPtr(-1));
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

        private void RemoveHwndHook()
        {
            this.AssociatedObject.SourceInitialized -= this.AssociatedObject_SourceInitialized;
            this.m_hwndSource.RemoveHook(this.HwndHook);
        }

        #endregion
    }
}