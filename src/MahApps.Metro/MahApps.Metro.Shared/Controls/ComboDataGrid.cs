using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MahApps.Metro.Controls
{
    [TemplatePart(Name = "PART_DataGrid", Type = typeof(DataGrid))]
    public class ComboDataGrid : ComboBox
    {
        private readonly ObservableCollection<DataGridColumn> columns;
        private DataGrid dataGrid;

        static ComboDataGrid()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ComboDataGrid), new FrameworkPropertyMetadata(typeof(ComboDataGrid)));
        }

        public ComboDataGrid()
        {
            this.columns = new ObservableCollection<DataGridColumn>();
            this.columns.CollectionChanged += this.OnColumnsCollectionChanged;
        }

        public static readonly DependencyProperty AutoGenerateColumnsProperty = DependencyProperty.Register("AutoGenerateColumns", typeof(bool), typeof(ComboDataGrid), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public bool AutoGenerateColumns
        {
            get { return (bool)GetValue(AutoGenerateColumnsProperty); }
            set { SetValue(AutoGenerateColumnsProperty, value); }
        }

        public ObservableCollection<DataGridColumn> Columns
        {
            get { return this.columns; }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this.dataGrid = (DataGrid)this.GetTemplateChild("PART_DataGrid");
            if (dataGrid == null)
            {
                return;
            }
            this.dataGrid.MouseLeftButtonUp += this.OnDataGridMouseLeftButtonUp;
            this.UpdateDataGridColumns();
        }

        private void OnColumnsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (this.dataGrid != null)
            {
                this.UpdateDataGridColumns();
            }
        }

        private void OnDataGridMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            this.IsDropDownOpen = false;
        }

        private void UpdateDataGridColumns()
        {
            this.dataGrid.Columns.Clear();
            foreach (DataGridColumn column in this.Columns)
            {
                column.IsReadOnly = true;
                this.dataGrid.Columns.Add(column);
            }
        }
    }
}
