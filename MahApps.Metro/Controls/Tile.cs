namespace MahApps.Metro.Controls
{
    using System;
    using System.Windows;
    using System.Windows.Controls;

    public class Tile : Button
    {
        #region Constants and Fields

        public static readonly DependencyProperty CountProperty = DependencyProperty.Register(
            "Count", typeof(string), typeof(Tile), new PropertyMetadata(default(string)));

        public static readonly DependencyProperty KeepDraggingProperty = DependencyProperty.Register(
            "KeepDragging", typeof(bool), typeof(Tile), new PropertyMetadata(true));

        public static readonly DependencyProperty TiltFactorProperty = DependencyProperty.Register(
            "TiltFactor", typeof(int), typeof(Tile), new PropertyMetadata(5));

        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(
            "Title", typeof(string), typeof(Tile), new PropertyMetadata(default(string)));

        #endregion

        #region Constructors and Destructors

        public Tile()
        {
            this.DefaultStyleKey = typeof(Tile);
        }

        #endregion

        #region Public Properties

        public string Count
        {
            get
            {
                return (string)this.GetValue(CountProperty);
            }
            set
            {
                this.SetValue(CountProperty, value);
            }
        }

        public bool KeepDragging
        {
            get
            {
                return (bool)this.GetValue(KeepDraggingProperty);
            }
            set
            {
                this.SetValue(KeepDraggingProperty, value);
            }
        }

        public int TiltFactor
        {
            get
            {
                return (Int32)this.GetValue(TiltFactorProperty);
            }
            set
            {
                this.SetValue(TiltFactorProperty, value);
            }
        }

        public string Title
        {
            get
            {
                return (string)this.GetValue(TitleProperty);
            }
            set
            {
                this.SetValue(TitleProperty, value);
            }
        }

        #endregion
    }
}