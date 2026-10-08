// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Xml;
using ControlzEx.Theming;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;

namespace MahApps.Metro.Gallery.Core
{
    /// <summary>
    /// The colours for the XAML shown by a card, kept in step with the theme.
    /// <para>
    /// AvalonEdit can colour XML on its own, but the colours are part of its definition and a dark
    /// theme leaves them barely readable. So Themes/Xaml.xshd is loaded with the palette of the base
    /// colour that is currently on, and loaded again whenever that changes. The editors bind to
    /// <see cref="Definition"/> rather than holding a definition of their own, which is what keeps
    /// all of them in step.
    /// </para>
    /// </summary>
    public sealed class XamlSyntaxHighlighting : DependencyObject
    {
        /// <summary>Identifies the <see cref="Definition"/> dependency property.</summary>
        public static readonly DependencyProperty DefinitionProperty
            = DependencyProperty.Register(nameof(Definition),
                                          typeof(IHighlightingDefinition),
                                          typeof(XamlSyntaxHighlighting),
                                          new PropertyMetadata(null));

        /// <summary>Identifies the <see cref="CSharpDefinition"/> dependency property.</summary>
        public static readonly DependencyProperty CSharpDefinitionProperty
            = DependencyProperty.Register(nameof(CSharpDefinition),
                                          typeof(IHighlightingDefinition),
                                          typeof(XamlSyntaxHighlighting),
                                          new PropertyMetadata(null));

        /// <summary>
        /// The colours of Visual Studio's dark theme for the names AvalonEdit gives the parts of C#.
        /// Its own colours are for a light background, so a dark theme gets these instead. The names
        /// not listed are the many groups of keywords, and those get the blue of a keyword.
        /// </summary>
        private static readonly Dictionary<string, string> DarkCSharp = new Dictionary<string, string>
                                                                        {
                                                                            { "Comment", "#57A64A" },
                                                                            { "String", "#CE9178" },
                                                                            { "StringInterpolation", "#DCDCDC" },
                                                                            { "Char", "#CE9178" },
                                                                            { "Preprocessor", "#9B9B9B" },
                                                                            { "Punctuation", "#DCDCDC" },
                                                                            { "MethodCall", "#DCDCAA" },
                                                                            { "NumberLiteral", "#B5CEA8" },
                                                                            { "GotoKeywords", "#C586C0" },
                                                                            { "ExceptionKeywords", "#C586C0" }
                                                                        };

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
        /// The C# colouring for the theme that is on right now, for a card that shows code.
        /// </summary>
        public IHighlightingDefinition? CSharpDefinition
        {
            get => (IHighlightingDefinition?)this.GetValue(CSharpDefinitionProperty);
            private set => this.SetValue(CSharpDefinitionProperty, value);
        }

        /// <summary>
        /// The XAML colouring for the theme that is on right now.
        /// </summary>
        public IHighlightingDefinition? Definition
        {
            get => (IHighlightingDefinition?)this.GetValue(DefinitionProperty);
            private set => this.SetValue(DefinitionProperty, value);
        }

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

            this.CSharpDefinition = LoadCSharp(isDark);
        }

        private static IHighlightingDefinition? LoadCSharp(bool isDark)
        {
            using var resource = typeof(HighlightingManager).Assembly.GetManifestResourceStream("ICSharpCode.AvalonEdit.Highlighting.Resources.CSharp-Mode.xshd");
            if (resource is null)
            {
                return null;
            }

            using var reader = XmlReader.Create(resource);
            var xshd = HighlightingLoader.LoadXshd(reader);

            if (!isDark)
            {
                return HighlightingLoader.Load(xshd, HighlightingManager.Instance);
            }

            Recolour(xshd, name => DarkCSharp.TryGetValue(name, out var known) ? known : "#569CD6");

            // a /// comment is coloured by the XmlDoc definition the C# one imports, and that one
            // comes with light colours as well, so the dark theme gets a copy in the green of a comment
            using var docResource = typeof(HighlightingManager).Assembly.GetManifestResourceStream("ICSharpCode.AvalonEdit.Highlighting.Resources.XmlDoc.xshd");
            if (docResource is null)
            {
                return HighlightingLoader.Load(xshd, HighlightingManager.Instance);
            }

            using var docReader = XmlReader.Create(docResource);
            var docXshd = HighlightingLoader.LoadXshd(docReader);
            Recolour(docXshd, _ => "#608B4E");

            return HighlightingLoader.Load(xshd, new DarkReferences(HighlightingLoader.Load(docXshd, HighlightingManager.Instance)));
        }

        private static void Recolour(XshdSyntaxDefinition xshd, Func<string, string> colourOf)
        {
            foreach (var colour in xshd.Elements.OfType<XshdColor>())
            {
                colour.Foreground = new SimpleHighlightingBrush((Color)ColorConverter.ConvertFromString(colourOf(colour.Name ?? string.Empty)));
            }
        }

        /// <summary>
        /// Hands the dark copy of XmlDoc to the C# definition that imports it, and every other name
        /// to the definitions AvalonEdit registered.
        /// </summary>
        private sealed class DarkReferences : IHighlightingDefinitionReferenceResolver
        {
            private readonly IHighlightingDefinition xmlDoc;

            public DarkReferences(IHighlightingDefinition xmlDoc)
            {
                this.xmlDoc = xmlDoc;
            }

            public IHighlightingDefinition GetDefinition(string name)
            {
                return name == "XmlDoc" ? this.xmlDoc : HighlightingManager.Instance.GetDefinition(name);
            }
        }
    }
}
