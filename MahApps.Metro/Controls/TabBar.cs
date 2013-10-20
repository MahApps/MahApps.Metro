using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media.Animation;
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
            DependencyProperty.Register("UpperTabBarContent", typeof(object), typeof(TabBar), new PropertyMetadata(null, OnUpperTabBarContentChanged));
        public object UpperTabBarContent
        {
            get { return (object)GetValue(UpperTabBarContentProperty); }
            set { SetValue(UpperTabBarContentProperty, value); }
        }
        private static void OnUpperTabBarContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((TabBar)d).OnUpperTabBarContentChanged(e);
        }
        protected virtual void OnUpperTabBarContentChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue != null)
            {
                RemoveLogicalChild(e.OldValue);
            }

            if (e.NewValue != null)
            {
                AddLogicalChild(e.NewValue);
            }
        }

        public static readonly DependencyProperty LowerTabBarContentProperty =
            DependencyProperty.Register("LowerTabBarContent", typeof(object), typeof(TabBar), new PropertyMetadata(null, OnLowerTabBarContentChanged));
        public object LowerTabBarContent
        {
            get { return (object)GetValue(LowerTabBarContentProperty); }
            set { SetValue(LowerTabBarContentProperty, value); }
        }
        private static void OnLowerTabBarContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((TabBar)d).OnLowerTabBarContentChanged(e);
        }
        protected virtual void OnLowerTabBarContentChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue != null)
            {
                RemoveLogicalChild(e.OldValue);
            }

            if (e.NewValue != null)
            {
                AddLogicalChild(e.NewValue);
            }
        }

        public static readonly DependencyProperty IsCollapsedProperty =
            DependencyProperty.Register("IsCollapsed", typeof(bool), typeof(TabBar), new PropertyMetadata(false));
        public bool IsCollapsed
        {
            get { return (bool)GetValue(IsCollapsedProperty); }
            set
            {
                if ((bool)GetValue(IsCollapsedProperty) != value)
                {
                    if (value)
                    {
                        //((DoubleAnimation)AnimationStoryboard.Children[0]).From = SidebarWidth;
                        ((DoubleAnimation)AnimationStoryboard.Children[0]).To = 100;
                    }
                    else
                    {
                        ((DoubleAnimation)AnimationStoryboard.Children[0]).To = SidebarWidth;
                        //((DoubleAnimation)AnimationStoryboard.Children[0]).From = 100;
                    }


                    if (!animating)
                    {
                        animating = true;
                        BeginStoryboardAsync(AnimationStoryboard).ContinueWith(x =>
                        {
                            Dispatcher.BeginInvoke(new Action(() =>
                                {
                                    animating = false;

                                    SetValue(IsCollapsedProperty, value);

                                    if (CollapsedStateChanged != null)
                                        CollapsedStateChanged(this, new TabBarCollapsedStateChangedEventArgs(value));
                                }));
                        });
                    }

                }
            }
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

            AnimationStoryboard = this.Template.Resources["AnimationStoryboard"] as Storyboard;
            var x = AnimationStoryboard.Clone();
            x.SetValue(Storyboard.TargetNameProperty, null);
            x.SetValue(Storyboard.TargetProperty, GetTemplateChild("SideBarGrid"));
            AnimationStoryboard = x;

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

        public delegate void TabBarCollapsedStateChangedHandler(object sender, TabBarCollapsedStateChangedEventArgs e);
        public event TabBarCollapsedStateChangedHandler CollapsedStateChanged;

        internal MetroContentControl PART_Presenter = null;
        private ListBox PART_TabItems = null;
        private volatile bool animating = false;
        private Storyboard AnimationStoryboard = null;

        private System.Threading.Tasks.Task BeginStoryboardAsync(Storyboard sb)
        {
            System.Threading.Tasks.TaskCompletionSource<object> tcs = new System.Threading.Tasks.TaskCompletionSource<object>();

            EventHandler eh = null;
            eh = new EventHandler((o, e) =>
                 {
                     sb.Completed -= eh;
                     tcs.TrySetResult(null);
                 });

            sb.Completed += eh;

            this.BeginStoryboard(sb);

            return tcs.Task;
        }
    }
    public class TabBarCollapsedStateChangedEventArgs : EventArgs
    {
        internal TabBarCollapsedStateChangedEventArgs(bool state)
        {
            IsCollapsed = state;
        }
        public bool IsCollapsed { get; private set; }
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
