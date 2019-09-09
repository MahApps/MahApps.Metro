using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using ControlzEx.Windows.Shell;
using JetBrains.Annotations;

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
        /// <param name="hitTestVisible"></param>
        public static void SetIsHitTestVisibleInChromeProperty<T>([NotNull] this MetroWindow window, string name, bool hitTestVisible = true)
            where T : class
        {
            if (window == null)
            {
                throw new ArgumentNullException(nameof(window));
            }

            var inputElement = window.GetPart<T>(name) as IInputElement;
            Debug.Assert(inputElement != null, $"{name} is not a IInputElement");
            if (WindowChrome.GetIsHitTestVisibleInChrome(inputElement) != hitTestVisible)
            {
                WindowChrome.SetIsHitTestVisibleInChrome(inputElement, hitTestVisible);
            }
        }

        /// <summary>
        /// Sets the WindowChrome ResizeGripDirection to a MetroWindow template child.
        /// </summary>
        /// <param name="window">The MetroWindow.</param>
        /// <param name="name">The name of the template child.</param>
        /// <param name="direction">The direction.</param>
        public static void SetWindowChromeResizeGripDirection([NotNull] this MetroWindow window, string name, ResizeGripDirection direction)
        {
            if (window == null)
            {
                throw new ArgumentNullException(nameof(window));
            }

            var inputElement = window.GetPart(name) as IInputElement;
            Debug.Assert(inputElement != null, $"{name} is not a IInputElement");
            if (WindowChrome.GetResizeGripDirection(inputElement) != direction)
            {
                WindowChrome.SetResizeGripDirection(inputElement, direction);
            }
        }

        /// <summary>
        /// Adapts the WindowCommands to the theme of the first opened, topmost &amp;&amp; (top || right || left) flyout
        /// </summary>
        /// <param name="window">The MetroWindow</param>
        /// <param name="flyouts">All the flyouts! Or flyouts that fall into the category described in the summary.</param>
        public static void HandleWindowCommandsForFlyouts(this MetroWindow window, IEnumerable<Flyout> flyouts)
        {
            var allOpenFlyouts = flyouts.Where(x => x.IsOpen);

            var anyFlyoutOpen = allOpenFlyouts.Any(x => x.Position != Position.Bottom);
            if (!anyFlyoutOpen)
            {
                window.ResetAllWindowCommandsBrush();
            }

            var topFlyout = allOpenFlyouts
                            .Where(x => x.Position == Position.Top)
                            .OrderByDescending(Panel.GetZIndex)
                            .FirstOrDefault();
            if (topFlyout != null)
            {
                window.UpdateWindowCommandsForFlyout(topFlyout);
            }
            else
            {
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
            window.ChangeAllWindowButtonCommandsBrush(window.OverrideDefaultWindowCommandsBrush);
        }

        public static void UpdateWindowCommandsForFlyout(this MetroWindow window, Flyout flyout)
        {
            window.ChangeAllWindowCommandsBrush(window.OverrideDefaultWindowCommandsBrush);
            window.ChangeAllWindowButtonCommandsBrush(window.OverrideDefaultWindowCommandsBrush ?? flyout.Foreground, flyout.Position);
        }

        private static bool NeedLightTheme(this Brush brush)
        {
            if (brush == null)
            {
                return true;
            }

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

            return lightness > 0.1;
        }

        private static void ChangeAllWindowCommandsBrush(this MetroWindow window, Brush brush)
        {
            // set the theme based on color lightness
            var theme = brush.NeedLightTheme() ? Theme.Light : Theme.Dark;

            // set the theme to light by default
            window.LeftWindowCommands?.SetValue(WindowCommands.ThemeProperty, theme);
            window.RightWindowCommands?.SetValue(WindowCommands.ThemeProperty, theme);

            // clear or set the foreground property
            if (brush != null)
            {
                window.LeftWindowCommands?.SetValue(Control.ForegroundProperty, brush);
                window.RightWindowCommands?.SetValue(Control.ForegroundProperty, brush);
            }
            else
            {
                window.LeftWindowCommands?.ClearValue(Control.ForegroundProperty);
                window.RightWindowCommands?.ClearValue(Control.ForegroundProperty);
            }
        }

        private static void ChangeAllWindowButtonCommandsBrush(this MetroWindow window, Brush brush, Position position = Position.Top)
        {
            // set the theme to light by default
            if (position == Position.Right || position == Position.Top)
            {
                // set the theme based on color lightness
                var theme = brush.NeedLightTheme() ? Theme.Light : Theme.Dark;

                window.WindowButtonCommands?.SetValue(WindowButtonCommands.ThemeProperty, theme);

                // clear or set the foreground property
                if (brush != null)
                {
                    window.WindowButtonCommands?.SetValue(Control.ForegroundProperty, brush);
                }
                else
                {
                    window.WindowButtonCommands?.ClearValue(Control.ForegroundProperty);
                }
            }
        }
    }
}