using System;
using System.ComponentModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using MahApps.Metro.Native;

namespace MahApps.Metro.Controls
{
    [TemplatePart(Name = "PART_Min", Type = typeof(Button))]
    [TemplatePart(Name = "PART_Max", Type = typeof(Button))]
    [TemplatePart(Name = "PART_Close", Type = typeof(Button))]
    public class WindowButtonCommands : ContentControl, INotifyPropertyChanged
    {
        public event ClosingWindowEventHandler ClosingWindow;
        public delegate void ClosingWindowEventHandler(object sender, ClosingWindowEventHandlerArgs args);

        public static readonly DependencyProperty LightMinButtonStyleProperty =
            DependencyProperty.Register("LightMinButtonStyle", typeof(Style), typeof(WindowButtonCommands),
                                        new PropertyMetadata(null, OnThemeChanged));

        /// <summary>
        /// Gets or sets the value indicating current light style for the minimize button.
        /// </summary>
        public Style LightMinButtonStyle
        {
            get { return (Style)GetValue(LightMinButtonStyleProperty); }
            set { SetValue(LightMinButtonStyleProperty, value); }
        }

        public static readonly DependencyProperty LightMaxButtonStyleProperty =
            DependencyProperty.Register("LightMaxButtonStyle", typeof(Style), typeof(WindowButtonCommands),
                                        new PropertyMetadata(null, OnThemeChanged));

        /// <summary>
        /// Gets or sets the value indicating current light style for the maximize button.
        /// </summary>
        public Style LightMaxButtonStyle
        {
            get { return (Style)GetValue(LightMaxButtonStyleProperty); }
            set { SetValue(LightMaxButtonStyleProperty, value); }
        }

        public static readonly DependencyProperty LightCloseButtonStyleProperty =
            DependencyProperty.Register("LightCloseButtonStyle", typeof(Style), typeof(WindowButtonCommands),
                                        new PropertyMetadata(null, OnThemeChanged));

        /// <summary>
        /// Gets or sets the value indicating current light style for the close button.
        /// </summary>
        public Style LightCloseButtonStyle
        {
            get { return (Style)GetValue(LightCloseButtonStyleProperty); }
            set { SetValue(LightCloseButtonStyleProperty, value); }
        }

        public static readonly DependencyProperty DarkMinButtonStyleProperty =
            DependencyProperty.Register("DarkMinButtonStyle", typeof(Style), typeof(WindowButtonCommands),
                                        new PropertyMetadata(null, OnThemeChanged));

        /// <summary>
        /// Gets or sets the value indicating current dark style for the minimize button.
        /// </summary>
        public Style DarkMinButtonStyle
        {
            get { return (Style)GetValue(DarkMinButtonStyleProperty); }
            set { SetValue(DarkMinButtonStyleProperty, value); }
        }

        public static readonly DependencyProperty DarkMaxButtonStyleProperty =
            DependencyProperty.Register("DarkMaxButtonStyle", typeof(Style), typeof(WindowButtonCommands),
                                        new PropertyMetadata(null, OnThemeChanged));

        /// <summary>
        /// Gets or sets the value indicating current dark style for the maximize button.
        /// </summary>
        public Style DarkMaxButtonStyle
        {
            get { return (Style)GetValue(DarkMaxButtonStyleProperty); }
            set { SetValue(DarkMaxButtonStyleProperty, value); }
        }

        public static readonly DependencyProperty DarkCloseButtonStyleProperty =
            DependencyProperty.Register("DarkCloseButtonStyle", typeof(Style), typeof(WindowButtonCommands),
                                        new PropertyMetadata(null, OnThemeChanged));
        
        /// <summary>
        /// Gets or sets the value indicating current dark style for the close button.
        /// </summary>
        public Style DarkCloseButtonStyle
        {
            get { return (Style)GetValue(DarkCloseButtonStyleProperty); }
            set { SetValue(DarkCloseButtonStyleProperty, value); }
        }

        public static readonly DependencyProperty ThemeProperty =
            DependencyProperty.Register("Theme", typeof(Theme), typeof(WindowButtonCommands),
                                        new PropertyMetadata(Theme.Light, OnThemeChanged));

        /// <summary>
        /// Gets or sets the value indicating current theme.
        /// </summary>
        public Theme Theme
        {
            get { return (Theme)GetValue(ThemeProperty); }
            set { SetValue(ThemeProperty, value); }
        }

        private static void OnThemeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue == e.OldValue)
            {
                return;
            }

