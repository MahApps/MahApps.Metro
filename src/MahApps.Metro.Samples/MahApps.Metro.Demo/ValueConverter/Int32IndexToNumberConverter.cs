using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace MetroDemo.ValueConverter
{
    /// <summary>
    /// Converts an Int32 zero-based index to a one-based number.
    /// </summary>
    public class Int32IndexToNumberConverter
        : MarkupExtension , IValueConverter
    {
        /// <summary>
        /// Returns the value for the target property of this markup extension.
        /// </summary>
        /// <param name="serviceProvider">Object that can provide services for the markup extension.</param>
        /// <returns>Reference to the instance of this Int32IndexToNumberConverter.</returns>
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }

        /// <summary>
        /// Converts an Int32 zero-based index to a one-based number.
        /// </summary>
        /// <param name="value">Int32 zero-based index.</param>
        /// <param name="targetType">Ignored.</param>
        /// <param name="parameter">Ignored.</param>
        /// <param name="culture">Ignored.</param>
        /// <returns>Int32 one-based number.</returns>
        /// <exception cref="FormatException">Incorrect format.</exception>
        /// <exception cref="InvalidCastException">Unsupported convversion.</exception>
        /// <exception cref="OverflowException">Out of range of Int32.</exception>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return System.Convert.ToInt32(value) + 1;
        }

        /// <summary>
        /// Converts an Int32 one-based number to a zero-based index.
        /// </summary>
        /// <param name="value">Int32 one-based number.</param>
        /// <param name="targetType">Ignored.</param>
        /// <param name="parameter">Ignored.</param>
        /// <param name="culture">Ignored.</param>
        /// <returns>Int32 zero-based index.</returns>
        /// <exception cref="FormatException">Incorrect format.</exception>
        /// <exception cref="InvalidCastException">Unsupported convversion.</exception>
        /// <exception cref="OverflowException">Out of range of Int32.</exception>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return System.Convert.ToInt32(value) - 1;
        }
    }
}
