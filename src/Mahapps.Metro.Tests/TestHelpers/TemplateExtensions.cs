// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows;
using System.Windows.Controls;
using MahApps.Metro.Controls;
using NUnit.Framework;

namespace MahApps.Metro.Tests.TestHelpers
{
    public static class TemplateExtensions
    {
        /// <summary>
        /// A named part of the control's own template. The boxes inside a template often carry parts
        /// of the same names, and a walk down the tree reaches those first.
        /// </summary>
        public static T PartOf<T>(this Control control, string name)
            where T : FrameworkElement
        {
            var part = control.Template?.FindName(name, control) as T;

            Assert.That(part, Is.Not.Null, $"the template should carry {name}");

            return part!;
        }

        /// <summary>
        /// A named element down the visual tree, wherever in the nested templates it stands.
        /// </summary>
        public static T NamedChild<T>(this DependencyObject parent, string name)
            where T : DependencyObject
        {
            var child = parent.FindChild<T>(name);

            Assert.That(child, Is.Not.Null, $"the template should carry {name}");

            return child!;
        }
    }
}
