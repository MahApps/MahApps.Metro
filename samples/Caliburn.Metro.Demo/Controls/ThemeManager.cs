namespace Caliburn.Metro.Demo.Controls
{
    using System;
    using System.ComponentModel.Composition;
    using System.Windows;

    [Export(typeof(IThemeManager))]
    public class ThemeManager : IThemeManager
    {
        #region Fields

        private readonly ResourceDictionary themeResources;

        #endregion

        #region Constructors and Destructors

        public ThemeManager()
        {
            this.themeResources = new ResourceDictionary
                                      {
                                          Source =
                                              new Uri("pack://application:,,,/Resources/Theme1.xaml")
                                      };
        }

        #endregion

        #region Public Methods and Operators

        public ResourceDictionary GetThemeResources()
        {
            return this.themeResources;
        }

        #endregion
    }
}