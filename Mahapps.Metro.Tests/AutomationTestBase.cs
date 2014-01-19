using System;

namespace Mahapps.Metro.Tests
{
    /// <summary>
    /// This is the base class for all of our UI tests.
    /// </summary>
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
