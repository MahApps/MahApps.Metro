// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using MahApps.Metro.Controls;
using MetroDemo.Models;

namespace MetroDemo.ExampleViews
{
    /// <summary>
    /// Interaction logic for AutoSuggestBoxExample.xaml
    /// </summary>
    public partial class AutoSuggestBoxExample : UserControl
    {
        public AutoSuggestBoxExample()
        {
            this.InitializeComponent();
        }

        private MainWindowViewModel? ViewModel => this.DataContext as MainWindowViewModel;

        private void ArtistBox_OnTextChanged(object sender, RoutedEventArgs e)
        {
            if (((AutoSuggestBoxTextChangedEventArgs)e).Reason != AutoSuggestionBoxTextChangeReason.UserInput)
            {
                return;
            }

            var artists = this.ViewModel?.Artists ?? new List<Artist>();

            this.ArtistBox.ItemsSource = artists
                                         .Where(artist => artist.Name?.StartsWith(this.ArtistBox.Text, StringComparison.CurrentCultureIgnoreCase) == true)
                                         .Take(10)
                                         .ToList();
        }

        private void ArtistBox_OnQuerySubmitted(object sender, RoutedEventArgs e)
        {
            var args = (AutoSuggestBoxQuerySubmittedEventArgs)e;

            this.Write(args.ChosenSuggestion is Artist artist
                           ? $"artist: {artist.Name}, picked from the list"
                           : $"artist: {args.QueryText}, typed and nothing picked");
        }

        private void AlbumBox_OnTextChanged(object sender, RoutedEventArgs e)
        {
            if (((AutoSuggestBoxTextChangedEventArgs)e).Reason != AutoSuggestionBoxTextChangeReason.UserInput)
            {
                return;
            }

            var albums = this.ViewModel?.Albums ?? new List<Album>();

            this.AlbumBox.ItemsSource = albums
                                        .Select(album => album.Title)
                                        .Where(title => title?.IndexOf(this.AlbumBox.Text, StringComparison.CurrentCultureIgnoreCase) >= 0)
                                        .Take(10)
                                        .ToList();
        }

        private void AlbumBox_OnQuerySubmitted(object sender, RoutedEventArgs e)
        {
            var args = (AutoSuggestBoxQuerySubmittedEventArgs)e;

            this.Write(args.ChosenSuggestion is string title
                           ? $"album: {title}, picked from the list"
                           : $"album: {args.QueryText}, typed and nothing picked");
        }

        private void Write(string line)
        {
            this.Log.Text = this.Log.Text.Length == 0 ? line : $"{this.Log.Text}{Environment.NewLine}{line}";
        }
    }
}
