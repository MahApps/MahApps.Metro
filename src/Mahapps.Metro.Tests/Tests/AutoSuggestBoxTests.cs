// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using MahApps.Metro.Controls;
using MahApps.Metro.Tests.TestHelpers;
using NUnit.Framework;

namespace MahApps.Metro.Tests.Tests
{
    /// <summary>
    /// GH-2625: a box that suggests while the user types. The suggestions come from the application,
    /// so these tests play both parts, the one typing and the one answering.
    /// </summary>
    [TestFixture]
    public class AutoSuggestBoxTests
    {
        private static readonly string[] Planets =
        {
            "Merkur", "Venus", "Erde", "Mars", "Jupiter", "Saturn", "Uranus", "Neptun"
        };

        private TestWindow window = null!;
        private AutoSuggestBox box = null!;
        private TextBox editableTextBox = null!;
        private readonly List<AutoSuggestionBoxTextChangeReason> reasons = new();
        private readonly List<string> queries = new();
        private readonly ObservableCollection<string> suggestions = new();
        private bool desktopWentAway;

        [OneTimeSetUp]
        public async Task OneTimeSetUp()
        {
            this.window = await WindowHelpers.CreateInvisibleWindowAsync<TestWindow>().ConfigureAwait(true);
            this.window.Deactivated += (_, _) => this.desktopWentAway = true;
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            this.window.Close();
        }

        [SetUp]
        public void SetUp()
        {
            this.desktopWentAway = false;
            this.reasons.Clear();
            this.queries.Clear();
            this.suggestions.Clear();

            this.box = new AutoSuggestBox
                       {
                           Width = 300,
                           VerticalAlignment = VerticalAlignment.Top,
                           ItemsSource = this.suggestions
                       };

            // this is the whole contract: the box says the text changed, the application says what to suggest
            this.box.TextChanged += (_, e) =>
                {
                    var args = (AutoSuggestBoxTextChangedEventArgs)e;
                    this.reasons.Add(args.Reason);

                    if (args.Reason != AutoSuggestionBoxTextChangeReason.UserInput)
                    {
                        return;
                    }

                    this.queries.Add(this.box.Text);

                    var hits = Planets.Where(planet => planet.StartsWith(this.box.Text, StringComparison.OrdinalIgnoreCase)).ToList();
                    this.suggestions.Clear();
                    foreach (var hit in hits)
                    {
                        this.suggestions.Add(hit);
                    }
                };

            this.window.Content = this.box;
            this.window.UpdateLayout();
            this.box.ApplyTemplate();
            ClipAssert.Pump();

            this.editableTextBox = this.box.FindChild<TextBox>("PART_EditableTextBox")!;
            Assert.That(this.editableTextBox, Is.Not.Null, "the template should carry the editable text box");
        }

        [TearDown]
        public void TearDown()
        {
            this.window.Content = null;
            ClipAssert.Pump();
        }

        /// <summary>
        /// Puts the user in the box and gives up on the test if that did not work. A build agent hands
        /// the keyboard to one window at a time, and it can take it away again between two steps, so
        /// every step that only means anything with somebody standing in the box says so here.
        /// </summary>
        private void TheUserIsInTheBox()
        {
            this.editableTextBox.Focus();
            Keyboard.Focus(this.editableTextBox);
            ClipAssert.Pump();

            Assume.That(this.box.IsKeyboardFocusWithin, Is.True);
        }

        /// <summary>
        /// And still standing in it, with this window having held the desktop the whole way. A
        /// ComboBox keeps the mouse while its list is up, so a window that loses the foreground
        /// loses the capture as well and the list goes down with it. Measured on a window handed the
        /// foreground right after a keystroke: the text arrives as the user's, the suggestion is
        /// there, the list opens, and the next turn of the message loop takes it down again. Once
        /// the foreground comes back, the window is active again and the box has the keyboard again,
        /// so asking how things stand afterwards finds nothing wrong and the list on the floor. What
        /// has to be asked is whether the desktop went away at all, which is why this counts the
        /// window being deactivated instead of looking at how it sits now. None of it is about the
        /// box, so a test that lost the desktop says nothing instead of saying something wrong.
        /// </summary>
        private void TheDesktopIsStillOurs()
        {
            Assume.That(this.desktopWentAway, Is.False, "the desktop went to another window in the middle of the test");
            Assume.That(this.box.IsKeyboardFocusWithin, Is.True);
        }

