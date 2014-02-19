using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace MahApps.Metro.Controls
{
    /// <summary>
    /// This class eats little children.
    /// </summary>
    internal static class MetroWindowHelpers
    {
        /// <summary>
        /// Adapts the WindowCommands to the theme of the first opened, topmost && && (top || right) flyout
        /// </summary>
        /// <param name="flyouts">All the flyouts! Or flyouts that fall into the category described in the summary.</param>
        /// <param name="resetBrush">An optional brush to reset the window commands brush to.</param>
        public static void HandleWindowCommandsForFlyouts(this MetroWindow window, IEnumerable<Flyout> flyouts, Brush resetBrush = null)
        {
            var flyout = flyouts
                .Where(x => x.IsOpen && (x.Position == Position.Right || x.Position == Position.Top))
                .OrderByDescending(Panel.GetZIndex)
                .FirstOrDefault();

            if (flyout != null)
            {
                window.UpdateWindowCommandsForFlyout(flyout);
            }

            else if(resetBrush == null)
            {
                window.ResetAllWindowCommandsBrush();
            }

            else
            {
                window.ChangeAllWindowCommandsBrush(resetBrush);
            }
        }

        public static void ResetAllWindowCommandsBrush(this MetroWindow window)
        {
            if (window.OverrideDefaultWindowCommandsBrush == null)
            {
                window.InvokeCommandButtons(x => x.ClearValue(Control.ForegroundProperty));

                window.WindowButtonCommands.ClearValue(Control.ForegroundProperty);
            }

            else
            {
                window.ChangeAllWindowCommandsBrush(window.OverrideDefaultWindowCommandsBrush);
            }
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
        public static void ChangeWindowCommandButtonsBrush(this MetroWindow window, string brush)
        {
            window.InvokeCommandButtons(x => x.SetResourceReference(Control.ForegroundProperty, brush));
        }

        private static void ChangeWindowCommandButtonsBrush(this MetroWindow window, Brush brush)
        {
            window.InvokeCommandButtons(x => x.SetValue(Control.ForegroundProperty, brush));
        }

        private static void InvokeCommandButtons(this MetroWindow window, Action<ButtonBase> action)
        {
            foreach (ButtonBase b in ((WindowCommands)window.WindowCommandsPresenter.Content).FindChildren<ButtonBase>())
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
