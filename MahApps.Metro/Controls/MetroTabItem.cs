using System;
using System.Windows;
using System.Windows.Controls;

namespace MahApps.Metro.Controls
{
    public class MetroTabItem : TabItem
    {
        public MetroTabItem()
        {
            DefaultStyleKey = typeof(MetroTabItem);
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

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            closeButton = GetTemplateChild("PART_CloseButton") as Button;
            closeButton.Margin = newButtonMargin;

            rootLabel = GetTemplateChild("root") as Label;
        }

        public TabControl OwningTabControl { get; internal set; }
    }
}
