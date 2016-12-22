using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace MahApps.Metro.Controls
{
    [StyleTypedProperty(Property = "ItemContainerStyle", StyleTargetType = typeof(WindowCommands))]
    public class WindowCommands : ItemsControl, INotifyPropertyChanged
    {
        public static readonly DependencyProperty ThemeProperty =
            DependencyProperty.Register("Theme", typeof(Theme), typeof(WindowCommands),
                                        new PropertyMetadata(Theme.Light, OnThemeChanged));

        /// <summary>
        /// Gets or sets the value indicating current theme.
        /// </summary>
        public Theme Theme
        {
            get { return (Theme)GetValue(ThemeProperty); }
            set { SetValue(ThemeProperty, value); }
        }

        public static readonly DependencyProperty LightTemplateProperty =
            DependencyProperty.Register("LightTemplate", typeof(ControlTemplate), typeof(WindowCommands),
                                        new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the value indicating light theme template.
        /// </summary>
        public ControlTemplate LightTemplate
        {
            get { return (ControlTemplate)GetValue(LightTemplateProperty); }
            set { SetValue(LightTemplateProperty, value); }
        }

        public static readonly DependencyProperty DarkTemplateProperty =
            DependencyProperty.Register("DarkTemplate", typeof(ControlTemplate), typeof(WindowCommands),
                                        new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the value indicating light theme template.
        /// </summary>
        public ControlTemplate DarkTemplate
        {
            get { return (ControlTemplate)GetValue(DarkTemplateProperty); }
            set { SetValue(DarkTemplateProperty, value); }
        }

        public static readonly DependencyProperty ShowSeparatorsProperty =
            DependencyProperty.Register("ShowSeparators", typeof(bool), typeof(WindowCommands),
                                        new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender,
                                                                      OnShowSeparatorsChanged));

        /// <summary>
        /// Gets or sets the value indicating whether to show the separators.
        /// </summary>
        public bool ShowSeparators
        {
            get { return (bool)GetValue(ShowSeparatorsProperty); }
            set { SetValue(ShowSeparatorsProperty, value); }
        }

        public static readonly DependencyProperty ShowLastSeparatorProperty =
            DependencyProperty.Register("ShowLastSeparator", typeof(bool), typeof(WindowCommands),
                                        new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender,
                                                                      OnShowLastSeparatorChanged));
        
        /// <summary>
        /// Gets or sets the value indicating whether to show the last separator.
        /// </summary>
        public bool ShowLastSeparator
        {
            get { return (bool)GetValue(ShowLastSeparatorProperty); }
            set { SetValue(ShowLastSeparatorProperty, value); }
        }

        public static readonly DependencyProperty SeparatorHeightProperty =
            DependencyProperty.Register("SeparatorHeight", typeof(int), typeof(WindowCommands),
                                        new FrameworkPropertyMetadata(15, FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender));

        /// <summary>
        /// Gets or sets the value indicating separator height.
        /// </summary>
        public int SeparatorHeight
        {
            get { return (int)GetValue(SeparatorHeightProperty); }
            set { SetValue(SeparatorHeightProperty, value); }
        }

        private static void OnThemeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            // only apply theme if value is changed
            if (e.NewValue == e.OldValue)
            {
                return;
            }

            // get the object
            var obj = (WindowCommands)d;

            // apply control template
            if ((Theme)e.NewValue == Theme.Light)
            {
                if (obj.LightTemplate != null)
                {
                    obj.SetValue(TemplateProperty, obj.LightTemplate);
                }
            }
            else if ((Theme)e.NewValue == Theme.Dark)
            {
                if (obj.DarkTemplate != null)
                {
                    obj.SetValue(TemplateProperty, obj.DarkTemplate);
                }
            }
        }

        private static void OnShowSeparatorsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue == e.OldValue)
            {
                return;
            }
            ((WindowCommands)d).ResetSeparators();
        }

        private static void OnShowLastSeparatorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue == e.OldValue)
            {
                return;
            }
            ((WindowCommands)d).ResetSeparators(false);
        }

        static WindowCommands()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(WindowCommands), new FrameworkPropertyMetadata(typeof(WindowCommands)));
        }

        public WindowCommands()
        {
            this.Loaded += WindowCommands_Loaded;
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new WindowCommandsItem();
        }

        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is WindowCommandsItem;
        }

        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            base.PrepareContainerForItemOverride(element, item);

            this.AttachVisibilityHandler(element as WindowCommandsItem, item as UIElement);
            ResetSeparators();
        }

        protected override void ClearContainerForItemOverride(DependencyObject element, object item)
        {
            base.ClearContainerForItemOverride(element, item);

            this.DetachVisibilityHandler(element as WindowCommandsItem);
            ResetSeparators(false);
        }

        private void AttachVisibilityHandler(WindowCommandsItem container, UIElement item)
        {
            if (container != null)
            {
                if (null == item)
                {
                    // if item is not a UIElement then maybe the ItemsSource binds to a collection of objects
                    // and an ItemTemplate is set, so let's try to solve this
                    container.ApplyTemplate();
                    var content = container.ContentTemplate?.LoadContent() as UIElement;
                    if (null == content)
                    {
                        // no UIElement was found, so don't show this container
                        container.Visibility = Visibility.Collapsed;
                    }
                    return;
                }

                container.Visibility = item.Visibility;
                var isVisibilityNotifier = new PropertyChangeNotifier(item, UIElement.VisibilityProperty);
                isVisibilityNotifier.ValueChanged += VisibilityPropertyChanged;
                container.VisibilityPropertyChangeNotifier = isVisibilityNotifier;
            }
        }

        private void DetachVisibilityHandler(WindowCommandsItem container)
        {
            if (container != null)
            {
                container.VisibilityPropertyChangeNotifier = null;
            }
        }

        private void VisibilityPropertyChanged(object sender, EventArgs e)
        {
            var item = sender as UIElement;
            if (item != null)
            {
                var container = GetWindowCommandsItem(item);
                if (container != null)
                {
                    container.Visibility = item.Visibility;
                    ResetSeparators();
                }
            }
        }

        protected override void OnItemsChanged(NotifyCollectionChangedEventArgs e)
        {
            base.OnItemsChanged(e);
            ResetSeparators();
        }

        private void ResetSeparators(bool reset = true)
        {
            if (Items.Count == 0)
            {
                return;
            }

            var windowCommandsItems = this.GetWindowCommandsItems().ToList();

            if (reset)
            {
                foreach (var windowCommandsItem in windowCommandsItems)
                {
                    windowCommandsItem.IsSeparatorVisible = ShowSeparators;
                }
            }

            var lastContainer = windowCommandsItems.LastOrDefault(i => i.IsVisible);
            if (lastContainer != null)
            {
                lastContainer.IsSeparatorVisible = ShowSeparators && ShowLastSeparator;
            }
        }

        private WindowCommandsItem GetWindowCommandsItem(object item)
        {
            var windowCommandsItem = item as WindowCommandsItem;
            if (windowCommandsItem != null)
            {
                return windowCommandsItem;
            }
            return (WindowCommandsItem)this.ItemContainerGenerator.ContainerFromItem(item);
        }

        private IEnumerable<WindowCommandsItem> GetWindowCommandsItems()
        {
            return (from object item in (IEnumerable)this.Items select this.GetWindowCommandsItem(item)).Where(i => i != null);
        }

        private void WindowCommands_Loaded(object sender, RoutedEventArgs e)
        {
            this.Loaded -= WindowCommands_Loaded;
            var parentWindow = this.ParentWindow;
            if (null == parentWindow)
            {
                this.ParentWindow = this.TryFindParent<Window>();
            }
        }

        private Window _parentWindow;

        public Window ParentWindow
        {
            get { return _parentWindow; }
            set
            {
                if (Equals(_parentWindow, value))
                {
                    return;
                }
                _parentWindow = value;
                this.RaisePropertyChanged("ParentWindow");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void RaisePropertyChanged(string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    [TemplatePart(Name = PART_ContentPresenter, Type = typeof(UIElement))]
    [TemplatePart(Name = PART_Separator, Type = typeof(UIElement))]
    public class WindowCommandsItem : ContentControl
    {
        private const string PART_ContentPresenter = "PART_ContentPresenter";
        private const string PART_Separator = "PART_Separator";

        internal PropertyChangeNotifier VisibilityPropertyChangeNotifier { get; set; }

        public static readonly DependencyProperty IsSeparatorVisibleProperty =
            DependencyProperty.Register("IsSeparatorVisible", typeof(bool), typeof(WindowCommandsItem),
                                        new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.Inherits|FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender));

        /// <summary>
        /// Gets or sets the value indicating whether to show the separator.
        /// </summary>
        public bool IsSeparatorVisible
        {
            get { return (bool)GetValue(IsSeparatorVisibleProperty); }
            set { SetValue(IsSeparatorVisibleProperty, value); }
        }

        static WindowCommandsItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(WindowCommandsItem), new FrameworkPropertyMetadata(typeof(WindowCommandsItem)));
        }
    }
}
