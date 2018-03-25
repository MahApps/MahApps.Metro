using System;
using System.Windows;

namespace MahApps.Metro.Controls
{
    public class RangeParameterChangedEventArgs : RoutedEventArgs
    {
        public RangeParameterChangeType ParameterType { get; private set; }
        public Double OldValue { get; private set; }
        public Double NewValue { get; private set; }

        internal RangeParameterChangedEventArgs(RangeParameterChangeType type, Double _old, Double _new)
        {
            ParameterType = type;

            OldValue = _old;
            NewValue = _new;
        }
    }

    public enum RangeParameterChangeType
    {
        Lower = 1,
        Upper = 2
    }
}