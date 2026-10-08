// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;

namespace MahApps.Metro.Gallery.Core
{
    /// <summary>
    /// Cuts named regions out of a source file, for a card that shows the code that runs.
    /// </summary>
    public static class CodeRegions
    {
        /// <summary>
        /// The lines between <c>#region name</c> and the <c>#endregion</c> that closes it, for every
        /// name in <paramref name="names"/> (separated by a semicolon), without the indentation they
        /// share and with an empty line between two regions.
        /// </summary>
        public static string Cut(string text, string names)
        {
            var lines = text.Replace("\r\n", "\n").Split('\n');
            var parts = new List<string>();

            foreach (var name in names.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries).Select(n => n.Trim()))
            {
                var start = Array.FindIndex(lines, l => l.Trim() == "#region " + name);
                if (start < 0)
                {
                    parts.Add($"// there is no region \"{name}\" in this file");
                    continue;
                }

                var depth = 0;
                var end = start + 1;
                for (; end < lines.Length; end++)
                {
                    var trimmed = lines[end].Trim();
                    if (trimmed.StartsWith("#region", StringComparison.Ordinal))
                    {
                        depth++;
                    }
                    else if (trimmed.StartsWith("#endregion", StringComparison.Ordinal))
                    {
                        if (depth == 0)
                        {
                            break;
                        }

                        depth--;
                    }
                }

                var body = lines.Skip(start + 1).Take(end - start - 1).ToList();
                var indent = body.Where(l => l.Trim().Length > 0).Select(l => l.Length - l.TrimStart().Length).DefaultIfEmpty(0).Min();
                parts.Add(string.Join(Environment.NewLine, body.Select(l => l.Length >= indent ? l.Substring(indent) : l.TrimStart())).Trim('\r', '\n'));
            }

            return string.Join(Environment.NewLine + Environment.NewLine, parts);
        }
    }
}
