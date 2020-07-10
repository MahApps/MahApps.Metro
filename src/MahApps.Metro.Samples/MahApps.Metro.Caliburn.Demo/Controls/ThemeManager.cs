// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.ComponentModel.Composition;
using System.Windows;

namespace Caliburn.Metro.Demo.Controls
{
    [Export(typeof(IThemeManager))]
    public class ThemeManager : IThemeManager
    {
        private readonly ResourceDictionary themeResources;

        public ThemeManager()
        {
            this.themeResources = new ResourceDictionary
                                  {
                                      Source = new Uri("pack://application:,,,/MahApps.Metro.Caliburn.Demo;component/Resources/Theme1.xaml")
                                  };
        }

        public ResourceDictionary GetThemeResources()
        {
            return this.themeResources;
        }
    }
}