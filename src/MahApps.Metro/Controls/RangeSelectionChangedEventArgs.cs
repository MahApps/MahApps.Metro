using System.Windows;

namespace MahApps.Metro.Controls
{
    /// <summary>
    /// Event arguments created for the RangeSlider's SelectionChanged event.
    /// <see cref="RangeSlider"/>
    /// </summary>
    public class RangeSelectionChangedEventArgs : RoutedEventArgs
    {
        /// <summary>
        /// The value of the new range's beginning.
        /// </summary>
        public double NewLowerValue { get; set; }

        /// <summary>
        /// The value of the new range's ending.
        /// </summary>
        public double NewUpperValue { get; set; }

        public double OldLowerValue { get; set; }

        public double OldUpperValue { get; set; }

        internal RangeSelectionChangedEventArgs(double newLowerValue, double newUpperValue, double oldLowerValue, double oldUpperValue)
        {
            NewLowerValue = newLowerValue;
            NewUpperValue = newUpperValue;
            OldLowerValue = oldLowerValue;
            OldUpperValue = oldUpperValue;
        }
    }
}