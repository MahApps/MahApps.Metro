﻿using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using MahApps.Metro.Controls;
using Xunit;

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

        public static void AssertWindowCommandsColor(this MetroWindow window, Color color)
        {
            Assert.True(window.RightWindowCommands.Items.Cast<Button>().Select(x => ((SolidColorBrush)x.Foreground).Color).All(x => x == color));
            Assert.Equal(color, ((SolidColorBrush)window.WindowButtonCommands.Foreground).Color);
        }
    }
}
