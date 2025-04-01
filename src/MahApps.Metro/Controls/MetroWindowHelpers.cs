﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using ControlzEx.Theming;
using ControlzEx;
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
            if (window is null)
            {
                throw new ArgumentNullException(nameof(window));
            }

            var inputElement = window.GetPart<T>(name) as IInputElement;
            Debug.Assert(inputElement != null, $"{name} is not a IInputElement");
            if (inputElement is not null && WindowChrome.GetIsHitTestVisibleInChrome(inputElement) != hitTestVisible)
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
            if (window is null)
            {
                throw new ArgumentNullException(nameof(window));
            }

            var inputElement = window.GetPart<IInputElement>(name);
            Debug.Assert(inputElement != null, $"{name} is not a IInputElement");
            if (inputElement is not null && WindowChrome.GetResizeGripDirection(inputElement) != direction)
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
            var allOpenFlyouts = flyouts.Where(f => f.IsOpen && f.Position != Position.Bottom).ToList();

            var anyFlyoutOpen = allOpenFlyouts.Any();
            if (!anyFlyoutOpen)
            {
                window.ResetAllWindowCommandsBrush();
            }

            var topFlyout = allOpenFlyouts
                            .Where(x => x.Position == Position.Top)
#if NET6_0_OR_GREATER
                            .MaxBy(Panel.GetZIndex);
#else
                            .OrderByDescending(Panel.GetZIndex)
                            .FirstOrDefault();
#endif
            if (topFlyout != null)
            {
                window.UpdateWindowCommandsForFlyout(topFlyout);
            }
            else
            {
                var leftFlyout = allOpenFlyouts
                                 .Where(x => x.Position == Position.Left)
#if NET6_0_OR_GREATER
                                 .MaxBy(Panel.GetZIndex);
#else
                                 .OrderByDescending(Panel.GetZIndex)
                                 .FirstOrDefault();
#endif
                if (leftFlyout != null)
                {
                    window.UpdateWindowCommandsForFlyout(leftFlyout);
                }

                var rightFlyout = allOpenFlyouts
                                  .Where(x => x.Position == Position.Right)
#if NET6_0_OR_GREATER
                                  .MaxBy(Panel.GetZIndex);
#else
                                  .OrderByDescending(Panel.GetZIndex)
                                  .FirstOrDefault();
#endif
                if (rightFlyout != null)
                {
                    window.UpdateWindowCommandsForFlyout(rightFlyout);
                }
            }
        }

        private static Theme? GetCurrentTheme(FrameworkElement frameworkElement)
        {
            var currentTheme = ThemeManager.Current.DetectTheme(frameworkElement);
            if (currentTheme is null)
            {
                var application = Application.Current;
                if (application is not null)
                {
                    currentTheme = application.MainWindow is null
                        ? ThemeManager.Current.DetectTheme(application)
                        : ThemeManager.Current.DetectTheme(application.MainWindow);
                }
            }

            return currentTheme;
        }

        public static void ResetAllWindowCommandsBrush(this MetroWindow window)
        {
            var currentTheme = GetCurrentTheme(window);

            window.ChangeAllWindowCommandsBrush(window.OverrideDefaultWindowCommandsBrush, currentTheme);
            window.ChangeAllWindowButtonCommandsBrush(window.OverrideDefaultWindowCommandsBrush, currentTheme);
        }

        public static void UpdateWindowCommandsForFlyout(this MetroWindow window, Flyout flyout)
        {
            var currentTheme = GetCurrentTheme(window);

            window.ChangeAllWindowCommandsBrush(window.OverrideDefaultWindowCommandsBrush, currentTheme);
            window.ChangeAllWindowButtonCommandsBrush(window.OverrideDefaultWindowCommandsBrush ?? flyout.Foreground, currentTheme, flyout.Theme, flyout.Position);
        }

        private static void ChangeAllWindowCommandsBrush(this MetroWindow window, Brush? foregroundBrush, ControlzEx.Theming.Theme? currentAppTheme)
        {
            if (foregroundBrush is null)
            {
                window.LeftWindowCommands?.ClearValue(Control.ForegroundProperty);
                window.RightWindowCommands?.ClearValue(Control.ForegroundProperty);
            }

            // set the theme based on current application or window theme
            var theme = currentAppTheme != null && currentAppTheme.BaseColorScheme == ThemeManager.BaseColorDark
                ? ThemeManager.BaseColorDark
                : ThemeManager.BaseColorLight;

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

        private static void ChangeAllWindowButtonCommandsBrush(this MetroWindow window, Brush? foregroundBrush, ControlzEx.Theming.Theme? currentAppTheme, FlyoutTheme flyoutTheme = FlyoutTheme.Adapt, Position position = Position.Top)
        {
            if (position == Position.Right || position == Position.Top)
            {
                if (foregroundBrush is null)
                {
                    window.WindowButtonCommands?.ClearValue(Control.ForegroundProperty);
                }

                // set the theme based on color lightness
                // otherwise set the theme based on current application or window theme
                var theme = currentAppTheme != null && currentAppTheme.BaseColorScheme == ThemeManager.BaseColorDark
                    ? ThemeManager.BaseColorDark
                    : ThemeManager.BaseColorLight;

                theme = flyoutTheme switch
                {
                    FlyoutTheme.Light => ThemeManager.BaseColorLight,
                    FlyoutTheme.Dark => ThemeManager.BaseColorDark,
                    FlyoutTheme.Inverse => theme == ThemeManager.BaseColorLight ? ThemeManager.BaseColorDark : ThemeManager.BaseColorLight,
                    _ => theme
                };

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