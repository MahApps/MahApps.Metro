// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using MahApps.Metro.Automation.Peers;
using MahApps.Metro.ValueBoxes;

namespace MahApps.Metro.Controls
{
    /// <summary>
    /// A text box that shows suggestions while the user types.
    /// </summary>
    /// <remarks>
    /// Which suggestions those are is the application's business. The box says that the text changed
    /// and who changed it, the application fills <see cref="ItemsControl.ItemsSource"/> with whatever
    /// fits. That leaves matching, sorting and the question of what counts as a hit where the data is,
    /// instead of in a pile of filter modes here.
    /// </remarks>
    [TemplatePart(Name = PART_EditableTextBox, Type = typeof(TextBox))]
    [TemplatePart(Name = PART_Popup, Type = typeof(Popup))]
    public class AutoSuggestBox : ComboBox
    {
        // ReSharper disable InconsistentNaming
        private const string PART_EditableTextBox = "PART_EditableTextBox";
        private const string PART_Popup = "PART_Popup";
        // ReSharper restore InconsistentNaming

        private readonly MouseButtonEventHandler suggestionClickHandler;

        private Popup? popup;
        private TextBox? editableTextBox;
        private bool weAreSettingTheText;
        private bool aSuggestionWasClicked;

        public AutoSuggestBox()
        {
            this.suggestionClickHandler = this.OnSuggestionClicked;
        }

        static AutoSuggestBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(AutoSuggestBox), new FrameworkPropertyMetadata(typeof(AutoSuggestBox)));

            // a suggestion box is a text box with a list behind it, so it is editable from the start,
            // and the text search WPF does on its own would pick an item while the user is still typing
            IsEditableProperty.OverrideMetadata(typeof(AutoSuggestBox), new FrameworkPropertyMetadata(BooleanBoxes.TrueBox));
            IsTextSearchEnabledProperty.OverrideMetadata(typeof(AutoSuggestBox), new FrameworkPropertyMetadata(BooleanBoxes.FalseBox));
            StaysOpenOnEditProperty.OverrideMetadata(typeof(AutoSuggestBox), new FrameworkPropertyMetadata(BooleanBoxes.TrueBox));

