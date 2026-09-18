// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using MahApps.Metro.Gallery.Core;
using ShowMeTheXAML;

namespace MahApps.Metro.Gallery.Controls
{
    /// <summary>
    /// One card of a page: a living control, the options to turn it with, and the XAML behind it.
    /// <para>
    /// The content is a ShowMeTheXAML display, which is what puts the markup of the sample into the
    /// assembly while the gallery is built. The card takes that markup, has
    /// <see cref="ExampleXamlFormatter"/> set the watched properties to whatever is currently on,
    /// and shows the result under the control. Both the WinUI gallery and the FluentAvalonia one
    /// keep the shown code as a second copy with placeholders in it, and this is the way around
    /// that: the template comes out of the build and nobody has to keep it in step.
    /// </para>
    /// </summary>
    public class ControlExample : HeaderedContentControl
    {
        /// <summary>Identifies the <see cref="Description"/> dependency property.</summary>
        public static readonly DependencyProperty DescriptionProperty
            = DependencyProperty.Register(nameof(Description),
                                          typeof(string),
                                          typeof(ControlExample),
                                          new PropertyMetadata(null));

        /// <summary>Identifies the <see cref="UniqueKey"/> dependency property.</summary>
        public static readonly DependencyProperty UniqueKeyProperty
            = DependencyProperty.Register(nameof(UniqueKey),
                                          typeof(string),
                                          typeof(ControlExample),
                                          new PropertyMetadata(null, OnUniqueKeyChanged));

        /// <summary>Identifies the <see cref="Markup"/> dependency property.</summary>
        public static readonly DependencyProperty MarkupProperty
            = DependencyProperty.Register(nameof(Markup),
                                          typeof(string),
                                          typeof(ControlExample),
                                          new PropertyMetadata(null));

        /// <summary>Identifies the <see cref="IsOptionsExpanded"/> dependency property.</summary>
        public static readonly DependencyProperty IsOptionsExpandedProperty
            = DependencyProperty.Register(nameof(IsOptionsExpanded),
                                          typeof(bool),
                                          typeof(ControlExample),
                                          new PropertyMetadata(true));

        /// <summary>Identifies the <see cref="HasOptions"/> dependency property.</summary>
        public static readonly DependencyProperty HasOptionsProperty
            = DependencyProperty.Register(nameof(HasOptions),
                                          typeof(bool),
                                          typeof(ControlExample),
                                          new PropertyMetadata(false));

        /// <summary>Identifies the <see cref="Options"/> dependency property.</summary>
        public static readonly DependencyProperty OptionsProperty
            = DependencyProperty.Register(nameof(Options),
                                          typeof(object),
                                          typeof(ControlExample),
                                          new PropertyMetadata(null));

        static ControlExample()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ControlExample),
                                                     new FrameworkPropertyMetadata(typeof(ControlExample)));
        }

        public ControlExample()
        {
            this.Properties.CollectionChanged += (_, _) =>
                                                     {
                                                         this.SetCurrentValue(HasOptionsProperty, this.Properties.Count > 0 || this.Options is not null);
                                                         this.RefreshXaml();
                                                     };

            // A card whose content arrives before the key of its display has nothing to read yet, so
            // it asks again once it is up. Without this a card with no options to turn, and so
            // nothing else to trigger a second look, came up with an empty code block.
            this.Loaded += (_, _) => this.RefreshXaml();

            // the grouping has to be built here rather than in the template: a resource is shared
            // and has no templated parent to bind its source to
            var grouped = new CollectionViewSource { Source = this.Properties };
            grouped.GroupDescriptions.Add(new PropertyGroupDescription(nameof(ExampleProperty.Group)));
            this.PropertyGroups = grouped.View;
        }

        /// <summary>
        /// The key of the sample to show the markup of, for a card whose sample is not its own
        /// content. A flyout is the case that asked for it: it belongs in the Flyouts of the window
        /// and cannot sit in a card, so the card shows a button and takes the markup from the key.
        /// </summary>
        public string? UniqueKey
        {
            get => (string?)this.GetValue(UniqueKeyProperty);
            set => this.SetValue(UniqueKeyProperty, value);
        }

        /// <summary>
        /// The markup this card shows, laid out and with the current values in it.
        /// </summary>
        public string? Markup
        {
            get => (string?)this.GetValue(MarkupProperty);
            private set => this.SetValue(MarkupProperty, value);
        }

        /// <summary>
        /// One sentence on what this card is showing.
        /// </summary>
        public string? Description
        {
            get => (string?)this.GetValue(DescriptionProperty);
            set => this.SetValue(DescriptionProperty, value);
        }

        /// <summary>
        /// Whether the options are shown. They can be folded away, which is what a narrow window
        /// and a card that is mostly about the sample both want.
        /// </summary>
        public bool IsOptionsExpanded
        {
            get => (bool)this.GetValue(IsOptionsExpandedProperty);
            set => this.SetValue(IsOptionsExpandedProperty, value);
        }

        /// <summary>
        /// Whether there is anything to fold away at all.
        /// </summary>
        public bool HasOptions
        {
            get => (bool)this.GetValue(HasOptionsProperty);
            private set => this.SetValue(HasOptionsProperty, value);
        }

        /// <summary>
        /// Anything else that belongs beside the sample, for a card whose options are more than a
        /// plain list of properties.
        /// </summary>
        public object? Options
        {
            get => this.GetValue(OptionsProperty);
            set => this.SetValue(OptionsProperty, value);
        }

        /// <summary>
        /// The properties the reader can turn, filled by <see cref="Watch(DependencyObject, DependencyProperty[])"/>.
        /// </summary>
        public ObservableCollection<ExampleProperty> Properties { get; } = new ObservableCollection<ExampleProperty>();

        /// <summary>
        /// The same properties under their headings, which is what the options list shows.
        /// </summary>
        public ICollectionView PropertyGroups { get; }

        /// <summary>
        /// Takes the given properties of a sample into the options of this card.
        /// </summary>
        public void Watch(DependencyObject target, params DependencyProperty[] properties)
        {
            this.Watch(null, target, properties);
        }

        /// <summary>
        /// Takes the given properties of a sample into the options of this card, under a heading of
        /// their own.
        /// </summary>
        public void Watch(string? group, DependencyObject target, params DependencyProperty[] properties)
        {
            foreach (var property in properties)
            {
                this.Properties.Add(new ExampleProperty(target, property, group, this.RefreshXaml));
            }
        }

        /// <inheritdoc />
        protected override void OnContentChanged(object oldContent, object newContent)
        {
            base.OnContentChanged(oldContent, newContent);

            this.RefreshXaml();
        }

        private static void OnUniqueKeyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as ControlExample)?.RefreshXaml();
        }

        private void RefreshXaml()
        {
            var formatter = new ExampleXamlFormatter(this.Properties);

            if (this.UniqueKey is { } key)
            {
                // the sample is somewhere else, so the markup comes straight out of what the build
                // wrote down under that key
                this.Markup = formatter.FormatXaml(XamlResolver.Resolve(key));
                return;
            }

            if (this.Content is XamlDisplay display)
            {
                // ShowMeTheXAML lays the markup out again when its formatter changes and on nothing
                // else that can be reached from out here, so a fresh formatter is how the new values
                // get in
                display.SetCurrentValue(XamlDisplay.FormatterProperty, formatter);
                this.Markup = display.Xaml;
            }
        }
    }
}
