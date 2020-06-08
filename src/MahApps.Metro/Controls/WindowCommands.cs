using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using ControlzEx;
using ControlzEx.Theming;

namespace MahApps.Metro.Controls
{
    [StyleTypedProperty(Property = "ItemContainerStyle", StyleTargetType = typeof(WindowCommands))]
    public class WindowCommands : ToolBar
    {
        /// <summary>Identifies the <see cref="Theme"/> dependency property.</summary>
        public static readonly DependencyProperty ThemeProperty
            = DependencyProperty.Register(nameof(Theme),
                                          typeof(string),
                                          typeof(WindowCommands),
                                          new PropertyMetadata(ThemeManager.BaseColorLight, OnThemePropertyChanged));

        private static void OnThemePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != e.OldValue && e.NewValue is string baseColor)
            {
                var windowCommands = (WindowCommands)d;

                switch (baseColor)
                {
                    case ThemeManager.BaseColorLightConst:
                    {
                        if (windowCommands.LightTemplate != null)
                        {
                            windowCommands.SetValue(TemplateProperty, windowCommands.LightTemplate);
                        }

                        break;
                    }
                    case ThemeManager.BaseColorDarkConst:
                    {
                        if (windowCommands.DarkTemplate != null)
                        {
                            windowCommands.SetValue(TemplateProperty, windowCommands.DarkTemplate);
                        }

                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the value indicating the current theme.
        /// </summary>
        public string Theme
        {
            get => (string)this.GetValue(ThemeProperty);
            set => this.SetValue(ThemeProperty, value);
        }

        /// <summary>Identifies the <see cref="LightTemplate"/> dependency property.</summary>
        public static readonly DependencyProperty LightTemplateProperty
            = DependencyProperty.Register(nameof(LightTemplate),
                                          typeof(ControlTemplate),
                                          typeof(WindowCommands),
                                          new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the value indicating the light theme ControlTemplate.
        /// </summary>
        public ControlTemplate LightTemplate
        {
            get => (ControlTemplate)this.GetValue(LightTemplateProperty);
            set => this.SetValue(LightTemplateProperty, value);
        }

        /// <summary>Identifies the <see cref="DarkTemplate"/> dependency property.</summary>
        public static readonly DependencyProperty DarkTemplateProperty
            = DependencyProperty.Register(nameof(DarkTemplate),
                                          typeof(ControlTemplate),
                                          typeof(WindowCommands),
                                          new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the value indicating the light theme ControlTemplate.
        /// </summary>
        public ControlTemplate DarkTemplate
        {
            get => (ControlTemplate)this.GetValue(DarkTemplateProperty);
            set => this.SetValue(DarkTemplateProperty, value);
        }

        /// <summary>Identifies the <see cref="ShowSeparators"/> dependency property.</summary>
        public static readonly DependencyProperty ShowSeparatorsProperty
            = DependencyProperty.Register(nameof(ShowSeparators),
                                          typeof(bool),
                                          typeof(WindowCommands),
                                          new FrameworkPropertyMetadata(true,
                                                                        FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender,
                                                                        OnShowSeparatorsPropertyChanged));

        private static void OnShowSeparatorsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != e.OldValue)
            {
                ((WindowCommands)d).ResetSeparators();
            }
        }

        /// <summary>
        /// Gets or sets the value indicating whether to show the separators or not.
        /// </summary>
        public bool ShowSeparators
        {
            get => (bool)this.GetValue(ShowSeparatorsProperty);
            set => this.SetValue(ShowSeparatorsProperty, value);
        }

        /// <summary>Identifies the <see cref="ShowLastSeparator"/> dependency property.</summary>
        public static readonly DependencyProperty ShowLastSeparatorProperty
            = DependencyProperty.Register(nameof(ShowLastSeparator),
                                          typeof(bool),
                                          typeof(WindowCommands),
                                          new FrameworkPropertyMetadata(true,
                                                                        FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender,
                                                                        OnShowLastSeparatorPropertyChanged));

        private static void OnShowLastSeparatorPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != e.OldValue)
            {
                ((WindowCommands)d).ResetSeparators(false);
            }
        }

        /// <summary>
        /// Gets or sets the value indicating whether to show the last separator or not.
        /// </summary>
        public bool ShowLastSeparator
        {
            get => (bool)this.GetValue(ShowLastSeparatorProperty);
            set => this.SetValue(ShowLastSeparatorProperty, value);
        }

        /// <summary>Identifies the <see cref="SeparatorHeight"/> dependency property.</summary>
        public static readonly DependencyProperty SeparatorHeightProperty
            = DependencyProperty.Register(nameof(SeparatorHeight),
                                          typeof(double),
                                          typeof(WindowCommands),
                                          new FrameworkPropertyMetadata(15d,
                                                                        FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender));

        /// <summary>
        /// Gets or sets the value indicating the height of the separators.
        /// </summary>
        [TypeConverter(typeof(LengthConverter))]
        public double SeparatorHeight
        {
            get => (double)this.GetValue(SeparatorHeightProperty);
            set => this.SetValue(SeparatorHeightProperty, value);
        }

        /// <summary>Identifies the <see cref="ParentWindow"/> dependency property.</summary>
        public static readonly DependencyPropertyKey ParentWindowPropertyKey =
            DependencyProperty.RegisterReadOnly(nameof(ParentWindow),
                                                typeof(Window),
                                                typeof(WindowCommands),
                                                new PropertyMetadata(null));

        /// <summary>Identifies the <see cref="ParentWindow"/> dependency property.</summary>
        public static readonly DependencyProperty ParentWindowProperty = ParentWindowPropertyKey.DependencyProperty;

        /// <summary>
        /// Gets the window.
        /// </summary>
        public Window ParentWindow
        {
            get => (Window)this.GetValue(ParentWindowProperty);
            protected set => this.SetValue(ParentWindowPropertyKey, value);
        }

        static WindowCommands()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(WindowCommands), new FrameworkPropertyMetadata(typeof(WindowCommands)));
        }

        public WindowCommands()
        {
            this.Loaded += this.WindowCommandsLoaded;
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

            if (!(element is WindowCommandsItem windowCommandsItem))
            {
                return;
            }

            if (!(item is FrameworkElement frameworkElement))
            {
                windowCommandsItem.ApplyTemplate();
                frameworkElement = windowCommandsItem.ContentTemplate?.LoadContent() as FrameworkElement;
            }

            frameworkElement?.SetBinding(ControlsHelper.ContentCharacterCasingProperty,
                                         new Binding { Source = this, Path = new PropertyPath(ControlsHelper.ContentCharacterCasingProperty) });

            this.AttachVisibilityHandler(windowCommandsItem, item as UIElement);
            this.ResetSeparators();
        }

        protected override void ClearContainerForItemOverride(DependencyObject element, object item)
        {
            base.ClearContainerForItemOverride(element, item);

            if (item is FrameworkElement frameworkElement)
            {
                BindingOperations.ClearBinding(frameworkElement, ControlsHelper.ContentCharacterCasingProperty);
            }

            this.DetachVisibilityHandler(element as WindowCommandsItem);
            this.ResetSeparators(false);
        }

        private void AttachVisibilityHandler(WindowCommandsItem container, UIElement item)
        {
            if (container == null)
            {
                return;
            }

            if (item is null)
            {
                // if item is not a UIElement then maybe the ItemsSource binds to a collection of objects
                // and an ItemTemplate is set, so let's try to solve this
                container.ApplyTemplate();
                if (!(container.ContentTemplate?.LoadContent() is UIElement))
                {
                    // no UIElement was found, so don't show this container
                    container.Visibility = Visibility.Collapsed;
                }

                return;
            }

            container.Visibility = item.Visibility;
            var isVisibilityNotifier = new PropertyChangeNotifier(item, VisibilityProperty);
            isVisibilityNotifier.ValueChanged += this.VisibilityPropertyChanged;
            container.VisibilityPropertyChangeNotifier = isVisibilityNotifier;
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
            if (sender is UIElement item)
            {
                var container = this.GetWindowCommandsItem(item);
                if (container != null)
                {
                    container.Visibility = item.Visibility;
                    this.ResetSeparators();
                }
            }
        }

        protected override void OnItemsChanged(NotifyCollectionChangedEventArgs e)
        {
            base.OnItemsChanged(e);

            this.ResetSeparators();
        }

        private void ResetSeparators(bool reset = true)
        {
            if (this.Items.Count == 0)
            {
                return;
            }

            var windowCommandsItems = this.GetWindowCommandsItems().ToList();

            if (reset)
            {
                foreach (var windowCommandsItem in windowCommandsItems)
                {
                    windowCommandsItem.IsSeparatorVisible = this.ShowSeparators;
                }
            }

            var lastContainer = windowCommandsItems.LastOrDefault(i => i.IsVisible);
            if (lastContainer != null)
            {
                lastContainer.IsSeparatorVisible = this.ShowSeparators && this.ShowLastSeparator;
            }
        }

        private WindowCommandsItem GetWindowCommandsItem(object item)
        {
            if (item is WindowCommandsItem windowCommandsItem)
            {
                return windowCommandsItem;
            }

            return (WindowCommandsItem)this.ItemContainerGenerator.ContainerFromItem(item);
        }

        private IEnumerable<WindowCommandsItem> GetWindowCommandsItems()
        {
            foreach (var item in this.Items)
            {
                var windowCommandsItem = this.GetWindowCommandsItem(item);
                if (windowCommandsItem != null)
                {
                    yield return windowCommandsItem;
                }
            }
        }

        private void WindowCommandsLoaded(object sender, RoutedEventArgs e)
        {
            this.Loaded -= this.WindowCommandsLoaded;

            var contentPresenter = this.TryFindParent<ContentPresenter>();
            if (contentPresenter != null)
            {
                this.SetCurrentValue(DockPanel.DockProperty, contentPresenter.GetValue(DockPanel.DockProperty));
            }

            if (null == this.ParentWindow)
            {
                var window = this.TryFindParent<Window>();
                this.SetValue(ParentWindowPropertyKey, window);
            }
        }
    }
}