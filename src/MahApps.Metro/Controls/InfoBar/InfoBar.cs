// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
// Ported from InfoBar.cpp of microsoft-ui-xaml, Copyright (c) Microsoft Corporation, MIT License.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Markup;
using MahApps.Metro.Automation.Peers;
using MahApps.Metro.ValueBoxes;

namespace MahApps.Metro.Controls
{
    /// <summary>
    /// A bar inside the layout that tells the user something about the state of the app, after the InfoBar of
    /// WinUI: a severity with its colour and icon, a title, a message, an optional action and a close button.
    /// </summary>
    [TemplatePart(Name = PART_CloseButton, Type = typeof(ButtonBase))]
    [TemplatePart(Name = PART_ContentArea, Type = typeof(FrameworkElement))]
    [StyleTypedProperty(Property = nameof(CloseButtonStyle), StyleTargetType = typeof(Button))]
    [ContentProperty(nameof(Content))]
    public class InfoBar : Control
    {
        private const string PART_CloseButton = "PART_CloseButton";
        private const string PART_ContentArea = "PART_ContentArea";

        private ButtonBase? closeButton;
        private FrameworkElement? contentArea;
        private InfoBarCloseReason lastCloseReason = InfoBarCloseReason.Programmatic;
        private bool refusingClose;

