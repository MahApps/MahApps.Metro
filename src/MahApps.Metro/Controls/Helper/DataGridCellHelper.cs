using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace MahApps.Metro.Controls
{
    public class DataGridCellHelper
    {
        public static readonly DependencyProperty SelectionUnitProperty
            = DependencyProperty.RegisterAttached("SelectionUnit",
                                                  typeof(DataGridSelectionUnit),
                                                  typeof(DataGridCellHelper),
                                                  new FrameworkPropertyMetadata(DataGridSelectionUnit.Cell, SelectionUnitOnPropertyChangedCallback));

        private static void SelectionUnitOnPropertyChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
        {
            if (args.OldValue != args.NewValue)
            {
                var cell = (DataGridCell)dependencyObject;
                SetIsCellOrRowHeader(cell, !Equals(args.NewValue, DataGridSelectionUnit.FullRow));
            }
        }

        /// <summary>
        /// Gets the value to define the DataGridCell selection behavior.
        /// </summary>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(DataGridCell))]
        public static DataGridSelectionUnit GetSelectionUnit(UIElement element)
        {
            return (DataGridSelectionUnit)element.GetValue(SelectionUnitProperty);
        }

        /// <summary>
        /// Sets the value to define the DataGridCell selection behavior.
        /// </summary>
        public static void SetSelectionUnit(UIElement element, DataGridSelectionUnit value)
        {
            element.SetValue(SelectionUnitProperty, value);
        }

        public static readonly DependencyProperty IsCellOrRowHeaderProperty
            = DependencyProperty.RegisterAttached("IsCellOrRowHeader",
                                                  typeof(bool),
                                                  typeof(DataGridCellHelper),
                                                  new FrameworkPropertyMetadata(true));

        /// <summary>
        /// Gets the value to define the DataGridCell selection behavior.
        /// </summary>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(DataGridCell))]
        public static bool GetIsCellOrRowHeader(UIElement element)
        {
            return (bool)element.GetValue(IsCellOrRowHeaderProperty);
        }

        /// <summary>
        /// Sets the value to define the DataGridCell selection behavior.
        /// </summary>
        internal static void SetIsCellOrRowHeader(UIElement element, bool value)
        {
            element.SetValue(IsCellOrRowHeaderProperty, value);
        }
    }
}