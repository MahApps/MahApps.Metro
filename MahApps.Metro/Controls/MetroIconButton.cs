using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace MahApps.Metro.Controls
{
    public class MetroIconButton : Button
    {
        public static readonly DependencyProperty IconProperty =
            DependencyProperty.Register("Icon", typeof(Object), typeof(MetroIconButton));

        public static readonly DependencyProperty IconRenderTransformProperty =
            DependencyProperty.Register("IconRenderTransform", typeof(Transform), typeof(MetroIconButton));

        public static readonly DependencyProperty IconWidthProperty = DependencyProperty.Register(
            "IconWidth", typeof(Double), typeof(MetroIconButton), new PropertyMetadata(18.0));

        public static readonly DependencyProperty IconHeightProperty = DependencyProperty.Register(
            "IconHeight", typeof(Double), typeof(MetroIconButton), new PropertyMetadata(18.0));

        public static readonly DependencyProperty IconMarginProperty = DependencyProperty.Register(
            "IconMargin", typeof(Thickness), typeof(MetroIconButton), new PropertyMetadata(default(Thickness)));

        static MetroIconButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MetroIconButton),
                new FrameworkPropertyMetadata(typeof(MetroIconButton)));
        }

        public Transform IconRenderTransform
        {
            get { return (Transform)GetValue(IconRenderTransformProperty); }
            set { SetValue(IconRenderTransformProperty, value); }
        }

        public Object Icon
        {
            get { return GetValue(IconProperty); }
            set { SetValue(IconProperty, value); }
        }

        public Double IconWidth
        {
            get { return (Double)GetValue(IconWidthProperty); }
            set { SetValue(IconWidthProperty, value); }
        }

        public Double IconHeight
        {
            get { return (Double)GetValue(IconHeightProperty); }
            set { SetValue(IconHeightProperty, value); }
        }

        public Thickness IconMargin
        {
            get { return (Thickness)GetValue(IconMarginProperty); }
            set { SetValue(IconMarginProperty, value); }
        }
    }
}