// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Caliburn.Micro;

namespace Caliburn.Metro.Demo.Controls
{
    [Export(typeof(IViewLocator))]
    public class ViewLocator : IViewLocator
    {
        private readonly IThemeManager themeManager;

        [ImportingConstructor]
        public ViewLocator(IThemeManager themeManager)
        {
            this.themeManager = themeManager;
        }

        public UIElement GetOrCreateViewType(Type viewType)

        {
            var cached = IoC.GetAllInstances(viewType).OfType<UIElement>().FirstOrDefault();
            if (cached != null)
            {
                Micro.ViewLocator.InitializeComponent(cached);
                return cached;
            }

            if (viewType.IsInterface || viewType.IsAbstract || !typeof(UIElement).IsAssignableFrom(viewType))
            {
                return new TextBlock { Text = string.Format("Cannot create {0}.", viewType.FullName) };
            }

            var newInstance = (UIElement)Activator.CreateInstance(viewType);

            Micro.ViewLocator.InitializeComponent(newInstance);

            // alternative way to use the MahApps resources
            // (newInstance as Window)?.Resources.MergedDictionaries.Add(themeManager.GetThemeResources());

            return newInstance;
        }
    }
}