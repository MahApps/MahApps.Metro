using System;
using System.Linq;
using System.Windows;
using MahApps.Metro;

namespace MahApps.Metro.Tests.TestHelpers
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

            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                var accent = ThemeManager.Accents.First(x => x.Name == "Blue");
                var theme = ThemeManager.GetAppTheme("BaseLight");
                ThemeManager.ChangeAppStyle(Application.Current, accent, theme);
            }));
        }
    }
}
