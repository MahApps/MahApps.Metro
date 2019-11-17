using ControlzEx.Native;
using System;
using System.ComponentModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

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
                                        new PropertyMetadata(null));

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
                                        new PropertyMetadata(null));

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
                                        new PropertyMetadata(null));

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
                                        new PropertyMetadata(null));

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
                                        new PropertyMetadata(null));

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
                                        new PropertyMetadata(null));
        
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
                                        new PropertyMetadata(Theme.Light));

        /// <summary>
        /// Gets or sets the value indicating current theme.
        /// </summary>
        public Theme Theme
        {
            get { return (Theme)GetValue(ThemeProperty); }
            set { SetValue(ThemeProperty, value); }
        }

        public static readonly DependencyProperty MinimizeProperty =
            DependencyProperty.Register("Minimize", typeof(string), typeof(WindowButtonCommands),
                                        new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the minimize button tooltip.
        /// </summary>
        public string Minimize
        {
            get { return (string)GetValue(MinimizeProperty); }
            set { SetValue(MinimizeProperty, value); }
        }

        public static readonly DependencyProperty MaximizeProperty =
            DependencyProperty.Register("Maximize", typeof(string), typeof(WindowButtonCommands),
                                        new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the maximize button tooltip.
        /// </summary>
        public string Maximize
        {
            get { return (string)GetValue(MaximizeProperty); }
            set { SetValue(MaximizeProperty, value); }
        }

        public static readonly DependencyProperty CloseProperty =
            DependencyProperty.Register("Close", typeof(string), typeof(WindowButtonCommands),
                                        new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the close button tooltip.
        /// </summary>
        public string Close
        {
            get { return (string)GetValue(CloseProperty); }
            set { SetValue(CloseProperty, value); }
        }

        public static readonly DependencyProperty RestoreProperty =
            DependencyProperty.Register("Restore", typeof(string), typeof(WindowButtonCommands),
                                        new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the restore button tooltip.
        /// </summary>
        public string Restore
        {
            get { return (string)GetValue(RestoreProperty); }
            set { SetValue(RestoreProperty, value); }
        }

#pragma warning disable 618
        private SafeLibraryHandle user32;
#pragma warning restore 618

        static WindowButtonCommands()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(WindowButtonCommands), new FrameworkPropertyMetadata(typeof(WindowButtonCommands)));
        }

        public WindowButtonCommands()
        {
            CommandBindings.Add(new CommandBinding(SystemCommands.MinimizeWindowCommand, MinimizeWindow));
            CommandBindings.Add(new CommandBinding(SystemCommands.MaximizeWindowCommand, MaximizeWindow));
            CommandBindings.Add(new CommandBinding(SystemCommands.RestoreWindowCommand, RestoreWindow));
            CommandBindings.Add(new CommandBinding(SystemCommands.CloseWindowCommand, CloseWindow));

            this.Dispatcher.BeginInvoke(DispatcherPriority.Loaded,
                                        new Action(() => {
                                                       if (string.IsNullOrWhiteSpace(this.Minimize))
                                                       {
                                                           this.SetCurrentValue(MinimizeProperty, GetCaption(900));
                                                       }
                                                       if (string.IsNullOrWhiteSpace(this.Maximize))
                                                       {
                                                           this.SetCurrentValue(MaximizeProperty, GetCaption(901));
                                                       }
                                                       if (string.IsNullOrWhiteSpace(this.Close))
                                                       {
                                                           this.SetCurrentValue(CloseProperty, GetCaption(905));
                                                       }
                                                       if (string.IsNullOrWhiteSpace(this.Restore))
                                                       {
                                                           this.SetCurrentValue(RestoreProperty, GetCaption(903));
                                                       }
                                                   }));
        }

        private void MinimizeWindow(object sender, ExecutedRoutedEventArgs e)
        {
            if (ParentWindow != null)
            {
                SystemCommands.MinimizeWindow(ParentWindow);
            }
        }

        private void MaximizeWindow(object sender, ExecutedRoutedEventArgs e)
        {
            if (ParentWindow != null)
            {
                SystemCommands.MaximizeWindow(ParentWindow);
            }
        }

        private void RestoreWindow(object sender, ExecutedRoutedEventArgs e)
        {
            if (ParentWindow != null)
            {
                SystemCommands.RestoreWindow(ParentWindow);
            }
        }

        private void CloseWindow(object sender, ExecutedRoutedEventArgs e)
        {
            if (this.ParentWindow != null)
            {
                var args = new ClosingWindowEventHandlerArgs();
                this.ClosingWindow?.Invoke(this, args);

                if (args.Cancelled)
                {
                    return;
                }

                SystemCommands.CloseWindow(this.ParentWindow);
            }
        }

#pragma warning disable 618
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
#pragma warning restore 618

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
