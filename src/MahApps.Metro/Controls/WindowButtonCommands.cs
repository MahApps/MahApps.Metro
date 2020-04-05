using ControlzEx.Native;
using ControlzEx.Theming;
using System;
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
    [StyleTypedProperty(Property = nameof(LightMinButtonStyle), StyleTargetType = typeof(Button))]
    [StyleTypedProperty(Property = nameof(LightMaxButtonStyle), StyleTargetType = typeof(Button))]
    [StyleTypedProperty(Property = nameof(LightCloseButtonStyle), StyleTargetType = typeof(Button))]
    [StyleTypedProperty(Property = nameof(DarkMinButtonStyle), StyleTargetType = typeof(Button))]
    [StyleTypedProperty(Property = nameof(DarkMaxButtonStyle), StyleTargetType = typeof(Button))]
    [StyleTypedProperty(Property = nameof(DarkCloseButtonStyle), StyleTargetType = typeof(Button))]
    public class WindowButtonCommands : ContentControl
    {
        public event ClosingWindowEventHandler ClosingWindow;

        public delegate void ClosingWindowEventHandler(object sender, ClosingWindowEventHandlerArgs args);

        /// <summary>Identifies the <see cref="LightMinButtonStyle"/> dependency property.</summary>
        public static readonly DependencyProperty LightMinButtonStyleProperty
            = DependencyProperty.Register(nameof(LightMinButtonStyle),
                                          typeof(Style),
                                          typeof(WindowButtonCommands),
                                          new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the value indicating current light style for the minimize button.
        /// </summary>
        public Style LightMinButtonStyle
        {
            get => (Style)this.GetValue(LightMinButtonStyleProperty);
            set => this.SetValue(LightMinButtonStyleProperty, value);
        }

        /// <summary>Identifies the <see cref="LightMaxButtonStyle"/> dependency property.</summary>
        public static readonly DependencyProperty LightMaxButtonStyleProperty
            = DependencyProperty.Register(nameof(LightMaxButtonStyle),
                                          typeof(Style),
                                          typeof(WindowButtonCommands),
                                          new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the value indicating current light style for the maximize button.
        /// </summary>
        public Style LightMaxButtonStyle
        {
            get => (Style)this.GetValue(LightMaxButtonStyleProperty);
            set => this.SetValue(LightMaxButtonStyleProperty, value);
        }

        /// <summary>Identifies the <see cref="LightCloseButtonStyle"/> dependency property.</summary>
        public static readonly DependencyProperty LightCloseButtonStyleProperty
            = DependencyProperty.Register(nameof(LightCloseButtonStyle),
                                          typeof(Style),
                                          typeof(WindowButtonCommands),
                                          new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the value indicating current light style for the close button.
        /// </summary>
        public Style LightCloseButtonStyle
        {
            get => (Style)this.GetValue(LightCloseButtonStyleProperty);
            set => this.SetValue(LightCloseButtonStyleProperty, value);
        }

        /// <summary>Identifies the <see cref="DarkMinButtonStyle"/> dependency property.</summary>
        public static readonly DependencyProperty DarkMinButtonStyleProperty
            = DependencyProperty.Register(nameof(DarkMinButtonStyle),
                                          typeof(Style),
                                          typeof(WindowButtonCommands),
                                          new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the value indicating current dark style for the minimize button.
        /// </summary>
        public Style DarkMinButtonStyle
        {
            get => (Style)this.GetValue(DarkMinButtonStyleProperty);
            set => this.SetValue(DarkMinButtonStyleProperty, value);
        }

        /// <summary>Identifies the <see cref="DarkMaxButtonStyle"/> dependency property.</summary>
        public static readonly DependencyProperty DarkMaxButtonStyleProperty
            = DependencyProperty.Register(nameof(DarkMaxButtonStyle),
                                          typeof(Style),
                                          typeof(WindowButtonCommands),
                                          new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the value indicating current dark style for the maximize button.
        /// </summary>
        public Style DarkMaxButtonStyle
        {
            get => (Style)this.GetValue(DarkMaxButtonStyleProperty);
            set => this.SetValue(DarkMaxButtonStyleProperty, value);
        }

        /// <summary>Identifies the <see cref="DarkCloseButtonStyle"/> dependency property.</summary>
        public static readonly DependencyProperty DarkCloseButtonStyleProperty
            = DependencyProperty.Register(nameof(DarkCloseButtonStyle),
                                          typeof(Style),
                                          typeof(WindowButtonCommands),
                                          new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the value indicating current dark style for the close button.
        /// </summary>
        public Style DarkCloseButtonStyle
        {
            get => (Style)this.GetValue(DarkCloseButtonStyleProperty);
            set => this.SetValue(DarkCloseButtonStyleProperty, value);
        }

        /// <summary>Identifies the <see cref="Theme"/> dependency property.</summary>
        public static readonly DependencyProperty ThemeProperty
            = DependencyProperty.Register(nameof(Theme),
                                          typeof(string),
                                          typeof(WindowButtonCommands),
                                          new PropertyMetadata(ThemeManager.BaseColorLight));

        /// <summary>
        /// Gets or sets the value indicating current theme.
        /// </summary>
        public string Theme
        {
            get => (string)this.GetValue(ThemeProperty);
            set => this.SetValue(ThemeProperty, value);
        }

        /// <summary>Identifies the <see cref="Minimize"/> dependency property.</summary>
        public static readonly DependencyProperty MinimizeProperty
            = DependencyProperty.Register(nameof(Minimize),
                                          typeof(string),
                                          typeof(WindowButtonCommands),
                                          new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the minimize button tooltip.
        /// </summary>
        public string Minimize
        {
            get => (string)this.GetValue(MinimizeProperty);
            set => this.SetValue(MinimizeProperty, value);
        }

        /// <summary>Identifies the <see cref="Maximize"/> dependency property.</summary>
        public static readonly DependencyProperty MaximizeProperty
            = DependencyProperty.Register(nameof(Maximize),
                                          typeof(string),
                                          typeof(WindowButtonCommands),
                                          new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the maximize button tooltip.
        /// </summary>
        public string Maximize
        {
            get => (string)this.GetValue(MaximizeProperty);
            set => this.SetValue(MaximizeProperty, value);
        }

        /// <summary>Identifies the <see cref="Close"/> dependency property.</summary>
        public static readonly DependencyProperty CloseProperty
            = DependencyProperty.Register(nameof(Close),
                                          typeof(string),
                                          typeof(WindowButtonCommands),
                                          new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the close button tooltip.
        /// </summary>
        public string Close
        {
            get => (string)this.GetValue(CloseProperty);
            set => this.SetValue(CloseProperty, value);
        }

        /// <summary>Identifies the <see cref="Restore"/> dependency property.</summary>
        public static readonly DependencyProperty RestoreProperty
            = DependencyProperty.Register(nameof(Restore),
                                          typeof(string),
                                          typeof(WindowButtonCommands),
                                          new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the restore button tooltip.
        /// </summary>
        public string Restore
        {
            get => (string)this.GetValue(RestoreProperty);
            set => this.SetValue(RestoreProperty, value);
        }

        /// <summary>Identifies the <see cref="ParentWindow"/> dependency property.</summary>
        public static readonly DependencyPropertyKey ParentWindowPropertyKey =
            DependencyProperty.RegisterReadOnly(nameof(ParentWindow),
                                                typeof(Window),
                                                typeof(WindowButtonCommands),
                                                new PropertyMetadata(null));

        /// <summary>Identifies the <see cref="ParentWindow"/> dependency property.</summary>
        public static readonly DependencyProperty ParentWindowProperty = ParentWindowPropertyKey.DependencyProperty;

        /// <summary>
        /// Gets the window.
        /// </summary>
        public Window ParentWindow
        {
            get => (Window)this.GetValue(ParentWindowProperty);
            protected set => this.SetValue(ParentWindowPropertyKey, value);
        }

        static WindowButtonCommands()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(WindowButtonCommands), new FrameworkPropertyMetadata(typeof(WindowButtonCommands)));
        }

        public WindowButtonCommands()
        {
            this.CommandBindings.Add(new CommandBinding(SystemCommands.MinimizeWindowCommand, this.MinimizeWindow));
            this.CommandBindings.Add(new CommandBinding(SystemCommands.MaximizeWindowCommand, this.MaximizeWindow));
            this.CommandBindings.Add(new CommandBinding(SystemCommands.RestoreWindowCommand, this.RestoreWindow));
            this.CommandBindings.Add(new CommandBinding(SystemCommands.CloseWindowCommand, this.CloseWindow));

            this.BeginInvoke(() =>
                                 {
                                     if (null == this.ParentWindow)
                                     {
                                         var window = this.TryFindParent<Window>();
                                         this.SetValue(ParentWindowPropertyKey, window);
                                     }

                                     if (string.IsNullOrWhiteSpace(this.Minimize))
                                     {
                                         this.SetCurrentValue(MinimizeProperty, this.GetCaption(900));
                                     }

                                     if (string.IsNullOrWhiteSpace(this.Maximize))
                                     {
                                         this.SetCurrentValue(MaximizeProperty, this.GetCaption(901));
                                     }

                                     if (string.IsNullOrWhiteSpace(this.Close))
                                     {
                                         this.SetCurrentValue(CloseProperty, this.GetCaption(905));
                                     }

                                     if (string.IsNullOrWhiteSpace(this.Restore))
                                     {
                                         this.SetCurrentValue(RestoreProperty, this.GetCaption(903));
                                     }
                                 },
                             DispatcherPriority.Loaded);
        }

        private void MinimizeWindow(object sender, ExecutedRoutedEventArgs e)
        {
            if (this.ParentWindow != null)
            {
                SystemCommands.MinimizeWindow(this.ParentWindow);
            }
        }

        private void MaximizeWindow(object sender, ExecutedRoutedEventArgs e)
        {
            if (this.ParentWindow != null)
            {
                SystemCommands.MaximizeWindow(this.ParentWindow);
            }
        }

        private void RestoreWindow(object sender, ExecutedRoutedEventArgs e)
        {
            if (this.ParentWindow != null)
            {
                SystemCommands.RestoreWindow(this.ParentWindow);
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
            if (this.user32 == null)
            {
                this.user32 = UnsafeNativeMethods.LoadLibrary(Environment.SystemDirectory + "\\User32.dll");
            }

            var sb = new StringBuilder(256);
            UnsafeNativeMethods.LoadString(this.user32, (uint)id, sb, sb.Capacity);
            return sb.ToString().Replace("&", "");
        }

        private SafeLibraryHandle user32;
#pragma warning restore 618
    }
}