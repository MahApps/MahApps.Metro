using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace MahApps.Metro.Controls
{
    public class WidthPercentageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var percentage = Double.Parse(parameter.ToString(), new CultureInfo("en-US"));
            return ((double) value)*percentage;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public partial class ProgressIndicator
    {
        public ProgressIndicator()
        {
            InitializeComponent();
            this.DataContext = this;
            IsVisibleChanged += (s, e) => ((ProgressIndicator)s).StartStopAnimation();
            DependencyPropertyDescriptor dpd = DependencyPropertyDescriptor.FromProperty(VisibilityProperty, GetType());
            dpd.AddValueChanged(this, (s, e) => ((ProgressIndicator)s).StartStopAnimation());
        }

        public static readonly DependencyProperty ProgressColourProperty = DependencyProperty.RegisterAttached("ProgressColour", typeof(Brush), typeof(ProgressIndicator), new UIPropertyMetadata(null));

        public Brush ProgressColour
        {
            get { return (Brush)GetValue(ProgressColourProperty); }
            set { SetValue(ProgressColourProperty, value); }
        }

        private void StartStopAnimation()
        {
            bool shouldAnimate = (Visibility == Visibility.Visible && IsVisible);
            Dispatcher.BeginInvoke(new Action(() =>
            {
                var s = Resources["animate"] as Storyboard;
                if (s != null)
                {
                    if (shouldAnimate)
                        s.Begin();
                    else
                        s.Stop();
                }
            }));
        }
    }
}