using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MahApps.Metro.Controls
{
    /// <summary>
    /// An extended TabItem with a metro style.
    /// </summary>
    public class MetroTabItem : TabItem
    {
        public MetroTabItem()
        {
            DefaultStyleKey = typeof(MetroTabItem);
            this.Unloaded += MetroTabItem_Unloaded;
            this.Loaded += MetroTabItem_Loaded;
        }
        
         public MetroTabItem(BaseMetroTabControl OwningTabControl)
        {
            DefaultStyleKey = typeof(MetroTabItem);
            this.Unloaded += MetroTabItem_Unloaded;
            this.Loaded += MetroTabItem_Loaded;
            this.OwningTabControl = OwningTabControl;
        }


        void MetroTabItem_Loaded(object sender, RoutedEventArgs e)
        {
            if (closeButton != null && closeButtonClickUnloaded)
            {
                closeButton.Click += closeButton_Click;

                closeButtonClickUnloaded = false;
            }
        }

        void MetroTabItem_Unloaded(object sender, RoutedEventArgs e)
        {
            this.Unloaded -= MetroTabItem_Unloaded;
            if (closeButton != null)
            {
                closeButton.Click -= closeButton_Click;
            }

            closeButtonClickUnloaded = true;
        }

        private delegate void EmptyDelegate();
        ~MetroTabItem()
        {
            if (Application.Current != null)
            {
                Application.Current.Dispatcher.Invoke(new EmptyDelegate(() =>
                {
                    this.Loaded -= MetroTabItem_Loaded;
                }));
            }
        }

        /// <summary>
        /// Gets/sets whether the Close Button is visible.
        /// </summary>
        public bool CloseButtonEnabled
        {
            get { return (bool)GetValue(CloseButtonEnabledProperty); }
            set { SetValue(CloseButtonEnabledProperty, value); }
        }

        public static readonly DependencyProperty CloseButtonEnabledProperty =
            DependencyProperty.Register("CloseButtonEnabled", typeof(bool), typeof(MetroTabItem), new PropertyMetadata(false, OnCloseButtonEnabledPropertyChangedCallback));

        private static void OnCloseButtonEnabledPropertyChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            var item = dependencyObject as MetroTabItem;
            if (item != null)
            {
                if (item.closeButton == null)
                {
                    item.ApplyTemplate();
                }

                if (item.closeButton != null)
                {
                    item.closeButton.Visibility = (bool)e.NewValue ? Visibility.Hidden : Visibility.Collapsed;
                }
            }
        }

        internal Button closeButton = null;
        internal Thickness newButtonMargin;
        internal ContentPresenter contentSite = null;
        private bool closeButtonClickUnloaded = false;

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            closeButton = GetTemplateChild("PART_CloseButton") as Button;
            if (closeButton != null)
            {
                closeButton.Margin = newButtonMargin;

                //TabControl's multi-loading/unloading issue
                closeButton.Click -= closeButton_Click;
                closeButton.Click += closeButton_Click;

                closeButton.Visibility = CloseButtonEnabled ? Visibility.Hidden : Visibility.Collapsed;
            }

            contentSite = GetTemplateChild("ContentSite") as ContentPresenter;
        }

        void closeButton_Click(object sender, RoutedEventArgs e)
        {
            //Binding RelativeSource={RelativeSource FindAncestor, AncestorType={x:Type TabControl}}, Path=InternalCloseTabCommand
            // Click event fires BEFORE the command does so we have time to set and handle the event before hand.

            if (CloseTabCommand != null)
            {
                // force the command handler to run
                CloseTabCommand.Execute(CloseTabCommandParameter);
                // cheat and dereference the handler now
                CloseTabCommand = null;
                CloseTabCommandParameter = null;
            }

            if (OwningTabControl == null) // see #555
                throw new InvalidOperationException();

            // run the command handler for the TabControl
            var itemFromContainer = OwningTabControl.ItemContainerGenerator.ItemFromContainer(this);

            var data = itemFromContainer == DependencyProperty.UnsetValue ? this.Content : itemFromContainer;
            OwningTabControl.InternalCloseTabCommand.Execute(new Tuple<object, MetroTabItem>(data, this));
        }

        /// <summary>
        /// Gets/sets the command that is executed when the Close Button is clicked.
        /// </summary>
        public ICommand CloseTabCommand { get { return (ICommand)GetValue(CloseTabCommandProperty); } set { SetValue(CloseTabCommandProperty, value); } }
        public static readonly DependencyProperty CloseTabCommandProperty = DependencyProperty.Register("CloseTabCommand", typeof(ICommand), typeof(MetroTabItem));

        public object CloseTabCommandParameter { get { return GetValue(CloseTabCommandParameterProperty); } set { SetValue(CloseTabCommandParameterProperty, value); } }
        public static readonly DependencyProperty CloseTabCommandParameterProperty =
            DependencyProperty.Register("CloseTabCommandParameter", typeof(object), typeof(MetroTabItem), new PropertyMetadata(null));

        /// <summary>
        /// Gets the owning MetroTabControl of this MetroTabItem.
        /// </summary>
        public BaseMetroTabControl OwningTabControl { get; internal set; }

        protected override void OnSelected(RoutedEventArgs e)
        {
            if (closeButton != null && CloseButtonEnabled)
                closeButton.Visibility = Visibility.Visible;

            base.OnSelected(e);
        }

        protected override void OnUnselected(RoutedEventArgs e)
        {
            if (closeButton != null && CloseButtonEnabled)
                closeButton.Visibility = Visibility.Hidden;

            base.OnUnselected(e);
        }

        protected override void OnMouseEnter(MouseEventArgs e)
        {
            if (closeButton != null)
                if (CloseButtonEnabled)
                    closeButton.Visibility = Visibility.Visible;

            base.OnMouseEnter(e);
        }

        protected override void OnMouseLeave(MouseEventArgs e)
        {
            if (!this.IsSelected && closeButton != null && CloseButtonEnabled)
                closeButton.Visibility = Visibility.Hidden;

            base.OnMouseLeave(e);
        }
    }
}