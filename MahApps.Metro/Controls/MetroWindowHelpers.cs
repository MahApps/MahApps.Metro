using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using Microsoft.Windows.Shell;

namespace MahApps.Metro.Controls
{
    /// <summary>
    /// This class eats little children.
    /// </summary>
    internal static class MetroWindowHelpers
    {
        /// <summary>
        /// Sets the IsHitTestVisibleInChromeProperty to a MetroWindow template child
        /// </summary>
        /// <param name="window">The MetroWindow.</param>
        /// <param name="name">The name of the template child.</param>
        public static void SetIsHitTestVisibleInChromeProperty<T>(this MetroWindow window, string name) where T : DependencyObject
        {
            if (window == null)
            {
                return;
            }
            var elementPart = window.GetPart<T>(name);
            if (elementPart != null)
            {
                elementPart.SetValue(WindowChrome.IsHitTestVisibleInChromeProperty, true);
            }
        }

        /// <summary>
        /// Sets the WindowChrome ResizeGripDirection to a MetroWindow template child.
        /// </summary>
        /// <param name="window">The MetroWindow.</param>
        /// <param name="name">The name of the template child.</param>
        /// <param name="direction">The direction.</param>
        public static void SetWindowChromeResizeGripDirection(this MetroWindow window, string name, ResizeGripDirection direction)
        {
            if (window == null)
            {
                return;
            }
            var inputElement = window.GetPart(name) as IInputElement;
            if (inputElement != null)
            {
                WindowChrome.SetResizeGripDirection(inputElement, direction);
            }
        }

        /// <summary>
        /// Adapts the WindowCommands to the theme of the first opened, topmost &amp;&amp; (top || right || left) flyout
        /// </summary>
        /// <param name="window">The MetroWindow</param>
        /// <param name="flyouts">All the flyouts! Or flyouts that fall into the category described in the summary.</param>
        /// <param name="resetBrush">An optional brush to reset the window commands brush to.</param>
        public static void HandleWindowCommandsForFlyouts(this MetroWindow window, IEnumerable<Flyout> flyouts, Brush resetBrush = null)
        {
            var allOpenFlyouts = flyouts.Where(x => x.IsOpen);
            
            var anyFlyoutOpen = allOpenFlyouts.Any(x => x.Position != Position.Bottom);
            if (!anyFlyoutOpen)
            {
                if (resetBrush == null)
                {
                    window.ResetAllWindowCommandsBrush();
                }
                else
                {
                    window.ChangeAllWindowCommandsBrush(resetBrush);
                }
            }

            var topFlyout = allOpenFlyouts
                .Where(x => x.Position == Position.Top)
                .OrderByDescending(Panel.GetZIndex)
                .FirstOrDefault();
            if (topFlyout != null)
            {
                window.UpdateWindowCommandsForFlyout(topFlyout);
            }
            else {
                var leftFlyout = allOpenFlyouts
                    .Where(x => x.Position == Position.Left)
                    .OrderByDescending(Panel.GetZIndex)
                    .FirstOrDefault();
                if (leftFlyout != null)
                {
                    window.UpdateWindowCommandsForFlyout(leftFlyout);
                }
                var rightFlyout = allOpenFlyouts
                    .Where(x => x.Position == Position.Right)
                    .OrderByDescending(Panel.GetZIndex)
                    .FirstOrDefault();
                if (rightFlyout != null)
                {
                    window.UpdateWindowCommandsForFlyout(rightFlyout);
                }
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
            Brush brush = flyout.Foreground;

            window.ChangeAllWindowCommandsBrush(brush, flyout.Position);
        }

        public static void ChangeWindowCommandButtonsBrush(this MetroWindow window, string brush)
        {
            window.InvokeCommandButtons(x => x.SetResourceReference(Control.ForegroundProperty, brush));
        }

        private static void ChangeWindowCommandButtonsBrush(this MetroWindow window, Brush brush)
        {
            window.InvokeCommandButtons(x => x.SetValue(Control.ForegroundProperty, brush));
        }

        public static void ChangeWindowCommandButtonsBrush(this MetroWindow window, string brush, Position position)
        {
            window.InvokeCommandButtons(x => x.SetResourceReference(Control.ForegroundProperty, brush), position);
        }

        private static void ChangeWindowCommandButtonsBrush(this MetroWindow window, Brush brush, Position position)
        {
            window.InvokeCommandButtons(x => x.SetValue(Control.ForegroundProperty, brush), position);
        }

        private static void InvokeCommandButtons(this MetroWindow window, Action<ButtonBase> action)
        {
            if (window.RightWindowCommandsPresenter == null || window.LeftWindowCommandsPresenter == null)
            {
                return;
            }

            var allCommandButtons = ((WindowCommands)window.RightWindowCommandsPresenter.Content)
                .FindChildren<ButtonBase>()
                .Concat(((WindowCommands)window.LeftWindowCommandsPresenter.Content).FindChildren<ButtonBase>());
            foreach (var b in allCommandButtons)
            {
                action(b);
            }
        }

        private static void InvokeCommandButtons(this MetroWindow window, Action<ButtonBase> action, Position position)
        {
            if (window.RightWindowCommandsPresenter == null || window.LeftWindowCommandsPresenter == null)
            {
                return;
            }

            var allCommandButtons = Enumerable.Empty<ButtonBase>();
            if (position == Position.Right || position == Position.Top)
            {
                allCommandButtons = allCommandButtons.Concat(((WindowCommands)window.RightWindowCommandsPresenter.Content).FindChildren<ButtonBase>());
            }
            if (position == Position.Left || position == Position.Top)
            {
                allCommandButtons = allCommandButtons.Concat(((WindowCommands)window.LeftWindowCommandsPresenter.Content).FindChildren<ButtonBase>());
            }
            foreach (var b in allCommandButtons)
            {
                action(b);
            }
        }

        private static void ChangeAllWindowCommandsBrush(this MetroWindow window, Brush brush)
        {
            window.ChangeWindowCommandButtonsBrush(brush);
            window.WindowButtonCommands.SetValue(Control.ForegroundProperty, brush);
        }

        private static void ChangeAllWindowCommandsBrush(this MetroWindow window, Brush brush, Position position)
        {
            window.ChangeWindowCommandButtonsBrush(brush, position);
            if (position == Position.Right || position == Position.Top)
            {
                window.WindowButtonCommands.SetValue(Control.ForegroundProperty, brush);
            }
        }
    }
}
