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
        /// How long a window gets to become the active one before the test gives up on it. A run of
        /// the whole suite needs a second or two per window, so this is room to spare on a loaded
        /// build agent and still well inside the five minutes the blame collector allows.
        /// </summary>
        private static readonly TimeSpan ActivationTimeout = TimeSpan.FromSeconds(30);

        /// <summary>
        /// A window for a test to work on, off screen unless somebody is watching through a debugger.
        /// </summary>
        /// <param name="onLoadedAction">What to do once it is up.</param>
        /// <param name="withAnimations">
        /// Whether the window fades its overlay in and out and animates the dialogs it shows. A test
        /// that is not about the animations waits through them for nothing, which is most of what a
        /// run of this suite used to spend its time on, so they are off unless a test asks for them.
        /// </param>
        public static async Task<T> CreateInvisibleWindowAsync<T>(Action<T>? onLoadedAction = null, bool withAnimations = false)
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
                completionSource.TrySetResult(window);
            }

            window.Activated += OnActivated;

            window.Show();

            // Only one window on a desktop can be the active one, and the one that comes up second
            // does not always get there on a build agent. Waiting for that with nothing behind it
            // holds the test host until somebody kills it, and the run says nothing about why, so
            // the wait gives up and names what it was waiting for.
            var finished = await Task.WhenAny(completionSource.Task, Task.Delay(ActivationTimeout)).ConfigureAwait(true);

            if (finished != completionSource.Task)
            {
                window.Activated -= OnActivated;
                window.Close();

                Assert.Fail($"the {typeof(T).Name} was not activated within {ActivationTimeout.TotalSeconds:0} seconds");
            }

            return await completionSource.Task.ConfigureAwait(true);
        }

        /// <summary>
        /// Puts something in the window and lets the layout and the dispatcher catch up, so that a
        /// test can look at what the templates made of it.
        /// </summary>
        public static T Show<T>(this Window? window, T content)
            where T : FrameworkElement
        {
            Assert.That(window, Is.Not.Null);

            window!.Content = content;
            window.UpdateLayout();
            content.UpdateLayout();
            ClipAssert.Pump();

            Assert.That(content.IsLoaded, Is.True, $"the {content.GetType().Name} should be up before a test looks at it");

            return content;
        }

        /// <summary>
        /// Lets the layout and the dispatcher catch up again, for a test that goes on poking at what
        /// the window is already showing.
        /// </summary>
        public static void Settle(this Window? window)
        {
            Assert.That(window, Is.Not.Null);

            window!.UpdateLayout();
            ClipAssert.Pump();
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