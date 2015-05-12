using System.Threading;
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
