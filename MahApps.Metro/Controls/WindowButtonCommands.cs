﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using MahApps.Metro.Native;

namespace MahApps.Metro.Controls
{
    [TemplatePart(Name = "PART_Max", Type = typeof(Button))]
    [TemplatePart(Name = "PART_Close", Type = typeof(Button))]
    [TemplatePart(Name = "PART_Min", Type = typeof(Button))]
    public class WindowButtonCommands : ContentControl
    {
        public event ClosingWindowEventHandler ClosingWindow;
        public delegate void ClosingWindowEventHandler(object sender, ClosingWindowEventHandlerArgs args);

        public string Minimize
        {
            get
            {
                if (string.IsNullOrEmpty(minimize))
                    minimize = GetCaption(900);
                return minimize;
            }
        }

        public string Maximize
        {
            get
            {
                if (string.IsNullOrEmpty(maximize))
                    maximize = GetCaption(901);
                return maximize;
            }
        }

        public string Close
        {
            get
            {
                if (string.IsNullOrEmpty(closeText))
                    closeText = GetCaption(905);
                return closeText;
            }
        }

        public string Restore
        {
            get
            {
                if (string.IsNullOrEmpty(restore))
                    restore = GetCaption(903);
                return restore;
            }
        }

        private static string minimize;
        private static string maximize;
        private static string closeText;
        private static string restore;
        private Button min;
        private Button max;
        private Button close;
        private IntPtr user32 = IntPtr.Zero;

        static WindowButtonCommands()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(WindowButtonCommands), new FrameworkPropertyMetadata(typeof(WindowButtonCommands)));
        }

        ~WindowButtonCommands()
        {
            if (user32 != IntPtr.Zero)
                UnsafeNativeMethods.FreeLibrary(user32);
        }

        private string GetCaption(int id)
        {
            if (user32 == IntPtr.Zero)
                user32 = UnsafeNativeMethods.LoadLibrary(Environment.SystemDirectory + "\\User32.dll");

            var sb = new StringBuilder(256);
            UnsafeNativeMethods.LoadString(user32, (uint)id, sb, sb.Capacity);
            return sb.ToString().Replace("&", "");
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            close = Template.FindName("PART_Close", this) as Button;
            if (close != null)
                close.Click += CloseClick;

            max = Template.FindName("PART_Max", this) as Button;
            if (max != null)
                max.Click += MaximizeClick;

            min = Template.FindName("PART_Min", this) as Button;
            if (min != null)
                min.Click += MinimizeClick;
        }

        protected void OnClosingWindow(ClosingWindowEventHandlerArgs args)
        {
            var handler = ClosingWindow;
            if (handler != null)
                handler(this, args);
        }

        private void MinimizeClick(object sender, RoutedEventArgs e)
        {
            var parentWindow = GetParentWindow();
            if (parentWindow != null)
                Microsoft.Windows.Shell.SystemCommands.MinimizeWindow(parentWindow);
        }

        private void MaximizeClick(object sender, RoutedEventArgs e)
        {
            var parentWindow = GetParentWindow();
            if (parentWindow == null)
                return;

            if (parentWindow.WindowState == WindowState.Maximized)
            {
                Microsoft.Windows.Shell.SystemCommands.RestoreWindow(parentWindow);
            }
            else
            {
                Microsoft.Windows.Shell.SystemCommands.MaximizeWindow(parentWindow);
            }
        }

        private void CloseClick(object sender, RoutedEventArgs e)
        {
            var closingWindowEventHandlerArgs = new ClosingWindowEventHandlerArgs();
            OnClosingWindow(closingWindowEventHandlerArgs);

            if (closingWindowEventHandlerArgs.Cancelled)
                return;

            var parentWindow = GetParentWindow();
            if (parentWindow != null)
            {
                parentWindow.Close();
            }
        }

        private Window GetParentWindow()
        {
            var parent = VisualTreeHelper.GetParent(this);

            while (parent != null && !(parent is Window))
            {
                parent = VisualTreeHelper.GetParent(parent);
            }

            var parentWindow = parent as Window;
            return parentWindow;
        }
    }
}
