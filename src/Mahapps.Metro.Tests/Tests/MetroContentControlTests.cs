// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using MahApps.Metro.Controls;
using MahApps.Metro.Tests.TestHelpers;
using MahApps.Metro.Tests.Views;
using NUnit.Framework;

namespace MahApps.Metro.Tests.Tests
{
    [TestFixture]
    public class MetroContentControlTests
    {
        private MetroContentControlWindow? window;

        [OneTimeSetUp]
        public async Task OneTimeSetUp()
        {
            this.window = await WindowHelpers.CreateInvisibleWindowAsync<MetroContentControlWindow>().ConfigureAwait(false);
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            this.window?.Close();
            this.window = null;
        }

        /// <summary>
        /// Runs the dispatcher until the condition holds or the time is up. The transition is an animation,
        /// so the test has to let the dispatcher work while it waits.
        /// </summary>
        private static bool PumpUntil(Func<bool> condition, TimeSpan timeout)
        {
            var frame = new DispatcherFrame();
            var timer = new DispatcherTimer(TimeSpan.FromMilliseconds(20),
                                            DispatcherPriority.Background,
                                            (_, _) =>
                                                {
                                                    if (condition())
                                                    {
                                                        frame.Continue = false;
                                                    }
                                                },
                                            Dispatcher.CurrentDispatcher);
            var deadline = new DispatcherTimer(timeout, DispatcherPriority.Background, (_, _) => frame.Continue = false, Dispatcher.CurrentDispatcher);

            try
            {
                Dispatcher.PushFrame(frame);
            }
            finally
            {
                timer.Stop();
                deadline.Stop();
            }

            return condition();
        }

        /// <summary>
        /// Triggers a transition and waits for it to finish, counting both events on the way.
        /// </summary>
        private static (int started, int completed) RunTransition(MetroContentControl control, Action trigger)
        {
            var started = 0;
            var completed = 0;

            void OnStarted(object? sender, RoutedEventArgs e) => started++;
            void OnCompleted(object? sender, RoutedEventArgs e) => completed++;

            control.TransitionStarted += OnStarted;
            control.TransitionCompleted += OnCompleted;
            try
            {
                trigger();

                // Reload ends the previous storyboard, which raises a completed event right away, so
                // wait for the new transition to start before waiting for it to finish.
                PumpUntil(() => started > 0, TimeSpan.FromSeconds(5));

                // that early completed belongs to the previous storyboard, only count from here on
                completed = 0;
                PumpUntil(() => control.IsTransitioning == false && completed > 0, TimeSpan.FromSeconds(5));
                PumpUntil(() => false, TimeSpan.FromMilliseconds(300));
            }
            finally
            {
                control.TransitionStarted -= OnStarted;
                control.TransitionCompleted -= OnCompleted;
            }

            return (started, completed);
        }

        [Test]
        public void ShouldReportIsTransitioningWhileTheTransitionRuns()
        {
            Assert.That(this.window, Is.Not.Null);

            var control = this.window.TheMetroContentControl;

            PumpUntil(() => control.IsTransitioning == false, TimeSpan.FromSeconds(5));

            bool? whileStarting = null;

            void OnStarted(object? sender, RoutedEventArgs e) => whileStarting ??= control.IsTransitioning;

            control.TransitionStarted += OnStarted;
            try
            {
                RunTransition(control, control.Reload);
            }
            finally
            {
                control.TransitionStarted -= OnStarted;
            }

            Assert.That(whileStarting, Is.True, "IsTransitioning should already be set when the event is raised");
            Assert.That(control.IsTransitioning, Is.False, "IsTransitioning should be cleared once the transition is over");
        }

        [Test]
        public void ShouldRaiseTransitionStartedOncePerTransition()
        {
            Assert.That(this.window, Is.Not.Null);

            var control = this.window.TheMetroContentControl;

            // the control transitions once while it loads, let that one finish first
            PumpUntil(() => control.IsTransitioning == false, TimeSpan.FromSeconds(5));

            var (started, completed) = RunTransition(control, control.Reload);

            Assert.That(completed, Is.EqualTo(1), "the transition should complete once");
            Assert.That(started, Is.EqualTo(1), "the transition should start once");
        }
    }
}
