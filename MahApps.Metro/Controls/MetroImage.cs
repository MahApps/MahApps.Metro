namespace MahApps.Metro.Controls
{
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;

    public class MetroImage : Control
    {
        #region Constants and Fields

        public static readonly DependencyProperty SourceProperty = DependencyProperty.Register(
            "Source", typeof(Visual), typeof(MetroImage), new PropertyMetadata(default(Visual)));

        #endregion

        #region Constructors and Destructors

        public MetroImage()
        {
            this.DefaultStyleKey = typeof(MetroImage);
        }

        #endregion

        #region Public Properties

        public Visual Source
        {
            get
            {
                return (Visual)this.GetValue(SourceProperty);
            }
            set
            {
                this.SetValue(SourceProperty, value);
            }
        }

        #endregion
    }
}