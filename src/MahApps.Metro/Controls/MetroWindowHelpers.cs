﻿using System;
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
            var currentAppTheme = ThemeManager.DetectTheme(window)
                                  ?? (Application.Current.MainWindow is MetroWindow metroWindow ? ThemeManager.DetectTheme(metroWindow) : null)
                                  ?? ThemeManager.DetectTheme(Application.Current);

            window.ChangeAllWindowCommandsBrush(window.OverrideDefaultWindowCommandsBrush, currentAppTheme);
            window.ChangeAllWindowButtonCommandsBrush(window.OverrideDefaultWindowCommandsBrush, currentAppTheme);
        }

        public static void UpdateWindowCommandsForFlyout(this MetroWindow window, Flyout flyout)
        {
            var currentAppTheme = ThemeManager.DetectTheme(window)
                                  ?? (Application.Current.MainWindow is MetroWindow metroWindow ? ThemeManager.DetectTheme(metroWindow) : null)
                                  ?? ThemeManager.DetectTheme(Application.Current);

            window.ChangeAllWindowCommandsBrush(window.OverrideDefaultWindowCommandsBrush, currentAppTheme);
            window.ChangeAllWindowButtonCommandsBrush(window.OverrideDefaultWindowCommandsBrush ?? flyout.Foreground, currentAppTheme, flyout.Theme, flyout.Position);
        }

        private static void ChangeAllWindowCommandsBrush(this MetroWindow window, Brush foregroundBrush, Metro.Theme currentAppTheme)
        {
            if (foregroundBrush == null)
            {
                window.LeftWindowCommands?.ClearValue(Control.ForegroundProperty);
                window.RightWindowCommands?.ClearValue(Control.ForegroundProperty);
            }

            // set the theme based on current application or window theme
            var theme = currentAppTheme != null && currentAppTheme.Type == ThemeType.Dark
                ? Theme.Dark
                : Theme.Light;

            // set the theme to light by default
            window.LeftWindowCommands?.SetValue(WindowCommands.ThemeProperty, theme);
            window.RightWindowCommands?.SetValue(WindowCommands.ThemeProperty, theme);

            // clear or set the foreground property
            if (foregroundBrush != null)
            {
                window.LeftWindowCommands?.SetValue(Control.ForegroundProperty, foregroundBrush);
                window.RightWindowCommands?.SetValue(Control.ForegroundProperty, foregroundBrush);
            }
        }

        private static void ChangeAllWindowButtonCommandsBrush(this MetroWindow window, Brush foregroundBrush, Metro.Theme currentAppTheme, FlyoutTheme flyoutTheme = FlyoutTheme.Adapt, Position position = Position.Top)
        {
            if (position == Position.Right || position == Position.Top)
            {
                if (foregroundBrush == null)
                {
                    window.WindowButtonCommands?.ClearValue(Control.ForegroundProperty);
                }

                // set the theme based on color lightness
                // otherwise set the theme based on current application or window theme
                var theme = currentAppTheme != null && currentAppTheme.Type == ThemeType.Dark
                    ? Theme.Dark
                    : Theme.Light;

                if (flyoutTheme == FlyoutTheme.Light)
                {
                    theme = Theme.Light;
                }
                else if (flyoutTheme == FlyoutTheme.Dark)
                {
                    theme = Theme.Dark;
                }
                else if (flyoutTheme == FlyoutTheme.Inverse)
                {
                    theme = theme == Theme.Light ? Theme.Dark : Theme.Light;
                }

                window.WindowButtonCommands?.SetValue(WindowButtonCommands.ThemeProperty, theme);

                // clear or set the foreground property
                if (foregroundBrush != null)
                {
                    window.WindowButtonCommands?.SetValue(Control.ForegroundProperty, foregroundBrush);
                }
            }
        }
    }
}