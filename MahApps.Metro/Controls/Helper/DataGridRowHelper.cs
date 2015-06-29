using System.Windows;
using System.Windows.Controls;

namespace MahApps.Metro.Controls
{
    public static class DataGridRowHelper
    {
        public static readonly DependencyProperty SelectionUnitProperty = DependencyProperty.RegisterAttached("SelectionUnit", typeof(DataGridSelectionUnit), typeof(DataGridRowHelper), new FrameworkPropertyMetadata(DataGridSelectionUnit.FullRow));

        /// <summary>
        /// Gets the value to define the DataGridRow selection behavior.
        /// </summary>
        [AttachedPropertyBrowsableForType(typeof(DataGridRow))]
        public static DataGridSelectionUnit GetSelectionUnit(UIElement element)
        {
            return (DataGridSelectionUnit)element.GetValue(SelectionUnitProperty);
        }

        /// <summary>
        /// Sets the value to define the DataGridRow selection behavior.
        /// </summary>
        public static void SetSelectionUnit(UIElement element, DataGridSelectionUnit value)
        {
            element.SetValue(SelectionUnitProperty, value);
        }

    }
}