using System;
using System.Windows.Controls;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;

namespace MetroDemo.Navigation
{
    /// <summary>
    ///     Interaction logic for HomePage.xaml
    /// </summary>
    public partial class HomePage : Page
    {
        public HomePage()
        {
            InitializeComponent();

            // Example of a synchronous operation when OnNavigated to here.
            this.OnNavigated(e =>
            {
                // Print some text on the console
                string format = String.Format("MetroWindow.OnNavigated example, extra data received: '{0}'",
                    e.ExtraData);
                Console.WriteLine(format);
            });

#if NET_4_5
            // Example of an asynchronous operation when OnNavigated to here.
            this.OnNavigated(async e =>
            {
                // Show a message dialog to the user
                var frame = (Frame)e.Navigator;
                var window = frame.TryFindParent<MetroWindow>();
                if (window != null)
                {
                    if (window is MetroNavigationWindow)
                    {
                        // NOTE : actually OnNavigated is useful only for MetroWindow so
                        // we do not show this example for MetroNavigationWindow 
                    }
                    else
                    {
                        string message = String.Format("Extra data received: {0}", e.ExtraData);
                        await window.ShowMessageAsync("MetroWindow.OnNavigated async action example", message);
                    }
                }
            });
#endif
        }
    }
}