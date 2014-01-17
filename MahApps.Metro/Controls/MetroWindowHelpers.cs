using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace MahApps.Metro.Controls
{
    /// <summary>
    /// This class eats little children.
    /// </summary>
    internal static class MetroWindowHelpers
    {
        public static void ResetAllWindowCommandsBrush(this MetroWindow window)
        {
            window.InvokeCommandButtons(x => x.ClearValue(Control.ForegroundProperty));

            window.WindowButtonCommands.ClearValue(Control.ForegroundProperty);
        }

        public static void ChangeWindowCommandButtonsBrush(this MetroWindow window, string resourceName)
        {
            window.InvokeCommandButtons(x => x.SetResourceReference(Control.ForegroundProperty, resourceName));
        }

        public static void UpdateWindowCommandsForFlyout(this MetroWindow window, Flyout flyout)
        {
            Brush brush = null;

            if (flyout.Theme == FlyoutTheme.Accent)
            {
                brush = (Brush)flyout.FindResource("IdealForegroundColorBrush");
            }

            else if (flyout.ActualTheme == Theme.Light)
            {
                brush = (Brush)ThemeManager.LightResource["BlackBrush"];
            }

            else if (flyout.ActualTheme == Theme.Dark)
            {
                brush = (Brush)ThemeManager.DarkResource["BlackBrush"];
            }

             window.ChangeAllWindowCommandsBrush(brush);

        }

        private static void ChangeWindowCommandButtonsBrush(this MetroWindow window, Brush brush)
        {
            window.InvokeCommandButtons(x => x.SetValue(Control.ForegroundProperty, brush));
        }

        private static void InvokeCommandButtons(this MetroWindow window, Action<Button> action)
        {
            foreach (Button b in ((WindowCommands)window.WindowCommandsPresenter.Content).FindChildren<Button>())
            {
                action(b);
            }
        }

        private static void ChangeAllWindowCommandsBrush(this MetroWindow window, Brush brush)
        {
            window.ChangeWindowCommandButtonsBrush(brush);

            window.WindowButtonCommands.SetValue(Control.ForegroundProperty, brush);
        }
    }
}
