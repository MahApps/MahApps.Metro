// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows.Media;
using NUnit.Framework;

namespace MahApps.Metro.Tests.TestHelpers
{
    public static class Colours
    {
        /// <summary>
        /// The colour of a brush a test expects to be plain, a resource looked up included.
        /// </summary>
        public static Color ColourOf(object? brush)
        {
            Assert.That(brush, Is.InstanceOf<SolidColorBrush>(), "this one should be a brush of a single colour");

            return ((SolidColorBrush)brush!).Color;
        }
    }
}
