// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using MahApps.Metro.Controls;
using MahApps.Metro.Tests.TestHelpers;
using MahApps.Metro.Tests.Views;
using NUnit.Framework;

namespace MahApps.Metro.Tests.Tests
{
    /// <summary>
    /// The close button of a tab asks before the tab goes. A command bound to it says no through
    /// <see cref="ICommand.CanExecute"/>, a handler of the closing event says no through
    /// <see cref="System.ComponentModel.CancelEventArgs.Cancel"/>, and a handler that needs a
    /// moment for that answer asks for a deferral first. See GH-4078 and GH-3714.
    /// </summary>
    [TestFixture]
    public class TabItemClosingTests : WindowTestFixture<TestWindow>
    {
        [Test]
        [Description("A command on the item that says no keeps the tab where it is, see GH-4078.")]
        public void ACommandThatSaysNoKeepsTheTab()
        {
            var command = new Probe();
            var control = TwoTabs(out var item, command);

            this.ShowAndClose(control, item, command);

            Assert.Multiple(() =>
                {
                    Assert.That(control.Items, Has.Count.EqualTo(2), "the tab should still be there");
                    Assert.That(command.AskedAbout, Is.SameAs(item), "the command should have been asked about the item whose button was clicked");
                    Assert.That(command.Ran, Is.Zero, "and a command that says no should not have run");
                });
        }

        [Test]
        [Description("The same holds for a MetroTabItem in a plain TabControl, which takes the other way through the action.")]
        public void ACommandThatSaysNoKeepsTheTabOfAPlainTabControl()
        {
            var command = new Probe();
            var control = new TabControl();
            var item = Tab("first", command);
            control.Items.Add(item);
            control.Items.Add(Tab("second"));

            this.ShowAndClose(control, item, command);

            Assert.That(control.Items, Has.Count.EqualTo(2));
        }

        [Test]
        [Description("And for a control on an ItemsSource, where closing means taking the item out of that collection.")]
        public void ACommandThatSaysNoKeepsTheItemInTheSource()
        {
            var command = new Probe();
            var source = new ObservableCollection<string> { "first", "second" };
            var control = new MetroTabControl
                          {
                              ItemsSource = source,
                              ItemContainerStyle = new Style(typeof(MetroTabItem))
                                                   {
                                                       Setters =
                                                       {
                                                           new Setter(MetroTabItem.CloseButtonEnabledProperty, true),
                                                           new Setter(MetroTabItem.CloseTabCommandProperty, command)
                                                       }
                                                   }
                          };

            this.window.Show(control);

            var item = control.ItemContainerGenerator.ContainerFromIndex(0) as MetroTabItem;
            Assert.That(item, Is.Not.Null, "the control should have made a container for the first item");

            command.Allow = false;
            Click(item!);

            Assert.That(source, Has.Count.EqualTo(2));
        }

        [Test]
        [Description("A command that says yes runs, is handed the item it is about, and the tab goes.")]
        public void ACommandThatSaysYesLetsTheTabGo()
        {
            var command = new Probe();
            var control = TwoTabs(out var item, command);

            this.ShowAndClose(control, item);

            Assert.Multiple(() =>
                {
                    Assert.That(command.Ran, Is.EqualTo(1), "the command should have run");
                    Assert.That(command.RanWith, Is.SameAs(item), "with the item whose button was clicked");
                    Assert.That(control.Items, Has.Count.EqualTo(1), "and the tab should be gone");
                });
        }

        [Test]
        [Description("A command that says its answer turned leaves the button disabled, so the click never arrives at all.")]
        public void ACommandThatAnnouncesItsNoDisablesTheButton()
        {
            var command = new Probe();
            var control = TwoTabs(out var item, command);

            this.window.Show(control);

            command.Allow = false;
            command.SayTheAnswerTurned();
            this.window.Settle();

            var button = item.FindChild<Button>("PART_CloseButton");
            Assert.That(button, Is.Not.Null);

            Click(item);

            Assert.Multiple(() =>
                {
                    Assert.That(button!.IsEnabled, Is.False, "the close button should be disabled");
                    Assert.That(control.Items, Has.Count.EqualTo(2), "and the tab should still be there");
                });
        }

        [Test]
        [Description("A handler of the closing event that cancels keeps the tab.")]
        public void AHandlerThatCancelsKeepsTheTab()
        {
            var asked = 0;
            var control = TwoTabs(out var item);
            control.TabItemClosing += (_, e) =>
                {
                    asked++;
                    e.Cancel = true;
                };

            this.ShowAndClose(control, item);

            Assert.Multiple(() =>
                {
                    Assert.That(asked, Is.EqualTo(1), "the handler should have been asked");
                    Assert.That(control.Items, Has.Count.EqualTo(2), "and its answer should have been taken");
                });
        }

        [Test]
        [Description("The event under its older name is still raised and can still cancel.")]
        public void TheOlderEventIsStillAsked()
        {
            var asked = 0;
            var control = TwoTabs(out var item);
            control.TabItemClosingEvent += (_, e) =>
                {
                    asked++;
                    e.Cancel = true;
                };

            this.ShowAndClose(control, item);

            Assert.Multiple(() =>
                {
                    Assert.That(asked, Is.EqualTo(1));
                    Assert.That(control.Items, Has.Count.EqualTo(2));
                });
        }

