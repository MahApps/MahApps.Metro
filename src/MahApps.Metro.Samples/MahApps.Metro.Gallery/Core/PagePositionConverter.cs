// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace MahApps.Metro.Gallery.Core
{
    /// <summary>
    /// The page on screen turned into its place in the whole list, for the strip at the bottom.
    /// <para>
    /// It reads <see cref="GalleryPages.All"/> the way the navigation and the search do, so the
    /// count follows the list and nobody has to keep a number in step with it. The settings are not
    /// part of that list and have no place in it, and the empty text is what lets the block showing
    /// this fold itself away.
    /// </para>
    /// </summary>
    public sealed class PagePositionConverter : IValueConverter
    {
        /// <inheritdoc />
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is not GalleryPage page)
            {
                return string.Empty;
            }

            var position = GalleryPages.All.TakeWhile(candidate => !ReferenceEquals(candidate, page)).Count();

            return position < GalleryPages.All.Count
                ? $"Page {position + 1} of {GalleryPages.All.Count}"
                : string.Empty;
        }

        /// <inheritdoc />
        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotSupportedException("The place of a page is read off the list, not written back into it.");
        }
    }
}
