using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace MahApps.Metro.Controls
{
    [TemplatePart(Name = "PART_SaturationValueBox", Type = typeof(Control))]
    [TemplatePart(Name = "PART_PickColorFromScreen", Type = typeof(ColorEyeDropper))]
    public class ColorCanvas : ColorPickerBase
    {
        static ColorCanvas()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ColorCanvas), new FrameworkPropertyMetadata(typeof(ColorCanvas)));
        }

        #region private Members
        FrameworkElement PART_SaturationValueBox;
        #endregion

        private void PART_SaturationValueBox_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var action = new Action(() =>
            {
                this.PART_SaturationValueBox.ReleaseMouseCapture();
                this.PART_SaturationValueBox.MouseMove -= this.PART_SaturationValueBox_MouseMove;
            });
            Dispatcher.BeginInvoke(DispatcherPriority.Background, action);
        }

        private void PART_SaturationValueBox_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var action = new Action(() =>
            {
                Mouse.Capture(this.PART_SaturationValueBox);
                this.PART_SaturationValueBox.MouseMove += this.PART_SaturationValueBox_MouseMove;

                this.PART_SaturationValueBox_UpdateValues(e.GetPosition(this.PART_SaturationValueBox));
            });
            Dispatcher.BeginInvoke(DispatcherPriority.Background, action);
        }

        private void PART_SaturationValueBox_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.PART_SaturationValueBox_UpdateValues(e.GetPosition(this.PART_SaturationValueBox));
            }
        }

        private void PART_SaturationValueBox_UpdateValues(Point position)
        {
            var action = new Action(() =>
            {
                if (this.PART_SaturationValueBox.ActualWidth < 1 || this.PART_SaturationValueBox.ActualHeight < 1)
                    return;

                var s = position.X / this.PART_SaturationValueBox.ActualWidth;
                var v = 1 - (position.Y / this.PART_SaturationValueBox.ActualHeight);

                if (s > 1) s = 1;
                if (v > 1) v = 1;

                if (s < 0) s = 0;
                if (v < 0) v = 0;

                this.SetCurrentValue(SaturationProperty, s);
                this.SetCurrentValue(ValueProperty, v);
            });
            Dispatcher.BeginInvoke(DispatcherPriority.Background, action);
        }

        #region Overrides

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            this.PART_SaturationValueBox = (FrameworkElement)this.GetTemplateChild("PART_SaturationValueBox");
            this.PART_SaturationValueBox.MouseLeftButtonDown += this.PART_SaturationValueBox_MouseLeftButtonDown;
            this.PART_SaturationValueBox.MouseLeftButtonUp += this.PART_SaturationValueBox_MouseLeftButtonUp;
        }
        #endregion
    }
}