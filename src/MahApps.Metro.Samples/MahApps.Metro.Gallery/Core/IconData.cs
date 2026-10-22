// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;

namespace MahApps.Metro.Gallery.Core
{
    /// <summary>
    /// One glyph of the Segoe icon fonts with the name and the tags the WinUI 3 Gallery gives it,
    /// read from Data/IconsData.json, which comes from there unchanged.
    /// </summary>
    [DataContract]
    public sealed class IconData
    {
        private const string Resource = "MahApps.Metro.Gallery.Data.IconsData.json";

        private static IReadOnlyList<IconData>? all;

        [DataMember(Name = "Code")]
        public string Code { get; set; } = string.Empty;

        [DataMember(Name = "Name")]
        public string Name { get; set; } = string.Empty;

        [DataMember(Name = "Tags")]
        public string[]? Tags { get; set; }

        /// <summary>
        /// The code point, which is what tells whether a font has the glyph at all.
        /// </summary>
        public int CodePoint => int.Parse(this.Code, NumberStyles.HexNumber, CultureInfo.InvariantCulture);

        /// <summary>
        /// The character itself, for showing the glyph.
        /// </summary>
        public string Glyph => char.ConvertFromUtf32(this.CodePoint);

        /// <summary>
        /// The glyph the way XAML writes it.
        /// </summary>
        public string TextGlyph => "&#x" + this.Code + ";";

        /// <summary>
        /// The glyph the way C# writes it.
        /// </summary>
        public string CodeGlyph => "\\u" + this.Code;

        /// <summary>
        /// Every entry of the file, read once. The DataContractJsonSerializer is used because it is
        /// there on net472 as well, where System.Text.Json would be another package.
        /// </summary>
        public static IReadOnlyList<IconData> All => all ??= Load();

        private static IReadOnlyList<IconData> Load()
        {
            using var stream = typeof(IconData).Assembly.GetManifestResourceStream(Resource)
                               ?? throw new InvalidOperationException($"The resource {Resource} is missing.");
            return (IconData[])new DataContractJsonSerializer(typeof(IconData[])).ReadObject(stream)!;
        }

        /// <summary>
        /// Whether every term of <paramref name="terms"/> is found in the code, the name or a tag.
        /// </summary>
        public bool Matches(IEnumerable<string> terms)
        {
            foreach (var term in terms)
            {
                if (!this.Contains(term))
                {
                    return false;
                }
            }

            return true;
        }

        private bool Contains(string term)
        {
            if (Contains(this.Code, term) || Contains(this.Name, term))
            {
                return true;
            }

            if (this.Tags is not null)
            {
                foreach (var tag in this.Tags)
                {
                    if (Contains(tag, term))
                    {
                        return true;
                    }
                }
            }

            return false;

            static bool Contains(string candidate, string term)
            {
                return candidate.IndexOf(term, StringComparison.OrdinalIgnoreCase) >= 0;
            }
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return this.Name;
        }
    }
}
