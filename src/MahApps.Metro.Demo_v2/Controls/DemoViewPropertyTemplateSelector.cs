// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows;
using System.Windows.Controls;

namespace MahApps.Demo.Controls
{
    public class DemoViewPropertyTemplateSelector : DataTemplateSelector
    {
        public DataTemplate FallbackTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is DemoViewProperty demoProperty)
            {
                return demoProperty.DataTemplate ?? this.FallbackTemplate;
            }
            else
            {
                return this.FallbackTemplate;
            }
        }
    }
}