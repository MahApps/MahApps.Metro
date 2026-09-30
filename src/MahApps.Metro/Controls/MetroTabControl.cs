// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ControlzEx.Controls;
using JetBrains.Annotations;
using MahApps.Metro.ValueBoxes;

namespace MahApps.Metro.Controls
{
    /// <summary>
    /// A standard MetroTabControl (Pivot).
    /// </summary>
    public class MetroTabControl : BaseMetroTabControl
    {
        /// <summary>
        /// Initializes a new instance of the MahApps.Metro.Controls.MetroTabControl class.
        /// </summary>
        static MetroTabControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MetroTabControl), new FrameworkPropertyMetadata(typeof(MetroTabControl)));
        }

        /// <summary>Identifies the <see cref="KeepVisualTreeInMemoryWhenChangingTabs"/> dependency property.</summary>
        public static readonly DependencyProperty KeepVisualTreeInMemoryWhenChangingTabsProperty
            = DependencyProperty.Register(nameof(KeepVisualTreeInMemoryWhenChangingTabs),
                                          typeof(bool),
                                          typeof(MetroTabControl),
                                          new PropertyMetadata(BooleanBoxes.FalseBox));

        public bool KeepVisualTreeInMemoryWhenChangingTabs
        {
            get => (bool)this.GetValue(KeepVisualTreeInMemoryWhenChangingTabsProperty);
            set => this.SetValue(KeepVisualTreeInMemoryWhenChangingTabsProperty, BooleanBoxes.Box(value));
        }
    }

    /// <summary>
    /// A base class for every MetroTabControl (Pivot).
    /// </summary>
    public abstract class BaseMetroTabControl : TabControlEx
    {
        static BaseMetroTabControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(BaseMetroTabControl), new FrameworkPropertyMetadata(typeof(BaseMetroTabControl)));
        }

        // Using a DependencyProperty as the backing store for TabStripMargin.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TabStripMarginProperty
            = DependencyProperty.Register(nameof(TabStripMargin),
                                          typeof(Thickness),
                                          typeof(BaseMetroTabControl),
                                          new PropertyMetadata(new Thickness(0)));

        /// <summary>
        /// Gets or sets the margin of the TabStrip.
        /// </summary>
        public Thickness TabStripMargin
        {
            get => (Thickness)this.GetValue(TabStripMarginProperty);
            set => this.SetValue(TabStripMarginProperty, value);
        }

        public static readonly DependencyProperty CloseTabCommandProperty
            = DependencyProperty.Register(nameof(CloseTabCommand),
                                          typeof(ICommand),
                                          typeof(BaseMetroTabControl),
                                          new PropertyMetadata(null));

        /// <summary>
        /// Get or sets the command that executes when a MetroTabItem's close button is clicked.
        /// </summary>
        public ICommand? CloseTabCommand
        {
            get => (ICommand?)this.GetValue(CloseTabCommandProperty);
            set => this.SetValue(CloseTabCommandProperty, value);
        }

        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is TabItem;
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new MetroTabItem(); //Overrides the TabControl's default behavior and returns a MetroTabItem instead of a regular one.
        }

        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            if (element != item)
            {
                element.SetValue(DataContextProperty, item); //dont want to set the datacontext to itself.
            }

            base.PrepareContainerForItemOverride(element, item);
        }

        public delegate void TabItemClosingEventHandler(object sender, TabItemClosingEventArgs e);

        /// <summary>
        /// An event that is raised when a TabItem is about to be closed.
        /// </summary>
        /// <remarks>
        /// <see cref="TabItemClosing"/> carries the name this event should have had and works the
        /// same way. Both are raised, this one first.
        /// </remarks>
        public event TabItemClosingEventHandler? TabItemClosingEvent;

        /// <summary>
        /// An event that is raised when a TabItem is about to be closed. A handler says no by
        /// setting <see cref="CancelEventArgs.Cancel"/>, and it can take its time over that answer
        /// by asking for a deferral first.
        /// </summary>
        /// <example>
        /// <code>
        /// private async void OnTabItemClosing(object sender, BaseMetroTabControl.TabItemClosingEventArgs e)
        /// {
        ///     using var deferral = e.GetDeferral();
        ///     e.Cancel = !await this.viewModel.MayTheTabCloseAsync(e.ClosingTabItem.DataContext);
        /// }
        /// </code>
        /// </example>
        public event EventHandler<TabItemClosingEventArgs>? TabItemClosing;

        private IEnumerable<Delegate> ClosingHandlers()
        {
            var tabItemClosingEvent = this.TabItemClosingEvent;
            if (tabItemClosingEvent is not null)
            {
                foreach (var handler in tabItemClosingEvent.GetInvocationList())
                {
                    yield return handler;
                }
            }

            var tabItemClosing = this.TabItemClosing;
            if (tabItemClosing is not null)
            {
                foreach (var handler in tabItemClosing.GetInvocationList())
                {
                    yield return handler;
                }
            }
        }

        /// <summary>
        /// Asks every handler whether the item may go and tells whether one of them said no.
        /// </summary>
        /// <remarks>
        /// As long as no handler asks for a deferral this runs to its end without suspending once,
        /// so the task that comes back is already done and the closing stays as synchronous as it
        /// has always been.
        /// </remarks>
        private async Task<bool> RaiseTabItemClosingEventAsync(MetroTabItem closingItem)
        {
            foreach (var handler in this.ClosingHandlers())
            {
                var args = new TabItemClosingEventArgs(closingItem);

                switch (handler)
                {
                    case TabItemClosingEventHandler tabItemClosingEventHandler:
                        tabItemClosingEventHandler(this, args);
                        break;

                    case EventHandler<TabItemClosingEventArgs> eventHandler:
                        eventHandler(this, args);
                        break;

                    default:
                        continue;
                }

                // a handler that asked for a deferral gets the time it wanted, and only then
                // is its answer read
                await args.WaitForDeferralsAsync().ConfigureAwait(true);

                if (args.Cancel)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Event args that is created when a TabItem is closed.
        /// </summary>
        public class TabItemClosingEventArgs : CancelEventArgs
        {
            private TaskCompletionSource<bool>? completion;

            private int outstandingDeferrals;

            internal TabItemClosingEventArgs(MetroTabItem item)
            {
                this.ClosingTabItem = item;
            }

            /// <summary>
            /// Gets the MetroTabItem that will be closed.
            /// </summary>
            public MetroTabItem ClosingTabItem { get; private set; }

            /// <summary>
            /// Asks the tab control to wait for this handler before it goes on with the closing.
            /// Set <see cref="CancelEventArgs.Cancel"/> while the answer is being worked out and
            /// complete or dispose the deferral once it stands.
            /// </summary>
            /// <returns>The deferral to complete when the handler is done.</returns>
            public TabItemClosingDeferral GetDeferral()
            {
                this.completion ??= new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
                this.outstandingDeferrals++;

                return new TabItemClosingDeferral(this.OneDeferralIsDone);
            }

            internal Task WaitForDeferralsAsync()
            {
                return this.completion?.Task ?? Task.CompletedTask;
            }

            private void OneDeferralIsDone()
            {
                if (--this.outstandingDeferrals <= 0)
                {
                    this.completion?.TrySetResult(true);
                }
            }
        }

        internal void CloseThisTabItem([NotNull] MetroTabItem tabItem)
        {
            if (tabItem is null)
            {
                throw new ArgumentNullException(nameof(tabItem));
            }

            // the handlers are allowed to say no, and to take their time over it
            var closing = this.RaiseTabItemClosingEventAsync(tabItem);

            if (closing.IsCompleted)
            {
                this.CloseThisTabItemNow(tabItem, closing.GetAwaiter().GetResult());
            }
            else
            {
                this.CloseThisTabItemWhenTheHandlersAreDone(tabItem, closing);
            }
        }

        private async void CloseThisTabItemWhenTheHandlersAreDone(MetroTabItem tabItem, Task<bool> closing)
        {
            this.CloseThisTabItemNow(tabItem, await closing.ConfigureAwait(true));
        }

        private void CloseThisTabItemNow(MetroTabItem tabItem, bool cancelled)
        {
            if (cancelled)
            {
                return;
            }

            if (this.CloseTabCommand is { } closeTabCommand)
            {
                var closeTabCommandParameter = tabItem.CloseTabCommandParameter ?? tabItem;
                if (closeTabCommand.CanExecute(closeTabCommandParameter))
                {
                    closeTabCommand.Execute(closeTabCommandParameter);
                }

                return;
            }

            // KIDS: don't try this at home
            // this is not good MVVM habits and I'm only doing it
            // because I want the demos to be absolutely bitching

            if (this.ItemsSource is null)
            {
                // if the list is hard-coded (i.e. has no ItemsSource)
                // then we remove the item from the collection
                tabItem.ClearStyle();
                this.Items.Remove(tabItem);
            }
            else
            {
                // if ItemsSource is something we cannot work with, bail out
                var collection = this.ItemsSource as IList;
                if (collection is null)
                {
                    return;
                }

                // find the item and kill it (I mean, remove it)
                var item2Remove = collection.OfType<object>().FirstOrDefault(item => tabItem == item || tabItem.DataContext == item);
                if (item2Remove != null)
                {
                    tabItem.ClearStyle();
                    collection.Remove(item2Remove);
                }
            }
        }
    }
}