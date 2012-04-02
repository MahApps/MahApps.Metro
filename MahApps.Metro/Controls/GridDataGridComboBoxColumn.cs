/*  
 * CustDataGridComboBoxColumn implements data grid column combobox with popup DataGrid control.
 * Bahrudin Hrnjica, bhrnjica@hotmail.com
 * First Release Oct, 2009
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
//using Microsoft.Windows.Controls;
using System.Windows.Markup;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Collections;
using System.Reflection;

namespace MahApps.Metro.Controls
{
    /// <summary>
    /// Follow steps 1a or 1b and then 2 to use this custom control in a XAML file.
    ///
    /// Step 1a) Using this custom control in a XAML file that exists in the current project.
    /// Add this XmlNamespace attribute to the root element of the markup file where it is 
    /// to be used:
    ///
    ///     xmlns:MyNamespace="clr-namespace:EdalSoft.Controls.WPF"
    ///
    ///
    /// Step 1b) Using this custom control in a XAML file that exists in a different project.
    /// Add this XmlNamespace attribute to the root element of the markup file where it is 
    /// to be used:
    ///
    ///     xmlns:MyNamespace="clr-namespace:EdalSoft.Controls.WPF;assembly=EdalSoft.Controls.WPF"
    ///
    /// You will also need to add a project reference from the project where the XAML file lives
    /// to this project and Rebuild to avoid compilation errors:
    ///
    ///     Right click on the target project in the Solution Explorer and
    ///     "Add Reference"->"Projects"->[Browse to and select this project]
    ///
    ///
    /// Step 2)
    /// Go ahead and use your control in the XAML file.
    ///
    ///     <MyNamespace:CustDataGridComboBoxColumn/>
    ///
    /// </summary>
    [DefaultProperty("Columns")]
    [ContentProperty("Columns")]
    public class CustDataGridComboBoxColumn : DataGridComboBoxColumn
    {
        //Columns of DataGrid
        private ObservableCollection<DataGridTextColumn> columns;
        //Cust Combobox  cell edit
        private  GridComboBox comboBox ;
       
        public CustDataGridComboBoxColumn()
        {
            comboBox = new GridComboBox();
        }
        
        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.Property == CustDataGridComboBoxColumn.ItemsSourceProperty)
            {
                comboBox.ItemsSource = ItemsSource;
                

            }
            else if (e.Property == CustDataGridComboBoxColumn.SelectedValuePathProperty)
            {
                
                comboBox.SelectedValuePath = SelectedValuePath;
                

            }
            else if (e.Property == CustDataGridComboBoxColumn.DisplayMemberPathProperty)
            {
                comboBox.DisplayMemberPath = DisplayMemberPath;
            }
            
            base.OnPropertyChanged(e);
        }

        //The property is default and Content property for GridComboBox
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public ObservableCollection<DataGridTextColumn> Columns
        {
            get
            {
                if (this.columns == null)
                {
                    this.columns = new ObservableCollection<DataGridTextColumn>();
                }
                return this.columns;
            }
        }
        /// <summary>
        ///     Creates the visual tree for text based cells.
        /// </summary>
        protected override FrameworkElement GenerateEditingElement(DataGridCell cell, object dataItem)
        {
           if(comboBox.Columns.Count==0)
           {
                //Add columns to DataGrid columns
                for (int i = 0; i < columns.Count; i++)
                    comboBox.Columns.Add(columns[i]);
            }
            return comboBox;
        }
        protected override object PrepareCellForEdit(FrameworkElement editingElement, RoutedEventArgs editingEventArgs)
        {
            DataGridCell cell=editingEventArgs.Source as DataGridCell;
           if (cell != null)
           {
               //For Typed DataSet
               object obj = ((DataRowView)editingElement.DataContext).Row[this.SelectedValuePath];
               comboBox.SelectedValue= obj;
           }

            return comboBox.SelectedItem;
        }
        protected override bool CommitCellEdit(FrameworkElement editingElement)
        {
            ((DataRowView)editingElement.DataContext).Row[this.SelectedValuePath] = comboBox.SelectedValue;
            return true;
        }
    }
}