        private void Type(string text)
        {
            this.TheUserIsInTheBox();

            this.editableTextBox.Text = text;
            this.editableTextBox.CaretIndex = text.Length;
            ClipAssert.Pump();

            this.TheDesktopIsStillOurs();
        }

        /// <summary>
        /// One key at a time, the way the keyboard delivers them: the text box inserts the character
        /// itself and moves the caret, which is what makes a marked text lose what was typed.
        /// </summary>
        private void TypeKeys(string text)
        {
            this.TheUserIsInTheBox();

            foreach (var character in text)
            {
                var composition = new TextComposition(InputManager.Current, this.editableTextBox, character.ToString());

                this.editableTextBox.RaiseEvent(new TextCompositionEventArgs(Keyboard.PrimaryDevice, composition)
                                                {
                                                    RoutedEvent = TextCompositionManager.TextInputEvent
                                                });

                ClipAssert.Pump();
            }

            this.TheDesktopIsStillOurs();
        }

        /// <summary>
        /// A key press the way the input system delivers it: the tunnelling one first, and the bubbling
        /// one only if nobody took the tunnelling one. A ComboBox does its key work in the tunnelling
        /// phase, so a test that skips it tests a path no user takes.
        /// </summary>
        private void Press(Key key)
        {
            var source = PresentationSource.FromVisual(this.box);

            var preview = new KeyEventArgs(Keyboard.PrimaryDevice, source, 0, key) { RoutedEvent = Keyboard.PreviewKeyDownEvent };
            this.editableTextBox.RaiseEvent(preview);

            if (!preview.Handled)
            {
                this.editableTextBox.RaiseEvent(new KeyEventArgs(Keyboard.PrimaryDevice, source, 0, key) { RoutedEvent = Keyboard.KeyDownEvent });
            }

            ClipAssert.Pump();

            this.TheDesktopIsStillOurs();
        }

        private void Click(string suggestion)
        {
            // a list on the floor has no rows, so this one asks before as well as after
            this.TheDesktopIsStillOurs();

            var container = (ComboBoxItem)this.box.ItemContainerGenerator.ContainerFromItem(suggestion);
            Assert.That(container, Is.Not.Null, "the open list should have a container for every suggestion");

            // the tunnelling one runs through the popup, where the box listens, the direct one is what
            // the item itself answers with a selection
            foreach (var routedEvent in new[] { UIElement.PreviewMouseUpEvent, UIElement.MouseLeftButtonUpEvent })
            {
                container.RaiseEvent(new MouseButtonEventArgs(Mouse.PrimaryDevice, 0, MouseButton.Left)
                                     {
                                         RoutedEvent = routedEvent,
                                         Source = container
                                     });
            }

            ClipAssert.Pump();

            this.TheDesktopIsStillOurs();
        }

        /// <summary>
        /// The clear button, pressed the way a click presses it: the command it carries goes to the box.
        /// </summary>
        private void ClickTheClearButton()
        {
            TextBoxHelper.SetClearTextButton(this.box, true);
            this.window.UpdateLayout();
            ClipAssert.Pump();

            var button = this.box.FindChild<Button>("PART_ClearText")!;
            Assert.That(button, Is.Not.Null, "the template should carry the clear button");

            ((IInvokeProvider)new ButtonAutomationPeer(button)).Invoke();
            ClipAssert.Pump();
            ClipAssert.Pump();
        }

        [Test]
        [Description("Typing reaches the application and the suggestions come up.")]
        public void TypingAsksTheApplicationForSuggestions()
        {
            this.Type("Ma");

            Assert.Multiple(() =>
                {
                    Assert.That(this.box.Text, Is.EqualTo("Ma"), "the text the user typed");
                    Assert.That(this.reasons, Does.Contain(AutoSuggestionBoxTextChangeReason.UserInput));
                    Assert.That(this.suggestions, Is.EquivalentTo(new[] { "Mars" }));
                    Assert.That(this.box.IsDropDownOpen, Is.True, "and the list shows itself");
                });
        }

