using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Navigation;

namespace MahApps.Metro.Controls
{
    public class MetroNavigationWindow: NavigationWindow, IMetroWindow
    {
        public static readonly DependencyProperty WindowPlacementSettingsProperty = DependencyProperty.Register("WindowPlacementSettings", typeof(IWindowPlacementSettings), typeof(MetroNavigationWindow), new PropertyMetadata(null));
        public static readonly DependencyProperty SaveWindowPositionProperty = DependencyProperty.Register("SaveWindowPosition", typeof(bool), typeof(MetroNavigationWindow), new PropertyMetadata(false));

        static MetroNavigationWindow()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MetroNavigationWindow), new FrameworkPropertyMetadata(typeof(MetroNavigationWindow)));
        }

        public IWindowPlacementSettings WindowPlacementSettings
        {
            get { return (IWindowPlacementSettings)GetValue(WindowPlacementSettingsProperty); }
            set { SetValue(WindowPlacementSettingsProperty, value); }
        }

        public bool SaveWindowPosition
        {
            get { return (bool)GetValue(SaveWindowPositionProperty); }
            set { SetValue(SaveWindowPositionProperty, value); }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (WindowCommands == null)
                WindowCommands = new WindowCommands();
        }

        public WindowCommands WindowCommands { get; set; }
    }
}
