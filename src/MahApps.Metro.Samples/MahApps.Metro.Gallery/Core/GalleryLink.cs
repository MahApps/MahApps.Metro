// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Reflection;

namespace MahApps.Metro.Gallery.Core
{
    /// <summary>
    /// A link with the text it is shown with, such as one of the libraries the gallery stands on.
    /// </summary>
    public sealed class GalleryLink
    {
        public GalleryLink(string text, string uri)
        {
            this.Text = text;
            this.Uri = uri;
        }

        public string Text { get; }

        public string Uri { get; }

        /// <summary>
        /// A library by its name and the version that is actually loaded, which says more than the
        /// one written in the project file.
        /// </summary>
        public static GalleryLink For(string name, Type typeInIt, string uri)
        {
            return new GalleryLink(name + " " + VersionOf(typeInIt.Assembly), uri);
        }

        /// <summary>
        /// The informational version of an assembly without the commit a build may append to it.
        /// </summary>
        public static string VersionOf(Assembly assembly)
        {
            var version = assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion
                          ?? assembly.GetName().Version?.ToString()
                          ?? string.Empty;

            var plus = version.IndexOf('+');
            return plus < 0 ? version : version.Substring(0, plus);
        }
    }
}
