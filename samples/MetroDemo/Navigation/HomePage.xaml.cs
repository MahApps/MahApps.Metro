using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;

namespace MetroDemo.Navigation
{
    /// <summary>
    /// Interaction logic for HomePage.xaml
    /// </summary>
    public partial class HomePage : Page
    {
        public HomePage()
        {
            InitializeComponent();


            // Example of a synchronous operation when OnNavigated to here.
            OnNavigated(this, e =>
            {
                // Print some text on the console
                string format = string.Format("MetroWindow.OnNavigated example, extra data received: {0}",
                    e.ExtraData);
                Console.WriteLine(format);
            });

#if NET_4_5
            // Example of an asynchronous operation when OnNavigated to here.
            OnNavigated(this, async e =>
            {
                // Show a message dialog to the user
                var frame = (Frame) e.Navigator;
                var window = frame.TryFindParent<MetroWindow>();
                if (window != null)
                {
                    string message = String.Format("Extra data received: {0}", e.ExtraData);
                    await window.ShowMessageAsync("MetroWindow.OnNavigated async example", message);
                }
            });
#endif
        }


        /// <summary>
        ///     Gets if the specified object is being navigated to.
        /// </summary>
        /// <param name="o"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        private static bool IsNavigatedTo(object o, NavigationEventArgs e)
        {
            if (o == null) throw new ArgumentNullException("o");
            if (e == null) throw new ArgumentNullException("e");
            return e.Content != null && Equals(e.Content, o);
        }

        /// <summary>
        ///     Provides an oppurtunity to execute an action when a content is being navigated to.
        /// </summary>
        /// <param name="content">Content object being navigated to.</param>
        /// <param name="action">Action to execute.</param>
        public static void OnNavigated(object content, Action<NavigationEventArgs> action)
        {
            if (content == null) throw new ArgumentNullException("content");
            if (action == null) throw new ArgumentNullException("action");
            NavigatedEventHandler navigated = null;
            navigated = (sender, e) =>
            {
                if (IsNavigatedTo(content, e))
                {
                    action(e);
                }

                Application.Current.Navigated -= navigated;
            };
            Application.Current.Navigated += navigated;
        }

#if NET_4_5
        /// <summary>
        ///     Provides an oppurtunity to execute an asynchronous action when a content is being navigated to.
        /// </summary>
        /// <param name="content">Content object being navigated to.</param>
        /// <param name="function">Asynchronous action to execute.</param>
        public static void OnNavigated(object content, Func<NavigationEventArgs, Task> function)
        {
            if (content == null) throw new ArgumentNullException("content");
            if (function == null) throw new ArgumentNullException("function");
            NavigatedEventHandler navigated = null;
            navigated = async (sender, e) =>
            {
                if (IsNavigatedTo(content, e))
                {
                    await function(e);
                }

                Application.Current.Navigated -= navigated;
            };
            Application.Current.Navigated += navigated;
        }
#endif
    }
}