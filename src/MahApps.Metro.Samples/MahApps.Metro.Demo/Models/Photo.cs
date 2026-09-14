// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

namespace MetroDemo.Models
{
    /// <summary>
    /// One picture of the gallery the FlipView sample shows: what to look at, what to call it and a
    /// line about it.
    /// </summary>
    public class Photo
    {
        public Photo(string file, string title, string caption)
        {
            this.Image = new Uri($"pack://application:,,,/MahApps.Metro.Demo;component/Assets/Photos/{file}", UriKind.RelativeOrAbsolute);
            this.Title = title;
            this.Caption = caption;
        }

        public Uri Image { get; }

        /// <summary>Short enough for the banner of a FlipView, which is where it ends up.</summary>
        public string Title { get; }

        public string Caption { get; }
    }
}
