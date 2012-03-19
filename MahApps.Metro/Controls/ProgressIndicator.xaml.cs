using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace MahApps.Metro.Controls
{
    public partial class ProgressIndicator
    {
        public ProgressIndicator()
        {
            InitializeComponent();
            this.DataContext = this;
            this.IsVisibleChanged += this.ToggleAnimation;
        }

        public static readonly DependencyProperty ProgressColourProperty = DependencyProperty.RegisterAttached("ProgressColour", typeof(Brush), typeof(ProgressIndicator), new UIPropertyMetadata());

        public Brush ProgressColour
        {
            get { return (Brush)GetValue(ProgressColourProperty); }
            set { SetValue(ProgressColourProperty, value); }
        }

        public void Stop()
        {
            this.Dispatcher.BeginInvoke(new Action(() =>
                                                  {
                                                      var s = this.Resources["animate"] as Storyboard;
                                                      s.Stop();
                                                      this.Visibility = Visibility.Collapsed;
                                                  })
                );
        }
        private void ToggleAnimation(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue)
            {
                this.Dispatcher.BeginInvoke(
                    new Action(
                        () =>
                            {
                                var s = this.Resources["animate"] as Storyboard;
                                s.Resume();
                            }));
            }
            else
            {
               Stop();
            }
        }
    }
}
