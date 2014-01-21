﻿using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Windows;

namespace Mahapps.Metro.Tests
{
    /// <summary>
    /// This class is the ultimate hack to work around that we can't 
    /// create more than one application in the same AppDomain
    /// 
    /// It is once initialized at the start and never properly cleaned up, 
    /// this means the AppDomain will throw an exception when xUnit unloads it.
    /// </summary>
    public class TestHost
    {
        private TestApp app;
        private readonly Thread appThread;
        private readonly AutoResetEvent gate = new AutoResetEvent(false);

        private static TestHost testHost;

        public static void Initialize()
        {
            if (testHost == null)
            {
                testHost = new TestHost();
            }
        }

        private TestHost()
        {
            appThread = new Thread(StartDispatcher);
            appThread.SetApartmentState(ApartmentState.STA);
            appThread.Start();
            
            gate.WaitOne();
        }

        private void StartDispatcher()
        {
            app = new TestApp { ShutdownMode = ShutdownMode.OnExplicitShutdown };
            app.Startup += (sender, args) => gate.Set();
            app.Run();
        }

        /// <summary>
        /// Await this method in every test that should run on the UI thread.
        /// </summary>
        public static SwitchContextToUiThreadAwaiter SwitchToAppThread()
        {
            return new SwitchContextToUiThreadAwaiter(testHost.app.Dispatcher);
        }
    }
}
