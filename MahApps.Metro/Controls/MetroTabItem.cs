using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MahApps.Metro.Controls
{
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
            if (CloseButton != null && closeButtonClickUnloaded)
            {
                CloseButton.Click += closeButton_Click;

                closeButtonClickUnloaded = false;
            }
        }

        void MetroTabItem_Unloaded(object sender, RoutedEventArgs e)
        {
            this.Unloaded -= MetroTabItem_Unloaded;
            if (CloseButton != null)
            {
                CloseButton.Click -= closeButton_Click;
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

        public bool CloseButtonEnabled
        {
            get { return (bool)GetValue(CloseButtonEnabledProperty); }
            set { SetValue(CloseButtonEnabledProperty, value); }
        }

        public static readonly DependencyProperty CloseButtonEnabledProperty =
            DependencyProperty.Register("CloseButtonEnabled", typeof(bool), typeof(MetroTabItem), new PropertyMetadata(false));

        internal Button closeButton = null;
        public Button CloseButton
        {
            get { return this.closeButton ?? (this.closeButton = GetTemplateChild("PART_CloseButton") as Button); }
        }

        internal Label rootLabel = null;
        public Label Label
        {
            get { return this.rootLabel ?? (this.rootLabel = GetTemplateChild("root") as Label); }
        }

        internal Thickness newButtonMargin;
        private bool closeButtonClickUnloaded = false;

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (CloseButton != null)
            {
                CloseButton.Margin = newButtonMargin;

                //TabControl's multi-loading/unloading issue
                CloseButton.Click -= closeButton_Click;
                CloseButton.Click += closeButton_Click;

                CloseButton.Visibility = CloseButtonEnabled ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
            }
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

        public ICommand CloseTabCommand { get { return (ICommand)GetValue(CloseTabCommandProperty); } set { SetValue(CloseTabCommandProperty, value); } }
        public static readonly DependencyProperty CloseTabCommandProperty = DependencyProperty.Register("CloseTabCommand", typeof(ICommand), typeof(MetroTabItem));

        public object CloseTabCommandParameter { get { return GetValue(CloseTabCommandParameterProperty); } set { SetValue(CloseTabCommandParameterProperty, value); } }
        public static readonly DependencyProperty CloseTabCommandParameterProperty =
            DependencyProperty.Register("CloseTabCommandParameter", typeof(object), typeof(MetroTabItem), new PropertyMetadata(null));

        public BaseMetroTabControl OwningTabControl { get; internal set; }

        protected override void OnSelected(RoutedEventArgs e)
        {
            if (CloseButton != null)
                if (CloseButtonEnabled)
                    CloseButton.Visibility = Visibility.Visible;

            base.OnSelected(e);
        }

        protected override void OnUnselected(RoutedEventArgs e)
        {
            if (CloseButton != null)
                CloseButton.Visibility = Visibility.Hidden;

            base.OnUnselected(e);
        }

        protected override void OnMouseEnter(MouseEventArgs e)
        {
            if (CloseButton != null)
                if (CloseButtonEnabled)
                    CloseButton.Visibility = Visibility.Visible;

            base.OnMouseEnter(e);
        }

        protected override void OnMouseLeave(MouseEventArgs e)
        {
            if (!this.IsSelected && CloseButton != null && CloseButtonEnabled)
                CloseButton.Visibility = Visibility.Hidden;

            base.OnMouseLeave(e);
        }
    }
}
