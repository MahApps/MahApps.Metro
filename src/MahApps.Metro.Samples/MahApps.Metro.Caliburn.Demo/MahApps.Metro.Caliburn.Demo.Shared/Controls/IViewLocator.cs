using System;
using System.Windows;

namespace Caliburn.Metro.Demo.Controls
{
    public interface IViewLocator
    {
        UIElement GetOrCreateViewType(Type viewType);
    }
}