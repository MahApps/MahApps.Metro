using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;
using MahApps.Metro.Controls;
#if NET_4
using Microsoft.Windows.Shell;
#else
using System.Windows.Shell;
#endif

namespace MahApps.Metro.Behaviours
{
    public class CustomChromeWindowBehavior : Behavior<Window>
    {
        protected override void OnAttached()
        {
            base.OnAttached();

            var window = AssociatedObject as MetroWindow;
            var captionHeight = window != null ? window.TitlebarHeight : 30;

            var windowChrome = new WindowChrome();
            windowChrome.ResizeBorderThickness = new Thickness(6);
            windowChrome.CaptionHeight = captionHeight;
            windowChrome.CornerRadius = new CornerRadius(0);
            windowChrome.GlassFrameThickness = new Thickness(0);

            AssociatedObject.SetValue(WindowChrome.WindowChromeProperty, windowChrome);

            AssociatedObject.Loaded += AssociatedObject_Loaded;
        }

        private void AssociatedObject_Loaded(object sender, RoutedEventArgs e)
        {
            var window = sender as MetroWindow;
            if (window == null)
            {
                return;
            }

            var icon = window.GetPart<UIElement>("PART_Icon");
            if (icon != null)
            {
                icon.SetValue(WindowChrome.IsHitTestVisibleInChromeProperty, true);
            }
            
            var titleBar = window.GetPart<UIElement>("PART_TitleBar");
            if (titleBar != null)
            {
                titleBar.SetValue(WindowChrome.IsHitTestVisibleInChromeProperty, true);
            }
            
            var leftWindowCommands = window.GetPart<ContentPresenter>("PART_LeftWindowCommands");
            if (leftWindowCommands != null)
            {
                leftWindowCommands.SetValue(WindowChrome.IsHitTestVisibleInChromeProperty, true);
            }
            
            var windowCommands = window.GetPart<ContentPresenter>("PART_RightWindowCommands");
            if (windowCommands != null)
            {
                windowCommands.SetValue(WindowChrome.IsHitTestVisibleInChromeProperty, true);
            }
            
            var windowButtonCommands = window.GetPart<ContentControl>("PART_WindowButtonCommands");
            if (windowButtonCommands != null)
            {
                windowButtonCommands.SetValue(WindowChrome.IsHitTestVisibleInChromeProperty, true);
            }
        }
    }
}
