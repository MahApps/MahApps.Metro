namespace MahApps.Metro.Controls
{
    using System;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Data;
    using System.Windows.Media;
    using System.Windows.Media.Animation;

    public class WidthPercentageConverter : IValueConverter
    {
        #region Public Methods and Operators

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var percentage = Double.Parse(parameter.ToString(), new CultureInfo("en-US"));
            return ((double)value) * percentage;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }

        #endregion
    }

    public partial class ProgressIndicator
    {
        #region Constants and Fields

        public static readonly DependencyProperty ProgressColourProperty =
            DependencyProperty.RegisterAttached(
                "ProgressColour", typeof(Brush), typeof(ProgressIndicator), new UIPropertyMetadata(null));

        #endregion

        #region Constructors and Destructors

        public ProgressIndicator()
        {
            this.InitializeComponent();
            this.DataContext = this;
        }

        #endregion

        #region Public Properties

        public Brush ProgressColour
        {
            get
            {
                return (Brush)this.GetValue(ProgressColourProperty);
            }
            set
            {
                this.SetValue(ProgressColourProperty, value);
            }
        }

        #endregion

        #region Public Methods and Operators

        public void Stop()
        {
            this.Dispatcher.BeginInvoke(
                new Action(
                    () =>
                    {
                        var s = this.Resources["animate"] as Storyboard;
                        s.Stop();
                        this.Visibility = Visibility.Collapsed;
                    }));
        }

        #endregion
    }
}