// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.ObjectModel;
using System.Collections.Specialized;
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
            this.Properties.CollectionChanged += this.OnPropertiesChanged;

            // the grouping has to be built here rather than in the template: a resource is shared
            // and has no templated parent to bind its source to
            var grouped = new CollectionViewSource { Source = this.Properties };
            grouped.GroupDescriptions.Add(new PropertyGroupDescription(nameof(ExampleProperty.Group)));
            this.PropertyGroups = grouped.View;
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
                this.Properties.Add(new ExampleProperty(target, property, group));
            }
        }

        /// <inheritdoc />
        protected override void OnContentChanged(object oldContent, object newContent)
        {
            base.OnContentChanged(oldContent, newContent);

            this.RefreshXaml();
        }

        private void OnPropertiesChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems is not null)
            {
                foreach (ExampleProperty property in e.OldItems)
                {
                    property.PropertyChanged -= this.OnPropertyValueChanged;
                }
            }

            if (e.NewItems is not null)
            {
                foreach (ExampleProperty property in e.NewItems)
                {
                    property.PropertyChanged += this.OnPropertyValueChanged;
                }
            }

            this.RefreshXaml();
        }

        private void OnPropertyValueChanged(object? sender, PropertyChangedEventArgs e)
        {
            this.RefreshXaml();
        }

        private void RefreshXaml()
        {
            if (this.Content is XamlDisplay display)
            {
                // ShowMeTheXAML lays the markup out again when its formatter changes and on nothing
                // else that can be reached from out here, so a fresh formatter is how the new values
                // get in
                display.SetCurrentValue(XamlDisplay.FormatterProperty, new ExampleXamlFormatter(this.Properties));
            }
        }
    }
}
