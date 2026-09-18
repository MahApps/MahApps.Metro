// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Xml;
using ControlzEx.Theming;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;

namespace MahApps.Metro.Gallery.Core
{
    /// <summary>
    /// The colours for the XAML shown by a XamlDisplay, kept in step with the theme.
    /// <para>
    /// AvalonEdit can colour XML on its own, but the colours are part of its definition and a dark
    /// theme leaves them barely readable. So Themes/Xaml.xshd is loaded with the palette of the base
    /// colour that is currently on, and loaded again whenever that changes. The editors bind to
    /// <see cref="Definition"/> rather than holding a definition of their own, which is what keeps
    /// all of them in step.
    /// </para>
    /// </summary>
    public sealed class XamlSyntaxHighlighting : INotifyPropertyChanged
    {
        private static readonly Dictionary<string, string> LightPalette = new Dictionary<string, string>
                                                                          {
                                                                              { "Comment", "#008000" },
                                                                              { "CData", "#808080" },
                                                                              { "Declaration", "#0000FF" },
                                                                              { "Tag", "#A31515" },
                                                                              { "AttributeName", "#FF0000" },
                                                                              { "AttributeValue", "#0000FF" },
                                                                              { "MarkupExtension", "#6F008A" },
                                                                              { "Entity", "#B8860B" }
                                                                          };

        private static readonly Dictionary<string, string> DarkPalette = new Dictionary<string, string>
                                                                         {
                                                                             { "Comment", "#57A64A" },
                                                                             { "CData", "#909090" },
                                                                             { "Declaration", "#569CD6" },
                                                                             { "Tag", "#569CD6" },
                                                                             { "AttributeName", "#9CDCFE" },
                                                                             { "AttributeValue", "#CE9178" },
                                                                             { "MarkupExtension", "#C586C0" },
                                                                             { "Entity", "#D7BA7D" }
                                                                         };

        private IHighlightingDefinition? definition;

        private XamlSyntaxHighlighting()
        {
            ThemeManager.Current.ThemeChanged += (_, _) => this.Reload();

            this.Reload();
        }

        /// <summary>
        /// The one instance the editors bind to, as in
        /// <c>{Binding Definition, Source={x:Static core:XamlSyntaxHighlighting.Current}}</c>.
        /// </summary>
        public static XamlSyntaxHighlighting Current { get; } = new XamlSyntaxHighlighting();

        /// <summary>
        /// The XAML colouring for the theme that is on right now.
        /// </summary>
        public IHighlightingDefinition? Definition
        {
            get => this.definition;
            private set
            {
                if (ReferenceEquals(this.definition, value))
                {
                    return;
                }

                this.definition = value;
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.Definition)));
            }
        }

        /// <inheritdoc />
        public event PropertyChangedEventHandler? PropertyChanged;

        private void Reload()
        {
            var application = Application.Current;
            var isDark = application is not null
                         && ThemeManager.Current.DetectTheme(application)?.BaseColorScheme == ThemeManager.BaseColorDark;
            var palette = isDark ? DarkPalette : LightPalette;

            var assembly = Assembly.GetExecutingAssembly();
            using var resource = assembly.GetManifestResourceStream("MahApps.Metro.Gallery.Themes.Xaml.xshd");
            if (resource is null)
            {
                return;
            }

            using var resourceReader = new StreamReader(resource);

            var builder = new StringBuilder(resourceReader.ReadToEnd());
            foreach (var colour in palette)
            {
                builder.Replace("$" + colour.Key + "$", colour.Value);
            }

            using var reader = XmlReader.Create(new StringReader(builder.ToString()));

            // a definition of its own for every reload, because a loaded one is frozen and its
            // colours cannot be changed afterwards
            this.Definition = HighlightingLoader.Load(reader, HighlightingManager.Instance);
        }
    }
}
