// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using System.Windows;
using System.Windows.Markup;
using JetBrains.Annotations;

namespace MahApps.Metro.Converters
{
    /// <summary>
    /// The math operations which can be used at the <see cref="MathConverter"/>
    /// </summary>
    public enum MathOperation
    {
        Add,
        Subtract,
        Multiply,
        Divide
    }

    /// <summary>
    /// MathConverter provides a value converter which can be used for math operations.
    /// It can be used for normal binding or multi binding as well.
    /// If it is used for normal binding the given parameter will be used as operands with the selected operation.
    /// If it is used for multi binding then the first and second binding will be used as operands with the selected operation.
    /// This class cannot be inherited.
    /// </summary>
    [ValueConversion(typeof(object), typeof(object))]
    public sealed class MathConverter : IValueConverter, IMultiValueConverter
    {
        public MathOperation Operation { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DoConvert(value, parameter, this.Operation);
        }

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            return values is null
                ? Binding.DoNothing
                : DoConvert(values.ElementAtOrDefault(0), values.ElementAtOrDefault(1), this.Operation);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return targetTypes.Select(t => DependencyProperty.UnsetValue).ToArray();
        }

        private static object DoConvert([CanBeNull] object firstValue, [CanBeNull] object secondValue, MathOperation operation)
        {
            if (firstValue is null
                || secondValue is null
                || firstValue == DependencyProperty.UnsetValue
                || secondValue == DependencyProperty.UnsetValue
                || firstValue == DBNull.Value
                || secondValue == DBNull.Value)
            {
                return Binding.DoNothing;
            }

            try
            {
                var value1 = (firstValue as double?).GetValueOrDefault(System.Convert.ToDouble(firstValue, CultureInfo.InvariantCulture));
                var value2 = (secondValue as double?).GetValueOrDefault(System.Convert.ToDouble(secondValue, CultureInfo.InvariantCulture));

                switch (operation)
                {
                    case MathOperation.Add: return value1 + value2;
                    case MathOperation.Divide:
                    {
                        if (value2 > 0)
                        {
                            return value1 / value2;
                        }
                        else
                        {
                            Trace.TraceWarning($"Second value can not be used by division, because it's '0' (value1={value1}, value2={value2})");
                            return Binding.DoNothing;
                        }
                    }
                    case MathOperation.Multiply: return value1 * value2;
                    case MathOperation.Subtract: return value1 - value2;
                    default: return Binding.DoNothing;
                }
            }
            catch (Exception e)
            {
                Trace.TraceError($"Error while math operation: operation={operation}, value1={firstValue}, value2={secondValue} => exception: {e}");
                return Binding.DoNothing;
            }
        }
    }

    /// <summary>
    /// MathAddConverter provides a multi value converter as a MarkupExtension which can be used for math operations.
    /// This class cannot be inherited.
    /// </summary>
    [MarkupExtensionReturnType(typeof(MathAddConverter))]
    public sealed class MathAddConverter : MarkupMultiConverter
    {
        private static readonly MathConverter MathConverter = new MathConverter { Operation = MathOperation.Add };

        public override object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            return MathConverter.Convert(values, targetType, parameter, culture);
        }

        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return MathConverter.Convert(value, targetType, parameter, culture);
        }

        public override object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return MathConverter.ConvertBack(value, targetTypes, parameter, culture);
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return MathConverter.ConvertBack(value, targetType, parameter, culture);
        }
    }

    /// <summary>
    /// MathSubtractConverter provides a multi value converter as a MarkupExtension which can be used for math operations.
    /// This class cannot be inherited.
    /// </summary>
    [MarkupExtensionReturnType(typeof(MathSubtractConverter))]
    public sealed class MathSubtractConverter : MarkupMultiConverter
    {
        private static readonly MathConverter MathConverter = new MathConverter { Operation = MathOperation.Subtract };

        public override object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            return MathConverter.Convert(values, targetType, parameter, culture);
        }

        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return MathConverter.Convert(value, targetType, parameter, culture);
        }

        public override object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return MathConverter.ConvertBack(value, targetTypes, parameter, culture);
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return MathConverter.ConvertBack(value, targetType, parameter, culture);
        }
    }

    /// <summary>
    /// MathMultiplyConverter provides a multi value converter as a MarkupExtension which can be used for math operations.
    /// This class cannot be inherited.
    /// </summary>
    [MarkupExtensionReturnType(typeof(MathMultiplyConverter))]
    public sealed class MathMultiplyConverter : MarkupMultiConverter
    {
        private static readonly MathConverter MathConverter = new MathConverter { Operation = MathOperation.Multiply };

        public override object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            return MathConverter.Convert(values, targetType, parameter, culture);
        }

        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return MathConverter.Convert(value, targetType, parameter, culture);
        }

        public override object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return MathConverter.ConvertBack(value, targetTypes, parameter, culture);
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return MathConverter.ConvertBack(value, targetType, parameter, culture);
        }
    }

    /// <summary>
    /// MathDivideConverter provides a multi value converter as a MarkupExtension which can be used for math operations.
    /// This class cannot be inherited.
    /// </summary>
    [MarkupExtensionReturnType(typeof(MathDivideConverter))]
    public sealed class MathDivideConverter : MarkupMultiConverter
    {
        private static readonly MathConverter MathConverter = new MathConverter { Operation = MathOperation.Divide };

        public override object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            return MathConverter.Convert(values, targetType, parameter, culture);
        }

        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return MathConverter.Convert(value, targetType, parameter, culture);
        }

        public override object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return MathConverter.ConvertBack(value, targetTypes, parameter, culture);
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return MathConverter.ConvertBack(value, targetType, parameter, culture);
        }
    }
}