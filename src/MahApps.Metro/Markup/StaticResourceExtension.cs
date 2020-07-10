// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows;
using System.Windows.Markup;

namespace MahApps.Metro.Markup
{
    /// <summary>
    /// Implements a markup extension that supports static (XAML load time) resource references made from XAML.
    /// </summary>
    [MarkupExtensionReturnType(typeof(object))]
    [Localizability(LocalizationCategory.NeverLocalize)]
    public class StaticResourceExtension : System.Windows.StaticResourceExtension
    {
        public StaticResourceExtension()
        {
        }

        public StaticResourceExtension(object resourceKey)
            : base(resourceKey)
        {
        }
    }
}