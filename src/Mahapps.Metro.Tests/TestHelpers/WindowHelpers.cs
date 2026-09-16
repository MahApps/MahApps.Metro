// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using MahApps.Metro.Controls;
using NUnit.Framework;

namespace MahApps.Metro.Tests.TestHelpers
{
    public static class WindowHelpers
    {
        /// <summary>
        /// A window for a test to work on, off screen unless somebody is watching through a debugger.
        /// </summary>
        /// <param name="onLoadedAction">What to do once it is up.</param>
        /// <param name="withAnimations">
        /// Whether the window fades its overlay in and out and animates the dialogs it shows. A test
        /// that is not about the animations waits through them for nothing, which is most of what a
        /// run of this suite used to spend its time on, so they are off unless a test asks for them.
        /// </param>
        public static Task<T> CreateInvisibleWindowAsync<T>(Action<T>? onLoadedAction = null, bool withAnimations = false)
            where T : Window, new()
        {
            var completionSource = new TaskCompletionSource<T>();

            var window = new T
                         {
                             Width = 800,
                             Height = 600,
                             ShowInTaskbar = false
                         };

            if (withAnimations == false && window is MetroWindow metroWindow)
            {
                metroWindow.SetCurrentValue(MetroWindow.OverlayFadeInProperty, null);
                metroWindow.SetCurrentValue(MetroWindow.OverlayFadeOutProperty, null);
                metroWindow.SetCurrentValue(MetroWindow.MetroDialogOptionsProperty,
                                            new MahApps.Metro.Controls.Dialogs.MetroDialogSettings
                                            {
                                                AnimateShow = false,
                                                AnimateHide = false
                                            });
            }

            if (Debugger.IsAttached == false)
            {
                window.Left = int.MinValue;
                window.Top = int.MinValue;
            }

            window.SetCurrentValue(FrameworkElement.UseLayoutRoundingProperty, true);

            void OnLoaded(object sender, RoutedEventArgs e)
            {
                window.Loaded -= OnLoaded;
                onLoadedAction?.Invoke(window);
            }

            window.Loaded += OnLoaded;

            void OnActivated(object sender, EventArgs args)
            {
                window.Activated -= OnActivated;
                completionSource.SetResult(window);
            }

            window.Activated += OnActivated;

            window.Show();

            return completionSource.Task;
        }

        public static void AssertWindowCommandsColor(this MetroWindow window, Color color)
        {
            Assert.That(window.RightWindowCommands, Is.Not.Null);

            foreach (var element in window.RightWindowCommands!.Items.OfType<Button>())
            {
                Assert.That(((SolidColorBrush)element.Foreground).Color, Is.EqualTo(color));
            }

            Assert.That(((SolidColorBrush)window.WindowButtonCommands!.Foreground).Color, Is.EqualTo(color));
        }

        public static void AssertWindowButtonCommandsColor(this MetroWindow window, Color color)
        {
            Assert.That(window.WindowButtonCommands, Is.Not.Null);

            Assert.That(((SolidColorBrush)window.WindowButtonCommands.Foreground).Color, Is.EqualTo(color));
        }
    }
}