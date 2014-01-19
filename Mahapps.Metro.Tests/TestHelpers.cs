using System;
using System.Threading.Tasks;
using System.Windows;

namespace Mahapps.Metro.Tests
{
    public static class TestHelpers
    {
        public static Task<T> CreateInvisibleWindowAsync<T>() where T : Window, new()
        {
            var window = new T
            {
                Visibility = Visibility.Hidden, 
                ShowInTaskbar = false
            };

            var completionSource = new TaskCompletionSource<T>();

            EventHandler handler = null;

            handler = (sender, args) =>
            {
                window.Activated -= handler;
                completionSource.SetResult(window);
            };

            window.Activated += handler;
            
            window.Show();

            return completionSource.Task;
        }
    }
}
