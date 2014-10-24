using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Navigation;

namespace MahApps.Metro.Controls
{
    /// <summary>
    ///     Helper methods related to navigation.
    /// </summary>
    public static class MetroWindowNavigationHelpers
    {
        /* NOTE : these members have not been put in MetroWindowHelpers
         * because we need a public class so that users can consume them. */

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
            return e.Content != null && ReferenceEquals(e.Content, o);
        }

        /// <summary>
        ///     Provides an oppurtunity to execute an action when a content is being navigated to.
        /// </summary>
        /// <param name="content">Content object being navigated to.</param>
        /// <param name="action">Action to execute.</param>
        public static void OnNavigated(this object content, Action<NavigationEventArgs> action)
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

#if !NET_4
    /// <summary>
    ///     Provides an oppurtunity to execute an asynchronous action when a content is being navigated to.
    /// </summary>
    /// <param name="content">Content object being navigated to.</param>
    /// <param name="function">Asynchronous action to execute.</param>
        public static void OnNavigated(this object content, Func<NavigationEventArgs, Task> function)
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