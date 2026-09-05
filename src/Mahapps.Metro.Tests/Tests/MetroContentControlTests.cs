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
        private static readonly TimeSpan Timeout = TimeSpan.FromSeconds(5);

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
        /// Runs the dispatcher for a while. The transition is an animation, so the test has to let the
        /// dispatcher work instead of blocking the thread it runs on.
        /// </summary>
        private static void Pump(TimeSpan duration)
        {
            var frame = new DispatcherFrame();
            var timer = new DispatcherTimer(duration, DispatcherPriority.Background, (_, _) => frame.Continue = false, Dispatcher.CurrentDispatcher);

            try
            {
                Dispatcher.PushFrame(frame);
            }
            finally
            {
                timer.Stop();
            }
        }

        /// <summary>
        /// Runs the dispatcher until the condition holds, and reports whether it did before the time was up.
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

            void OnStarted(object? sender, RoutedEventArgs e)
            {
                started++;
            }

            void OnCompleted(object? sender, RoutedEventArgs e)
            {
                completed++;
            }

            control.TransitionStarted += OnStarted;
            control.TransitionCompleted += OnCompleted;
            try
            {
                trigger();

                // Reload ends the previous storyboard, which raises a completed event right away, so
                // wait for the new transition to start before waiting for it to finish.
                Assert.That(PumpUntil(() => started > 0, Timeout), Is.True, "the transition did not start in time");

                // that early completed belongs to the previous storyboard, only count from here on
                completed = 0;
                Assert.That(PumpUntil(() => control.IsTransitioning == false && completed > 0, Timeout), Is.True, "the transition did not finish in time");

                // the clock keeps ticking for a moment after the storyboard completed
                Pump(TimeSpan.FromMilliseconds(300));
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

            Assert.That(PumpUntil(() => control.IsTransitioning == false, Timeout), Is.True, "the control should settle before the test starts");

            bool? whileStarting = null;

            void OnStarted(object? sender, RoutedEventArgs e)
            {
                whileStarting ??= control.IsTransitioning;
            }

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

            Assert.That(PumpUntil(() => control.IsTransitioning == false, Timeout), Is.True, "the control should settle before the test starts");

            var (started, completed) = RunTransition(control, control.Reload);

            Assert.That(completed, Is.EqualTo(1), "the transition should complete once");
            Assert.That(started, Is.EqualTo(1), "the transition should start once");
        }
    }
}
