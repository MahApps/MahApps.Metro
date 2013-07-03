using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

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
                closeButton.Click -= closeButton_Click;

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

        public double HeaderFontSize
        {
            get { return (double)GetValue(HeaderFontSizeProperty); }
            set { SetValue(HeaderFontSizeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for HeaderSize.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HeaderFontSizeProperty =
            DependencyProperty.Register("HeaderFontSize", typeof(double), typeof(MetroTabItem), new PropertyMetadata(26.67, new PropertyChangedCallback((obj, args) =>
                {
                    MetroTabItem item = (MetroTabItem)obj;

                    if (item.closeButton == null)
                    {
                        item.ApplyTemplate();
                    }

                    double fontDpiSize = (double)args.NewValue;

                    double fontHeight = Math.Ceiling(fontDpiSize * item.rootLabel.FontFamily.LineSpacing);

                    var newMargin = (Math.Round(fontHeight) / 2.2) - (item.rootLabel.Padding.Top);

                    var previousMargin = item.closeButton.Margin;
                    item.newButtonMargin = new Thickness(previousMargin.Left, newMargin, previousMargin.Right, previousMargin.Bottom);
                    item.closeButton.Margin = item.newButtonMargin;

                    item.closeButton.UpdateLayout();

                })));

        public FontStretch HeaderFontStretch
        {
            get { return (FontStretch)GetValue(HeaderFontStretchProperty); }
            set { SetValue(HeaderFontStretchProperty, value); }
        }

        public static readonly DependencyProperty HeaderFontStretchProperty =
            DependencyProperty.Register("HeaderFontStretch", typeof(FontStretch), typeof(MetroTabItem), new PropertyMetadata(FontStretches.Normal));


        public FontWeight HeaderFontWeight
        {
            get { return (FontWeight)GetValue(HeaderFontWeightProperty); }
            set { SetValue(HeaderFontWeightProperty, value); }
        }

        public static readonly DependencyProperty HeaderFontWeightProperty =
            DependencyProperty.Register("HeaderFontWeight", typeof(FontWeight), typeof(MetroTabItem), new PropertyMetadata(FontWeights.Normal));

        public bool CloseButtonEnabled
        {
            get { return (bool)GetValue(CloseButtonEnabledProperty); }
            set { SetValue(CloseButtonEnabledProperty, value); }
        }

        public static readonly DependencyProperty CloseButtonEnabledProperty =
            DependencyProperty.Register("CloseButtonEnabled", typeof(bool), typeof(MetroTabItem), new PropertyMetadata(false));

        internal Button closeButton = null;
        internal Thickness newButtonMargin;
        internal Label rootLabel = null;
        private bool closeButtonClickUnloaded = false;

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            bool closeButtonNullBefore = closeButton == null; //TabControl's multi-loading/unloading issue

            closeButton = GetTemplateChild("PART_CloseButton") as Button;
            closeButton.Margin = newButtonMargin;

            if (closeButtonNullBefore)
                closeButton.Click += closeButton_Click;


            closeButton.Visibility = CloseButtonEnabled ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;

            rootLabel = GetTemplateChild("root") as Label;
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
            if (closeButton != null)
                if (CloseButtonEnabled)
                    closeButton.Visibility = System.Windows.Visibility.Visible;

            base.OnSelected(e);
        }

        protected override void OnUnselected(RoutedEventArgs e)
        {
            if (closeButton != null)
                closeButton.Visibility = System.Windows.Visibility.Collapsed;

            base.OnUnselected(e);
        }

        protected override void OnMouseEnter(System.Windows.Input.MouseEventArgs e)
        {
            if (closeButton != null)
                if (CloseButtonEnabled)
                    closeButton.Visibility = System.Windows.Visibility.Visible;

            base.OnMouseEnter(e);
        }

        protected override void OnMouseLeave(System.Windows.Input.MouseEventArgs e)
        {
            if (!this.IsSelected)
                if (closeButton != null)
                    if (CloseButtonEnabled)
                        closeButton.Visibility = System.Windows.Visibility.Collapsed;

            base.OnMouseLeave(e);
        }
    }
}
