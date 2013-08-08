using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Shapes;

namespace MahApps.Metro.Controls
{
    public class TabBar : Selector
    {
        static TabBar()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TabBar), new FrameworkPropertyMetadata(typeof(TabBar)));
        }

        public static readonly DependencyProperty SidebarWidthProperty =
            DependencyProperty.Register("SidebarWidth", typeof(double), typeof(TabBar), new FrameworkPropertyMetadata(200.0, FrameworkPropertyMetadataOptions.AffectsArrange));
        public double SidebarWidth
        {
            get { return (double)GetValue(SidebarWidthProperty); }
            set { SetValue(SidebarWidthProperty, value); }
        }

        public static readonly DependencyProperty UpperTabBarContentProperty =
            DependencyProperty.Register("UpperTabBarContent", typeof(object), typeof(TabBar), new PropertyMetadata(null));
        public object UpperTabBarContent
        {
            get { return (object)GetValue(UpperTabBarContentProperty); }
            set { SetValue(UpperTabBarContentProperty, value); }
        }

        public static readonly DependencyProperty LowerTabBarContentProperty =
            DependencyProperty.Register("LowerTabBarContent", typeof(object), typeof(TabBar), new PropertyMetadata(null));
        public object LowerTabBarContent
        {
            get { return (object)GetValue(LowerTabBarContentProperty); }
            set { SetValue(LowerTabBarContentProperty, value); }
        }

        public static readonly DependencyProperty IsCollapsedProperty =
            DependencyProperty.Register("IsCollapsed", typeof(bool), typeof(TabBar), new PropertyMetadata(false));
        public bool IsCollapsed
        {
            get { return (bool)GetValue(IsCollapsedProperty); }
            set { SetValue(IsCollapsedProperty, value); }
        }

        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is TabBarItem;
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new TabBarItem();
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            PART_Presenter = (MetroContentControl)GetTemplateChild("PART_Presenter");
            PART_TabItems = (ListBox)GetTemplateChild("PART_TabItems");

            if (HasItems)
            {
                if (Items.Count > 0)
                    PART_TabItems.ItemsSource = Items;
                else
                    PART_TabItems.ItemsSource = ItemsSource;

                SelectedIndex = 0;
            }

            PART_TabItems.SelectionChanged += PART_TabItems_SelectionChanged;

            //PART_Presenter.Margin = new Thickness(SidebarWidth,0,0,0);
        }

        void PART_TabItems_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectedItem = Items[SelectedIndex];
        }

        protected override void OnRender(System.Windows.Media.DrawingContext drawingContext)
        {
            // PART_Presenter.Margin = new Thickness(SidebarWidth, 0, 0, 0);

            base.OnRender(drawingContext);
        }

        internal MetroContentControl PART_Presenter = null;
        private ListBox PART_TabItems = null;
    }
    public class TabBarItem : HeaderedContentControl
    {
        static TabBarItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TabBarItem), new FrameworkPropertyMetadata(typeof(TabBarItem)));
        }

        public static readonly DependencyProperty IconProperty =
            DependencyProperty.Register("Icon", typeof(object), typeof(TabBarItem), new PropertyMetadata(null));
        public object Icon
        {
            get { return GetValue(IconProperty); }
            set { SetValue(IconProperty, value); }
        }
    }
}
