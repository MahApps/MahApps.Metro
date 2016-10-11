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
                                          Source = new Uri("pack://application:,,,/Caliburn.Metro.Demo;component/Resources/Theme1.xaml")
                                      };
        }

        public ResourceDictionary GetThemeResources()
        {
            return this.themeResources;
        }
    }
}