// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Windows;

namespace MahApps.Metro.Tests.TestHelpers
{
    public static class DependencyObjectExtensions
    {
        public static IEnumerable<DependencyProperty> EnumerateDependencyProperties(this DependencyObject? obj)
        {
            if (obj is null)
            {
                yield break;
            }

            var localValueEnumerator = obj.GetLocalValueEnumerator();
            while (localValueEnumerator.MoveNext())
            {
                yield return localValueEnumerator.Current.Property;
            }
        }

        public static void ClearDependencyProperties(this DependencyObject? obj, IList<string>? properties = null)
        {
            if (obj is null)
            {
                return;
            }

            foreach (var property in EnumerateDependencyProperties(obj))
            {
                if (property.ReadOnly == false)
                {
                    if (properties is null || properties.Contains(property.Name))
                    {
                        obj.ClearValue(property);
                    }
                }
            }
        }
    }
}