        [Test]
        [Description("The typed text survives the application swapping the suggestions underneath it.")]
        public void TheTypedTextSurvivesTheNewSuggestions()
        {
            this.Type("Ju");

            Assert.Multiple(() =>
                {
                    Assert.That(this.box.Text, Is.EqualTo("Ju"));
                    Assert.That(this.editableTextBox.Text, Is.EqualTo("Ju"));
                    Assert.That(this.box.SelectedItem, Is.Null, "nothing is chosen until the user chooses");
                });
        }

        [Test]
        [Description("Text that matches nothing stays put, and nothing is all the list would have to show.")]
        public void TextWithoutAnySuggestionStaysPut()
        {
            this.Type("xyz");

            Assert.Multiple(() =>
                {
                    Assert.That(this.box.Text, Is.EqualTo("xyz"));
                    Assert.That(this.suggestions, Is.Empty);
                    Assert.That(this.box.IsDropDownOpen, Is.False, "an empty list is a sliver of border, not an answer");
                });
        }

        [Test]
        [Description("Suggestions an application goes looking for arrive late, and the list comes up then.")]
        public void SuggestionsThatArriveLateStillShowUp()
        {
            this.Type("Ma");
            this.suggestions.Clear();
            ClipAssert.Pump();

            Assert.That(this.box.IsDropDownOpen, Is.False, "nothing to show yet");

            // the answer arrives while the user is still standing in the box, which is the only case
            // where a list has any business coming up on its own
            this.TheUserIsInTheBox();

            this.suggestions.Add("Mars");
            ClipAssert.Pump();

            this.TheDesktopIsStillOurs();

            Assert.That(this.box.IsDropDownOpen, Is.True);
        }

        [Test]
        [Description("Every key the user presses lands in the box, the first one included.")]
        public void EveryKeystrokeLandsInTheBox()
        {
            this.TypeKeys("Mar");

            Assert.Multiple(() =>
                {
                    Assert.That(this.editableTextBox.Text, Is.EqualTo("Mar"), "a ComboBox marks the whole text when its list opens");
                    Assert.That(this.box.Text, Is.EqualTo("Mar"));
                    Assert.That(this.editableTextBox.CaretIndex, Is.EqualTo(3), "and the caret belongs behind what was typed");
                    Assert.That(this.editableTextBox.SelectionLength, Is.Zero, "with nothing left marked for the next key to overwrite");
                    Assert.That(this.suggestions, Is.EquivalentTo(new[] { "Mars" }));
                });
        }

        [Test]
        [Description("Text set from code is not somebody typing, so it asks for no suggestions and opens nothing.")]
        public void TextSetFromCodeIsNotUserInput()
        {
            this.box.SetCurrentValue(AutoSuggestBox.TextProperty, "Venus");
            ClipAssert.Pump();

            Assert.Multiple(() =>
                {
                    Assert.That(this.reasons, Does.Contain(AutoSuggestionBoxTextChangeReason.ProgrammaticChange));
                    Assert.That(this.reasons, Does.Not.Contain(AutoSuggestionBoxTextChangeReason.UserInput));
                    Assert.That(this.box.IsDropDownOpen, Is.False);
                });
        }

        [Test]
        [Description("An emptied box has nothing left to suggest, so the list goes away.")]
        public void AnEmptyBoxClosesTheList()
        {
            this.Type("Ma");
            Assert.That(this.box.IsDropDownOpen, Is.True);

            this.Type(string.Empty);

            Assert.That(this.box.IsDropDownOpen, Is.False);
        }

        [Test]
        [Description("Picking a suggestion writes it into the box and tells the application.")]
        public void PickingASuggestionFillsTheBox()
        {
            object? chosen = null;
            this.box.SuggestionChosen += (_, e) => chosen = ((AutoSuggestBoxSuggestionChosenEventArgs)e).SelectedItem;

            this.Type("Ne");
            this.box.SelectedItem = "Neptun";
            ClipAssert.Pump();

            Assert.Multiple(() =>
                {
                    Assert.That(chosen, Is.EqualTo("Neptun"));
                    Assert.That(this.box.Text, Is.EqualTo("Neptun"));
                    Assert.That(this.editableTextBox.Text, Is.EqualTo("Neptun"));
                });
        }

