using System.Threading;
using System.Windows;
using System.Windows.Threading;
using NUnit.Framework;

namespace MahApps.Metro.Tests
{
    [SetUpFixture]
    public class AssemblySetup
    {
        [OneTimeSetUp]
        public void Setup()
        {
            SynchronizationContext.SetSynchronizationContext(new DispatcherSynchronizationContext());

            var app = new TestApp { ShutdownMode = ShutdownMode.OnExplicitShutdown };
            app.InitializeComponent();
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            Dispatcher.CurrentDispatcher.InvokeShutdown();
        }
    }
}