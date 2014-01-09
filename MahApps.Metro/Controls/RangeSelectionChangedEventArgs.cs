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
        public double NewRangeStart { get; set; }
        /// <summary>
        /// The value of the new range's ending.
        /// </summary>
        public double NewRangeStop { get; set; }

        internal RangeSelectionChangedEventArgs(double newRangeStart, double newRangeStop)
        {
            NewRangeStart = newRangeStart;
            NewRangeStop = newRangeStop;
        }

        internal RangeSelectionChangedEventArgs(RangeSlider slider)
            : this(slider.LowerValue, slider.UpperValue)
        { }
    }
}