using System.Threading.Tasks;
using System.Windows;

namespace Mahapps.Metro.Tests
{
    public static class TestHelpers
    {
        public static Task AwaitLoaded(this Window window)
        {
            var completionSource = new TaskCompletionSource<object>();

            RoutedEventHandler handler = null;

            handler = (sender, args) =>
            {
                completionSource.TrySetResult(null);
                window.Loaded -= handler;
            };

            window.Loaded += handler;
            
            if (window.IsLoaded)
            {
                completionSource.TrySetResult(null);
            }

            return completionSource.Task;
        }

        public static async Task<T> CreateInvisibleWindowAsync<T>() where T : Window, new()
        {
            var wnd = new T
            {
                Visibility = Visibility.Hidden, 
                ShowInTaskbar = false
            };

            wnd.Show();

            await wnd.AwaitLoaded();

            return wnd;
        }
    }
}
