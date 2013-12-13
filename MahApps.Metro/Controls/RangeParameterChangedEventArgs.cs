using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace MahApps.Metro.Controls
{
    public class RangeParameterChangedEventArgs : RoutedEventArgs
    {
        public RangeParameterChangeType ParameterType { get; private set; }
        public long OldValue { get; private set; }
        public long NewValue { get; private set; }

        internal RangeParameterChangedEventArgs(RangeParameterChangeType type, long old, long _new)
        {
            ParameterType = type;

            OldValue = old;
            NewValue = _new;
        }
    }

    public enum RangeParameterChangeType
    {
        Start = 1,
        Stop = 2
    }
}
