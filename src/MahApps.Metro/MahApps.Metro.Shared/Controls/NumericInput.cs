using System;

namespace MahApps.Metro.Controls
{
    /// <summary>
    /// Enum NumericInput which indicates what input is allowed for NumericUpdDown.
    /// </summary>
    [Flags]
    public enum NumericInput
    {
        /// <summary>
        /// Only numbers are allowed
        /// </summary>
        Numbers = 1 << 1, // Only Numbers
        /// <summary>
        /// Decimal numbers are allowed
        /// </summary>
        Decimal = 2 << 1, // Numbers with decimal point
        /// <summary>
        /// Scientific is allowed for (decimal) numbers
        /// </summary>
        Scientific = 3 << 1, // Numbers and Decimal with scientific
        /// <summary>
        /// All is allowed
        /// </summary>
        All = Decimal | Scientific
    }
}