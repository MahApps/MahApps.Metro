using System.Threading;
using System.Windows;
using System.Windows.Threading;

namespace Mahapps.Metro.Tests
{
    public static class TestHost
    {
        private static Application app;
        private static Thread appThread;
        private static AutoResetEvent gate = new AutoResetEvent(false);

        public static void Start()
        {
            appThread = new Thread(StartDispatcher);
            appThread.SetApartmentState(ApartmentState.STA);
            appThread.Start();

            gate.WaitOne();
        }

        public static void StartDispatcher()
        {
            app = new Application { ShutdownMode = ShutdownMode.OnExplicitShutdown };
            app.Startup += (sender, args) => gate.Set();
            app.Run();
        }

        public static SwitchContextToUiThreadAwaiter SwitchToAppThread()
        {
            return new SwitchContextToUiThreadAwaiter(app.Dispatcher);
        }

        public static void Shutdown()
        {
            app.Dispatcher.BeginInvokeShutdown(DispatcherPriority.Send);
            appThread.Join();
        }
    }
}