        [Test]
        [Description("The arrow keys walk the list, the box takes what they land on, and enter submits it.")]
        public void TheArrowKeysWalkTheListAndEnterSubmits()
        {
            AutoSuggestBoxQuerySubmittedEventArgs? submitted = null;
            this.box.QuerySubmitted += (_, e) => submitted = (AutoSuggestBoxQuerySubmittedEventArgs)e;

            this.TypeKeys("Ma");
            this.Press(Key.Down);

            Assert.Multiple(() =>
                {
                    Assert.That(this.box.SelectedItem, Is.EqualTo("Mars"), "walking onto a suggestion takes it");
                    Assert.That(this.box.Text, Is.EqualTo("Mars"));
                    Assert.That(this.editableTextBox.SelectionLength, Is.EqualTo(4), "and marks it, so typing on replaces it");
                });

            this.Press(Key.Enter);

            Assert.Multiple(() =>
                {
                    Assert.That(submitted, Is.Not.Null, "enter has to reach the application, and a ComboBox takes its keys in the tunnelling phase");
                    Assert.That(submitted!.ChosenSuggestion, Is.EqualTo("Mars"));
                    Assert.That(submitted.QueryText, Is.EqualTo("Mars"));
                    Assert.That(this.box.IsDropDownOpen, Is.False);
                });
        }

        [Test]
        [Description("Typing on after walking the list starts over, the way a marked completion does.")]
        public void TypingOnAfterWalkingTheListReplacesWhatWasTaken()
        {
            this.TypeKeys("Ma");
            this.Press(Key.Down);
            this.TypeKeys("x");

            Assert.Multiple(() =>
                {
                    Assert.That(this.box.Text, Is.EqualTo("x"));
                    Assert.That(this.box.SelectedItem, Is.Null, "and the box is back to a plain query");
                });
        }

        [Test]
        [Description("Enter submits what stands in the box, together with whatever was picked.")]
        public void EnterSubmitsTheQuery()
        {
            AutoSuggestBoxQuerySubmittedEventArgs? submitted = null;
            this.box.QuerySubmitted += (_, e) => submitted = (AutoSuggestBoxQuerySubmittedEventArgs)e;

            this.TypeKeys("Sat");
            this.Press(Key.Enter);

            Assert.Multiple(() =>
                {
                    Assert.That(submitted, Is.Not.Null);
                    Assert.That(submitted!.QueryText, Is.EqualTo("Sat"));
                    Assert.That(submitted.ChosenSuggestion, Is.Null, "nobody picked anything, the text is the whole query");
                    Assert.That(this.box.IsDropDownOpen, Is.False, "and the list goes away");
                });
        }

        [Test]
        [Description("Clicking a suggestion is the user saying they are done.")]
        public void ClickingASuggestionSubmitsIt()
        {
            AutoSuggestBoxQuerySubmittedEventArgs? submitted = null;
            this.box.QuerySubmitted += (_, e) => submitted = (AutoSuggestBoxQuerySubmittedEventArgs)e;

            this.Type("Ur");
            this.Click("Uranus");

            Assert.Multiple(() =>
                {
                    Assert.That(submitted, Is.Not.Null);
                    Assert.That(submitted!.ChosenSuggestion, Is.EqualTo("Uranus"));
                    Assert.That(submitted.QueryText, Is.EqualTo("Uranus"));
                    Assert.That(this.box.IsDropDownOpen, Is.False);
                });
        }

        [Test]
        [Description("Sending the list away with escape submits nothing.")]
        public void EscapeSubmitsNothing()
        {
            var submissions = 0;
            this.box.QuerySubmitted += (_, _) => submissions++;

            this.TypeKeys("Ma");
            this.Press(Key.Escape);

            Assert.Multiple(() =>
                {
                    Assert.That(this.box.IsDropDownOpen, Is.False, "the list should be gone");
                    Assert.That(submissions, Is.Zero);
                });
        }

