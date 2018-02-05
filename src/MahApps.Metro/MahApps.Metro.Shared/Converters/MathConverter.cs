using System;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using System.Windows;
using System.Windows.Markup;

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
    public sealed class MathConverter : IValueConverter, IMultiValueConverter
    {
        public MathOperation Operation { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DoConvert(value, parameter, this.Operation);
        }

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null || values.Length < 2)
            {
                return Binding.DoNothing;
            }
            return DoConvert(values[0], values[1], this.Operation);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return targetTypes.Select(t => Binding.DoNothing).ToArray();
        }

        private static object DoConvert(object firstValue, object secondValue, MathOperation operation)
        {
            if (firstValue == null
                || secondValue == null
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
                    case MathOperation.Add:
                        return value1 + value2;
                    case MathOperation.Divide:
                        return value1 / value2;
                    case MathOperation.Multiply:
                        return value1 * value2;
                    case MathOperation.Subtract:
                        return value1 - value2;
                    default:
                        return Binding.DoNothing;
                }
            }
            catch (Exception e)
            {
                Trace.TraceError($"Error while converting: value1={firstValue} value2={secondValue} operation={operation} exception: {e}");
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
        private static MathAddConverter _instance;
        private readonly MathConverter theMathConverter = new MathConverter() { Operation = MathOperation.Add };

        // Explicit static constructor to tell C# compiler
        // not to mark type as beforefieldinit
        static MathAddConverter()
        {
        }

        public override object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            return this.theMathConverter.Convert(values, targetType, parameter, culture);
        }

        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return this.theMathConverter.Convert(value, targetType, parameter, culture);
        }

        public override object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return this.theMathConverter.ConvertBack(value, targetTypes, parameter, culture);
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return this.theMathConverter.ConvertBack(value, targetType, parameter, culture);
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return _instance ?? (_instance = new MathAddConverter());
        }
    }

    /// <summary>
    /// MathSubtractConverter provides a multi value converter as a MarkupExtension which can be used for math operations.
    /// This class cannot be inherited.
    /// </summary>
    [MarkupExtensionReturnType(typeof(MathSubtractConverter))]
    public sealed class MathSubtractConverter : MarkupMultiConverter
    {
        private static MathSubtractConverter _instance;
        private readonly MathConverter theMathConverter = new MathConverter() { Operation = MathOperation.Subtract };

        // Explicit static constructor to tell C# compiler
        // not to mark type as beforefieldinit
        static MathSubtractConverter()
        {
        }

        public override object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            return this.theMathConverter.Convert(values, targetType, parameter, culture);
        }

        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return this.theMathConverter.Convert(value, targetType, parameter, culture);
        }

        public override object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return this.theMathConverter.ConvertBack(value, targetTypes, parameter, culture);
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return this.theMathConverter.ConvertBack(value, targetType, parameter, culture);
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return _instance ?? (_instance = new MathSubtractConverter());
        }
    }

    /// <summary>
    /// MathMultiplyConverter provides a multi value converter as a MarkupExtension which can be used for math operations.
    /// This class cannot be inherited.
    /// </summary>
    [MarkupExtensionReturnType(typeof(MathMultiplyConverter))]
    public sealed class MathMultiplyConverter : MarkupMultiConverter
    {
        private static MathMultiplyConverter _instance;
        private readonly MathConverter theMathConverter = new MathConverter() { Operation = MathOperation.Multiply };

        // Explicit static constructor to tell C# compiler
        // not to mark type as beforefieldinit
        static MathMultiplyConverter()
        {
        }

        public override object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            return this.theMathConverter.Convert(values, targetType, parameter, culture);
        }

        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return this.theMathConverter.Convert(value, targetType, parameter, culture);
        }

        public override object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return this.theMathConverter.ConvertBack(value, targetTypes, parameter, culture);
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return this.theMathConverter.ConvertBack(value, targetType, parameter, culture);
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return _instance ?? (_instance = new MathMultiplyConverter());
        }
    }

    /// <summary>
    /// MathDivideConverter provides a multi value converter as a MarkupExtension which can be used for math operations.
    /// This class cannot be inherited.
    /// </summary>
    [MarkupExtensionReturnType(typeof(MathDivideConverter))]
    public sealed class MathDivideConverter : MarkupMultiConverter
    {
        private static MathDivideConverter _instance;
        private readonly MathConverter theMathConverter = new MathConverter() { Operation = MathOperation.Divide };

        // Explicit static constructor to tell C# compiler
        // not to mark type as beforefieldinit
        static MathDivideConverter()
        {
        }

        public override object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            return this.theMathConverter.Convert(values, targetType, parameter, culture);
        }

        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return this.theMathConverter.Convert(value, targetType, parameter, culture);
        }

        public override object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return this.theMathConverter.ConvertBack(value, targetTypes, parameter, culture);
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return this.theMathConverter.ConvertBack(value, targetType, parameter, culture);
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return _instance ?? (_instance = new MathDivideConverter());
        }
    }
}