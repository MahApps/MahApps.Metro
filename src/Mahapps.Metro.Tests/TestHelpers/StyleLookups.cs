// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;

namespace MahApps.Metro.Tests.TestHelpers
{
    public static class StyleLookups
    {
        /// <summary>
        /// Whether the style is the expected one or stands on it somewhere down its BasedOn chain.
        /// </summary>
        public static bool StandsOn(Style? style, Style expected)
        {
            for (var current = style; current is not null; current = current.BasedOn)
            {
                if (ReferenceEquals(current, expected))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// A fresh copy of a style set, away from what the application has merged.
        /// </summary>
        public static ResourceDictionary Load(string set)
        {
            return new ResourceDictionary { Source = new Uri(set, UriKind.Absolute) };
        }
    }
}