        [Test]
        [Description("The MahApps text box outfit is there, and so is the behaviour a suggestion box needs.")]
        public void TheBoxComesDressedAndSetUp()
        {
            TextBoxHelper.SetWatermark(this.box, "Planet");
            TextBoxHelper.SetClearTextButton(this.box, true);
            this.window.UpdateLayout();
            ClipAssert.Pump();

            Assert.Multiple(() =>
                {
                    Assert.That(this.box.FindChild<Button>("PART_ClearText"), Is.Not.Null, "the button spot is in the template");
                    Assert.That(TextBoxHelper.GetWatermark(this.editableTextBox), Is.EqualTo("Planet"), "and the watermark reaches the text box");
                    Assert.That(this.box.IsEditable, Is.True, "a suggestion box is editable from the start");
                    Assert.That(this.box.IsTextSearchEnabled, Is.False, "and does not let WPF complete the text behind our back");
                    Assert.That(this.box.StaysOpenOnEdit, Is.True, "the list stays up while the user keeps typing");
                });
        }

        [Test]
        [Description("Clearing the box after a pick empties it and takes the list with it.")]
        public void ClearingThePickedSuggestionTakesTheListWithIt()
        {
            this.TypeKeys("Ma");
            this.Press(Key.Down);
            this.Press(Key.Enter);

            this.queries.Clear();
            this.reasons.Clear();
            this.ClickTheClearButton();

            Assert.Multiple(() =>
                {
                    Assert.That(this.box.Text, Is.Empty, "the button is there to empty the box");
                    Assert.That(this.box.SelectedItem, Is.Null, "and nothing is picked any more");
                    Assert.That(this.queries, Does.Not.Contain("Mars"), "a suggestion that leaves the list must not be handed back to the application as something the user typed");
                    Assert.That(this.box.IsDropDownOpen, Is.False, "an empty box has nothing to suggest");
                });
        }

        [Test]
        [Description("The list stays under the box when the box grows, the way a floating watermark grows it.")]
        public void TheListFollowsTheBoxWhenItGrows()
        {
            // This one needs real coordinates. A window parked off screen cannot have its popup where
            // it belongs, because Windows keeps a popup on the desktop, and then nothing here compares.
            var parkedLeft = this.window.Left;
            var parkedTop = this.window.Top;
            this.window.Left = 100;
            this.window.Top = 100;
            this.window.UpdateLayout();
            ClipAssert.Pump();

            try
            {
                this.TypeKeys("Ma");
                Assume.That(this.box.IsDropDownOpen, Is.True);

                var popup = this.box.FindChild<Popup>("PART_Popup")!;
                var list = (FrameworkElement)popup.Child;

                Assume.That(list.PointToScreen(default).Y, Is.GreaterThanOrEqualTo(this.box.PointToScreen(new Point(0, this.box.ActualHeight)).Y), "the list should start under the box to begin with");

                // a floating watermark takes a fifth of a second to grow into place, and by then the
                // first keystroke has the list up: the box grows on over it
                this.box.SetCurrentValue(FrameworkElement.HeightProperty, this.box.ActualHeight + 20d);
                this.window.UpdateLayout();
                ClipAssert.Pump();

                var boxBottom = this.box.PointToScreen(new Point(0, this.box.ActualHeight)).Y;
                var listTop = list.PointToScreen(default).Y;

                Assert.That(listTop, Is.GreaterThanOrEqualTo(boxBottom), $"the list belongs under the box: it starts at {listTop} and the box now ends at {boxBottom}");
            }
            finally
            {
                this.window.Left = parkedLeft;
                this.window.Top = parkedTop;
            }
        }

        [Test]
        [Description("A client is told what this is instead of finding a ComboBox.")]
        public void TheBoxSaysWhatItIs()
        {
            var peer = UIElementAutomationPeer.CreatePeerForElement(this.box);

            Assert.That(peer, Is.Not.Null);
            Assert.That(peer!.GetClassName(), Is.EqualTo("AutoSuggestBox"));
        }
    }
}
