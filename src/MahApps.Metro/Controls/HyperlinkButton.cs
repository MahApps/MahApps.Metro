// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Navigation;
using MahApps.Metro.Automation.Peers;
using MahApps.Metro.ValueBoxes;

namespace MahApps.Metro.Controls
{
    /// <summary>
    /// A button that looks like a link, after the HyperlinkButton of WinUI. A click raises <see cref="ButtonBase.Click"/>
    /// and runs the command, then hands <see cref="NavigateUri"/> on through <see cref="RequestNavigate"/>, the event of
    /// the WPF <see cref="Hyperlink"/>, so a <see cref="System.Windows.Controls.Frame"/> navigates to it. When nobody
    /// handles it, a web or mail URI opens in the program Windows has for it.
    /// </summary>
    [TemplatePart(Name = PART_ContentPresenter, Type = typeof(ContentPresenter))]
    public class HyperlinkButton : ButtonBase
    {
        private const string PART_ContentPresenter = "PART_ContentPresenter";
        private const string TextTemplateKey = "MahApps.Templates.HyperlinkButton.Text";

        /// <summary>Identifies the <see cref="NavigateUri"/> dependency property.</summary>
        public static readonly DependencyProperty NavigateUriProperty
            = DependencyProperty.Register(nameof(NavigateUri),
                                          typeof(Uri),
                                          typeof(HyperlinkButton),
                                          new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the URI a click hands on. An <c>http</c>, <c>https</c> or <c>mailto</c> URI that no handler takes
        /// opens in its default program; any other only goes to the handlers of <see cref="RequestNavigate"/>.
        /// </summary>
        public Uri? NavigateUri
        {
            get => (Uri?)this.GetValue(NavigateUriProperty);
            set => this.SetValue(NavigateUriProperty, value);
        }

        /// <summary>Identifies the <see cref="IsUnderlineVisible"/> dependency property.</summary>
        public static readonly DependencyProperty IsUnderlineVisibleProperty
            = DependencyProperty.Register(nameof(IsUnderlineVisible),
                                          typeof(bool),
                                          typeof(HyperlinkButton),
                                          new PropertyMetadata(BooleanBoxes.TrueBox));

        /// <summary>
        /// Gets or sets whether text content is underlined. Only a string is; an element given as content keeps its own look.
        /// </summary>
        public bool IsUnderlineVisible
        {
            get => (bool)this.GetValue(IsUnderlineVisibleProperty);
            set => this.SetValue(IsUnderlineVisibleProperty, BooleanBoxes.Box(value));
        }

        /// <summary>Identifies the <see cref="RequestNavigate"/> routed event, the one of <see cref="Hyperlink"/>.</summary>
        public static readonly RoutedEvent RequestNavigateEvent = Hyperlink.RequestNavigateEvent.AddOwner(typeof(HyperlinkButton));

        /// <summary>
        /// Occurs after a click when <see cref="NavigateUri"/> is set. Setting <see cref="RoutedEventArgs.Handled"/> keeps
        /// the URI from opening in its default program.
        /// </summary>
        public event RequestNavigateEventHandler RequestNavigate
        {
            add => this.AddHandler(RequestNavigateEvent, value);
            remove => this.RemoveHandler(RequestNavigateEvent, value);
        }

        static HyperlinkButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(HyperlinkButton), new FrameworkPropertyMetadata(typeof(HyperlinkButton)));
        }

        /// <inheritdoc />
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            // only the presenter of the button itself picks the underlined text; a template for String among the
            // resources would reach every presenter inside content the app draws through a template of its own
            if (this.GetTemplateChild(PART_ContentPresenter) is ContentPresenter presenter)
            {
                presenter.ContentTemplateSelector = new TextTemplateSelector(this);
            }
        }

        /// <inheritdoc />
        protected override void OnClick()
        {
            base.OnClick();

            var uri = this.NavigateUri;
            if (uri is null)
            {
                return;
            }

            var args = new RequestNavigateEventArgs(uri, null) { Source = this };
            this.RaiseEvent(args);

            if (!args.Handled && IsLaunchable(uri))
            {
                this.LaunchUri(uri);
            }
        }

        // WinUI opens every scheme but file:, from a sandbox. A desktop app has none, and a scheme can start whatever
        // program registered it, so only the web and mail open by themselves; any other URI goes to the handlers alone.
        private static bool IsLaunchable(Uri uri)
        {
            return uri.IsAbsoluteUri
                   && (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps || uri.Scheme == Uri.UriSchemeMailto);
        }

        /// <summary>
        /// Opens an <c>http</c>, <c>https</c> or <c>mailto</c> URI that no handler took, in the program Windows has for it.
        /// </summary>
        protected virtual void LaunchUri(Uri uri)
        {
            // a mailto: on a machine without a mail program has nothing to start; like the launcher of WinUI the click
            // then does nothing rather than take the app down
            try
            {
                using (Process.Start(new ProcessStartInfo(uri.AbsoluteUri) { UseShellExecute = true }))
                {
                }
            }
            catch (Exception e) when (e is System.ComponentModel.Win32Exception or InvalidOperationException)
            {
                Trace.TraceWarning($"{nameof(HyperlinkButton)} could not open {uri}: {e.Message}");
            }
        }

        // a plain string gets the underlined text; a format, a selector of the app or anything else goes the usual way
        private sealed class TextTemplateSelector : DataTemplateSelector
        {
            private readonly HyperlinkButton owner;

            public TextTemplateSelector(HyperlinkButton owner)
            {
                this.owner = owner;
            }

            public override DataTemplate? SelectTemplate(object? item, DependencyObject container)
            {
                var appSelector = this.owner.ContentTemplateSelector;
                if (appSelector is not null)
                {
                    return appSelector.SelectTemplate(item, container);
                }

                return item is string && string.IsNullOrEmpty(this.owner.ContentStringFormat)
                    ? this.owner.TryFindResource(TextTemplateKey) as DataTemplate
                    : null;
            }
        }

        internal void AutomationClick()
        {
            this.OnClick();
        }

        /// <inheritdoc />
        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new HyperlinkButtonAutomationPeer(this);
        }
    }
}