        static InfoBar()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(InfoBar), new FrameworkPropertyMetadata(typeof(InfoBar)));
        }

        /// <summary>Identifies the <see cref="IsOpen"/> dependency property.</summary>
        public static readonly DependencyProperty IsOpenProperty
            = DependencyProperty.Register(nameof(IsOpen),
                                          typeof(bool),
                                          typeof(InfoBar),
                                          new FrameworkPropertyMetadata(BooleanBoxes.FalseBox, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnIsOpenChanged));

        /// <summary>
        /// Gets or sets whether the bar is shown. A closed bar takes no room in the layout.
        /// </summary>
        public bool IsOpen
        {
            get => (bool)this.GetValue(IsOpenProperty);
            set => this.SetValue(IsOpenProperty, BooleanBoxes.Box(value));
        }

        /// <summary>Identifies the <see cref="Title"/> dependency property.</summary>
        public static readonly DependencyProperty TitleProperty
            = DependencyProperty.Register(nameof(Title),
                                          typeof(string),
                                          typeof(InfoBar),
                                          new PropertyMetadata(null, OnBannerContentChanged));

        /// <summary>
        /// Gets or sets the title, written in bold in front of the message.
        /// </summary>
        public string? Title
        {
            get => (string?)this.GetValue(TitleProperty);
            set => this.SetValue(TitleProperty, value);
        }

        /// <summary>Identifies the <see cref="Message"/> dependency property.</summary>
        public static readonly DependencyProperty MessageProperty
            = DependencyProperty.Register(nameof(Message),
                                          typeof(string),
                                          typeof(InfoBar),
                                          new PropertyMetadata(null, OnBannerContentChanged));

        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        public string? Message
        {
            get => (string?)this.GetValue(MessageProperty);
            set => this.SetValue(MessageProperty, value);
        }

        /// <summary>Identifies the <see cref="Severity"/> dependency property.</summary>
        public static readonly DependencyProperty SeverityProperty
            = DependencyProperty.Register(nameof(Severity),
                                          typeof(InfoBarSeverity),
                                          typeof(InfoBar),
                                          new PropertyMetadata(InfoBarSeverity.Informational));

        /// <summary>
        /// Gets or sets how serious the message is, which sets the colour of the bar and its icon.
        /// </summary>
        public InfoBarSeverity Severity
        {
            get => (InfoBarSeverity)this.GetValue(SeverityProperty);
            set => this.SetValue(SeverityProperty, value);
        }

        /// <summary>Identifies the <see cref="Icon"/> dependency property.</summary>
        public static readonly DependencyProperty IconProperty
            = DependencyProperty.Register(nameof(Icon),
                                          typeof(object),
                                          typeof(InfoBar),
                                          new PropertyMetadata(null, OnLogicalChildChanged));

        /// <summary>
        /// Gets or sets an icon in place of the one of the severity. It is shown at 16 by 16.
        /// An element can stand in one bar only; to give many bars the same icon, set <see cref="IconTemplate"/> in a style.
        /// </summary>
        public object? Icon
        {
            get => this.GetValue(IconProperty);
            set => this.SetValue(IconProperty, value);
        }

        /// <summary>Identifies the <see cref="IconTemplate"/> dependency property.</summary>
        public static readonly DependencyProperty IconTemplateProperty
            = DependencyProperty.Register(nameof(IconTemplate),
                                          typeof(DataTemplate),
                                          typeof(InfoBar),
                                          new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the template of the icon. Without an <see cref="Icon"/> it is given the <see cref="Severity"/>,
        /// so one template in a style can draw an icon of its own for each severity.
        /// </summary>
        public DataTemplate? IconTemplate
        {
            get => (DataTemplate?)this.GetValue(IconTemplateProperty);
            set => this.SetValue(IconTemplateProperty, value);
        }

        /// <summary>Identifies the <see cref="IsIconVisible"/> dependency property.</summary>
        public static readonly DependencyProperty IsIconVisibleProperty
            = DependencyProperty.Register(nameof(IsIconVisible),
                                          typeof(bool),
                                          typeof(InfoBar),
                                          new PropertyMetadata(BooleanBoxes.TrueBox));

        /// <summary>
        /// Gets or sets whether the bar shows an icon.
        /// </summary>
        public bool IsIconVisible
        {
            get => (bool)this.GetValue(IsIconVisibleProperty);
            set => this.SetValue(IsIconVisibleProperty, BooleanBoxes.Box(value));
        }

        /// <summary>Identifies the <see cref="IsClosable"/> dependency property.</summary>
        public static readonly DependencyProperty IsClosableProperty
            = DependencyProperty.Register(nameof(IsClosable),
                                          typeof(bool),
                                          typeof(InfoBar),
                                          new PropertyMetadata(BooleanBoxes.TrueBox));

        /// <summary>
        /// Gets or sets whether the user can close the bar with its close button.
        /// </summary>
        public bool IsClosable
        {
            get => (bool)this.GetValue(IsClosableProperty);
            set => this.SetValue(IsClosableProperty, BooleanBoxes.Box(value));
        }

        /// <summary>Identifies the <see cref="ActionButton"/> dependency property.</summary>
        public static readonly DependencyProperty ActionButtonProperty
            = DependencyProperty.Register(nameof(ActionButton),
                                          typeof(object),
                                          typeof(InfoBar),
                                          new PropertyMetadata(null, OnActionButtonChanged));

        /// <summary>
        /// Gets or sets the action after the message, usually a button or a text with a hyperlink.
        /// </summary>
        public object? ActionButton
        {
            get => this.GetValue(ActionButtonProperty);
            set => this.SetValue(ActionButtonProperty, value);
        }

        /// <summary>Identifies the <see cref="Content"/> dependency property.</summary>
        public static readonly DependencyProperty ContentProperty
            = DependencyProperty.Register(nameof(Content),
                                          typeof(object),
                                          typeof(InfoBar),
                                          new PropertyMetadata(null, OnLogicalChildChanged));

        /// <summary>
        /// Gets or sets content under the title and the message.
        /// </summary>
        public object? Content
        {
            get => this.GetValue(ContentProperty);
            set => this.SetValue(ContentProperty, value);
        }

        /// <summary>Identifies the <see cref="ContentTemplate"/> dependency property.</summary>
        public static readonly DependencyProperty ContentTemplateProperty
            = DependencyProperty.Register(nameof(ContentTemplate),
                                          typeof(DataTemplate),
                                          typeof(InfoBar),
                                          new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the template of <see cref="Content"/>.
        /// </summary>
        public DataTemplate? ContentTemplate
        {
            get => (DataTemplate?)this.GetValue(ContentTemplateProperty);
            set => this.SetValue(ContentTemplateProperty, value);
        }

        /// <summary>Identifies the <see cref="CloseButtonStyle"/> dependency property.</summary>
        public static readonly DependencyProperty CloseButtonStyleProperty
            = DependencyProperty.Register(nameof(CloseButtonStyle),
                                          typeof(Style),
                                          typeof(InfoBar),
                                          new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the style of the close button.
        /// </summary>
        public Style? CloseButtonStyle
        {
            get => (Style?)this.GetValue(CloseButtonStyleProperty);
            set => this.SetValue(CloseButtonStyleProperty, value);
        }

        /// <summary>Identifies the <see cref="CloseButtonCommand"/> dependency property.</summary>
        public static readonly DependencyProperty CloseButtonCommandProperty
            = DependencyProperty.Register(nameof(CloseButtonCommand),
                                          typeof(ICommand),
                                          typeof(InfoBar),
                                          new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets a command the close button runs as it closes the bar.
        /// </summary>
        public ICommand? CloseButtonCommand
        {
            get => (ICommand?)this.GetValue(CloseButtonCommandProperty);
            set => this.SetValue(CloseButtonCommandProperty, value);
        }

        /// <summary>Identifies the <see cref="CloseButtonCommandParameter"/> dependency property.</summary>
        public static readonly DependencyProperty CloseButtonCommandParameterProperty
            = DependencyProperty.Register(nameof(CloseButtonCommandParameter),
                                          typeof(object),
                                          typeof(InfoBar),
                                          new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the parameter of <see cref="CloseButtonCommand"/>.
        /// </summary>
        public object? CloseButtonCommandParameter
        {
            get => this.GetValue(CloseButtonCommandParameterProperty);
            set => this.SetValue(CloseButtonCommandParameterProperty, value);
        }

        /// <summary>Identifies the <see cref="CloseButtonClick"/> routed event.</summary>
        public static readonly RoutedEvent CloseButtonClickEvent
            = EventManager.RegisterRoutedEvent(nameof(CloseButtonClick),
                                               RoutingStrategy.Bubble,
                                               typeof(RoutedEventHandler),
                                               typeof(InfoBar));

        /// <summary>
        /// Occurs when the user clicks the close button, before the bar closes.
        /// </summary>
        public event RoutedEventHandler CloseButtonClick
        {
            add => this.AddHandler(CloseButtonClickEvent, value);
            remove => this.RemoveHandler(CloseButtonClickEvent, value);
        }

        /// <summary>Identifies the <see cref="Closing"/> routed event.</summary>
        public static readonly RoutedEvent ClosingEvent
            = EventManager.RegisterRoutedEvent(nameof(Closing),
                                               RoutingStrategy.Bubble,
                                               typeof(EventHandler<InfoBarClosingEventArgs>),
                                               typeof(InfoBar));

        /// <summary>
        /// Occurs when the bar is about to close. Setting <see cref="InfoBarClosingEventArgs.Cancel"/> keeps it open.
        /// </summary>
        /// <remarks>
        /// <see cref="IsOpen"/> is already false while this runs, so a two-way binding has handed false to its source,
        /// and hands true back when the close is refused. The event bubbles: a handler on a bar also hears a bar inside
        /// its <see cref="Content"/>, so check <see cref="RoutedEventArgs.Source"/> against the sender where that matters.
        /// </remarks>
        public event EventHandler<InfoBarClosingEventArgs> Closing
        {
            add => this.AddHandler(ClosingEvent, value);
            remove => this.RemoveHandler(ClosingEvent, value);
        }

        /// <summary>Identifies the <see cref="Closed"/> routed event.</summary>
        public static readonly RoutedEvent ClosedEvent
            = EventManager.RegisterRoutedEvent(nameof(Closed),
                                               RoutingStrategy.Bubble,
                                               typeof(EventHandler<InfoBarClosedEventArgs>),
                                               typeof(InfoBar));

        /// <summary>
        /// Occurs when the bar has closed.
        /// </summary>
        public event EventHandler<InfoBarClosedEventArgs> Closed
        {
            add => this.AddHandler(ClosedEvent, value);
            remove => this.RemoveHandler(ClosedEvent, value);
        }

        /// <inheritdoc />
        public override void OnApplyTemplate()
        {
            if (this.closeButton is not null)
            {
                this.closeButton.Click -= this.OnCloseButtonClick;
            }

            base.OnApplyTemplate();

            this.closeButton = this.GetTemplateChild(PART_CloseButton) as ButtonBase;
            if (this.closeButton is not null)
            {
                this.closeButton.Click += this.OnCloseButtonClick;
            }

            this.contentArea = this.GetTemplateChild(PART_ContentArea) as FrameworkElement;
            this.UpdateContentPosition();
        }

        private void OnCloseButtonClick(object sender, RoutedEventArgs e)
        {
            this.RaiseEvent(new RoutedEventArgs(CloseButtonClickEvent, this));
            this.lastCloseReason = InfoBarCloseReason.CloseButton;
            this.SetCurrentValue(IsOpenProperty, BooleanBoxes.FalseBox);
        }

        private static void OnIsOpenChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((InfoBar)d).OnIsOpenChanged((bool)e.NewValue);
        }

        private void OnIsOpenChanged(bool isOpen)
        {
            if (isOpen)
            {
                this.lastCloseReason = InfoBarCloseReason.Programmatic;

                // a refused close puts the bar back, but it never went, so there is nothing to open
                if (!this.refusingClose)
                {
                    this.UpdatePeer();
                    this.OnOpened();
                }

                return;
            }

            var closing = new InfoBarClosingEventArgs(ClosingEvent, this.lastCloseReason) { Source = this };
            this.RaiseEvent(closing);

            if (closing.Cancel)
            {
                // the bar stays, and the next close starts from scratch
                this.refusingClose = true;
                try
                {
                    this.SetCurrentValue(IsOpenProperty, BooleanBoxes.TrueBox);
                }
                finally
                {
                    this.refusingClose = false;
                }

                return;
            }

            // a close by keyboard would leave the focus on a button that is gone, so it moves on past the bar
            if (this.closeButton?.IsKeyboardFocused == true)
            {
                this.closeButton.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
            }

            this.UpdatePeer();
            this.OnClosed();
            this.RaiseEvent(new InfoBarClosedEventArgs(ClosedEvent, closing.Reason) { Source = this });
        }

        /// <inheritdoc />
        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new InfoBarAutomationPeer(this);
        }

        /// <summary>
        /// Called when the bar has opened. Tells a screen reader what the bar says.
        /// </summary>
        protected virtual void OnOpened()
        {
            this.Notify(this.GetOpenedNotification(), "InfoBarOpenedActivityId");
        }

        /// <summary>
        /// Called when the bar has closed, before <see cref="Closed"/> is raised. Tells a screen reader it has gone.
        /// </summary>
        protected virtual void OnClosed()
        {
            this.Notify(Lang.Automation.InfoBarDismissed, "InfoBarClosedActivityId");
        }

        /// <summary>
        /// Gets what a screen reader hears when the bar opens: the name of the icon, the title and the message.
        /// </summary>
        protected virtual string GetOpenedNotification()
        {
            var icon = this.Severity switch
            {
                InfoBarSeverity.Success => Lang.Automation.InfoBarIconSuccess,
                InfoBarSeverity.Warning => Lang.Automation.InfoBarIconWarning,
                InfoBarSeverity.Error => Lang.Automation.InfoBarIconError,
                _ => Lang.Automation.InfoBarIconInformational
            };

            return $"{icon} {this.Title} {this.Message}";
        }

        // the bar leaves the control view when it closes and comes back when it opens, which a client that keeps the tree has to hear
        private void UpdatePeer()
        {
            var peer = UIElementAutomationPeer.FromElement(this);
            if (peer is null)
            {
                return;
            }

            peer.InvalidatePeer();
            if (AutomationPeer.ListenerExists(AutomationEvents.StructureChanged))
            {
                peer.RaiseAutomationEvent(AutomationEvents.StructureChanged);
            }
        }

        private void Notify(string text, string activityId)
        {
#if NETCOREAPP
            // only when a reader listens, the peer is already there
            var peer = UIElementAutomationPeer.FromElement(this);
            if (peer is null)
            {
                return;
            }

            var processing = this.Severity is InfoBarSeverity.Warning or InfoBarSeverity.Error
                ? System.Windows.Automation.AutomationNotificationProcessing.ImportantAll
                : System.Windows.Automation.AutomationNotificationProcessing.CurrentThenMostRecent;

            peer.RaiseNotificationEvent(System.Windows.Automation.AutomationNotificationKind.Other, processing, text, activityId);
#endif
        }

        /// <inheritdoc />
        protected override IEnumerator LogicalChildren
        {
            get
            {
                // content, action and icon belong to the bar even while it is closed and nothing shows them,
                // so that their bindings find the data context of the bar from the start
                var children = new List<object>();
                foreach (var child in new[] { this.Icon, this.ActionButton, this.Content })
                {
                    if (child is not null)
                    {
                        children.Add(child);
                    }
                }

                return children.GetEnumerator();
            }
        }

        private static void OnLogicalChildChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var infoBar = (InfoBar)d;
            if (e.OldValue is not null)
            {
                infoBar.RemoveLogicalChild(e.OldValue);
            }

            if (e.NewValue is not null)
            {
                infoBar.AddLogicalChild(e.NewValue);
            }
        }

        private static void OnActionButtonChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            OnLogicalChildChanged(d, e);
            OnBannerContentChanged(d, e);
        }

        private static void OnBannerContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((InfoBar)d).UpdateContentPosition();
        }

        private void UpdateContentPosition()
        {
            if (this.contentArea is null)
            {
                return;
            }

            // without a title, a message or an action the content moves up into the first line
            var hasBannerContent = !string.IsNullOrEmpty(this.Title) || !string.IsNullOrEmpty(this.Message) || this.ActionButton is not null;
            Grid.SetRow(this.contentArea, hasBannerContent ? 1 : 0);
        }
    }
}
