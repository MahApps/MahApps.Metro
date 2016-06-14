using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace MahApps.Metro.Controls
{
    public class Tile : Button
    {
        static Tile()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Tile), new FrameworkPropertyMetadata(typeof(Tile)));
        }

        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register("Title", typeof(string), typeof(Tile), new PropertyMetadata(default(string)));

        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        /// <summary> 
        /// HorizontalTitleAlignment Dependency Property.
        /// Default Value: HorizontalAlignment.Left
        /// </summary>
        public static readonly DependencyProperty HorizontalTitleAlignmentProperty =
            DependencyProperty.Register(
                "HorizontalTitleAlignment",
                typeof(HorizontalAlignment),
                typeof(Tile),
                new FrameworkPropertyMetadata(HorizontalAlignment.Left));

        /// <summary> 
        /// Gets/Sets the horizontal alignment of the title.
        /// </summary> 
        [Bindable(true), Category("Layout")]
        public HorizontalAlignment HorizontalTitleAlignment
        {
            get { return (HorizontalAlignment)GetValue(HorizontalTitleAlignmentProperty); }
            set { SetValue(HorizontalTitleAlignmentProperty, value); }
        }

        /// <summary>
        /// VerticalTitleAlignment Dependency Property. 
        /// Default Value: VerticalAlignment.Bottom
        /// </summary> 
        public static readonly DependencyProperty VerticalTitleAlignmentProperty =
            DependencyProperty.Register(
                "VerticalTitleAlignment",
                typeof(VerticalAlignment),
                typeof(Tile),
                new FrameworkPropertyMetadata(VerticalAlignment.Bottom));

        /// <summary>
        /// Gets/Sets the vertical alignment of the title.
        /// </summary>
        [Bindable(true), Category("Layout")]
        public VerticalAlignment VerticalTitleAlignment
        {
            get { return (VerticalAlignment)GetValue(VerticalTitleAlignmentProperty); }
            set { SetValue(VerticalTitleAlignmentProperty, value); }
        }

        public static readonly DependencyProperty CountProperty = DependencyProperty.Register("Count", typeof(string), typeof(Tile), new PropertyMetadata(default(string)));

        public string Count
        {
            get { return (string)GetValue(CountProperty); }
            set { SetValue(CountProperty, value); }
        }

        public static readonly DependencyProperty KeepDraggingProperty = DependencyProperty.Register("KeepDragging", typeof(bool), typeof(Tile), new PropertyMetadata(true));

        public bool KeepDragging
        {
            get { return (bool)GetValue(KeepDraggingProperty); }
            set { SetValue(KeepDraggingProperty, value); }
        }

        public static readonly DependencyProperty TiltFactorProperty = DependencyProperty.Register("TiltFactor", typeof(int), typeof(Tile), new PropertyMetadata(5));

        public int TiltFactor
        {
            get { return (Int32)GetValue(TiltFactorProperty); }
            set { SetValue(TiltFactorProperty, value); }
        }

        public static readonly DependencyProperty TitleFontSizeProperty = DependencyProperty.Register("TitleFontSize", typeof(double), typeof(Tile), new PropertyMetadata(16d));

        public double TitleFontSize
        {
            get { return (double)GetValue(TitleFontSizeProperty); }
            set { SetValue(TitleFontSizeProperty, value); }
        }

        public static readonly DependencyProperty CountFontSizeProperty = DependencyProperty.Register("CountFontSize", typeof(double), typeof(Tile), new PropertyMetadata(28d));

        public double CountFontSize
        {
            get { return (double)GetValue(CountFontSizeProperty); }
            set { SetValue(CountFontSizeProperty, value); }
        }
    }
}