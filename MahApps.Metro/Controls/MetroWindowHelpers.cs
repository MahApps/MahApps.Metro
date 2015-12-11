using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
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
            window.ChangeAllWindowCommandsBrush(window.OverrideDefaultWindowCommandsBrush);
        }

        public static void UpdateWindowCommandsForFlyout(this MetroWindow window, Flyout flyout)
        {
            window.ChangeAllWindowCommandsBrush(flyout.Foreground, flyout.Position);
        }
        
        private static void InvokeActionOnWindowCommands(this MetroWindow window, Action<Control> action1, Action<Control> action2 = null, Position position = Position.Top)
        {
            if (window.LeftWindowCommandsPresenter == null || window.RightWindowCommandsPresenter == null || window.WindowButtonCommands == null)
            {
                return;
            }

            if (position == Position.Left || position == Position.Top)
            {
                action1(window.LeftWindowCommands);
            }

            if (position == Position.Right || position == Position.Top)
            {
                action1(window.RightWindowCommands);
                if (action2 == null)
                {
                    action1(window.WindowButtonCommands);
                }
                else
                {
                    action2(window.WindowButtonCommands);
                }
            }
        }

        private static void ChangeAllWindowCommandsBrush(this MetroWindow window, Brush brush, Position position = Position.Top)
        {
            if (brush == null)
            {
                // set the theme to light by default
                window.InvokeActionOnWindowCommands(x => x.SetValue(WindowCommands.ThemeProperty, Theme.Light),
                                                    x => x.SetValue(WindowButtonCommands.ThemeProperty, Theme.Light), position);

                // clear the foreground property
                window.InvokeActionOnWindowCommands(x => x.ClearValue(Control.ForegroundProperty), null, position);
            }
            else
            {
                // calculate brush color lightness
                var color = ((SolidColorBrush)brush).Color;

                var r = color.R / 255.0f;
                var g = color.G / 255.0f;
                var b = color.B / 255.0f;

                var max = r;
                var min = r;

                if (g > max) max = g;
                if (b > max) max = b;

                if (g < min) min = g;
                if (b < min) min = b;

                var lightness = (max + min) / 2;

                // set the theme based on color lightness
                if (lightness > 0.1)
                {
                    window.InvokeActionOnWindowCommands(x => x.SetValue(WindowCommands.ThemeProperty, Theme.Light),
                                                        x => x.SetValue(WindowButtonCommands.ThemeProperty, Theme.Light), position);
                }
                else
                {
                    window.InvokeActionOnWindowCommands(x => x.SetValue(WindowCommands.ThemeProperty, Theme.Dark),
                                                        x => x.SetValue(WindowButtonCommands.ThemeProperty, Theme.Dark), position);
                }

                // set the foreground property
                window.InvokeActionOnWindowCommands(x => x.SetValue(Control.ForegroundProperty, brush), null, position);
            }
        }
    }
}
