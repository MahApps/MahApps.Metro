// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace MahApps.Metro.Tests.TestHelpers
{
    /// <summary>
    /// This class is the ultimate hack to work around that we can't 
    /// create more than one application in the same AppDomain
    /// 
    /// It is initialized once at startup and is never properly cleaned up, 
    /// this means the AppDomain will throw an exception when xUnit unloads it.
    /// 
    /// Your test runner will inevitably hate you and hang endlessly after every test has run.
    /// The Resharper runner will also throw an exception message in your face.
    /// 
    /// Better than no unit tests.
    /// </summary>
    public class TestHost
    {
        private TestApp? app;
        private readonly Thread? appThread;
        private readonly AutoResetEvent gate = new(false);

        private static TestHost? testHost;

        public static void Initialize()
        {
            testHost ??= new TestHost();
        }

        private TestHost()
        {
            this.appThread = new Thread(this.StartDispatcher);
            this.appThread.SetApartmentState(ApartmentState.STA);
            this.appThread.Start();

            this.gate.WaitOne();
        }

        private void StartDispatcher()
        {
            this.app = new TestApp { ShutdownMode = ShutdownMode.OnExplicitShutdown };
            this.app.InitializeComponent();
            this.app.Exit += (_, _) =>
                {
                    var message = $"Exit TestApp with Thread.CurrentThread: {Thread.CurrentThread.ManagedThreadId}" +
                                  $" and Current.Dispatcher.Thread: {Application.Current.Dispatcher.Thread.ManagedThreadId}";
                    Debug.WriteLine(message);
                };
            this.app.Startup += async (_, _) =>
                {
                    var message = $"Start TestApp with Thread.CurrentThread: {Thread.CurrentThread.ManagedThreadId}" +
                                  $" and Current.Dispatcher.Thread: {Application.Current.Dispatcher.Thread.ManagedThreadId}";
                    Debug.WriteLine(message);
                    this.gate.Set();
                    await Task.Yield();
                };
            this.app.Run();
        }

        /// <summary>
        /// Await this method in every test that should run on the UI thread.
        /// </summary>
        public static SwitchContextToUiThreadAwaiter SwitchToAppThread()
        {
            if (testHost?.app is null)
            {
                throw new InvalidOperationException($"{nameof(TestHost)} is not initialized!");
            }

            return new SwitchContextToUiThreadAwaiter(testHost.app.Dispatcher);
        }
    }
}