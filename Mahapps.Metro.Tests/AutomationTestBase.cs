using System;
using System.Linq;
using System.Windows;
using MahApps.Metro;

namespace Mahapps.Metro.Tests
{
    /// <summary>
    /// This is the base class for all of our UI tests.
    /// </summary>
    public class AutomationTestBase
    {
        public AutomationTestBase()
        {
            TestHost.Initialize();

            // Reset the application as good as we can
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                foreach (Window window in Application.Current.Windows)
                {
                    window.Close();
                }
            }));

            ThemeManager.ChangeTheme(Application.Current, ThemeManager.DefaultAccents.First(x => x.Name == "Blue"), Theme.Light);
        }
    }
}
