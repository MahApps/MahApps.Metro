using System;

namespace Mahapps.Metro.Tests
{
    public class AutomationTestBase : IDisposable
    {
        public AutomationTestBase()
        {
            TestHost.Start();
        }

        public void Dispose()
        {
            TestHost.Shutdown();
        }
    }
}
