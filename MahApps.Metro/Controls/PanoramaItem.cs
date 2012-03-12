namespace MahApps.Metro.Controls
{
    using System.Windows;
    using System.Windows.Controls;

    public class PanoramaItem : ContentControl
    {
        #region Constants and Fields

        public static readonly DependencyProperty HeaderOpacityProperty = DependencyProperty.Register(
            "HeaderOpacity", typeof(double), typeof(PanoramaItem), new PropertyMetadata(1.0));

        public static readonly DependencyProperty HeaderProperty = DependencyProperty.Register(
            "Header", typeof(object), typeof(PanoramaItem), new PropertyMetadata(null));

        public static readonly DependencyProperty HeaderTemplateProperty = DependencyProperty.Register(
            "HeaderTemplate", typeof(DataTemplate), typeof(PanoramaItem), new PropertyMetadata(null));

        public static readonly DependencyProperty OrientationProperty = DependencyProperty.Register(
            "Orientation", typeof(Orientation), typeof(PanoramaItem), new PropertyMetadata(Orientation.Horizontal));

        #endregion

        #region Constructors and Destructors

        static PanoramaItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(
                typeof(PanoramaItem), new FrameworkPropertyMetadata(typeof(PanoramaItem)));
        }

        #endregion

        #region Public Properties

        public object Header
        {
            get
            {
                return this.GetValue(HeaderProperty);
            }
            set
            {
                this.SetValue(HeaderProperty, value);
            }
        }

        public double HeaderOpacity
        {
            get
            {
                return (double)this.GetValue(HeaderOpacityProperty);
            }
            set
            {
                this.SetValue(HeaderOpacityProperty, value);
            }
        }

        public DataTemplate HeaderTemplate
        {
            get
            {
                return (DataTemplate)this.GetValue(HeaderTemplateProperty);
            }
            set
            {
                this.SetValue(HeaderTemplateProperty, value);
            }
        }

        public Orientation Orientation
        {
            get
            {
                return (Orientation)this.GetValue(OrientationProperty);
            }
            set
            {
                this.SetValue(OrientationProperty, value);
            }
        }

        #endregion
    }
}