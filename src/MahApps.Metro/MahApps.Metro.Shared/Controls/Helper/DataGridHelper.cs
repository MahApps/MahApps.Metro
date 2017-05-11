using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace MahApps.Metro.Controls
{
    public static class DataGridHelper
    {
        private static DataGrid _suppressComboAutoDropDown;

        public static readonly DependencyProperty EnableCellEditAssistProperty
            = DependencyProperty.RegisterAttached(
                "EnableCellEditAssist",
                typeof(bool),
                typeof(DataGridHelper),
                new PropertyMetadata(default(bool), EnableCellEditAssistPropertyChangedCallback));

        /// <summary>
        /// Gets a value which indicates the preview cell editing is enabled or not.
        /// </summary>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(DataGrid))]
        public static bool GetEnableCellEditAssist(DependencyObject element)
        {
            return (bool)element.GetValue(EnableCellEditAssistProperty);
        }

        /// <summary>
        /// Sets a value which indicates the preview cell editing is enabled or not.
        /// </summary>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(DataGrid))]
        public static void SetEnableCellEditAssist(DependencyObject element, bool value)
        {
            element.SetValue(EnableCellEditAssistProperty, value);
        }

        private static void EnableCellEditAssistPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var dataGrid = d as DataGrid;
            if (dataGrid == null)
            {
                return;
            }

            dataGrid.PreviewMouseLeftButtonDown -= DataGridOnPreviewMouseLeftButtonDown;
            if ((bool)e.NewValue)
            {
                dataGrid.PreviewMouseLeftButtonDown += DataGridOnPreviewMouseLeftButtonDown;
            }
        }

        private static void DataGridOnPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var dataGrid = (DataGrid)sender;

            var inputHitTest = dataGrid.InputHitTest(e.GetPosition((DataGrid)sender)) as DependencyObject;

            while (inputHitTest != null)
            {
                var dataGridCell = inputHitTest as DataGridCell;
                if (dataGridCell != null && dataGrid.Equals(dataGridCell.TryFindParent<DataGrid>()))
                {
                    if (dataGridCell.IsReadOnly) return;

                    ToggleButton toggleButton;
                    ComboBox comboBox;
                    if (IsDirectHitOnEditComponent(dataGridCell, e, out toggleButton))
                    {
                        dataGrid.CurrentCell = new DataGridCellInfo(dataGridCell);
                        dataGrid.BeginEdit();
                        toggleButton.SetCurrentValue(ToggleButton.IsCheckedProperty, !toggleButton.IsChecked);
                        dataGrid.CommitEdit();
                        e.Handled = true;
                    }
                    else if (IsDirectHitOnEditComponent(dataGridCell, e, out comboBox))
                    {
                        if (_suppressComboAutoDropDown != null) return;

                        dataGrid.CurrentCell = new DataGridCellInfo(dataGridCell);
                        dataGrid.BeginEdit();
                        //check again, as we move to  the edit  template
                        if (IsDirectHitOnEditComponent(dataGridCell, e, out comboBox))
                        {
                            _suppressComboAutoDropDown = dataGrid;
                            comboBox.DropDownClosed += ComboBoxOnDropDownClosed;
                            comboBox.IsDropDownOpen = true;
                        }
                        e.Handled = true;
                    }

                    return;
                }

                inputHitTest = (inputHitTest is Visual || inputHitTest is Visual3D)
                    ? VisualTreeHelper.GetParent(inputHitTest)
                    : null;
            }
        }

        private static void ComboBoxOnDropDownClosed(object sender, EventArgs eventArgs)
        {
            _suppressComboAutoDropDown.CommitEdit();
            _suppressComboAutoDropDown = null;
            ((ComboBox)sender).DropDownClosed -= ComboBoxOnDropDownClosed;
        }

        private static bool IsDirectHitOnEditComponent<TControl>(ContentControl contentControl, MouseEventArgs e, out TControl control)
            where TControl : Control
        {
            control = contentControl.Content as TControl;
            if (control == null)
            {
                return false;
            }

            var frameworkElement = VisualTreeHelper.GetChild(contentControl, 0) as FrameworkElement;
            if (frameworkElement == null)
            {
                return false;
            }

            var transformToAncestor = (MatrixTransform)control.TransformToAncestor(frameworkElement);
            var rect = new Rect(new Point(transformToAncestor.Value.OffsetX, transformToAncestor.Value.OffsetY),
                                new Size(control.ActualWidth, control.ActualHeight));

            return rect.Contains(e.GetPosition(frameworkElement));
        }
    }
}