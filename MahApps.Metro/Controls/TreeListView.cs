using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace MahApps.Metro.Controls
{
    //Based on https://blogs.msdn.microsoft.com/atc_avalon_team/2006/03/01/treelistview-show-hierarchy-data-with-details-in-columns/
    public class TreeListView : TreeView
    {
        public static readonly DependencyProperty ViewProperty = DependencyProperty.Register(
            "View", typeof(GridView), typeof(TreeListView), new PropertyMetadata(default(GridView), OnViewChanged));

        public static readonly DependencyProperty SelectItemOnRightClickProperty = DependencyProperty.Register(
            "SelectItemOnRightClick", typeof(bool), typeof(TreeListView), new PropertyMetadata(true));

        public static readonly DependencyProperty SelectedItemExProperty = DependencyProperty.Register(
            "SelectedItemEx", typeof(object), typeof(TreeListView), new PropertyMetadata(default(object)));

        public static readonly DependencyProperty ChildrenPropertyNameProperty = DependencyProperty.Register(
            "ChildrenPropertyName", typeof(string), typeof(TreeListView),
            new PropertyMetadata(default(string), OnChildrenPropertyNameChanged));

        //Important: Disable property ItemTemplate
        public static readonly DependencyPropertyKey ReadOnlyItemTemplateProperty = DependencyProperty.RegisterReadOnly(
            "ItemTemplate", typeof(DataTemplate), typeof(TreeListView), new PropertyMetadata(default(DataTemplate)));

        public new static readonly DependencyProperty ItemTemplateProperty
            = ReadOnlyItemTemplateProperty.DependencyProperty;

        private GridViewColumn _currentGridViewColumn;
        private bool _currentGridViewColumnCustomTemplate;
        private DataTemplate _oldDataTemplate;
        private BindingBase _oldDisplayMemberBindingBase;
        private GridViewColumnCollection _oldGridViewColumnCollection;

        static TreeListView()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TreeListView), new FrameworkPropertyMetadata(typeof(TreeListView)));
        }

        public new DataTemplate ItemTemplate
        {
            get { return (DataTemplate)GetValue(ItemTemplateProperty); }
            protected set { SetValue(ItemTemplateProperty, value); }
        }

        public string ChildrenPropertyName
        {
            get { return (string)GetValue(ChildrenPropertyNameProperty); }
            set { SetValue(ChildrenPropertyNameProperty, value); }
        }

        public GridView View
        {
            get { return (GridView)GetValue(ViewProperty); }
            set { SetValue(ViewProperty, value); }
        }

        public object SelectedItemEx
        {
            get { return GetValue(SelectedItemExProperty); }
            set { SetValue(SelectedItemExProperty, value); }
        }

        public bool SelectItemOnRightClick
        {
            get { return (bool)GetValue(SelectItemOnRightClickProperty); }
            set { SetValue(SelectItemOnRightClickProperty, value); }
        }

        private static void OnViewChanged(DependencyObject dependencyObject,
            DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var treeListView = (TreeListView)dependencyObject;
            var gridView = (GridView)dependencyPropertyChangedEventArgs.NewValue;
            treeListView.OnUpdateGridView(gridView?.Columns);
        }

        private void OnUpdateGridView(GridViewColumnCollection gridViewColumnCollection)
        {
            var isOldGridView = gridViewColumnCollection == _oldGridViewColumnCollection;

            if (!isOldGridView && _oldGridViewColumnCollection != null)
            {
                //unsubscribe old GridView
                _oldGridViewColumnCollection.CollectionChanged -= ColumnsOnCollectionChanged;
                ResetCurrentGridViewColumn();
                _oldGridViewColumnCollection = null;
            }

            if (gridViewColumnCollection == null)
                return;

            if (!isOldGridView)
                gridViewColumnCollection.CollectionChanged += ColumnsOnCollectionChanged;

            if (gridViewColumnCollection.Count == 0)
                return;

            var firstColumn = gridViewColumnCollection[0];
            ResetCurrentGridViewColumn();
            _currentGridViewColumn = firstColumn;

            if (firstColumn.CellTemplate == null)
            {
                _oldDataTemplate = firstColumn.CellTemplate;
                var dataTemplate = new DataTemplate();
                var spFactory = new FrameworkElementFactory(typeof(ContentPresenter));
                spFactory.SetBinding(ContentPresenter.ContentProperty, firstColumn.DisplayMemberBinding);
                spFactory.SetBinding(MarginProperty,
                    new Binding
                    {
                        RelativeSource =
                            new RelativeSource(RelativeSourceMode.FindAncestor, typeof(TreeListViewItem), 1),
                        Converter = (IValueConverter)Application.Current.Resources["LengthConverter"]
                    });

                dataTemplate.VisualTree = spFactory;
                _oldDisplayMemberBindingBase = firstColumn.DisplayMemberBinding;
                firstColumn.DisplayMemberBinding = null;
                firstColumn.CellTemplate = dataTemplate;
                _currentGridViewColumnCustomTemplate = false;
            }
            else
            {
                _oldDataTemplate = firstColumn.CellTemplate;
                var dataTemplate = new DataTemplate();
                var spFactory = new FrameworkElementFactory(typeof(ContentPresenter));
                spFactory.SetValue(ContentPresenter.ContentTemplateProperty, firstColumn.CellTemplate);
                spFactory.SetBinding(ContentPresenter.ContentProperty, new Binding("."));
                spFactory.SetBinding(MarginProperty,
                    new Binding
                    {
                        RelativeSource =
                            new RelativeSource(RelativeSourceMode.FindAncestor, typeof(TreeListViewItem), 1),
                        Converter = (IValueConverter)Application.Current.Resources["LengthConverter"]
                    });
                dataTemplate.VisualTree = spFactory;

                firstColumn.CellTemplate = dataTemplate;
                _currentGridViewColumnCustomTemplate = true;
            }

            _oldGridViewColumnCollection = gridViewColumnCollection;
        }

        private void ResetCurrentGridViewColumn()
        {
            if (_currentGridViewColumn == null)
                return;

            if (_currentGridViewColumnCustomTemplate)
            {
                _currentGridViewColumn.CellTemplate = _oldDataTemplate;
            }
            else
            {
                _currentGridViewColumn.CellTemplate = null;
                _currentGridViewColumn.DisplayMemberBinding = _oldDisplayMemberBindingBase;
            }

            _oldDataTemplate = null;
            _currentGridViewColumnCustomTemplate = false;
            _oldDisplayMemberBindingBase = null;
        }

        private void ColumnsOnCollectionChanged(object sender,
            NotifyCollectionChangedEventArgs notifyCollectionChangedEventArgs)
        {
            OnUpdateGridView((GridViewColumnCollection)sender);
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Controls.TreeView.SelectedItemChanged"/> event when the <see cref="P:System.Windows.Controls.TreeView.SelectedItem"/> property value changes.
        /// </summary>
        /// <param name="e">Provides the item that was previously selected and the item that is currently selected for the <see cref="E:System.Windows.Controls.TreeView.SelectedItemChanged"/> event.</param>
        protected override void OnSelectedItemChanged(RoutedPropertyChangedEventArgs<object> e)
        {
            base.OnSelectedItemChanged(e);
            SelectedItemEx = e.NewValue;
        }

        private static void OnChildrenPropertyNameChanged(DependencyObject dependencyObject,
            DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var treeListView = (TreeListView)dependencyObject;
            var newValue = (string)dependencyPropertyChangedEventArgs.NewValue;
            treeListView.UpdateItemTemplate(new HierarchicalDataTemplate { ItemsSource = new Binding(newValue) });
        }

        private void UpdateItemTemplate(DataTemplate dataTemplate)
        {
            base.ItemTemplate = dataTemplate;
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new TreeListViewItem();
        }

        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is TreeListViewItem;
        }

        protected override void OnPreviewMouseRightButtonDown(MouseButtonEventArgs e)
        {
            base.OnPreviewMouseRightButtonDown(e);
            if (!SelectItemOnRightClick)
                return;

            var treeListViewItem = (e.OriginalSource as DependencyObject).TryFindParent<TreeListViewItem>();
            if (treeListViewItem != null)
            {
                treeListViewItem.Focus();
                treeListViewItem.IsSelected = true;
                e.Handled = true;
            }
        }
    }
}