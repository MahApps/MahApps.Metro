using System;
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

        public static readonly DependencyProperty TitleFontSizeProperty = DependencyProperty.Register("TitleFontSize", typeof(int), typeof(Tile), new PropertyMetadata(16));

        public int TitleFontSize
        {
            get { return (int)GetValue(TitleFontSizeProperty); }
            set { SetValue(TitleFontSizeProperty, value); }
        }

        public static readonly DependencyProperty CountFontSizeProperty = DependencyProperty.Register("CountFontSize", typeof(int), typeof(Tile), new PropertyMetadata(28));

        public int CountFontSize
        {
            get { return (int)GetValue(CountFontSizeProperty); }
            set { SetValue(CountFontSizeProperty, value); }
        }
    }
}
