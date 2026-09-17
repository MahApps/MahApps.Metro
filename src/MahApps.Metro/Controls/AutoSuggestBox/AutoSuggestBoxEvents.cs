// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows;

namespace MahApps.Metro.Controls
{
    /// <summary>
    /// Says who changed the text of an <see cref="AutoSuggestBox"/>.
    /// </summary>
    public enum AutoSuggestionBoxTextChangeReason
    {
        /// <summary>The user typed.</summary>
        UserInput,

        /// <summary>Something set the text, a binding or code.</summary>
        ProgrammaticChange,

        /// <summary>The user picked one of the suggestions.</summary>
        SuggestionChosen
    }

    public class AutoSuggestBoxTextChangedEventArgs : RoutedEventArgs
    {
        public AutoSuggestBoxTextChangedEventArgs(RoutedEvent routedEvent, object source, AutoSuggestionBoxTextChangeReason reason)
            : base(routedEvent, source)
        {
            this.Reason = reason;
        }

        public AutoSuggestionBoxTextChangeReason Reason { get; }
    }

    public class AutoSuggestBoxSuggestionChosenEventArgs : RoutedEventArgs
    {
        public AutoSuggestBoxSuggestionChosenEventArgs(RoutedEvent routedEvent, object source, object? selectedItem)
            : base(routedEvent, source)
        {
            this.SelectedItem = selectedItem;
        }

        public object? SelectedItem { get; }
    }

    public class AutoSuggestBoxQuerySubmittedEventArgs : RoutedEventArgs
    {
        public AutoSuggestBoxQuerySubmittedEventArgs(RoutedEvent routedEvent, object source, string? queryText, object? chosenSuggestion)
            : base(routedEvent, source)
        {
            this.QueryText = queryText;
            this.ChosenSuggestion = chosenSuggestion;
        }

        public string? QueryText { get; }

        public object? ChosenSuggestion { get; }
    }
}