        [Test]
        [Description("A cancelling handler is asked before the command of the control runs, so the command stays out of it.")]
        public void AHandlerThatCancelsStopsTheCommandOfTheControl()
        {
            var command = new Probe();
            var control = TwoTabs(out var item);
            control.CloseTabCommand = command;
            control.TabItemClosing += (_, e) => e.Cancel = true;

            this.ShowAndClose(control, item);

            Assert.Multiple(() =>
                {
                    Assert.That(command.Ran, Is.Zero, "the command should not have run");
                    Assert.That(control.Items, Has.Count.EqualTo(2), "and the tab should still be there");
                });
        }

        [Test]
        [Description("With nobody cancelling, the command of the control runs and is left to do the removing itself.")]
        public void TheCommandOfTheControlRunsWhenNobodyCancels()
        {
            var command = new Probe();
            var control = TwoTabs(out var item);
            control.CloseTabCommand = command;

            this.ShowAndClose(control, item);

            Assert.Multiple(() =>
                {
                    Assert.That(command.Ran, Is.EqualTo(1), "the command should have run");
                    Assert.That(control.Items, Has.Count.EqualTo(2), "and the removing is the business of whoever handles it");
                });
        }

        [Test]
        [Description("A handler that asked for a deferral holds the closing until it is done, and then the tab goes.")]
        public void ADeferralHoldsTheClosingUntilItIsDone()
        {
            var letGo = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
            var control = TwoTabs(out var item);
            control.TabItemClosing += async (_, e) =>
                {
                    using var deferral = e.GetDeferral();
                    e.Cancel = !await letGo.Task.ConfigureAwait(true);
                };

            this.ShowAndClose(control, item);

            Assert.That(control.Items, Has.Count.EqualTo(2), "nothing should go while the handler is still thinking");

            letGo.SetResult(true);
            ClipAssert.PumpUntil(() => control.Items.Count == 1);

            Assert.That(control.Items, Has.Count.EqualTo(1), "and the tab should go once the handler is done");
        }

        [Test]
        [Description("A handler that says no after its work keeps the tab, which is what a blocking call could never do.")]
        public void ADeferralThatSaysNoAfterItsWorkKeepsTheTab()
        {
            var letGo = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
            var done = false;
            var control = TwoTabs(out var item);
            control.TabItemClosing += async (_, e) =>
                {
                    using var deferral = e.GetDeferral();
                    e.Cancel = !await letGo.Task.ConfigureAwait(true);
                    done = true;
                };

            this.ShowAndClose(control, item);

            letGo.SetResult(false);
            ClipAssert.PumpUntil(() => done);
            ClipAssert.Pump();

            Assert.That(control.Items, Has.Count.EqualTo(2));
        }

        /// <summary>
        /// A control of two closable tabs, the first of which is the one every test here clicks.
        /// </summary>
        private static MetroTabControl TwoTabs(out MetroTabItem first, ICommand? closeTabCommand = null)
        {
            var control = new MetroTabControl();
            first = Tab("first", closeTabCommand);
            control.Items.Add(first);
            control.Items.Add(Tab("second"));

            return control;
        }

        private static MetroTabItem Tab(string header, ICommand? closeTabCommand = null)
        {
            return new MetroTabItem
                   {
                       Header = header,
                       CloseButtonEnabled = true,
                       CloseTabCommand = closeTabCommand,
                       Content = new TextBlock { Text = "42" }
                   };
        }

        private void ShowAndClose(TabControl control, MetroTabItem item, Probe? commandThatChangesItsMind = null)
        {
            this.window.Show(control);

            // the button was enabled while the command still said yes, and the answer turns
            // without the command telling anyone, the way a dirty flag does
            if (commandThatChangesItsMind is not null)
            {
                commandThatChangesItsMind.Allow = false;
            }

            Click(item);
        }

        private static void Click(MetroTabItem item)
        {
            var button = item.FindChild<Button>("PART_CloseButton");
            Assert.That(button, Is.Not.Null, "the close button should be in the template of the item");

            button!.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent, button));
            ClipAssert.Pump();
        }

        /// <summary>
        /// A command that answers what the test tells it to and only says that its answer turned
        /// when the test asks it to, which is how a command that stays quiet about it gets tested.
        /// </summary>
        private sealed class Probe : ICommand
        {
            public event EventHandler? CanExecuteChanged;

            public bool Allow { get; set; } = true;

            public int Ran { get; private set; }

            /// <summary>
            /// Gets what the command was last asked about.
            /// </summary>
            public object? AskedAbout { get; private set; }

            /// <summary>
            /// Gets what the command was last told to run with.
            /// </summary>
            public object? RanWith { get; private set; }

            public bool CanExecute(object? parameter)
            {
                this.AskedAbout = parameter;

                return this.Allow;
            }

            public void Execute(object? parameter)
            {
                this.RanWith = parameter;
                this.Ran++;
            }

            /// <summary>
            /// Says that the answer turned, the way a well behaved command does.
            /// </summary>
            public void SayTheAnswerTurned()
            {
                this.CanExecuteChanged?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}
