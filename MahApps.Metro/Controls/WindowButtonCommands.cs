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
        private static string _minimize;
        private static string _maximize;
        private static string _closeText;
        private static string _restore;
        private Button _min;
        private Button _max;
        private Button _close;
        private SafeLibraryHandle _user32;

        public string Minimize
        {
            get
            {
                if (string.IsNullOrEmpty(_minimize))
                {
                    _minimize = GetCaption(900);
                }
                return _minimize;
            }
        }

        public string Maximize
        {
            get
            {
                if (string.IsNullOrEmpty(_maximize))
                {
                    _maximize = GetCaption(901);
                }
                return _maximize;
            }
        }

        public string Close
        {
            get
            {
                if (string.IsNullOrEmpty(_closeText))
                {
                    _closeText = GetCaption(905);
                }
                return _closeText;
            }
        }

        public string Restore
        {
            get
            {
                if (string.IsNullOrEmpty(_restore))
                {
                    _restore = GetCaption(903);
                }
                return _restore;
            }
        }

        public static readonly DependencyProperty LightMinButtonStyleProperty =
            DependencyProperty.Register("LightMinButtonStyle", typeof(Style), typeof(WindowButtonCommands),
                                        new PropertyMetadata(null, OnThemeChanged));

        /// <summary>
        /// Gets or sets the value indicating current LightMinButton style.
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
        /// Gets or sets the value indicating current LightMaxButton style.
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
        /// Gets or sets the value indicating current LightCloseButton style.
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
        /// Gets or sets the value indicating current DarkMinButton style.
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
        /// Gets or sets the value indicating current DarkMaxButton style.
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
        /// Gets or sets the value indicating current DarkCloseButton style.
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

        static WindowButtonCommands()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(WindowButtonCommands), new FrameworkPropertyMetadata(typeof(WindowButtonCommands)));
        }

        public WindowButtonCommands()
        {
            Loaded += WindowButtonCommands_Loaded;
        }

        private string GetCaption(int id)
        {
            if (_user32 == null)
            {
                _user32 = UnsafeNativeMethods.LoadLibrary(Environment.SystemDirectory + "\\User32.dll");
            }

            var sb = new StringBuilder(256);
            UnsafeNativeMethods.LoadString(_user32, (uint)id, sb, sb.Capacity);
            return sb.ToString().Replace("&", "");
        }

        // todo: change back to private once Window(Close/Max/Min)ButtonStyle are removed from MetroWindow
        public void ApplyTheme()
        {
            if (_close != null)
            {
                // todo: delete this if statement once WindowCloseButtonStyle is removed from MetroWindow
                if ((ParentWindow != null) && (ParentWindow.WindowCloseButtonStyle != null))
                {
                    _close.Style = ParentWindow.WindowCloseButtonStyle;
                }
                else
                {
                    _close.Style = Theme == Theme.Light ? LightCloseButtonStyle : DarkCloseButtonStyle;
                }
            }
            if (_max != null)
            {
                // todo: delete this if statement once WindowMaxButtonStyle is removed from MetroWindow
                if ((ParentWindow != null) && (ParentWindow.WindowMaxButtonStyle != null))
                {
                    _max.Style = ParentWindow.WindowMaxButtonStyle;
                }
                else
                {
                    _max.Style = Theme == Theme.Light ? LightMaxButtonStyle : DarkMaxButtonStyle;
                }
            }
            if (_min != null)
            {
                // todo: delete this if statement once WindowMinButtonStyle is removed from MetroWindow
                if ((ParentWindow != null) && (ParentWindow.WindowMinButtonStyle != null))
                {
                    _min.Style = ParentWindow.WindowMinButtonStyle;
                }
                else
                {
                    _min.Style = Theme == Theme.Light ? LightMinButtonStyle : DarkMinButtonStyle;
                }
            }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _close = Template.FindName("PART_Close", this) as Button;
            if (_close != null)
            {
                _close.Click += CloseClick;
            }

            _max = Template.FindName("PART_Max", this) as Button;
            if (_max != null)
            {
                _max.Click += MaximizeClick;
            }

            _min = Template.FindName("PART_Min", this) as Button;
            if (_min != null)
            {
                _min.Click += MinimizeClick;
            }

            ApplyTheme();
        }

        public event ClosingWindowEventHandler ClosingWindow;

        public delegate void ClosingWindowEventHandler(object sender, ClosingWindowEventHandlerArgs args);

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
            if (ParentWindow == null)
            {
                return;
            }
            Microsoft.Windows.Shell.SystemCommands.MinimizeWindow(ParentWindow);
        }

        private void MaximizeClick(object sender, RoutedEventArgs e)
        {
            if (ParentWindow == null)
            {
                return;
            }
            if (ParentWindow.WindowState == WindowState.Maximized)
            {
                Microsoft.Windows.Shell.SystemCommands.RestoreWindow(ParentWindow);
            }
            else
            {
                Microsoft.Windows.Shell.SystemCommands.MaximizeWindow(ParentWindow);
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

            if (ParentWindow != null)
            {
                ParentWindow.Close();
            }
        }

        private void WindowButtonCommands_Loaded(object sender, RoutedEventArgs e)
        {
            Loaded -= WindowButtonCommands_Loaded;
            if (ParentWindow == null)
            {
                ParentWindow = this.TryFindParent<MetroWindow>();
            }
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
                RaisePropertyChanged("ParentWindow");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void RaisePropertyChanged(string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}