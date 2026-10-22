// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace MahApps.Metro.Gallery.Core
{
    /// <summary>
    /// One icon font on the Icons page: the glyphs of it that the file has a name for, the search
    /// over them and the snippets for the one picked.
    /// </summary>
    public sealed class IconFont : INotifyPropertyChanged
    {
        private static readonly char[] Separators = { ' ', '\t' };

        private string searchText = string.Empty;
        private IReadOnlyList<IconData> icons;
        private IconData? selectedIcon;

        /// <param name="name">The name of the font, which is also the header of its tab.</param>
        /// <param name="fontFamily">What the glyphs are shown with.</param>
        /// <param name="glyphs">The face whose map says which glyphs the font really has.</param>
        /// <param name="source">Where the font comes from, said under the search.</param>
        /// <param name="isDefault">Whether it is what mah:FontIcon uses anyway, so a snippet can leave the family out.</param>
        public IconFont(string name, FontFamily fontFamily, GlyphTypeface glyphs, string source, bool isDefault)
        {
            this.Name = name;
            this.FontFamily = fontFamily;
            this.Source = source;
            this.IsDefault = isDefault;

            var map = glyphs.CharacterToGlyphMap;
            this.All = IconData.All.Where(icon => map.ContainsKey(icon.CodePoint)).ToList();
            this.icons = this.All;
            this.selectedIcon = this.All.FirstOrDefault();

            this.SearchTag = new RelayCommand(tag => this.SearchText = tag?.ToString() ?? string.Empty);
        }

        public string Name { get; }

        public FontFamily FontFamily { get; }

        public string Source { get; }

        public bool IsDefault { get; }

        /// <summary>
        /// Every glyph of this font the file knows.
        /// </summary>
        public IReadOnlyList<IconData> All { get; }

        /// <summary>
        /// What the search leaves of <see cref="All"/>.
        /// </summary>
        public IReadOnlyList<IconData> Icons
        {
            get => this.icons;
            private set
            {
                this.icons = value;
                this.OnPropertyChanged(nameof(this.Icons));
                this.OnPropertyChanged(nameof(this.Summary));
            }
        }

        public string Summary => this.icons.Count == this.All.Count
            ? $"{this.All.Count} icons, {this.Source}"
            : $"{this.icons.Count} of {this.All.Count} icons, {this.Source}";

        /// <summary>
        /// Every word typed has to be found in the code, the name or a tag, as in the WinUI 3 Gallery.
        /// </summary>
        public string SearchText
        {
            get => this.searchText;
            set
            {
                value ??= string.Empty;
                if (this.searchText == value)
                {
                    return;
                }

                this.searchText = value;
                this.OnPropertyChanged(nameof(this.SearchText));

                var terms = value.Split(Separators, StringSplitOptions.RemoveEmptyEntries);
                this.Icons = terms.Length == 0 ? this.All : this.All.Where(icon => icon.Matches(terms)).ToList();

                // the first match stands in the details, so typing alone gets to the snippet
                if (this.selectedIcon is null || !this.icons.Contains(this.selectedIcon))
                {
                    this.SelectedIcon = this.icons.FirstOrDefault();
                }
            }
        }

        public IconData? SelectedIcon
        {
            get => this.selectedIcon;
            set
            {
                if (ReferenceEquals(this.selectedIcon, value))
                {
                    return;
                }

                this.selectedIcon = value;
                this.OnPropertyChanged(nameof(this.SelectedIcon));
                this.OnPropertyChanged(nameof(this.Snippets));
            }
        }

        /// <summary>
        /// What can be copied for the picked glyph. A font other than the default one has to be
        /// named, since mah:FontIcon would otherwise show the glyph of Segoe MDL2 Assets.
        /// </summary>
        public IReadOnlyList<Snippet> Snippets
        {
            get
            {
                var icon = this.selectedIcon;
                if (icon is null)
                {
                    return Array.Empty<Snippet>();
                }

                var xaml = this.IsDefault
                    ? $"<mah:FontIcon Glyph=\"{icon.TextGlyph}\" />"
                    : $"<mah:FontIcon FontFamily=\"{this.Name}\" Glyph=\"{icon.TextGlyph}\" />";
                var code = this.IsDefault
                    ? $"var icon = new FontIcon {{ Glyph = \"{icon.CodeGlyph}\" }};"
                    : $"var icon = new FontIcon {{ FontFamily = new FontFamily(\"{this.Name}\"), Glyph = \"{icon.CodeGlyph}\" }};";

                return new[]
                       {
                           new Snippet("Icon name", icon.Name),
                           new Snippet("Text glyph", icon.TextGlyph),
                           new Snippet("Code glyph", icon.CodeGlyph),
                           new Snippet("XAML", xaml),
                           new Snippet("C#", code)
                       };
            }
        }

        /// <summary>
        /// Puts a tag of the picked glyph into the search.
        /// </summary>
        public ICommand SearchTag { get; }

        /// <inheritdoc />
        public event PropertyChangedEventHandler? PropertyChanged;

        private void OnPropertyChanged(string name)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        /// <summary>
        /// The icon fonts there are here: Segoe Fluent Icons where Windows has it, and Segoe MDL2
        /// Assets always, from Windows or else from MahApps.Metro, which is how mah:FontIcon gets it.
        /// </summary>
        public static IReadOnlyList<IconFont> Available(FontFamily symbolTheme)
        {
            var fonts = new List<IconFont>();

            var fluent = Installed("Segoe Fluent Icons");
            if (fluent is not null && TryGetGlyphs(fluent, out var fluentGlyphs))
            {
                fonts.Add(new IconFont("Segoe Fluent Icons", fluent, fluentGlyphs, "from Windows", false));
            }

            // the map comes from the copy that is really used, which on Windows 10 and 11 is the
            // newer one of the system rather than the one in the assembly
            var mdl2 = Installed("Segoe MDL2 Assets");
            var source = "from Windows";
            if (mdl2 is null)
            {
                mdl2 = new FontFamily(new Uri("pack://application:,,,/MahApps.Metro;component/Assets/"), "./#Segoe MDL2 Assets");
                source = "from MahApps.Metro";
            }

            if (TryGetGlyphs(mdl2, out var mdl2Glyphs))
            {
                fonts.Add(new IconFont("Segoe MDL2 Assets", symbolTheme, mdl2Glyphs, source, true));
            }

            return fonts;
        }

        private static FontFamily? Installed(string name)
        {
            // a FontFamily of a name that is not installed falls back to another font without a
            // word, so the list of the system is asked instead
            return Fonts.SystemFontFamilies.FirstOrDefault(family => string.Equals(family.Source, name, StringComparison.OrdinalIgnoreCase));
        }

        private static bool TryGetGlyphs(FontFamily family, out GlyphTypeface glyphs)
        {
            return new Typeface(family, FontStyles.Normal, FontWeights.Normal, FontStretches.Normal).TryGetGlyphTypeface(out glyphs);
        }

        public sealed class Snippet
        {
            public Snippet(string header, string text)
            {
                this.Header = header;
                this.Text = text;
            }

            public string Header { get; }

            public string Text { get; }
        }
    }
}