            ((WindowButtonCommands)d).ApplyTheme();
        }

        public string Minimize
        {
            get
            {
                if (string.IsNullOrEmpty(minimize))
                {
                    minimize = GetCaption(900);
                }
                return minimize;
            }
        }

        public string Maximize
        {
            get
            {
                if (string.IsNullOrEmpty(maximize))
                {
                    maximize = GetCaption(901);
                }
                return maximize;
            }
        }

        public string Close
        {
            get
            {
                if (string.IsNullOrEmpty(closeText))
                {
                    closeText = GetCaption(905);
                }
                return closeText;
            }
        }

        public string Restore
        {
            get
            {
                if (string.IsNullOrEmpty(restore))
                {
                    restore = GetCaption(903);
                }
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
        private SafeLibraryHandle user32;

        static WindowButtonCommands()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(WindowButtonCommands), new FrameworkPropertyMetadata(typeof(WindowButtonCommands)));
        }

        public WindowButtonCommands()
        {
            this.Loaded += WindowButtonCommands_Loaded;
        }

        private void WindowButtonCommands_Loaded(object sender, RoutedEventArgs e)
        {
            this.Loaded -= WindowButtonCommands_Loaded;
            var parentWindow = this.ParentWindow;
            if (null == parentWindow)
            {
                this.ParentWindow = this.TryFindParent<MetroWindow>();
            }
        }

        private string GetCaption(int id)
        {
            if (user32 == null)
            {
                user32 = UnsafeNativeMethods.LoadLibrary(Environment.SystemDirectory + "\\User32.dll");
            }

            var sb = new StringBuilder(256);
            UnsafeNativeMethods.LoadString(user32, (uint)id, sb, sb.Capacity);
            return sb.ToString().Replace("&", "");
        }

        // TODO: Change back to private once Window(Min/Max/Close)ButtonStyle properties are deleted from MetroWindow!
        public void ApplyTheme()
        {
            if (close != null)
            {
                // TODO: Delete this if statement once WindowCloseButtonStyle property is deleted from MetroWindow!
                if ((ParentWindow != null) && (ParentWindow.WindowCloseButtonStyle != null))
                {
                    close.Style = ParentWindow.WindowCloseButtonStyle;
                }
                else
                {
                    close.Style = (Theme == Theme.Light) ? LightCloseButtonStyle : DarkCloseButtonStyle;
                }
            }
            if (max != null)
            {
                // TODO: Delete this if statement once WindowMaxButtonStyle property is deleted from MetroWindow!
                if ((ParentWindow != null) && (ParentWindow.WindowMaxButtonStyle != null))
                {
                    max.Style = ParentWindow.WindowMaxButtonStyle;
                }
                else
                {
                    max.Style = (Theme == Theme.Light) ? LightMaxButtonStyle : DarkMaxButtonStyle;
                }
            }
            if (min != null)
            {
                // TODO: Delete this if statement once WindowMinButtonStyle property is deleted from MetroWindow!
                if ((ParentWindow != null) && (ParentWindow.WindowMinButtonStyle != null))
                {
                    min.Style = ParentWindow.WindowMinButtonStyle;
                }
                else
                {
                    min.Style = (Theme == Theme.Light) ? LightMinButtonStyle : DarkMinButtonStyle;
                }
            }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            close = Template.FindName("PART_Close", this) as Button;
            if (close != null)
            {
                close.Click += CloseClick;
            }

            max = Template.FindName("PART_Max", this) as Button;
            if (max != null)
            {
                max.Click += MaximizeClick;
            }

            min = Template.FindName("PART_Min", this) as Button;
            if (min != null)
            {
                min.Click += MinimizeClick;
            }

            ApplyTheme();
        }

        protected void OnClosingWindow(ClosingWindowEventHandlerArgs args)
        {
            var handler = ClosingWindow;
            if (handler != null)
            {
                handler(this, args);
            }
        }

        private void MinimizeClick(object sender, RoutedEventArgs e)
        {
            if (null == this.ParentWindow) return;
            Microsoft.Windows.Shell.SystemCommands.MinimizeWindow(this.ParentWindow);
        }

        private void MaximizeClick(object sender, RoutedEventArgs e)
        {
            if (null == this.ParentWindow) return;
            if (this.ParentWindow.WindowState == WindowState.Maximized)
            {
                Microsoft.Windows.Shell.SystemCommands.RestoreWindow(this.ParentWindow);
            }
            else
            {
                Microsoft.Windows.Shell.SystemCommands.MaximizeWindow(this.ParentWindow);
            }
        }

        private void CloseClick(object sender, RoutedEventArgs e)
        {
            var closingWindowEventHandlerArgs = new ClosingWindowEventHandlerArgs();
            OnClosingWindow(closingWindowEventHandlerArgs);

            if (closingWindowEventHandlerArgs.Cancelled)
            {
                return;
            }

            if (null == this.ParentWindow) return;
            this.ParentWindow.Close();
        }

        private MetroWindow _parentWindow;

        public MetroWindow ParentWindow
        {
            get { return _parentWindow; }
            set
            {
                if (Equals(_parentWindow, value))
                {
                    return;
                }
                _parentWindow = value;
                this.RaisePropertyChanged("ParentWindow");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void RaisePropertyChanged(string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
