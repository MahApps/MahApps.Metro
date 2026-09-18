// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Globalization;
using System.Windows.Controls;
using MahApps.Metro.IconPacks;

namespace MahApps.Metro.Gallery.Core
{
    /// <summary>
    /// One page of the gallery, as the navigation, the search and the links to the documentation and
    /// to the source all need to know it. This is the only place a page is written down, so a new
    /// one is one entry in <see cref="GalleryPages"/> and nothing else.
    /// </summary>
    public sealed class GalleryPage
    {
        private const string DocumentationRoot = "https://mahapps.github.io/mahapps.com/docs/";

        // the categories that have a place in the documentation, so that a page in one of them gets
        // its link worked out and the others simply have none
        private static readonly string[] DocumentedCategories = { "Controls", "Styles", "Helper", "Themes", "Dialogs" };
        private const string PageRoot = "https://github.com/MahApps/MahApps.Metro/blob/develop/src/MahApps.Metro.Samples/MahApps.Metro.Gallery/Pages/";
        private const string LibraryRoot = "https://github.com/MahApps/MahApps.Metro/blob/develop/src/MahApps.Metro/";

        private UserControl? view;

        public GalleryPage(string title,
                           string category,
                           Type pageType,
                           PackIconMaterialKind icon = PackIconMaterialKind.None,
                           string? documentation = null,
                           string? keywords = null,
                           string? control = null)
        {
            this.Title = title;
            this.Category = category;
            this.PageType = pageType;
            this.Icon = icon;
            this.Documentation = Link(title, category, documentation);
            this.PageSource = PageRoot + pageType.Name + ".xaml";
            this.ControlSource = control is null ? null : LibraryRoot + control;
            this.Keywords = keywords;
        }

        public string Title { get; }

        public string Category { get; }

        public Type PageType { get; }

        public PackIconMaterialKind Icon { get; }

        /// <summary>
        /// The page on mahapps.com, where there is one.
        /// </summary>
        public string? Documentation { get; }

        /// <summary>
        /// The XAML of this page on GitHub, which answers the question the samples cannot.
        /// </summary>
        public string PageSource { get; }

        /// <summary>
        /// The control itself on GitHub. The files do not follow one shape, some sit in a folder of
        /// their own, so a page says where its control lives rather than having it guessed.
        /// </summary>
        public string? ControlSource { get; }

        /// <summary>
        /// What else somebody might type when looking for this page.
        /// </summary>
        public string? Keywords { get; }

        /// <summary>
        /// The page itself, built when it is asked for the first time and kept afterwards, so that
        /// walking the navigation does not build all of them.
        /// </summary>
        public UserControl View => this.view ??= (UserControl)Activator.CreateInstance(this.PageType)!;

        /// <summary>
        /// Where the documentation of this page sits. The pages over there are named after the
        /// control and written in lower case, so a page only says it when it is one of the few that
        /// are named differently, such as the buttons page or the flyouts one.
        /// </summary>
        private static string? Link(string title, string category, string? documentation)
        {
            if (documentation is not null)
            {
                return DocumentationRoot + documentation;
            }

            return Array.IndexOf(DocumentedCategories, category) < 0
                ? null
                : DocumentationRoot + (category + "/" + title).ToLowerInvariant();
        }

        /// <summary>
        /// Whether this page is what somebody typing <paramref name="text"/> is after.
        /// </summary>
        public bool Matches(string? text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return false;
            }

            return Contains(this.Title, text!) || Contains(this.Category, text!) || Contains(this.Keywords, text!);

            static bool Contains(string? candidate, string text)
            {
                return candidate is not null
                       && CultureInfo.CurrentCulture.CompareInfo.IndexOf(candidate, text, CompareOptions.IgnoreCase) >= 0;
            }
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return this.Title;
        }
    }
}
