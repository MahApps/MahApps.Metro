using System;
using System.Windows;
using System.Windows.Controls;
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
            this.Loaded -= MetroTabItem_Loaded;
            this.Unloaded -= MetroTabItem_Unloaded;
            closeButton.Click -= closeButton_Click;

            closeButtonClickUnloaded = true;
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


            closeButton.Visibility = CloseButtonEnabled ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;

            rootLabel = GetTemplateChild("root") as Label;
        }

        void closeButton_Click(object sender, RoutedEventArgs e)
        {
            //Binding RelativeSource={RelativeSource FindAncestor, AncestorType={x:Type TabControl}}, Path=InternalCloseTabCommand
            OwningTabControl.InternalCloseTabCommand.Execute(new Tuple<object, MetroTabItem>(this.Content, this));
        }

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
                closeButton.Visibility = System.Windows.Visibility.Hidden;

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
                        closeButton.Visibility = System.Windows.Visibility.Hidden;

            base.OnMouseLeave(e);
        }
    }
}
