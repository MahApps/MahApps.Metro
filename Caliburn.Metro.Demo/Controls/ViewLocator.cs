﻿namespace Caliburn.Metro.Demo.Controls
{
    using System;
    using System.ComponentModel.Composition;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;

    using Caliburn.Micro;

    [Export(typeof(IViewLocator))]
    public class ViewLocator : IViewLocator
    {
        #region Fields

        private readonly IThemeManager themeManager;

        #endregion

        #region Constructors and Destructors

        [ImportingConstructor]
        public ViewLocator(IThemeManager themeManager)
        {
            this.themeManager = themeManager;
        }

        #endregion

        #region Public Methods and Operators

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
            var frameworkElement = newInstance as FrameworkElement;
            if (frameworkElement != null)
            {
                frameworkElement.Resources.MergedDictionaries.Add(this.themeManager.GetThemeResources());
            }

            Micro.ViewLocator.InitializeComponent(newInstance);
            return newInstance;
        }

        #endregion
    }
}