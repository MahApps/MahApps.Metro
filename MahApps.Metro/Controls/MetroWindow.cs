using System.Windows;
using System.Windows.Input;

namespace MahApps.Metro.Controls
{
    [TemplatePart(Name = PART_TitleBar, Type = typeof(UIElement))]
    [TemplatePart(Name = PART_WindowCommands, Type = typeof(WindowCommands))]
    public class MetroWindow : Window
    {
        private const string PART_TitleBar = "PART_TitleBar";
        private const string PART_WindowCommands = "PART_WindowCommands";

        public static readonly DependencyProperty ShowIconOnTitleBarProperty = DependencyProperty.Register("ShowIconOnTitleBar", typeof(bool), typeof(MetroWindow), new PropertyMetadata(true));
        public static readonly DependencyProperty ShowTitleBarProperty = DependencyProperty.Register("ShowTitleBar", typeof(bool), typeof(MetroWindow), new PropertyMetadata(true));

        private WindowCommands windowCommands;

        static MetroWindow()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MetroWindow), new FrameworkPropertyMetadata(typeof(MetroWindow)));
        }

        public bool ShowIconOnTitleBar
        {
            get { return (bool) GetValue(ShowIconOnTitleBarProperty); }
            set { SetValue(ShowIconOnTitleBarProperty, value); }
        }

        public bool ShowTitleBar
        {
            get { return (bool) GetValue(ShowTitleBarProperty); }
            set { SetValue(ShowTitleBarProperty, value); }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            var titleBar = GetTemplateChild(PART_TitleBar) as UIElement;
            windowCommands = GetTemplateChild(PART_WindowCommands) as WindowCommands;

            if (titleBar != null)
            {
                titleBar.MouseDown += TitleBarMouseDown;
                titleBar.MouseMove += TitleBarMouseMove;
            }
            else
            {
                MouseDown += TitleBarMouseDown;
                MouseMove += TitleBarMouseMove;
            }
        }

        protected override void OnStateChanged(System.EventArgs e)
        {
            if (windowCommands != null)
            {
                windowCommands.RefreshMaximiseIconState();
            }

            base.OnStateChanged(e);
        }

        private void TitleBarMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.RightButton != MouseButtonState.Pressed && e.MiddleButton != MouseButtonState.Pressed && e.LeftButton == MouseButtonState.Pressed)
                DragMove();

            if (e.ClickCount == 2)
            {
                if (WindowState == WindowState.Maximized)
                    WindowState = WindowState.Normal;
                else
                    WindowState = WindowState.Maximized;
            }
        }

        private void TitleBarMouseMove(object sender, MouseEventArgs e)
        {
            if (e.RightButton != MouseButtonState.Pressed && e.MiddleButton != MouseButtonState.Pressed && e.LeftButton == MouseButtonState.Pressed && this.WindowState == WindowState.Maximized)
            {
                // restore window to normal state
                WindowState = WindowState.Normal;
                Top = 0;
                Left = Mouse.GetPosition(this).X - Width/2;
                DragMove();
            }
        }
    }
}
