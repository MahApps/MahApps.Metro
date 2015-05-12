using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MahApps.Metro.Controls;

namespace MetroDemo.ExampleViews
{
    /// <summary>
    /// Interaction logic for SliderProgressExamples.xaml
    /// </summary>
    public partial class SliderProgressExamples : UserControl
    {
        public SliderProgressExamples()
        {
            InitializeComponent();
        }

        private void RangeSlider_OnLowerValueChanged(object sender, RangeParameterChangedEventArgs e)
        {
            //MessageBox.Show(e.OldValue.ToString() + "->" + e.NewValue.ToString());
        }

        private void RangeSlider_OnUpperValueChanged(object sender, RangeParameterChangedEventArgs e)
        {
            //MessageBox.Show(e.OldValue.ToString() + "->" + e.NewValue.ToString());
        }

        private void RangeSlider_OnLowerThumbDragStarted(object sender, DragStartedEventArgs e)
        {
            //TestBlock.Text = "lower thumb drag started";
        }

        private void RangeSlider_OnLowerThumbDragCompleted(object sender, DragCompletedEventArgs e)
        {
            try
            {
                //RangeSlider1.MinRange = RangeSlider1.LowerValue;
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message + exception.StackTrace + exception.TargetSite);
            }
            //TestBlock.Text = "lower thumb drag completed";
        }

        private void RangeSlider_OnUpperThumbDragStarted(object sender, DragStartedEventArgs e)
        {
            //TestBlock.Text = "upper thumb drag started";
        }

        private void RangeSlider_OnUpperThumbDragCompleted(object sender, DragCompletedEventArgs e)
        {
            //TestBlock.Text = "upper thumb drag completed";
        }

        private void RangeSlider_OnCentralThumbDragStarted(object sender, DragStartedEventArgs e)
        {
            //TestBlock.Text = "central thumb drag started";
        }

        private void RangeSlider_OnCentralThumbDragCompleted(object sender, DragCompletedEventArgs e)
        {
            
            //TestBlock.Text = "central thumb drag completed";
        }

        private void RangeSlider_OnLowerThumbDragDelta(object sender, DragDeltaEventArgs e)
        {
        }

        private void RangeSlider_OnUpperThumbDragDelta(object sender, DragDeltaEventArgs e)
        {
        }

        private void RangeSlider_OnCentralThumbDragDelta(object sender, DragDeltaEventArgs e)
        {
            //TestBlock.Text = "central thumb drag delta";
        }
    }
}
