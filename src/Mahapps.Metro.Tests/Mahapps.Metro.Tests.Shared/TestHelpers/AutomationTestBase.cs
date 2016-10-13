using System.Linq;
using System.Windows;
using MahApps.Metro.Controls;

namespace MahApps.Metro.Tests.TestHelpers
{
    using System;
    using System.Diagnostics;
    using System.Reflection;
    using System.Threading;
    using Xunit;

    /// <summary>
    /// This is the base class for all of our UI tests.
    /// </summary>
    public class AutomationTestBase : IDisposable
    {
        public AutomationTestBase()
        {
            TestHost.Initialize();

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
            //Console.WriteLine("Setup for test '{0}.'", methodUnderTest.Name);
        }

        public override void After(MethodInfo methodUnderTest)
        {
            var message = $"TearDown for test '{methodUnderTest.Name}' with Thread.CurrentThread: {Thread.CurrentThread.ManagedThreadId}" +
                          $" and Current.Dispatcher.Thread: {Application.Current.Dispatcher.Thread.ManagedThreadId}";
            Debug.WriteLine(message);
            //Console.WriteLine("TearDown for test '{0}.'", methodUnderTest.Name);
        }
    }
}