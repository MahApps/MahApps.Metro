namespace Caliburn.Metro.Demo.Controls
{
    using System;
    using System.Windows;

    public interface IViewLocator
    {
        #region Public Methods and Operators

        UIElement GetOrCreateViewType(Type viewType);

        #endregion
    }
}