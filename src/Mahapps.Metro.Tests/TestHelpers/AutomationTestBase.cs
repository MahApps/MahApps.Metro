// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using ControlzEx.Theming;
using MahApps.Metro.Controls;
using Xunit;
using Xunit.Sdk;

namespace MahApps.Metro.Tests.TestHelpers
{
    public class ApplicationFixture : IDisposable
    {
        public ApplicationFixture()
        {
            // ... initialize
            TestHost.Initialize();
        }

        public void Dispose()
        {
            // ... clean up
            GC.Collect();
            Dispatcher.ExitAllFrames();
            Application.Current.Dispatcher.Invoke(Application.Current.Shutdown);
        }
    }

    [CollectionDefinition("ApplicationFixtureCollection")]
    public class ApplicationFixtureCollectionClass : ICollectionFixture<ApplicationFixture>
    {
    }

    /// <summary>
    /// This is the base class for all of our UI tests.
    /// </summary>
    [Collection("ApplicationFixtureCollection")]
    public class AutomationTestBase : IDisposable
    {
        public AutomationTestBase()
        {
            var message = $"Create test class '{this.GetType().Name}' with Thread.CurrentThread: {Thread.CurrentThread.ManagedThreadId}" +
                          $" and Current.Dispatcher.Thread: {Application.Current.Dispatcher.Thread.ManagedThreadId}";
            Debug.WriteLine(message);

            // Reset the application as good as we can
            Application.Current.Invoke(() =>
                {
                    var windows = Application.Current.Windows.OfType<Window>().ToList();
                    foreach (Window window in windows)
                    {
                        window.Close();
                    }
                });

            Application.Current.Invoke(() => { ThemeManager.Current.ChangeTheme(Application.Current, "Light.Blue"); });
        }

        /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
        public virtual void Dispose()
        {
            var message = $"Dispose test class '{this.GetType().Name}' with Thread.CurrentThread: {Thread.CurrentThread.ManagedThreadId}" +
                          $" and Current.Dispatcher.Thread: {Application.Current.Dispatcher.Thread.ManagedThreadId}";
            Debug.WriteLine(message);
        }
    }

    public class DisplayTestMethodNameAttribute : BeforeAfterTestAttribute
    {
        public override void Before(MethodInfo methodUnderTest)
        {
            var message = $"Setup for test '{methodUnderTest.Name}' with Thread.CurrentThread: {Thread.CurrentThread.ManagedThreadId}" +
                          $" and Current.Dispatcher.Thread: {Application.Current.Dispatcher.Thread.ManagedThreadId}";
            Debug.WriteLine(message);
        }

        public override void After(MethodInfo methodUnderTest)
        {
            var message = $"TearDown for test '{methodUnderTest.Name}' with Thread.CurrentThread: {Thread.CurrentThread.ManagedThreadId}" +
                          $" and Current.Dispatcher.Thread: {Application.Current.Dispatcher.Thread.ManagedThreadId}";
            Debug.WriteLine(message);
        }
    }
}