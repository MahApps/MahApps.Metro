using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Windows;
using MahApps.Metro.Controls;
using Xunit;

#if !NET40
using System.Windows.Threading;
using Xunit.Sdk;
#endif

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

#if !NET40
            Application.Current.Dispatcher.Invoke(Application.Current.Shutdown);
#endif
        }
    }

#if !NET40
    [CollectionDefinition("ApplicationFixtureCollection")]
    public class ApplicationFixtureCollectionClass : ICollectionFixture<ApplicationFixture>
    {
    }

#endif

    /// <summary>
    /// This is the base class for all of our UI tests.
    /// </summary>
#if !NET40
    [Collection("ApplicationFixtureCollection")]
#endif
    public class AutomationTestBase : IDisposable
#if NET40
        , IUseFixture<ApplicationFixture>
#endif
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

            Application.Current.Invoke(() =>
                {
                    var accent = ThemeManager.Accents.First(x => x.Name == "Blue");
                    var theme = ThemeManager.GetAppTheme("BaseLight");
                    ThemeManager.ChangeAppStyle(Application.Current, accent, theme);
                });
        }

#if NET40
        private ApplicationFixture fixture;
        public void SetFixture(ApplicationFixture data)
        {
            this.fixture = data;
        }
#endif

        /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
        public void Dispose()
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