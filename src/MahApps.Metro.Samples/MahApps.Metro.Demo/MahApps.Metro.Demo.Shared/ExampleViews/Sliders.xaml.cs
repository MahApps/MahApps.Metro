using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using MahApps.Metro.Controls;

namespace MetroDemo.ExampleViews
{
    /// <summary>
    /// Interaction logic for Sliders.xaml
    /// </summary>
    public partial class Sliders : UserControl
    {
        public Sliders()
        {
            InitializeComponent();
        }

        private void RangeSlider_OnLowerValueChanged(object sender, RangeParameterChangedEventArgs e)
        {
            Trace.TraceInformation($"LowerValueChanged: {e.ParameterType}, {e.OldValue} -> {e.OldValue}");
        }

        private void RangeSlider_OnLowerThumbDragStarted(object sender, DragStartedEventArgs e)
        {
            Trace.TraceInformation("LowerThumbDragStarted");
        }

        private void RangeSlider_OnLowerThumbDragDelta(object sender, DragDeltaEventArgs e)
        {
            Trace.TraceInformation($"LowerThumbDragDelta: h={e.HorizontalChange}, v={e.VerticalChange}");
        }

        private void RangeSlider_OnLowerThumbDragCompleted(object sender, DragCompletedEventArgs e)
        {
            Trace.TraceInformation("LowerThumbDragCompleted");
        }

        private void RangeSlider_OnUpperValueChanged(object sender, RangeParameterChangedEventArgs e)
        {
            Trace.TraceInformation($"UpperValueChanged: {e.ParameterType}, {e.OldValue} -> {e.OldValue}");
        }

        private void RangeSlider_OnUpperThumbDragStarted(object sender, DragStartedEventArgs e)
        {
            Trace.TraceInformation("UpperThumbDragStarted");
        }

        private void RangeSlider_OnUpperThumbDragDelta(object sender, DragDeltaEventArgs e)
        {
            Trace.TraceInformation($"LowerThumbDragDelta: h={e.HorizontalChange}, v={e.VerticalChange}");
        }

        private void RangeSlider_OnUpperThumbDragCompleted(object sender, DragCompletedEventArgs e)
        {
            Trace.TraceInformation("UpperThumbDragCompleted");
        }

        private void RangeSlider_OnCentralThumbDragStarted(object sender, DragStartedEventArgs e)
        {
            Trace.TraceInformation("CentralThumbDragStarted");
        }

        private void RangeSlider_OnCentralThumbDragDelta(object sender, DragDeltaEventArgs e)
        {
            Trace.TraceInformation($"CentralThumbDragDelta: h={e.HorizontalChange}, v={e.VerticalChange}");
        }

        private void RangeSlider_OnCentralThumbDragCompleted(object sender, DragCompletedEventArgs e)
        {
            Trace.TraceInformation("CentralThumbDragCompleted");
        }
    }
}