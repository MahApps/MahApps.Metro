namespace MahApps.Metro.Controls
{
    using System.Windows;

    using MahApps.Metro.Behaviours;

    [TemplatePart(Name = PART_TitleBar, Type = typeof(UIElement))]
    [TemplatePart(Name = PART_WindowCommands, Type = typeof(WindowCommands))]
    public class MetroWindow : Window
    {
        private const string PART_TitleBar = "PART_TitleBar";
        private const string PART_WindowCommands = "PART_WindowCommands";

        public static readonly DependencyProperty ShowIconOnTitleBarProperty =
            DependencyProperty.Register(
                "ShowIconOnTitleBar", typeof(bool), typeof(MetroWindow), new PropertyMetadata(true));

        public static readonly DependencyProperty ShowTitleBarProperty = DependencyProperty.Register(
            "ShowTitleBar", typeof(bool), typeof(MetroWindow), new PropertyMetadata(true));

        private readonly DragWindowBehavior _dragWindowBehavior = new DragWindowBehavior();
        private UIElement _titleBar;

        static MetroWindow()
        {
            DefaultStyleKeyProperty.OverrideMetadata(
                typeof(MetroWindow), new FrameworkPropertyMetadata(typeof(MetroWindow)));
        }

        public bool ShowIconOnTitleBar
        {
            get { return (bool)GetValue(ShowIconOnTitleBarProperty); }
            set { SetValue(ShowIconOnTitleBarProperty, value); }
        }

        public bool ShowTitleBar
        {
            get { return (bool)GetValue(ShowTitleBarProperty); }
            set { SetValue(ShowTitleBarProperty, value); }
        }

        private UIElement TitleBar
        {
            get { return _titleBar; }
            set
            {
                if (_titleBar != null)
                    _titleBar.RemoveBehavior(_dragWindowBehavior);
                _titleBar = value;
                if (_titleBar != null)
                    _titleBar.AddBehavior(_dragWindowBehavior);
            }
        }

        private WindowCommands WindowCommands { get; set; }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            TitleBar = GetTemplateChild(PART_TitleBar) as UIElement;
            WindowCommands = GetTemplateChild(PART_WindowCommands) as WindowCommands;
        }
    }
}