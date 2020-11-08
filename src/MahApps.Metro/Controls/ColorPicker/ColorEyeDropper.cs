// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace MahApps.Metro.Controls
{
    [TemplatePart(Name = nameof(PART_PreviewToolTip), Type = typeof(ToolTip))]
    [TemplatePart(Name = nameof(PART_PreviewImage), Type = typeof(Image))]
    public class ColorEyeDropper : ContentControl
    {
        static ColorEyeDropper()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ColorEyeDropper), new FrameworkPropertyMetadata(typeof(ColorEyeDropper)));
        }

        private ToolTip PART_PreviewToolTip;
        private Image PART_PreviewImage;
        private DispatcherOperation currentTask;

        // Depency Properties
        /// <summary>Identifies the <see cref="SelectedColor"/> dependency property.</summary>
        public static readonly DependencyProperty SelectedColorProperty = DependencyProperty.Register(nameof(SelectedColor), typeof(Color?), typeof(ColorEyeDropper), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        /// <summary>Identifies the <see cref="PreviewImageOuterPixelCount"/> dependency property.</summary>
        public static readonly DependencyProperty PreviewImageOuterPixelCountProperty = DependencyProperty.Register(nameof(PreviewImageOuterPixelCount), typeof(int), typeof(ColorEyeDropper), new PropertyMetadata(2));

        /// <summary>Identifies the <see cref="EyeDropperCursor"/> dependency property.</summary>
        public static readonly DependencyProperty EyeDropperCursorProperty = DependencyProperty.Register(nameof(EyeDropperCursor), typeof(Cursor), typeof(ColorEyeDropper), new PropertyMetadata(null));

        /// <summary>Identifies the <see cref="PreviewBrush"/> dependency property.</summary>
        public static readonly DependencyProperty PreviewBrushProperty = DependencyProperty.Register(nameof(PreviewBrush), typeof(Brush), typeof(ColorEyeDropper), new PropertyMetadata(Brushes.Transparent));

        /// <summary>
        /// Gets the preview image while the cursor is moving
        /// </summary>
        public Color? SelectedColor
        {
            get => (Color?)this.GetValue(SelectedColorProperty);
            set => this.SetValue(SelectedColorProperty, value);
        }

        /// <summary>
        /// Gets the preview brush while the cursor is moving
        /// </summary>
        public Brush PreviewBrush => (Brush)this.GetValue(PreviewBrushProperty);

        /// <summary>
        /// Gets or Sets the number of additional pixel in the preview image
        /// </summary>
        public int PreviewImageOuterPixelCount
        {
            get => (int)this.GetValue(PreviewImageOuterPixelCountProperty);
            set => this.SetValue(PreviewImageOuterPixelCountProperty, value);
        }

        /// <summary>
        /// Gets or Sets the Cursor in Selecting Color Mode
        /// </summary>
        public Cursor EyeDropperCursor
        {
            get => (Cursor)this.GetValue(EyeDropperCursorProperty);
            set => this.SetValue(EyeDropperCursorProperty, value);
        }

        private void SetPreview(Point mousePos)
        {
            if (this.currentTask?.Status == DispatcherOperationStatus.Executing || this.currentTask?.Status == DispatcherOperationStatus.Pending)
            {
                this.currentTask.Abort();
            }

            var action = new Action(() =>
                {
                    var outerPixelCount = this.PreviewImageOuterPixelCount;
                    var posX = (int)Math.Round(mousePos.X - outerPixelCount);
                    var posY = (int)Math.Round(mousePos.Y - outerPixelCount);
                    var region = new Int32Rect(posX, posY, 2 * outerPixelCount + 1, 2 * outerPixelCount + 1);
                    var previewImage = EyeDropperHelper.CaptureRegion(region);
                    var previewBrush = new SolidColorBrush(EyeDropperHelper.GetPixelColor(mousePos));
                    previewBrush.Freeze();

                    this.PART_PreviewImage.Source = previewImage;
                    this.SetCurrentValue(PreviewBrushProperty, previewBrush);
                });

            this.currentTask = this.Dispatcher.BeginInvoke(DispatcherPriority.Background, action);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            this.PART_PreviewToolTip = this.GetTemplateChild(nameof(this.PART_PreviewToolTip)) as ToolTip;
            this.PART_PreviewImage = this.GetTemplateChild(nameof(this.PART_PreviewImage)) as Image;
        }

        protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnPreviewMouseLeftButtonDown(e);

            Mouse.Capture(this);

            if (!(this.PART_PreviewToolTip is null))
            {
                this.PART_PreviewToolTip.Visibility = Visibility.Visible;
                this.PART_PreviewToolTip.IsOpen = true;
            }

            this.Cursor = this.EyeDropperCursor;

            var mousePos = this.PointToScreen(e.GetPosition(this));
            this.SetPreview(mousePos);
        }

        protected override void OnPreviewMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            base.OnPreviewMouseLeftButtonUp(e);

            Mouse.Capture(null);

            if (!(this.PART_PreviewToolTip is null))
            {
                this.PART_PreviewToolTip.IsOpen = false;
                this.PART_PreviewToolTip.Visibility = Visibility.Collapsed;
            }

            this.Cursor = Cursors.Arrow;

            if (!this.IsMouseOver)
            {
                var mousePos = this.PointToScreen(e.GetPosition(this));
                this.SelectedColor = EyeDropperHelper.GetPixelColor(mousePos);
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (e.LeftButton == MouseButtonState.Pressed)
            {
                var mousePos = e.GetPosition(this.PART_PreviewToolTip.PlacementTarget);

                this.PART_PreviewToolTip.Placement = PlacementMode.Relative;
                this.PART_PreviewToolTip.HorizontalOffset = mousePos.X + 16;
                this.PART_PreviewToolTip.VerticalOffset = mousePos.Y + 16;

                this.SetPreview(this.PART_PreviewToolTip.PlacementTarget.PointToScreen(mousePos));
            }
        }
    }
}