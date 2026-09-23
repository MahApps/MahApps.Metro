// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using MahApps.Metro.Controls;
using MahApps.Metro.Gallery.Controls;
using MahApps.Metro.Gallery.Core;

namespace MahApps.Metro.Gallery.Pages
{
    /// <summary>
    /// Interaction logic for AutoSuggestBoxPage.xaml
    /// </summary>
    public partial class AutoSuggestBoxPage : UserControl
    {
        public AutoSuggestBoxPage()
        {
            this.InitializeComponent();

            Options(this.SuggestExample, this.Suggest);
            Options(this.Win10Example, this.Win10);
            Options(this.WinUIExample, this.WinUI);
        }

        private static void Options(ControlExample example, AutoSuggestBox box)
        {
            example.Watch(box, IsEnabledProperty);
            example.Watch("Attached",
                          box,
                          TextBoxHelper.WatermarkProperty,
                          TextBoxHelper.ClearTextButtonProperty,
                          TextBoxHelper.UseFloatingWatermarkProperty);
            example.Watch("Layout", box, WidthProperty);
        }

        private void OnTextChanged(object sender, RoutedEventArgs e)
        {
            // only what the reader typed, otherwise picking a suggestion would ask for suggestions
            // about the suggestion
            if (((AutoSuggestBoxTextChangedEventArgs)e).Reason != AutoSuggestionBoxTextChangeReason.UserInput)
            {
                return;
            }

            var box = (AutoSuggestBox)sender;

            // the pages of this gallery are the list, so the sample has something to say without
            // carrying data of its own around
            box.ItemsSource = GalleryPages.All
                                          .Select(page => page.Title)
                                          .Where(title => title.IndexOf(box.Text, StringComparison.CurrentCultureIgnoreCase) >= 0)
                                          .Take(8)
                                          .ToList();
        }
    }
}
