using System.Windows;
using System.Windows.Controls;

namespace MahApps.Metro.Controls
{
    using System;
    using System.ComponentModel;

    public class DataGridCellHelper
    {
        [Obsolete(@"This property will be deleted in the next release.")]
        public static readonly DependencyProperty SaveDataGridProperty
            = DependencyProperty.RegisterAttached("SaveDataGrid",
                                                  typeof(bool),
                                                  typeof(DataGridCellHelper),
                                                  new FrameworkPropertyMetadata(default(bool), CellPropertyChangedCallback));

        private static void CellPropertyChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            var cell = dependencyObject as DataGridCell;
            if (cell != null && e.NewValue != e.OldValue && e.NewValue is bool)
            {
                cell.Loaded -= DataGridCellLoaded;
                cell.Unloaded -= DataGridCellUnloaded;
                DataGrid dataGrid = null;
                if ((bool)e.NewValue)
                {
                    dataGrid = cell.TryFindParent<DataGrid>();
                    cell.Loaded += DataGridCellLoaded;
                    cell.Unloaded += DataGridCellUnloaded;
                }
                SetDataGrid(cell, dataGrid);
            }
        }

        static void DataGridCellLoaded(object sender, RoutedEventArgs e)
        {
            var cell = (DataGridCell)sender;
            if (GetDataGrid(cell) == null)
            {
                var dataGrid = cell.TryFindParent<DataGrid>();
                SetDataGrid(cell, dataGrid);
            }
        }

        static void DataGridCellUnloaded(object sender, RoutedEventArgs e)
        {
            SetDataGrid((DataGridCell)sender, null);
        }

        /// <summary>
        /// Save the DataGrid.
        /// </summary>
        [AttachedPropertyBrowsableForType(typeof(DataGridCell))]
        [Obsolete(@"This property will be deleted in the next release.")]
        public static bool GetSaveDataGrid(UIElement element)
        {
            return (bool)element.GetValue(SaveDataGridProperty);
        }

        [Obsolete(@"This property will be deleted in the next release.")]
        public static void SetSaveDataGrid(UIElement element, bool value)
        {
            element.SetValue(SaveDataGridProperty, value);
        }

        [Obsolete(@"This property will be deleted in the next release.")]
        public static readonly DependencyProperty DataGridProperty
            = DependencyProperty.RegisterAttached("DataGrid",
                                                  typeof(DataGrid),
                                                  typeof(DataGridCellHelper),
                                                  new FrameworkPropertyMetadata(default(DataGrid)));

        /// <summary>
        /// Get the DataGrid.
        /// </summary>
        [AttachedPropertyBrowsableForType(typeof(DataGridCell))]
        [Obsolete(@"This property will be deleted in the next release.")]
        public static DataGrid GetDataGrid(UIElement element)
        {
            return (DataGrid)element.GetValue(DataGridProperty);
        }

        [Obsolete(@"This property will be deleted in the next release.")]
        public static void SetDataGrid(UIElement element, DataGrid value)
        {
            element.SetValue(DataGridProperty, value);
        }

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