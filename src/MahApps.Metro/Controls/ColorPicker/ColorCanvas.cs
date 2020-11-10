// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MahApps.Metro.Controls
{
    [TemplatePart(Name = "PART_SaturationValueBox", Type = typeof(Control))]
    [TemplatePart(Name = "PART_ColorEyeDropper", Type = typeof(ColorEyeDropper))]
    public class ColorCanvas : ColorPickerBase
    {
        private FrameworkElement saturationValueBox;

        static ColorCanvas()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ColorCanvas), new FrameworkPropertyMetadata(typeof(ColorCanvas)));
        }

        private void PART_SaturationValueBox_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            this.saturationValueBox.ReleaseMouseCapture();
            this.saturationValueBox.MouseMove -= this.PART_SaturationValueBox_MouseMove;
        }

        private void PART_SaturationValueBox_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Mouse.Capture(this.saturationValueBox);
            this.saturationValueBox.MouseMove += this.PART_SaturationValueBox_MouseMove;

            this.UpdateValues(e.GetPosition(this.saturationValueBox));
        }

        private void PART_SaturationValueBox_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.UpdateValues(e.GetPosition(this.saturationValueBox));
            }
        }

        private void UpdateValues(Point position)
        {
            if (this.saturationValueBox.ActualWidth < 1 || this.saturationValueBox.ActualHeight < 1)
            {
                return;
            }

            var s = position.X / this.saturationValueBox.ActualWidth;
            var v = 1 - (position.Y / this.saturationValueBox.ActualHeight);

            if (s > 1)
            {
                s = 1;
            }

            if (v > 1)
            {
                v = 1;
            }

            if (s < 0)
            {
                s = 0;
            }

            if (v < 0)
            {
                v = 0;
            }

            this.SetCurrentValue(SaturationProperty, s);
            this.SetCurrentValue(ValueProperty, v);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            this.saturationValueBox = (FrameworkElement)this.GetTemplateChild("PART_SaturationValueBox");
            if (this.saturationValueBox != null)
            {
                this.saturationValueBox.MouseLeftButtonDown += this.PART_SaturationValueBox_MouseLeftButtonDown;
                this.saturationValueBox.MouseLeftButtonUp += this.PART_SaturationValueBox_MouseLeftButtonUp;
            }
        }
    }
}