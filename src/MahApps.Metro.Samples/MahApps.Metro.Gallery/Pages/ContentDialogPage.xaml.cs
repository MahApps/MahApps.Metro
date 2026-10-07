// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using MahApps.Metro.Controls;
using MahApps.Metro.Gallery.Core;
using ShowMeTheXAML;

namespace MahApps.Metro.Gallery.Pages
{
    /// <summary>
    /// Interaction logic for ContentDialogPage.xaml
    /// </summary>
    public partial class ContentDialogPage : UserControl
    {
        /// <summary>
        /// The dialogs by card and look, ThreeButtons0 the Metro one, ThreeButtons1 the Windows 10 one, ThreeButtons2 the WinUI one.
        /// </summary>
        private readonly Dictionary<string, ContentDialog> dialogs = new();

        public ContentDialogPage()
        {
            this.InitializeComponent();

            this.Loaded += this.OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            if (this.dialogs.Count > 0)
            {
                return;
            }

            var texts = new[]
                        {
                            ContentDialog.TitleProperty,
                            ContentDialog.PrimaryButtonTextProperty,
                            ContentDialog.SecondaryButtonTextProperty,
                            ContentDialog.CloseButtonTextProperty,
                            ContentDialog.DefaultButtonProperty
                        };
            var enabled = new[] { ContentDialog.IsPrimaryButtonEnabledProperty, ContentDialog.IsSecondaryButtonEnabledProperty };
            var titleBar = new[] { ChildWindow.ShowTitleBarProperty, ChildWindow.AllowMoveProperty, ChildWindow.ShowTitleBarCloseButtonProperty };

            var threeButtons = this.TakeDialogs("ThreeButtons", "ThreeButtonsDisplay", texts.Concat(enabled));
            this.TakeDialogs("Result", "ResultDisplay", texts);
            this.TakeDialogs("Input", "InputDisplay", texts);
            var withTitleBar = this.TakeDialogs("TitleBar", "TitleBarDisplay", texts.Concat(titleBar));

            this.ThreeButtonsExample.Watch(threeButtons,
                                           ContentDialog.TitleProperty,
                                           ContentDialog.PrimaryButtonTextProperty,
                                           ContentDialog.SecondaryButtonTextProperty,
                                           ContentDialog.CloseButtonTextProperty,
                                           ContentDialog.DefaultButtonProperty,
                                           ContentDialog.IsPrimaryButtonEnabledProperty,
                                           ContentDialog.IsSecondaryButtonEnabledProperty);

            this.TitleBarExample.Watch(withTitleBar,
                                       ChildWindow.ShowTitleBarProperty,
                                       ChildWindow.AllowMoveProperty,
                                       ChildWindow.ShowTitleBarCloseButtonProperty);
        }

        /// <summary>
        /// Takes the three dialogs out of the display, binds the Windows 10 and the WinUI one to the Metro one
        /// for the given properties, and returns the Metro one. They are samples, so they wear the default set,
        /// not the one of the gallery, apart from the style each of the two copies names.
        /// </summary>
        private ContentDialog TakeDialogs(string name, string displayKey, IEnumerable<DependencyProperty> followed)
        {
            var display = (XamlDisplay)this.FindResource(displayKey);
            var panel = (Panel)display.Content;
            var three = panel.Children.OfType<ContentDialog>().ToList();
            panel.Children.Clear();

            var first = three[0];
            foreach (var property in followed)
            {
                foreach (var copy in three.Skip(1))
                {
                    BindingOperations.SetBinding(copy, property, new Binding(property.Name) { Source = first });
                }
            }

            // the Metro one wears the default set; the two copies name the style of their look on
            // the dialog and on whatever control is in it, the way the other pages show a look
            DefaultStyleSet.Apply(three[0]);

            for (var i = 0; i < three.Count; i++)
            {
                this.dialogs[name + i] = three[i];
            }

            return first;
        }

        private async void OnShow(object sender, RoutedEventArgs e)
        {
            if (sender is FrameworkElement { Tag: string name }
                && this.dialogs.TryGetValue(name, out var dialog)
                && dialog is { IsOpen: false, Parent: null }
                && Window.GetWindow(this) is MetroWindow window)
            {
                var result = await dialog.ShowAsync(window);
                if (name.StartsWith("Result", StringComparison.Ordinal))
                {
                    this.ResultText.Text = $"The dialog handed back: {result}";
                }
            }
        }

        private void OnAnswerTextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is TextBox box && box.TryFindParent<ContentDialog>() is { } dialog)
            {
                dialog.SetCurrentValue(ContentDialog.IsPrimaryButtonEnabledProperty, !string.IsNullOrWhiteSpace(box.Text));
            }
        }
    }
}
