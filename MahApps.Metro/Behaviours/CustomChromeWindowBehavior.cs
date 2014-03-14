using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interactivity;
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

            var windowChrome = new WindowChrome();
            windowChrome.ResizeBorderThickness = new Thickness(6);
            windowChrome.CaptionHeight = 30;
            windowChrome.CornerRadius = new CornerRadius(0);
            windowChrome.GlassFrameThickness = new Thickness(0);

            AssociatedObject.SetValue(WindowChrome.WindowChromeProperty, windowChrome);
        }
    }
}