            TextProperty.OverrideMetadata(typeof(AutoSuggestBox),
                                          new FrameworkPropertyMetadata(string.Empty,
                                                                        FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.Journal,
                                                                        OnTextChanged));
        }

        /// <summary>Identifies the <see cref="TextChanged"/> routed event.</summary>
        public static readonly RoutedEvent TextChangedEvent
            = EventManager.RegisterRoutedEvent(nameof(TextChanged),
                                               RoutingStrategy.Bubble,
                                               typeof(RoutedEventHandler),
                                               typeof(AutoSuggestBox));

        /// <summary>
        /// Occurs after the text changed. The <see cref="AutoSuggestBoxTextChangedEventArgs.Reason"/>
        /// says who changed it, and only <see cref="AutoSuggestionBoxTextChangeReason.UserInput"/> is
        /// worth new suggestions.
        /// </summary>
        public event RoutedEventHandler TextChanged
        {
            add => this.AddHandler(TextChangedEvent, value);
            remove => this.RemoveHandler(TextChangedEvent, value);
        }

        /// <summary>Identifies the <see cref="SuggestionChosen"/> routed event.</summary>
        public static readonly RoutedEvent SuggestionChosenEvent
            = EventManager.RegisterRoutedEvent(nameof(SuggestionChosen),
                                               RoutingStrategy.Bubble,
                                               typeof(RoutedEventHandler),
                                               typeof(AutoSuggestBox));

        /// <summary>
        /// Occurs when the user lands on one of the suggestions, by arrow key or by click.
        /// </summary>
        public event RoutedEventHandler SuggestionChosen
        {
            add => this.AddHandler(SuggestionChosenEvent, value);
            remove => this.RemoveHandler(SuggestionChosenEvent, value);
        }

        /// <summary>Identifies the <see cref="QuerySubmitted"/> routed event.</summary>
        public static readonly RoutedEvent QuerySubmittedEvent
            = EventManager.RegisterRoutedEvent(nameof(QuerySubmitted),
                                               RoutingStrategy.Bubble,
                                               typeof(RoutedEventHandler),
                                               typeof(AutoSuggestBox));

        /// <summary>
        /// Occurs when the user is done, by pressing enter or by clicking a suggestion.
        /// </summary>
        public event RoutedEventHandler QuerySubmitted
        {
            add => this.AddHandler(QuerySubmittedEvent, value);
            remove => this.RemoveHandler(QuerySubmittedEvent, value);
        }

        private static void OnTextChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            var autoSuggestBox = (AutoSuggestBox)dependencyObject;

            var reason = autoSuggestBox.weAreSettingTheText
                ? AutoSuggestionBoxTextChangeReason.SuggestionChosen
                : autoSuggestBox.IsKeyboardFocusWithin
                    ? AutoSuggestionBoxTextChangeReason.UserInput
                    : AutoSuggestionBoxTextChangeReason.ProgrammaticChange;

            autoSuggestBox.RaiseEvent(new AutoSuggestBoxTextChangedEventArgs(TextChangedEvent, autoSuggestBox, reason));

            autoSuggestBox.UpdateTheList(reason == AutoSuggestionBoxTextChangeReason.UserInput);
        }

        /// <inheritdoc />
        protected override void OnItemsChanged(NotifyCollectionChangedEventArgs e)
        {
            base.OnItemsChanged(e);

            // suggestions do not have to be there by the time the text changes. An application that goes
            // asking somewhere for them hands them over later, and the list comes up then.
            this.UpdateTheList(this.IsKeyboardFocusWithin);
        }

        /// <inheritdoc />
        protected override void ClearContainerForItemOverride(DependencyObject element, object item)
        {
            // A suggestion that leaves the list takes its row with it, and emptying that row is where a
            // ComboBox notices that what was selected is gone and writes its text into the box once more.
            // Reported as the user typing, that write sends the application looking for the suggestion it
            // has just dropped, which puts it back into the list and brings the list up over an empty box.
            var alreadySettingTheText = this.weAreSettingTheText;
            this.weAreSettingTheText = true;
            try
            {
                base.ClearContainerForItemOverride(element, item);
            }
            finally
            {
                this.weAreSettingTheText = alreadySettingTheText;
            }
        }

        /// <summary>
        /// The list is up while there is something to show for what stands in the box. It comes up for
        /// the user's own typing, and goes away whenever there is nothing left to show, whoever wrote it.
        /// </summary>
        private void UpdateTheList(bool mayOpen)
        {
            // an empty box has nothing to suggest, and a list with nothing in it is a sliver of border
            var worthShowing = !string.IsNullOrEmpty(this.Text) && this.HasItems;

            if (worthShowing == this.IsDropDownOpen || (worthShowing && !mayOpen))
            {
                return;
            }

            // A ComboBox marks the whole text when its list opens, which is what a click on the arrow
            // should do and the last thing wanted here: the user is mid-word, and the next key they press
            // would replace everything they have typed so far. So the caret is put back where it was.
            var selectionStart = this.editableTextBox?.SelectionStart;
            var selectionLength = this.editableTextBox?.SelectionLength;

            this.SetCurrentValue(IsDropDownOpenProperty, BooleanBoxes.Box(worthShowing));

            if (this.editableTextBox is not null && selectionStart.HasValue)
            {
                this.editableTextBox.SelectionStart = selectionStart.Value;
                this.editableTextBox.SelectionLength = selectionLength ?? 0;
            }
        }

        /// <inheritdoc />
        protected override void OnSelectionChanged(SelectionChangedEventArgs e)
        {
            // a ComboBox writes the picked item into the text right here, and that write is not the user
            // typing: reported as such it would send the application filtering for the very item it just
            // offered, which empties the list, drops the selection and leaves the box blank
            var alreadySettingTheText = this.weAreSettingTheText;
            this.weAreSettingTheText = true;
            try
            {
                base.OnSelectionChanged(e);
            }
            finally
            {
                this.weAreSettingTheText = alreadySettingTheText;
            }

            if (e.AddedItems.Count == 0 || this.SelectedItem is null)
            {
                return;
            }

            this.RaiseEvent(new AutoSuggestBoxSuggestionChosenEventArgs(SuggestionChosenEvent, this, this.SelectedItem));
        }

        /// <inheritdoc />
        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            // A ComboBox does its key work here for everything that comes out of the editable text box,
            // which is where a suggestion box gets its keys, and it is where enter closes the list and
            // takes over whatever the arrow keys had walked to. So the base goes first and the query is
            // submitted with what it picked.
            base.OnPreviewKeyDown(e);

            if (ReferenceEquals(e.OriginalSource, this.editableTextBox))
            {
                this.SubmitOnEnter(e);
            }
        }

        /// <inheritdoc />
        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            this.SubmitOnEnter(e);
        }

        private void SubmitOnEnter(KeyEventArgs e)
        {
            if (e.Key != Key.Enter)
            {
                return;
            }

            this.Submit(this.SelectedItem);
            e.Handled = true;
        }

        /// <inheritdoc />
        public override void OnApplyTemplate()
        {
            this.popup?.Child?.RemoveHandler(PreviewMouseUpEvent, this.suggestionClickHandler);

            base.OnApplyTemplate();

            this.editableTextBox = this.GetTemplateChild(PART_EditableTextBox) as TextBox;
            this.popup = this.GetTemplateChild(PART_Popup) as Popup;

            // the list sits in a window of its own, out of reach of anything routed through this control,
            // and the item handles the click itself and closes the list over it. So the note that a click
            // happened is taken inside the popup, on the way down, before the item gets to it.
            this.popup?.Child?.AddHandler(PreviewMouseUpEvent, this.suggestionClickHandler, true);
        }

        private void OnSuggestionClicked(object sender, MouseButtonEventArgs e)
        {
            this.aSuggestionWasClicked = e.ChangedButton == MouseButton.Left
                                         && e.OriginalSource is DependencyObject clicked
                                         && (clicked is ComboBoxItem || clicked.TryFindParent<ComboBoxItem>() is not null);
        }

        /// <inheritdoc />
        protected override void OnDropDownOpened(EventArgs e)
        {
            this.aSuggestionWasClicked = false;

            base.OnDropDownOpened(e);
        }

        /// <inheritdoc />
        protected override void OnDropDownClosed(EventArgs e)
        {
            base.OnDropDownClosed(e);

            // clicking a suggestion is the user saying they are done; closing the list with escape is not
            if (this.aSuggestionWasClicked)
            {
                this.aSuggestionWasClicked = false;
                this.Submit(this.SelectedItem);
            }
        }

        /// <inheritdoc />
        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new AutoSuggestBoxAutomationPeer(this);
        }

        private void Submit(object? chosenSuggestion)
        {
            this.SetCurrentValue(IsDropDownOpenProperty, BooleanBoxes.FalseBox);
            this.RaiseEvent(new AutoSuggestBoxQuerySubmittedEventArgs(QuerySubmittedEvent, this, this.Text, chosenSuggestion));
        }
    }
}
