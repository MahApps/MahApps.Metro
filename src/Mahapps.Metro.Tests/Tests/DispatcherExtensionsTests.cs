// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Threading;
using System.Windows.Controls;
using System.Windows.Threading;
using MahApps.Metro.Controls;
using NUnit.Framework;

namespace MahApps.Metro.Tests.Tests
{
    /// <summary>
    /// GH-4085: a window on a thread of its own is still on the events it subscribed to after its
    /// dispatcher has been shut down. Asking such a dispatcher to run something throws a cancelled task
    /// at whoever raised the event, which in the report brought the application down on a theme change.
    /// </summary>
    [TestFixture]
    public class DispatcherExtensionsTests
    {
        private Thread? thread;
        private Dispatcher? otherDispatcher;
        private Border? onTheOtherThread;

        [SetUp]
        public void SetUp()
        {
            var standing = new ManualResetEventSlim();

            this.thread = new Thread(() =>
                {
                    this.onTheOtherThread = new Border();
                    this.otherDispatcher = Dispatcher.CurrentDispatcher;
                    standing.Set();
                    Dispatcher.Run();
                });

            this.thread.SetApartmentState(ApartmentState.STA);
            this.thread.IsBackground = true;
            this.thread.Start();

            Assert.That(standing.Wait(5000), Is.True, "the other thread should have come up");
        }

        [TearDown]
        public void TearDown()
        {
            this.otherDispatcher?.InvokeShutdown();
            this.thread?.Join(5000);

            this.thread = null;
            this.otherDispatcher = null;
            this.onTheOtherThread = null;
        }

        [Test]
        [Description("While the other thread is standing, the action runs on it.")]
        public void InvokingReachesTheOtherThread()
        {
            var ranOn = 0;

            // a block body, or the assignment would be an expression and this would be the overload that hands a value back
            this.onTheOtherThread!.Invoke(() => { ranOn = Thread.CurrentThread.ManagedThreadId; });

            Assert.That(ranOn, Is.EqualTo(this.thread!.ManagedThreadId), "the action belongs on the thread of its object");
        }

        [Test]
        [Description("Once that thread is gone there is nobody to run it, and saying so with an exception helps no one.")]
        public void InvokingOnADispatcherThatIsGoingAwayIsLetGo()
        {
            this.otherDispatcher!.InvokeShutdown();
            this.thread!.Join(5000);

            Assume.That(this.otherDispatcher.HasShutdownStarted, Is.True);

            var ran = false;

            Assert.DoesNotThrow(() => this.onTheOtherThread!.Invoke(() => { ran = true; }));
            Assert.That(ran, Is.False, "there is nobody left to run it");
        }
    }
}
