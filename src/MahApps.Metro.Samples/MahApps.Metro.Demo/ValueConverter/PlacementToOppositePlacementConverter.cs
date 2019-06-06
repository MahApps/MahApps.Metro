using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Markup;

namespace MetroDemo.ValueConverter
{
    internal class PlacementToOppositePlacementConverter : MarkupExtension, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch (value as Dock?)
            {
                case Dock.Bottom:
                    return Dock.Top;

                case Dock.Left:
                    return Dock.Right;

                case Dock.Right:
                    return Dock.Left;

                case Dock.Top:
                default:
                    return Dock.Bottom;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }
}
