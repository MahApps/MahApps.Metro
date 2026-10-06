// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows;
using System.Windows.Controls;
using MahApps.Metro.Controls;
using MahApps.Metro.Gallery.Core;
using ShowMeTheXAML;

namespace MahApps.Metro.Gallery.Pages
{
    /// <summary>
    /// Interaction logic for ChildWindowPage.xaml
    /// </summary>
    public partial class ChildWindowPage : UserControl
    {
        private ChildWindow? plain;
        private ChildWindow? movable;
        private ChildWindow? modeless;
        private ChildWindow? answer;

        public ChildWindowPage()
        {
            this.InitializeComponent();

            this.Loaded += this.OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            if (this.plain is not null)
            {
                return;
            }

            this.plain = this.TakeChildWindow("PlainChildWindow");
            this.movable = this.TakeChildWindow("MovableChildWindow");
            this.modeless = this.TakeChildWindow("ModelessChildWindow");
            this.answer = this.TakeChildWindow("AnswerChildWindow");

            this.PlainExample.Watch(this.plain,
                                    ChildWindow.TitleProperty,
                                    ChildWindow.ShowTitleBarProperty,
                                    ChildWindow.IsModalProperty,
                                    ChildWindow.CloseOnOverlayProperty,
                                    ChildWindow.CloseByEscapeProperty);
            this.PlainExample.Watch("Layout", this.plain, ChildWindow.ChildWindowWidthProperty);

            this.MovableExample.Watch(this.movable,
                                      ChildWindow.AllowMoveProperty,
                                      ChildWindow.ShowTitleBarCloseButtonProperty,
                                      ChildWindow.ShowTitleBarProperty);

            this.ModelessExample.Watch(this.modeless,
                                       ChildWindow.IsModalProperty,
                                       ChildWindow.CloseOnOverlayProperty);

            this.AnswerExample.Watch(this.answer,
                                     ChildWindow.TitleProperty,
                                     ChildWindow.CloseByEscapeProperty);
        }

        /// <summary>
        /// Takes the child window out of the display that holds it, so the window can put it into
        /// its dialog container. It is a sample, so it wears the default set, not the one of the gallery.
        /// </summary>
        private ChildWindow TakeChildWindow(string key)
        {
            var display = (XamlDisplay)this.FindResource(key);
            var childWindow = (ChildWindow)display.Content;

            display.Content = null;

            DefaultStyleSet.Apply(childWindow);

            return childWindow;
        }

        private async void Show(ChildWindow? childWindow)
        {
            // still on its way out, it is in the container until the hide is over
            if (childWindow is { IsOpen: false, Parent: null } && Window.GetWindow(this) is MetroWindow window)
            {
                await window.ShowChildWindowAsync(childWindow);
            }
        }

        private void OnShowPlain(object sender, RoutedEventArgs e) => this.Show(this.plain);

        private void OnShowMovable(object sender, RoutedEventArgs e) => this.Show(this.movable);

        private void OnShowModeless(object sender, RoutedEventArgs e) => this.Show(this.modeless);

        private async void OnShowAnswer(object sender, RoutedEventArgs e)
        {
            if (this.answer is not { IsOpen: false, Parent: null } || Window.GetWindow(this) is not MetroWindow window)
            {
                return;
            }

            var result = await window.ShowChildWindowAsync<object>(this.answer);
            this.AnswerText.Text = $"The answer was: {result}";
        }

        private void OnClose(object sender, RoutedEventArgs e)
        {
            (sender as DependencyObject)?.TryFindParent<ChildWindow>()?.Close();
        }

        private void OnAnswer(object sender, RoutedEventArgs e)
        {
            if (sender is FrameworkElement button)
            {
                button.TryFindParent<ChildWindow>()?.Close(button.Tag);
            }
        }
    }
}
