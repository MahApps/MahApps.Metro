using System.Windows;
using System.Windows.Input;

namespace MahApps.Metro.Controls
{
    [TemplatePart(Name = PART_TitleBar, Type = typeof(UIElement))]
    public class MetroWindow : Window
    {
        private const string PART_TitleBar = "PART_TitleBar";

        public static readonly DependencyProperty ShowIconOnTitleBarProperty = DependencyProperty.Register("ShowIconOnTitleBar", typeof(bool), typeof(MetroWindow), new PropertyMetadata(true));

        static MetroWindow()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MetroWindow), new FrameworkPropertyMetadata(typeof(MetroWindow)));
        }

        public bool ShowIconOnTitleBar
        {
            get { return (bool) GetValue(ShowIconOnTitleBarProperty); }
            set { SetValue(ShowIconOnTitleBarProperty, value); }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            var titleBar = GetTemplateChild(PART_TitleBar) as UIElement;
            if (titleBar != null)
            {
                titleBar.MouseDown += TitleBarMouseDown;
            }
            else
            {
                MouseDown += TitleBarMouseDown;
            }
        }

        private void TitleBarMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.RightButton != MouseButtonState.Pressed && e.MiddleButton != MouseButtonState.Pressed && e.LeftButton == MouseButtonState.Pressed)
                DragMove();
        }
    }
}
